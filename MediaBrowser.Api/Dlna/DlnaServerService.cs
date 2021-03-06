﻿using MediaBrowser.Controller.Dlna;
using ServiceStack;
using ServiceStack.Text.Controller;
using ServiceStack.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediaBrowser.Api.Dlna
{
    [Route("/Dlna/{UuId}/description.xml", "GET", Summary = "Gets dlna server info")]
    [Route("/Dlna/{UuId}/description", "GET", Summary = "Gets dlna server info")]
    public class GetDescriptionXml
    {
        [ApiMember(Name = "UuId", Description = "Server UuId", IsRequired = false, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string UuId { get; set; }
    }

    [Route("/Dlna/contentdirectory/contentdirectory.xml", "GET", Summary = "Gets dlna content directory xml")]
    [Route("/Dlna/contentdirectory/contentdirectory", "GET", Summary = "Gets dlna content directory xml")]
    public class GetContentDirectory
    {
    }

    [Route("/Dlna/contentdirectory/{UuId}/control", "POST", Summary = "Processes a control request")]
    public class ProcessControlRequest : IRequiresRequestStream
    {
        [ApiMember(Name = "UuId", Description = "Server UuId", IsRequired = false, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string UuId { get; set; }

        public Stream RequestStream { get; set; }
    }

    [Route("/Dlna/contentdirectory/{UuId}/events", Summary = "Processes an event subscription request")]
    public class ProcessEventRequest
    {
        [ApiMember(Name = "UuId", Description = "Server UuId", IsRequired = false, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string UuId { get; set; }
    }

    [Route("/Dlna/icons/{Filename}", "GET", Summary = "Gets a server icon")]
    public class GetIcon
    {
        [ApiMember(Name = "Filename", Description = "The icon filename", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Filename { get; set; }
    }

    public class DlnaServerService : BaseApiService
    {
        private readonly IDlnaManager _dlnaManager;
        private readonly IContentDirectory _contentDirectory;
        private readonly IEventManager _eventManager;

        public DlnaServerService(IDlnaManager dlnaManager, IContentDirectory contentDirectory, IEventManager eventManager)
        {
            _dlnaManager = dlnaManager;
            _contentDirectory = contentDirectory;
            _eventManager = eventManager;
        }

        public object Get(GetDescriptionXml request)
        {
            var xml = _dlnaManager.GetServerDescriptionXml(GetRequestHeaders(), request.UuId);

            return ResultFactory.GetResult(xml, "text/xml");
        }

        public object Get(GetContentDirectory request)
        {
            var xml = _contentDirectory.GetContentDirectoryXml(GetRequestHeaders());

            return ResultFactory.GetResult(xml, "text/xml");
        }

        public object Post(ProcessControlRequest request)
        {
            var response = PostAsync(request).Result;

            return ResultFactory.GetResult(response.Xml, "text/xml");
        }

        private async Task<ControlResponse> PostAsync(ProcessControlRequest request)
        {
            var pathInfo = PathInfo.Parse(Request.PathInfo);
            var id = pathInfo.GetArgumentValue<string>(2);

            using (var reader = new StreamReader(request.RequestStream))
            {
                return _contentDirectory.ProcessControlRequest(new ControlRequest
                {
                    Headers = GetRequestHeaders(),
                    InputXml = await reader.ReadToEndAsync().ConfigureAwait(false),
                    TargetServerUuId = id
                });
            }
        }

        private IDictionary<string, string> GetRequestHeaders()
        {
            var headers = new Dictionary<string, string>();

            foreach (var key in Request.Headers.AllKeys)
            {
                headers[key] = Request.Headers[key];
            }

            return headers;
        }

        public object Get(GetIcon request)
        {
            using (var response = _dlnaManager.GetIcon(request.Filename))
            {
                using (var ms = new MemoryStream())
                {
                    response.Stream.CopyTo(ms);

                    ms.Position = 0;
                    var bytes = ms.ToArray();
                    return ResultFactory.GetResult(bytes, "image/" + response.Format.ToString().ToLower());
                }
            }
        }

        public object Any(ProcessEventRequest request)
        {
            var subscriptionId = GetHeader("SID");
            var notificationType = GetHeader("NT");
            var callback = GetHeader("CALLBACK");
            var timeoutString = GetHeader("TIMEOUT");

            var timeout = ParseTimeout(timeoutString);

            if (string.Equals(Request.Verb, "SUBSCRIBE", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(notificationType))
                {
                    return GetSubscriptionResponse(_eventManager.RenewEventSubscription(subscriptionId, timeout));
                }

                return GetSubscriptionResponse(_eventManager.CreateEventSubscription(notificationType, timeout, callback));
            }

            return GetSubscriptionResponse(_eventManager.CancelEventSubscription(subscriptionId));
        }

        private object GetSubscriptionResponse(EventSubscriptionResponse response)
        {
            return ResultFactory.GetResult(response.Content, response.ContentType, response.Headers);
        }

        private readonly CultureInfo _usCulture = new CultureInfo("en-US");
        private int? ParseTimeout(string header)
        {
            if (!string.IsNullOrEmpty(header))
            {
                // Starts with SECOND-
                header = header.Split('-').Last();

                int val;

                if (int.TryParse(header, NumberStyles.Any, _usCulture, out val))
                {
                    return val;
                }
            }

            return null;
        }
    }
}
