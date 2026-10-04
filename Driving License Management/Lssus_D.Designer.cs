namespace Driving_License_Management
{
    partial class Lssus_D
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Lssus_D));
            this.driving_License_Application_Info1 = new Driving_License_Management.Driving_License_Application_Info();
            this.application_Basic_Info1 = new Driving_License_Management.Application_Basic_Info();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnIssue = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // driving_License_Application_Info1
            // 
            this.driving_License_Application_Info1.Location = new System.Drawing.Point(3, 3);
            this.driving_License_Application_Info1.Name = "driving_License_Application_Info1";
            this.driving_License_Application_Info1.Size = new System.Drawing.Size(1081, 124);
            this.driving_License_Application_Info1.TabIndex = 29;
            // 
            // application_Basic_Info1
            // 
            this.application_Basic_Info1.LocalDLAppID = -1;
            this.application_Basic_Info1.Location = new System.Drawing.Point(3, 133);
            this.application_Basic_Info1.Name = "application_Basic_Info1";
            this.application_Basic_Info1.Size = new System.Drawing.Size(1081, 309);
            this.application_Basic_Info1.TabIndex = 28;
            this.application_Basic_Info1.Load += new System.EventHandler(this.application_Basic_Info1_Load);
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(182, 460);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(889, 209);
            this.txtNotes.TabIndex = 30;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(114, 485);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(50, 25);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 33;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 481);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 29);
            this.label2.TabIndex = 31;
            this.label2.Text = "Notes :";
            // 
            // btnIssue
            // 
            this.btnIssue.BackColor = System.Drawing.Color.Blue;
            this.btnIssue.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnIssue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIssue.ForeColor = System.Drawing.Color.White;
            this.btnIssue.Location = new System.Drawing.Point(888, 685);
            this.btnIssue.Name = "btnIssue";
            this.btnIssue.Size = new System.Drawing.Size(183, 32);
            this.btnIssue.TabIndex = 34;
            this.btnIssue.Text = "Lssue";
            this.btnIssue.UseVisualStyleBackColor = false;
            this.btnIssue.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Blue;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(669, 685);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(183, 32);
            this.button2.TabIndex = 35;
            this.button2.Text = "Close";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Lssus_D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(1083, 738);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnIssue);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.driving_License_Application_Info1);
            this.Controls.Add(this.application_Basic_Info1);
            this.Name = "Lssus_D";
            this.Text = "Lssus_D";
            this.Load += new System.EventHandler(this.Lssus_D_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Driving_License_Application_Info driving_License_Application_Info1;
        private Application_Basic_Info application_Basic_Info1;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnIssue;
        private System.Windows.Forms.Button button2;
    }
}