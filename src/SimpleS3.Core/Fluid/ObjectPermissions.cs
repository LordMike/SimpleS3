﻿using System;

namespace Genbox.SimpleS3.Core.Fluid
{
    [Flags]
    public enum ObjectPermissions
    {
        Unknown = 0,

        /// <summary>Equivalent to s3:GetObject, s3:GetObjectVersion, and s3:GetObjectTorrent</summary>
        Read = 1,

        /// <summary>Equivalent to s3:GetObjectAcl and s3:GetObjectVersionAcl</summary>
        ReadAcl = 2,

        /// <summary>Equivalent to s3:PutObjectAcl and s3:PutObjectVersionAcl</summary>
        WriteAcl = 4,

        /// <summary>Equivalent to granting Read, ReadAcl, and WriteAcl permissions.</summary>
        FullControl = 8
    }
}