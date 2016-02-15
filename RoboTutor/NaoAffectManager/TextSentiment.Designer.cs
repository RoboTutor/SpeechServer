namespace NaoAffectManager
{
    partial class TextSentiment
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
            this.components = new System.ComponentModel.Container();
            this.btnRequest = new System.Windows.Forms.Button();
            this.btnLaunchServer = new System.Windows.Forms.Button();
            this.zgcValence = new ZedGraph.ZedGraphControl();
            this.btnDrawCurve = new System.Windows.Forms.Button();
            this.cbSentiText = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnRequest
            // 
            this.btnRequest.Location = new System.Drawing.Point(84, 3);
            this.btnRequest.Name = "btnRequest";
            this.btnRequest.Size = new System.Drawing.Size(58, 38);
            this.btnRequest.TabIndex = 0;
            this.btnRequest.Text = "Request";
            this.btnRequest.UseVisualStyleBackColor = true;
            this.btnRequest.Click += new System.EventHandler(this.btnRequest_Click);
            // 
            // btnLaunchServer
            // 
            this.btnLaunchServer.Location = new System.Drawing.Point(3, 3);
            this.btnLaunchServer.Name = "btnLaunchServer";
            this.btnLaunchServer.Size = new System.Drawing.Size(75, 38);
            this.btnLaunchServer.TabIndex = 1;
            this.btnLaunchServer.Text = "Launch\r\nServer";
            this.btnLaunchServer.UseVisualStyleBackColor = true;
            this.btnLaunchServer.Click += new System.EventHandler(this.btnLaunchServer_Click);
            // 
            // zgcValence
            // 
            this.zgcValence.Location = new System.Drawing.Point(3, 47);
            this.zgcValence.Name = "zgcValence";
            this.zgcValence.ScrollGrace = 0D;
            this.zgcValence.ScrollMaxX = 0D;
            this.zgcValence.ScrollMaxY = 0D;
            this.zgcValence.ScrollMaxY2 = 0D;
            this.zgcValence.ScrollMinX = 0D;
            this.zgcValence.ScrollMinY = 0D;
            this.zgcValence.ScrollMinY2 = 0D;
            this.zgcValence.Size = new System.Drawing.Size(353, 197);
            this.zgcValence.TabIndex = 2;
            // 
            // btnDrawCurve
            // 
            this.btnDrawCurve.Location = new System.Drawing.Point(148, 3);
            this.btnDrawCurve.Name = "btnDrawCurve";
            this.btnDrawCurve.Size = new System.Drawing.Size(58, 38);
            this.btnDrawCurve.TabIndex = 3;
            this.btnDrawCurve.Text = "Curve";
            this.btnDrawCurve.UseVisualStyleBackColor = true;
            this.btnDrawCurve.Click += new System.EventHandler(this.btnDrawCurve_Click);
            // 
            // cbSentiText
            // 
            this.cbSentiText.AutoSize = true;
            this.cbSentiText.Location = new System.Drawing.Point(212, 15);
            this.cbSentiText.Name = "cbSentiText";
            this.cbSentiText.Size = new System.Drawing.Size(71, 17);
            this.cbSentiText.TabIndex = 4;
            this.cbSentiText.Text = "SentiText";
            this.cbSentiText.UseVisualStyleBackColor = true;
            // 
            // TextSentiment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbSentiText);
            this.Controls.Add(this.btnDrawCurve);
            this.Controls.Add(this.zgcValence);
            this.Controls.Add(this.btnLaunchServer);
            this.Controls.Add(this.btnRequest);
            this.Name = "TextSentiment";
            this.Size = new System.Drawing.Size(359, 248);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRequest;
        private System.Windows.Forms.Button btnLaunchServer;
        private ZedGraph.ZedGraphControl zgcValence;
        private System.Windows.Forms.Button btnDrawCurve;
        private System.Windows.Forms.CheckBox cbSentiText;
    }
}
