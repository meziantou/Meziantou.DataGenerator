using System;
using System.Collections.Generic;
using CodeFluent.Runtime.Database.Management;

namespace Meziantou.DataGenerator.Core.ScriptWriters
{
    public class TeeScriptWriter : ScriptWriter
    {
        private readonly IList<ScriptWriter> _writers = new List<ScriptWriter>();

        public IList<ScriptWriter> Writers
        {
            get { return _writers; }
        }

        public TeeScriptWriter()
        {
        }

        public TeeScriptWriter(ScriptWriter scriptWriter)
        {
            _writers.Add(scriptWriter);
        }

        public TeeScriptWriter(ScriptWriter scriptWriter1, ScriptWriter scriptWriter2)
        {
            _writers.Add(scriptWriter1);
            _writers.Add(scriptWriter2);
        }

        private void Execute(Action<ScriptWriter> action)
        {
            foreach (var scriptWriter in Writers)
            {
                if (scriptWriter != null)
                {
                    action(scriptWriter);
                }
            }
        }

        public override void WriteHeader(Project project)
        {
            Execute(sw => sw.WriteHeader(project));
        }

        public override void WriteFooter(Project project)
        {
            Execute(sw => sw.WriteFooter(project));
        }

        public override void WriteBeginTable(Database database, Table table)
        {
            Execute(sw => sw.WriteBeginTable(database, table));
        }

        public override void WriteEndTable(Database database, Table table)
        {
            Execute(sw => sw.WriteEndTable(database, table));
        }

        public override void WriteBeginRow(Database database, Table table, IList<Column> columns)
        {
            Execute(sw => sw.WriteBeginRow(database, table, columns));
        }

        public override void WriteEndRow(Database database, Table table, IEnumerable<Column> columns)
        {
            Execute(sw => sw.WriteEndRow(database, table, columns));
        }

        public override void WriteValue(Database database, Column column, object value)
        {
            Execute(sw => sw.WriteValue(database, column, value));
        }

        public override void WriteValueSeparator(Database database)
        {
            Execute(sw => sw.WriteValueSeparator(database));
        }

        public override void WriteBatchSeparator(Database database)
        {
            Execute(sw => sw.WriteBatchSeparator(database));
        }
    }
}