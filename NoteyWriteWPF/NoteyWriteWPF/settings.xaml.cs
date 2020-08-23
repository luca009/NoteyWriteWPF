using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        customMessageBox messageBox;
        string filePathBG;

        public settings()
        {
            InitializeComponent();
            cbDoLogging.IsChecked = Properties.Settings.Default.doLogging;
            cbDeleteLogs.IsChecked = Properties.Settings.Default.autoDeleteLogs;
            logStoreDays = Properties.Settings.Default.autoDeleteLogsDays;
            switch (Properties.Settings.Default.themeName)
            {
                case "white":
                    rbThemeWhite.IsChecked = true;
                    break;
                case "blue":
                    rbThemeBlue.IsChecked = true;
                    break;
                case "green":
                    rbThemeGreen.IsChecked = true;
                    break;
            }
            filePathBG = Properties.Settings.Default.themeBG;
            if (filePathBG != "" && filePathBG != null)
            {
                try
                {
                    imgBG.Source = new BitmapImage(new Uri(filePathBG));
                    rbThemeAdaptive.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    messageBox = new customMessageBox();
                    messageBox.SetupMsgBox(ex.Message + "\nYour background image may be corrupt.", "Error!", this.FindResource("iconError"));
                    messageBox.ShowDialog();
                }
            }
            if (Properties.Settings.Default.themeName == "adaptive" && rbThemeAdaptive.IsEnabled)
                rbThemeAdaptive.IsChecked = true;
            else if (Properties.Settings.Default.themeName == "adaptive" && !rbThemeAdaptive.IsEnabled)
                rbThemeBlue.IsChecked = true;
            if (filePathBG != "" && Properties.Settings.Default.themeName != "adaptive")
                frameAdaptiveWarning.Visibility = Visibility.Visible;
        }

        private void bApply_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.doLogging = (bool)cbDoLogging.IsChecked;
            Properties.Settings.Default.autoDeleteLogs = (bool)cbDeleteLogs.IsChecked;
            Properties.Settings.Default.autoDeleteLogsDays = logStoreDays;
            if (rbThemeWhite.IsChecked == true)
                Properties.Settings.Default.themeName = "white";
            else if (rbThemeBlue.IsChecked == true)
                Properties.Settings.Default.themeName = "blue";
            else if (rbThemeGreen.IsChecked == true)
                Properties.Settings.Default.themeName = "green";
            else if (rbThemeAdaptive.IsChecked == true)
                Properties.Settings.Default.themeName = "adaptive";
            Properties.Settings.Default.themeBG = "";
            if (filePathBG != "" && filePathBG != null)
                if (File.Exists(filePathBG))
                    Properties.Settings.Default.themeBG = filePathBG;
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

        private void bSetBG_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofdOpen = new OpenFileDialog();
            ofdOpen.Filter = "Known Image Formats (*.png, *.jpg, *.jpeg, *.bmp)|*.png;*.jpg;*.jpeg;*.bmp";
            if (ofdOpen.ShowDialog() == true)
            {
                try
                {
                    imgBG.Source = new BitmapImage(new Uri(ofdOpen.FileName));
                    filePathBG = ofdOpen.FileName;
                    rbThemeAdaptive.IsEnabled = true;
                    if (rbThemeAdaptive.IsChecked == false)
                        frameAdaptiveWarning.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    messageBox = new customMessageBox();
                    messageBox.SetupMsgBox(ex.Message + "\nYour image may be corrupt.", "Error!", this.FindResource("iconError"));
                    messageBox.ShowDialog();
                }
            }

        }

        private void bRemoveBG_Click(object sender, RoutedEventArgs e)
        {
            filePathBG = "";
            rbThemeAdaptive.IsEnabled = false;
            if (rbThemeAdaptive.IsChecked == true)
                rbThemeBlue.IsChecked = true;
            imgBG.Source = null;
            frameAdaptiveWarning.Visibility = Visibility.Hidden;
        }

        private void rbThemeAdaptive_Unchecked(object sender, RoutedEventArgs e)
        {
            if (filePathBG != "")
                frameAdaptiveWarning.Visibility = Visibility.Visible;
        }

        private void rbThemeAdaptive_Checked(object sender, RoutedEventArgs e)
        {
            frameAdaptiveWarning.Visibility = Visibility.Hidden;
        }
    }
}
