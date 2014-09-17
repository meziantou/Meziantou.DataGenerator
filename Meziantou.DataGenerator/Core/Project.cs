using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CodeFluent.Runtime.Database.Management;
using CodeFluent.Runtime.Utilities;
using Meziantou.DataGenerator.Core.DataGenerators;
using Meziantou.DataGenerator.Diagnostics;

namespace Meziantou.DataGenerator.Core
{
    public class Project
    {
        private readonly Database _database;
        private readonly IDictionary<Table, IDictionary<Column, DataGenerator>> _dataGenerators;
        private readonly HashSet<Column> _columns;
        private readonly IDictionary<Column, IList<object>> _generatedValues = new Dictionary<Column, IList<object>>();
        private RowValues _currentRow;

        public Database Database
        {
            get { return _database; }
        }

        public IEnumerable<Column> Columns
        {
            get { return _columns; }
        }

        public IEnumerable<Table> Tables
        {
            get { return _dataGenerators.Keys; }
        }

        public Project(Database database)
        {
            if (database == null) throw new ArgumentNullException("database");

            _dataGenerators = new Dictionary<Table, IDictionary<Column, DataGenerator>>();
            _columns = new HashSet<Column>();
            _database = database;
            BatchStatementCount = 50;
        }

        public void InitializeDefaultGenerator()
        {
            List<Hint> hints = new List<Hint>();

            // Load user hints
            var hintPath = CommandLineUtilities.GetArgument<string>("hint", null);
            if (hintPath != null)
            {
                AddHintsFromFile(hintPath, hints);
            }

            // Maybe there is an hint file at executable level
            AddHintsFromFile("hints.xml", hints);

            // default Hints
            hints.Add(new Hint(null, "First.?Name", new FirstNameGenerator()));
            hints.Add(new Hint(null, "Last.?Name", new LastNameGenerator()));
            hints.Add(new Hint(null, "Full.?Name", new FullNameGenerator()));
            hints.Add(new Hint(null, "User.?Name|CreationUser|LastWriteUser", new UserNameGenerator()));
            hints.Add(new Hint(null, "File.?Name$", new FileNameGenerator()));
            hints.Add(new Hint(null, "Name$", new BrandNameGenerator()));
            //hints.Add(new Hint("Name$", new StringGenerator() { CharacterSet = CharacterSet.LowerAlpha | CharacterSet.UpperAlpha | CharacterSet.Specials }));
            hints.Add(new Hint(null, "Email", new EmailGenerator()));
            hints.Add(new Hint(null, "Culture|Lcid", new CultureGenerator()));
            hints.Add(new Hint(null, "Country|Location", new CountryGenerator()));
            hints.Add(new Hint(null, "Title|Gender", new GenderGenerator()));
            hints.Add(new Hint(null, "Password|SecurityStamp", new PasswordGenerator()));
            hints.Add(new Hint(null, "Count|Quantity|Qty", new NumberGenerator() { Minimum = 0, WellKnownDataType = WellKnownDataType.Quantity }));
            hints.Add(new Hint(null, "StartDate", new DateGenerator() { WellKnownDataType = WellKnownDataType.StartDate }));
            hints.Add(new Hint(null, "EndDate", new DateGenerator() { WellKnownDataType = WellKnownDataType.EndDate }));
            hints.Add(new Hint(null, "DateOfBirth|BirthDate", new DateGenerator() { WellKnownDataType = WellKnownDataType.DateOfBirth, Minimum = DateTime.Today.Subtract(TimeSpan.FromDays(365 * 100)), Maximum = DateTime.UtcNow }));
            hints.Add(new Hint(null, "(Creation|Last.?Write).?(Date|Time)|TimeStamp", new DateGenerator() { Maximum = DateTime.UtcNow }));
            hints.Add(new Hint(null, "Percentage$", new NumberGenerator() { Minimum = 0, Maximum = 100 }));
            hints.Add(new Hint(null, "TotalPrice|TotalAmount|TotalCost", new NumberGenerator() { Minimum = 0, WellKnownDataType = WellKnownDataType.TotalPrice }));
            hints.Add(new Hint(null, "Price|Cost", new NumberGenerator() { Minimum = 0, WellKnownDataType = WellKnownDataType.UnitPrice }));
            hints.Add(new Hint(null, "Size", new NumberGenerator() { Minimum = 0 }));
            hints.Add(new Hint(null, "(Phone|Tel|Telephone)(Number)?$", new PhoneNumberGenerator()));
            hints.Add(new Hint(null, "Color$", new ColorGenerator()));

            // Add generators by priority
            List<DataGenerator> dataGenerators = new List<DataGenerator>();
            dataGenerators.Add(new GuidGenerator());
            dataGenerators.Add(new NumberGenerator());
            dataGenerators.Add(new BooleanGenerator());
            dataGenerators.Add(new DateGenerator());
            dataGenerators.Add(new ForeignKeyGenerator());
            dataGenerators.Add(new LipsumGenerator());
            dataGenerators.Add(new BinaryGenerator());

            foreach (Table table in _database.Tables)
            {
                Logger.Log(LogType.Information, value: string.Format("Table {0}", table.FullName));
                foreach (Column column in table.Columns)
                {
                    if (column.IsIdentity)
                        continue;

                    bool set = false;
                    foreach (var hint in hints)
                    {
                        if (hint.Match(column) && hint.DataGenerator.CanGenerate(column))
                        {
                            SetDataGenerator(column, (DataGenerator)hint.DataGenerator.Clone());
                            set = true;
                            break;
                        }
                    }

                    if (!set)
                    {
                        foreach (var dataGenerator in dataGenerators)
                        {
                            if (dataGenerator.CanGenerate(column))
                            {
                                SetDataGenerator(column, (DataGenerator)dataGenerator.Clone());
                            }
                        }
                    }
                }
            }
        }

