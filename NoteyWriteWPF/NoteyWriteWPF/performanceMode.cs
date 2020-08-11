using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoteyWriteWPF
{
    public partial class performanceMode : Form
    {
        string currentlyOpenFile = "";

        public performanceMode()
        {
            InitializeComponent();

            List<string> fonts = new List<string>();
            foreach (System.Drawing.FontFamily font in System.Drawing.FontFamily.Families)
            {
                cbFont.Items.Add(font.Name);
            }
        }

        public void openDocument(string filePath)
        {
            try
            {
                string fileExtension = System.IO.Path.GetExtension(filePath);
                if (fileExtension != ".rtf" && fileExtension != ".txt")
                {
                    //rtbMain.Rtf = new FileStream(filePath, FileMode.Open);
                    rtbMain.LoadFile(filePath, System.Windows.Forms.RichTextBoxStreamType.PlainText);
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

        private Font fontReturn()
        {
            Font resultingFont;
            FontStyle fontStyle = new FontStyle();

            if (bBold.Checked)
                fontStyle = fontStyle ^ FontStyle.Bold;
            if (bItalic.Checked)
                fontStyle = fontStyle ^ FontStyle.Italic;
            if (bUnderline.Checked)
                fontStyle = fontStyle ^ FontStyle.Underline;

            resultingFont = new Font(new FontFamily(cbFont.Text), float.Parse(cbFontSize.Text), fontStyle);
            return resultingFont;
        }

        private void miNew_Click(object sender, EventArgs e)
        {
            currentlyOpenFile = "";
            rtbMain.Text = "";
        }

        private void miOpen_Click(object sender, EventArgs e)
        {
            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.Title = "Open a document | NoteyWrite";
            if (openFileDialog.ShowDialog() == DialogResult.Yes)
                openDocument(openFileDialog.FileName);
        }

        private void miSave_Click(object sender, EventArgs e)
        {
            if (currentlyOpenFile == "")
            {
                saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.Title = "Save a document | NoteyWrite";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
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
                return;
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

        private void miSaveAs_Click(object sender, EventArgs e)
        {
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.Title = "Save a document | NoteyWrite";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
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

        private void miExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bBold_CheckedChanged(object sender, EventArgs e)
        {
            rtbMain.SelectionFont = fontReturn();
        }

        private void bItalic_CheckedChanged(object sender, EventArgs e)
        {
            rtbMain.SelectionFont = fontReturn();
        }

        private void bUnderline_CheckedChanged(object sender, EventArgs e)
        {
            rtbMain.SelectionFont = fontReturn();
        }

        private void rtbMain_SelectionChanged(object sender, EventArgs e)
        {
            bBold.Checked = (rtbMain.SelectionFont.Bold == true);
            bItalic.Checked = (rtbMain.SelectionFont.Italic == true);
            bUnderline.Checked = (rtbMain.SelectionFont.Underline == true);

            /*if (rtbMain.SelectionFont.Bold)
                bBold.Checked = true;
            if (rtbMain.SelectionFont.Italic)
                bItalic.Checked = true;
            if (rtbMain.SelectionFont.Underline)
                bUnderline.Checked = true;*/

            cbFont.Text = rtbMain.SelectionFont.Name;
            cbFontSize.Text = rtbMain.SelectionFont.Size.ToString();
        }

        private void cbFont_Click(object sender, EventArgs e)
        {
            rtbMain.SelectionFont = fontReturn();
        }

        private void cbFontSize_Click(object sender, EventArgs e)
        {
            rtbMain.SelectionFont = fontReturn();
        }

        private void cbFontSize_TextChanged(object sender, EventArgs e)
        {
            if (cbFontSize.Text == "13") return;
            rtbMain.SelectionFont = fontReturn();
        }

        private void cbFont_TextChanged(object sender, EventArgs e)
        {
            rtbMain.SelectionFont = fontReturn();
        }
    }
}
