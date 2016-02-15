namespace ScriptEngine
{
    partial class ScriptEditor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptEditor));
            this.nudJumpNum = new System.Windows.Forms.NumericUpDown();
            this.nudJumpNum.Maximum = 999; // RUUD
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.fctbScriptEditor = new FastColoredTextBoxNS.FastColoredTextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.tsUpper = new System.Windows.Forms.ToolStrip();
            this.tsbtnJumpLine = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbtnSave = new System.Windows.Forms.ToolStripSplitButton();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbtnCheck = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbtnSymbol = new System.Windows.Forms.ToolStripDropDownButton();
            this.insertBehaviorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertQuizToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbtnInsertBehavior = new System.Windows.Forms.ToolStripButton();
            this.tsbtnRemoveBehavior = new System.Windows.Forms.ToolStripButton();
            this.tsbtnInsertQuiz = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.nudJumpNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fctbScriptEditor)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.tsUpper.SuspendLayout();
            this.SuspendLayout();
            // 
            // nudJumpNum
            // 
            this.nudJumpNum.AutoSize = true;
            this.nudJumpNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nudJumpNum.Location = new System.Drawing.Point(3, 3);
            this.nudJumpNum.Name = "nudJumpNum";
            this.nudJumpNum.Size = new System.Drawing.Size(90, 45);
            this.nudJumpNum.TabIndex = 6;
            // 
            // btnExecute
            // 
            this.btnExecute.BackColor = System.Drawing.Color.Transparent;
            this.btnExecute.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnExecute.BackgroundImage")));
            this.btnExecute.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExecute.Enabled = false;
            this.btnExecute.Location = new System.Drawing.Point(84, 3);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 75);
            this.btnExecute.TabIndex = 3;
            this.btnExecute.UseVisualStyleBackColor = false;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnLoad.BackgroundImage")));
            this.btnLoad.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLoad.Location = new System.Drawing.Point(3, 3);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 75);
            this.btnLoad.TabIndex = 4;
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // fctbScriptEditor
            // 
            this.fctbScriptEditor.AutoScrollMinSize = new System.Drawing.Size(0, 18);
            this.fctbScriptEditor.BackBrush = null;
            this.fctbScriptEditor.CharHeight = 18;
            this.fctbScriptEditor.CharWidth = 10;
            this.fctbScriptEditor.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fctbScriptEditor.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fctbScriptEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fctbScriptEditor.IsReplaceMode = false;
            this.fctbScriptEditor.Location = new System.Drawing.Point(3, 61);
            this.fctbScriptEditor.Name = "fctbScriptEditor";
            this.fctbScriptEditor.Paddings = new System.Windows.Forms.Padding(0);
            this.fctbScriptEditor.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fctbScriptEditor.Size = new System.Drawing.Size(600, 500);
            this.fctbScriptEditor.TabIndex = 1;
            this.fctbScriptEditor.WordWrap = true;
            this.fctbScriptEditor.Zoom = 100;
            this.fctbScriptEditor.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fctbScriptEditor_TextChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.btnLoad);
            this.flowLayoutPanel1.Controls.Add(this.btnExecute);
            this.flowLayoutPanel1.Controls.Add(this.btnPause);
            this.flowLayoutPanel1.Controls.Add(this.btnStop);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 567);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(600, 81);
            this.flowLayoutPanel1.TabIndex = 7;
            // 
            // btnPause
            // 
            this.btnPause.BackColor = System.Drawing.Color.Transparent;
            this.btnPause.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnPause.BackgroundImage")));
            this.btnPause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPause.Enabled = false;
            this.btnPause.Location = new System.Drawing.Point(165, 3);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 75);
            this.btnPause.TabIndex = 6;
            this.btnPause.UseVisualStyleBackColor = false;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.Transparent;
            this.btnStop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnStop.BackgroundImage")));
            this.btnStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(246, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 75);
            this.btnStop.TabIndex = 7;
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.fctbScriptEditor, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(606, 651);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.nudJumpNum);
            this.flowLayoutPanel2.Controls.Add(this.tsUpper);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(574, 52);
            this.flowLayoutPanel2.TabIndex = 9;
            // 
            // tsUpper
            // 
            this.tsUpper.BackColor = System.Drawing.Color.Transparent;
            this.tsUpper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tsUpper.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tsUpper.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.tsUpper.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnJumpLine,
            this.toolStripSeparator1,
            this.tsbtnSave,
            this.tsbtnCheck,
            this.toolStripSeparator2,
            this.tsbtnSymbol,
            this.tsbtnInsertBehavior,
            this.tsbtnRemoveBehavior,
            this.tsbtnInsertQuiz});
            this.tsUpper.Location = new System.Drawing.Point(96, 0);
            this.tsUpper.Name = "tsUpper";
            this.tsUpper.Size = new System.Drawing.Size(345, 51);
            this.tsUpper.TabIndex = 7;
            this.tsUpper.Text = "toolStripUpper";
            // 
            // tsbtnJumpLine
            // 
            this.tsbtnJumpLine.AutoSize = false;
            this.tsbtnJumpLine.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tsbtnJumpLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnJumpLine.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnJumpLine.Image")));
            this.tsbtnJumpLine.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnJumpLine.Name = "tsbtnJumpLine";
            this.tsbtnJumpLine.Size = new System.Drawing.Size(40, 40);
            this.tsbtnJumpLine.Text = "Jump Line";
            this.tsbtnJumpLine.Click += new System.EventHandler(this.tsbtnJumpLine_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 51);
            // 
            // tsbtnSave
            // 
            this.tsbtnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tsbtnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnSave.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.tsbtnSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnSave.Image")));
            this.tsbtnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnSave.Name = "tsbtnSave";
            this.tsbtnSave.Size = new System.Drawing.Size(56, 48);
            this.tsbtnSave.Text = "Save";
            this.tsbtnSave.ButtonClick += new System.EventHandler(this.tsbtnSave_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(134, 24);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.tsbtnSave_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(134, 24);
            this.saveAsToolStripMenuItem.Text = "SaveAs...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // tsbtnCheck
            // 
            this.tsbtnCheck.AutoSize = false;
            this.tsbtnCheck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tsbtnCheck.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnCheck.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnCheck.Image")));
            this.tsbtnCheck.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnCheck.Name = "tsbtnCheck";
            this.tsbtnCheck.Size = new System.Drawing.Size(40, 40);
            this.tsbtnCheck.Text = "Check";
            this.tsbtnCheck.Click += new System.EventHandler(this.tsbtnCheck_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 51);
            // 
            // tsbtnSymbol
            // 
            this.tsbtnSymbol.BackColor = System.Drawing.Color.Transparent;
            this.tsbtnSymbol.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnSymbol.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertBehaviorToolStripMenuItem,
            this.insertQuizToolStripMenuItem});
            this.tsbtnSymbol.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tsbtnSymbol.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnSymbol.Image")));
            this.tsbtnSymbol.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnSymbol.Name = "tsbtnSymbol";
            this.tsbtnSymbol.Size = new System.Drawing.Size(53, 48);
            this.tsbtnSymbol.Text = "Symbol";
            this.tsbtnSymbol.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // insertBehaviorToolStripMenuItem
            // 
            this.insertBehaviorToolStripMenuItem.Name = "insertBehaviorToolStripMenuItem";
            this.insertBehaviorToolStripMenuItem.Size = new System.Drawing.Size(176, 24);
            this.insertBehaviorToolStripMenuItem.Text = "Insert behavior";
            this.insertBehaviorToolStripMenuItem.Click += new System.EventHandler(this.insertBehavior_Click);
            // 
            // insertQuizToolStripMenuItem
            // 
            this.insertQuizToolStripMenuItem.Name = "insertQuizToolStripMenuItem";
            this.insertQuizToolStripMenuItem.Size = new System.Drawing.Size(176, 24);
            this.insertQuizToolStripMenuItem.Text = "Insert quiz";
            this.insertQuizToolStripMenuItem.Click += new System.EventHandler(this.insertQuiz_Click);
            // 
            // tsbtnInsertBehavior
            // 
            this.tsbtnInsertBehavior.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnInsertBehavior.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnInsertBehavior.Image")));
            this.tsbtnInsertBehavior.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnInsertBehavior.Name = "tsbtnInsertBehavior";
            this.tsbtnInsertBehavior.Size = new System.Drawing.Size(44, 48);
            this.tsbtnInsertBehavior.Text = "InsertBehavior";
            this.tsbtnInsertBehavior.Click += new System.EventHandler(this.insertBehavior_Click);
            // 
            // tsbtnRemoveBehavior
            // 
            this.tsbtnRemoveBehavior.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnRemoveBehavior.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnRemoveBehavior.Image")));
            this.tsbtnRemoveBehavior.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnRemoveBehavior.Name = "tsbtnRemoveBehavior";
            this.tsbtnRemoveBehavior.Size = new System.Drawing.Size(44, 48);
            this.tsbtnRemoveBehavior.Text = "BehaviorRemove";
            this.tsbtnRemoveBehavior.Click += new System.EventHandler(this.tsbtnRemoveBehavior_Click);
            // 
            // tsbtnInsertQuiz
            // 
            this.tsbtnInsertQuiz.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnInsertQuiz.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnInsertQuiz.Image")));
            this.tsbtnInsertQuiz.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnInsertQuiz.Name = "tsbtnInsertQuiz";
            this.tsbtnInsertQuiz.Size = new System.Drawing.Size(44, 48);
            this.tsbtnInsertQuiz.Text = "InsertQuiz";
            this.tsbtnInsertQuiz.Click += new System.EventHandler(this.insertQuiz_Click);
            // 
            // ScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "ScriptEditor";
            this.Size = new System.Drawing.Size(606, 651);
            ((System.ComponentModel.ISupportInitialize)(this.nudJumpNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fctbScriptEditor)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.tsUpper.ResumeLayout(false);
            this.tsUpper.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nudJumpNum;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnLoad;
        private FastColoredTextBoxNS.FastColoredTextBox fctbScriptEditor;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.ToolStrip tsUpper;
        private System.Windows.Forms.ToolStripSplitButton tsbtnSave;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton tsbtnJumpLine;
        private System.Windows.Forms.ToolStripDropDownButton tsbtnSymbol;
        private System.Windows.Forms.ToolStripMenuItem insertBehaviorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertQuizToolStripMenuItem;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.ToolStripButton tsbtnCheck;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ToolStripButton tsbtnInsertBehavior;
        private System.Windows.Forms.ToolStripButton tsbtnInsertQuiz;
        private System.Windows.Forms.ToolStripButton tsbtnRemoveBehavior;
    }
}