        private static void AddHintsFromFile(string hintPath, ICollection<Hint> hints)
        {
            if (!File.Exists(hintPath))
            {
                Logger.Log(LogType.Information, value: "File not found: " + hintPath);
                return;
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(hintPath);
                var nodes = doc.SelectNodes("//hint");
                if (nodes != null)
                {
                    foreach (XmlElement node in nodes.OfType<XmlElement>())
                    {
                        var tablePattern = XmlUtilities.GetAttribute<string>(node, "table", null);
                        var columnPattern = XmlUtilities.GetAttribute<string>(node, "column", null);
                        var generatorTypeName = XmlUtilities.GetAttribute<string>(node, "typeName", null);
                        var seed = XmlUtilities.GetAttribute(node, "seed", -1);

                        if (generatorTypeName != null)
                        {
                            var type = Type.GetType(generatorTypeName, false, true);
                            if (type == null)
                                continue;

                            try
                            {
                                var dataGenerator = (DataGenerator)Activator.CreateInstance(type);
                                if (seed != -1)
                                {
                                    dataGenerator.Seed = seed;
                                }

                                hints.Add(new Hint(tablePattern, columnPattern, dataGenerator));
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(LogType.Error, value: ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogType.Error, value: ex);
            }
        }

        public void SetDataGenerator(Column column, DataGenerator dataGenerator)
        {
            if (column == null) throw new ArgumentNullException("column");

            _columns.Add(column);

            var table = column.Parent;
            IDictionary<Column, DataGenerator> dic;
            if (!_dataGenerators.TryGetValue(table, out dic))
            {
                dic = new Dictionary<Column, DataGenerator>();
                _dataGenerators.Add(table, dic);
            }

            if (dataGenerator == null)
            {
                dic.Remove(column);
            }
            else
            {
                dic[column] = dataGenerator;
                Logger.Log(LogType.Information, indent: 1, value: string.Format("{0}: {1} ({2})", column.FullName, dataGenerator.GetType().Name, dataGenerator.Seed));
            }
        }

        public IList<object> GetGeneratedValues(Column column)
        {
            if (column == null) throw new ArgumentNullException("column");

            return _generatedValues[column];
        }

        public void Generate(ScriptWriter scriptWriter, int count, int nullCount)
        {
            if (scriptWriter == null) throw new ArgumentNullException("scriptWriter");

            _generatedValues.Clear();

            // We need to keep some columns values...
            foreach (var column in _columns)
            {
                foreach (var fk in column.Parent.ForeignKeys)
                {
                    Key pk = fk.ReferencedTable.PrimaryKey;
                    if (pk == null)
                        continue;

                    foreach (var pkColumn in pk.Columns)
                    {
                        if (_columns.Contains(pkColumn) && !_generatedValues.ContainsKey(pkColumn))
                        {
                            _generatedValues.Add(pkColumn, new List<object>());
                        }
                    }
                }
            }

            // Compute generation order
            Logger.Log(LogType.Verbose, value: "Compute table generation order");
            List<Table> tables = new List<Table>();
            var allTables = _dataGenerators.Keys.ToList();
            while (allTables.Any())
            {
                Table addedTable = null;
                foreach (var table in allTables)
                {
                    bool canAdd = true;
                    foreach (var foreignKey in table.ForeignKeys)
                    {
                        if (!tables.Contains(foreignKey.ReferencedTable))
                        {
                            canAdd = false;
                            break;
                        }
                    }

                    if (canAdd)
                    {
                        addedTable = table;
                        break;
                    }
                }

                if (allTables.Any() && addedTable == null)
                {
                    throw new Exception("Cannot determine generation order.");
                }

                tables.Add(addedTable);
                allTables.Remove(addedTable);
            }

            // Generate values
            scriptWriter.WriteHeader(this);

            foreach (var table in tables)
            {
                Generate(scriptWriter, table, count, nullCount);
            }

            scriptWriter.WriteFooter(this);
        }

        public RowValues CurrentRow
        {
            get { return _currentRow; }
        }

        private void Generate(ScriptWriter scriptWriter, Table table, int count, int nullCount)
        {
            if (scriptWriter == null) throw new ArgumentNullException("scriptWriter");
            if (table == null) throw new ArgumentNullException("table");

            IDictionary<Column, DataGenerator> dataGenerators = _dataGenerators[table];

            // Sort columns
            Logger.Log(LogType.Verbose, value: "Compute column generation order for table " + table.FullName);
            List<KeyValuePair<Column, DataGenerator>> sortedList = new List<KeyValuePair<Column, DataGenerator>>();
            List<KeyValuePair<Column, DataGenerator>> pairs = dataGenerators.ToList();
            while (pairs.Any())
            {
                KeyValuePair<Column, DataGenerator>? addedValue = null;
                foreach (var first in pairs)
                {
                    bool canAdd = true;
                    foreach (var second in pairs)
                    {
                        if (first.Value.CompareTo(second.Value) > 0)
                        {
                            canAdd = false;
                        }
                    }

                    if (canAdd)
                    {
                        addedValue = first;
                        break;
                    }
                }

                if (pairs.Any() && addedValue == null)
                {
                    throw new Exception("Cannot determine genration order.");
                }

                if (addedValue.HasValue)
                {
                    sortedList.Add(addedValue.Value);
                    pairs.Remove(addedValue.Value);
                }
            }

            List<Column> columns = sortedList.Select(dataGenerator => dataGenerator.Key).ToList();
            List<DataGenerator> generators = sortedList.Select(_ => _.Value).ToList();
            var values = GenerateValues(this, columns, generators, count, nullCount);

            // Generate values
            Logger.Log(LogType.Verbose, value: "Begin generating rows for table " + table.FullName);
            scriptWriter.WriteBeginTable(_database, table);

            for (int i = 0; i < count; i++)
            {
                if (i > 0 && BatchStatementCount > 0 && i % BatchStatementCount == 0)
                {
                    scriptWriter.WriteBatchSeparator(_database);
                }

                scriptWriter.WriteBeginRow(_database, table, columns);
                _currentRow = new RowValues(columns, generators);
                bool first = true;
                foreach (var column in columns)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        scriptWriter.WriteValueSeparator(_database);
                    }

                    var enumerator = values[column];
                    if (!enumerator.MoveNext())
                    {
                        throw new Exception("Can't move next.");
                    }

                    CurrentRow.AddValue(column, enumerator.Current);

                    IList<object> generatedValues;
                    if (_generatedValues.TryGetValue(column, out generatedValues))
                    {
                        generatedValues.Add(enumerator.Current);
                    }

                    scriptWriter.WriteValue(_database, column, enumerator.Current);
                }

                scriptWriter.WriteEndRow(_database, table, columns);
            }

            scriptWriter.WriteEndTable(_database, table);
            _currentRow = null;
        }

        private IDictionary<Column, IEnumerator<object>> GenerateValues(Project project, ICollection<Column> columns, ICollection<DataGenerator> dataGenerators, int count, int nullCount)
        {
            if (columns.Count != dataGenerators.Count)
                throw new ArgumentException();

            var result = new Dictionary<Column, IEnumerator<object>>();
            using (var e1 = columns.GetEnumerator())
            using (var e2 = dataGenerators.GetEnumerator())
            {
                while (e1.MoveNext() && e2.MoveNext())
                {
                    result.Add(e1.Current, e2.Current.Generate(this, e1.Current, count, nullCount).GetEnumerator());
                }
            }

            return result;
        }

        public int BatchStatementCount { get; set; }
    }
}