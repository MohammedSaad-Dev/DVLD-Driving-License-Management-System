using DB_Layer;
using Driving_License_Management.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Driving_License_Management
{
    public partial class International_Lic : Form
    {
        int _LicenseID = -1;
        int _DriverID = -1;
        int _PersonID = -1;
        bool _IsActive = false;
        bool _IsDetained = false;
        DateTime _ExpirationDate;
        int _InternationalLicenseID = -1;
        public International_Lic()
        {
            InitializeComponent();
        }

        private int GetActiveInternationalLicenseID(int driverID)
        {
            int activeInternationalLicenseID = -1;
            string query = "SELECT InternationalLicenseID FROM InternationalLicenses WHERE DriverID = @DriverID AND IsActive = 1";

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DriverID", driverID);
                   
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int licenseID))
                        {
                            activeInternationalLicenseID = licenseID;
                        }
                    
                    
                }
            }
            return activeInternationalLicenseID;
        }

        private decimal GetInternationalLicenseFees()
        {
            decimal fees = 0;
            string query = "SELECT ApplicationFees FROM ApplicationTypes WHERE ApplicationTypeID = 6";

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                   
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null && decimal.TryParse(result.ToString(), out decimal fetchedFees))
                        {
                            fees = fetchedFees;
                        }
                   
                }
            }
            return fees;
        }


        private void LoadApplicationInfo()
        {
           
          


            lblApplicationDate.Text = DateTime.Now.ToString("dd/MMM/yyyy");
            lblIssueDate222222222.Text = DateTime.Now.ToString("dd/MMM/yyyy");
            lblExpirationDateApp.Text = DateTime.Now.AddYears(1).ToString("dd/MMM/yyyy"); 
            lblLocalLicenseID.Text = _LicenseID.ToString();

            lblFees.Text = GetInternationalLicenseFees().ToString("0.00");

            lblCreatedBy.Text = clsGlobal.CurrentUserName;
            lblLocalLicenseID.Text = "[???]";
            lblILApplicationID.Text = "[???]";

            btnIssue.Enabled = true;
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
            lblILApplicationID.Text = "[???]";
            lblApplicationDate.Text = "[???]";
            lblFees.Text = "[???]";
            lblILLicenseID.Text = "[???]";
            lblExpirationDateApp.Text = "[???]";
            lblCreatedBy.Text = "[???]";
            lblIssueDate.Text = "[???]";
        }

        private void button1_Click(object sender, EventArgs e)
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

        private void txtLicenseID_TextChanged(object sender, EventArgs e)
        {

        }

        private void International_Lic_Load(object sender, EventArgs e)
        {
            ResetFormInfo();
            LoadApplicationInfo();
            llShowLicenseInfo.Enabled=false;

        }

        private void label38_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnIssue_Click(object sender, EventArgs e)
        {
          
            int ActiveLicenseID = GetActiveInternationalLicenseID(_DriverID);

            if (ActiveLicenseID != -1)
            {
                MessageBox.Show($"Person already has an active international license with ID = {ActiveLicenseID}",
                                "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);

                _InternationalLicenseID = ActiveLicenseID;
                lblILLicenseID.Text = _InternationalLicenseID.ToString();

                btnIssue.Enabled = false;
                llShowLicenseInfo.Enabled = true;
                return;
            }

            if (MessageBox.Show("Are you sure you want to issue the license?", "Confirm",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
               
            }

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    string insertAppQuery = @"
                INSERT INTO Applications 
                (ApplicantPersonID, ApplicationDate, ApplicationTypeID, ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID)
                VALUES (@PersonID, @AppDate, 6, 3, @StatusDate, @Fees, @CreatedBy);
                SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmdApp = new SqlCommand(insertAppQuery, conn, transaction))
                    {
                        cmdApp.Parameters.AddWithValue("@PersonID", _PersonID);
                        cmdApp.Parameters.AddWithValue("@AppDate", DateTime.Now);
                        cmdApp.Parameters.AddWithValue("@StatusDate", DateTime.Now);
                        cmdApp.Parameters.AddWithValue("@Fees", 50.00);
                        cmdApp.Parameters.AddWithValue("@CreatedBy", clsGlobal.CurrentUserID);

                        int newAppID = Convert.ToInt32(cmdApp.ExecuteScalar());

                        string insertILQuery = @"
                    INSERT INTO InternationalLicenses
                    (ApplicationID, DriverID, IssuedUsingLocalLicenseID, IssueDate, ExpirationDate, IsActive, CreatedByUserID)
                    VALUES (@AppID, @DriverID, @LocalLicenseID, @IssueDate, @ExpDate, 1, @CreatedBy);
                    SELECT SCOPE_IDENTITY();";

                        using (SqlCommand cmdIL = new SqlCommand(insertILQuery, conn, transaction))
                        {
                            cmdIL.Parameters.AddWithValue("@AppID", newAppID);
                            cmdIL.Parameters.AddWithValue("@DriverID", _DriverID);
                            cmdIL.Parameters.AddWithValue("@LocalLicenseID", _LicenseID);
                            cmdIL.Parameters.AddWithValue("@IssueDate", DateTime.Now);
                            cmdIL.Parameters.AddWithValue("@ExpDate", DateTime.Now.AddYears(1));
                            cmdIL.Parameters.AddWithValue("@CreatedBy", clsGlobal.CurrentUserID);

                            int newILID = Convert.ToInt32(cmdIL.ExecuteScalar());

                            transaction.Commit();

                            _InternationalLicenseID = newILID;

                            lblILApplicationID.Text = newAppID.ToString();
                            lblILLicenseID.Text = _InternationalLicenseID.ToString();

                            btnIssue.Enabled = false;
                            llShowLicenseInfo.Enabled = true; 

                            MessageBox.Show($"International License Issued Successfully with ID = {_InternationalLicenseID}",
                                            "License Issued", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            llShowLicenseInfo.Enabled = true;

                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error issuing international license: " + ex.Message, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {



            License_History frm = new License_History(_DriverID);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {


            Show_Lice_Internatioal frm = new Show_Lice_Internatioal(_InternationalLicenseID);
            frm.StartPosition = FormStartPosition.CenterScreen;

            frm.ShowDialog();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }
    }
    
}
