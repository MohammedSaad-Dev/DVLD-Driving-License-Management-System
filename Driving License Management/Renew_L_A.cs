using DB_Layer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Driving_License_Management
{
    public partial class Renew_L_A : Form
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
        private int _DefaultValidityYears = 10;
        public Renew_L_A()
        {
            InitializeComponent();
        }

        


        private void ResetFormInfo()
        {


            lblClass.Text = "[???]";
            lblNationalNo.Text = "[???]";
            lblGender.Text = "[???]";
            lblLicenseID.Text = "[???]";
            lblDriverID.Text = "[???]";
            lblExpirationDate.Text = "[???]";
            lblDateOfBirth.Text = "[???]";
            lblIsActive.Text = "[???]";
            lblIsDetained.Text = "[???]";
            lblIssueReason.Text = "[???]";
            lblNotes.Text = "[???]";
            lblFullName.Text = "[???]";
            lblRLApplicationID.Text = "[???]";
            lblApplicationDate.Text = "[???]";
            lblFees.Text = "[???]";
            lblRenewedLicenseID.Text = "[???]";
            lblExpirationDateApp.Text = "[???]";
            lblCreatedBy.Text = "[???]";
            lblIssueDate.Text = "[???]";

            lblIssueDate222222222.Text = "[???]";
            lblApplicationFees.Text = "[???]";
            lblLicenseFees.Text = "[???]";
            Old_L.Text = "[???]";
        }
        private void btnSearch_Click(object sender, EventArgs e)
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
                            Old_L.Text = _LicenseID.ToString();
                            lblIsActive.Text = _IsActive ? "Yes" : "No";
                            lblIsDetained.Text = _IsDetained ? "Yes" : "No";

                            switch (issueReason)
                            {
                                case 1: lblIssueReason.Text = "First Time"; break;
                                case 2: lblIssueReason.Text = "Renew"; break;
                                case 3: lblIssueReason.Text = "Replacement for Damaged"; break;
                                case 4: lblIssueReason.Text = "Replacement for Lost"; break;
                                default: lblIssueReason.Text = "First Time"; break;
                            }

                            lblNotes.Text = string.IsNullOrEmpty(reader["Notes"].ToString()) ? "No Notes" : reader["Notes"].ToString();

                         
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







        private void Renew_L_A_Load(object sender, EventArgs e)
        {
            ResetFormInfo();
            llShowNewLicenseInfo.Enabled = false;

        }

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            Show_Lic frm = new Show_Lic(_NewLicenseID);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();

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

        private void btnIssue_Click(object sender, EventArgs e)
        {


          
            DateTime expirationDate = Convert.ToDateTime(lblExpirationDate.Text);
            if (expirationDate > DateTime.Now)
            {
                MessageBox.Show($"The license can't be renewed because it's still valid until {expirationDate.ToShortDateString()}.",
                                "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (lblIsActive.Text != "Yes")
            {
                MessageBox.Show("This license is inactive.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (lblIsDetained.Text == "Yes")
            {
                MessageBox.Show("This license is blocked, you need to lift the block first before renewing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Are you sure you want to renew this license?", "Renewal confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
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
                                                  VALUES (@ApplicantPersonID, GETDATE(), 2, 3, GETDATE(), @PaidFees, @CreatedByUserID);
                                                  SELECT SCOPE_IDENTITY();";

                        using (SqlCommand cmdApp = new SqlCommand(insertAppQuery, connection, transaction))
                        {
                            cmdApp.Parameters.AddWithValue("@ApplicantPersonID", _DriverID); 
                            cmdApp.Parameters.AddWithValue("@PaidFees", Convert.ToSingle(lblApplicationFees.Text));
                            cmdApp.Parameters.AddWithValue("@CreatedByUserID", 1); 
                            _NewApplicationID = Convert.ToInt32(cmdApp.ExecuteScalar());
                        }

                        string updateOldLicQuery = @"UPDATE Licenses SET IsActive = 0 WHERE LicenseID = @OldLicenseID";
                        using (SqlCommand cmdUpdate = new SqlCommand(updateOldLicQuery, connection, transaction))
                        {
                            cmdUpdate.Parameters.AddWithValue("@OldLicenseID", _OldLicenseID);
                            cmdUpdate.ExecuteNonQuery();
                        }

                        string insertNewLicQuery = @"INSERT INTO Licenses 
                                                     (ApplicationID, DriverID, LicenseClass, IssueDate, ExpirationDate, Notes, PaidFees, IsActive, IssueReason, CreatedByUserID)
                                                     VALUES 
                                                     (@ApplicationID, @DriverID, @LicenseClass, GETDATE(), @ExpirationDate, @Notes, @PaidFees, 1, 2, @CreatedByUserID);
                                                     SELECT SCOPE_IDENTITY();";

                        using (SqlCommand cmdLic = new SqlCommand(insertNewLicQuery, connection, transaction))
                        {
                            cmdLic.Parameters.AddWithValue("@ApplicationID", _NewApplicationID);
                            cmdLic.Parameters.AddWithValue("@DriverID", _DriverID);
                            cmdLic.Parameters.AddWithValue("@LicenseClass", 3); 
                            cmdLic.Parameters.AddWithValue("@ExpirationDate", DateTime.Now.AddYears(_DefaultValidityYears));
                            cmdLic.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text.Trim());
                            cmdLic.Parameters.AddWithValue("@PaidFees", Convert.ToSingle(lblLicenseFees.Text));
                            cmdLic.Parameters.AddWithValue("@CreatedByUserID", 1);

                            _NewLicenseID = Convert.ToInt32(cmdLic.ExecuteScalar());
                        }

                        transaction.Commit();

                        MessageBox.Show($"The license has been successfully renewed! The new license number is [{_NewLicenseID}]",
                                        "Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        lblRLApplicationID.Text = _NewApplicationID.ToString();
                        lblRenewedLicenseID.Text = _NewLicenseID.ToString();

                        btnRenew.Enabled = false;
                        gbFilter.Enabled = false;
                        llShowNewLicenseInfo.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("An error occurred while performing the renewal: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to connect to the database: " + ex.Message, "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                llShowNewLicenseInfo.Enabled = true;

            }

        }
    }
}
