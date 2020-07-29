using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : Window
    {
        public ColorPicker()
        {
            InitializeComponent();
            setPreview();
        }

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Point point;
            point = this.PointToScreen(Mouse.GetPosition(this));
            this.Left = point.X;
            this.Top = point.Y;
        }

        private void setTextBoxToSlider(Slider slider, TextBox textBox)
        {
            if (slider == null || textBox == null)
                return;
            slider.Value = Math.Round(slider.Value);
            textBox.Text = slider.Value.ToString();
        }

        private void setSliderToTextBox(Slider slider, TextBox textBox)
        {
            if (slider == null || textBox == null)
                return;
            Regex regexObj = new Regex(@"[^\d]");
            textBox.Text = regexObj.Replace(textBox.Text, "");
            if (textBox.Text == "")
                return;
            slider.Value = Double.Parse(textBox.Text);
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

        private void setPreview()
        {
            if (slHue == null || slSaturation == null || slValue == null || rectPreview == null)
                return;
            Color color = ColorFromHSV(Convert.ToDouble(slHue.Value), Convert.ToDouble(slSaturation.Value / 100), Convert.ToDouble(slValue.Value / 100));
            rectPreview.Fill = new SolidColorBrush(color);
        }

        private void slHue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setTextBoxToSlider(slHue, tbHue);
            setPreview();
        }

        private void tbHue_TextChanged(object sender, TextChangedEventArgs e)
        {
            setSliderToTextBox(slHue, tbHue);
            setPreview();
        }

        private void tbSaturation_TextChanged(object sender, TextChangedEventArgs e)
        {
            setSliderToTextBox(slSaturation, tbSaturation);
            setPreview();
        }

        private void slSaturation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setTextBoxToSlider(slSaturation, tbSaturation);
            setPreview();
        }

        private void slValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setTextBoxToSlider(slValue, tbValue);
            setPreview();
        }

        private void tbValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            setSliderToTextBox(slValue, tbValue);
            setPreview();
        }
    }
}
