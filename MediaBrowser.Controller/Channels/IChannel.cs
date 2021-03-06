﻿using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MediaBrowser.Controller.Channels
{
    public interface IChannel
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the home page URL.
        /// </summary>
        /// <value>The home page URL.</value>
        string HomePageUrl { get; }

        /// <summary>
        /// Gets the capabilities.
        /// </summary>
        /// <returns>ChannelCapabilities.</returns>
        ChannelCapabilities GetCapabilities();

        /// <summary>
        /// Determines whether [is enabled for] [the specified user].
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns><c>true</c> if [is enabled for] [the specified user]; otherwise, <c>false</c>.</returns>
        bool IsEnabledFor(User user);

        /// <summary>
        /// Searches the specified search term.
        /// </summary>
        /// <param name="searchInfo">The search information.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{IEnumerable{ChannelItemInfo}}.</returns>
        Task<IEnumerable<ChannelItemInfo>> Search(ChannelSearchInfo searchInfo, User user, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the channel items.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{IEnumerable{ChannelItem}}.</returns>
        Task<ChannelItemResult> GetChannelItems(InternalChannelItemQuery query, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the channel image.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{DynamicImageInfo}.</returns>
        Task<DynamicImageResponse> GetChannelImage(ImageType type, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the supported channel images.
        /// </summary>
        /// <returns>IEnumerable{ImageType}.</returns>
        IEnumerable<ImageType> GetSupportedChannelImages();
    }

    public class ChannelCapabilities
    {
        public bool CanSearch { get; set; }
    }

    public class ChannelSearchInfo
    {
        public string SearchTerm { get; set; }
    }

    public class InternalChannelItemQuery
    {
        public string CategoryId { get; set; }

        public User User { get; set; }
    }

    public class ChannelItemResult
    {
        public List<ChannelItemInfo> Items { get; set; }

        public TimeSpan CacheLength { get; set; }
    }
}
