namespace _3DPRN_Fiware
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
         this.txtprnstatus = new System.Windows.Forms.TextBox();
         this.SuspendLayout();
         // 
         // txtprnstatus
         // 
         this.txtprnstatus.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtprnstatus.Location = new System.Drawing.Point(0, 0);
         this.txtprnstatus.Multiline = true;
         this.txtprnstatus.Name = "txtprnstatus";
         this.txtprnstatus.Size = new System.Drawing.Size(841, 427);
         this.txtprnstatus.TabIndex = 0;
         this.txtprnstatus.Text = "Test";
         // 
         // frmMain
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(841, 427);
         this.Controls.Add(this.txtprnstatus);
         this.Name = "frmMain";
         this.Text = "3Dprn Fiware";
         this.Shown += new System.EventHandler(this.frmMain_Shown);
         this.ResumeLayout(false);
         this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtprnstatus;
    }
}
