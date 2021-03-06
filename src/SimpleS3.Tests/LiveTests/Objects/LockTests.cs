﻿using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class LockTests : LiveTestBase
    {
        public LockTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Theory]
        [InlineData(Core.Enums.LockMode.Compliance)]
        [InlineData(Core.Enums.LockMode.Governance)]
        public async Task LockMode(LockMode lockMode)
        {
            DateTimeOffset lockRetainUntil = DateTimeOffset.UtcNow.AddMinutes(1);

            //We add a unique guid to prevent contamination across runs
            string resource = $"{nameof(LockMode)}-{lockMode}-{Guid.NewGuid()}";

            await UploadAsync(resource, request =>
            {
                request.LockMode = lockMode;
                request.LockRetainUntil = lockRetainUntil;
            }).ConfigureAwait(false);

            GetObjectResponse resp = await AssertAsync(resource).ConfigureAwait(false);
            Assert.Equal(lockMode, resp.LockMode);
            Assert.Equal(lockRetainUntil.DateTime, resp.LockRetainUntilDate.DateTime, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [InlineData(Core.Enums.LockMode.Compliance)]
        [InlineData(Core.Enums.LockMode.Governance)]
        public async Task LockModeFluid(LockMode lockMode)
        {
            DateTimeOffset lockRetainUntil = DateTimeOffset.UtcNow.AddMinutes(1);

            //We add a unique guid to prevent contamination across runs
            string resource = $"{nameof(LockModeFluid)}-{lockMode}-{Guid.NewGuid()}";

            await UploadTransferAsync(resource, upload => upload.WithLock(lockMode, lockRetainUntil)).ConfigureAwait(false);

            GetObjectResponse resp = await AssertAsync(resource).ConfigureAwait(false);
            Assert.Equal(lockMode, resp.LockMode);
            Assert.Equal(lockRetainUntil.DateTime, resp.LockRetainUntilDate.DateTime, TimeSpan.FromSeconds(1));
        }
    }
}