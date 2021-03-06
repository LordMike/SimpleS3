﻿using Genbox.SimpleS3.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Requests.Service
{
    /// <summary>This implementation of the GET operation returns a list of all buckets owned by the authenticated sender of the request.</summary>
    public class GetServiceRequest : BaseRequest
    {
        public GetServiceRequest() : base(HttpMethod.GET, string.Empty, string.Empty)
        {
        }
    }
}