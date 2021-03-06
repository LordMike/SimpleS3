using System.IO;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Constants;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request
{
    [UsedImplicitly]
    internal class DeleteObjectRequestMarshal : IRequestMarshal<DeleteObjectRequest>
    {
        public Stream MarshalRequest(DeleteObjectRequest request)
        {
            request.AddHeader(AmzHeaders.XAmzMfa, request.Mfa);
            request.AddQueryParameter(ObjectParameters.VersionId, request.VersionId);
            return null;
        }
    }
}