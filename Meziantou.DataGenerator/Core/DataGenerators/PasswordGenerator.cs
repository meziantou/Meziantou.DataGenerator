using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using CodeFluent.Runtime.Database.Management;
using CodeFluent.Runtime.Utilities;
using Meziantou.DataGenerator.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class PasswordGenerator : DataGenerator
    {
        public PasswordFormat Format { get; set; }

        public PasswordGenerator()
        {
            Format = PasswordFormat.Auto;
        }

        public override void Configure(XmlElement element)
        {
            base.Configure(element);
            Format = XmlUtilities.GetAttribute(element, "format", Format);
        }

        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            return IsStringDbType(column) || IsBinaryDbType(column);
        }

        protected override object GenerateCore(Project project, Column column)
        {
            string password = Random.NextString(4, 32);

            PasswordFormat format = Format;
            if (Format == PasswordFormat.Auto)
            {
                if (IsBinaryDbType(column))
                {
                    if (column.MaxLength >= 512 / 8)
                    {
                        format = PasswordFormat.Sha512;
                    }
                    else if (column.MaxLength >= 384 / 8)
                    {
                        format = PasswordFormat.Sha384;
                    }
                    else if (column.MaxLength >= 256 / 8)
                    {
                        format = PasswordFormat.Sha256;
                    }
                    else if (column.MaxLength >= 160 / 8)
                    {
                        format = PasswordFormat.Sha1;
                    }
                    else if (column.MaxLength >= 128 / 8)
                    {
                        format = PasswordFormat.Md5;
                    }
                }
                else if (IsStringDbType(column))
                {
                    if (column.MaxLength >= 512 / 8 * 2)
                    {
                        format = PasswordFormat.Sha512;
                    }
                    else if (column.MaxLength >= 384 / 8 * 2)
                    {
                        format = PasswordFormat.Sha384;
                    }
                    else if (column.MaxLength >= 256 / 8 * 2)
                    {
                        format = PasswordFormat.Sha256;
                    }
                    else if (column.MaxLength >= 160 / 8 * 2)
                    {
                        format = PasswordFormat.Sha1;
                    }
                    else if (column.MaxLength >= 128 / 8 * 2)
                    {
                        format = PasswordFormat.Md5;
                    }
                }
            }

            switch (format)
            {
                case PasswordFormat.Clear:
                    return password;
                case PasswordFormat.Sha1:
                case PasswordFormat.Sha256:
                case PasswordFormat.Sha384:
                case PasswordFormat.Sha512:
                case PasswordFormat.Md5:
                    return GetHash(column, format, password);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private object GetHash(Column column, PasswordFormat format, string password)
        {
            using (HashAlgorithm algorithm = CreateAlgorithm(format))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(password);
                byte[] hash = algorithm.ComputeHash(buffer);

                if (IsStringDbType(column))
                {
                    return ConvertUtilities.ToHexa(hash);
                }

                return hash;
            }
        }

        private HashAlgorithm CreateAlgorithm(PasswordFormat passwordFormat)
        {
            switch (passwordFormat)
            {
                case PasswordFormat.Sha1:
                    return new SHA1Managed();
                case PasswordFormat.Sha256:
                    return new SHA256Managed();
                case PasswordFormat.Sha384:
                    return new SHA384Managed();
                case PasswordFormat.Sha512:
                    return new SHA512Managed();
                case PasswordFormat.Md5:
                    return new MD5Cng();
                default:
                    throw new ArgumentOutOfRangeException("passwordFormat");
            }
        }
    }
}