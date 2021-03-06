using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Requests.Service;
using Genbox.SimpleS3.Core.Responses.S3Types;
using Genbox.SimpleS3.Core.Responses.Service;
using Genbox.SimpleS3.Core.Responses.Service.XML;
using Genbox.SimpleS3.Core.Responses.XMLTypes;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response
{
    [UsedImplicitly]
    internal class GetServiceResponseMarshal : IResponseMarshal<GetServiceRequest, GetServiceResponse>
    {
        public void MarshalResponse(GetServiceRequest request, GetServiceResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            XmlSerializer s = new XmlSerializer(typeof(ListAllMyBucketsResult));

            using (XmlTextReader r = new XmlTextReader(responseStream))
            {
                r.Namespaces = false;

                ListAllMyBucketsResult listResult = (ListAllMyBucketsResult)s.Deserialize(r);

                if (listResult.Owner != null)
                {
                    response.Owner = new S3ObjectIdentity();
                    response.Owner.Id = listResult.Owner.Id;
                    response.Owner.Name = listResult.Owner.DisplayName;
                }

                if (listResult.Buckets != null)
                {
                    response.Buckets = new List<S3Bucket>(listResult.Buckets.Count);

                    foreach (Bucket lb in listResult.Buckets)
                    {
                        S3Bucket b = new S3Bucket();
                        b.Name = lb.Name;
                        b.CreationDate = lb.CreationDate;

                        response.Buckets.Add(b);
                    }
                }
                else
                    response.Buckets = Array.Empty<S3Bucket>();
            }
        }
    }
}