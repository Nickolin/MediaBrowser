﻿using MediaBrowser.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace MediaBrowser.Model.Session
{
    [DebuggerDisplay("Client = {Client}, Username = {UserName}")]
    public class SessionInfoDto : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance can seek.
        /// </summary>
        /// <value><c>true</c> if this instance can seek; otherwise, <c>false</c>.</value>
        public bool CanSeek { get; set; }

        /// <summary>
        /// Gets or sets the supported commands.
        /// </summary>
        /// <value>The supported commands.</value>
        public List<string> SupportedCommands { get; set; }
        
        /// <summary>
        /// Gets or sets the remote end point.
        /// </summary>
        /// <value>The remote end point.</value>
        public string RemoteEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the queueable media types.
        /// </summary>
        /// <value>The queueable media types.</value>
        public List<string> QueueableMediaTypes { get; set; }

        /// <summary>
        /// Gets or sets the playable media types.
        /// </summary>
        /// <value>The playable media types.</value>
        public List<string> PlayableMediaTypes { get; set; }
        
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>The user id.</value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user primary image tag.
        /// </summary>
        /// <value>The user primary image tag.</value>
        public Guid? UserPrimaryImageTag { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the additional users present.
        /// </summary>
        /// <value>The additional users present.</value>
        public List<SessionUserInfo> AdditionalUsers { get; set; }

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>The application version.</value>
        public string ApplicationVersion { get; set; }
        
        /// <summary>
        /// Gets or sets the type of the client.
        /// </summary>
        /// <value>The type of the client.</value>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets the last activity date.
        /// </summary>
        /// <value>The last activity date.</value>
        public DateTime LastActivityDate { get; set; }

        /// <summary>
        /// Gets or sets the now viewing context.
        /// </summary>
        /// <value>The now viewing context.</value>
        public string NowViewingContext { get; set; }

        /// <summary>
        /// Gets or sets the now viewing item.
        /// </summary>
        /// <value>The now viewing item.</value>
        public BaseItemInfo NowViewingItem { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the device.
        /// </summary>
        /// <value>The name of the device.</value>
        public string DeviceName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is paused.
        /// </summary>
        /// <value><c>true</c> if this instance is paused; otherwise, <c>false</c>.</value>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is muted.
        /// </summary>
        /// <value><c>true</c> if this instance is muted; otherwise, <c>false</c>.</value>
        public bool IsMuted { get; set; }
        
        /// <summary>
        /// Gets or sets the now playing item.
        /// </summary>
        /// <value>The now playing item.</value>
        public BaseItemInfo NowPlayingItem { get; set; }

        /// <summary>
        /// Gets or sets the now playing position ticks.
        /// </summary>
        /// <value>The now playing position ticks.</value>
        public long? NowPlayingPositionTicks { get; set; }

        /// <summary>
        /// Gets or sets the device id.
        /// </summary>
        /// <value>The device id.</value>
        public string DeviceId { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether [supports remote control].
        /// </summary>
        /// <value><c>true</c> if [supports remote control]; otherwise, <c>false</c>.</value>
        public bool SupportsRemoteControl { get; set; }

        public PlayerStateInfo PlayState { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public SessionInfoDto()
        {
            AdditionalUsers = new List<SessionUserInfo>();

            PlayableMediaTypes = new List<string>();
            QueueableMediaTypes = new List<string>();
            SupportedCommands = new List<string>();
        }
    }

    /// <summary>
    /// Class SessionUserInfo.
    /// </summary>
    public class SessionUserInfo
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        public string UserId { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }
    }

    public class ClientCapabilities
    {
        public List<string> PlayableMediaTypes { get; set; }
        public List<string> SupportedCommands { get; set; }

        public ClientCapabilities()
        {
            PlayableMediaTypes = new List<string>();
            SupportedCommands = new List<string>();
        }
    }

    public class PlayerStateInfo
    {
        /// <summary>
        /// Gets or sets the now playing position ticks.
        /// </summary>
        /// <value>The now playing position ticks.</value>
        public long? PositionTicks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can seek.
        /// </summary>
        /// <value><c>true</c> if this instance can seek; otherwise, <c>false</c>.</value>
        public bool CanSeek { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is paused.
        /// </summary>
        /// <value><c>true</c> if this instance is paused; otherwise, <c>false</c>.</value>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is muted.
        /// </summary>
        /// <value><c>true</c> if this instance is muted; otherwise, <c>false</c>.</value>
        public bool IsMuted { get; set; }

        /// <summary>
        /// Gets or sets the volume level.
        /// </summary>
        /// <value>The volume level.</value>
        public int? VolumeLevel { get; set; }

        /// <summary>
        /// Gets or sets the index of the now playing audio stream.
        /// </summary>
        /// <value>The index of the now playing audio stream.</value>
        public int? AudioStreamIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the now playing subtitle stream.
        /// </summary>
        /// <value>The index of the now playing subtitle stream.</value>
        public int? SubtitleStreamIndex { get; set; }

        /// <summary>
        /// Gets or sets the now playing media version identifier.
        /// </summary>
        /// <value>The now playing media version identifier.</value>
        public string MediaSourceId { get; set; }

        /// <summary>
        /// Gets or sets the play method.
        /// </summary>
        /// <value>The play method.</value>
        public PlayMethod? PlayMethod { get; set; }
    }
}
