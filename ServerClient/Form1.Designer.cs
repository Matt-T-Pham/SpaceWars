namespace ServerClient
{
    partial class Form1
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
            this.Connect = new System.Windows.Forms.Button();
            this.SeverInput = new System.Windows.Forms.TextBox();
            this.messages = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Connect
            // 
            this.Connect.Location = new System.Drawing.Point(1087, 12);
            this.Connect.Name = "Connect";
            this.Connect.Size = new System.Drawing.Size(446, 31);
            this.Connect.TabIndex = 0;
            this.Connect.Text = "ConnectToSever";
            this.Connect.UseVisualStyleBackColor = true;
            this.Connect.Click += new System.EventHandler(this.Connect_Click);
            // 
            // SeverInput
            // 
            this.SeverInput.Location = new System.Drawing.Point(12, 12);
            this.SeverInput.Name = "SeverInput";
            this.SeverInput.Size = new System.Drawing.Size(463, 31);
            this.SeverInput.TabIndex = 1;
            // 
            // messages
            // 
            this.messages.Location = new System.Drawing.Point(485, 12);
            this.messages.Name = "messages";
            this.messages.Size = new System.Drawing.Size(596, 31);
            this.messages.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1552, 1505);
            this.Controls.Add(this.messages);
            this.Controls.Add(this.SeverInput);
            this.Controls.Add(this.Connect);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Connect;
        private System.Windows.Forms.TextBox SeverInput;
        private System.Windows.Forms.TextBox messages;
    }
}

