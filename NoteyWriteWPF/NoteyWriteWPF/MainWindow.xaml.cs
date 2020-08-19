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
using Microsoft;
using Microsoft.Win32;
using System.Windows.Media;
using System.Net;
using System.Windows.Media.Animation;
using System.Windows.Markup;
using System.Xml;

namespace NoteyWriteWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Define Variables
        private Storyboard animationStoryboard;
        public string currentVersion = "Alpha 0.4.2";
        public SaveFileDialog sfdSave = new SaveFileDialog();
        public OpenFileDialog ofdOpen = new OpenFileDialog();
        public string currentlyOpenPath;
        private bool unsavedChanges = false;
        private string[] arguments;
        private int performanceModeMinSize = 6144;
        customMessageBox messageBox = new customMessageBox();
        string errorPreset = "\nNoteyWrite will attempt to continue normal operation.";
        string logFile;
        bool canDrop = true;

        public MainWindow()
        {
            // Initialize
            InitializeComponent();
            applySettings();
            //logFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            nwDebug.nwLog("NoteyWrite started.", 1, logFile);
            arguments = getArguments();
            if (arguments.Length > 1)
            {
                nwDebug.nwLog("Opening from argument.", 0, logFile);
                if (File.Exists(arguments[1]))
                {
                    nwDebug.nwLog("Argument file exists.", 1, logFile);
                    try
                    {
                        long length = new FileInfo(arguments[1]).Length;
                        if (length > performanceModeMinSize)
                        {
                            nwDebug.nwLog("File is above performance treshold.", 1, logFile);
                            messageBox = new customMessageBox();
                            messageBox.SetupMsgBox("Opening this file may cause performance issues. Do you wish to open it using Performance Mode?", "Performance Mode", this.FindResource("iconWarning"), "Yes", "No", "Cancel");
                            messageBox.ShowDialog();
                            switch (messageBox.result.ToString())
                            {
                                case "Yes":
                                    nwDebug.nwLog("Attempting to open file in Performance Mode.", 1, logFile);
                                    performanceMode performanceMode = new performanceMode();
                                    performanceMode.openDocument(arguments[1]);
                                    performanceMode.Show();
                                    return;
                                case "No":
                                    break;
                                case "Cancel":
                                    return;
                            }
                        }
                        openDocument(arguments[1], rtbMain);
                        currentlyOpenPath = arguments[1];
                    }
                    catch (Exception ex)
                    {
                        nwDebug.nwLog("Exception " + ex.Message, 0, logFile);
                        messageBox = new customMessageBox();
                        messageBox.SetupMsgBox(ex.Message + errorPreset, "Error!", this.FindResource("iconError"));
                        messageBox.ShowDialog();
                    }
                }
            }

            mainWindow.Title = "NoteyWriteWPF " + currentVersion;
            sfdSave.Filter = "XML Document (*.xml)|*.xml|Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt|All files (*.*)|*.*";
            sfdSave.Title = "Save a document | NoteyWriteWPF";
            ofdOpen.Filter = "XML Document (*.xml)|*.xml|Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt|All files (*.*)|*.*";
            ofdOpen.Title = "Open a document | NoteyWriteWPF";

            List<string> fonts = new List<string>();
            foreach (System.Drawing.FontFamily font in System.Drawing.FontFamily.Families)
            {
                fonts.Add(font.Name);
            }
            nwDebug.nwLog("Added all fonts.", 1, logFile);

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
            else if (fileExtension == ".xml")
            {
                XmlReader xmlReader = XmlReader.Create(filePath);
                XamlReader xamlReader = new XamlReader();
                rtbLoad.Document = new FlowDocument();
                rtbLoad.Document = (FlowDocument)(XamlReader.Load(xmlReader));
            }
            else
                rtbLoad.Selection.Load(new FileStream(filePath, FileMode.Open), DataFormats.Text);

            nwDebug.nwLog("Opened File.", 1, logFile);
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
            else if (fileExtension == ".xml")
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(XamlWriter.Save(rtbSave.Document));
                xdoc.Save(filePath);
            }
            else
                rtbSave.Selection.Save(stream = new FileStream(filePath, FileMode.Create), DataFormats.Text);

            nwDebug.nwLog("Saved File.", 1, logFile);
            unsavedChanges = false;
            currentlyOpenPath = filePath;
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

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void applySettings()
        {
            string filePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
            if (Properties.Settings.Default.doLogging)
                logFile = filePath + "\\log" + DateTime.UtcNow.ToString().Replace(' ', '_').Replace('/', '-').Replace(':', '-') + ".nwlog";
            else
                logFile = null;
            
            if (Properties.Settings.Default.autoDeleteLogs)
            {
                //Delete all logs older than 2 days
                foreach (var file in directoryInfo.GetFiles("*.nwlog"))
                {
                    if (file.CreationTimeUtc < DateTime.UtcNow.AddDays(-Properties.Settings.Default.autoDeleteLogsDays))
                        file.Delete();
                }
            }

            //Apply skins
            IEnumerable<Control> skinables = FindVisualChildren<Control>(gridMain).Where(x => x.Tag != null && x.Tag.ToString() == "skinable");
            switch (Properties.Settings.Default.themeName)
            {
                case "white":
                    foreach (Control control in skinables)
                        control.Background = (Brush)this.FindResource("colorWhiteBG");
                    tbtrayTop.Background = (Brush)this.FindResource("colorWhiteBG");
                    break;
                case "blue":
                    foreach (Control control in skinables)
                        control.Background = (Brush)this.FindResource("colorBlueBG");
                    tbtrayTop.Background = (Brush)this.FindResource("colorBlueBG");
                    break;
                case "green":
                    foreach (Control control in skinables)
                        control.Background = (Brush)this.FindResource("colorGreenBG");
                    tbtrayTop.Background = (Brush)this.FindResource("colorGreenBG");
                    break;
            }
        }

        private void anyNew_Click(object sender, RoutedEventArgs e)
        {
            rtbMain.SelectAll();
            rtbMain.Selection.Text = "";
            unsavedChanges = false;
            currentlyOpenPath = "";
            nwDebug.nwLog("New Document", 1, logFile);
        }

        private void anyOpen_Click(object sender, RoutedEventArgs e)
        {
            Nullable<bool> result = ofdOpen.ShowDialog();
            if (result == true)
            {
                nwDebug.nwLog("Opening from Open.", 1, logFile);
                try
                {
                    long length = new FileInfo(ofdOpen.FileName).Length;
                    if (length > performanceModeMinSize)
                    {
                        nwDebug.nwLog("File is above performance treshold.", 1, logFile);
                        messageBox = new customMessageBox();
                        messageBox.SetupMsgBox("Opening this file may cause performance issues. Do you wish to open it using Performance Mode?", "Performance Mode", this.FindResource("iconWarning"), "Yes", "No", "Cancel");
                        messageBox.ShowDialog();
                        switch (messageBox.result.ToString())
                        {
                            case "Yes":
                                nwDebug.nwLog("Attempting to open file in Performance Mode.", 1, logFile);
                                performanceMode performanceMode = new performanceMode();
                                performanceMode.openDocument(ofdOpen.FileName);
                                performanceMode.Show();
                                return;
                            case "No":
                                break;
                            case "Cancel":
                                return;
                        }
                    }
                    openDocument(ofdOpen.FileName, rtbMain);
                    currentlyOpenPath = ofdOpen.FileName;
                }
                catch (Exception ex)
                {
                    messageBox = new customMessageBox();
                    messageBox.SetupMsgBox(ex.Message + errorPreset, "Error!", this.FindResource("iconError"));
                    messageBox.ShowDialog();
                    nwDebug.nwLog("Exception " + ex.Message, 0, logFile);
                }
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
                    messageBox = new customMessageBox();
                    messageBox.SetupMsgBox(ex.Message + errorPreset, "Error!", this.FindResource("iconError"));
                    messageBox.ShowDialog();
                    nwDebug.nwLog("Exception " + ex.Message, 0, logFile);
                }
            }
        }

        private void anySaveAs_Click(object sender, RoutedEventArgs e)
        {
            Nullable<bool> result = sfdSave.ShowDialog();
            if (result == true)
            {
                nwDebug.nwLog("Saving from Save.", 1, logFile);
                try
                {
                    saveDocument(sfdSave.FileName, rtbMain);
                }
                catch (Exception ex)
                {
                    messageBox = new customMessageBox();
                    messageBox.SetupMsgBox(ex.Message + errorPreset, "Error!", this.FindResource("iconError"));
                    messageBox.ShowDialog();
                    nwDebug.nwLog("Exception " + ex.Message, 0, logFile);
                }
            }
        }

        private void anyExit_Click(object sender, RoutedEventArgs e)
        {
            //If there are unsaved changes, the "Unsaved Changes" dialog openes, if not, close the application
            nwDebug.nwLog("Exiting", 1, logFile);
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
            nwDebug.nwLog("Exiting", 1, logFile);
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
            nwDebug.nwLog("Changed font.", 1, logFile);
            rtbMain.Focus();
        }

        private void anyFontSize_DropDownClosed(object sender, EventArgs e)
        {
            //Apply the selected font size
            if (cbFontSize.SelectedItem != null)
                rtbMain.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cbFontSize.SelectedItem);
            nwDebug.nwLog("Canged font size.", 1, logFile);
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
            //Make sure the Drop event doesn't trigger twice, very hacky solution since nothing else worked
            if (!canDrop)
            {
                canDrop = true;
                return;
            }
            canDrop = false;
            //Check if the dropped data is a file to not override the existing functionality
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                nwDebug.nwLog("Opening from DragDrop.", 1, logFile);
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                animate(new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.25))), new PropertyPath(RichTextBox.OpacityProperty), animationStoryboard, canvasDragDrop.Name);
                //Until multiple documents can be opened, only open the first one
                try
                {
                    long length = new FileInfo(files[0]).Length;
                    if (length > performanceModeMinSize)
                    {
                        nwDebug.nwLog("File is above performance treshold.", 1, logFile);
                        messageBox = new customMessageBox();
                        messageBox.SetupMsgBox("Opening this file may cause performance issues. Do you wish to open it using Performance Mode?", "Performance Mode", this.FindResource("iconWarning"), "Yes", "No", "Cancel");
                        messageBox.ShowDialog();
                        switch (messageBox.result.ToString())
                        {
                            case "Yes":
                                nwDebug.nwLog("Attempting to open file in Performance Mode.", 1, logFile);
                                performanceMode performanceMode = new performanceMode();
                                performanceMode.openDocument(files[0]);
                                performanceMode.Show();
                                return;
                            case "No":
                                break;
                            case "Cancel":
                                return;
                            default:
                                nwDebug.nwLog("Unknown result returned.", 2, logFile);
                                return;
                        }
                    }
                    openDocument(files[0], rtbMain);
                    currentlyOpenPath = files[0];
                }
                catch (Exception ex)
                {
                    messageBox = new customMessageBox();
                    messageBox.SetupMsgBox(ex.Message + errorPreset, "Error!", this.FindResource("iconError"));
                    messageBox.ShowDialog();
                    nwDebug.nwLog("Exception " + ex.Message, 0, logFile);
                }
            }
            e.Handled = true;
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
            nwDebug.nwLog("Show Performance Mode.", 1, logFile);
        }

        private void miError_Click(object sender, RoutedEventArgs e)
        {
            messageBox = new customMessageBox();
            messageBox.SetupMsgBox("A user-initiated error has occured." + errorPreset, "Error!", this.FindResource("iconError"), "OK", "Cancel", "I'm special!");
            messageBox.ShowDialog();
            if (messageBox.result.ToString() == "Button3")
                Console.WriteLine("Selected 3");
        }

        private void miDebug_Click(object sender, RoutedEventArgs e)
        {
            var resourceIcons = new ResourceDictionary();
            resourceIcons.Source = new Uri("icons.xaml", UriKind.RelativeOrAbsolute);
            settings settings = new settings();
            if (settings.ShowDialog() == true)
                applySettings();
        }
    }
}
