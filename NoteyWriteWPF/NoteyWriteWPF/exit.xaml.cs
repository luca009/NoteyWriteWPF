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
    /// Interaction logic for exit.xaml
    /// </summary>
    public partial class exit : Window
    {
        private string mWCurrentlyOpenFile;
        private string mWRawRtf;

        public void getArguments(string currentlyOpenPath, string rawRtf)
        {
            mWCurrentlyOpenFile = currentlyOpenPath;
            mWRawRtf = rawRtf;
        }

        public exit()
        {
            InitializeComponent();
        }

        private void bExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            if (mWCurrentlyOpenFile == null)
            {
                Nullable<bool> result = mainWindow.sfdSave.ShowDialog();
                if (result == true)
                {
                    if (mainWindow.saveDocumentFromRawRtf(mainWindow.sfdSave.FileName, mWRawRtf))
                    {
                        Application.Current.Shutdown();
                    }
                }
            }
            else
            {
                try
                {
                    mainWindow.saveDocumentFromRawRtf(mWCurrentlyOpenFile, mWRawRtf);
                }
                catch (Exception ex)
                {
                    nwDebug.nwError(ex.ToString());
                    throw;
                }
                Application.Current.Shutdown();
            }
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
