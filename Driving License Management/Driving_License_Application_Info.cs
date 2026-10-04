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
    public partial class Driving_License_Application_Info : UserControl
    {

        private int _LocalDrivingLicenseApplicationID = -1;

        public int LocalDrivingLicenseApplicationID => _LocalDrivingLicenseApplicationID;
        public Driving_License_Application_Info()
        {
            InitializeComponent();
        }


            public void LoadApplicationInfo(int localDLAppID)
            {
                _LocalDrivingLicenseApplicationID = localDLAppID;

                string query = @"
            SELECT 
                LDLA.LocalDrivingLicenseApplicationID,
                LC.ClassName,
                PassedTests = (
                    SELECT COUNT(1) 
                    FROM TestAppointments TA 
                    INNER JOIN Tests T ON TA.TestAppointmentID = T.TestAppointmentID
                    WHERE TA.LocalDrivingLicenseApplicationID = LDLA.LocalDrivingLicenseApplicationID 
                      AND T.TestResult = 1
                )
            FROM LocalDrivingLicenseApplications LDLA
            INNER JOIN LicenseClasses LC ON LDLA.LicenseClassID = LC.LicenseClassID
            WHERE LDLA.LocalDrivingLicenseApplicationID = @LocalDLAppID";

                using (SqlConnection connection = new SqlConnection(DB.connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@LocalDLAppID", localDLAppID);

                        try
                        {
                            connection.Open();
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    lblDLAppID.Text = reader["LocalDrivingLicenseApplicationID"].ToString();
                                    lblAppliedForLicense.Text = reader["ClassName"].ToString();
                                    lblPassedTests.Text = reader["PassedTests"].ToString() + "/3";
                                }
                                else
                                {
                                    MessageBox.Show("No Application found with ID = " + localDLAppID, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        

        private void Driving_License_Application_Info_Load(object sender, EventArgs e)
        {

        }

        private void lblAppliedForLicense_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
