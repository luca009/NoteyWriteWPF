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
using Microsoft.Win32;
using NoteyWriteWPF;
using System.IO;

namespace NoteyWriteWPF
{
    /// <summary>
    /// Interaction logic for performanceModeOld.xaml
    /// </summary>
    public partial class performanceModeOld : Window
    {
        OpenFileDialog openFileDialog;
        SaveFileDialog saveFileDialog;
        public string currentlyOpenFile;

        public performanceModeOld()
        {
            InitializeComponent();
        }

        public void openDocument(string filePath)
        {
            try
            {
                string fileExtension = System.IO.Path.GetExtension(filePath);
                if (fileExtension != ".txt" && fileExtension != ".txt")
                {
                    //rtbMain.Rtf = new FileStream(filePath, FileMode.Open);
                    rtbMain.LoadFile(new FileStream(filePath, FileMode.Open), System.Windows.Forms.RichTextBoxStreamType.PlainText);
                    currentlyOpenFile = filePath;
                    return;
                }
                rtbMain.LoadFile(filePath);
                currentlyOpenFile = filePath;
            }
            catch (Exception ex)
            {
                nwDebug.nwError(ex.Message);
            }
        }

        private void miSave_Click(object sender, RoutedEventArgs e)
        {
            if (currentlyOpenFile == "")
            {
                saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.Title = "Save a document | NoteyWriteWPF";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        rtbMain.SaveFile(saveFileDialog.FileName);
                        currentlyOpenFile = saveFileDialog.FileName;
                    }
                    catch (Exception ex)
                    {
                        nwDebug.nwError(ex.Message);
                    }
                }
            }
            try
            {
                rtbMain.SaveFile(currentlyOpenFile);
            }
            catch (Exception ex)
            {
                nwDebug.nwError(ex.Message);
            }
        }

        private void miOpen_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.Title = "Open a document | NoteyWriteWPF";
            if (openFileDialog.ShowDialog() == true)
                openDocument(openFileDialog.FileName);
        }

        private void miSaveAs_Click(object sender, RoutedEventArgs e)
        {
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.Title = "Save a document | NoteyWriteWPF";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    rtbMain.SaveFile(saveFileDialog.FileName);
                    currentlyOpenFile = saveFileDialog.FileName;
                }
                catch (Exception ex)
                {
                    nwDebug.nwError(ex.Message);
                }
            }
        }

        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
