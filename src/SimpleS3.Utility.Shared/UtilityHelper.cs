﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Extensions.AmazonS3.Extensions;
using Genbox.SimpleS3.Extensions.BackBlazeB2.Extensions;
using Genbox.SimpleS3.Extensions.GoogleCloudStorage.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Utility.Shared
{
    public static class UtilityHelper
    {
        public static S3Provider SelectProvider()
        {
            ConsoleKeyInfo key;
            int intVal = 0;

            S3Provider[] enumValues = Enum.GetValues<S3Provider>();

            //Skip 'unknown' and 'all'
            S3Provider[] choices = enumValues.Skip(1).Take(enumValues.Length - 2).ToArray();

            do
            {
                Console.WriteLine("Please select which provider you want to use:");

                for (int i = 0; i < choices.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {choices[i]}");
                }

                key = Console.ReadKey(true);
            } while (!choices.Any(x => int.TryParse(key.KeyChar.ToString(), out intVal) && intVal >= 0 && intVal <= choices.Length));

            return choices[intVal - 1];
        }

        public static string GetProfileName(S3Provider provider)
        {
            return "TestSetup-" + provider;
        }

        public static string GetTestBucket(IProfile profile)
        {
            return "testbucket-" + profile.KeyId[..8].ToLowerInvariant();
        }

        public static string GetTemporaryBucket()
        {
            return "tempbucket-" + Guid.NewGuid();
        }

        public static bool IsTestBucket(string bucketName, IProfile profile)
        {
            return string.Equals(bucketName, GetTestBucket(profile), StringComparison.Ordinal);
        }

        public static bool IsTemporaryBucket(string bucketName)
        {
            return bucketName.StartsWith("tempbucket-", StringComparison.OrdinalIgnoreCase);
        }

        public static ServiceProvider CreateSimpleS3(S3Provider provider, string profileName, bool enableRetry)
        {
            ServiceCollection services = new ServiceCollection();
            ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(services);

            IConfigurationRoot configRoot = new ConfigurationBuilder()
                                            .SetBasePath(Environment.CurrentDirectory)
                                            .AddJsonFile("Config.json", false)
                                            .Build();

            IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();

            if (enableRetry)
                httpBuilder.UseDefaultHttpPolicy();

            IConfigurationSection? proxySection = configRoot.GetSection("Proxy");

            if (proxySection != null && proxySection.GetValue<bool>("UseProxy"))
                httpBuilder.UseProxy(new WebProxy(proxySection.GetValue<string>("ProxyAddress")));

            coreBuilder.UseProfileManager()
                       .BindConfigToProfile(profileName)
                       .UseConsoleSetup();

            if (provider == S3Provider.AmazonS3)
                coreBuilder.UseAmazonS3();
            else if (provider == S3Provider.BackBlazeB2)
                coreBuilder.UseBackBlazeB2();
            else if (provider == S3Provider.GoogleCloudStorage)
                coreBuilder.UseGoogleCloudStorage();
            else
                throw new ArgumentOutOfRangeException(nameof(provider), provider, null);

            return services.BuildServiceProvider();
        }

        public static IProfile GetOrSetupProfile(IServiceProvider serviceProvider, S3Provider provider, string profileName)
        {
            //Check if there is a profile for this provider
            IProfileManager profileManager = serviceProvider.GetRequiredService<IProfileManager>();
            IProfile? profile = profileManager.GetProfile(profileName);

            if (profile == null)
            {
                Console.WriteLine("The profile " + profileName + " does not exist.");

                if (provider == S3Provider.AmazonS3)
                    Console.WriteLine("You can create a new API key at https://console.aws.amazon.com/iam/home?#/security_credentials");
                else if (provider == S3Provider.BackBlazeB2)
                    Console.WriteLine("You can create a new API key at https://secure.backblaze.com/app_keys.htm");

                IProfileSetup profileSetup = serviceProvider.GetRequiredService<IProfileSetup>();
                return profileSetup.SetupProfile(profileName);
            }

            return profile;
        }

        public static async Task<int> ForceDeleteBucketAsync(S3Provider provider, ISimpleClient client, string bucket)
        {
            int errors = 0;

            await foreach (S3DeleteError error in DeleteAllObjects(provider, client, bucket))
            {
                errors++;

                PutObjectLegalHoldResponse legalResp = await client.PutObjectLegalHoldAsync(bucket, error.ObjectKey, false, r => r.VersionId = error.VersionId);

                if (legalResp.IsSuccess)
                {
                    DeleteObjectResponse delResp = await client.DeleteObjectAsync(bucket, error.ObjectKey, x => x.VersionId = error.VersionId);

                    if (delResp.IsSuccess)
                        errors--;
                }
            }

            if (errors > 0)
            {
                //Google counts multipart uploads as part of bucket
                if (provider == S3Provider.GoogleCloudStorage)
                {
                    //Abort all incomplete multipart uploads
                    IAsyncEnumerable<S3Upload> partUploads = client.ListAllMultipartUploadsAsync(bucket);

                    await foreach (S3Upload partUpload in partUploads)
                    {
                        AbortMultipartUploadResponse abortResp = await client.AbortMultipartUploadAsync(bucket, partUpload.ObjectKey, partUpload.UploadId);

                        if (abortResp.IsSuccess)
                            errors--;
                    }
                }
            }

            return errors;
        }

        private static async IAsyncEnumerable<S3DeleteError> DeleteAllObjects(S3Provider provider, ISimpleClient client, string bucket)
        {
            ListObjectVersionsResponse response;
            Task<ListObjectVersionsResponse> responseTask = client.ListObjectVersionsAsync(bucket);

            do
            {
                response = await responseTask;

                if (!response.IsSuccess)
                    yield break;

                if (response.Versions.Count + response.DeleteMarkers.Count == 0)
                    break;

                if (response.IsTruncated)
                {
                    string keyMarker = response.NextKeyMarker;
                    responseTask = client.ListObjectVersionsAsync(bucket, req => req.KeyMarker = keyMarker);
                }

                IEnumerable<S3DeleteInfo> delete = response.Versions.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId))
                                                           .Concat(response.DeleteMarkers.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId)));

                //Google does not support DeleteObjects
                if (provider == S3Provider.GoogleCloudStorage)
                {
                    foreach (S3DeleteInfo info in delete)
                    {
                        await client.DeleteObjectAsync(bucket, info.ObjectKey, info.VersionId);
                    }
                }
                else
                {
                    DeleteObjectsResponse multiDelResponse = await client.DeleteObjectsAsync(bucket, delete, req => req.Quiet = false).ConfigureAwait(false);

                    if (!multiDelResponse.IsSuccess)
                        yield break;

                    foreach (S3DeleteError error in multiDelResponse.Errors)
                    {
                        yield return error;
                    }
                }
            } while (response.IsTruncated);
        }
    }
}