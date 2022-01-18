using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal class GetBucketTaggingRequestMarshal : IRequestMarshal<GetBucketTaggingRequest>
{
    public Stream? MarshalRequest(GetBucketTaggingRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Tagging, string.Empty);
        return null;
    }
}