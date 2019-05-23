namespace ScreenViewer.Client
{
    partial class SystemInformation
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
            richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new System.Drawing.Point(22, 31);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new System.Drawing.Size(534, 280);
            richTextBox1.TabIndex = 4;
            richTextBox1.Text = "";
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 343);
            this.Controls.Add(richTextBox1);
            this.Name = "Form3";
            this.Text = "Системная информация";
            this.ResumeLayout(false);

        }

        #endregion

        public static System.Windows.Forms.RichTextBox richTextBox1;
    }
}