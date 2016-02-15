namespace NaoManager
{
    partial class NaoRobotManager
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
            this.pnlNaoConnector = new System.Windows.Forms.Panel();
            this.cbEndlessMove = new System.Windows.Forms.CheckBox();
            this.btnStartModulatedBehavior = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.cbbModulatedBehaviors = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnPlayAllList = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.btnStopPlayAll = new System.Windows.Forms.Button();
            this.txtbCurBehavior = new System.Windows.Forms.TextBox();
            this.btnListParams = new System.Windows.Forms.Button();
            this.nudPlayAllStep = new System.Windows.Forms.NumericUpDown();
            this.btnPlayAll = new System.Windows.Forms.Button();
            this.gbControl = new System.Windows.Forms.GroupBox();
            this.rbtnSingleBehavior = new System.Windows.Forms.RadioButton();
            this.rbtnRoboTutor = new System.Windows.Forms.RadioButton();
            this.rbtnVideoAllBehaviors = new System.Windows.Forms.RadioButton();
            this.btnStartCrgBehavior = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cbbCrgBehaviors = new System.Windows.Forms.ComboBox();
            this.btnHeadPose = new System.Windows.Forms.Button();
            this.NaoAffect = new NaoAffectManager.AffectMonitor();
            this.btnInsertCrgBehavior = new System.Windows.Forms.Button();
            this.btnRemoveCrgBehavior = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlayAllStep)).BeginInit();
            this.gbControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlNaoConnector
            // 
            this.pnlNaoConnector.Location = new System.Drawing.Point(14, 14);
            this.pnlNaoConnector.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.pnlNaoConnector.Name = "pnlNaoConnector";
            this.pnlNaoConnector.Size = new System.Drawing.Size(519, 210);
            this.pnlNaoConnector.TabIndex = 17;
            // 
            // cbEndlessMove
            // 
            this.cbEndlessMove.AutoSize = true;
            this.cbEndlessMove.Checked = true;
            this.cbEndlessMove.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbEndlessMove.Location = new System.Drawing.Point(206, 109);
            this.cbEndlessMove.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.cbEndlessMove.Name = "cbEndlessMove";
            this.cbEndlessMove.Size = new System.Drawing.Size(100, 19);
            this.cbEndlessMove.TabIndex = 33;
            this.cbEndlessMove.Text = "EndlessMove";
            this.cbEndlessMove.UseVisualStyleBackColor = true;
            // 
            // btnStartModulatedBehavior
            // 
            this.btnStartModulatedBehavior.BackColor = System.Drawing.Color.LightGreen;
            this.btnStartModulatedBehavior.Enabled = false;
            this.btnStartModulatedBehavior.Location = new System.Drawing.Point(366, 18);
            this.btnStartModulatedBehavior.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.btnStartModulatedBehavior.Name = "btnStartModulatedBehavior";
            this.btnStartModulatedBehavior.Size = new System.Drawing.Size(147, 54);
            this.btnStartModulatedBehavior.TabIndex = 27;
            this.btnStartModulatedBehavior.Text = "Start Modulated Behavior";
            this.btnStartModulatedBehavior.UseVisualStyleBackColor = false;
            this.btnStartModulatedBehavior.Click += new System.EventHandler(this.btnStartModulatedBehavior_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(117, 15);
            this.label6.TabIndex = 30;
            this.label6.Text = "Modulated Behavior";
            // 
            // cbbModulatedBehaviors
            // 
            this.cbbModulatedBehaviors.FormattingEnabled = true;
            this.cbbModulatedBehaviors.Location = new System.Drawing.Point(14, 46);
            this.cbbModulatedBehaviors.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.cbbModulatedBehaviors.Name = "cbbModulatedBehaviors";
            this.cbbModulatedBehaviors.Size = new System.Drawing.Size(114, 23);
            this.cbbModulatedBehaviors.TabIndex = 29;
            this.cbbModulatedBehaviors.SelectedIndexChanged += new System.EventHandler(this.cbbBehaviorName_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnPlayAllList);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.btnStopPlayAll);
            this.groupBox3.Controls.Add(this.txtbCurBehavior);
            this.groupBox3.Controls.Add(this.btnListParams);
            this.groupBox3.Controls.Add(this.nudPlayAllStep);
            this.groupBox3.Controls.Add(this.btnPlayAll);
            this.groupBox3.Location = new System.Drawing.Point(14, 387);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(341, 126);
            this.groupBox3.TabIndex = 41;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Play All";
            // 
            // btnPlayAllList
            // 
            this.btnPlayAllList.Location = new System.Drawing.Point(6, 59);
            this.btnPlayAllList.Name = "btnPlayAllList";
            this.btnPlayAllList.Size = new System.Drawing.Size(75, 29);
            this.btnPlayAllList.TabIndex = 36;
            this.btnPlayAllList.Text = "Play List";
            this.btnPlayAllList.UseVisualStyleBackColor = true;
            this.btnPlayAllList.Click += new System.EventHandler(this.btnPlayAllList_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(175, 26);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 15);
            this.label7.TabIndex = 35;
            this.label7.Text = "V Step";
            // 
            // btnStopPlayAll
            // 
            this.btnStopPlayAll.Location = new System.Drawing.Point(85, 90);
            this.btnStopPlayAll.Name = "btnStopPlayAll";
            this.btnStopPlayAll.Size = new System.Drawing.Size(84, 29);
            this.btnStopPlayAll.TabIndex = 34;
            this.btnStopPlayAll.Text = "Stop";
            this.btnStopPlayAll.UseVisualStyleBackColor = true;
            this.btnStopPlayAll.Click += new System.EventHandler(this.btnStopPlayAll_Click);
            // 
            // txtbCurBehavior
            // 
            this.txtbCurBehavior.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtbCurBehavior.Location = new System.Drawing.Point(6, 20);
            this.txtbCurBehavior.Multiline = true;
            this.txtbCurBehavior.Name = "txtbCurBehavior";
            this.txtbCurBehavior.ReadOnly = true;
            this.txtbCurBehavior.Size = new System.Drawing.Size(163, 33);
            this.txtbCurBehavior.TabIndex = 33;
            this.txtbCurBehavior.Text = "Current Behavior";
            // 
            // btnListParams
            // 
            this.btnListParams.Location = new System.Drawing.Point(87, 58);
            this.btnListParams.Name = "btnListParams";
            this.btnListParams.Size = new System.Drawing.Size(82, 30);
            this.btnListParams.TabIndex = 44;
            this.btnListParams.Text = "ListParams";
            this.btnListParams.UseVisualStyleBackColor = true;
            this.btnListParams.Click += new System.EventHandler(this.btnListParams_Click);
            // 
            // nudPlayAllStep
            // 
            this.nudPlayAllStep.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nudPlayAllStep.Location = new System.Drawing.Point(223, 18);
            this.nudPlayAllStep.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudPlayAllStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPlayAllStep.Name = "nudPlayAllStep";
            this.nudPlayAllStep.Size = new System.Drawing.Size(50, 29);
            this.nudPlayAllStep.TabIndex = 32;
            this.nudPlayAllStep.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // btnPlayAll
            // 
            this.btnPlayAll.Location = new System.Drawing.Point(6, 90);
            this.btnPlayAll.Name = "btnPlayAll";
            this.btnPlayAll.Size = new System.Drawing.Size(73, 29);
            this.btnPlayAll.TabIndex = 31;
            this.btnPlayAll.Text = "Play All";
            this.btnPlayAll.UseVisualStyleBackColor = true;
            this.btnPlayAll.Click += new System.EventHandler(this.btnPlayAll_Click);
            // 
            // gbControl
            // 
            this.gbControl.Controls.Add(this.btnRemoveCrgBehavior);
            this.gbControl.Controls.Add(this.btnInsertCrgBehavior);
            this.gbControl.Controls.Add(this.cbEndlessMove);
            this.gbControl.Controls.Add(this.rbtnSingleBehavior);
            this.gbControl.Controls.Add(this.rbtnRoboTutor);
            this.gbControl.Controls.Add(this.rbtnVideoAllBehaviors);
            this.gbControl.Controls.Add(this.btnStartCrgBehavior);
            this.gbControl.Controls.Add(this.label3);
            this.gbControl.Controls.Add(this.cbbCrgBehaviors);
            this.gbControl.Controls.Add(this.btnStartModulatedBehavior);
            this.gbControl.Controls.Add(this.cbbModulatedBehaviors);
            this.gbControl.Controls.Add(this.label6);
            this.gbControl.Location = new System.Drawing.Point(14, 232);
            this.gbControl.Name = "gbControl";
            this.gbControl.Size = new System.Drawing.Size(519, 142);
            this.gbControl.TabIndex = 38;
            this.gbControl.TabStop = false;
            this.gbControl.Text = "Control";
            // 
            // rbtnSingleBehavior
            // 
            this.rbtnSingleBehavior.AutoSize = true;
            this.rbtnSingleBehavior.Location = new System.Drawing.Point(206, 56);
            this.rbtnSingleBehavior.Name = "rbtnSingleBehavior";
            this.rbtnSingleBehavior.Size = new System.Drawing.Size(108, 19);
            this.rbtnSingleBehavior.TabIndex = 45;
            this.rbtnSingleBehavior.Text = "SingleBehavior";
            this.rbtnSingleBehavior.UseVisualStyleBackColor = true;
            // 
            // rbtnRoboTutor
            // 
            this.rbtnRoboTutor.AutoSize = true;
            this.rbtnRoboTutor.Checked = true;
            this.rbtnRoboTutor.Location = new System.Drawing.Point(206, 31);
            this.rbtnRoboTutor.Name = "rbtnRoboTutor";
            this.rbtnRoboTutor.Size = new System.Drawing.Size(83, 19);
            this.rbtnRoboTutor.TabIndex = 47;
            this.rbtnRoboTutor.TabStop = true;
            this.rbtnRoboTutor.Text = "RoboTutor";
            this.rbtnRoboTutor.UseVisualStyleBackColor = true;
            this.rbtnRoboTutor.CheckedChanged += new System.EventHandler(this.rbtnMode_CheckedChanged);
            // 
            // rbtnVideoAllBehaviors
            // 
            this.rbtnVideoAllBehaviors.AutoSize = true;
            this.rbtnVideoAllBehaviors.Location = new System.Drawing.Point(206, 81);
            this.rbtnVideoAllBehaviors.Name = "rbtnVideoAllBehaviors";
            this.rbtnVideoAllBehaviors.Size = new System.Drawing.Size(123, 19);
            this.rbtnVideoAllBehaviors.TabIndex = 46;
            this.rbtnVideoAllBehaviors.Text = "VideoAllBehaviors";
            this.rbtnVideoAllBehaviors.UseVisualStyleBackColor = true;
            this.rbtnVideoAllBehaviors.CheckedChanged += new System.EventHandler(this.rbtnMode_CheckedChanged);
            // 
            // btnStartCrgBehavior
            // 
            this.btnStartCrgBehavior.BackColor = System.Drawing.Color.LightGreen;
            this.btnStartCrgBehavior.Enabled = false;
            this.btnStartCrgBehavior.Location = new System.Drawing.Point(366, 83);
            this.btnStartCrgBehavior.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.btnStartCrgBehavior.Name = "btnStartCrgBehavior";
            this.btnStartCrgBehavior.Size = new System.Drawing.Size(147, 51);
            this.btnStartCrgBehavior.TabIndex = 40;
            this.btnStartCrgBehavior.Text = "Start \r\nCrg Behavior";
            this.btnStartCrgBehavior.UseVisualStyleBackColor = false;
            this.btnStartCrgBehavior.Click += new System.EventHandler(this.btnStartCrgBehavior_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 15);
            this.label3.TabIndex = 39;
            this.label3.Text = "Crg Behavior";
            // 
            // cbbCrgBehaviors
            // 
            this.cbbCrgBehaviors.FormattingEnabled = true;
            this.cbbCrgBehaviors.Location = new System.Drawing.Point(14, 99);
            this.cbbCrgBehaviors.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.cbbCrgBehaviors.Name = "cbbCrgBehaviors";
            this.cbbCrgBehaviors.Size = new System.Drawing.Size(114, 23);
            this.cbbCrgBehaviors.TabIndex = 38;
            this.cbbCrgBehaviors.SelectedIndexChanged += new System.EventHandler(this.cbbCrgBehaviors_SelectedIndexChanged);
            // 
            // btnHeadPose
            // 
            this.btnHeadPose.Location = new System.Drawing.Point(380, 400);
            this.btnHeadPose.Name = "btnHeadPose";
            this.btnHeadPose.Size = new System.Drawing.Size(93, 40);
            this.btnHeadPose.TabIndex = 39;
            this.btnHeadPose.Text = "HeadPose";
            this.btnHeadPose.UseVisualStyleBackColor = true;
            this.btnHeadPose.Click += new System.EventHandler(this.btnHeadPose_Click);
            // 
            // NaoAffect
            // 
            this.NaoAffect.Arousal = 0D;
            this.NaoAffect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.NaoAffect.Location = new System.Drawing.Point(540, 14);
            this.NaoAffect.Margin = new System.Windows.Forms.Padding(4);
            this.NaoAffect.Name = "NaoAffect";
            this.NaoAffect.Size = new System.Drawing.Size(503, 479);
            this.NaoAffect.TabIndex = 42;
            this.NaoAffect.Valence = 0D;
            // 
            // btnInsertCrgBehavior
            // 
            this.btnInsertCrgBehavior.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnInsertCrgBehavior.Location = new System.Drawing.Point(129, 97);
            this.btnInsertCrgBehavior.Name = "btnInsertCrgBehavior";
            this.btnInsertCrgBehavior.Size = new System.Drawing.Size(25, 25);
            this.btnInsertCrgBehavior.TabIndex = 48;
            this.btnInsertCrgBehavior.Text = "+";
            this.btnInsertCrgBehavior.UseVisualStyleBackColor = true;
            this.btnInsertCrgBehavior.Click += new System.EventHandler(this.btnInsertCrgBehavior_Click);
            // 
            // btnRemoveCrgBehavior
            // 
            this.btnRemoveCrgBehavior.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRemoveCrgBehavior.Location = new System.Drawing.Point(154, 97);
            this.btnRemoveCrgBehavior.Name = "btnRemoveCrgBehavior";
            this.btnRemoveCrgBehavior.Size = new System.Drawing.Size(25, 25);
            this.btnRemoveCrgBehavior.TabIndex = 49;
            this.btnRemoveCrgBehavior.Text = "-";
            this.btnRemoveCrgBehavior.UseVisualStyleBackColor = true;
            this.btnRemoveCrgBehavior.Click += new System.EventHandler(this.btnRemoveCrgBehavior_Click);
            // 
            // NaoRobotManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnHeadPose);
            this.Controls.Add(this.NaoAffect);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.gbControl);
            this.Controls.Add(this.pnlNaoConnector);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "NaoRobotManager";
            this.Size = new System.Drawing.Size(1045, 525);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlayAllStep)).EndInit();
            this.gbControl.ResumeLayout(false);
            this.gbControl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlNaoConnector;
        private System.Windows.Forms.CheckBox cbEndlessMove;
        private System.Windows.Forms.Button btnStartModulatedBehavior;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbbModulatedBehaviors;
        private System.Windows.Forms.GroupBox gbControl;
        private System.Windows.Forms.Button btnStartCrgBehavior;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbbCrgBehaviors;
        private System.Windows.Forms.Button btnHeadPose;
        private System.Windows.Forms.Button btnListParams;
        private System.Windows.Forms.NumericUpDown nudPlayAllStep;
        private System.Windows.Forms.Button btnPlayAll;
        private System.Windows.Forms.RadioButton rbtnVideoAllBehaviors;
        private System.Windows.Forms.RadioButton rbtnSingleBehavior;
        private System.Windows.Forms.RadioButton rbtnRoboTutor;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtbCurBehavior;
        private System.Windows.Forms.Button btnStopPlayAll;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnPlayAllList;
        private NaoAffectManager.AffectMonitor NaoAffect;
        private System.Windows.Forms.Button btnInsertCrgBehavior;
        private System.Windows.Forms.Button btnRemoveCrgBehavior;
    }
}