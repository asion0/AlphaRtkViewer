using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RtkViewer
{
    public partial class TestForm : RtkViewer.BaseConfigureFormClass
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Test_Load");
        }
    }
}
