﻿using System.Collections.Generic;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Internal;
using Genbox.SimpleS3.Core.Internal.Helpers;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Responses.Errors
{
    [PublicAPI]
    public class GenericError : IError
    {
        public GenericError(IDictionary<string, string> lookup)
        {
            Validator.RequireNotNull(lookup);

            Code = ValueHelper.ParseEnum<ErrorCode>(lookup["Code"]);
            Message = lookup["Message"];
        }

        public ErrorCode Code { get; }
        public string Message { get; }

        public virtual string GetExtraData()
        {
            return string.Empty;
        }
    }
}