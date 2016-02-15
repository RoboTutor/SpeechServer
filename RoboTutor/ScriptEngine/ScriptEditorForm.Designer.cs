namespace ScriptEngine
{
    partial class ScriptEditorForm
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
            this.scriptEditor1 = new ScriptEngine.ScriptEditor();
            this.SuspendLayout();
            // 
            // scriptEditor1
            // 
            this.scriptEditor1.AutoSize = true;
            this.scriptEditor1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.scriptEditor1.Location = new System.Drawing.Point(9, 10);
            this.scriptEditor1.Margin = new System.Windows.Forms.Padding(2);
            this.scriptEditor1.Name = "scriptEditor1";
            this.scriptEditor1.Size = new System.Drawing.Size(606, 651);
            this.scriptEditor1.TabIndex = 0;
            // 
            // ScriptEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 665);
            this.Controls.Add(this.scriptEditor1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ScriptEditorForm";
            this.Text = "ScriptEditorForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ScriptEditor scriptEditor1;
    }
}