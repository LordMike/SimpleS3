﻿using System.Collections.Generic;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects
{
    public class ListObjectVersionsResponse : BaseResponse, IHasTruncated, IHasTruncatedExt
    {
        public string KeyMarker { get; internal set; }
        public string VersionIdMarker { get; internal set; }
        public string NextKeyMarker { get; internal set; }
        public string NextVersionIdMarker { get; internal set; }
        public IList<S3Version> Versions { get; internal set; }
        public IList<S3DeleteMarker> DeleteMarkers { get; internal set; }
        public string Name { get; internal set; }
        public int MaxKeys { get; internal set; }
        public bool IsTruncated { get; internal set; }
        public EncodingType EncodingType { get; internal set; }
        public string? Prefix { get; internal set; }
        public string? Delimiter { get; internal set; }
        public IList<string> CommonPrefixes { get; internal set; }
    }
}