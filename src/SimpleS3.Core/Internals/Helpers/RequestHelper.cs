using System.Text;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Helpers;

namespace Genbox.SimpleS3.Core.Internals.Helpers;

internal static class RequestHelper
{
    public static void AppendQueryParameters<TReq>(StringBuilder sb, TReq request) where TReq : IRequest
    {
        if (request.QueryParameters.Count > 0)
            sb.Append('?').Append(UrlHelper.CreateQueryString(request.QueryParameters));
    }
}