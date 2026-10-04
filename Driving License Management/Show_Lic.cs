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
using DB_Layer;

namespace Driving_License_Management
{
    public partial class Show_Lic : Form
    {
        private int _LicenseID = -1;
        public Show_Lic()
        {
            InitializeComponent();
        }
        public Show_Lic(int licenseID)
        {
            InitializeComponent();
            _LicenseID = licenseID;
        }


        private bool IsLicenseDetained(int licenseID)
        {
            bool isDetained = false;
            string query = "SELECT IsReleased FROM DetainedLicenses WHERE LicenseID = @LicenseID AND IsReleased = 0";

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LicenseID", licenseID);
                conn.Open();

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    isDetained = true; 
                }
            }

            return isDetained;
        }

        private void _LoadLicenseData()
        {
            string query = @"
            SELECT 
                Licenses.LicenseID,
                LicenseClasses.ClassName,
                People.FirstName + ' ' + People.SecondName + ' ' + People.ThirdName + ' ' + People.LastName AS FullName,
                People.NationalNo,
                People.Gendor,
                Licenses.IssueDate,
                Licenses.IssueReason,
                Licenses.Notes,
                Licenses.IsActive,
                People.DateOfBirth,
                Licenses.DriverID,
                Licenses.ExpirationDate,
                People.ImagePath
            FROM Licenses
            INNER JOIN LicenseClasses ON Licenses.LicenseClass = LicenseClasses.LicenseClassID
            INNER JOIN Drivers ON Licenses.DriverID = Drivers.DriverID
            INNER JOIN People ON Drivers.PersonID = People.PersonID
            WHERE Licenses.LicenseID = @LicenseID";

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LicenseID", _LicenseID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblClass.Text = reader["ClassName"].ToString();
                    lblName.Text = reader["FullName"].ToString();
                    lblicense.Text = reader["LicenseID"].ToString();
                    lblNationalNo.Text = reader["NationalNo"].ToString();
                    lblGendor.Text = Convert.ToByte(reader["Gendor"]) == 0 ? "Male" : "Female";
                    lblIssueDate.Text = Convert.ToDateTime(reader["IssueDate"]).ToString("dd/MMM/yyyy");

                    byte issueReason = Convert.ToByte(reader["IssueReason"]);
                    lblIssueReason.Text = issueReason == 1 ? "First Time" :
                                         issueReason == 2 ? "Renew" :
                                         issueReason == 3 ? "Replacement for Damaged" : "Replacement for Lost";

                    lblNotes.Text = string.IsNullOrEmpty(reader["Notes"].ToString()) ? "No Notes" : reader["Notes"].ToString();
                    lblIsActive.Text = Convert.ToBoolean(reader["IsActive"]) ? "Yes" : "No";
                    lblDateOfBirth.Text = Convert.ToDateTime(reader["DateOfBirth"]).ToString("dd/MMM/yyyy");
                    lblDriverID.Text = reader["DriverID"].ToString();
                    lblExpirationDate.Text = Convert.ToDateTime(reader["ExpirationDate"]).ToString("dd/MMM/yyyy");
                    lDetained.Text = IsLicenseDetained(_LicenseID) ? "Yes" : "No";

                    string imagePath = reader["ImagePath"].ToString();
                    if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                    {
                        pbPersonImage.ImageLocation = imagePath;
                    }
                }
            }
        }





        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Show_Lic_Load(object sender, EventArgs e)
        {
            _LoadLicenseData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
