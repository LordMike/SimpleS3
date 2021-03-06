﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Requests.Buckets;
using Genbox.SimpleS3.Core.Requests.Objects;
using Genbox.SimpleS3.Core.Requests.Objects.Types;
using Genbox.SimpleS3.Core.Requests.Service;
using Genbox.SimpleS3.Core.Responses.Buckets;
using Genbox.SimpleS3.Core.Responses.Objects;
using Genbox.SimpleS3.Core.Responses.Service;
using Genbox.SimpleS3.Extensions.HttpClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;

namespace Genbox.SimpleS3
{
    /// <summary>This class provides a convenient way to access all the functionality related to the S3 service, buckets and objects at the same time.</summary>
    public sealed class S3Client : IDisposable, IS3ServiceClient, IS3BucketClient, IS3ObjectClient
    {
        private readonly IS3BucketClient _bucketClient;
        private readonly IS3ObjectClient _objectClient;
        private readonly ServiceProvider _provider;
        private readonly IS3ServiceClient _serviceClient;

        /// <summary>Creates a new instance of <see cref="S3Client" /></summary>
        /// <param name="keyId">The key id</param>
        /// <param name="accessKey">The secret access key</param>
        /// <param name="region">The region you wish to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public S3Client(string keyId, byte[] accessKey, AwsRegion region, WebProxy proxy = null) : this(new S3Config(new AccessKey(keyId, accessKey), region), proxy)
        {
        }

        /// <summary>Creates a new instance of <see cref="S3Client" /></summary>
        /// <param name="keyId">The key id</param>
        /// <param name="accessKey">The secret access key</param>
        /// <param name="region">The region you wish to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public S3Client(string keyId, string accessKey, AwsRegion region, WebProxy proxy = null) : this(new S3Config(new StringAccessKey(keyId, accessKey), region), proxy)
        {
        }

        /// <summary>Creates a new instance of <see cref="S3Client" /></summary>
        /// <param name="config">The configuration you want to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public S3Client(S3Config config, WebProxy proxy = null) : this(config, new HttpClientHandler {Proxy = proxy})
        {
        }

        public S3Client(S3Config config, HttpMessageHandler messageHandler)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton(x => Options.Create(config));

            IS3ClientBuilder builder = services.AddSimpleS3Core();
            IHttpClientBuilder httpBuilder = builder.UseHttpClientFactory();

            if (messageHandler != null)
                httpBuilder.ConfigurePrimaryHttpMessageHandler(x => messageHandler);

            httpBuilder.SetHandlerLifetime(TimeSpan.FromMinutes(5));

            Random random = new Random();

            // Policy is:
            // Retries: 3
            // Timeout: 2^attempt seconds (2, 4, 8 seconds) + -100 to 100 ms jitter
            httpBuilder.AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                + TimeSpan.FromMilliseconds(random.Next(-100, 100))));

            _provider = services.BuildServiceProvider();
            _objectClient = _provider.GetRequiredService<IS3ObjectClient>();
            _bucketClient = _provider.GetRequiredService<IS3BucketClient>();
            _serviceClient = _provider.GetRequiredService<IS3ServiceClient>();
        }

        public void Dispose()
        {
            _provider?.Dispose();
        }

        public Task<GetBucketResponse> GetBucketAsync(string bucketName, Action<GetBucketRequest> config = null, CancellationToken token = default)
        {
            return _bucketClient.GetBucketAsync(bucketName, config, token);
        }

        public Task<PutBucketResponse> PutBucketAsync(string bucketName, Action<PutBucketRequest> config = null, CancellationToken token = default)
        {
            return _bucketClient.PutBucketAsync(bucketName, config, token);
        }

        public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest> config = null, CancellationToken token = default)
        {
            return _bucketClient.DeleteBucketAsync(bucketName, config, token);
        }

        public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest> config = null, CancellationToken token = default)
        {
            return _bucketClient.ListMultipartUploadsAsync(bucketName, config, token);
        }

        public Task<DeleteBucketStatus> EmptyBucket(string bucketName, CancellationToken token = default)
        {
            return _bucketClient.EmptyBucket(bucketName, token);
        }

        public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string resource, Action<DeleteObjectRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.DeleteObjectAsync(bucketName, resource, config, token);
        }

        public Task<DeleteMultipleObjectsResponse> DeleteMultipleObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> resources, Action<DeleteMultipleObjectsRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.DeleteMultipleObjectsAsync(bucketName, resources, config, token);
        }

        public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string resource, Action<HeadObjectRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.HeadObjectAsync(bucketName, resource, config, token);
        }

        public Task<InitiateMultipartUploadResponse> InitiateMultipartUploadAsync(string bucketName, string resource, Action<InitiateMultipartUploadRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.InitiateMultipartUploadAsync(bucketName, resource, config, token);
        }

        public Task<UploadPartResponse> UploadPartAsync(string bucketName, string resource, int partNumber, string uploadId, Stream content, Action<UploadPartRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.UploadPartAsync(bucketName, resource, partNumber, uploadId, content, config, token);
        }

        public Task<ListPartsResponse> ListPartsAsync(string bucketName, string resource, string uploadId, Action<ListPartsRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.ListPartsAsync(bucketName, resource, uploadId, config, token);
        }

        public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string resource, string uploadId, IEnumerable<UploadPartResponse> parts, Action<CompleteMultipartUploadRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.CompleteMultipartUploadAsync(bucketName, resource, uploadId, parts, config, token);
        }

        public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string resource, string uploadId, Action<AbortMultipartUploadRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.AbortMultipartUploadAsync(bucketName, resource, uploadId, config, token);
        }

        public Task<GetObjectResponse> GetObjectAsync(string bucketName, string resource, Action<GetObjectRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.GetObjectAsync(bucketName, resource, config, token);
        }

        public Task<PutObjectResponse> PutObjectAsync(string bucketName, string resource, Stream data, Action<PutObjectRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.PutObjectAsync(bucketName, resource, data, config, token);
        }

        public Task<MultipartUploadStatus> MultipartUploadAsync(string bucketName, string resource, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<InitiateMultipartUploadRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.MultipartUploadAsync(bucketName, resource, data, partSize, numParallelParts, config, token);
        }

        public Task<MultipartDownloadStatus> MultipartDownloadAsync(string bucketName, string resource, Stream output, int numParallelParts = 4, int bufferSize = 16777216, CancellationToken token = default)
        {
            return _objectClient.MultipartDownloadAsync(bucketName, resource, output, numParallelParts, bufferSize, token);
        }

        public Task<GetServiceResponse> GetServiceAsync(Action<GetServiceRequest> config = null, CancellationToken token = default)
        {
            return _serviceClient.GetServiceAsync(config, token);
        }
    }
}