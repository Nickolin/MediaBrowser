﻿using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Persistence;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Resolvers;
using MediaBrowser.Model.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MediaBrowser.Controller.Entities
{
    /// <summary>
    /// Class Video
    /// </summary>
    public class Video : BaseItem, IHasMediaStreams, IHasAspectRatio, IHasTags, ISupportsPlaceHolders
    {
        public bool IsMultiPart { get; set; }
        public bool HasLocalAlternateVersions { get; set; }
        public Guid? PrimaryVersionId { get; set; }

        public List<Guid> AdditionalPartIds { get; set; }
        public List<Guid> LocalAlternateVersionIds { get; set; }

        public string FormatName { get; set; }
        public long? Size { get; set; }
        public string Container { get; set; }

        public Video()
        {
            PlayableStreamFileNames = new List<string>();
            AdditionalPartIds = new List<Guid>();
            LocalAlternateVersionIds = new List<Guid>();
            Tags = new List<string>();
            SubtitleFiles = new List<string>();
            LinkedAlternateVersions = new List<LinkedChild>();
        }

        [IgnoreDataMember]
        public int MediaSourceCount
        {
            get
            {
                return LinkedAlternateVersions.Count + LocalAlternateVersionIds.Count + 1;
            }
        }

        public List<LinkedChild> LinkedAlternateVersions { get; set; }

        /// <summary>
        /// Gets the linked children.
        /// </summary>
        /// <returns>IEnumerable{BaseItem}.</returns>
        public IEnumerable<Video> GetAlternateVersions()
        {
            var filesWithinSameDirectory = LocalAlternateVersionIds
                .Select(i => LibraryManager.GetItemById(i))
                .Where(i => i != null)
                .OfType<Video>();

            return filesWithinSameDirectory.Concat(GetLinkedAlternateVersions())
                .OrderBy(i => i.SortName);
        }

        public IEnumerable<Video> GetLinkedAlternateVersions()
        {
            var linkedVersions = LinkedAlternateVersions
                .Select(GetLinkedChild)
                .Where(i => i != null)
                .OfType<Video>();

            return linkedVersions
                .OrderBy(i => i.SortName);
        }

        /// <summary>
        /// Gets the additional parts.
        /// </summary>
        /// <returns>IEnumerable{Video}.</returns>
        public IEnumerable<Video> GetAdditionalParts()
        {
            return AdditionalPartIds
                .Select(i => LibraryManager.GetItemById(i))
                .Where(i => i != null)
                .OfType<Video>()
                .OrderBy(i => i.SortName);
        }

        /// <summary>
        /// Gets or sets the subtitle paths.
        /// </summary>
        /// <value>The subtitle paths.</value>
        public List<string> SubtitleFiles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has subtitles.
        /// </summary>
        /// <value><c>true</c> if this instance has subtitles; otherwise, <c>false</c>.</value>
        public bool HasSubtitles { get; set; }

        public bool IsPlaceHolder { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        public List<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the video bit rate.
        /// </summary>
        /// <value>The video bit rate.</value>
        public int? VideoBitRate { get; set; }

        /// <summary>
        /// Gets or sets the default index of the video stream.
        /// </summary>
        /// <value>The default index of the video stream.</value>
        public int? DefaultVideoStreamIndex { get; set; }

        /// <summary>
        /// Gets or sets the type of the video.
        /// </summary>
        /// <value>The type of the video.</value>
        public VideoType VideoType { get; set; }

        /// <summary>
        /// Gets or sets the type of the iso.
        /// </summary>
        /// <value>The type of the iso.</value>
        public IsoType? IsoType { get; set; }

        /// <summary>
        /// Gets or sets the video3 D format.
        /// </summary>
        /// <value>The video3 D format.</value>
        public Video3DFormat? Video3DFormat { get; set; }

        /// <summary>
        /// If the video is a folder-rip, this will hold the file list for the largest playlist
        /// </summary>
        public List<string> PlayableStreamFileNames { get; set; }

        /// <summary>
        /// Gets the playable stream files.
        /// </summary>
        /// <returns>List{System.String}.</returns>
        public List<string> GetPlayableStreamFiles()
        {
            return GetPlayableStreamFiles(Path);
        }

        /// <summary>
        /// Gets or sets the aspect ratio.
        /// </summary>
        /// <value>The aspect ratio.</value>
        public string AspectRatio { get; set; }

        [IgnoreDataMember]
        public override string ContainingFolderPath
        {
            get
            {
                if (IsMultiPart)
                {
                    return System.IO.Path.GetDirectoryName(Path);
                }

                if (!IsPlaceHolder)
                {
                    if (VideoType == VideoType.BluRay || VideoType == VideoType.Dvd ||
                        VideoType == VideoType.HdDvd)
                    {
                        return Path;
                    }
                }

                return base.ContainingFolderPath;
            }
        }

        public string MainFeaturePlaylistName { get; set; }

        /// <summary>
        /// Gets the playable stream files.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <returns>List{System.String}.</returns>
        public List<string> GetPlayableStreamFiles(string rootPath)
        {
            var allFiles = Directory.EnumerateFiles(rootPath, "*", SearchOption.AllDirectories).ToList();

            return PlayableStreamFileNames.Select(name => allFiles.FirstOrDefault(f => string.Equals(System.IO.Path.GetFileName(f), name, StringComparison.OrdinalIgnoreCase)))
                .Where(f => !string.IsNullOrEmpty(f))
                .ToList();
        }

        /// <summary>
        /// Gets a value indicating whether [is3 D].
        /// </summary>
        /// <value><c>true</c> if [is3 D]; otherwise, <c>false</c>.</value>
        [IgnoreDataMember]
        public bool Is3D
        {
            get { return Video3DFormat.HasValue; }
        }

        public bool IsHD { get; set; }

        /// <summary>
        /// Gets the type of the media.
        /// </summary>
        /// <value>The type of the media.</value>
        public override string MediaType
        {
            get
            {
                return Model.Entities.MediaType.Video;
            }
        }

        protected override async Task<bool> RefreshedOwnedItems(MetadataRefreshOptions options, List<FileSystemInfo> fileSystemChildren, CancellationToken cancellationToken)
        {
            var hasChanges = await base.RefreshedOwnedItems(options, fileSystemChildren, cancellationToken).ConfigureAwait(false);

            // Must have a parent to have additional parts or alternate versions
            // In other words, it must be part of the Parent/Child tree
            // The additional parts won't have additional parts themselves
            if (LocationType == LocationType.FileSystem && Parent != null)
            {
                if (IsMultiPart)
                {
                    var additionalPartsChanged = await RefreshAdditionalParts(options, fileSystemChildren, cancellationToken).ConfigureAwait(false);

                    if (additionalPartsChanged)
                    {
                        hasChanges = true;
                    }
                }
                else
                {
                    RefreshLinkedAlternateVersions();

                    var additionalPartsChanged = await RefreshAlternateVersionsWithinSameDirectory(options, fileSystemChildren, cancellationToken).ConfigureAwait(false);

                    if (additionalPartsChanged)
                    {
                        hasChanges = true;
                    }
                }
            }

            return hasChanges;
        }

        private bool RefreshLinkedAlternateVersions()
        {
            foreach (var child in LinkedAlternateVersions)
            {
                // Reset the cached value
                if (child.ItemId.HasValue && child.ItemId.Value == Guid.Empty)
                {
                    child.ItemId = null;
                }
            }

            return false;
        }

        /// <summary>
        /// Refreshes the additional parts.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="fileSystemChildren">The file system children.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{System.Boolean}.</returns>
        private async Task<bool> RefreshAdditionalParts(MetadataRefreshOptions options, IEnumerable<FileSystemInfo> fileSystemChildren, CancellationToken cancellationToken)
        {
            var newItems = LoadAdditionalParts(fileSystemChildren, options.DirectoryService).ToList();

            var newItemIds = newItems.Select(i => i.Id).ToList();

            var itemsChanged = !AdditionalPartIds.SequenceEqual(newItemIds);

            var tasks = newItems.Select(i => i.RefreshMetadata(options, cancellationToken));

            await Task.WhenAll(tasks).ConfigureAwait(false);

            AdditionalPartIds = newItemIds;

            return itemsChanged;
        }

        /// <summary>
        /// Loads the additional parts.
        /// </summary>
        /// <returns>IEnumerable{Video}.</returns>
        private IEnumerable<Video> LoadAdditionalParts(IEnumerable<FileSystemInfo> fileSystemChildren, IDirectoryService directoryService)
        {
            IEnumerable<FileSystemInfo> files;

            var path = Path;

            if (VideoType == VideoType.BluRay || VideoType == VideoType.Dvd)
            {
                files = fileSystemChildren.Where(i =>
                {
                    if ((i.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        return !string.Equals(i.FullName, path, StringComparison.OrdinalIgnoreCase) && EntityResolutionHelper.IsMultiPartFolder(i.FullName) && EntityResolutionHelper.IsMultiPartFile(i.Name);
                    }

                    return false;
                });
            }
            else
            {
                files = fileSystemChildren.Where(i =>
                {
                    if ((i.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        return false;
                    }

                    return !string.Equals(i.FullName, path, StringComparison.OrdinalIgnoreCase) && EntityResolutionHelper.IsVideoFile(i.FullName) && EntityResolutionHelper.IsMultiPartFile(i.Name);
                });
            }

            return LibraryManager.ResolvePaths<Video>(files, directoryService, null).Select(video =>
            {
                // Try to retrieve it from the db. If we don't find it, use the resolved version
                var dbItem = LibraryManager.GetItemById(video.Id) as Video;

                if (dbItem != null)
                {
                    video = dbItem;
                }

                return video;

                // Sort them so that the list can be easily compared for changes
            }).OrderBy(i => i.Path).ToList();
        }

        private async Task<bool> RefreshAlternateVersionsWithinSameDirectory(MetadataRefreshOptions options, IEnumerable<FileSystemInfo> fileSystemChildren, CancellationToken cancellationToken)
        {
            var newItems = HasLocalAlternateVersions ?
                LoadAlternateVersionsWithinSameDirectory(fileSystemChildren, options.DirectoryService).ToList() :
                new List<Video>();

            var newItemIds = newItems.Select(i => i.Id).ToList();

            var itemsChanged = !LocalAlternateVersionIds.SequenceEqual(newItemIds);

            var tasks = newItems.Select(i => RefreshAlternateVersion(options, i, cancellationToken));

            await Task.WhenAll(tasks).ConfigureAwait(false);

            LocalAlternateVersionIds = newItemIds;

            return itemsChanged;
        }

        private Task RefreshAlternateVersion(MetadataRefreshOptions options, Video video, CancellationToken cancellationToken)
        {
            var currentImagePath = video.GetImagePath(ImageType.Primary);
            var ownerImagePath = this.GetImagePath(ImageType.Primary);

            var newOptions = new MetadataRefreshOptions
            {
                DirectoryService = options.DirectoryService,
                ImageRefreshMode = options.ImageRefreshMode,
                MetadataRefreshMode = options.MetadataRefreshMode,
                ReplaceAllMetadata = options.ReplaceAllMetadata
            };

            if (!string.Equals(currentImagePath, ownerImagePath, StringComparison.OrdinalIgnoreCase))
            {
                newOptions.ForceSave = true;

                if (string.IsNullOrWhiteSpace(ownerImagePath))
                {
                    video.ImageInfos.Clear();
                }
                else
                {
                    video.SetImagePath(ImageType.Primary, ownerImagePath);
                }
            }

            return video.RefreshMetadata(newOptions, cancellationToken);
        }

        public override async Task UpdateToRepository(ItemUpdateType updateReason, CancellationToken cancellationToken)
        {
            await base.UpdateToRepository(updateReason, cancellationToken).ConfigureAwait(false);

            foreach (var item in LocalAlternateVersionIds.Select(i => LibraryManager.GetItemById(i)))
            {
                item.ImageInfos = ImageInfos;
                item.Overview = Overview;
                item.ProductionYear = ProductionYear;
                item.PremiereDate = PremiereDate;
                item.CommunityRating = CommunityRating;
                item.OfficialRating = OfficialRating;
                item.Genres = Genres;
                item.ProviderIds = ProviderIds;

                await item.UpdateToRepository(ItemUpdateType.MetadataDownload, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Loads the additional parts.
        /// </summary>
        /// <returns>IEnumerable{Video}.</returns>
        private IEnumerable<Video> LoadAlternateVersionsWithinSameDirectory(IEnumerable<FileSystemInfo> fileSystemChildren, IDirectoryService directoryService)
        {
            IEnumerable<FileSystemInfo> files;

            // Only support this for video files. For folder rips, they'll have to use the linking feature
            if (VideoType == VideoType.VideoFile || VideoType == VideoType.Iso)
            {
                var path = Path;

                var filenamePrefix = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(path));

                files = fileSystemChildren.Where(i =>
                {
                    if ((i.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        return false;
                    }

                    return !string.Equals(i.FullName, path, StringComparison.OrdinalIgnoreCase) &&
                           EntityResolutionHelper.IsVideoFile(i.FullName) &&
                           i.Name.StartsWith(filenamePrefix + " - ", StringComparison.OrdinalIgnoreCase);
                });
            }
            else
            {
                files = new List<FileSystemInfo>();
            }

            return LibraryManager.ResolvePaths<Video>(files, directoryService, null).Select(video =>
            {
                // Try to retrieve it from the db. If we don't find it, use the resolved version
                var dbItem = LibraryManager.GetItemById(video.Id) as Video;

                if (dbItem != null)
                {
                    video = dbItem;
                }

                video.PrimaryVersionId = Id;

                return video;

                // Sort them so that the list can be easily compared for changes
            }).OrderBy(i => i.Path).ToList();
        }

        public override IEnumerable<string> GetDeletePaths()
        {
            if (!IsInMixedFolder)
            {
                return new[] { ContainingFolderPath };
            }

            return base.GetDeletePaths();
        }

        public virtual IEnumerable<MediaStream> GetMediaStreams()
        {
            return ItemRepository.GetMediaStreams(new MediaStreamQuery
            {
                ItemId = Id
            });
        }

        public virtual MediaStream GetDefaultVideoStream()
        {
            if (!DefaultVideoStreamIndex.HasValue)
            {
                return null;
            }

            return ItemRepository.GetMediaStreams(new MediaStreamQuery
            {
                ItemId = Id,
                Index = DefaultVideoStreamIndex.Value

            }).FirstOrDefault();
        }
    }
}
