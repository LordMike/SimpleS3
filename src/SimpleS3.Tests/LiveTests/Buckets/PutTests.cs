﻿using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Responses.Buckets;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Buckets
{
    public class PutTests : LiveTestBase
    {
        public PutTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task PutAndGet()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            PutBucketResponse pResp = await BucketClient.PutBucketAsync(tempBucketName, request =>
            {
                request.Region = Config.Region;
                request.Acl = BucketCannedAcl.Private;
            }).ConfigureAwait(false);
            Assert.True(pResp.IsSuccess);

            //We prefix with a number here to keep sort order when we download the list of objects further down
            string tempObjName = "object-1" + Guid.NewGuid();
            string tempObjName2 = "object-2" + Guid.NewGuid();

            await ObjectClient.PutObjectStringAsync(tempBucketName, tempObjName, "hello").ConfigureAwait(false);
            await ObjectClient.PutObjectStringAsync(tempBucketName, tempObjName2, "world!", null, request => request.StorageClass = StorageClass.OneZoneIa).ConfigureAwait(false);

            GetBucketResponse gResp = await BucketClient.GetBucketAsync(tempBucketName).ConfigureAwait(false);
            Assert.True(gResp.IsSuccess);

            Assert.Equal(2, gResp.KeyCount);
            Assert.Equal(2, gResp.Objects.Count);
            Assert.Equal(string.Empty, gResp.Prefix);

            Assert.Equal(tempObjName, gResp.Objects[0].Name);
            Assert.Equal(DateTime.UtcNow, gResp.Objects[0].LastModified.DateTime, TimeSpan.FromSeconds(5));
            Assert.Equal("\"5d41402abc4b2a76b9719d911017c592\"", gResp.Objects[0].ETag);
            Assert.Equal(5, gResp.Objects[0].Size);
            Assert.Equal(StorageClass.Standard, gResp.Objects[0].StorageClass);

            Assert.Equal(tempObjName2, gResp.Objects[1].Name);
            Assert.Equal(DateTime.UtcNow, gResp.Objects[1].LastModified.DateTime, TimeSpan.FromSeconds(5));
            Assert.Equal("\"08cf82251c975a5e9734699fadf5e9c0\"", gResp.Objects[1].ETag);
            Assert.Equal(6, gResp.Objects[1].Size);
            Assert.Equal(StorageClass.OneZoneIa, gResp.Objects[1].StorageClass);

            GetBucketResponse gResp2 = await BucketClient.GetBucketAsync(tempBucketName, request => request.EncodingType = EncodingType.Url).ConfigureAwait(false);
            Assert.True(gResp2.IsSuccess);
            Assert.Equal(2, gResp2.KeyCount);

            //The keys should be URL encoded at this point
            Assert.Equal(UrlHelper.UrlEncode(tempObjName), gResp.Objects[0].Name);
            Assert.Equal(UrlHelper.UrlEncode(tempObjName2), gResp.Objects[1].Name);
        }

        [Fact]
        public async Task PutWithAcl()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            PutBucketResponse pResp = await BucketClient.PutBucketAsync(tempBucketName, request =>
            {
                request.AclGrantReadAcp.AddEmail(TestConstants.TestEmail);
                request.AclGrantWriteAcp.AddEmail(TestConstants.TestEmail);
                request.AclGrantRead.AddEmail(TestConstants.TestEmail);
                request.AclGrantWrite.AddEmail(TestConstants.TestEmail);
                request.AclGrantFullControl.AddEmail(TestConstants.TestEmail);
                request.Region = Config.Region;
            }).ConfigureAwait(false);
            Assert.True(pResp.IsSuccess);
        }

        [Fact]
        public async Task PutWithObjectLocking()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            PutBucketResponse pResp = await BucketClient.PutBucketAsync(tempBucketName, request =>
            {
                request.EnableObjectLocking = true;
                request.Region = Config.Region;
            }).ConfigureAwait(false);
            Assert.True(pResp.IsSuccess);
        }
    }
}