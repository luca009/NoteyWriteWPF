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
    /// Interaction logic for find.xaml
    /// </summary>
    public partial class find : Window
    {
        public string searchString;
        public RichTextBox richTextBox;
        public TextRange textRange;

        public find()
        {
            InitializeComponent();
        }

        internal TextRange getTextRangeFromIndex(RichTextBox rtb, int index, int length)
        {
            //Select using an index and length
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);

            if (textRange.Text.Length >= (index + length))
            {
                TextPointer start = textRange.Start.GetPositionAtOffset(index, LogicalDirection.Forward);
                TextPointer end = textRange.Start.GetPositionAtOffset(index + length, LogicalDirection.Backward);
                textRange = new TextRange(start, end);
                return textRange;
            }
            return null;
        }

        private void bTrue_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void bFind_Click(object sender, RoutedEventArgs e)
        {
            int nextIndex = 0;
            int lastIndex = -1;
            find find = new find();
            string comparisonString = "";
            if (tbFind.Text.Length != 0)
            {
                richTextBox.SelectAll();
                comparisonString = tbFind.Text;
                lastIndex = richTextBox.Selection.Text.IndexOf(comparisonString, nextIndex, StringComparison.OrdinalIgnoreCase);
                if (nextIndex != -1)
                {
                    //MainWindow mainWindow = new MainWindow();
                    //rtbMain.Selection.Text.Substring(rtbMain.Selection.Text.IndexOf(comparisonString, StringComparison.OrdinalIgnoreCase), comparisonString.Length);
                    textRange = getTextRangeFromIndex(richTextBox, lastIndex, comparisonString.Length);
                    nextIndex = lastIndex + comparisonString.Length;
                    return;
                }
                this.DialogResult = true;
            }
        }

        private void find1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DialogResult != true)
                this.DialogResult = false;
        }
    }
}
