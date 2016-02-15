namespace ConnectorNao
{
    partial class ucNaoConnector
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
            this.gpbNaoConnector = new System.Windows.Forms.GroupBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnIdlePause = new System.Windows.Forms.Button();
            this.btnIdleMove = new System.Windows.Forms.Button();
            this.btnWalk = new System.Windows.Forms.Button();
            this.btnSquat = new System.Windows.Forms.Button();
            this.btnInitPose = new System.Windows.Forms.Button();
            this.btnMotors = new System.Windows.Forms.Button();
            this.btnStandUp = new System.Windows.Forms.Button();
            this.cbbNaoIP = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnConnectNao = new System.Windows.Forms.Button();
            this.gpbNaoConnector.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpbNaoConnector
            // 
            this.gpbNaoConnector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gpbNaoConnector.Controls.Add(this.btnTest);
            this.gpbNaoConnector.Controls.Add(this.btnIdlePause);
            this.gpbNaoConnector.Controls.Add(this.btnIdleMove);
            this.gpbNaoConnector.Controls.Add(this.btnWalk);
            this.gpbNaoConnector.Controls.Add(this.btnSquat);
            this.gpbNaoConnector.Controls.Add(this.btnInitPose);
            this.gpbNaoConnector.Controls.Add(this.btnMotors);
            this.gpbNaoConnector.Controls.Add(this.btnStandUp);
            this.gpbNaoConnector.Controls.Add(this.cbbNaoIP);
            this.gpbNaoConnector.Controls.Add(this.label1);
            this.gpbNaoConnector.Controls.Add(this.btnConnectNao);
            this.gpbNaoConnector.Location = new System.Drawing.Point(4, 5);
            this.gpbNaoConnector.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gpbNaoConnector.Name = "gpbNaoConnector";
            this.gpbNaoConnector.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gpbNaoConnector.Size = new System.Drawing.Size(401, 164);
            this.gpbNaoConnector.TabIndex = 3;
            this.gpbNaoConnector.TabStop = false;
            this.gpbNaoConnector.Text = "NaoConnector";
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(297, 111);
            this.btnTest.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(85, 28);
            this.btnTest.TabIndex = 11;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnIdlePause
            // 
            this.btnIdlePause.Enabled = false;
            this.btnIdlePause.Location = new System.Drawing.Point(189, 119);
            this.btnIdlePause.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnIdlePause.Name = "btnIdlePause";
            this.btnIdlePause.Size = new System.Drawing.Size(97, 28);
            this.btnIdlePause.TabIndex = 10;
            this.btnIdlePause.Text = "IdleMovePause";
            this.btnIdlePause.UseVisualStyleBackColor = true;
            this.btnIdlePause.Click += new System.EventHandler(this.btnIdlePauseResume_Click);
            // 
            // btnIdleMove
            // 
            this.btnIdleMove.Enabled = false;
            this.btnIdleMove.Location = new System.Drawing.Point(189, 82);
            this.btnIdleMove.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnIdleMove.Name = "btnIdleMove";
            this.btnIdleMove.Size = new System.Drawing.Size(97, 30);
            this.btnIdleMove.TabIndex = 9;
            this.btnIdleMove.Text = "IdleMoveOn";
            this.btnIdleMove.UseVisualStyleBackColor = true;
            this.btnIdleMove.Click += new System.EventHandler(this.btnIdleMoveOnOff_Click);
            // 
            // btnWalk
            // 
            this.btnWalk.Enabled = false;
            this.btnWalk.Location = new System.Drawing.Point(111, 119);
            this.btnWalk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnWalk.Name = "btnWalk";
            this.btnWalk.Size = new System.Drawing.Size(71, 28);
            this.btnWalk.TabIndex = 8;
            this.btnWalk.Text = "Walk";
            this.btnWalk.UseVisualStyleBackColor = true;
            this.btnWalk.Click += new System.EventHandler(this.btnWalk_Click);
            // 
            // btnSquat
            // 
            this.btnSquat.Enabled = false;
            this.btnSquat.Location = new System.Drawing.Point(111, 82);
            this.btnSquat.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSquat.Name = "btnSquat";
            this.btnSquat.Size = new System.Drawing.Size(71, 28);
            this.btnSquat.TabIndex = 7;
            this.btnSquat.Text = "Squat";
            this.btnSquat.UseVisualStyleBackColor = true;
            this.btnSquat.Click += new System.EventHandler(this.btnSquat_Click);
            // 
            // btnInitPose
            // 
            this.btnInitPose.Enabled = false;
            this.btnInitPose.Location = new System.Drawing.Point(15, 119);
            this.btnInitPose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnInitPose.Name = "btnInitPose";
            this.btnInitPose.Size = new System.Drawing.Size(88, 28);
            this.btnInitPose.TabIndex = 6;
            this.btnInitPose.Text = "Initial Pose";
            this.btnInitPose.UseVisualStyleBackColor = true;
            this.btnInitPose.Click += new System.EventHandler(this.btnInitPose_Click);
            // 
            // btnMotors
            // 
            this.btnMotors.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnMotors.Enabled = false;
            this.btnMotors.Location = new System.Drawing.Point(297, 60);
            this.btnMotors.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnMotors.Name = "btnMotors";
            this.btnMotors.Size = new System.Drawing.Size(85, 42);
            this.btnMotors.TabIndex = 5;
            this.btnMotors.Text = "MotorsOn";
            this.btnMotors.UseVisualStyleBackColor = false;
            this.btnMotors.Click += new System.EventHandler(this.btnMotors_Click);
            // 
            // btnStandUp
            // 
            this.btnStandUp.Enabled = false;
            this.btnStandUp.Location = new System.Drawing.Point(15, 82);
            this.btnStandUp.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStandUp.Name = "btnStandUp";
            this.btnStandUp.Size = new System.Drawing.Size(88, 28);
            this.btnStandUp.TabIndex = 4;
            this.btnStandUp.Text = "StandUp";
            this.btnStandUp.UseVisualStyleBackColor = true;
            this.btnStandUp.Click += new System.EventHandler(this.btnStandUp_Click);
            // 
            // cbbNaoIP
            // 
            this.cbbNaoIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbbNaoIP.FormattingEnabled = true;
            this.cbbNaoIP.Items.AddRange(new object[] {
            "192.168.0.103",
            "127.0.0.1",
            "192.168.0.109",
            "192.168.0.102",
            "192.168.0.101",
            "192.168.0.100"});
            this.cbbNaoIP.Location = new System.Drawing.Point(15, 44);
            this.cbbNaoIP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbbNaoIP.Name = "cbbNaoIP";
            this.cbbNaoIP.Size = new System.Drawing.Size(247, 28);
            this.cbbNaoIP.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "IP and Port";
            // 
            // btnConnectNao
            // 
            this.btnConnectNao.Location = new System.Drawing.Point(297, 15);
            this.btnConnectNao.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnConnectNao.Name = "btnConnectNao";
            this.btnConnectNao.Size = new System.Drawing.Size(85, 37);
            this.btnConnectNao.TabIndex = 1;
            this.btnConnectNao.Text = "Connect";
            this.btnConnectNao.UseVisualStyleBackColor = true;
            this.btnConnectNao.Click += new System.EventHandler(this.btnConnectNao_Click);
            // 
            // ucNaoConnector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gpbNaoConnector);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ucNaoConnector";
            this.Size = new System.Drawing.Size(427, 172);
            this.gpbNaoConnector.ResumeLayout(false);
            this.gpbNaoConnector.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gpbNaoConnector;
        private System.Windows.Forms.Button btnInitPose;
        private System.Windows.Forms.Button btnMotors;
        private System.Windows.Forms.Button btnStandUp;
        private System.Windows.Forms.ComboBox cbbNaoIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnConnectNao;
        private System.Windows.Forms.Button btnSquat;
        private System.Windows.Forms.Button btnWalk;
        private System.Windows.Forms.Button btnIdleMove;
        private System.Windows.Forms.Button btnIdlePause;
        private System.Windows.Forms.Button btnTest;
    }
}
