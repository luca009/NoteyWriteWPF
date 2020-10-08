using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace NoteyWriteWPF
{
    /// <summary>
    /// Interaction logic for detailEdit.xaml
    /// </summary>
    public partial class detailEdit : Window
    {
        bool allowNumbers = true;
        public string text = "";

        public detailEdit()
        {
            InitializeComponent();
            tbInput.Focus();
        }

        public void SetupMsgBox(string info, string title = "Input", bool filterNumbers = false)
        {
            textInfo.Text = info;
            this.Title = title;
            allowNumbers = !filterNumbers;
        }
        public void SetupMsgBox(string info, string value, string title = "Input", bool filterNumbers = false)
        {
            textInfo.Text = info;
            this.Title = title;
            allowNumbers = !filterNumbers;
            tbInput.Text = value;
        }

        private void tbInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!allowNumbers) {
                Regex regexObj = new Regex(@"[^\d]");
                tbInput.Text = regexObj.Replace(tbInput.Text, "");
                if (tbInput.Text.Length > 4)
                    tbInput.Text = tbInput.Text.Remove(4);
                if (tbInput.Text.StartsWith("0"))
                    tbInput.Text = "1";
            }
        }

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            text = tbInput.Text;
            this.DialogResult = true;
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
