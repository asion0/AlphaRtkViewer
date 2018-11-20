namespace RtkViewer
{
    partial class DetectingDeviceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetectingDeviceForm));
            this.progressLbl = new System.Windows.Forms.Label();
            this.msgLsb = new System.Windows.Forms.ListBox();
            this.detectWorker = new System.ComponentModel.BackgroundWorker();
            this.okBtn = new System.Windows.Forms.Button();
            this.infoPanel = new System.Windows.Forms.Panel();
            this.licenseLbl = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.optFunctionLbl = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.rtkModeLbl = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.receiverTypeLbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.noticePanel = new System.Windows.Forms.Panel();
            this.noticeLbl = new System.Windows.Forms.Label();
            this.infoPanel.SuspendLayout();
            this.noticePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressLbl
            // 
            this.progressLbl.Image = ((System.Drawing.Image)(resources.GetObject("progressLbl.Image")));
            this.progressLbl.Location = new System.Drawing.Point(267, 277);
            this.progressLbl.Name = "progressLbl";
            this.progressLbl.Size = new System.Drawing.Size(126, 109);
            this.progressLbl.TabIndex = 0;
            // 
            // msgLsb
            // 
            this.msgLsb.FormattingEnabled = true;
            this.msgLsb.ItemHeight = 12;
            this.msgLsb.Location = new System.Drawing.Point(12, 12);
            this.msgLsb.Name = "msgLsb";
            this.msgLsb.Size = new System.Drawing.Size(637, 244);
            this.msgLsb.TabIndex = 1;
            // 
            // detectWorker
            // 
            this.detectWorker.WorkerReportsProgress = true;
            this.detectWorker.WorkerSupportsCancellation = true;
            this.detectWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.detectWorker_DoWork);
            this.detectWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.detectWorker_ProgressChanged);
            this.detectWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.detectWorker_RunWorkerCompleted);
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(556, 397);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(93, 29);
            this.okBtn.TabIndex = 2;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // infoPanel
            // 
            this.infoPanel.Controls.Add(this.licenseLbl);
            this.infoPanel.Controls.Add(this.label4);
            this.infoPanel.Controls.Add(this.optFunctionLbl);
            this.infoPanel.Controls.Add(this.label3);
            this.infoPanel.Controls.Add(this.rtkModeLbl);
            this.infoPanel.Controls.Add(this.label2);
            this.infoPanel.Controls.Add(this.receiverTypeLbl);
            this.infoPanel.Controls.Add(this.label1);
            this.infoPanel.Location = new System.Drawing.Point(12, 263);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(361, 128);
            this.infoPanel.TabIndex = 3;
            this.infoPanel.Visible = false;
            // 
            // licenseLbl
            // 
            this.licenseLbl.AutoSize = true;
            this.licenseLbl.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.licenseLbl.Location = new System.Drawing.Point(153, 99);
            this.licenseLbl.Name = "licenseLbl";
            this.licenseLbl.Size = new System.Drawing.Size(29, 18);
            this.licenseLbl.TabIndex = 0;
            this.licenseLbl.Text = "---";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(14, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(133, 18);
            this.label4.TabIndex = 0;
            this.label4.Text = "License Period:";
            // 
            // optFunctionLbl
            // 
            this.optFunctionLbl.AutoSize = true;
            this.optFunctionLbl.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optFunctionLbl.Location = new System.Drawing.Point(191, 69);
            this.optFunctionLbl.Name = "optFunctionLbl";
            this.optFunctionLbl.Size = new System.Drawing.Size(29, 18);
            this.optFunctionLbl.TabIndex = 0;
            this.optFunctionLbl.Text = "---";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(14, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(171, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "Operation Function:";
            // 
            // rtkModeLbl
            // 
            this.rtkModeLbl.AutoSize = true;
            this.rtkModeLbl.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtkModeLbl.Location = new System.Drawing.Point(116, 39);
            this.rtkModeLbl.Name = "rtkModeLbl";
            this.rtkModeLbl.Size = new System.Drawing.Size(29, 18);
            this.rtkModeLbl.TabIndex = 0;
            this.rtkModeLbl.Text = "---";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(14, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 18);
            this.label2.TabIndex = 0;
            this.label2.Text = "RTK Mode:";
            // 
            // receiverTypeLbl
            // 
            this.receiverTypeLbl.AutoSize = true;
            this.receiverTypeLbl.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.receiverTypeLbl.Location = new System.Drawing.Point(147, 9);
            this.receiverTypeLbl.Name = "receiverTypeLbl";
            this.receiverTypeLbl.Size = new System.Drawing.Size(29, 18);
            this.receiverTypeLbl.TabIndex = 0;
            this.receiverTypeLbl.Text = "---";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Receiver Type:";
            // 
            // noticePanel
            // 
            this.noticePanel.Controls.Add(this.noticeLbl);
            this.noticePanel.Location = new System.Drawing.Point(12, 263);
            this.noticePanel.Name = "noticePanel";
            this.noticePanel.Size = new System.Drawing.Size(637, 128);
            this.noticePanel.TabIndex = 4;
            this.noticePanel.Visible = false;
            // 
            // noticeLbl
            // 
            this.noticeLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.noticeLbl.Location = new System.Drawing.Point(-1, 9);
            this.noticeLbl.Name = "noticeLbl";
            this.noticeLbl.Size = new System.Drawing.Size(638, 119);
            this.noticeLbl.TabIndex = 0;
            this.noticeLbl.Text = "Device detection failed! Please confirm that the baud rate and COM port are corre" +
    "ct.\r\n";
            // 
            // DetectingDeviceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 441);
            this.Controls.Add(this.noticePanel);
            this.Controls.Add(this.infoPanel);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.msgLsb);
            this.Controls.Add(this.progressLbl);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DetectingDeviceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Detecting...";
            this.Load += new System.EventHandler(this.DetectingDeviceForm_Load);
            this.infoPanel.ResumeLayout(false);
            this.infoPanel.PerformLayout();
            this.noticePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label progressLbl;
        private System.Windows.Forms.ListBox msgLsb;
        private System.ComponentModel.BackgroundWorker detectWorker;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Panel infoPanel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label licenseLbl;
        private System.Windows.Forms.Label optFunctionLbl;
        private System.Windows.Forms.Label rtkModeLbl;
        private System.Windows.Forms.Label receiverTypeLbl;
        private System.Windows.Forms.Panel noticePanel;
        private System.Windows.Forms.Label noticeLbl;
    }
}