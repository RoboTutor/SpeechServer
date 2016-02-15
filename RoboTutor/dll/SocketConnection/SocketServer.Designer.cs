namespace SocketConnection
{
    partial class ucSocketServer
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
            this.gpbSocketServer = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtbServerMessage = new System.Windows.Forms.TextBox();
            this.btnServerSend = new System.Windows.Forms.Button();
            this.txtbConnectedClients = new System.Windows.Forms.TextBox();
            this.gpbSocketServer.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpbSocketServer
            // 
            this.gpbSocketServer.Controls.Add(this.label4);
            this.gpbSocketServer.Controls.Add(this.txtbServerMessage);
            this.gpbSocketServer.Controls.Add(this.btnServerSend);
            this.gpbSocketServer.Controls.Add(this.txtbConnectedClients);
            this.gpbSocketServer.Location = new System.Drawing.Point(3, 3);
            this.gpbSocketServer.Name = "gpbSocketServer";
            this.gpbSocketServer.Size = new System.Drawing.Size(348, 171);
            this.gpbSocketServer.TabIndex = 6;
            this.gpbSocketServer.TabStop = false;
            this.gpbSocketServer.Text = "Socket Server";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "Message";
            // 
            // txtbServerMessage
            // 
            this.txtbServerMessage.Location = new System.Drawing.Point(12, 110);
            this.txtbServerMessage.Multiline = true;
            this.txtbServerMessage.Name = "txtbServerMessage";
            this.txtbServerMessage.Size = new System.Drawing.Size(216, 53);
            this.txtbServerMessage.TabIndex = 9;
            this.txtbServerMessage.Text = "Server sent this message to you.";
            // 
            // btnServerSend
            // 
            this.btnServerSend.Location = new System.Drawing.Point(246, 110);
            this.btnServerSend.Name = "btnServerSend";
            this.btnServerSend.Size = new System.Drawing.Size(80, 35);
            this.btnServerSend.TabIndex = 1;
            this.btnServerSend.Text = "Send";
            this.btnServerSend.UseVisualStyleBackColor = true;
            // 
            // txtbConnectedClients
            // 
            this.txtbConnectedClients.Location = new System.Drawing.Point(12, 21);
            this.txtbConnectedClients.Multiline = true;
            this.txtbConnectedClients.Name = "txtbConnectedClients";
            this.txtbConnectedClients.ReadOnly = true;
            this.txtbConnectedClients.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtbConnectedClients.Size = new System.Drawing.Size(230, 65);
            this.txtbConnectedClients.TabIndex = 0;
            // 
            // SocketServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gpbSocketServer);
            this.Name = "SocketServer";
            this.Size = new System.Drawing.Size(357, 200);
            this.gpbSocketServer.ResumeLayout(false);
            this.gpbSocketServer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gpbSocketServer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtbServerMessage;
        private System.Windows.Forms.Button btnServerSend;
        private System.Windows.Forms.TextBox txtbConnectedClients;
    }
}
