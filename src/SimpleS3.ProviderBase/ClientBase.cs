﻿using System;
using System.Net;
using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.ProviderBase
{
    /// <summary>This class provides a convenient way to access all the functionality related to the S3 service, buckets and objects at the same time.</summary>
    public abstract class ClientBase : SimpleS3Client, IDisposable
    {
        private ServiceProvider? _serviceProvider;

        protected internal ClientBase(IInputValidator inputValidator, IUrlBuilder urlBuilder, Config config, IWebProxy? proxy = null)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton(inputValidator);
            services.AddSingleton(urlBuilder);
            services.AddSingleton(Options.Create(config));

            ICoreBuilder builder = SimpleS3CoreServices.AddSimpleS3Core(services);

            IHttpClientBuilder httpBuilder = builder.UseHttpClientFactory();

            if (proxy != null)
                httpBuilder.UseProxy(proxy);

            Build(services);
        }

        protected internal ClientBase(IInputValidator inputValidator, IUrlBuilder urlBuilder, Config config, INetworkDriver networkDriver)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton(inputValidator);
            services.AddSingleton(urlBuilder);
            SimpleS3CoreServices.AddSimpleS3Core(services);

            services.AddSingleton(networkDriver);
            services.AddSingleton(Options.Create(config));

            Build(services);
        }

        protected internal ClientBase(IObjectClient objectClient, IBucketClient bucketClient, IMultipartClient multipartClient, IMultipartTransfer multipartTransfer, ITransfer transfer)
        {
            Initialize(objectClient, bucketClient, multipartClient, multipartTransfer, transfer);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Build(IServiceCollection services)
        {
            _serviceProvider = services.BuildServiceProvider();

            IObjectClient objectClient = _serviceProvider.GetRequiredService<IObjectClient>();
            IBucketClient bucketClient = _serviceProvider.GetRequiredService<IBucketClient>();
            IMultipartClient multipartClient = _serviceProvider.GetRequiredService<IMultipartClient>();
            IMultipartTransfer multipartTransfer = _serviceProvider.GetRequiredService<IMultipartTransfer>();
            ITransfer transfer = _serviceProvider.GetRequiredService<ITransfer>();

            Initialize(objectClient, bucketClient, multipartClient, multipartTransfer, transfer);
        }

        protected virtual void Dispose(bool disposing)
        {
            _serviceProvider?.Dispose();
        }
    }
}