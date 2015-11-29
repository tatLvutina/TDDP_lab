namespace MergeSortAdmin
{
    partial class InitForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtAddr = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtAddr
            // 
            this.txtAddr.Location = new System.Drawing.Point(12, 12);
            this.txtAddr.Name = "txtAddr";
            this.txtAddr.Size = new System.Drawing.Size(231, 20);
            this.txtAddr.TabIndex = 0;
            this.txtAddr.Text = "tcp://localhost:8080/MergeSort";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 38);
            this.btnConnect.MaximumSize = new System.Drawing.Size(231, 33);
            this.btnConnect.MinimumSize = new System.Drawing.Size(231, 33);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(231, 33);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect to server";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // InitForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(255, 80);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtAddr);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(271, 118);
            this.MinimumSize = new System.Drawing.Size(271, 118);
            this.Name = "InitForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MergeSortClient";
            this.Load += new System.EventHandler(this.InitForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAddr;
        private System.Windows.Forms.Button btnConnect;
    }
}

