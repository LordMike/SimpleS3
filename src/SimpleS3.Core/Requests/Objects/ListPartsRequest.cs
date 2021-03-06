﻿using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Requests.Objects
{
    /// <summary>This operation lists the parts that have been uploaded for a specific multipart upload.</summary>
    public class ListPartsRequest : BaseRequest
    {
        public ListPartsRequest(string bucketName, string resource, string uploadId) : base(HttpMethod.GET, bucketName, resource)
        {
            UploadId = uploadId;
        }

        /// <summary>Requests Amazon S3 to encode the response and specifies the encoding method to use.</summary>
        public EncodingType EncodingType { get; set; }

        /// <summary>Upload ID identifying the multipart upload whose parts are being listed.</summary>
        public string UploadId { get; }

        /// <summary>Sets the maximum number of parts to return in the response body.</summary>
        public int? MaxParts { get; set; }

        /// <summary>Specifies the part after which listing should begin. Only parts with higher part numbers will be listed.</summary>
        public string PartNumberMarker { get; set; }
    }
}