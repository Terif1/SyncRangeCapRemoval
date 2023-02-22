using System.Windows;
using System.Windows.Controls;

namespace SyncRangeCapRemoval
{
    public partial class SyncRangeCapRemovalControl : UserControl
    {

        private SyncRangeCapRemoval Plugin { get; }

        private SyncRangeCapRemovalControl()
        {
            InitializeComponent();
        }

        public SyncRangeCapRemovalControl(SyncRangeCapRemoval plugin) : this()
        {
            Plugin = plugin;
            DataContext = plugin.Config;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Plugin.Save();
        }
    }
}
