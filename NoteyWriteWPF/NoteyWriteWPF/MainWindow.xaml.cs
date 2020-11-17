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
using NoteyWriteWPF.Classes;

namespace NoteyWriteWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Define Variables
        private Storyboard animationStoryboard;
        public string currentVersion = "Alpha 0.5";
        public SaveFileDialog sfdSave = new SaveFileDialog();
        public OpenFileDialog ofdOpen = new OpenFileDialog();
        private OpenFileDialog ofdImage = new OpenFileDialog();
        public string currentlyOpenPath;
        private bool unsavedChanges = false;
        private string[] arguments;
        private int performanceModeMinSize = 6144;
        customMessageBox messageBox = new customMessageBox();
        string errorPreset = "\nNoteyWrite will attempt to continue normal operation.";
        string logFile;
        bool canDrop = true;
        Image insertImage;
        BlockUIContainer insertImageUIContainer;
        System.Windows.Forms.Timer spellcheckTimer = new System.Windows.Forms.Timer() { Enabled = false, Interval = 2000 };
        System.Windows.Forms.Timer timerAutosave = new System.Windows.Forms.Timer() { Enabled = false, Interval = 300000 };
        bool delaySpellcheckExecution = true;
        bool sidebarOpen = false;
        bool findReplaceChanges = true;
        int nextIndex = 0;
        FindAndReplaceManager findAndReplace = new FindAndReplaceManager(new FlowDocument());

        public MainWindow()
        {
            // Initialize
            InitializeComponent();
            applySettings();
            //logFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            nwDebug.nwLog("NoteyWrite started.", nwDebug.Severity.Info, logFile);
            arguments = getArguments();
            if (arguments.Length > 1)
            {
                nwDebug.nwLog("Opening from argument.", nwDebug.Severity.Info, logFile);
                if (File.Exists(arguments[1]))
                {
                    nwDebug.nwLog("Argument file exists.", nwDebug.Severity.Info, logFile);
                    try
                    {
                        long length = new FileInfo(arguments[1]).Length;
                        if (length > performanceModeMinSize)
                        {
                            nwDebug.nwLog("File is above performance treshold.", nwDebug.Severity.Info, logFile);
                            messageBox = new customMessageBox();
                            messageBox.SetupMsgBox("Opening this file may cause performance issues. Do you wish to open it using Performance Mode?", "Performance Mode", this.FindResource("iconWarning"), "Yes", "No", "Cancel");
                            messageBox.ShowDialog();
                            switch (messageBox.result.ToString())
                            {
                                case "Yes":
                                    nwDebug.nwLog("Attempting to open file in Performance Mode.", nwDebug.Severity.Info, logFile);
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
                        nwDebug.nwLog("Exception " + ex.Message, nwDebug.Severity.Error, logFile);
                        messageBox = new customMessageBox();
                        messageBox.SetupMsgBox(ex.Message + errorPreset, "Error!", this.FindResource("iconError"));
                        messageBox.ShowDialog();
                    }
                }
            }

            mainWindow.Title = "NoteyWriteWPF " + currentVersion;
            sfdSave.Filter = "XML Document (*.xml)|*.xml|Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt|All files (*.*)|*.*";
            sfdSave.Title = "Save a document | NoteyWriteWPF";
            ofdOpen.Filter = "Known Documents (*.xml, *.rtf, *.txt)|*.xml;*.rtf;*.txt|XML Document (*.xml)|*.xml|Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt|All files (*.*)|*.*";
            ofdOpen.Title = "Open a document | NoteyWriteWPF";
            ofdImage.Filter = "Known Image Formats (*.png, *.jpg, *.jpeg, *.bmp)|*.png;*.jpg;*.jpeg;*.bmp";
            ofdImage.Title = "Open an image | NoteyWrite";

            //List<string> fonts = new List<string>();
            foreach (System.Drawing.FontFamily font in System.Drawing.FontFamily.Families)
            {
                //fonts.Add(font.Name);
                cbFont.Items.Add(new ComboBoxItem() { Content = font.Name, FontFamily = new FontFamily(font.Name) });
            }
            nwDebug.nwLog("Added all fonts.", nwDebug.Severity.Info, logFile);

            //cbFont.ItemsSource = fonts;
            cbFontSize.ItemsSource = new List<Double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72, 102, 144, 288 };

            //Override the default RichTextBox Events
            rtbMain.AddHandler(RichTextBox.DragOverEvent, new DragEventHandler(rtbMain_DragOver), true);
            rtbMain.AddHandler(RichTextBox.DropEvent, new DragEventHandler(rtbMain_Drop), true);
            rtbMain.AddHandler(RichTextBox.DragEnterEvent, new DragEventHandler(rtbMain_DragEnter), true);
            rtbMain.AddHandler(RichTextBox.DragLeaveEvent, new DragEventHandler(rtbMain_DragLeave), true);

            spellcheckTimer.Tick += SpellcheckTimer_Tick;
            timerAutosave.Tick += timerAutosave_Click;
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
            /*if (fileExtension == ".rtf")
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
                rtbLoad.Selection.Load(new FileStream(filePath, FileMode.Open), DataFormats.Text);*/
            switch (fileExtension)
            {
                case ".rtf":
                    rtbLoad.Selection.Load(new FileStream(filePath, FileMode.Open), DataFormats.Rtf);
                    break;
                case ".txt":
                    rtbLoad.Selection.Load(new FileStream(filePath, FileMode.Open), DataFormats.Text);
                    break;
                case ".xml":
                    XmlReader xmlReader = XmlReader.Create(filePath);
                    XamlReader xamlReader = new XamlReader();
                    rtbLoad.Document = new FlowDocument();
                    rtbLoad.Document = (FlowDocument)(XamlReader.Load(xmlReader));
                    /*foreach (Image image in rtbLoad.Document.Blocks)
                    {

                    }*/
                        break;
                default:
                    rtbLoad.Selection.Load(new FileStream(filePath, FileMode.Open), DataFormats.Text);
                    break;
            }
            

            nwDebug.nwLog("Opened File.", nwDebug.Severity.Info, logFile);
            unsavedChanges = false;
            return true;
        }

        public bool saveDocument(string filePath, RichTextBox rtbSave)
        {
            //Uses a FileStream to save the contents of the RichTextBox
            string fileExtension = System.IO.Path.GetExtension(filePath);
            FileStream stream;
            rtbSave.SelectAll();

            /*if (fileExtension == ".rtf")
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
                rtbSave.Selection.Save(stream = new FileStream(filePath, FileMode.Create), DataFormats.Text);*/
            switch (fileExtension)
            {
                case ".rtf":
                    rtbSave.Selection.Save(new FileStream(filePath, FileMode.Create), DataFormats.Rtf);
                    break;
                case ".txt":
                    rtbSave.Selection.Save(stream = new FileStream(filePath, FileMode.Create), DataFormats.Text);
                    break;
                case ".xml":
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(XamlWriter.Save(rtbSave.Document));
                    xdoc.Save(filePath);
                    break;
                default:
                    rtbSave.Selection.Save(stream = new FileStream(filePath, FileMode.Create), DataFormats.Text);
                    break;
            }

            nwDebug.nwLog("Saved File.", nwDebug.Severity.Info, logFile);
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
            if (index < 0)
                return;
            RichTextBox rtbTemp = XamlReader.Parse(XamlWriter.Save(rtbMain)) as RichTextBox;
            rtbTemp.SelectAll();
            string shortenedText = rtbTemp.Selection.Text.Remove(index);
            int carriageReturnCount = shortenedText.Count(x => x == '\n');
            index += carriageReturnCount * 2;
            if (index + length > rtbTemp.Selection.Text.Length)
                return;
            /*if (rtbTemp.Selection.Text.IndexOf("\r") != -1)
            {
                int tempIndex = index;
                for (int i = 1; i < tempIndex; i++)
                {
                    if (rtbTemp.Selection.Text.IndexOf("\r", i) == -1)
                        break;
                    else
                        if (rtbTemp.Selection.Text.IndexOf("\r", i, 1) != -1)
                            index += 2;
                }
            }*/
            //string sanitizedText = rtbTemp.Selection.Text.Replace(@"\r\n", @"\n");
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
        private void animate(ThicknessAnimation animation, PropertyPath propertyPath, Storyboard storyboard, string objectName)
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
                //Delete all logs older than autoDeleteLogsDays days
                foreach (var file in directoryInfo.GetFiles("*.nwlog"))
                {
                    if (file.CreationTimeUtc < DateTime.UtcNow.AddDays(-Properties.Settings.Default.autoDeleteLogsDays))
                        file.Delete();
                }
            }

            //Delete all logs older than 2 days
            foreach (var file in directoryInfo.GetFiles("autosave*.xml"))
            {
                if (file.CreationTimeUtc < DateTime.UtcNow.AddDays(-2))
                    file.Delete();
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
                case "adaptive":
                    try
                    {
                        ImageBrush imgBG = new ImageBrush(new BitmapImage(new Uri(Properties.Settings.Default.themeBG)));
                        imgBG.Stretch = Stretch.UniformToFill;
                        tbtrayTop.Background = imgBG;
                        System.Drawing.Color avgColor = getAverageColor((System.Drawing.Bitmap)System.Drawing.Image.FromFile(Properties.Settings.Default.themeBG));
                        SolidColorBrush bgBrush = new SolidColorBrush(Color.FromRgb(avgColor.R, avgColor.G, avgColor.B));
                        foreach (Control control in skinables)
                            control.Background = bgBrush;
                    }
                    catch (Exception ex)
                    {
                        messageBox = new customMessageBox();
                        messageBox.SetupMsgBox(ex.Message + errorPreset + "\nTheme will be reset to Blue.", "Error!", this.FindResource("iconError"));
                        messageBox.ShowDialog();
                        nwDebug.nwLog("Exception " + ex.Message, nwDebug.Severity.Error, logFile);
                        Properties.Settings.Default.themeName = "blue";
                        Properties.Settings.Default.Save();
                    }
                    break;
            }

            spellcheckTimer.Interval = Properties.Settings.Default.spellcheckExecutionDelay;
            delaySpellcheckExecution = Properties.Settings.Default.delaySpellcheckExecution;
            timerAutosave.Interval = Properties.Settings.Default.autosaveInterval;
            timerAutosave.Enabled = Properties.Settings.Default.doAutosave;

            //Add Keyboard Shortcuts
            RoutedCommand commandNew = new RoutedCommand();
            commandNew.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(commandNew, anyNew_Click));
            RoutedCommand commandOpen = new RoutedCommand();
            commandOpen.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(commandOpen, anyOpen_Click));
            RoutedCommand commandSave = new RoutedCommand();
            commandSave.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(commandSave, anySave_Click));
            RoutedCommand commandSaveAs = new RoutedCommand();
            commandSaveAs.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control ^ ModifierKeys.Shift));
            CommandBindings.Add(new CommandBinding(commandSaveAs, anySaveAs_Click));
        }

        private System.Drawing.Color getAverageColor(System.Drawing.Bitmap bm)
        {
            var list = new Dictionary<System.Drawing.Color, int>();
            int addR = 0;
            int addG = 0;
            int addB = 0;
            System.Drawing.Bitmap myImage = new System.Drawing.Bitmap(bm, new System.Drawing.Size(50, 50));
            for (int x = 0; x < myImage.Width; x++)
            {
                for (int y = 0; y < myImage.Height; y++)
                {
                    System.Drawing.Color color = myImage.GetPixel(x, y);
                    if (!list.ContainsKey(color))
                        list.Add(color, 1);
                    else
                        list[color]++;
                }
            }
            System.Drawing.Color keyOfMaxValue = list.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            if (keyOfMaxValue.GetBrightness() < 0.5F)
            {
                Color higherBrightness = ColorFromHSV(keyOfMaxValue.GetHue(), keyOfMaxValue.GetSaturation(), 0.85F);
                keyOfMaxValue = System.Drawing.Color.FromArgb(higherBrightness.R, higherBrightness.G, higherBrightness.B);
            }
            System.Drawing.Color returnColor = System.Drawing.Color.FromArgb((int)keyOfMaxValue.R + addR, (int)keyOfMaxValue.G + addG, (int)keyOfMaxValue.B + addB);
            return returnColor;
        }

        private void anyNew_Click(object sender, RoutedEventArgs e)
        {
            rtbMain.SelectAll();
            rtbMain.Selection.Text = "";
            unsavedChanges = false;
            currentlyOpenPath = "";
            nwDebug.nwLog("New Document", nwDebug.Severity.Info, logFile);
        }

        private void anyOpen_Click(object sender, RoutedEventArgs e)
        {
            Nullable<bool> result = ofdOpen.ShowDialog();
            if (result == true)
            {
                nwDebug.nwLog("Opening from Open.", nwDebug.Severity.Info, logFile);
                try
                {
                    long length = new FileInfo(ofdOpen.FileName).Length;
                    if (length > performanceModeMinSize)
                    {
                        nwDebug.nwLog("File is above performance treshold.", nwDebug.Severity.Info, logFile);
                        messageBox = new customMessageBox();
                        messageBox.SetupMsgBox("Opening this file may cause performance issues. Do you wish to open it using Performance Mode?", "Performance Mode", this.FindResource("iconWarning"), "Yes", "No", "Cancel");
                        messageBox.ShowDialog();
                        switch (messageBox.result.ToString())
                        {
                            case "Yes":
                                nwDebug.nwLog("Attempting to open file in Performance Mode.", nwDebug.Severity.Info, logFile);
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
                    nwDebug.nwLog("Exception " + ex.Message, nwDebug.Severity.Error, logFile);
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
                    nwDebug.nwLog("Exception " + ex.Message, nwDebug.Severity.Error, logFile);
                }
            }
        }

        private void anySaveAs_Click(object sender, RoutedEventArgs e)
        {
            Nullable<bool> result = sfdSave.ShowDialog();
            if (result == true)
            {
                nwDebug.nwLog("Saving from Save.", nwDebug.Severity.Info, logFile);
                try
                {
                    saveDocument(sfdSave.FileName, rtbMain);
                }
                catch (Exception ex)
                {
                    messageBox = new customMessageBox();
                    messageBox.SetupMsgBox(ex.Message + errorPreset, "Error!", this.FindResource("iconError"));
                    messageBox.ShowDialog();
                    nwDebug.nwLog("Exception " + ex.Message, nwDebug.Severity.Error, logFile);
                }
            }
        }

        private void anyExit_Click(object sender, RoutedEventArgs e)
        {
            //If there are unsaved changes, the "Unsaved Changes" dialog openes, if not, close the application
            nwDebug.nwLog("Exiting", nwDebug.Severity.Info, logFile);
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
            findReplaceChanges = true;
            if (delaySpellcheckExecution)
            {
                rtbMain.SpellCheck.IsEnabled = false;
                spellcheckTimer.Enabled = true;
            }
        }

        private void SpellcheckTimer_Tick(object sender, EventArgs e)
        {
            spellcheckTimer.Enabled = false;
            rtbMain.SpellCheck.IsEnabled = true;
        }

        private void timerAutosave_Click(object sender, EventArgs e)
        {
            if (unsavedChanges)
            {
                string runningPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                saveDocument(runningPath + "\\autosave" + DateTime.UtcNow.ToString().Replace(' ', '_').Replace('/', '-').Replace(':', '-') + ".xml", rtbMain);
            }
        }

        private void rtbMain_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //Change the formatting options accordingly to the selected text
            //if (rtbMain.Selection.Start.Paragraph != null)
            //{
            //if (rtbMain.Selection.Start.Paragraph.NextBlock != null)
            //{ 
            /*string temps = null;
            var curCaret = rtbMain.CaretPosition;
            var curBlock = rtbMain.Document.Blocks.Where(x => x.ContentStart.CompareTo(curCaret) == -1 || x.ContentEnd.CompareTo(curCaret) == 1).FirstOrDefault();

            rtbMain.Document.

            if (curBlock != null)
                temps = curBlock.Name;
            if (temps == "Image")
                Console.WriteLine("image");*/
                //}
            //}

            object temp = rtbMain.Selection.GetPropertyValue(Inline.FontWeightProperty);
            if (temp != null)
                miBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            temp = rtbMain.Selection.GetPropertyValue(Inline.FontStyleProperty);
            if (temp != null)
                miItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            temp = rtbMain.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            if (temp != null)
                miUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));
            var ttemp = rtbMain.Selection.Start.Parent;
            if (ttemp.GetType().Name == "BlockUIContainer") 
                AddBlockUIContainerAdorner((BlockUIContainer)ttemp);
            //ttemp.ReadLocalValue()
            /*foreach (var child in rtbMain.Selection.Start.Parent)
            {

            }
            if (rtbMain.Selection.Start.Parent)
            {

            }*/
           //if (ttemp.GetValue(Child))

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
            nwDebug.nwLog("Exiting", nwDebug.Severity.Info, logFile);
            if (unsavedChanges)
            {
                exit exit = new exit();
                exit.mWRichTextBox = XamlReader.Parse(XamlWriter.Save(rtbMain)) as RichTextBox;
                exit.mWCurrentlyOpenFile = currentlyOpenPath;
                if (exit.ShowDialog() == true)
                {
                    e.Cancel = true;
                    return;
                }
                Application.Current.Shutdown();
            }
            else
                Application.Current.Shutdown();
        }

        private void anyFont_DropDownClosed(object sender, EventArgs e)
        {
            //Apply the selected font family with some questionable code
            if (cbFont.SelectedItem != null)
                rtbMain.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, ((ComboBoxItem)cbFont.SelectedItem).FontFamily);
            nwDebug.nwLog("Changed font.", nwDebug.Severity.Info, logFile);
            rtbMain.Focus();
        }

        private void anyFontSize_DropDownClosed(object sender, EventArgs e)
        {
            //Apply the selected font size
            if (cbFontSize.SelectedItem != null)
                rtbMain.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cbFontSize.SelectedItem);
            nwDebug.nwLog("Changed font size.", nwDebug.Severity.Info, logFile);
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

        private void SidebarToggle(Uri uri)
        {
            frameSidebar.Source = uri;
            float width = 200f;
            EasingMode easing = EasingMode.EaseOut;
            if (sidebarOpen)
            {
                width = -200f;
                sidebarOpen = false;
                easing = EasingMode.EaseIn;
            }
            else
                sidebarOpen = true;
            ThicknessAnimation sidebar = new ThicknessAnimation(new Thickness(-width, 0f, 0f, 0f), new Duration(TimeSpan.FromSeconds(0.6f))) { EasingFunction = new QuinticEase() { EasingMode = easing } };
            ThicknessAnimation textBox = new ThicknessAnimation(new Thickness(0f, 0f, width, 0f), new Duration(TimeSpan.FromSeconds(0.6f))) { EasingFunction = new QuinticEase() { EasingMode = easing } };
            animate(sidebar, new PropertyPath(Canvas.MarginProperty), animationStoryboard, "canvasSidebar");
            animate(textBox, new PropertyPath(RichTextBox.MarginProperty), animationStoryboard, "rtbMain");
        }

        private void frameSidebar_ContentRendered(object sender, EventArgs e)
        {
            var content = frameSidebar.Content as Page;
            var grid = content.Content as Grid;
            switch (content.Title)
            {
                case "FindReplace":
                    Button bFind = (Button)grid.Children[5];
                    bFind.Click += bFind_Click;
                    break;
                default:
                    break;
            }
        }

        private void miFindReplace_Click(object sender, RoutedEventArgs e)
        {
            SidebarToggle(new Uri("/SidebarPages/FindReplace.xaml", UriKind.Relative));
            
            //var temp = frameSidebar.CanGoBack;
            
            /*int nextIndex = 0;
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

        private void bFind_Click(object sender, RoutedEventArgs e)
        {
            var content = frameSidebar.Content as Page;
            var grid = content.Content as Grid;
            TextBox tbFind = (TextBox)grid.Children[2];
            TextBox tbReplace = (TextBox)grid.Children[4];
            if (findReplaceChanges)
            {
                findAndReplace = new FindAndReplaceManager(rtbMain.Document);
                findReplaceChanges = false;
            }
            TextRange textRange = findAndReplace.FindNext(tbFind.Text, FindOptions.None);
            if (textRange == null)
            {
                customMessageBox customMessageBox = new customMessageBox();
                customMessageBox.SetupMsgBox("No (more) results.\nDo you wish to start at the top again?", "Reset search?", this.FindResource("iconInformation"), "Yes", "No");
                customMessageBox.ShowDialog();
                if (customMessageBox.result.ToString() == "Yes")
                {
                    findAndReplace = new FindAndReplaceManager(rtbMain.Document);
                    findReplaceChanges = false;
                    textRange = findAndReplace.FindNext(tbFind.Text, FindOptions.None);
                }
                else
                    return;
            }
            rtbMain.Selection.Select(textRange.Start, textRange.End);
            rtbMain.Focus();
            /*rtbMain.SelectAll();
            int lastIndex = rtbMain.Selection.Text.IndexOf(tbFind.Text, nextIndex, StringComparison.OrdinalIgnoreCase);
            if (nextIndex == -1)
            {
                MessageBox.Show("yo");
            }
            else
            {
                selectFromIndex(rtbMain, lastIndex, tbFind.Text.Length);
                //rtbMain.Selection.Select(rtbMain.Document.ContentStart.GetPositionAtOffset(4), rtbMain.Document.ContentEnd);
                /*TextRange text = new TextRange(rtbMain.Document.ContentStart, rtbMain.Document.ContentEnd);
                TextPointer current = text.Start.GetInsertionPosition(LogicalDirection.Forward);
                TextPointer selectionStart = current.GetPositionAtOffset(lastIndex, LogicalDirection.Forward);
                TextPointer selectionEnd = selectionStart.GetPositionAtOffset(tbFind.Text.Length, LogicalDirection.Forward);
                TextRange selection = new TextRange(selectionStart, selectionEnd);
                rtbMain.Selection.Select(selection.Start, selection.End);
                rtbMain.Focus();
                nextIndex = lastIndex + tbFind.Text.Length;
                
                /*TextPointer selectionStart = current.GetPositionAtOffset(index, LogicalDirection.Forward);
                TextPointer selectionEnd = selectionStart.GetPositionAtOffset(keyword.Length, LogicalDirection.Forward);
                TextRange selection = new TextRange(selectionStart, selectionEnd);
                selection.Text = newString;
                selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                rtbMain.Selection.Select(selection.Start, selection.End);
                rtbMain.Focus();
            }
            /*string keyword = tbFind.Text;
            string newString = tbFind.Text;
            TextRange text = new TextRange(rtbMain.Document.ContentStart, rtbMain.Document.ContentEnd);
            TextPointer current = text.Start.GetInsertionPosition(LogicalDirection.Forward);
                string textInRun = current.GetTextInRun(LogicalDirection.Forward);
                if (!string.IsNullOrWhiteSpace(textInRun))
                {
                    int index = textInRun.IndexOf(keyword);
                    if (index != -1)
                    {
                        TextPointer selectionStart = current.GetPositionAtOffset(index, LogicalDirection.Forward);
                        TextPointer selectionEnd = selectionStart.GetPositionAtOffset(keyword.Length, LogicalDirection.Forward);
                        TextRange selection = new TextRange(selectionStart, selectionEnd);
                        selection.Text = newString;
                        rtbMain.Selection.Select(selection.Start, selection.End);
                        rtbMain.Focus();
                    }
                }
                current = current.GetNextContextPosition(LogicalDirection.Forward);*/
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

        private void miAnyBar_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == null || rtbMain == null)
                return;

            switch (((MenuItem)sender).Name)
            {
                case "miFormattingBar":
                    tbFormatting.Visibility = Visibility.Visible;
                    break;
                case "miParagraphBar":
                    tbParagraph.Visibility = Visibility.Visible;
                    break;
                default:
                    nwDebug.nwLog("Unknown Bar Visibility", nwDebug.Severity.Warning, logFile);
                    break;
            }
        }

        private void miAnyBar_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender == null || rtbMain == null)
                return;

            switch (((MenuItem)sender).Name)
            {
                case "miFormattingBar":
                    tbFormatting.Visibility = Visibility.Collapsed;
                    break;
                case "miParagraphBar":
                    tbParagraph.Visibility = Visibility.Collapsed;
                    break;
                default:
                    nwDebug.nwLog("Unknown Bar Visibility", nwDebug.Severity.Warning, logFile);
                    break;
            }
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
                nwDebug.nwLog("Opening from DragDrop.", nwDebug.Severity.Info, logFile);
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                animate(new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.25))), new PropertyPath(RichTextBox.OpacityProperty), animationStoryboard, canvasDragDrop.Name);
                //Until multiple documents can be opened, only open the first one
                try
                {
                    if (Path.GetExtension(files[0]) == ".jpg" || Path.GetExtension(files[0]) == ".png")
                    {
                        insertImage = new Image() { Source = new BitmapImage(new Uri(files[0])), Stretch = Stretch.Uniform, Width = rtbMain.ActualWidth - 50 };
                        insertImageUIContainer = new BlockUIContainer(insertImage);
                        rtbMain.Document.Blocks.Add(insertImageUIContainer);
                        return;
                    }
                    long length = new FileInfo(files[0]).Length;
                    if (length > performanceModeMinSize)
                    {
                        nwDebug.nwLog("File is above performance treshold.", nwDebug.Severity.Info, logFile);
                        messageBox = new customMessageBox();
                        messageBox.SetupMsgBox("Opening this file may cause performance issues. Do you wish to open it using Performance Mode?", "Performance Mode", this.FindResource("iconWarning"), "Yes", "No", "Cancel");
                        messageBox.ShowDialog();
                        switch (messageBox.result.ToString())
                        {
                            case "Yes":
                                nwDebug.nwLog("Attempting to open file in Performance Mode.", nwDebug.Severity.Info, logFile);
                                performanceMode performanceMode = new performanceMode();
                                performanceMode.openDocument(files[0]);
                                performanceMode.Show();
                                return;
                            case "No":
                                break;
                            case "Cancel":
                                return;
                            default:
                                nwDebug.nwLog("Unknown result returned.", nwDebug.Severity.Warning, logFile);
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
                    nwDebug.nwLog("Exception " + ex.Message, nwDebug.Severity.Error, logFile);
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
            nwDebug.nwLog("Show Performance Mode.", nwDebug.Severity.Info, logFile);
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
            /*var resourceIcons = new ResourceDictionary();
            resourceIcons.Source = new Uri("icons.xaml", UriKind.RelativeOrAbsolute);
            var resourceKeys = resourceIcons.Keys;*/
            RemoveBlockUIContainerAdorner((BlockUIContainer)rtbMain.Selection.Start.Parent);
        }

        private void miSettings_Click(object sender, RoutedEventArgs e)
        {
            settings settings = new settings();
            if (settings.ShowDialog() == true)
                applySettings();
        }

        private void miInsertImage_Click(object sender, RoutedEventArgs e)
        {
            if (ofdImage.ShowDialog() == true)
            {
                insertImage = new Image() { Source = new BitmapImage(new Uri(ofdImage.FileName)), Stretch = Stretch.Uniform, Width = rtbMain.ActualWidth - 50 };
                insertImageUIContainer = new BlockUIContainer(insertImage);
                rtbMain.Document.Blocks.Add(insertImageUIContainer);
                //rtbMain.Document.Blocks.Add(new BlockUIContainer(new Image() { Source = new BitmapImage(new Uri(ofdImage.FileName)), Stretch = Stretch.Uniform, Width = rtbMain.ActualWidth, MaxWidth = rtbMain.ActualWidth, MinWidth = rtbMain.ActualWidth } ));
                //uiContainer.
            }
        }

        private void AddBlockUIContainerAdorner(BlockUIContainer container)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(container.Child);
            if (adornerLayer != null)
                adornerLayer.Add(new imageResizeAdorner(container.Child));
        }

        private void RemoveBlockUIContainerAdorner(BlockUIContainer container)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(container.Child);
            if (adornerLayer != null)
                foreach (Adorner adorner in adornerLayer.GetAdorners(container.Child))
                    adornerLayer.Remove(adorner);
        }
    }
}
