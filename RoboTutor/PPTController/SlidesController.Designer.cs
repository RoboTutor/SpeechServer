namespace PPTController
{
    partial class SlidesController
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
            this.btnOpenPPT = new System.Windows.Forms.Button();
            this.btnNextSlide = new System.Windows.Forms.Button();
            this.btnPreviousSlide = new System.Windows.Forms.Button();
            this.btnClosePPT = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnGoToSlide = new System.Windows.Forms.Button();
            this.txtbSlideNum = new System.Windows.Forms.TextBox();
            this.cbbFileAddress = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOpenPPT
            // 
            this.btnOpenPPT.Location = new System.Drawing.Point(8, 79);
            this.btnOpenPPT.Margin = new System.Windows.Forms.Padding(4);
            this.btnOpenPPT.Name = "btnOpenPPT";
            this.btnOpenPPT.Size = new System.Drawing.Size(100, 28);
            this.btnOpenPPT.TabIndex = 0;
            this.btnOpenPPT.Text = "Open";
            this.btnOpenPPT.UseVisualStyleBackColor = true;
            this.btnOpenPPT.Click += new System.EventHandler(this.btnOpenPPT_Click);
            // 
            // btnNextSlide
            // 
            this.btnNextSlide.Location = new System.Drawing.Point(8, 114);
            this.btnNextSlide.Margin = new System.Windows.Forms.Padding(4);
            this.btnNextSlide.Name = "btnNextSlide";
            this.btnNextSlide.Size = new System.Drawing.Size(100, 28);
            this.btnNextSlide.TabIndex = 1;
            this.btnNextSlide.Text = "Next";
            this.btnNextSlide.UseVisualStyleBackColor = true;
            this.btnNextSlide.Click += new System.EventHandler(this.btnNextSlide_Click);
            // 
            // btnPreviousSlide
            // 
            this.btnPreviousSlide.Location = new System.Drawing.Point(116, 114);
            this.btnPreviousSlide.Margin = new System.Windows.Forms.Padding(4);
            this.btnPreviousSlide.Name = "btnPreviousSlide";
            this.btnPreviousSlide.Size = new System.Drawing.Size(100, 28);
            this.btnPreviousSlide.TabIndex = 2;
            this.btnPreviousSlide.Text = "Previous";
            this.btnPreviousSlide.UseVisualStyleBackColor = true;
            this.btnPreviousSlide.Click += new System.EventHandler(this.btnPreviousSlide_Click);
            // 
            // btnClosePPT
            // 
            this.btnClosePPT.Location = new System.Drawing.Point(116, 79);
            this.btnClosePPT.Margin = new System.Windows.Forms.Padding(4);
            this.btnClosePPT.Name = "btnClosePPT";
            this.btnClosePPT.Size = new System.Drawing.Size(100, 28);
            this.btnClosePPT.TabIndex = 3;
            this.btnClosePPT.Text = "Close";
            this.btnClosePPT.UseVisualStyleBackColor = true;
            this.btnClosePPT.Click += new System.EventHandler(this.btnClosePPT_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnGoToSlide);
            this.groupBox1.Controls.Add(this.txtbSlideNum);
            this.groupBox1.Controls.Add(this.cbbFileAddress);
            this.groupBox1.Controls.Add(this.btnOpenPPT);
            this.groupBox1.Controls.Add(this.btnClosePPT);
            this.groupBox1.Controls.Add(this.btnPreviousSlide);
            this.groupBox1.Controls.Add(this.btnNextSlide);
            this.groupBox1.Location = new System.Drawing.Point(17, 16);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(329, 185);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Slides Control";
            // 
            // btnGoToSlide
            // 
            this.btnGoToSlide.Location = new System.Drawing.Point(116, 150);
            this.btnGoToSlide.Margin = new System.Windows.Forms.Padding(4);
            this.btnGoToSlide.Name = "btnGoToSlide";
            this.btnGoToSlide.Size = new System.Drawing.Size(100, 28);
            this.btnGoToSlide.TabIndex = 6;
            this.btnGoToSlide.Text = "GoTo";
            this.btnGoToSlide.UseVisualStyleBackColor = true;
            this.btnGoToSlide.Click += new System.EventHandler(this.btnGoToSlide_Click);
            // 
            // txtbSlideNum
            // 
            this.txtbSlideNum.Location = new System.Drawing.Point(8, 150);
            this.txtbSlideNum.Margin = new System.Windows.Forms.Padding(4);
            this.txtbSlideNum.Name = "txtbSlideNum";
            this.txtbSlideNum.Size = new System.Drawing.Size(99, 22);
            this.txtbSlideNum.TabIndex = 5;
            // 
            // cbbFileAddress
            // 
            this.cbbFileAddress.FormattingEnabled = true;
            this.cbbFileAddress.Location = new System.Drawing.Point(15, 37);
            this.cbbFileAddress.Margin = new System.Windows.Forms.Padding(4);
            this.cbbFileAddress.Name = "cbbFileAddress";
            this.cbbFileAddress.Size = new System.Drawing.Size(268, 24);
            this.cbbFileAddress.TabIndex = 4;
            // 
            // SlidesController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SlidesController";
            this.Size = new System.Drawing.Size(361, 222);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOpenPPT;
        private System.Windows.Forms.Button btnNextSlide;
        private System.Windows.Forms.Button btnPreviousSlide;
        private System.Windows.Forms.Button btnClosePPT;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbbFileAddress;
        private System.Windows.Forms.Button btnGoToSlide;
        private System.Windows.Forms.TextBox txtbSlideNum;
    }
}

