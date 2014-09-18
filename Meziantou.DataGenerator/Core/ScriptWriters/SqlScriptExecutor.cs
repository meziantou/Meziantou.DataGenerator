using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Permissions;
using System.Text;
using CodeFluent.Runtime;
using CodeFluent.Runtime.Database.Management;
using Meziantou.DataGenerator.Diagnostics;

namespace Meziantou.DataGenerator.Core.ScriptWriters
{
    public class SqlScriptExecutor : ScriptWriter
    {
        private List<IDbDataParameter> _parameters;
        private string _command;

        protected virtual string GetTableName(Table table)
        {
            return table.FullName;
        }

        protected virtual string GetColumnName(Column column)
        {
            return column.FullName;
        }

        protected virtual string GetParameterName(Column column)
        {
            if (column.SortOrder >= 0)
            {
                return "@Column" + column.SortOrder;
            }

            return "@" + column.Name;
        }

        public override void WriteHeader(Project project)
        {
            // Delete data before generating
            foreach (var table in project.Tables)
            {
                project.Database.ExecuteNonQuery("DELETE FROM " + GetTableName(table));
            }
        }

        public override void WriteBeginRow(Database database, Table table, IList<Column> columns)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(GetTableName(table));
            sb.Append(" (");
            bool first = true;
            foreach (var column in columns)
            {
                if (!first)
                {
                    sb.Append(",");
                }

                sb.Append(GetColumnName(column));
                first = false;
            }

            sb.Append(")");
            sb.Append("VALUES (");

            first = true;
            foreach (var column in columns)
            {
                if (!first)
                {
                    sb.Append(",");
                }

                sb.Append(GetParameterName(column));
                first = false;
            }
            sb.Append(")");

            _command = sb.ToString();
            _parameters = new List<IDbDataParameter>();
            Logger.Log(LogType.Information, value: "Command: " + _command);
        }

        public override void WriteEndRow(Database database, Table table, IEnumerable<Column> columns)
        {
            if (string.IsNullOrEmpty(_command))
                return;

            try
            {
                database.ExecuteNonQuery(_command, _parameters == null ? null : _parameters.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Log(LogType.Error, value: ex);
            }
        }

        public override void WriteValue(Database database, Column column, object value)
        {
            using (IDbConnection newConnection = database.NewConnection())
            {
                using (IDbCommand dbCommand = newConnection.CreateCommand())
                {
                    var parameter = dbCommand.CreateParameter();
                    parameter.ParameterName = GetParameterName(column);
                    parameter.Value = value;
                    parameter.DbType = column.CodeFluentType.DbType;
                    _parameters.Add(parameter);
                    Logger.Log(LogType.Information, indent: 1, value: string.Format("Add Parameter: {0}={1}", column.FullName, parameter.Value));
                }
            }
        }
    }
}