using System.Threading;
using System.Windows;
using Meziantou.DataGenerator.Core.DataGenerators;

namespace Meziantou.DataGenerator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(state => ReferentialData.LoadAll());
        }
    }
}
