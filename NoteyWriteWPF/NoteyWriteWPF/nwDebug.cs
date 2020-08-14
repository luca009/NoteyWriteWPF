using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace NoteyWriteWPF
{
    public class nwDebug
    {
        /// <summary>
        /// Shows MessageBox with content of info + extra details.
        /// level is 0 by default.
        /// level = 0 = Error, level = 1 = Info, level = 2 = Warning, level = 3 = Only info
        /// </summary>
        public static bool nwError(string info, int level = 0)
        {
            switch (level)
            {
                case 0:
                    //MessageBox.Show("Error!\nDescription: " + info + "\nNoteyWrite will attempt to continue operating.", "Error - NoteyWrite", MessageBoxButton.OK, MessageBoxImage.Error);
                    //new customMessageBox().ShowDialog("Error!\nDescription: " + info + "\nNoteyWrite will attempt to continue operating.", "Error - NoteyWrite", "{StaticResource iconError}");
                    break;
                case 1:
                    MessageBox.Show("Information: " + info, "Information - NoteyWrite", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 2:
                    MessageBox.Show("Warning: " + info + "\nNoteyWrite should continue normal operation.", "Warning - NoteyWrite", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case 3:
                    MessageBox.Show(info, "NoteyWrite", MessageBoxButton.OK);
                    break;
            }
            return true;
        }

        /// <summary>
        /// Logs info with extra informnation to a text file. Work in progress.
        /// level is 0 by default.
        /// level = 0 = Error, level = 1 = Info, level = 2 = Warning
        /// </summary>
        public static bool nwLog(string info, int level = 0, string filePath = "/log.txt", string special = null)
        {
            if (!File.Exists(filePath))
                File.AppendAllText(filePath, DateTime.UtcNow.ToString() + "UTC Start of new log file\n");
            switch (level)
            {
                case 0:
                    File.AppendAllText(filePath, DateTime.UtcNow.ToString() + "UTC Error: " + info + "\n");
                    break;
                case 1:
                    File.AppendAllText(filePath, DateTime.UtcNow.ToString() + "UTC Info: " + info + "\n");
                    break;
                case 2:
                    File.AppendAllText(filePath, DateTime.UtcNow.ToString() + "UTC Warning: " + info + "\n");
                    break;
                default:
                    File.AppendAllText(filePath, DateTime.UtcNow.ToString() + "UTC Unknown Level! " + info + "\n");
                    break;
            }
            return true;
        }
    }
}
