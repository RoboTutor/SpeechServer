namespace NaoAffectManager
{
    partial class AffectMonitor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nudValence = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbValence = new System.Windows.Forms.TrackBar();
            this.UITextSentiment = new NaoAffectManager.TextSentiment();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudValence)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbValence)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nudValence);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbValence);
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(494, 143);
            this.groupBox1.TabIndex = 36;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Robot Mood";
            // 
            // nudValence
            // 
            this.nudValence.DecimalPlaces = 1;
            this.nudValence.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nudValence.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudValence.Location = new System.Drawing.Point(372, 98);
            this.nudValence.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            this.nudValence.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudValence.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.nudValence.Name = "nudValence";
            this.nudValence.Size = new System.Drawing.Size(107, 29);
            this.nudValence.TabIndex = 26;
            this.nudValence.ValueChanged += new System.EventHandler(this.nudValence_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(26, 98);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 24);
            this.label4.TabIndex = 28;
            this.label4.Text = "Valence";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(384, 31);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 20);
            this.label2.TabIndex = 30;
            this.label2.Text = "Positive";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(26, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 20);
            this.label1.TabIndex = 29;
            this.label1.Text = "Negative";
            // 
            // tbValence
            // 
            this.tbValence.LargeChange = 10;
            this.tbValence.Location = new System.Drawing.Point(21, 57);
            this.tbValence.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            this.tbValence.Maximum = 1000;
            this.tbValence.Minimum = -1000;
            this.tbValence.Name = "tbValence";
            this.tbValence.Size = new System.Drawing.Size(458, 45);
            this.tbValence.TabIndex = 25;
            this.tbValence.TickFrequency = 100;
            this.tbValence.Scroll += new System.EventHandler(this.tbValence_Scroll);
            // 
            // UITextSentiment
            // 
            this.UITextSentiment.Location = new System.Drawing.Point(4, 154);
            this.UITextSentiment.Name = "UITextSentiment";
            this.UITextSentiment.Size = new System.Drawing.Size(494, 373);
            this.UITextSentiment.TabIndex = 37;
            // 
            // AffectMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.UITextSentiment);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AffectMonitor";
            this.Size = new System.Drawing.Size(507, 530);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudValence)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbValence)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudValence;
        private System.Windows.Forms.TrackBar tbValence;
        private System.Windows.Forms.Label label4;
        private TextSentiment UITextSentiment;

    }
}
