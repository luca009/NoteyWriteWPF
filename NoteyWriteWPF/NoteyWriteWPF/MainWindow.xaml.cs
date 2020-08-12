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
using System.Windows.Media.Animation;
using System.Windows.Markup;

namespace NoteyWriteWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Define Variables
        private Storyboard animationStoryboard;
        public string currentVersion = "Alpha 0.4.1.1";
        public SaveFileDialog sfdSave = new SaveFileDialog();
        public OpenFileDialog ofdOpen = new OpenFileDialog();
        public string currentlyOpenPath;
        private bool unsavedChanges = false;
        private string[] arguments;
        private int performanceModeMinSize = 6144;
        customMessageBox messageBox = new customMessageBox();

        public MainWindow()
        {
            // Initialize
            InitializeComponent();
            arguments = getArguments();
            if (arguments.Length > 1)
            {
                if (File.Exists(arguments[1]))
                {
                    long length = new FileInfo(arguments[1]).Length;
                    if (length > performanceModeMinSize)
                    {
                        switch (MessageBox.Show("Opening this file may cause performance issues. Do you wish to open it using Performance Mode?", "NoteyWrite", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning))
                        {
                            case MessageBoxResult.Yes:
                                performanceMode performanceMode = new performanceMode();
                                performanceMode.openDocument(arguments[1]);
                                performanceMode.Show();
                                return;
                            case MessageBoxResult.No:
                                break;
                            case MessageBoxResult.Cancel:
                                return;
                        }
                    }
                    openDocument(arguments[1], rtbMain);
                    currentlyOpenPath = arguments[1];
                }
            }

            mainWindow.Title = "NoteyWriteWPF " + currentVersion;
            sfdSave.Filter = "Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt|All files (*.*)|*.*";
            sfdSave.Title = "Save a document | NoteyWriteWPF";
            ofdOpen.Filter = "Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt|All files (*.*)|*.*";
            ofdOpen.Title = "Open a document | NoteyWriteWPF";

            List<string> fonts = new List<string>();
            foreach (System.Drawing.FontFamily font in System.Drawing.FontFamily.Families)
            {
                fonts.Add(font.Name);
            }

            cbFont.ItemsSource = fonts;
            cbFontSize.ItemsSource = new List<Double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72, 102, 144, 288 };

            //Override the default RichTextBox Events
            rtbMain.AddHandler(RichTextBox.DragOverEvent, new DragEventHandler(rtbMain_DragOver), true);
            rtbMain.AddHandler(RichTextBox.DropEvent, new DragEventHandler(rtbMain_Drop), true);
            rtbMain.AddHandler(RichTextBox.DragEnterEvent, new DragEventHandler(rtbMain_DragEnter), true);
            rtbMain.AddHandler(RichTextBox.DragLeaveEvent, new DragEventHandler(rtbMain_DragLeave), true);
        }

        private string[] getArguments()
        {
            string[] arguments = null;
            arguments = Environment.GetCommandLineArgs();
            if (arguments != null)
                return arguments;
            else
                return null;
        }

        public bool openDocument(string filePath, RichTextBox rtbLoad, Boolean append = false)
        {
            //Uses a FileStream to open the contents of a file to the RichTextBox
            string fileExtension = System.IO.Path.GetExtension(filePath);

            if (!append)
                rtbLoad.SelectAll();
            if (fileExtension == ".rtf")
                rtbLoad.Selection.Load(new FileStream(filePath, FileMode.Open), DataFormats.Rtf);
            else if (fileExtension == ".txt")
                rtbLoad.Selection.Load(new FileStream(filePath, FileMode.Open), DataFormats.Text);
            else
                rtbLoad.Selection.Load(new FileStream(filePath, FileMode.Open), DataFormats.Text);

            unsavedChanges = false;
            return true;
        }

        public bool saveDocument(string filePath, RichTextBox rtbSave)
        {
            //Uses a FileStream to save the contents of the RichTextBox
            string fileExtension = System.IO.Path.GetExtension(filePath);
            FileStream stream;
            rtbSave.SelectAll();

            if (fileExtension == ".rtf")
                rtbSave.Selection.Save(new FileStream(filePath, FileMode.Create), DataFormats.Rtf);
            else if (fileExtension == ".txt")
                rtbSave.Selection.Save(stream = new FileStream(filePath, FileMode.Create), DataFormats.Text);
            else
                rtbSave.Selection.Save(stream = new FileStream(filePath, FileMode.Create), DataFormats.Text);

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
            //Math to convert HSV to System.Windows.Media.Color
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
            //Select using an index and length
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);

            if (textRange.Text.Length >= (index + length))
            {
                TextPointer start = textRange.Start.GetPositionAtOffset(index, LogicalDirection.Forward);
                TextPointer end = textRange.Start.GetPositionAtOffset(index + length, LogicalDirection.Backward);
                textRange = new TextRange(start, end);
                rtb.Selection.Select(start, end);
            }
            return;
        }

        private void animate(DoubleAnimation animation, PropertyPath propertyPath, Storyboard storyboard, string objectName)
        {
            storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            Storyboard.SetTargetName(animation, objectName);
            Storyboard.SetTargetProperty(animation, propertyPath);
            storyboard.Begin(this);
        }

        private void anyNew_Click(object sender, RoutedEventArgs e)
        {
            rtbMain.SelectAll();
            rtbMain.Selection.Text = "";
            unsavedChanges = false;
            currentlyOpenPath = "";
        }

        private void anyOpen_Click(object sender, RoutedEventArgs e)
        {
            Nullable<bool> result = ofdOpen.ShowDialog();
            if (result == true)
            {
                long length = new FileInfo(ofdOpen.FileName).Length;
                if (length > performanceModeMinSize)
                {
                    switch (MessageBox.Show("Opening this file may cause performance issues. Do you wish to open it using Performance Mode?", "NoteyWrite", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning))
                    {
                        case MessageBoxResult.Yes:
                            performanceMode performanceMode = new performanceMode();
                            performanceMode.openDocument(ofdOpen.FileName);
                            performanceMode.Show();
                            return;
                        case MessageBoxResult.No:
                            break;
                        case MessageBoxResult.Cancel:
                            return;
                    }
                } 
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
            //If there are unsaved changes, the "Unsaved Changes" dialog openes, if not, close the application
            if (unsavedChanges)
            {
                exit exit = new exit();
                exit.mWRichTextBox = XamlReader.Parse(XamlWriter.Save(rtbMain)) as RichTextBox;
                exit.mWCurrentlyOpenFile = currentlyOpenPath;
                if (exit.ShowDialog() == true)
                    return;
                Application.Current.Shutdown();
            }
            else
                Application.Current.Shutdown();
        }

        private void rtbMain_TextChanged(object sender, TextChangedEventArgs e)
        {
            unsavedChanges = true;
        }

        private void rtbMain_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //Change the formatting options accordingly to the selected text
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
            //If there are unsaved changes, the "Unsaved Changes" dialog openes, if not, close the application
            if (unsavedChanges)
            {
                exit exit = new exit();
                exit.mWRichTextBox = XamlReader.Parse(XamlWriter.Save(rtbMain)) as RichTextBox;
                exit.mWCurrentlyOpenFile = currentlyOpenPath;
                if (exit.ShowDialog() == true)
                    e.Cancel = true;
                Application.Current.Shutdown();
            }
            else
                Application.Current.Shutdown();
        }

        private void anyFont_DropDownClosed(object sender, EventArgs e)
        {
            //Apply the selected font family
            if (cbFont.SelectedItem != null)
                rtbMain.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cbFont.SelectedItem);
            rtbMain.Focus();
        }

        private void anyFontSize_DropDownClosed(object sender, EventArgs e)
        {
            //Apply the selected font size
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
            //Enable/Disable Undo/Redo buttons
            miUndo.IsEnabled = false;
            miRedo.IsEnabled = false;
            if (rtbMain.CanUndo)
                miUndo.IsEnabled = true;
            if (rtbMain.CanRedo)
                miRedo.IsEnabled = true;
        }

        private void miFindReplace_Click(object sender, RoutedEventArgs e)
        {
            int nextIndex = 0;
            int lastIndex = -1;
            find find = new find();
            string comparisonString = "";
            rtbMain.SelectAll();
            find.richTextBox = XamlReader.Parse(XamlWriter.Save(rtbMain)) as RichTextBox;
            bool result = true;
            while (result)
            {
                result = (bool)find.ShowDialog();
                //rtbMain.Selection.Select(find.textRange.Start, find.textRange.End);
                comparisonString = find.tbFind.Text;
                find.searchString = rtbMain.Selection.Text;
                comparisonString = find.tbFind.Text;
                rtbMain.SelectAll();
                lastIndex = rtbMain.Selection.Text.IndexOf(comparisonString, nextIndex, StringComparison.OrdinalIgnoreCase);
               if (nextIndex != -1)
               {
                    //rtbMain.Selection.Text.Substring(rtbMain.Selection.Text.IndexOf(comparisonString, StringComparison.OrdinalIgnoreCase), comparisonString.Length);
                    selectFromIndex(rtbMain, lastIndex, comparisonString.Length);
                    nextIndex = lastIndex + comparisonString.Length;
                    rtbMain.Focus();
               }
            }

            /*comparisonString = find.tbFind.Text;
            find.searchString = rtbMain.Selection.Text;
            if (find.ShowDialog() == true)
            {
                comparisonString = find.tbFind.Text;
                rtbMain.SelectAll();
                lastIndex = rtbMain.Selection.Text.IndexOf(comparisonString, nextIndex, StringComparison.OrdinalIgnoreCase);
                if (nextIndex != -1)
                {
                    //rtbMain.Selection.Text.Substring(rtbMain.Selection.Text.IndexOf(comparisonString, StringComparison.OrdinalIgnoreCase), comparisonString.Length);
                    selectFromIndex(rtbMain, lastIndex, comparisonString.Length);
                    nextIndex = lastIndex + comparisonString.Length;
                }
            }*/
        }

        private void miForeground_Click(object sender, RoutedEventArgs e)
        {
            //Open the ColorPicker
            ColorPicker colorPicker = new ColorPicker();
            if (colorPicker.ShowDialog() == true)
            {
                //Get Values from the Color Picker, convert to RGB and apply it as the text background
                double hue = Convert.ToByte(colorPicker.slHue.Value);
                double saturation = Convert.ToByte(colorPicker.slSaturation.Value);
                double value = Convert.ToByte(colorPicker.slValue.Value);
                Color color = new Color();

                color = ColorFromHSV(hue, saturation / 100, value / 100);

                rtbMain.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
            }
        }

        private void miBackground_Click(object sender, RoutedEventArgs e)
        {
            //Open the ColorPicker
            ColorPicker colorPicker = new ColorPicker();
            if (colorPicker.ShowDialog() == true)
            {
                //Get Values from the Color Picker, convert to RGB and apply it as the text background
                double hue = Convert.ToByte(colorPicker.slHue.Value);
                double saturation = Convert.ToByte(colorPicker.slSaturation.Value);
                double value = Convert.ToByte(colorPicker.slValue.Value);
                Color color = new Color();

                color = ColorFromHSV(hue, saturation / 100, value / 100);
                
                rtbMain.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(color));
            }
        }

        private void miFormattingBar_Checked(object sender, RoutedEventArgs e)
        {
            if (tbFormatting == null || rtbMain == null)
                return;

            //Animate the Opacity & Margin of the formatting bar & RichTextBox
            DoubleAnimation fadeIn = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(0.8)));
            ThicknessAnimation increaseMargin = new ThicknessAnimation(new Thickness(0, 56, 0, 0), new Duration(TimeSpan.FromSeconds(0.5)));
            animationStoryboard = new Storyboard();
            animationStoryboard.Children.Add(fadeIn);
            animationStoryboard.Children.Add(increaseMargin);
            Storyboard.SetTargetName(fadeIn, tbFormatting.Name);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(ToolBar.OpacityProperty));
            Storyboard.SetTargetName(increaseMargin, rtbMain.Name);
            Storyboard.SetTargetProperty(increaseMargin, new PropertyPath(RichTextBox.MarginProperty));
            animationStoryboard.Begin(this);
            tbFormatting.IsHitTestVisible = true;
        }

        private void miFormattingBar_Unchecked(object sender, RoutedEventArgs e)
        {
            if (tbFormatting == null || rtbMain == null)
                return;

            //Animate the Opacity & Margin of the formatting bar & RichTextBox
            DoubleAnimation fadeOut = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromSeconds(0.8)));
            ThicknessAnimation decreaseMargin = new ThicknessAnimation(new Thickness(0, 26, 0, 0), new Duration(TimeSpan.FromSeconds(0.8)));
            animationStoryboard = new Storyboard();
            animationStoryboard.Children.Add(fadeOut);
            animationStoryboard.Children.Add(decreaseMargin);
            Storyboard.SetTargetName(fadeOut, tbFormatting.Name);
            Storyboard.SetTargetProperty(fadeOut, new PropertyPath(ToolBar.OpacityProperty)); 
            Storyboard.SetTargetName(decreaseMargin, rtbMain.Name);
            Storyboard.SetTargetProperty(decreaseMargin, new PropertyPath(RichTextBox.MarginProperty));
            animationStoryboard.Begin(this);
            tbFormatting.IsHitTestVisible = false;
        }

        private void rtbMain_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = false;
        }

        private void rtbMain_Drop(object sender, DragEventArgs e)
        {
            //Check if the dropped data is a file to not override the existing functionality
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                animate(new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.25))), new PropertyPath(RichTextBox.OpacityProperty), animationStoryboard, canvasDragDrop.Name);
                //Until multiple documents can be opened, only open the first one
                long length = new FileInfo(files[0]).Length;
                if (length > performanceModeMinSize)
                {
                    switch (MessageBox.Show("Opening this file may cause performance issues. Do you wish to open it using Performance Mode?", "NoteyWrite", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning))
                    {
                        case MessageBoxResult.Yes:
                            performanceMode performanceMode = new performanceMode();
                            performanceMode.openDocument(files[0]);
                            performanceMode.Show();
                            return;
                        case MessageBoxResult.No:
                            break;
                        case MessageBoxResult.Cancel:
                            return;
                    }
                }
                openDocument(files[0], rtbMain);
            }
        }

        private void rtbMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                canvasDragDrop.Visibility = Visibility.Visible;
                animate(new DoubleAnimation(0.5, new Duration(TimeSpan.FromSeconds(0.25))), new PropertyPath(RichTextBox.OpacityProperty), animationStoryboard, canvasDragDrop.Name);
            }
        }

        private void rtbMain_DragLeave(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) 
                animate(new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.25))), new PropertyPath(RichTextBox.OpacityProperty), animationStoryboard, canvasDragDrop.Name);
        }

        private void miPerformanceMode_Click(object sender, RoutedEventArgs e)
        {
            performanceMode performanceMode = new performanceMode();
            performanceMode.Show();
        }

        private void miError_Click(object sender, RoutedEventArgs e)
        {
            messageBox = new customMessageBox();
            messageBox.SetupMsgBox("This is the description.", "This is a title.", this.FindResource("iconError"), "retrun true", "return false");
            messageBox.ShowDialog();
        }
    }
}
