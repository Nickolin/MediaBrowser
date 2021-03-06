﻿using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Querying;
using ServiceStack;
using System.Collections.Generic;
using System.Linq;

namespace MediaBrowser.Api.Music
{
    [Route("/Songs/{Id}/InstantMix", "GET", Summary = "Creates an instant playlist based on a given song")]
    public class GetInstantMixFromSong : BaseGetSimilarItemsFromItem
    {
    }

    [Route("/Albums/{Id}/InstantMix", "GET", Summary = "Creates an instant playlist based on a given album")]
    public class GetInstantMixFromAlbum : BaseGetSimilarItemsFromItem
    {
    }

    [Route("/Artists/{Name}/InstantMix", "GET", Summary = "Creates an instant playlist based on a given artist")]
    public class GetInstantMixFromArtist : BaseGetSimilarItems
    {
        [ApiMember(Name = "Name", Description = "The artist name", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Name { get; set; }
    }

    [Route("/MusicGenres/{Name}/InstantMix", "GET", Summary = "Creates an instant playlist based on a music genre")]
    public class GetInstantMixFromMusicGenre : BaseGetSimilarItems
    {
        [ApiMember(Name = "Name", Description = "The genre name", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Name { get; set; }
    }

    public class InstantMixService : BaseApiService
    {
        private readonly IUserManager _userManager;

        private readonly IDtoService _dtoService;
        private readonly IMusicManager _musicManager;

        public InstantMixService(IUserManager userManager, IDtoService dtoService, IMusicManager musicManager)
        {
            _userManager = userManager;
            _dtoService = dtoService;
            _musicManager = musicManager;
        }

        public object Get(GetInstantMixFromSong request)
        {
            var item = (Audio)_dtoService.GetItemByDtoId(request.Id);

            var user = _userManager.GetUserById(request.UserId.Value);

            var items = _musicManager.GetInstantMixFromSong(item, user);

            return GetResult(items, user, request);
        }

        public object Get(GetInstantMixFromAlbum request)
        {
            var album = (MusicAlbum)_dtoService.GetItemByDtoId(request.Id);

            var user = _userManager.GetUserById(request.UserId.Value);

            var items = _musicManager.GetInstantMixFromAlbum(album, user);

            return GetResult(items, user, request);
        }

        public object Get(GetInstantMixFromMusicGenre request)
        {
            var user = _userManager.GetUserById(request.UserId.Value);

            var items = _musicManager.GetInstantMixFromGenres(new[] { request.Name }, user);

            return GetResult(items, user, request);
        }

        public object Get(GetInstantMixFromArtist request)
        {
            var user = _userManager.GetUserById(request.UserId.Value);

            var items = _musicManager.GetInstantMixFromArtist(request.Name, user);

            return GetResult(items, user, request);
        }

        private object GetResult(IEnumerable<Audio> items, User user, BaseGetSimilarItems request)
        {
            var fields = request.GetItemFields().ToList();

            var list = items.ToList();

            var result = new ItemsResult
            {
                TotalRecordCount = list.Count
            };

            var dtos = list.Take(request.Limit ?? list.Count)
                .Select(i => _dtoService.GetBaseItemDto(i, fields, user));

            result.Items = dtos.ToArray();

            return ToOptimizedResult(result);
        }

    }
}
