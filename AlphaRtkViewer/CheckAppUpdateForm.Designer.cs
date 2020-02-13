namespace RtkViewer
{
    partial class CheckAppUpdateForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckAppUpdateForm));
            this.softwareCheckStep1Panel = new System.Windows.Forms.Panel();
            this.progressLbl = new System.Windows.Forms.Label();
            this.progressTextLbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.softwareCheckStep3Panel = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.softwareCheclStep3CancelBtn = new System.Windows.Forms.Button();
            this.softwareCheclStep3DownloadBtn = new System.Windows.Forms.Button();
            this.softwareCheckStep3DontAskChk = new System.Windows.Forms.CheckBox();
            this.softwareCheclStep3PromptLbl = new System.Windows.Forms.Label();
            this.softwareCheckStep2Panel = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.softwareCheclStep2OkBtn = new System.Windows.Forms.Button();
            this.softwareCheckStep2DontAskChk = new System.Windows.Forms.CheckBox();
            this.softwareCheclStep2PromptLbl = new System.Windows.Forms.Label();
            this.softwareCheckStep1Panel.SuspendLayout();
            this.softwareCheckStep3Panel.SuspendLayout();
            this.softwareCheckStep2Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // softwareCheckStep1Panel
            // 
            this.softwareCheckStep1Panel.Controls.Add(this.progressLbl);
            this.softwareCheckStep1Panel.Controls.Add(this.progressTextLbl);
            this.softwareCheckStep1Panel.Controls.Add(this.label1);
            this.softwareCheckStep1Panel.Location = new System.Drawing.Point(0, 0);
            this.softwareCheckStep1Panel.Name = "softwareCheckStep1Panel";
            this.softwareCheckStep1Panel.Size = new System.Drawing.Size(504, 360);
            this.softwareCheckStep1Panel.TabIndex = 0;
            // 
            // progressLbl
            // 
            this.progressLbl.Image = ((System.Drawing.Image)(resources.GetObject("progressLbl.Image")));
            this.progressLbl.Location = new System.Drawing.Point(189, 102);
            this.progressLbl.Name = "progressLbl";
            this.progressLbl.Size = new System.Drawing.Size(126, 109);
            this.progressLbl.TabIndex = 5;
            // 
            // progressTextLbl
            // 
            this.progressTextLbl.AutoSize = true;
            this.progressTextLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.progressTextLbl.Location = new System.Drawing.Point(115, 75);
            this.progressTextLbl.Name = "progressTextLbl";
            this.progressTextLbl.Size = new System.Drawing.Size(273, 20);
            this.progressTextLbl.TabIndex = 3;
            this.progressTextLbl.Text = "Checking for updates, please wait...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Leelawadee UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(131, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(241, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "Check RTK Viewer Update";
            // 
            // softwareCheckStep3Panel
            // 
            this.softwareCheckStep3Panel.Controls.Add(this.label4);
            this.softwareCheckStep3Panel.Controls.Add(this.softwareCheclStep3CancelBtn);
            this.softwareCheckStep3Panel.Controls.Add(this.softwareCheclStep3DownloadBtn);
            this.softwareCheckStep3Panel.Controls.Add(this.softwareCheckStep3DontAskChk);
            this.softwareCheckStep3Panel.Controls.Add(this.softwareCheclStep3PromptLbl);
            this.softwareCheckStep3Panel.Location = new System.Drawing.Point(510, 0);
            this.softwareCheckStep3Panel.Name = "softwareCheckStep3Panel";
            this.softwareCheckStep3Panel.Size = new System.Drawing.Size(504, 360);
            this.softwareCheckStep3Panel.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Leelawadee UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(131, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(241, 25);
            this.label4.TabIndex = 4;
            this.label4.Text = "Check RTK Viewer Update";
            // 
            // softwareCheclStep3CancelBtn
            // 
            this.softwareCheclStep3CancelBtn.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.softwareCheclStep3CancelBtn.Location = new System.Drawing.Point(163, 317);
            this.softwareCheclStep3CancelBtn.Name = "softwareCheclStep3CancelBtn";
            this.softwareCheclStep3CancelBtn.Size = new System.Drawing.Size(120, 32);
            this.softwareCheclStep3CancelBtn.TabIndex = 2;
            this.softwareCheclStep3CancelBtn.Text = "Cancel";
            this.softwareCheclStep3CancelBtn.UseVisualStyleBackColor = true;
            this.softwareCheclStep3CancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // softwareCheclStep3DownloadBtn
            // 
            this.softwareCheclStep3DownloadBtn.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.softwareCheclStep3DownloadBtn.Location = new System.Drawing.Point(294, 317);
            this.softwareCheclStep3DownloadBtn.Name = "softwareCheclStep3DownloadBtn";
            this.softwareCheclStep3DownloadBtn.Size = new System.Drawing.Size(200, 32);
            this.softwareCheclStep3DownloadBtn.TabIndex = 2;
            this.softwareCheclStep3DownloadBtn.Text = "Download and Update";
            this.softwareCheclStep3DownloadBtn.UseVisualStyleBackColor = true;
            this.softwareCheclStep3DownloadBtn.Click += new System.EventHandler(this.downloadBtn_Click);
            // 
            // softwareCheckStep3DontAskChk
            // 
            this.softwareCheckStep3DontAskChk.AutoSize = true;
            this.softwareCheckStep3DontAskChk.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.softwareCheckStep3DontAskChk.Location = new System.Drawing.Point(28, 276);
            this.softwareCheckStep3DontAskChk.Name = "softwareCheckStep3DontAskChk";
            this.softwareCheckStep3DontAskChk.Size = new System.Drawing.Size(298, 23);
            this.softwareCheckStep3DontAskChk.TabIndex = 1;
            this.softwareCheckStep3DontAskChk.Text = "Automatic application update checking";
            this.softwareCheckStep3DontAskChk.UseVisualStyleBackColor = true;
            this.softwareCheckStep3DontAskChk.CheckedChanged += new System.EventHandler(this.dontAskChk_CheckedChanged);
            // 
            // softwareCheclStep3PromptLbl
            // 
            this.softwareCheclStep3PromptLbl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.softwareCheclStep3PromptLbl.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.softwareCheclStep3PromptLbl.Location = new System.Drawing.Point(24, 74);
            this.softwareCheclStep3PromptLbl.Name = "softwareCheclStep3PromptLbl";
            this.softwareCheclStep3PromptLbl.Size = new System.Drawing.Size(457, 128);
            this.softwareCheclStep3PromptLbl.TabIndex = 0;
            this.softwareCheclStep3PromptLbl.Text = "A new RTK Viewer version (1.0.1) is available. World you like to download it and " +
    "update your software now?";
            this.softwareCheclStep3PromptLbl.Visible = false;
            // 
            // softwareCheckStep2Panel
            // 
            this.softwareCheckStep2Panel.Controls.Add(this.label8);
            this.softwareCheckStep2Panel.Controls.Add(this.softwareCheclStep2OkBtn);
            this.softwareCheckStep2Panel.Controls.Add(this.softwareCheckStep2DontAskChk);
            this.softwareCheckStep2Panel.Controls.Add(this.softwareCheclStep2PromptLbl);
            this.softwareCheckStep2Panel.Location = new System.Drawing.Point(0, 366);
            this.softwareCheckStep2Panel.Name = "softwareCheckStep2Panel";
            this.softwareCheckStep2Panel.Size = new System.Drawing.Size(504, 360);
            this.softwareCheckStep2Panel.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Leelawadee UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Blue;
            this.label8.Location = new System.Drawing.Point(131, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(241, 25);
            this.label8.TabIndex = 4;
            this.label8.Text = "Check RTK Viewer Update";
            // 
            // softwareCheclStep2OkBtn
            // 
            this.softwareCheclStep2OkBtn.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.softwareCheclStep2OkBtn.Location = new System.Drawing.Point(370, 318);
            this.softwareCheclStep2OkBtn.Name = "softwareCheclStep2OkBtn";
            this.softwareCheclStep2OkBtn.Size = new System.Drawing.Size(120, 32);
            this.softwareCheclStep2OkBtn.TabIndex = 2;
            this.softwareCheclStep2OkBtn.Text = "OK";
            this.softwareCheclStep2OkBtn.UseVisualStyleBackColor = true;
            this.softwareCheclStep2OkBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // softwareCheckStep2DontAskChk
            // 
            this.softwareCheckStep2DontAskChk.AutoSize = true;
            this.softwareCheckStep2DontAskChk.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.softwareCheckStep2DontAskChk.Location = new System.Drawing.Point(28, 276);
            this.softwareCheckStep2DontAskChk.Name = "softwareCheckStep2DontAskChk";
            this.softwareCheckStep2DontAskChk.Size = new System.Drawing.Size(298, 23);
            this.softwareCheckStep2DontAskChk.TabIndex = 1;
            this.softwareCheckStep2DontAskChk.Text = "Automatic application update checking";
            this.softwareCheckStep2DontAskChk.UseVisualStyleBackColor = true;
            this.softwareCheckStep2DontAskChk.CheckedChanged += new System.EventHandler(this.dontAskChk_CheckedChanged);
            // 
            // softwareCheclStep2PromptLbl
            // 
            this.softwareCheclStep2PromptLbl.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.softwareCheclStep2PromptLbl.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.softwareCheclStep2PromptLbl.Location = new System.Drawing.Point(24, 74);
            this.softwareCheclStep2PromptLbl.Name = "softwareCheclStep2PromptLbl";
            this.softwareCheclStep2PromptLbl.Size = new System.Drawing.Size(457, 128);
            this.softwareCheclStep2PromptLbl.TabIndex = 0;
            this.softwareCheclStep2PromptLbl.Text = "A new RTK Viewer version (1.0.1) is available. World you like to download it and " +
    "update your software now?";
            this.softwareCheclStep2PromptLbl.Visible = false;
            // 
            // CheckAppUpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 360);
            this.Controls.Add(this.softwareCheckStep2Panel);
            this.Controls.Add(this.softwareCheckStep3Panel);
            this.Controls.Add(this.softwareCheckStep1Panel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckAppUpdateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MyMessageBox";
            this.Load += new System.EventHandler(this.CheckAppUpdateForm_Load);
            this.softwareCheckStep1Panel.ResumeLayout(false);
            this.softwareCheckStep1Panel.PerformLayout();
            this.softwareCheckStep3Panel.ResumeLayout(false);
            this.softwareCheckStep3Panel.PerformLayout();
            this.softwareCheckStep2Panel.ResumeLayout(false);
            this.softwareCheckStep2Panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel softwareCheckStep1Panel;
        private System.Windows.Forms.Label progressTextLbl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label progressLbl;
        private System.Windows.Forms.Panel softwareCheckStep3Panel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button softwareCheclStep3CancelBtn;
        private System.Windows.Forms.Button softwareCheclStep3DownloadBtn;
        private System.Windows.Forms.CheckBox softwareCheckStep3DontAskChk;
        private System.Windows.Forms.Label softwareCheclStep3PromptLbl;
        private System.Windows.Forms.Panel softwareCheckStep2Panel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button softwareCheclStep2OkBtn;
        private System.Windows.Forms.CheckBox softwareCheckStep2DontAskChk;
        private System.Windows.Forms.Label softwareCheclStep2PromptLbl;
    }
}