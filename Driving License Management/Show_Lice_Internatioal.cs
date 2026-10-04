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
using System.Windows.Forms.DataVisualization.Charting;
using DB_Layer;


namespace Driving_License_Management
{
    public partial class Show_Lice_Internatioal : Form
    {
        private int _InternationalLicenseID = -1;
        public Show_Lice_Internatioal()
        {
            InitializeComponent();
        }
        public Show_Lice_Internatioal(int InternationalLicenseID)
        {
            InitializeComponent();
            _InternationalLicenseID = InternationalLicenseID; 
        }

        public void LoadInternationalLicenseInfo()
        {
            string query = @"
        SELECT 
            People.FirstName + ' ' + People.SecondName + ' ' + People.ThirdName + ' ' + People.LastName AS FullName,
            InternationalLicenses.InternationalLicenseID,
            InternationalLicenses.ApplicationID,
            InternationalLicenses.IssuedUsingLocalLicenseID AS LicenseID,
            InternationalLicenses.IsActive,
            People.NationalNo,
            People.DateOfBirth,
            People.Gendor,
            InternationalLicenses.DriverID,
            InternationalLicenses.IssueDate,
            InternationalLicenses.ExpirationDate,
            People.ImagePath
        FROM InternationalLicenses
        INNER JOIN Applications ON InternationalLicenses.ApplicationID = Applications.ApplicationID
        INNER JOIN People ON Applications.ApplicantPersonID = People.PersonID
        WHERE InternationalLicenses.InternationalLicenseID = @InternationalLicenseID;";

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@InternationalLicenseID", _InternationalLicenseID);

                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            lblName.Text = reader["FullName"].ToString();
                            lblIntLicenseID.Text = reader["InternationalLicenseID"].ToString();
                            lblApplicationID.Text = reader["ApplicationID"].ToString();
                            lblicense.Text = reader["LicenseID"].ToString();
                            lblIsActive.Text = Convert.ToBoolean(reader["IsActive"]) ? "Yes" : "No";
                            lblNationalNo.Text = reader["NationalNo"].ToString();
                            lblDateOfBirth.Text = Convert.ToDateTime(reader["DateOfBirth"]).ToString("dd/MMM/yyyy");
                            lblGendor.Text = Convert.ToInt32(reader["Gendor"]) == 0 ? "Male" : "Female";
                            lblDriverID.Text = reader["DriverID"].ToString();
                            lblIssueDate.Text = Convert.ToDateTime(reader["IssueDate"]).ToString("dd/MMM/yyyy");
                            lblExpirationDate.Text = Convert.ToDateTime(reader["ExpirationDate"]).ToString("dd/MMM/yyyy");

                            string imagePath = reader["ImagePath"].ToString();
                            if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                            {
                                pbPersonImage.ImageLocation = imagePath;
                            }
                            
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Show_Lice_Internatioal_Load(object sender, EventArgs e)
        {
            LoadInternationalLicenseInfo();
        }
    }
}
