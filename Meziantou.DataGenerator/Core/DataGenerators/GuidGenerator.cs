using System;
using System.Data;
using System.Linq;
using System.Xml;
using CodeFluent.Runtime.Database.Management;
using CodeFluent.Runtime.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class GuidGenerator : DataGenerator
    {
        private Guid? _sequentialGuid = null;

        public bool IsSequential { get; set; }
        public Guid InitialValue { get; set; }

        public GuidGenerator()
        {
            WellKnownDataType = WellKnownDataType.Guid;
            IsSequential = false;
        }

        public override void Configure(XmlElement element)
        {
            base.Configure(element);
            IsSequential = XmlUtilities.GetAttribute(element, "isSequential", IsSequential);
            InitialValue = XmlUtilities.GetAttribute(element, "initialValue", InitialValue);
        }

        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            DbType[] types =
            {
                DbType.Guid
            };

            return types.Contains(column.CodeFluentType.DbType);
        }

        protected override object GenerateCore(Project project, Column column)
        {
            if (IsSequential)
            {
                return IncrementValue();
            }

            return Guid.NewGuid();
        }

        private Guid IncrementValue()
        {
            if (!_sequentialGuid.HasValue)
            {
                _sequentialGuid = InitialValue;
            }
            else
            {
                // 3 - the least significant byte in Guid ByteArray [for SQL Server ORDER BY clause]
                // 10 - the most significant byte in Guid ByteArray [for SQL Server ORDERY BY clause]
                //byte[] sqlOrderMap = { 3, 2, 1, 0, 5, 4, 7, 6, 9, 8, 15, 14, 13, 12, 11, 10 };

                /*
                this._a = (int) b[3] << 24 | (int) b[2] << 16 | (int) b[1] << 8 | (int) b[0];
                this._b = (short) ((int) b[5] << 8 | (int) b[4]);
                this._c = (short) ((int) b[7] << 8 | (int) b[6]);
                this._d = b[8];
                this._e = b[9];
                this._f = b[10];
                this._g = b[11];
                this._h = b[12];
                this._i = b[13];
                this._j = b[14];
                this._k = b[15];
                */
                byte[] sqlOrderMap = { 3, 2, 1, 0, 5, 4, 4, 6, 8, 9, 10, 11, 12, 13, 14, 15 };
                Array.Reverse(sqlOrderMap);


                byte[] bytes = _sequentialGuid.Value.ToByteArray();
                for (int mapIndex = 0; mapIndex < sqlOrderMap.Length; mapIndex++)
                {
                    int bytesIndex = sqlOrderMap[mapIndex];
                    bytes[bytesIndex]++;
                    if (bytes[bytesIndex] != 0)
                    {
                        break; // No need to increment more significant bytes
                    }
                }

                _sequentialGuid = new Guid(bytes);
            }

            return _sequentialGuid.Value;
        }
    }
}