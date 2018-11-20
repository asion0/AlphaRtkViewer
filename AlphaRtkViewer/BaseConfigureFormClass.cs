using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RtkViewer
{
    public class BaseConfigureFormClass : Form
    {
        public static int ConfigureFormWidth = 520;
        public static int ConfigureFormHeight = 400;
        protected override void OnLoad(EventArgs e)
        {
            if(this.Width > ConfigureFormWidth || this.Height > ConfigureFormHeight)
            {
                this.Width = ConfigureFormWidth;
                this.Height = ConfigureFormHeight;
                this.CenterToParent();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BaseConfigureFormClass
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "BaseConfigureFormClass";
            this.Load += new System.EventHandler(this.Form_Load);
            this.ResumeLayout(false);

        }

        public virtual void Form_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Base_Load");
        }
    }
}
