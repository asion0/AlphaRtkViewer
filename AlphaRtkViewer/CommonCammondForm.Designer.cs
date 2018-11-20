namespace RtkViewer
{
    partial class CommonCammondForm
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
            this.setFactoryDefaultPanel = new System.Windows.Forms.Panel();
            this.setFactoryDefaultAccaptBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.setFactoryDefaultCancelBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.configureSerialPortAcceptBtn = new System.Windows.Forms.Button();
            this.configureSerialPortCancelBtn = new System.Windows.Forms.Button();
            this.configureSerialPortPanel = new System.Windows.Forms.Panel();
            this.label35 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButton7 = new System.Windows.Forms.RadioButton();
            this.radioButton6 = new System.Windows.Forms.RadioButton();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.queryRtkModePanel = new System.Windows.Forms.Panel();
            this.rtkBasePanel = new System.Windows.Forms.Panel();
            this.label25 = new System.Windows.Forms.Label();
            this.runtimeRtkOpLbl = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.savedRtkAltLbl = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.savedRtkLonLbl = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.savedRtkLatLbl = new System.Windows.Forms.Label();
            this.runtimeRtkOpTLbl = new System.Windows.Forms.Label();
            this.savedRtkOpLbl = new System.Windows.Forms.Label();
            this.queryRtkModeOkBtn = new System.Windows.Forms.Button();
            this.rtkModeLbl = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.configurePositionUpdateRatePanel = new System.Windows.Forms.Panel();
            this.radioButton9 = new System.Windows.Forms.RadioButton();
            this.radioButton10 = new System.Windows.Forms.RadioButton();
            this.configurePositionUpdateRateCancelBtn = new System.Windows.Forms.Button();
            this.configurePositionUpdateRateAcceptBtn = new System.Windows.Forms.Button();
            this.radioButton11 = new System.Windows.Forms.RadioButton();
            this.radioButton12 = new System.Windows.Forms.RadioButton();
            this.radioButton13 = new System.Windows.Forms.RadioButton();
            this.radioButton14 = new System.Windows.Forms.RadioButton();
            this.label36 = new System.Windows.Forms.Label();
            this.setFactoryDefaultPanel.SuspendLayout();
            this.configureSerialPortPanel.SuspendLayout();
            this.queryRtkModePanel.SuspendLayout();
            this.rtkBasePanel.SuspendLayout();
            this.configurePositionUpdateRatePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // setFactoryDefaultPanel
            // 
            this.setFactoryDefaultPanel.Controls.Add(this.setFactoryDefaultAccaptBtn);
            this.setFactoryDefaultPanel.Controls.Add(this.label2);
            this.setFactoryDefaultPanel.Controls.Add(this.setFactoryDefaultCancelBtn);
            this.setFactoryDefaultPanel.Controls.Add(this.label1);
            this.setFactoryDefaultPanel.Location = new System.Drawing.Point(560, 0);
            this.setFactoryDefaultPanel.Name = "setFactoryDefaultPanel";
            this.setFactoryDefaultPanel.Size = new System.Drawing.Size(504, 360);
            this.setFactoryDefaultPanel.TabIndex = 0;
            // 
            // setFactoryDefaultAccaptBtn
            // 
            this.setFactoryDefaultAccaptBtn.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.setFactoryDefaultAccaptBtn.Location = new System.Drawing.Point(246, 317);
            this.setFactoryDefaultAccaptBtn.Name = "setFactoryDefaultAccaptBtn";
            this.setFactoryDefaultAccaptBtn.Size = new System.Drawing.Size(120, 32);
            this.setFactoryDefaultAccaptBtn.TabIndex = 19;
            this.setFactoryDefaultAccaptBtn.Text = "Accept";
            this.setFactoryDefaultAccaptBtn.UseVisualStyleBackColor = true;
            this.setFactoryDefaultAccaptBtn.Click += new System.EventHandler(this.setFactoryDefaultOkBtn_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(13, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(480, 191);
            this.label2.TabIndex = 4;
            this.label2.Text = "Set factory default will erase all settings of your RTK device. Include RTK opera" +
    "ting mode, output message type etc. Do you want to continue?\r\n";
            // 
            // setFactoryDefaultCancelBtn
            // 
            this.setFactoryDefaultCancelBtn.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.setFactoryDefaultCancelBtn.Location = new System.Drawing.Point(372, 317);
            this.setFactoryDefaultCancelBtn.Name = "setFactoryDefaultCancelBtn";
            this.setFactoryDefaultCancelBtn.Size = new System.Drawing.Size(120, 32);
            this.setFactoryDefaultCancelBtn.TabIndex = 2;
            this.setFactoryDefaultCancelBtn.Text = "Cancel";
            this.setFactoryDefaultCancelBtn.UseVisualStyleBackColor = true;
            this.setFactoryDefaultCancelBtn.Click += new System.EventHandler(this.configurePositionUpdateRateCancelBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Leelawadee UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(197, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "WARNING!";
            // 
            // configureSerialPortAcceptBtn
            // 
            this.configureSerialPortAcceptBtn.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.configureSerialPortAcceptBtn.Location = new System.Drawing.Point(246, 317);
            this.configureSerialPortAcceptBtn.Name = "configureSerialPortAcceptBtn";
            this.configureSerialPortAcceptBtn.Size = new System.Drawing.Size(120, 32);
            this.configureSerialPortAcceptBtn.TabIndex = 1;
            this.configureSerialPortAcceptBtn.Text = "Accept";
            this.configureSerialPortAcceptBtn.UseVisualStyleBackColor = true;
            this.configureSerialPortAcceptBtn.Click += new System.EventHandler(this.configureSerialPortAcceptBtn_Click);
            // 
            // configureSerialPortCancelBtn
            // 
            this.configureSerialPortCancelBtn.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.configureSerialPortCancelBtn.Location = new System.Drawing.Point(372, 317);
            this.configureSerialPortCancelBtn.Name = "configureSerialPortCancelBtn";
            this.configureSerialPortCancelBtn.Size = new System.Drawing.Size(120, 32);
            this.configureSerialPortCancelBtn.TabIndex = 2;
            this.configureSerialPortCancelBtn.Text = "Cancel";
            this.configureSerialPortCancelBtn.UseVisualStyleBackColor = true;
            this.configureSerialPortCancelBtn.Click += new System.EventHandler(this.configureSerialPortCancelBtn_Click);
            // 
            // configureSerialPortPanel
            // 
            this.configureSerialPortPanel.Controls.Add(this.label35);
            this.configureSerialPortPanel.Controls.Add(this.label3);
            this.configureSerialPortPanel.Controls.Add(this.radioButton7);
            this.configureSerialPortPanel.Controls.Add(this.radioButton6);
            this.configureSerialPortPanel.Controls.Add(this.radioButton5);
            this.configureSerialPortPanel.Controls.Add(this.configureSerialPortCancelBtn);
            this.configureSerialPortPanel.Controls.Add(this.configureSerialPortAcceptBtn);
            this.configureSerialPortPanel.Controls.Add(this.radioButton4);
            this.configureSerialPortPanel.Controls.Add(this.radioButton3);
            this.configureSerialPortPanel.Controls.Add(this.radioButton2);
            this.configureSerialPortPanel.Controls.Add(this.label4);
            this.configureSerialPortPanel.Location = new System.Drawing.Point(594, 154);
            this.configureSerialPortPanel.Name = "configureSerialPortPanel";
            this.configureSerialPortPanel.Size = new System.Drawing.Size(504, 360);
            this.configureSerialPortPanel.TabIndex = 0;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label35.ForeColor = System.Drawing.Color.Red;
            this.label35.Location = new System.Drawing.Point(217, 61);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(70, 21);
            this.label35.TabIndex = 6;
            this.label35.Text = "NOTICE";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(73, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(358, 44);
            this.label3.TabIndex = 5;
            this.label3.Text = "Low baud rate will lose output data and cause commands timeout";
            // 
            // radioButton7
            // 
            this.radioButton7.AutoSize = true;
            this.radioButton7.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioButton7.Location = new System.Drawing.Point(265, 198);
            this.radioButton7.Name = "radioButton7";
            this.radioButton7.Size = new System.Drawing.Size(81, 23);
            this.radioButton7.TabIndex = 4;
            this.radioButton7.Text = "230400";
            this.radioButton7.UseVisualStyleBackColor = true;
            this.radioButton7.Click += new System.EventHandler(this.baudrateRadio_clicked);
            // 
            // radioButton6
            // 
            this.radioButton6.AutoSize = true;
            this.radioButton6.Checked = true;
            this.radioButton6.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioButton6.Location = new System.Drawing.Point(265, 170);
            this.radioButton6.Name = "radioButton6";
            this.radioButton6.Size = new System.Drawing.Size(81, 23);
            this.radioButton6.TabIndex = 4;
            this.radioButton6.TabStop = true;
            this.radioButton6.Text = "115200";
            this.radioButton6.UseVisualStyleBackColor = true;
            this.radioButton6.Click += new System.EventHandler(this.baudrateRadio_clicked);
            // 
            // radioButton5
            // 
            this.radioButton5.AutoSize = true;
            this.radioButton5.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioButton5.Location = new System.Drawing.Point(265, 142);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(72, 23);
            this.radioButton5.TabIndex = 4;
            this.radioButton5.Text = "57600";
            this.radioButton5.UseVisualStyleBackColor = true;
            this.radioButton5.Click += new System.EventHandler(this.baudrateRadio_clicked);
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioButton4.Location = new System.Drawing.Point(159, 198);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(72, 23);
            this.radioButton4.TabIndex = 4;
            this.radioButton4.Text = "38400";
            this.radioButton4.UseVisualStyleBackColor = true;
            this.radioButton4.Click += new System.EventHandler(this.baudrateRadio_clicked);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioButton3.Location = new System.Drawing.Point(159, 170);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(72, 23);
            this.radioButton3.TabIndex = 4;
            this.radioButton3.Text = "19200";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.Click += new System.EventHandler(this.baudrateRadio_clicked);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioButton2.Location = new System.Drawing.Point(159, 142);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(63, 23);
            this.radioButton2.TabIndex = 4;
            this.radioButton2.Text = "9600";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.Click += new System.EventHandler(this.baudrateRadio_clicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Leelawadee UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(138, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(228, 25);
            this.label4.TabIndex = 3;
            this.label4.Text = "Select Output Baud Rate";
            // 
            // queryRtkModePanel
            // 
            this.queryRtkModePanel.Controls.Add(this.rtkBasePanel);
            this.queryRtkModePanel.Controls.Add(this.queryRtkModeOkBtn);
            this.queryRtkModePanel.Controls.Add(this.rtkModeLbl);
            this.queryRtkModePanel.Controls.Add(this.label30);
            this.queryRtkModePanel.Controls.Add(this.label31);
            this.queryRtkModePanel.Controls.Add(this.label32);
            this.queryRtkModePanel.Location = new System.Drawing.Point(0, 0);
            this.queryRtkModePanel.Name = "queryRtkModePanel";
            this.queryRtkModePanel.Size = new System.Drawing.Size(504, 360);
            this.queryRtkModePanel.TabIndex = 18;
            // 
            // rtkBasePanel
            // 
            this.rtkBasePanel.Controls.Add(this.label25);
            this.rtkBasePanel.Controls.Add(this.runtimeRtkOpLbl);
            this.rtkBasePanel.Controls.Add(this.label29);
            this.rtkBasePanel.Controls.Add(this.savedRtkAltLbl);
            this.rtkBasePanel.Controls.Add(this.label28);
            this.rtkBasePanel.Controls.Add(this.savedRtkLonLbl);
            this.rtkBasePanel.Controls.Add(this.label27);
            this.rtkBasePanel.Controls.Add(this.savedRtkLatLbl);
            this.rtkBasePanel.Controls.Add(this.runtimeRtkOpTLbl);
            this.rtkBasePanel.Controls.Add(this.savedRtkOpLbl);
            this.rtkBasePanel.Location = new System.Drawing.Point(0, 106);
            this.rtkBasePanel.Name = "rtkBasePanel";
            this.rtkBasePanel.Size = new System.Drawing.Size(504, 136);
            this.rtkBasePanel.TabIndex = 18;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label25.Location = new System.Drawing.Point(43, 3);
            this.label25.Name = "label25";
            this.label25.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label25.Size = new System.Drawing.Size(203, 20);
            this.label25.TabIndex = 12;
            this.label25.Text = "Saved Operional Function";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // runtimeRtkOpLbl
            // 
            this.runtimeRtkOpLbl.AutoSize = true;
            this.runtimeRtkOpLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.runtimeRtkOpLbl.Location = new System.Drawing.Point(257, 99);
            this.runtimeRtkOpLbl.Name = "runtimeRtkOpLbl";
            this.runtimeRtkOpLbl.Size = new System.Drawing.Size(99, 20);
            this.runtimeRtkOpLbl.TabIndex = 6;
            this.runtimeRtkOpLbl.Text = "Static Mode";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label29.Location = new System.Drawing.Point(126, 27);
            this.label29.Name = "label29";
            this.label29.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label29.Size = new System.Drawing.Size(120, 20);
            this.label29.TabIndex = 16;
            this.label29.Text = "Saved Latitude";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // savedRtkAltLbl
            // 
            this.savedRtkAltLbl.AutoSize = true;
            this.savedRtkAltLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.savedRtkAltLbl.Location = new System.Drawing.Point(257, 75);
            this.savedRtkAltLbl.Name = "savedRtkAltLbl";
            this.savedRtkAltLbl.Size = new System.Drawing.Size(117, 20);
            this.savedRtkAltLbl.TabIndex = 7;
            this.savedRtkAltLbl.Text = "114.24 (meter)";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label28.Location = new System.Drawing.Point(111, 51);
            this.label28.Name = "label28";
            this.label28.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label28.Size = new System.Drawing.Size(135, 20);
            this.label28.TabIndex = 15;
            this.label28.Text = "Saved Longitude";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // savedRtkLonLbl
            // 
            this.savedRtkLonLbl.AutoSize = true;
            this.savedRtkLonLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.savedRtkLonLbl.Location = new System.Drawing.Point(257, 51);
            this.savedRtkLonLbl.Name = "savedRtkLonLbl";
            this.savedRtkLonLbl.Size = new System.Drawing.Size(170, 20);
            this.savedRtkLonLbl.TabIndex = 8;
            this.savedRtkLonLbl.Text = "121.0087237 (degree)";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label27.Location = new System.Drawing.Point(58, 75);
            this.label27.Name = "label27";
            this.label27.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label27.Size = new System.Drawing.Size(188, 20);
            this.label27.TabIndex = 14;
            this.label27.Text = "Saved Ellipsoidal Height";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // savedRtkLatLbl
            // 
            this.savedRtkLatLbl.AutoSize = true;
            this.savedRtkLatLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.savedRtkLatLbl.Location = new System.Drawing.Point(257, 27);
            this.savedRtkLatLbl.Name = "savedRtkLatLbl";
            this.savedRtkLatLbl.Size = new System.Drawing.Size(161, 20);
            this.savedRtkLatLbl.TabIndex = 9;
            this.savedRtkLatLbl.Text = "24.7848257 (degree)";
            // 
            // runtimeRtkOpTLbl
            // 
            this.runtimeRtkOpTLbl.AutoSize = true;
            this.runtimeRtkOpTLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.runtimeRtkOpTLbl.Location = new System.Drawing.Point(24, 99);
            this.runtimeRtkOpTLbl.Name = "runtimeRtkOpTLbl";
            this.runtimeRtkOpTLbl.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.runtimeRtkOpTLbl.Size = new System.Drawing.Size(222, 20);
            this.runtimeRtkOpTLbl.TabIndex = 13;
            this.runtimeRtkOpTLbl.Text = "Runtime Operional Function";
            this.runtimeRtkOpTLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // savedRtkOpLbl
            // 
            this.savedRtkOpLbl.AutoSize = true;
            this.savedRtkOpLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.savedRtkOpLbl.Location = new System.Drawing.Point(257, 3);
            this.savedRtkOpLbl.Name = "savedRtkOpLbl";
            this.savedRtkOpLbl.Size = new System.Drawing.Size(99, 20);
            this.savedRtkOpLbl.TabIndex = 10;
            this.savedRtkOpLbl.Text = "Static Mode";
            // 
            // queryRtkModeOkBtn
            // 
            this.queryRtkModeOkBtn.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.queryRtkModeOkBtn.Location = new System.Drawing.Point(370, 317);
            this.queryRtkModeOkBtn.Name = "queryRtkModeOkBtn";
            this.queryRtkModeOkBtn.Size = new System.Drawing.Size(120, 32);
            this.queryRtkModeOkBtn.TabIndex = 1;
            this.queryRtkModeOkBtn.Text = "OK";
            this.queryRtkModeOkBtn.UseVisualStyleBackColor = true;
            this.queryRtkModeOkBtn.Click += new System.EventHandler(this.queryRtkModeOkBtn_Click);
            // 
            // rtkModeLbl
            // 
            this.rtkModeLbl.AutoSize = true;
            this.rtkModeLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rtkModeLbl.Location = new System.Drawing.Point(257, 83);
            this.rtkModeLbl.Name = "rtkModeLbl";
            this.rtkModeLbl.Size = new System.Drawing.Size(125, 20);
            this.rtkModeLbl.TabIndex = 11;
            this.rtkModeLbl.Text = "RTK Base Mode";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label30.Location = new System.Drawing.Point(160, 83);
            this.label30.Name = "label30";
            this.label30.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label30.Size = new System.Drawing.Size(86, 20);
            this.label30.TabIndex = 17;
            this.label30.Text = "RTK Mode";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label31
            // 
            this.label31.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label31.Location = new System.Drawing.Point(13, 64);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(29, 20);
            this.label31.TabIndex = 5;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Leelawadee UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.ForeColor = System.Drawing.Color.Blue;
            this.label32.Location = new System.Drawing.Point(171, 25);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(163, 25);
            this.label32.TabIndex = 3;
            this.label32.Text = "Query RTK Mode";
            // 
            // configurePositionUpdateRatePanel
            // 
            this.configurePositionUpdateRatePanel.Controls.Add(this.radioButton9);
            this.configurePositionUpdateRatePanel.Controls.Add(this.radioButton10);
            this.configurePositionUpdateRatePanel.Controls.Add(this.configurePositionUpdateRateCancelBtn);
            this.configurePositionUpdateRatePanel.Controls.Add(this.configurePositionUpdateRateAcceptBtn);
            this.configurePositionUpdateRatePanel.Controls.Add(this.radioButton11);
            this.configurePositionUpdateRatePanel.Controls.Add(this.radioButton12);
            this.configurePositionUpdateRatePanel.Controls.Add(this.radioButton13);
            this.configurePositionUpdateRatePanel.Controls.Add(this.radioButton14);
            this.configurePositionUpdateRatePanel.Controls.Add(this.label36);
            this.configurePositionUpdateRatePanel.Location = new System.Drawing.Point(581, 100);
            this.configurePositionUpdateRatePanel.Name = "configurePositionUpdateRatePanel";
            this.configurePositionUpdateRatePanel.Size = new System.Drawing.Size(504, 360);
            this.configurePositionUpdateRatePanel.TabIndex = 6;
            // 
            // radioButton9
            // 
            this.radioButton9.AutoSize = true;
            this.radioButton9.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioButton9.Location = new System.Drawing.Point(218, 227);
            this.radioButton9.Name = "radioButton9";
            this.radioButton9.Size = new System.Drawing.Size(69, 24);
            this.radioButton9.TabIndex = 4;
            this.radioButton9.Text = "10 Hz";
            this.radioButton9.UseVisualStyleBackColor = true;
            this.radioButton9.CheckedChanged += new System.EventHandler(this.updateRateRadio_clicked);
            // 
            // radioButton10
            // 
            this.radioButton10.AutoSize = true;
            this.radioButton10.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioButton10.Location = new System.Drawing.Point(218, 198);
            this.radioButton10.Name = "radioButton10";
            this.radioButton10.Size = new System.Drawing.Size(60, 24);
            this.radioButton10.TabIndex = 4;
            this.radioButton10.Text = "8 Hz";
            this.radioButton10.UseVisualStyleBackColor = true;
            this.radioButton10.CheckedChanged += new System.EventHandler(this.updateRateRadio_clicked);
            // 
            // configurePositionUpdateRateCancelBtn
            // 
            this.configurePositionUpdateRateCancelBtn.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.configurePositionUpdateRateCancelBtn.Location = new System.Drawing.Point(372, 317);
            this.configurePositionUpdateRateCancelBtn.Name = "configurePositionUpdateRateCancelBtn";
            this.configurePositionUpdateRateCancelBtn.Size = new System.Drawing.Size(120, 32);
            this.configurePositionUpdateRateCancelBtn.TabIndex = 2;
            this.configurePositionUpdateRateCancelBtn.Text = "Cancel";
            this.configurePositionUpdateRateCancelBtn.UseVisualStyleBackColor = true;
            this.configurePositionUpdateRateCancelBtn.Click += new System.EventHandler(this.configurePositionUpdateRateCancelBtn_Click);
            // 
            // configurePositionUpdateRateAcceptBtn
            // 
            this.configurePositionUpdateRateAcceptBtn.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.configurePositionUpdateRateAcceptBtn.Location = new System.Drawing.Point(246, 317);
            this.configurePositionUpdateRateAcceptBtn.Name = "configurePositionUpdateRateAcceptBtn";
            this.configurePositionUpdateRateAcceptBtn.Size = new System.Drawing.Size(120, 32);
            this.configurePositionUpdateRateAcceptBtn.TabIndex = 1;
            this.configurePositionUpdateRateAcceptBtn.Text = "Accept";
            this.configurePositionUpdateRateAcceptBtn.UseVisualStyleBackColor = true;
            this.configurePositionUpdateRateAcceptBtn.Click += new System.EventHandler(this.configurePositionUpdateRateAcceptBtn_Click);
            // 
            // radioButton11
            // 
            this.radioButton11.AutoSize = true;
            this.radioButton11.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioButton11.Location = new System.Drawing.Point(218, 169);
            this.radioButton11.Name = "radioButton11";
            this.radioButton11.Size = new System.Drawing.Size(60, 24);
            this.radioButton11.TabIndex = 4;
            this.radioButton11.Text = "5 Hz";
            this.radioButton11.UseVisualStyleBackColor = true;
            this.radioButton11.CheckedChanged += new System.EventHandler(this.updateRateRadio_clicked);
            // 
            // radioButton12
            // 
            this.radioButton12.AutoSize = true;
            this.radioButton12.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioButton12.Location = new System.Drawing.Point(218, 140);
            this.radioButton12.Name = "radioButton12";
            this.radioButton12.Size = new System.Drawing.Size(60, 24);
            this.radioButton12.TabIndex = 4;
            this.radioButton12.Text = "4 Hz";
            this.radioButton12.UseVisualStyleBackColor = true;
            this.radioButton12.CheckedChanged += new System.EventHandler(this.updateRateRadio_clicked);
            // 
            // radioButton13
            // 
            this.radioButton13.AutoSize = true;
            this.radioButton13.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioButton13.Location = new System.Drawing.Point(218, 111);
            this.radioButton13.Name = "radioButton13";
            this.radioButton13.Size = new System.Drawing.Size(60, 24);
            this.radioButton13.TabIndex = 4;
            this.radioButton13.Text = "2 Hz";
            this.radioButton13.UseVisualStyleBackColor = true;
            this.radioButton13.CheckedChanged += new System.EventHandler(this.updateRateRadio_clicked);
            // 
            // radioButton14
            // 
            this.radioButton14.AutoSize = true;
            this.radioButton14.Checked = true;
            this.radioButton14.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioButton14.Location = new System.Drawing.Point(218, 82);
            this.radioButton14.Name = "radioButton14";
            this.radioButton14.Size = new System.Drawing.Size(60, 24);
            this.radioButton14.TabIndex = 4;
            this.radioButton14.TabStop = true;
            this.radioButton14.Text = "1 Hz";
            this.radioButton14.UseVisualStyleBackColor = true;
            this.radioButton14.CheckedChanged += new System.EventHandler(this.updateRateRadio_clicked);
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Font = new System.Drawing.Font("Leelawadee UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label36.ForeColor = System.Drawing.Color.Blue;
            this.label36.Location = new System.Drawing.Point(105, 25);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(294, 25);
            this.label36.TabIndex = 3;
            this.label36.Text = "Configure Position Update Rate";
            // 
            // CommonCammondForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1132, 681);
            this.Controls.Add(this.setFactoryDefaultPanel);
            this.Controls.Add(this.configurePositionUpdateRatePanel);
            this.Controls.Add(this.configureSerialPortPanel);
            this.Controls.Add(this.queryRtkModePanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CommonCammondForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CommonCammondForm";
            this.Load += new System.EventHandler(this.CommonCammondForm_Load);
            this.setFactoryDefaultPanel.ResumeLayout(false);
            this.setFactoryDefaultPanel.PerformLayout();
            this.configureSerialPortPanel.ResumeLayout(false);
            this.configureSerialPortPanel.PerformLayout();
            this.queryRtkModePanel.ResumeLayout(false);
            this.queryRtkModePanel.PerformLayout();
            this.rtkBasePanel.ResumeLayout(false);
            this.rtkBasePanel.PerformLayout();
            this.configurePositionUpdateRatePanel.ResumeLayout(false);
            this.configurePositionUpdateRatePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel setFactoryDefaultPanel;
        private System.Windows.Forms.Button configureSerialPortAcceptBtn;
        private System.Windows.Forms.Button configureSerialPortCancelBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel configureSerialPortPanel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radioButton7;
        private System.Windows.Forms.RadioButton radioButton6;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel queryRtkModePanel;
        private System.Windows.Forms.Label runtimeRtkOpLbl;
        private System.Windows.Forms.Label savedRtkAltLbl;
        private System.Windows.Forms.Label savedRtkLonLbl;
        private System.Windows.Forms.Label savedRtkLatLbl;
        private System.Windows.Forms.Label savedRtkOpLbl;
        private System.Windows.Forms.Label rtkModeLbl;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label runtimeRtkOpTLbl;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Button queryRtkModeOkBtn;
        private System.Windows.Forms.Panel configurePositionUpdateRatePanel;
        private System.Windows.Forms.RadioButton radioButton9;
        private System.Windows.Forms.RadioButton radioButton10;
        private System.Windows.Forms.Button configurePositionUpdateRateCancelBtn;
        private System.Windows.Forms.Button configurePositionUpdateRateAcceptBtn;
        private System.Windows.Forms.RadioButton radioButton11;
        private System.Windows.Forms.RadioButton radioButton12;
        private System.Windows.Forms.RadioButton radioButton13;
        private System.Windows.Forms.RadioButton radioButton14;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Button setFactoryDefaultAccaptBtn;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Panel rtkBasePanel;
        private System.Windows.Forms.Button setFactoryDefaultCancelBtn;
    }
}