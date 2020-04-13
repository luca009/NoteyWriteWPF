using System;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft;
using Microsoft.Win32;

namespace NoteyWriteWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string currentVersion = "Alpha 0.1";
        SaveFileDialog sfdSave = new SaveFileDialog();
        OpenFileDialog ofdOpen = new OpenFileDialog();

        public MainWindow()
        {
            InitializeComponent();
            mainWindow.Title = "NoteyWriteWPF " + currentVersion;
            sfdSave.Filter = "Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt";
            sfdSave.Title = "Save a document | NoteyWriteWPF";
            ofdOpen.Filter = "Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt";
            ofdOpen.Title = "Save a document | NoteyWriteWPF";
        }

        public void openDocument(string filePath, RichTextBox rtbLoad, Boolean append = false)
        {
            string fileExtension = System.IO.Path.GetExtension(filePath);

            if (!append)
            { rtbLoad.SelectAll(); }

            if (fileExtension == ".rtf")
            { rtbLoad.Selection.Load(new FileStream(filePath, FileMode.Open), DataFormats.Rtf); }
            else if (fileExtension == ".txt")
            { rtbLoad.Selection.Load(new FileStream(filePath, FileMode.Open), DataFormats.UnicodeText); }
            else
            { rtbLoad.Selection.Load(new FileStream(filePath, FileMode.Open), DataFormats.UnicodeText); }
        }

        public void saveDocument(string filePath, RichTextBox rtbSave)
        {
            string fileExtension = System.IO.Path.GetExtension(filePath);
            rtbSave.SelectAll();

            if (fileExtension == ".rtf")
            { rtbSave.Selection.Save(new FileStream(sfdSave.FileName, FileMode.Create), DataFormats.Rtf); }
            else if (fileExtension == ".txt")
            { rtbSave.Selection.Save(new FileStream(sfdSave.FileName, FileMode.Create), DataFormats.UnicodeText); }
            else
            { rtbSave.Selection.Save(new FileStream(sfdSave.FileName, FileMode.Create), DataFormats.UnicodeText); }
        }

        private void anySaveAs_Click(object sender, RoutedEventArgs e)
        {
            Nullable<bool> result = sfdSave.ShowDialog();
            if (result == true)
            {
                saveDocument(sfdSave.FileName, rtbMain);
            }
        }

        private void anyOpen_Click(object sender, RoutedEventArgs e)
        {
            Nullable<bool> result = ofdOpen.ShowDialog();
            if (result == true)
            {
                openDocument(ofdOpen.FileName, rtbMain);
            }
        }
    }
}
