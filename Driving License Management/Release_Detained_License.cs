using DB_Layer;
using Driving_License_Management.Global;
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
    public partial class Release_Detained_License : Form
    {
        int _PersonID;
        int _LicenseID = -1;
        int _DriverID = -1;
        bool _IsActive = false;
        bool _IsDetained = false;
        DateTime _ExpirationDate;

        public Release_Detained_License()
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
            lblDetainID.Text = "[???]";
            lblCreatedBy.Text = "[???]";
            lblIssueDate.Text = "[???]";
            lblDetainDate.Text = "[???]";
            lblLocalLicenseID.Text = "[???]";
            lblApplicationID.Text ="[???]";
            lblApplicationFees.Text = "[???]";
            lblTotalFees.Text = "[???]";
            lblFineFees.Text = "[???]";

        }



        private void LoadDetainAndFeeInfo(int licenseID)
        {

            string query = @"
        SELECT TOP 1 
            DL.DetainID,
            DL.DetainDate,
            DL.FineFees,
            U.UserName AS CreatedBy,
            AppType.ApplicationFees
        FROM DetainedLicenses DL
        INNER JOIN Users U ON DL.CreatedByUserID = U.UserID
        CROSS JOIN ApplicationTypes AppType
        WHERE DL.LicenseID = @LicenseID 
          AND DL.IsReleased = 0
          AND AppType.ApplicationTypeID = 5
        ORDER BY DL.DetainID DESC;";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@LicenseID", licenseID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        int detainID = reader["DetainID"] != DBNull.Value ? Convert.ToInt32(reader["DetainID"]) : -1;
                        DateTime detainDate = reader["DetainDate"] != DBNull.Value ? Convert.ToDateTime(reader["DetainDate"]) : DateTime.Now;
                        decimal fineFees = reader["FineFees"] != DBNull.Value ? Convert.ToDecimal(reader["FineFees"]) : 0;
                        string createdBy = reader["CreatedBy"] != DBNull.Value ? reader["CreatedBy"].ToString() : "";
                        decimal applicationFees = reader["ApplicationFees"] != DBNull.Value ? Convert.ToDecimal(reader["ApplicationFees"]) : 0;

                        lblDetainID.Text = detainID.ToString();
                        lblDetainDate.Text = detainDate.ToShortDateString();
                        lblFineFees.Text = fineFees.ToString("N2");
                        lblCreatedBy.Text = createdBy;
                        lblApplicationFees.Text = applicationFees.ToString("N2");

                        decimal totalFees = fineFees + applicationFees;
                        lblTotalFees.Text = totalFees.ToString("N2");

                        btnRelease.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("This license is not currently reserved.", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnRelease.Enabled = false;
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching the data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }


        private void gbFilter_Click(object sender, EventArgs e)
        {



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

                            LoadDetainAndFeeInfo(_LicenseID);

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

        private void Detain_Date_Click(object sender, EventArgs e)
        {

        }

        private void llShowNewLicensesInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Show_Lic frm = new Show_Lic(_LicenseID);
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

        private void Release_Detained_License_Load(object sender, EventArgs e)
        {
            ResetFormInfo();
        }

        private void btnIssueReplacement_Click(object sender, EventArgs e)
        {


       
            if (MessageBox.Show("Are you sure about unblocking this license?؟", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;


            decimal applicationFees = Convert.ToDecimal(lblApplicationFees.Text);
            decimal fineFees = Convert.ToDecimal(lblFineFees.Text);
            decimal totalFees = applicationFees + fineFees;

            int applicationID = -1;
            int detainID = Convert.ToInt32(lblDetainID.Text);


            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    string insertAppQuery = @"INSERT INTO Applications 
                                    (ApplicantPersonID, ApplicationDate, ApplicationTypeID, ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID)
                                    VALUES 
                                    (@ApplicantPersonID, GETDATE(), 5, 3, GETDATE(), @PaidFees, @CreatedByUserID);
                                    SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmdApp = new SqlCommand(insertAppQuery, connection, transaction))
                    {

                        cmdApp.Parameters.AddWithValue("@ApplicantPersonID", _PersonID);
                        cmdApp.Parameters.AddWithValue("@PaidFees", applicationFees);
                        cmdApp.Parameters.AddWithValue("@CreatedByUserID", clsGlobal.CurrentUserID);

                        applicationID = Convert.ToInt32(cmdApp.ExecuteScalar());
                    }

                    string updateDetainQuery = @"UPDATE DetainedLicenses 
                                         SET IsReleased = 1, 
                                             ReleaseDate = GETDATE(), 
                                             ReleasedByUserID = @ReleasedByUserID, 
                                             ReleaseApplicationID = @ReleaseApplicationID
                                         WHERE DetainID = @DetainID;";

                    using (SqlCommand cmdDetain = new SqlCommand(updateDetainQuery, connection, transaction))
                    {
                        cmdDetain.Parameters.AddWithValue("@ReleasedByUserID",clsGlobal.CurrentUserID);
                        cmdDetain.Parameters.AddWithValue("@ReleaseApplicationID", applicationID);
                        cmdDetain.Parameters.AddWithValue("@DetainID", detainID);

                        cmdDetain.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    lblApplicationID.Text = applicationID.ToString();
                    btnRelease.Enabled = false;

                    MessageBox.Show($"The license hold has been successfully lifted! Request number: {applicationID}", "تمت العملية", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("An error occurred while unbooking:" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


    }
    
}
