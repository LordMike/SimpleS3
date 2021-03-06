﻿using FluentValidation;
using Genbox.SimpleS3.Core.Requests.Buckets;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests.Buckets
{
    public class PutBucketRequestValidator : BaseRequestValidator<PutBucketRequest>
    {
        public PutBucketRequestValidator(IOptions<S3Config> config) : base(config)
        {
            RuleFor(x => x.Resource).Empty();
        }
    }
}