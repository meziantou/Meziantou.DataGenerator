using System.Collections.Generic;
using CodeFluent.Runtime.Database.Management;

namespace Meziantou.DataGenerator.Core
{
    public abstract class ScriptWriter
    {
        public virtual void WriteHeader(Project project)
        {
        }

        public virtual void WriteFooter(Project project)
        {
        }

        public virtual void WriteBeginTable(Database database, Table table)
        {
        }

        public virtual void WriteEndTable(Database database, Table table)
        {
        }

        public virtual void WriteBeginRow(Database database, Table table, IList<Column> columns)
        {

        }

        public virtual void WriteEndRow(Database database, Table table, IEnumerable<Column> columns)
        {

        }

        public virtual void WriteValue(Database database, Column column, object value)
        {

        }

        public virtual void WriteValueSeparator(Database database)
        {

        }

        public virtual void WriteBatchSeparator(Database database)
        {
        }
    }
}