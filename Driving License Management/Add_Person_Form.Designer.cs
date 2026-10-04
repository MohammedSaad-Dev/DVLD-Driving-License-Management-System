namespace Driving_License_Management
{
    partial class Add_Person_Form
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
            this.add_Person1 = new Driving_License_Management.Add_Person();
            this.SuspendLayout();
            // 
            // add_Person1
            // 
            this.add_Person1.AutoSize = true;
            this.add_Person1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.add_Person1.Location = new System.Drawing.Point(2, 3);
            this.add_Person1.Name = "add_Person1";
            this.add_Person1.Size = new System.Drawing.Size(1209, 545);
            this.add_Person1.TabIndex = 0;
            this.add_Person1.Load += new System.EventHandler(this.add_Person1_Load);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1204, 533);
            this.Controls.Add(this.add_Person1);
            this.MaximizeBox = false;
            this.Name = "Form3";
            this.Text = "Form3";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Add_Person add_Person1;
    }
}