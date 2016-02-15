namespace DashBoard
{
    partial class ComponentManager
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
            this.tbcMainTab = new System.Windows.Forms.TabControl();
            this.tbpMain = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSecondPart = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.rdbtnNegativeMoodCondition = new System.Windows.Forms.RadioButton();
            this.rdbtnPositiveMoodCondition = new System.Windows.Forms.RadioButton();
            this.tbpCameraControl = new System.Windows.Forms.TabPage();
            this.tbpRobotMgr = new System.Windows.Forms.TabPage();
            this.tbpPPT = new System.Windows.Forms.TabPage();
            this.tbpScriptEngine = new System.Windows.Forms.TabPage();
            this.tbpExtSE = new System.Windows.Forms.TabPage();
            this.tbpSpeech = new System.Windows.Forms.TabPage();
            this.tbcMainTab.SuspendLayout();
            this.tbpMain.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbcMainTab
            // 
            this.tbcMainTab.Controls.Add(this.tbpMain);
            this.tbcMainTab.Controls.Add(this.tbpCameraControl);
            this.tbcMainTab.Controls.Add(this.tbpRobotMgr);
            this.tbcMainTab.Controls.Add(this.tbpPPT);
            this.tbcMainTab.Controls.Add(this.tbpScriptEngine);
            this.tbcMainTab.Controls.Add(this.tbpExtSE);
            this.tbcMainTab.Controls.Add(this.tbpSpeech);
            this.tbcMainTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcMainTab.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbcMainTab.Location = new System.Drawing.Point(0, 0);
            this.tbcMainTab.Name = "tbcMainTab";
            this.tbcMainTab.SelectedIndex = 0;
            this.tbcMainTab.Size = new System.Drawing.Size(908, 482);
            this.tbcMainTab.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tbcMainTab.TabIndex = 0;
            // 
            // tbpMain
            // 
            this.tbpMain.BackColor = System.Drawing.SystemColors.Control;
            this.tbpMain.Controls.Add(this.groupBox2);
            this.tbpMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbpMain.Location = new System.Drawing.Point(4, 24);
            this.tbpMain.Name = "tbpMain";
            this.tbpMain.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tbpMain.Size = new System.Drawing.Size(900, 454);
            this.tbpMain.TabIndex = 0;
            this.tbpMain.Text = "Main";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSecondPart);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.rdbtnNegativeMoodCondition);
            this.groupBox2.Controls.Add(this.rdbtnPositiveMoodCondition);
            this.groupBox2.Location = new System.Drawing.Point(8, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(156, 100);
            this.groupBox2.TabIndex = 41;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Experiment";
            // 
            // btnSecondPart
            // 
            this.btnSecondPart.Location = new System.Drawing.Point(88, 37);
            this.btnSecondPart.Name = "btnSecondPart";
            this.btnSecondPart.Size = new System.Drawing.Size(62, 44);
            this.btnSecondPart.TabIndex = 44;
            this.btnSecondPart.Text = "2nd Part";
            this.btnSecondPart.UseVisualStyleBackColor = true;
            this.btnSecondPart.Click += new System.EventHandler(this.btnSecondPart_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 15);
            this.label5.TabIndex = 43;
            this.label5.Text = "Mood Condition";
            // 
            // rdbtnNegativeMoodCondition
            // 
            this.rdbtnNegativeMoodCondition.AutoSize = true;
            this.rdbtnNegativeMoodCondition.Location = new System.Drawing.Point(9, 62);
            this.rdbtnNegativeMoodCondition.Name = "rdbtnNegativeMoodCondition";
            this.rdbtnNegativeMoodCondition.Size = new System.Drawing.Size(73, 19);
            this.rdbtnNegativeMoodCondition.TabIndex = 42;
            this.rdbtnNegativeMoodCondition.Text = "Negative";
            this.rdbtnNegativeMoodCondition.UseVisualStyleBackColor = true;
            this.rdbtnNegativeMoodCondition.Click += new System.EventHandler(this.rdbtnNegativeMoodCondition_CheckedChanged);
            // 
            // rdbtnPositiveMoodCondition
            // 
            this.rdbtnPositiveMoodCondition.AutoSize = true;
            this.rdbtnPositiveMoodCondition.Location = new System.Drawing.Point(9, 37);
            this.rdbtnPositiveMoodCondition.Name = "rdbtnPositiveMoodCondition";
            this.rdbtnPositiveMoodCondition.Size = new System.Drawing.Size(67, 19);
            this.rdbtnPositiveMoodCondition.TabIndex = 41;
            this.rdbtnPositiveMoodCondition.Text = "Positive";
            this.rdbtnPositiveMoodCondition.UseVisualStyleBackColor = true;
            this.rdbtnPositiveMoodCondition.Click += new System.EventHandler(this.rdbtnPositiveMoodCondition_CheckedChanged);
            // 
            // tbpCameraControl
            // 
            this.tbpCameraControl.BackColor = System.Drawing.SystemColors.Control;
            this.tbpCameraControl.Location = new System.Drawing.Point(4, 24);
            this.tbpCameraControl.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbpCameraControl.Name = "tbpCameraControl";
            this.tbpCameraControl.Size = new System.Drawing.Size(900, 454);
            this.tbpCameraControl.TabIndex = 5;
            this.tbpCameraControl.Text = "CameraControl";
            // 
            // tbpRobotMgr
            // 
            this.tbpRobotMgr.BackColor = System.Drawing.SystemColors.Control;
            this.tbpRobotMgr.Location = new System.Drawing.Point(4, 24);
            this.tbpRobotMgr.Name = "tbpRobotMgr";
            this.tbpRobotMgr.Size = new System.Drawing.Size(900, 454);
            this.tbpRobotMgr.TabIndex = 4;
            this.tbpRobotMgr.Text = "Robot";
            // 
            // tbpPPT
            // 
            this.tbpPPT.BackColor = System.Drawing.SystemColors.Control;
            this.tbpPPT.Location = new System.Drawing.Point(4, 24);
            this.tbpPPT.Name = "tbpPPT";
            this.tbpPPT.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tbpPPT.Size = new System.Drawing.Size(900, 454);
            this.tbpPPT.TabIndex = 3;
            this.tbpPPT.Text = "PPT";
            // 
            // tbpScriptEngine
            // 
            this.tbpScriptEngine.BackColor = System.Drawing.SystemColors.Control;
            this.tbpScriptEngine.Location = new System.Drawing.Point(4, 24);
            this.tbpScriptEngine.Name = "tbpScriptEngine";
            this.tbpScriptEngine.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tbpScriptEngine.Size = new System.Drawing.Size(1076, 634);
            this.tbpScriptEngine.TabIndex = 1;
            this.tbpScriptEngine.Text = "ScriptEngine";
            // 
            // tbpExtSE
            // 
            this.tbpExtSE.BackColor = System.Drawing.SystemColors.Control;
            this.tbpExtSE.Location = new System.Drawing.Point(4, 24);
            this.tbpExtSE.Name = "tbpExtSE";
            this.tbpExtSE.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tbpExtSE.Size = new System.Drawing.Size(900, 454);
            this.tbpExtSE.TabIndex = 2;
            this.tbpExtSE.Text = "ExternalScriptEngine";
            // 
            // tbpSpeech
            // 
            this.tbpSpeech.BackColor = System.Drawing.SystemColors.Control; 
            this.tbpSpeech.Location = new System.Drawing.Point(4, 24);
            this.tbpSpeech.Name = "tbpSpeech";
            this.tbpSpeech.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tbpSpeech.Size = new System.Drawing.Size(900, 454);
            this.tbpSpeech.TabIndex = 6;
            this.tbpSpeech.Text = "Speech";
            // 
            // ComponentManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(908, 482);
            this.Controls.Add(this.tbcMainTab);
            this.Name = "ComponentManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Component Manager";
            this.tbcMainTab.ResumeLayout(false);
            this.tbpMain.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbcMainTab;
        private System.Windows.Forms.TabPage tbpMain;
        private System.Windows.Forms.TabPage tbpScriptEngine;
        private System.Windows.Forms.TabPage tbpExtSE;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSecondPart;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rdbtnNegativeMoodCondition;
        private System.Windows.Forms.RadioButton rdbtnPositiveMoodCondition;
        private System.Windows.Forms.TabPage tbpPPT;
        private System.Windows.Forms.TabPage tbpRobotMgr;
        private System.Windows.Forms.TabPage tbpCameraControl;
        private System.Windows.Forms.TabPage tbpSpeech;
    }
}

