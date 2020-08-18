using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NoteyWriteWPF
{
    /// <summary>
    /// Interaction logic for settings.xaml
    /// </summary>
    public partial class settings : Window
    {
        int logStoreDays;

        public settings()
        {
            InitializeComponent();
            cbDoLogging.IsChecked = Properties.Settings.Default.doLogging;
            cbDeleteLogs.IsChecked = Properties.Settings.Default.autoDeleteLogs;
            logStoreDays = Properties.Settings.Default.autoDeleteLogsDays;
        }

        private void bApply_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.doLogging = (bool)cbDoLogging.IsChecked;
            Properties.Settings.Default.autoDeleteLogs = (bool)cbDeleteLogs.IsChecked;
            Properties.Settings.Default.autoDeleteLogsDays = logStoreDays;

            Properties.Settings.Default.Save();
            this.DialogResult = true;
        }

        private void textAutoDeleteTime_MouseDown(object sender, MouseButtonEventArgs e)
        {
            detailEdit detailEdit = new detailEdit();
            detailEdit.SetupMsgBox("Time to delete logs after (in days).", "Input", true);
            if (detailEdit.ShowDialog() == true)
                logStoreDays = Int32.Parse(detailEdit.text);
        }
    }
}
