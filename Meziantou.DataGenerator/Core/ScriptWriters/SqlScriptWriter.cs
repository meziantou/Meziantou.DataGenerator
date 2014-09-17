using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CodeFluent.Runtime.Database.Management;
using CodeFluent.Runtime.Utilities;

namespace Meziantou.DataGenerator.Core.ScriptWriters
{
    public class SqlScriptWriter : ScriptWriter
    {
        private readonly TextWriter _writer;

        public SqlScriptWriter(TextWriter writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            _writer = writer;
        }

        protected TextWriter Writer
        {
            get { return _writer; }
        }

        private static void Concatenate<T>(TextWriter writer, IEnumerable<T> objects, Func<T, object> select, string separator)
        {
            if (writer == null) throw new ArgumentNullException("sb");

            if (objects == null)
                return;

            bool first = true;
            foreach (T obj in objects)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.Write(separator);
                }

                object value = @select(obj);
                if (value != null)
                {
                    writer.Write(value);
                }
            }
        }

        protected virtual string BatchSeparator
        {
            get { return "GO"; }
        }

        protected virtual string GetTableName(Table table)
        {
            return table.FullName;
        }

        protected virtual string GetColumnName(Column column)
        {
            return column.FullName;
        }

        public override void WriteHeader(Project project)
        {
            // Delete data before generating
            foreach (var table in project.Tables)
            {
                Writer.WriteLine("DELETE FROM " + GetTableName(table));
                WriteBatchSeparator(project.Database);
            }
        }

        public override void WriteBeginTable(Database database, Table table)
        {
            Writer.WriteLine();
            Writer.WriteLine("-- Begin " + table.FullName);
        }

        public override void WriteEndTable(Database database, Table table)
        {
            Writer.WriteLine("-- End " + table.FullName);
            Writer.WriteLine(BatchSeparator);
        }

        public override void WriteBeginRow(Database database, Table table, IList<Column> columns)
        {
            Writer.Write("INSERT INTO ");
            Writer.Write(GetTableName(table));
            Writer.Write(" (");
            Concatenate(Writer, columns, GetColumnName, ",");
            Writer.WriteLine(")");
            Writer.Write("VALUES (");
        }

        public override void WriteValueSeparator(Database database)
        {
            Writer.Write(", ");
        }

        public override void WriteEndRow(Database database, Table table, IEnumerable<Column> columns)
        {
            Writer.WriteLine(")");
        }

        public override void WriteBatchSeparator(Database database)
        {
            Writer.WriteLine(BatchSeparator);
        }

        public override void WriteValue(Database database, Column column, object value)
        {
            Writer.Write(GetValue(column, value));
        }

        private void WriteComment(string comment, int indent = 0)
        {
            if (comment == null)
                return;

            var lines = comment.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                Writer.WriteLine("-- " + new string(' ', indent * 4) + line);
            }
        }

        private string GetValue(Column column, object value)
        {
            if (value == null)
            {
                return "NULL";
            }
            else
            {
                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.Empty:
                        break;
                    case TypeCode.Object:
                        if (value is Guid)
                        {
                            return string.Format("'{0}'", ((Guid)value).ToString("D"));
                        }

                        byte[] bytes = value as byte[];
                        if (bytes != null)
                        {
                            return "0x" + ConvertUtilities.ToHexa(bytes);
                        }
                        break;
                    case TypeCode.DBNull:
                        return "NULL";
                    case TypeCode.Boolean:
                        return (bool)value ? "1" : "0";
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return string.Format(CultureInfo.InvariantCulture, "{0}", value);
                    case TypeCode.DateTime:
                        return string.Format("'{0:O}'", (DateTime)value);
                    case TypeCode.Char:
                    case TypeCode.String:
                        return EscapeStringValue(column, string.Format("{0}", value));
                }
            }

            throw new InvalidOperationException();
        }

        private string EscapeStringValue(Column column, string value)
        {
            StringBuilder sb = new StringBuilder();
            if (column.CodeFluentType.IsDbUnicode)
            {
                sb.Append('N');
            }

            sb.Append('\'');
            sb.Append(value.Replace("'", "''"));
            sb.Append('\'');
            return sb.ToString();
        }
    }
}