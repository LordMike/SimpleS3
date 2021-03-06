﻿using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Requests.Buckets.Properties;

namespace Genbox.SimpleS3.Core.Requests.Buckets
{
    /// <summary>
    /// Create a bucket. By creating the bucket, you become the bucket owner. By default, the bucket is created in the US East (N. Virginia) region.
    /// You can optionally specify a region in the request body. You might choose a region to optimize latency, minimize costs, or address regulatory
    /// requirements.
    /// </summary>
    public class PutBucketRequest : BaseRequest, IAclProperties
    {
        public PutBucketRequest(string bucketName) : base(HttpMethod.PUT, bucketName, string.Empty)
        {
            AclGrantRead = new AclBuilder();
            AclGrantWrite = new AclBuilder();
            AclGrantReadAcp = new AclBuilder();
            AclGrantWriteAcp = new AclBuilder();
            AclGrantFullControl = new AclBuilder();
        }

        /// <summary>Enable object locking on the bucket.</summary>
        public bool EnableObjectLocking { get; set; }

        /// <summary>The region where you wish to create the bucket. If not set, it defaults to us-east-1 (US East, N. Virginia).</summary>
        public AwsRegion Region { get; set; }

        /// <inheritdoc />
        public BucketCannedAcl Acl { get; set; }

        /// <inheritdoc />
        public AclBuilder AclGrantRead { get; }

        /// <inheritdoc />
        public AclBuilder AclGrantWrite { get; }

        /// <inheritdoc />
        public AclBuilder AclGrantReadAcp { get; }

        /// <inheritdoc />
        public AclBuilder AclGrantWriteAcp { get; }

        /// <inheritdoc />
        public AclBuilder AclGrantFullControl { get; }
    }
}