namespace RtkViewer
{
    partial class EnableRTKFunctionForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.serialNoLbl = new System.Windows.Forms.Label();
            this.copySerNoBtn = new System.Windows.Forms.Button();
            this.licKeyTxt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.activateBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Leelawadee UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Serial Number";
            // 
            // serialNoLbl
            // 
            this.serialNoLbl.AutoSize = true;
            this.serialNoLbl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.serialNoLbl.Location = new System.Drawing.Point(13, 50);
            this.serialNoLbl.Name = "serialNoLbl";
            this.serialNoLbl.Size = new System.Drawing.Size(220, 20);
            this.serialNoLbl.TabIndex = 3;
            this.serialNoLbl.Text = "W8810X4679A3412758863D";
            // 
            // copySerNoBtn
            // 
            this.copySerNoBtn.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.copySerNoBtn.Location = new System.Drawing.Point(17, 73);
            this.copySerNoBtn.Name = "copySerNoBtn";
            this.copySerNoBtn.Size = new System.Drawing.Size(147, 30);
            this.copySerNoBtn.TabIndex = 2;
            this.copySerNoBtn.Text = "Copy Serial Number";
            this.copySerNoBtn.UseVisualStyleBackColor = true;
            this.copySerNoBtn.Click += new System.EventHandler(this.copySerNoBtn_Click);
            // 
            // licKeyTxt
            // 
            this.licKeyTxt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.licKeyTxt.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.licKeyTxt.ForeColor = System.Drawing.Color.Red;
            this.licKeyTxt.Location = new System.Drawing.Point(12, 202);
            this.licKeyTxt.MaxLength = 48;
            this.licKeyTxt.Name = "licKeyTxt";
            this.licKeyTxt.Size = new System.Drawing.Size(480, 27);
            this.licKeyTxt.TabIndex = 1;
            this.licKeyTxt.TextChanged += new System.EventHandler(this.licKeyTxt_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Leelawadee UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(12, 174);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 25);
            this.label3.TabIndex = 2;
            this.label3.Text = "License Key";
            // 
            // activateBtn
            // 
            this.activateBtn.Enabled = false;
            this.activateBtn.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.activateBtn.Location = new System.Drawing.Point(12, 235);
            this.activateBtn.Name = "activateBtn";
            this.activateBtn.Size = new System.Drawing.Size(152, 30);
            this.activateBtn.TabIndex = 3;
            this.activateBtn.Text = "Activate";
            this.activateBtn.UseVisualStyleBackColor = true;
            this.activateBtn.Click += new System.EventHandler(this.activateBtn_Click);
            // 
            // EnableRTKFunctionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 361);
            this.Controls.Add(this.licKeyTxt);
            this.Controls.Add(this.activateBtn);
            this.Controls.Add(this.copySerNoBtn);
            this.Controls.Add(this.serialNoLbl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnableRTKFunctionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure RTK License";
            this.Load += new System.EventHandler(this.EnableRTKFunctionForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label serialNoLbl;
        private System.Windows.Forms.Button copySerNoBtn;
        private System.Windows.Forms.TextBox licKeyTxt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button activateBtn;
    }
}