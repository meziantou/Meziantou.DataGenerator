using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CodeFluent.Runtime.Database.Design;
using CodeFluent.Runtime.Database.Management;
using CodeFluent.Runtime.Utilities;
using Meziantou.DataGenerator.Core;
using Meziantou.DataGenerator.Core.ScriptWriters;
using Meziantou.DataGenerator.Utilities;

namespace Meziantou.DataGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ComboBoxDatabaseSystem.SelectedIndex = 0;

            List<RecentConnectionString> recents = Settings.Current.RecentConnectionStrings;
            RecentComboBox.ItemsSource = recents;
            if (recents == null || recents.Count == 0)
            {
                RecentComboBox.Visibility = Visibility.Collapsed;
                RecentTextBlock.Visibility = Visibility.Collapsed;
            }

            if (Settings.Current.RecentConnectionStrings != null)
            {
                RecentConnectionString recentConnectionString = Settings.Current.RecentConnectionStrings.FirstOrDefault();
                if (recentConnectionString != null)
                {
                    ConnectionStringObject = recentConnectionString.ToConnectionStringObject();
                }
            }

            UpdateControls();
        }

        private ConnectionStringObject ConnectionStringObject
        {
            get { return TextBoxConnectionString.ConnectionStringObject; }
            set
            {
                TextBoxConnectionString.ConnectionStringObject = value;
                UpdateControls();
            }
        }

        private DatabaseSystem SelectedDatabaseSystem
        {
            get { return (DatabaseSystem)ComboBoxDatabaseSystem.SelectedItem; }
        }

        private async void ButtonGenerate_OnClick(object sender, RoutedEventArgs e)
        {
            if (ConnectionStringObject == null)
                return;

            Database database = Database.Get(SelectedDatabaseSystem.ToString(), ConnectionStringObject.ToString(true));
            if (!database.Exists)
            {
                MessageBox.Show("Cannot connect to the database");
                return;
            }

            // Update Recent configurations
            Settings.Current.AddRecentConnectionString(SelectedDatabaseSystem, database);
            Settings.Current.SerializeToConfiguration();

            // Generate
            try
            {
                CircularProgressBar.Visibility = Visibility.Visible;
                Exception exception = null;
                var count = ConvertUtilities.ChangeType(TextBoxRows.Text, 100);
                var nullCount = ConvertUtilities.ChangeType(TextBoxNullRows.Text, 5);

                await Task.Run(() =>
                {
                    Project project = new Project(database);
                    project.BatchStatementCount = 1;
                    project.InitializeDefaultGenerator();
                    SqlScriptExecutor sqlScriptExecutor = new SqlScriptExecutor();

                    if (Debugger.IsAttached)
                    {
                        project.Generate(sqlScriptExecutor, count, nullCount);
                    }
                    else
                    {
                        try
                        {
                            project.Generate(sqlScriptExecutor, count, nullCount);
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }
                    }
                });

                if (exception == null)
                {
                    MessageBox.Show("Done");
                }
                else
                {
                    MessageBox.Show(exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                CircularProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateControls()
        {
            if (ConnectionStringObject == null)
            {
                TextBoxConnectionString.IsEnabled = false;
                ButtonTestConnection.IsEnabled = false;
                ButtonConnectionStringForm.IsEnabled = false;
            }
            else
            {
                TextBoxConnectionString.IsEnabled = true;
                ButtonTestConnection.IsEnabled = true;
                ButtonConnectionStringForm.IsEnabled = true;
            }
        }

        private void ButtonTestConnection_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectionStringObject == null)
            {
                return;
            }

            Window window = GetWindow(this);
            ConnectionStringObject.TestConnection(window.AsWin32Window());
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string defaultConnectionString = ConnectionStringObject.GetDefaultConnectionString(SelectedDatabaseSystem, "sample");
            ConnectionStringObject = ConnectionStringObject.CreateObject(SelectedDatabaseSystem, defaultConnectionString);
        }

        private void ButtonConnectionStringForm_Click(object sender, RoutedEventArgs e)
        {
            ConnectionStringObjectForm form = new ConnectionStringObjectForm();
            form.Value = ConnectionStringObject;
            form.ShowInTaskbar = false;
            form.ShowDialog();
        }

        private void RecentComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RecentConnectionString recentConnectionString = e.AddedItems.OfType<RecentConnectionString>().FirstOrDefault();
            if (recentConnectionString == null)
                return;

            TextBoxConnectionString.ConnectionStringObject = ConnectionStringObject.CreateObject(recentConnectionString.DatabaseSystem, recentConnectionString.ConnectionString);
        }

        private void MenuItemExit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItemAbout_OnClick(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }
    }
}