namespace Driving_License_Management
{
    partial class Update_Form
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
            this.update_Person1 = new Driving_License_Management.Update_Person();
            this.SuspendLayout();
            // 
            // update_Person1
            // 
            this.update_Person1.BackColor = System.Drawing.Color.Azure;
            this.update_Person1.Location = new System.Drawing.Point(6, 30);
            this.update_Person1.Name = "update_Person1";
            this.update_Person1.Size = new System.Drawing.Size(1224, 572);
            this.update_Person1.TabIndex = 0;
            this.update_Person1.Load += new System.EventHandler(this.update_Person1_Load);
            // 
            // Update_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1220, 588);
            this.Controls.Add(this.update_Person1);
            this.MaximizeBox = false;
            this.Name = "Update_Form";
            this.Text = "Update_Form";
            this.Load += new System.EventHandler(this.Update_Form_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public Update_Person update_Person1;
    }
}