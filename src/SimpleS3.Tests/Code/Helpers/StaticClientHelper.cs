﻿using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Tests.Code.Other;

namespace Genbox.SimpleS3.Tests.Code.Helpers
{
    public static class StaticClientHelper
    {
        public static (FakeHttpHandler handler, S3Client client) CreateFakeClient()
        {
            S3Config config = new S3Config(new StringAccessKey("ExampleKeyId00000000", "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY"), AwsRegion.USEast1);

            FakeHttpHandler fakeHandler = new FakeHttpHandler();

            S3Client client = new S3Client(config, fakeHandler);
            return (fakeHandler, client);
        }
    }
}