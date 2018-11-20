using MiscUtil.App;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RtkViewer
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            Text = AppTools.GetAppTitleWithVersion();
            ver.Text = AppTools.GetAppTitleWithVersion();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
