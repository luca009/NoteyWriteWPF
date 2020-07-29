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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft;
using Microsoft.Win32;
using System.Windows.Media;
using System.Net;

namespace NoteyWriteWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Define Variables
        public string currentVersion = "Alpha 0.3";
        public SaveFileDialog sfdSave = new SaveFileDialog();
        public OpenFileDialog ofdOpen = new OpenFileDialog();
        public string currentlyOpenPath;
        private bool unsavedChanges = false;

        public MainWindow()
        {
            // Initialize
            InitializeComponent();
            mainWindow.Title = "NoteyWriteWPF " + currentVersion;
            sfdSave.Filter = "Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt";
            sfdSave.Title = "Save a document | NoteyWriteWPF";
            ofdOpen.Filter = "Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt";
            ofdOpen.Title = "Open a document | NoteyWriteWPF";

            List<string> fonts = new List<string>();
            foreach (System.Drawing.FontFamily font in System.Drawing.FontFamily.Families)
            {
                fonts.Add(font.Name);
            }

            cbFont.ItemsSource = fonts;
            cbFontSize.ItemsSource = new List<Double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72, 102, 144, 288 };
        }

        public bool openDocument(string filePath, RichTextBox rtbLoad, Boolean append = false)
        {
            string fileExtension = System.IO.Path.GetExtension(filePath);

            if (!append)
            { rtbLoad.SelectAll(); }
            rtbLoad.SelectAll();
            if (fileExtension == ".rtf")
            { rtbLoad.Selection.Load(new FileStream(filePath, FileMode.Open), DataFormats.Rtf); }
            else if (fileExtension == ".txt")
            { rtbLoad.Selection.Load(new FileStream(filePath, FileMode.Open), DataFormats.UnicodeText); }
            else
            { rtbLoad.Selection.Load(new FileStream(filePath, FileMode.Open), DataFormats.UnicodeText); }

            unsavedChanges = false;
            return true;
        }

        public bool saveDocument(string filePath, RichTextBox rtbSave)
        {
            string fileExtension = System.IO.Path.GetExtension(filePath);
            FileStream stream;
            rtbSave.SelectAll();

            if (fileExtension == ".rtf")
            { rtbSave.Selection.Save(new FileStream(sfdSave.FileName, FileMode.Create), DataFormats.Rtf); }
            else if (fileExtension == ".txt")
            { rtbSave.Selection.Save(stream = new FileStream(sfdSave.FileName, FileMode.Create), DataFormats.Text); }
            else
            { rtbSave.Selection.Save(stream = new FileStream(sfdSave.FileName, FileMode.Create), DataFormats.Text); }

            unsavedChanges = false;
            currentlyOpenPath = filePath;
            //while (stream Length != stream.Position){}

            return true;
        }

        public bool saveDocumentFromRawRtf(string filePath, string rawRtf)
        {
            string fileExtension = System.IO.Path.GetExtension(filePath);
            //FileStream stream;

            if (fileExtension == ".rtf")
            { System.IO.File.WriteAllText(filePath, rawRtf); }
            else if (fileExtension == ".txt")
            { 
                nwDebug.nwError("File type .txt not supported using this function. File will be saved as .rtf. Later use NoteyWrite to convert it.", 2);
                System.IO.File.WriteAllText(filePath + ".rtf", rawRtf);
            }
            else
            {
                nwDebug.nwError("File type " + fileExtension + " not supported using this function. File will be saved as .rtf. Later use NoteyWrite to convert it.", 2);
                System.IO.File.WriteAllText(filePath + ".rtf", rawRtf); 
            }

            unsavedChanges = false;

            //while (stream Length != stream.Position){}

            return true;
        }

        public void checkForAlignment()
        {
            if (rtbMain.Selection.Start.Paragraph != null)
            {
                miAlignLeft.IsChecked = (rtbMain.Selection.Start.Paragraph.TextAlignment == TextAlignment.Left);
                miAlignCenter.IsChecked = (rtbMain.Selection.Start.Paragraph.TextAlignment == TextAlignment.Center);
                miAlignRight.IsChecked = (rtbMain.Selection.Start.Paragraph.TextAlignment == TextAlignment.Right);
                miAlignJustify.IsChecked = (rtbMain.Selection.Start.Paragraph.TextAlignment == TextAlignment.Justify);
            }
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            byte v = Convert.ToByte(value);
            byte p = Convert.ToByte(value * (1 - saturation));
            byte q = Convert.ToByte(value * (1 - f * saturation));
            byte t = Convert.ToByte(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        internal void selectFromIndex(RichTextBox rtb, int index, int length)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);

            if (textRange.Text.Length >= (index + length))
            {
                TextPointer start = textRange.Start.GetPositionAtOffset(index, LogicalDirection.Forward);
                TextPointer end = textRange.Start.GetPositionAtOffset(index + length, LogicalDirection.Backward);
                rtb.Selection.Select(start, end);
            }
            return;
        }

        private void anyNew_Click(object sender, RoutedEventArgs e)
        {
            rtbMain.SelectAll();
            rtbMain.Selection.Text = "";
            unsavedChanges = false;
        }

        private void anyOpen_Click(object sender, RoutedEventArgs e)
        {
            Nullable<bool> result = ofdOpen.ShowDialog();
            if (result == true)
            {
                openDocument(ofdOpen.FileName, rtbMain);
                currentlyOpenPath = ofdOpen.FileName;
            }
        }

        private void anySave_Click(object sender, RoutedEventArgs e)
        {
            if (currentlyOpenPath == null)
            {
                anySaveAs_Click(sender, e);
            } else {
                try
                {
                    saveDocument(currentlyOpenPath, rtbMain);
                }
                catch (Exception ex)
                {
                    nwDebug.nwError(ex.Message);
                    throw;
                }
            }
        }

        private void anySaveAs_Click(object sender, RoutedEventArgs e)
        {
            Nullable<bool> result = sfdSave.ShowDialog();
            if (result == true)
            {
                saveDocument(sfdSave.FileName, rtbMain);
            }
        }

        private void anyExit_Click(object sender, RoutedEventArgs e)
        {
            if (unsavedChanges)
            {
                TextRange tr = new TextRange(rtbMain.Document.ContentStart, rtbMain.Document.ContentEnd);
                MemoryStream ms = new MemoryStream();
                tr.Save(ms, DataFormats.Rtf);
                string rawRtf = UnicodeEncoding.Default.GetString(ms.ToArray());

                exit exit = new exit();
                rtbMain.SelectAll();
                exit.getArguments(currentlyOpenPath, rawRtf);
                exit.Show();
            } else {
                Application.Current.Shutdown();
            }
        }

        private void rtbMain_TextChanged(object sender, TextChangedEventArgs e)
        {
            unsavedChanges = true;
        }

        private void rtbMain_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = rtbMain.Selection.GetPropertyValue(Inline.FontWeightProperty);
            miBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            temp = rtbMain.Selection.GetPropertyValue(Inline.FontStyleProperty);
            miItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            temp = rtbMain.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            miUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            checkForAlignment();

            temp = rtbMain.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            cbFont.Text = temp.ToString();
            temp = rtbMain.Selection.GetPropertyValue(Inline.FontSizeProperty);
            cbFontSize.Text = temp.ToString();
        }

        private void anyFormat_Click(object sender, RoutedEventArgs e)
        {
            rtbMain.Focus();
            checkForAlignment();
        }

        private void anyFormat_MouseDown(object sender, MouseButtonEventArgs e)
        {
            rtbMain.Focus();
            checkForAlignment();
        }

        private void anyFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbFontSize.SelectedItem != null)
                rtbMain.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cbFontSize.SelectedItem);
            rtbMain.Focus();
        }

        private void mainWindow_Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (unsavedChanges)
            {
                e.Cancel = true;
                TextRange tr = new TextRange(rtbMain.Document.ContentStart, rtbMain.Document.ContentEnd);
                MemoryStream ms = new MemoryStream();
                tr.Save(ms, DataFormats.Rtf);
                string rawRtf = ASCIIEncoding.Default.GetString(ms.ToArray());

                exit exit = new exit();
                rtbMain.SelectAll();
                exit.getArguments(currentlyOpenPath, rawRtf);
                exit.Show();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void anyFont_DropDownClosed(object sender, EventArgs e)
        {
            if (cbFont.SelectedItem != null)
                rtbMain.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cbFont.SelectedItem);
            rtbMain.Focus();
        }

        private void anyFontSize_DropDownClosed(object sender, EventArgs e)
        {
            if (cbFontSize.SelectedItem != null)
            {
                rtbMain.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cbFontSize.SelectedItem);
            }
            rtbMain.Focus();
        }

        private void anyUndo_Click(object sender, RoutedEventArgs e)
        {
            if (rtbMain.CanUndo)
                rtbMain.Undo();
        }

        private void anyRedo_Click(object sender, RoutedEventArgs e)
        {
            if (rtbMain.CanRedo)
                rtbMain.Redo();
        }

        private void miEdit_Focus(object sender, RoutedEventArgs e)
        {
            miUndo.IsEnabled = false;
            miRedo.IsEnabled = false;
            if (rtbMain.CanUndo)
                miUndo.IsEnabled = true;
            if (rtbMain.CanRedo)
                miRedo.IsEnabled = true;
        }

        private void miFindReplace_Click(object sender, RoutedEventArgs e)
        {
            find find = new find();
            string comparisonString = "test";
            if (find.ShowDialog() == true)
            {
                rtbMain.SelectAll();
                if (rtbMain.Selection.Text.IndexOf(comparisonString, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    //rtbMain.Selection.Text.Substring(rtbMain.Selection.Text.IndexOf(comparisonString, StringComparison.OrdinalIgnoreCase), comparisonString.Length);
                    selectFromIndex(rtbMain, rtbMain.Selection.Text.IndexOf(comparisonString, StringComparison.OrdinalIgnoreCase), comparisonString.Length);
                }
            }
        }

        private void miForeground_Click(object sender, RoutedEventArgs e)
        {
            ColorPicker colorPicker = new ColorPicker();
            if (colorPicker.ShowDialog() == true)
            {
                double hue = Convert.ToByte(colorPicker.slHue.Value);
                double saturation = Convert.ToByte(colorPicker.slSaturation.Value);
                double value = Convert.ToByte(colorPicker.slValue.Value);
                Color color = new Color();

                color = ColorFromHSV(hue, saturation / 100, value / 100);

                rtbMain.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(color));
            }
        }

        private void miBackground_Click(object sender, RoutedEventArgs e)
        {
            ColorPicker colorPicker = new ColorPicker();
            if (colorPicker.ShowDialog() == true)
            {
                double hue = Convert.ToByte(colorPicker.slHue.Value);
                double saturation = Convert.ToByte(colorPicker.slSaturation.Value);
                double value = Convert.ToByte(colorPicker.slValue.Value);
                Color color = new Color();

                color = ColorFromHSV(hue, saturation / 100, value / 100);
                
                rtbMain.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(color));
            }
        }
    }
}
