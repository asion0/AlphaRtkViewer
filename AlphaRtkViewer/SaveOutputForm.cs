using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RtkViewer
{
    
    public partial class SaveOutputForm : Form
    {
        public SaveOutputForm()
        {
            InitializeComponent();
        }

        private string file = "";
        public void SetFile(string s) { file = s; }

        private void SaveOutputForm_Load(object sender, EventArgs e)
        {
            fileTxt.Text = file;
            fileTxt.SelectionStart = fileTxt.TextLength;
            sizeLbl.Text = "0";
            fileTxt.ScrollToCaret();
        }

        public bool SaveOutput(byte[] b)
        {
            using (FileStream data = new FileStream(file, FileMode.Append))
            {
                data.Write(b, 0, b.Length);
                sizeLbl.Text = data.Length.ToString();
            }
            return true;
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
