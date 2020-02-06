﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Common;

namespace Genbox.SimpleS3.Core.Tests.Code.Other
{
    /// <summary>
    /// HTTP handler that delays all request, except each N requests
    /// </summary>
    internal class SlowHttpHandler : BaseFailingHttpHandler
    {
        private readonly int _successRate;
        private readonly TimeSpan _delay;

        public SlowHttpHandler(int successRate, TimeSpan delay)
        {
            Validator.RequireThat(successRate >= 1, nameof(successRate), "successRate must be greater than or equal 1");

            _successRate = successRate;
            _delay = delay;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await ConsumeRequestAsync(request).ConfigureAwait(false);

            if (++RequestCounter % _successRate == 0)
            {
                // Success
                return CreateResponse(request, HttpStatusCode.OK);
            }

            // Delayed
            await Task.Delay(_delay, cancellationToken).ConfigureAwait(false);

            // TODO: Return timeouts?
            return CreateResponse(request, HttpStatusCode.OK);
        }
    }
}