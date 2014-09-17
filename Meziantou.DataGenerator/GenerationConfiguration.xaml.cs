using System;
using System.Windows;
using System.Windows.Controls;
using CodeFluent.Runtime.Database.Management;
using Meziantou.DataGenerator.Core;
using Meziantou.DataGenerator.Core.ScriptWriters;

namespace Meziantou.DataGenerator
{
    /// <summary>
    /// Interaction logic for GenerationConfiguration.xaml
    /// </summary>
    public partial class GenerationConfiguration : UserControl
    {
        private readonly Database _database;

        public GenerationConfiguration(Database database)
        {
            if (database == null)
                throw new ArgumentNullException("database");

            _database = database;
            InitializeComponent();
            TreeViewDatabase.Items.Add(database);
        }

        private void ButtonGenerate_OnClick(object sender, RoutedEventArgs e)
        {
            Project project = new Project(_database);
            project.BatchStatementCount = 1;
            project.InitializeDefaultGenerator();
            var sqlScriptExecutor = new SqlScriptExecutor();
            project.Generate(sqlScriptExecutor, 100, 5);

            MessageBox.Show("Done");
        }


    }
}
