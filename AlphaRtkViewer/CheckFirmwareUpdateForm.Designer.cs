namespace RtkViewer
{
    partial class CheckFirmwareUpdateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckFirmwareUpdateForm));
            this.label1 = new System.Windows.Forms.Label();
            this.downloadUpdatePanel = new System.Windows.Forms.Panel();
            this.progressLbl = new System.Windows.Forms.Label();
            this.checkFwOkBtn = new System.Windows.Forms.Button();
            this.progressTextLbl = new System.Windows.Forms.Label();
            this.checkFwPromptLbl = new System.Windows.Forms.Label();
            this.downloadStep2Panel = new System.Windows.Forms.Panel();
            this.opModeLbl = new System.Windows.Forms.Label();
            this.slaveGroup = new System.Windows.Forms.GroupBox();
            this.newFwKVerSLbl = new System.Windows.Forms.Label();
            this.newFwCrcSLbl = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.newFwRevSLbl = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.newFwSVerSLbl = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.newFwKVerMLbl = new System.Windows.Forms.Label();
            this.newFwCrcMLbl = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.newFwRevMLbl = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.newFwSVerMLbl = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.step2NoBtn = new System.Windows.Forms.Button();
            this.step2YesBtn = new System.Windows.Forms.Button();
            this.downloadStep2TitleLbl = new System.Windows.Forms.Label();
            this.downloadStep2PromptLbl = new System.Windows.Forms.Label();
            this.downloadStep3Panel = new System.Windows.Forms.Panel();
            this.downloadStep3ProgressLbl = new System.Windows.Forms.Label();
            this.downloadStep3Progress = new System.Windows.Forms.ProgressBar();
            this.label37 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.downloadChangePanel = new System.Windows.Forms.Panel();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.step0NoBtn = new System.Windows.Forms.Button();
            this.step0YesBtn = new System.Windows.Forms.Button();
            this.label39 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.downloadUpdatePanel.SuspendLayout();
            this.downloadStep2Panel.SuspendLayout();
            this.slaveGroup.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.downloadStep3Panel.SuspendLayout();
            this.downloadChangePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Leelawadee UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(141, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(223, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "Check Firmware Update";
            // 
            // downloadUpdatePanel
            // 
            this.downloadUpdatePanel.Controls.Add(this.progressLbl);
            this.downloadUpdatePanel.Controls.Add(this.checkFwOkBtn);
            this.downloadUpdatePanel.Controls.Add(this.progressTextLbl);
            this.downloadUpdatePanel.Controls.Add(this.label1);
            this.downloadUpdatePanel.Controls.Add(this.checkFwPromptLbl);
            this.downloadUpdatePanel.Location = new System.Drawing.Point(0, 0);
            this.downloadUpdatePanel.Name = "downloadUpdatePanel";
            this.downloadUpdatePanel.Size = new System.Drawing.Size(504, 360);
            this.downloadUpdatePanel.TabIndex = 5;
            // 
            // progressLbl
            // 
            this.progressLbl.Image = ((System.Drawing.Image)(resources.GetObject("progressLbl.Image")));
            this.progressLbl.Location = new System.Drawing.Point(189, 102);
            this.progressLbl.Name = "progressLbl";
            this.progressLbl.Size = new System.Drawing.Size(126, 109);
            this.progressLbl.TabIndex = 7;
            // 
            // checkFwOkBtn
            // 
            this.checkFwOkBtn.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.checkFwOkBtn.Location = new System.Drawing.Point(370, 318);
            this.checkFwOkBtn.Name = "checkFwOkBtn";
            this.checkFwOkBtn.Size = new System.Drawing.Size(120, 32);
            this.checkFwOkBtn.TabIndex = 11;
            this.checkFwOkBtn.Text = "OK";
            this.checkFwOkBtn.UseVisualStyleBackColor = true;
            this.checkFwOkBtn.Visible = false;
            this.checkFwOkBtn.Click += new System.EventHandler(this.checkFwOkBtn_Click);
            // 
            // progressTextLbl
            // 
            this.progressTextLbl.AutoSize = true;
            this.progressTextLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.progressTextLbl.Location = new System.Drawing.Point(115, 75);
            this.progressTextLbl.Name = "progressTextLbl";
            this.progressTextLbl.Size = new System.Drawing.Size(273, 20);
            this.progressTextLbl.TabIndex = 6;
            this.progressTextLbl.Text = "Checking for updates, please wait...";
            // 
            // checkFwPromptLbl
            // 
            this.checkFwPromptLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.checkFwPromptLbl.Location = new System.Drawing.Point(18, 75);
            this.checkFwPromptLbl.Name = "checkFwPromptLbl";
            this.checkFwPromptLbl.Size = new System.Drawing.Size(468, 66);
            this.checkFwPromptLbl.TabIndex = 4;
            this.checkFwPromptLbl.Text = "Test";
            this.checkFwPromptLbl.Visible = false;
            // 
            // downloadStep2Panel
            // 
            this.downloadStep2Panel.Controls.Add(this.opModeLbl);
            this.downloadStep2Panel.Controls.Add(this.slaveGroup);
            this.downloadStep2Panel.Controls.Add(this.groupBox2);
            this.downloadStep2Panel.Controls.Add(this.step2NoBtn);
            this.downloadStep2Panel.Controls.Add(this.step2YesBtn);
            this.downloadStep2Panel.Controls.Add(this.downloadStep2TitleLbl);
            this.downloadStep2Panel.Controls.Add(this.downloadStep2PromptLbl);
            this.downloadStep2Panel.Location = new System.Drawing.Point(514, 0);
            this.downloadStep2Panel.Name = "downloadStep2Panel";
            this.downloadStep2Panel.Size = new System.Drawing.Size(504, 360);
            this.downloadStep2Panel.TabIndex = 6;
            // 
            // opModeLbl
            // 
            this.opModeLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.opModeLbl.Location = new System.Drawing.Point(18, 173);
            this.opModeLbl.Name = "opModeLbl";
            this.opModeLbl.Size = new System.Drawing.Size(326, 21);
            this.opModeLbl.TabIndex = 31;
            this.opModeLbl.Text = "RTK Rover GPS + GLO";
            this.opModeLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // slaveGroup
            // 
            this.slaveGroup.Controls.Add(this.newFwKVerSLbl);
            this.slaveGroup.Controls.Add(this.newFwCrcSLbl);
            this.slaveGroup.Controls.Add(this.label6);
            this.slaveGroup.Controls.Add(this.newFwRevSLbl);
            this.slaveGroup.Controls.Add(this.label8);
            this.slaveGroup.Controls.Add(this.newFwSVerSLbl);
            this.slaveGroup.Controls.Add(this.label10);
            this.slaveGroup.Controls.Add(this.label11);
            this.slaveGroup.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.slaveGroup.Location = new System.Drawing.Point(255, 193);
            this.slaveGroup.Name = "slaveGroup";
            this.slaveGroup.Size = new System.Drawing.Size(234, 114);
            this.slaveGroup.TabIndex = 30;
            this.slaveGroup.TabStop = false;
            this.slaveGroup.Text = "Slave Firmware";
            // 
            // newFwKVerSLbl
            // 
            this.newFwKVerSLbl.AutoSize = true;
            this.newFwKVerSLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.newFwKVerSLbl.Location = new System.Drawing.Point(139, 27);
            this.newFwKVerSLbl.Name = "newFwKVerSLbl";
            this.newFwKVerSLbl.Size = new System.Drawing.Size(44, 20);
            this.newFwKVerSLbl.TabIndex = 23;
            this.newFwKVerSLbl.Text = "2.2.6";
            // 
            // newFwCrcSLbl
            // 
            this.newFwCrcSLbl.AutoSize = true;
            this.newFwCrcSLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.newFwCrcSLbl.Location = new System.Drawing.Point(139, 87);
            this.newFwCrcSLbl.Name = "newFwCrcSLbl";
            this.newFwCrcSLbl.Size = new System.Drawing.Size(45, 20);
            this.newFwCrcSLbl.TabIndex = 20;
            this.newFwCrcSLbl.Text = "0678";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label6.Location = new System.Drawing.Point(15, 27);
            this.label6.Name = "label6";
            this.label6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label6.Size = new System.Drawing.Size(118, 20);
            this.label6.TabIndex = 29;
            this.label6.Text = "Kernel Version";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // newFwRevSLbl
            // 
            this.newFwRevSLbl.AutoSize = true;
            this.newFwRevSLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.newFwRevSLbl.Location = new System.Drawing.Point(139, 67);
            this.newFwRevSLbl.Name = "newFwRevSLbl";
            this.newFwRevSLbl.Size = new System.Drawing.Size(81, 20);
            this.newFwRevSLbl.TabIndex = 21;
            this.newFwRevSLbl.Text = "20180826";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label8.Location = new System.Drawing.Point(62, 67);
            this.label8.Name = "label8";
            this.label8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label8.Size = new System.Drawing.Size(71, 20);
            this.label8.TabIndex = 28;
            this.label8.Text = "Revision";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // newFwSVerSLbl
            // 
            this.newFwSVerSLbl.AutoSize = true;
            this.newFwSVerSLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.newFwSVerSLbl.Location = new System.Drawing.Point(139, 47);
            this.newFwSVerSLbl.Name = "newFwSVerSLbl";
            this.newFwSVerSLbl.Size = new System.Drawing.Size(53, 20);
            this.newFwSVerSLbl.TabIndex = 22;
            this.newFwSVerSLbl.Text = "1.7.28";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label10.Location = new System.Drawing.Point(92, 87);
            this.label10.Name = "label10";
            this.label10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label10.Size = new System.Drawing.Size(41, 20);
            this.label10.TabIndex = 27;
            this.label10.Text = "CRC";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label11.Location = new System.Drawing.Point(30, 47);
            this.label11.Name = "label11";
            this.label11.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label11.Size = new System.Drawing.Size(103, 20);
            this.label11.TabIndex = 24;
            this.label11.Text = "S.W. Version";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.newFwKVerMLbl);
            this.groupBox2.Controls.Add(this.newFwCrcMLbl);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.newFwRevMLbl);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.newFwSVerMLbl);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox2.Location = new System.Drawing.Point(15, 193);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(234, 114);
            this.groupBox2.TabIndex = 30;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Master Firmware";
            // 
            // newFwKVerMLbl
            // 
            this.newFwKVerMLbl.AutoSize = true;
            this.newFwKVerMLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.newFwKVerMLbl.Location = new System.Drawing.Point(139, 27);
            this.newFwKVerMLbl.Name = "newFwKVerMLbl";
            this.newFwKVerMLbl.Size = new System.Drawing.Size(44, 20);
            this.newFwKVerMLbl.TabIndex = 23;
            this.newFwKVerMLbl.Text = "2.2.6";
            // 
            // newFwCrcMLbl
            // 
            this.newFwCrcMLbl.AutoSize = true;
            this.newFwCrcMLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.newFwCrcMLbl.Location = new System.Drawing.Point(139, 87);
            this.newFwCrcMLbl.Name = "newFwCrcMLbl";
            this.newFwCrcMLbl.Size = new System.Drawing.Size(45, 20);
            this.newFwCrcMLbl.TabIndex = 20;
            this.newFwCrcMLbl.Text = "0678";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label12.Location = new System.Drawing.Point(15, 27);
            this.label12.Name = "label12";
            this.label12.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label12.Size = new System.Drawing.Size(118, 20);
            this.label12.TabIndex = 29;
            this.label12.Text = "Kernel Version";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // newFwRevMLbl
            // 
            this.newFwRevMLbl.AutoSize = true;
            this.newFwRevMLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.newFwRevMLbl.Location = new System.Drawing.Point(139, 67);
            this.newFwRevMLbl.Name = "newFwRevMLbl";
            this.newFwRevMLbl.Size = new System.Drawing.Size(81, 20);
            this.newFwRevMLbl.TabIndex = 21;
            this.newFwRevMLbl.Text = "20180826";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label14.Location = new System.Drawing.Point(62, 67);
            this.label14.Name = "label14";
            this.label14.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label14.Size = new System.Drawing.Size(71, 20);
            this.label14.TabIndex = 28;
            this.label14.Text = "Revision";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // newFwSVerMLbl
            // 
            this.newFwSVerMLbl.AutoSize = true;
            this.newFwSVerMLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.newFwSVerMLbl.Location = new System.Drawing.Point(139, 47);
            this.newFwSVerMLbl.Name = "newFwSVerMLbl";
            this.newFwSVerMLbl.Size = new System.Drawing.Size(53, 20);
            this.newFwSVerMLbl.TabIndex = 22;
            this.newFwSVerMLbl.Text = "1.7.28";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label19.Location = new System.Drawing.Point(92, 87);
            this.label19.Name = "label19";
            this.label19.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label19.Size = new System.Drawing.Size(41, 20);
            this.label19.TabIndex = 27;
            this.label19.Text = "CRC";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label20.Location = new System.Drawing.Point(30, 47);
            this.label20.Name = "label20";
            this.label20.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label20.Size = new System.Drawing.Size(103, 20);
            this.label20.TabIndex = 24;
            this.label20.Text = "S.W. Version";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // step2NoBtn
            // 
            this.step2NoBtn.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.step2NoBtn.Location = new System.Drawing.Point(166, 317);
            this.step2NoBtn.Name = "step2NoBtn";
            this.step2NoBtn.Size = new System.Drawing.Size(120, 32);
            this.step2NoBtn.TabIndex = 11;
            this.step2NoBtn.Text = "No";
            this.step2NoBtn.UseVisualStyleBackColor = true;
            this.step2NoBtn.Click += new System.EventHandler(this.step2NoBtn_Click);
            // 
            // step2YesBtn
            // 
            this.step2YesBtn.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.step2YesBtn.Location = new System.Drawing.Point(297, 317);
            this.step2YesBtn.Name = "step2YesBtn";
            this.step2YesBtn.Size = new System.Drawing.Size(200, 32);
            this.step2YesBtn.TabIndex = 10;
            this.step2YesBtn.Text = "Download and Update";
            this.step2YesBtn.UseVisualStyleBackColor = true;
            this.step2YesBtn.Click += new System.EventHandler(this.step2YesBtn_Click);
            // 
            // downloadStep2TitleLbl
            // 
            this.downloadStep2TitleLbl.Font = new System.Drawing.Font("Leelawadee UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downloadStep2TitleLbl.ForeColor = System.Drawing.Color.Blue;
            this.downloadStep2TitleLbl.Location = new System.Drawing.Point(73, 13);
            this.downloadStep2TitleLbl.Name = "downloadStep2TitleLbl";
            this.downloadStep2TitleLbl.Size = new System.Drawing.Size(359, 50);
            this.downloadStep2TitleLbl.TabIndex = 3;
            this.downloadStep2TitleLbl.Text = "New Firmware Found";
            this.downloadStep2TitleLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // downloadStep2PromptLbl
            // 
            this.downloadStep2PromptLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.downloadStep2PromptLbl.Location = new System.Drawing.Point(18, 69);
            this.downloadStep2PromptLbl.Name = "downloadStep2PromptLbl";
            this.downloadStep2PromptLbl.Size = new System.Drawing.Size(468, 101);
            this.downloadStep2PromptLbl.TabIndex = 4;
            this.downloadStep2PromptLbl.Text = "New firmware is now available.\r\nWould you like to download it from the server and" +
    " update onto it?\r\n";
            // 
            // downloadStep3Panel
            // 
            this.downloadStep3Panel.Controls.Add(this.downloadStep3ProgressLbl);
            this.downloadStep3Panel.Controls.Add(this.downloadStep3Progress);
            this.downloadStep3Panel.Controls.Add(this.label37);
            this.downloadStep3Panel.Controls.Add(this.label38);
            this.downloadStep3Panel.Location = new System.Drawing.Point(514, 371);
            this.downloadStep3Panel.Name = "downloadStep3Panel";
            this.downloadStep3Panel.Size = new System.Drawing.Size(504, 360);
            this.downloadStep3Panel.TabIndex = 31;
            // 
            // downloadStep3ProgressLbl
            // 
            this.downloadStep3ProgressLbl.AutoSize = true;
            this.downloadStep3ProgressLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.downloadStep3ProgressLbl.Location = new System.Drawing.Point(18, 169);
            this.downloadStep3ProgressLbl.Name = "downloadStep3ProgressLbl";
            this.downloadStep3ProgressLbl.Size = new System.Drawing.Size(122, 20);
            this.downloadStep3ProgressLbl.TabIndex = 6;
            this.downloadStep3ProgressLbl.Text = "Downloading...";
            // 
            // downloadStep3Progress
            // 
            this.downloadStep3Progress.Location = new System.Drawing.Point(18, 193);
            this.downloadStep3Progress.Name = "downloadStep3Progress";
            this.downloadStep3Progress.Size = new System.Drawing.Size(464, 23);
            this.downloadStep3Progress.Step = 5;
            this.downloadStep3Progress.TabIndex = 5;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Font = new System.Drawing.Font("Leelawadee UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label37.ForeColor = System.Drawing.Color.Blue;
            this.label37.Location = new System.Drawing.Point(170, 25);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(165, 25);
            this.label37.TabIndex = 3;
            this.label37.Text = "Firmware Update";
            // 
            // label38
            // 
            this.label38.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label38.ForeColor = System.Drawing.Color.Red;
            this.label38.Location = new System.Drawing.Point(18, 75);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(468, 96);
            this.label38.TabIndex = 4;
            this.label38.Text = "WARNING!\r\nFirmware update is a critial operation. Please make sure the power stay" +
    "s on and do not disconnect the device before the update is complete.";
            // 
            // downloadChangePanel
            // 
            this.downloadChangePanel.Controls.Add(this.radioButton3);
            this.downloadChangePanel.Controls.Add(this.radioButton2);
            this.downloadChangePanel.Controls.Add(this.radioButton1);
            this.downloadChangePanel.Controls.Add(this.step0NoBtn);
            this.downloadChangePanel.Controls.Add(this.step0YesBtn);
            this.downloadChangePanel.Controls.Add(this.label39);
            this.downloadChangePanel.Controls.Add(this.label40);
            this.downloadChangePanel.Location = new System.Drawing.Point(-2, 371);
            this.downloadChangePanel.Name = "downloadChangePanel";
            this.downloadChangePanel.Size = new System.Drawing.Size(504, 360);
            this.downloadChangePanel.TabIndex = 33;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioButton3.Location = new System.Drawing.Point(132, 180);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(62, 24);
            this.radioButton3.TabIndex = 12;
            this.radioButton3.Text = "ODR";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.Visible = false;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioButton2.Location = new System.Drawing.Point(132, 134);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(226, 24);
            this.radioButton2.TabIndex = 12;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "GPS/BEIDOU RTK Receiver";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Enabled = false;
            this.radioButton1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioButton1.Location = new System.Drawing.Point(132, 88);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(240, 24);
            this.radioButton1.TabIndex = 12;
            this.radioButton1.Text = "GPS/GLONASS RTK Receiver";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // step0NoBtn
            // 
            this.step0NoBtn.Location = new System.Drawing.Point(370, 319);
            this.step0NoBtn.Name = "step0NoBtn";
            this.step0NoBtn.Size = new System.Drawing.Size(120, 30);
            this.step0NoBtn.TabIndex = 11;
            this.step0NoBtn.Text = "No";
            this.step0NoBtn.UseVisualStyleBackColor = true;
            this.step0NoBtn.Click += new System.EventHandler(this.step0NoBtn_Click);
            // 
            // step0YesBtn
            // 
            this.step0YesBtn.Location = new System.Drawing.Point(244, 319);
            this.step0YesBtn.Name = "step0YesBtn";
            this.step0YesBtn.Size = new System.Drawing.Size(120, 30);
            this.step0YesBtn.TabIndex = 10;
            this.step0YesBtn.Text = "Yes";
            this.step0YesBtn.UseVisualStyleBackColor = true;
            this.step0YesBtn.Click += new System.EventHandler(this.step0YesBtn_Click);
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Font = new System.Drawing.Font("Leelawadee UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label39.ForeColor = System.Drawing.Color.Blue;
            this.label39.Location = new System.Drawing.Point(72, 25);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(361, 25);
            this.label39.TabIndex = 3;
            this.label39.Text = "Alpha RTK Receiver Type Configuration";
            // 
            // label40
            // 
            this.label40.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label40.Location = new System.Drawing.Point(15, 197);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(475, 44);
            this.label40.TabIndex = 4;
            this.label40.Text = "When changing type configuration, RTK Viewer will fetch firmware from Polaris ser" +
    "ver to update Alpha.";
            // 
            // CheckFirmwareUpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 360);
            this.Controls.Add(this.downloadStep2Panel);
            this.Controls.Add(this.downloadStep3Panel);
            this.Controls.Add(this.downloadChangePanel);
            this.Controls.Add(this.downloadUpdatePanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckFirmwareUpdateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Alpha RTK Receiver Type Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CheckFirmwareUpdateForm_FormClosing);
            this.Load += new System.EventHandler(this.CheckFirmwareUpdateForm_Load);
            this.downloadUpdatePanel.ResumeLayout(false);
            this.downloadUpdatePanel.PerformLayout();
            this.downloadStep2Panel.ResumeLayout(false);
            this.slaveGroup.ResumeLayout(false);
            this.slaveGroup.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.downloadStep3Panel.ResumeLayout(false);
            this.downloadStep3Panel.PerformLayout();
            this.downloadChangePanel.ResumeLayout(false);
            this.downloadChangePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel downloadUpdatePanel;
        private System.Windows.Forms.Panel downloadStep2Panel;
        private System.Windows.Forms.Label downloadStep2TitleLbl;
        private System.Windows.Forms.Label downloadStep2PromptLbl;
        private System.Windows.Forms.Button step2NoBtn;
        private System.Windows.Forms.Button step2YesBtn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label newFwKVerMLbl;
        private System.Windows.Forms.Label newFwCrcMLbl;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label newFwRevMLbl;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label newFwSVerMLbl;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Panel downloadStep3Panel;
        private System.Windows.Forms.ProgressBar downloadStep3Progress;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Panel downloadChangePanel;
        private System.Windows.Forms.Button step0NoBtn;
        private System.Windows.Forms.Button step0YesBtn;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label progressLbl;
        private System.Windows.Forms.Label progressTextLbl;
        private System.Windows.Forms.Label checkFwPromptLbl;
        private System.Windows.Forms.Button checkFwOkBtn;
        private System.Windows.Forms.GroupBox slaveGroup;
        private System.Windows.Forms.Label newFwKVerSLbl;
        private System.Windows.Forms.Label newFwCrcSLbl;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label newFwRevSLbl;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label newFwSVerSLbl;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label downloadStep3ProgressLbl;
        private System.Windows.Forms.Label opModeLbl;
    }
}