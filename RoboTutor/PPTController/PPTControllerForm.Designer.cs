namespace PPTController
{
    partial class PPTControllerForm
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
            this.pptController1 = new PPTController.SlidesController();
            this.SuspendLayout();
            // 
            // pptController1
            // 
            this.pptController1.Location = new System.Drawing.Point(13, 13);
            this.pptController1.Margin = new System.Windows.Forms.Padding(4);
            this.pptController1.Name = "pptController1";
            this.pptController1.Size = new System.Drawing.Size(361, 222);
            this.pptController1.TabIndex = 0;
            // 
            // PPTControllerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 257);
            this.Controls.Add(this.pptController1);
            this.Name = "PPTControllerForm";
            this.Text = "PPTControllerForm";
            this.ResumeLayout(false);

        }

        #endregion

        private SlidesController pptController1;
    }
}