namespace ScriptEngine
{
    partial class ExternalScriptEngine
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
            this.ucSocketClient = new SocketConnection.ucSocketClient();
            this.SuspendLayout();
            // 
            // ucSocketClient
            // 
            this.ucSocketClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucSocketClient.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ucSocketClient.Location = new System.Drawing.Point(0, 0);
            this.ucSocketClient.Name = "ucSocketClient";
            this.ucSocketClient.Size = new System.Drawing.Size(360, 322);
            this.ucSocketClient.TabIndex = 0;
            // 
            // ExternalScriptEngine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ucSocketClient);
            this.Name = "ExternalScriptEngine";
            this.Size = new System.Drawing.Size(360, 322);
            this.ResumeLayout(false);

        }

        #endregion

        private SocketConnection.ucSocketClient ucSocketClient;
    }
}
