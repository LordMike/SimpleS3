﻿using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests.Buckets
{
    public class CreateBucketRequestValidator : RequestWithoutObjectKeyBase<CreateBucketRequest>
    {
        public CreateBucketRequestValidator(IOptions<S3Config> config) : base(config)
        {
        }
    }
}