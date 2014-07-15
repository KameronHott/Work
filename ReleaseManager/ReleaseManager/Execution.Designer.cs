namespace ReleaseManager
{
    partial class Execution
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
            this.gbCompass = new System.Windows.Forms.GroupBox();
            this.tbExecuteViewer = new System.Windows.Forms.TextBox();
            this.pbar = new System.Windows.Forms.ProgressBar();
            this.lPBE = new System.Windows.Forms.Label();
            this.lPB0 = new System.Windows.Forms.Label();
            this.gbCompass.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbCompass
            // 
            this.gbCompass.Controls.Add(this.tbExecuteViewer);
            this.gbCompass.Location = new System.Drawing.Point(12, 12);
            this.gbCompass.Name = "gbCompass";
            this.gbCompass.Size = new System.Drawing.Size(592, 458);
            this.gbCompass.TabIndex = 0;
            this.gbCompass.TabStop = false;
            this.gbCompass.Text = "Progress Viewer";
            // 
            // tbExecuteViewer
            // 
            this.tbExecuteViewer.Location = new System.Drawing.Point(6, 19);
            this.tbExecuteViewer.Multiline = true;
            this.tbExecuteViewer.Name = "tbExecuteViewer";
            this.tbExecuteViewer.ReadOnly = true;
            this.tbExecuteViewer.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbExecuteViewer.Size = new System.Drawing.Size(571, 433);
            this.tbExecuteViewer.TabIndex = 0;
            this.tbExecuteViewer.TextChanged += new System.EventHandler(this.tbExecuteCompass_TextChanged);
            // 
            // pbar
            // 
            this.pbar.Location = new System.Drawing.Point(28, 473);
            this.pbar.Name = "pbar";
            this.pbar.Size = new System.Drawing.Size(523, 15);
            this.pbar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbar.TabIndex = 19;
            // 
            // lPBE
            // 
            this.lPBE.AutoSize = true;
            this.lPBE.Location = new System.Drawing.Point(557, 475);
            this.lPBE.Name = "lPBE";
            this.lPBE.Size = new System.Drawing.Size(13, 13);
            this.lPBE.TabIndex = 20;
            this.lPBE.Text = "0";
            // 
            // lPB0
            // 
            this.lPB0.AutoSize = true;
            this.lPB0.Location = new System.Drawing.Point(9, 475);
            this.lPB0.Name = "lPB0";
            this.lPB0.Size = new System.Drawing.Size(13, 13);
            this.lPB0.TabIndex = 21;
            this.lPB0.Text = "0";
            // 
            // Execution
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 495);
            this.Controls.Add(this.lPB0);
            this.Controls.Add(this.lPBE);
            this.Controls.Add(this.pbar);
            this.Controls.Add(this.gbCompass);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(700, 100);
            this.Name = "Execution";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Execution";
            this.Load += new System.EventHandler(this.Execution_Load);
            this.gbCompass.ResumeLayout(false);
            this.gbCompass.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbCompass;
        public System.Windows.Forms.TextBox tbExecuteViewer;
        public System.Windows.Forms.ProgressBar pbar;
        private System.Windows.Forms.Label lPBE;
        private System.Windows.Forms.Label lPB0;
    }
}