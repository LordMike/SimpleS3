﻿using System.Threading.Tasks;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.Core.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class ContentPropertyTests : LiveTestBase
    {
        public ContentPropertyTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task ContentProperties()
        {
            await UploadAsync(nameof(ContentProperties), request =>
            {
                request.ContentDisposition.Set(ContentDispositionType.Attachment, "filename.jpg");
                request.ContentEncoding.Add(ContentEncodingType.Identity);
                request.ContentType.Set("text/html", "utf-8");
            }).ConfigureAwait(false);

            GetObjectResponse resp = await AssertAsync(nameof(ContentProperties)).ConfigureAwait(false);
            Assert.Equal(4, resp.ContentLength);
            Assert.Equal("attachment; filename*=\"filename.jpg\"", resp.ContentDisposition);
            Assert.Equal("text/html; charset=utf-8", resp.ContentType);
        }

        [Fact]
        public async Task ContentPropertiesFluid()
        {
            await UploadTransferAsync(nameof(ContentPropertiesFluid), upload =>
            {
                upload.WithContentDisposition(ContentDispositionType.Attachment, "filename.jpg");
                upload.WithContentEncoding(ContentEncodingType.Identity);
                upload.WithContentType("text/html", "utf-8");
            }).ConfigureAwait(false);

            GetObjectResponse resp = await AssertAsync(nameof(ContentPropertiesFluid)).ConfigureAwait(false);
            Assert.Equal(4, resp.ContentLength);
            Assert.Equal("attachment; filename*=\"filename.jpg\"", resp.ContentDisposition);
            Assert.Equal("text/html; charset=utf-8", resp.ContentType);
        }
    }
}