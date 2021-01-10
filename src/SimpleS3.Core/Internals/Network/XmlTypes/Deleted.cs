﻿using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Network.XmlTypes
{
    [UsedImplicitly]
    [XmlType]
    internal sealed class Deleted
    {
        public string Key { get; set; }
        public string VersionId { get; set; }
        public bool DeleteMarker { get; set; }
        public string DeleteMarkerVersionId { get; set; }
    }
}