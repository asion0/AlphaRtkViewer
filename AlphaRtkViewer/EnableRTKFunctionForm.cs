using MiscUtil.Conversion;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RtkViewer
{
    public partial class EnableRTKFunctionForm : Form
    {
        public EnableRTKFunctionForm()
        {
            InitializeComponent();
        }

        private string serialNo;
        public void SetSerialNumber(string s)
        {
            serialNo = s;
            licKeyTxt.Text = "";
        }

        private void EnableRTKFunctionForm_Load(object sender, EventArgs e)
        {
            serialNoLbl.Text = serialNo;
        }

        private void copySerNoBtn_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(serialNoLbl.Text);
        }

        private byte[] licenseKey = null;
        public byte[] GetLicenseKey() { return licenseKey; }
        private void activateBtn_Click(object sender, EventArgs e)
        {
            licenseKey = MiscConverter.StringToByteArrayWithSpace(licKeyTxt.Text);
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool IsValidateLicenseText(string s)
        {
            if (s.Length < 47)
                return false;

            char[] trimTable = { ' ' };
            string t = s.TrimStart(trimTable).TrimEnd(trimTable).ToUpper();
            string[] tArray = t.Split(trimTable);

            if (tArray.Length != 16)
                return false;

            foreach(string txt in tArray)
            {
                if (txt.Length != 2)
                    return false;
                if((t[0] < '0' || t[0] > '9') && (t[0] < 'A' || t[0] > 'Z'))
                    return false;
            }
            return true;
        }

        private void licKeyTxt_TextChanged(object sender, EventArgs e)
        {
            if(IsValidateLicenseText(licKeyTxt.Text))
            {
                licKeyTxt.ForeColor = Color.Blue;
                activateBtn.Enabled = true;
            }
            else
            {
                licKeyTxt.ForeColor = Color.Red;
                activateBtn.Enabled = false;
            }
        }

    }
}
