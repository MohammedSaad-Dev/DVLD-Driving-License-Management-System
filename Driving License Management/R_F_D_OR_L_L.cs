using DB_Layer;
using Driving_License_Management.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Driving_License_Management
{
    public partial class R_F_D_OR_L_L : Form
    {
        int _LicenseID = -1;
        int _DriverID = -1;
        int _PersonID = -1;
        bool _IsActive = false;
        bool _IsDetained = false;
        DateTime _ExpirationDate;
        private int _OldLicenseID = -1;
        private int _NewLicenseID = -1;
        private int _NewApplicationID = -1;
        public R_F_D_OR_L_L()
        {
            InitializeComponent();
        }

        private void R_F_D_OR_L_L_Load(object sender, EventArgs e)
        {
            ResetFormInfo();
        }

     
        private void gbFilter_Click(object sender, EventArgs e)
        {

            /*
            if (!_IsActive)
            {
                MessageBox.Show("This license is not active (Not Active)! An international license cannot be issued for it.", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnIssue.Enabled = false;
                return;
            }

            if (_IsDetained)
            {
                MessageBox.Show("This license is detained! You need to lift the detention first.", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnIssue.Enabled = false;
                return;
            }

            if (_ExpirationDate < DateTime.Now)
            {
                MessageBox.Show("This license is expired! You need to renew it first.", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnIssue.Enabled = false;
                return;
            }
            */

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please enter the license number (License ID)", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBox1.Text.Trim(), out int licenseID))
            {
                MessageBox.Show("Please enter a valid number!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            string query = @"
        SELECT 
            L.LicenseID,
            L.ApplicationID,
            L.DriverID,
            L.IssueDate,
            L.ExpirationDate,
            L.Notes,
            L.IsActive,
            L.IssueReason,
            LC.ClassName,
            P.PersonID,
            P.NationalNo,
            (P.FirstName + ' ' + P.SecondName + ' ' + ISNULL(P.ThirdName, '') + ' ' + P.LastName) AS FullName,
            P.Gendor,
            P.DateOfBirth,
            P.ImagePath,
            
            ISNULL((SELECT TOP 1 IsReleased FROM DetainedLicenses 
                    WHERE LicenseID = L.LicenseID 
                    ORDER BY DetainID DESC), 1) AS IsReleasedStatus
        FROM Licenses L
        INNER JOIN LicenseClasses LC ON L.LicenseClass = LC.LicenseClassID
        INNER JOIN Drivers D ON L.DriverID = D.DriverID
        INNER JOIN People P ON D.PersonID = P.PersonID
        WHERE L.LicenseID = @LicenseID";

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LicenseID", licenseID);

                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            _LicenseID = Convert.ToInt32(reader["LicenseID"]);
                            _DriverID = Convert.ToInt32(reader["DriverID"]);
                            _PersonID = Convert.ToInt32(reader["PersonID"]);
                            _IsActive = Convert.ToBoolean(reader["IsActive"]);
                            _ExpirationDate = Convert.ToDateTime(reader["ExpirationDate"]);

                            byte gender = Convert.ToByte(reader["Gendor"]);
                            byte issueReason = Convert.ToByte(reader["IssueReason"]);

                            byte isReleasedStatus = Convert.ToByte(reader["IsReleasedStatus"]);
                            _IsDetained = (isReleasedStatus == 0);

                            lblClass.Text = reader["ClassName"].ToString();
                            lblFullName.Text = reader["FullName"].ToString();
                            lblNationalNo.Text = reader["NationalNo"].ToString();
                            lblGender.Text = (gender == 0) ? "Male" : "Female";
                            lblLicenseID.Text = _LicenseID.ToString();
                            lblDriverID.Text = _DriverID.ToString();
                            lblIssueDate.Text = Convert.ToDateTime(reader["IssueDate"]).ToString("dd/MMM/yyyy");
                            lblExpirationDate.Text = _ExpirationDate.ToString("dd/MMM/yyyy");
                            lblDateOfBirth.Text = Convert.ToDateTime(reader["DateOfBirth"]).ToString("dd/MMM/yyyy");
                            lblLocalLicenseID.Text = _LicenseID.ToString();
                            lblIsActive.Text = _IsActive ? "Yes" : "No";
                            lblIsDetained.Text = _IsDetained ? "Yes" : "No";
                            lblApplicationDate.Text = DateTime.Now.ToString("dd/MMM/yyyy");
                            lblCreatedBy.Text = clsGlobal.CurrentUserName;


                            if (rbDamaged.Checked == true)
                            {
                                lblApplicationFees.Text = "5";
                            }
                            else
                            {
                                lblApplicationFees.Text = "10";
                            }
                            switch (issueReason)
                            {
                                case 1: lblIssueReason.Text = "First Time"; break;
                                case 2: lblIssueReason.Text = "Renew"; break;
                                case 3: lblIssueReason.Text = "Replacement for Damaged"; break;
                                case 4: lblIssueReason.Text = "Replacement for Lost"; break;
                                default: lblIssueReason.Text = "First Time"; break;
                            }

                            lblNotes.Text = string.IsNullOrEmpty(reader["Notes"].ToString()) ? "No Notes" : reader["Notes"].ToString();

                            if (reader["ImagePath"] != DBNull.Value && !string.IsNullOrEmpty(reader["ImagePath"].ToString()))
                            {
                                pbPersonImage.ImageLocation = reader["ImagePath"].ToString();
                            }

                        }
                        else
                        {
                            MessageBox.Show($"No license was found with the number {licenseID}", "Not available", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ResetFormInfo();

                            reader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred during the display: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }



        private void ResetFormInfo()
        {


            lblClass.Text = "[???]";
            lblNationalNo.Text = "[???]";
            lblGender.Text = "[???]";
            lblLicenseID.Text = "[???]";
            lblDriverID.Text = "[???]";
            lblExpirationDate.Text = "[???]";
            lblIsActive.Text = "[???]";
            lblIsDetained.Text = "[???]";
            lblIssueReason.Text = "[???]";
            lblNotes.Text = "[???]";
            lblFullName.Text = "[???]";
            lblApplicationDate.Text = "[???]";
            lblCreatedBy.Text = "[???]";
            lblIssueDate.Text = "[???]";

            lblRLApplicationID.Text = "[???]";
            lblApplicationFees.Text = "[???]";
            lblLocalLicenseID.Text = "[???]";
            lblRenewedLicenseID.Text = "[???]";
            lblDateOfBirth.Text = "[???]";

            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            License_History frm = new License_History(_DriverID);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRenew_Click(object sender, EventArgs e)
        {

           

            if (lblIsActive.Text != "Yes")
            {
                MessageBox.Show("A replacement can't be issued for an inactive license.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (lblIsDetained.Text == "Yes")
            {
                MessageBox.Show("This license is reserved, you need to unreserve it firstاً.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int appTypeID = rbDamaged.Checked ? 4 : 3; 
            byte issueReason = rbDamaged.Checked ? (byte)3 : (byte)4; 
            float appFees = rbDamaged.Checked ? 5f : 10f;

            if (MessageBox.Show("Are you sure you want to replace this license?", "Confirm action", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                try
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {

                        string insertAppQuery = @"INSERT INTO Applications 
                                                  (ApplicantPersonID, ApplicationDate, ApplicationTypeID, ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID)
                                                  VALUES (@ApplicantPersonID, GETDATE(), @AppTypeID, 3, GETDATE(), @PaidFees, @CreatedByUserID);
                                                  SELECT SCOPE_IDENTITY();";

                        using (SqlCommand cmdApp = new SqlCommand(insertAppQuery, connection, transaction))
                        {
                            cmdApp.Parameters.AddWithValue("@ApplicantPersonID", _PersonID);
                            cmdApp.Parameters.AddWithValue("@AppTypeID", appTypeID);
                            cmdApp.Parameters.AddWithValue("@PaidFees", appFees);
                            cmdApp.Parameters.AddWithValue("@CreatedByUserID", clsGlobal.CurrentUserID);

                            _NewApplicationID = Convert.ToInt32(cmdApp.ExecuteScalar());

                        }
                        _OldLicenseID = Convert.ToInt32(lblLicenseID.Text);
                        string updateOldLicQuery = @"UPDATE Licenses SET IsActive = 0 WHERE LicenseID = @OldLicenseID";
                        using (SqlCommand cmdUpdate = new SqlCommand(updateOldLicQuery, connection, transaction))
                        {
                            cmdUpdate.Parameters.AddWithValue("@OldLicenseID", _OldLicenseID);
                            int rowsAffected = cmdUpdate.ExecuteNonQuery();
                            if (rowsAffected == 0)
                            {
                                transaction.Rollback();
                                MessageBox.Show("فشل في إلغاء تفعيل الرخصة القديمة، تأكد من رقم الرخصة.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }

                        DateTime oldExpirationDate = Convert.ToDateTime(lblExpirationDate.Text);

                        string insertNewLicQuery = @"INSERT INTO Licenses 
                                                     (ApplicationID, DriverID, LicenseClass, IssueDate, ExpirationDate, Notes, PaidFees, IsActive, IssueReason, CreatedByUserID)
                                                     VALUES 
                                                     (@ApplicationID, @DriverID, @LicenseClass, GETDATE(), @ExpirationDate, @Notes, 0, 1, @IssueReason, @CreatedByUserID);
                                                     SELECT SCOPE_IDENTITY();";

                        using (SqlCommand cmdLic = new SqlCommand(insertNewLicQuery, connection, transaction))
                        {
                            cmdLic.Parameters.AddWithValue("@ApplicationID", _NewApplicationID);
                            cmdLic.Parameters.AddWithValue("@DriverID", Convert.ToInt32(lblDriverID.Text));
                            cmdLic.Parameters.AddWithValue("@LicenseClass", 3);
                            cmdLic.Parameters.AddWithValue("@ExpirationDate", oldExpirationDate);
                            cmdLic.Parameters.AddWithValue("@Notes", DBNull.Value);
                            cmdLic.Parameters.AddWithValue("@IssueReason", issueReason);
                            cmdLic.Parameters.AddWithValue("@CreatedByUserID", 1);

                            _NewLicenseID = Convert.ToInt32(cmdLic.ExecuteScalar());
                        }

                        transaction.Commit();

                        MessageBox.Show($"The license replacement has been issued successfully! The new license number is:[{_NewLicenseID}]",
                                        "The operation is done", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        lblRLApplicationID.Text = _NewApplicationID.ToString();
                        lblRenewedLicenseID.Text = _NewLicenseID.ToString();

                        btnIssueReplacement.Enabled = false;
                        gbReplacementFor.Enabled = false;
                        gbFilter.Enabled = false;
                        llShowNewLicensesInfo.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("An error occurred during execution:" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to connect to the database: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void llShowNewLicensesInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Show_Lic frm = new Show_Lic(_NewLicenseID);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
        }
    }
}
