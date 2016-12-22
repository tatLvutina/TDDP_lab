namespace Server
{
    partial class Listener
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
            this.btnListen = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnListen
            // 
            this.btnListen.Location = new System.Drawing.Point(53, 43);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(135, 23);
            this.btnListen.TabIndex = 5;
            this.btnListen.Text = "Start Listening";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 26);
            this.label1.TabIndex = 6;
            this.label1.Text = "Server Port:\r\n\r\n";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(122, 13);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(63, 26);
            this.lblPort.TabIndex = 7;
            this.lblPort.Text = "Server Port:\r\n\r\n";
            // 
            // Listener
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 112);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnListen);
            this.Name = "Listener";
            this.Text = "Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnListen;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblPort;
    }
}

