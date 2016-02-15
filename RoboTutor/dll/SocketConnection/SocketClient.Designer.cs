namespace SocketConnection
{
    partial class ucSocketClient
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
            this.cbbClientIP = new System.Windows.Forms.ComboBox();
            this.gpbSocketClient = new System.Windows.Forms.GroupBox();
            this.txtbOutput = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtbClientMessage = new System.Windows.Forms.TextBox();
            this.btnClientSend = new System.Windows.Forms.Button();
            this.cbbClientPort = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSocketClientConnect = new System.Windows.Forms.Button();
            this.gpbSocketClient.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbbClientIP
            // 
            this.cbbClientIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbbClientIP.FormattingEnabled = true;
            this.cbbClientIP.Items.AddRange(new object[] {
            "192.168.0.103",
            "192.168.0.102",
            "192.168.0.101",
            "192.168.0.109",
            "127.0.0.1"});
            this.cbbClientIP.Location = new System.Drawing.Point(13, 38);
            this.cbbClientIP.Name = "cbbClientIP";
            this.cbbClientIP.Size = new System.Drawing.Size(142, 24);
            this.cbbClientIP.TabIndex = 3;
            // 
            // gpbSocketClient
            // 
            this.gpbSocketClient.Controls.Add(this.txtbOutput);
            this.gpbSocketClient.Controls.Add(this.label3);
            this.gpbSocketClient.Controls.Add(this.txtbClientMessage);
            this.gpbSocketClient.Controls.Add(this.btnClientSend);
            this.gpbSocketClient.Controls.Add(this.cbbClientPort);
            this.gpbSocketClient.Controls.Add(this.label2);
            this.gpbSocketClient.Controls.Add(this.cbbClientIP);
            this.gpbSocketClient.Controls.Add(this.label1);
            this.gpbSocketClient.Controls.Add(this.btnSocketClientConnect);
            this.gpbSocketClient.Location = new System.Drawing.Point(3, 3);
            this.gpbSocketClient.Name = "gpbSocketClient";
            this.gpbSocketClient.Size = new System.Drawing.Size(349, 312);
            this.gpbSocketClient.TabIndex = 4;
            this.gpbSocketClient.TabStop = false;
            this.gpbSocketClient.Text = "Socket Client";
            // 
            // txtbOutput
            // 
            this.txtbOutput.Location = new System.Drawing.Point(13, 142);
            this.txtbOutput.Multiline = true;
            this.txtbOutput.Name = "txtbOutput";
            this.txtbOutput.Size = new System.Drawing.Size(314, 154);
            this.txtbOutput.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Message";
            // 
            // txtbClientMessage
            // 
            this.txtbClientMessage.Location = new System.Drawing.Point(13, 83);
            this.txtbClientMessage.Multiline = true;
            this.txtbClientMessage.Name = "txtbClientMessage";
            this.txtbClientMessage.Size = new System.Drawing.Size(216, 53);
            this.txtbClientMessage.TabIndex = 7;
            this.txtbClientMessage.Text = "This is a message sent by client.";
            // 
            // btnClientSend
            // 
            this.btnClientSend.Location = new System.Drawing.Point(247, 101);
            this.btnClientSend.Name = "btnClientSend";
            this.btnClientSend.Size = new System.Drawing.Size(80, 35);
            this.btnClientSend.TabIndex = 6;
            this.btnClientSend.Text = "Send";
            this.btnClientSend.UseVisualStyleBackColor = true;
            this.btnClientSend.Click += new System.EventHandler(this.btnClientSend_Click);
            // 
            // cbbClientPort
            // 
            this.cbbClientPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbbClientPort.FormattingEnabled = true;
            this.cbbClientPort.Items.AddRange(new object[] {
            "8311",
            "1986",
            "9559",
            "54000"});
            this.cbbClientPort.Location = new System.Drawing.Point(161, 38);
            this.cbbClientPort.Name = "cbbClientPort";
            this.cbbClientPort.Size = new System.Drawing.Size(66, 24);
            this.cbbClientPort.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(158, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "IP";
            // 
            // btnSocketClientConnect
            // 
            this.btnSocketClientConnect.BackColor = System.Drawing.Color.LightGreen;
            this.btnSocketClientConnect.Location = new System.Drawing.Point(247, 30);
            this.btnSocketClientConnect.Name = "btnSocketClientConnect";
            this.btnSocketClientConnect.Size = new System.Drawing.Size(80, 35);
            this.btnSocketClientConnect.TabIndex = 1;
            this.btnSocketClientConnect.Text = "Connect";
            this.btnSocketClientConnect.UseVisualStyleBackColor = false;
            this.btnSocketClientConnect.Click += new System.EventHandler(this.btnClientConnect_Click);
            // 
            // ucSocketClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gpbSocketClient);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "ucSocketClient";
            this.Size = new System.Drawing.Size(360, 329);
            this.gpbSocketClient.ResumeLayout(false);
            this.gpbSocketClient.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbbClientIP;
        private System.Windows.Forms.GroupBox gpbSocketClient;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSocketClientConnect;
        private System.Windows.Forms.ComboBox cbbClientPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnClientSend;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtbClientMessage;
        private System.Windows.Forms.TextBox txtbOutput;
    }
}
