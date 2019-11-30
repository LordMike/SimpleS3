﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Clients
{
    [PublicAPI]
    public interface IS3ObjectClient
    {
        IObjectOperations ObjectOperations { get; }

        /// <summary>Delete an object</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest> config = null, CancellationToken token = default);

        /// <summary>Delete multiple objects</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKeys">A list of object keys</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest> config = null, CancellationToken token = default);

        /// <summary>Head an object. Can be used to check if an object exists without downloading the content of it.</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest> config = null, CancellationToken token = default);

        /// <summary>Get (download) an object</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest> config = null, CancellationToken token = default);

        /// <summary>Put (upload) an object</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="data">The content of the object</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream data, Action<PutObjectRequest> config = null, CancellationToken token = default);

        /// <summary>List all objects within a bucket</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest> config = null, CancellationToken token = default);

        /// <summary>Restores an object from Amazon Glacier</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<RestoreObjectResponse> RestoreObjectAsync(string bucketName, string objectKey, Action<RestoreObjectRequest> config = null, CancellationToken token = default);

        /// <summary>Copies an object already present in an S3 bucket to a new object in the same or another S3 bucket</summary>
        /// <param name="sourceBucketName">The source bucket</param>
        /// <param name="sourceObjectKey">The source object key</param>
        /// <param name="destinationBucket">The destination bucket</param>
        /// <param name="destinationObjectKey">The destination object key</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<CopyObjectResponse> CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest> config = null, CancellationToken token = default);
    }
}