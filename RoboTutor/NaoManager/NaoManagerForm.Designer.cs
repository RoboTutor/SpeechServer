namespace NaoManager
{
    partial class NaoManagerForm
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
            this.naoRobotManager1 = new NaoManager.NaoRobotManager();
            this.SuspendLayout();
            // 
            // naoRobotManager1
            // 
            this.naoRobotManager1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.naoRobotManager1.Location = new System.Drawing.Point(11, 3);
            this.naoRobotManager1.Name = "naoRobotManager1";
            this.naoRobotManager1.Size = new System.Drawing.Size(765, 631);
            this.naoRobotManager1.TabIndex = 0;
            // 
            // NaoManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 646);
            this.Controls.Add(this.naoRobotManager1);
            this.Name = "NaoManagerForm";
            this.Text = "NaoManagerForm";
            this.ResumeLayout(false);

        }

        #endregion

        private NaoRobotManager naoRobotManager1;
    }
}