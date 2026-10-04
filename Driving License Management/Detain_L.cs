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
    public partial class Detain_L : Form
    {
       
      

        int _LicenseID = -1;
        int _DriverID = -1;
        bool _IsActive = false;
        bool _IsDetained = false;
        DateTime _ExpirationDate;
        public Detain_L()
        {
            InitializeComponent();
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

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
            Detain_Date.Text = "[???]";
            lblCreatedBy.Text = "[???]";
            lblIssueDate.Text = "[???]";


            lblDetainID.Text = "[???]";
            lblLocalLicenseID.Text = "[???]";

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
                            Detain_Date.Text=DateTime.Now.ToString("dd/MMM/yyyy");
                            lblExpirationDate.Text = _ExpirationDate.ToString("dd/MMM/yyyy");
                            lblDateOfBirth.Text = Convert.ToDateTime(reader["DateOfBirth"]).ToString("dd/MMM/yyyy");
                            lblLocalLicenseID.Text = _LicenseID.ToString();
                            lblIsActive.Text = _IsActive ? "Yes" : "No";
                            lblIsDetained.Text = _IsDetained ? "Yes" : "No";
                            lblCreatedBy.Text = clsGlobal.CurrentUserName;
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

            if (IsLicenseDetained(_LicenseID))
            {
                MessageBox.Show("This license is already reserved!", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnDetain.Enabled = false;
                return;
            }
        }

        private void Detain_L_Load(object sender, EventArgs e)
        {
            ResetFormInfo();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private bool IsLicenseDetained(int licenseID)
        {
            bool isDetained = false;

            string query = "SELECT IsDetained = 1 FROM DetainedLicenses WHERE LicenseID = @LicenseID AND IsReleased = 0";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@LicenseID", licenseID);
                connection.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                    isDetained = Convert.ToBoolean(result);
            }

            return isDetained;
        }
        private void btnIssueReplacement_Click(object sender, EventArgs e)
        {
          
            if (string.IsNullOrEmpty(txtFineFees.Text) || !float.TryParse(txtFineFees.Text, out float fineFees))
            {
                MessageBox.Show("Please enter a valid fine amount.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to revoke this license?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            int selectedLicenseID = _LicenseID;

            int detainID = -1;


            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    string insertQuery = @"INSERT INTO DetainedLicenses 
                                  (LicenseID, DetainDate, FineFees, CreatedByUserID, IsReleased) 
                                  VALUES 
                                  (@LicenseID, GETDATE(), @FineFees, @CreatedByUserID, 0);
                                  SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@LicenseID", selectedLicenseID);
                        cmd.Parameters.AddWithValue("@FineFees", fineFees);
                        cmd.Parameters.AddWithValue("@CreatedByUserID", clsGlobal.CurrentUserID);

                        detainID = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    transaction.Commit();

                    lblDetainID.Text = detainID.ToString();
                    btnDetain.Enabled = false;
                    txtFineFees.Enabled = false;

                    MessageBox.Show($"The license has been successfully withdrawn! The booking number is: {detainID}", "Done successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("An error occurred while saving the data:" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
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

        private void lblLocalLicenseID_Click(object sender, EventArgs e)
        {

        }
    }
}
