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
    public partial class Lssus_D : Form
    {
        private int _LocalDLAppID = -1;
        private int _LicenseClassID = -1;
        private int _LicenseFees = -1;

        public Lssus_D()
        {
            InitializeComponent();
        }
        public Lssus_D(int localDLAppID)
        {
            InitializeComponent();
            _LocalDLAppID = localDLAppID;
        }



        private void Lssus_D_Load(object sender, EventArgs e)
        {
            driving_License_Application_Info1.LoadApplicationInfo(_LocalDLAppID);

            application_Basic_Info1.LoadApplicationBasicInfo(_LocalDLAppID);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    int personID = -1;
                    string getPersonQuery = @"
                SELECT A.ApplicantPersonID 
                FROM LocalDrivingLicenseApplications LDLA
                INNER JOIN Applications A ON LDLA.ApplicationID = A.ApplicationID
                WHERE LDLA.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

                    using (SqlCommand cmdGetPerson = new SqlCommand(getPersonQuery, conn, transaction))
                    {
                        cmdGetPerson.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", _LocalDLAppID);

                        object result = cmdGetPerson.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            personID = Convert.ToInt32(result);
                        }
                    }

                    if (personID == -1)
                    {
                        throw new Exception("Could not find PersonID associated with this application.");
                    }

                    int driverID = -1;
                    string checkDriverQuery = @"SELECT DriverID FROM Drivers WHERE PersonID = @PersonID";

                    using (SqlCommand cmdCheckDriver = new SqlCommand(checkDriverQuery, conn, transaction))
                    {
                        cmdCheckDriver.Parameters.AddWithValue("@PersonID", personID);
                        object driverResult = cmdCheckDriver.ExecuteScalar();

                        if (driverResult != null && driverResult != DBNull.Value)
                        {
                            driverID = Convert.ToInt32(driverResult);
                        }
                        else
                        {
                            string insertDriverQuery = @"INSERT INTO Drivers (PersonID, CreatedByUserID, CreatedDate) 
                                                 VALUES (@PersonID, @CreatedByUserID, GETDATE());
                                                 SELECT SCOPE_IDENTITY();";

                            using (SqlCommand cmdInsertDriver = new SqlCommand(insertDriverQuery, conn, transaction))
                            {
                                cmdInsertDriver.Parameters.AddWithValue("@PersonID", personID);
                                cmdInsertDriver.Parameters.AddWithValue("@CreatedByUserID", clsGlobal.CurrentUserID != -1 ? clsGlobal.CurrentUserID : 1);

                                driverID = Convert.ToInt32(cmdInsertDriver.ExecuteScalar());
                            }
                        }
                    }

                    string insertLicenseQuery = @"
                                       INSERT INTO Licenses 
                                       (
                                           ApplicationID, 
                                           DriverID, 
                                           LicenseClass, 
                                           IssueDate, 
                                           ExpirationDate, 
                                           Notes, 
                                           PaidFees, 
                                           IsActive, 
                                           IssueReason, 
                                           CreatedByUserID
                                       )
                                       SELECT 
                                           LDLA.ApplicationID,
                                           @DriverID,
                                           LDLA.LicenseClassID,
                                           GETDATE(),
                                           DATEADD(YEAR, 10, GETDATE()),
                                           @Notes,
                                           @PaidFees,
                                           1,
                                           1,
                                           @CreatedByUserID
                                       FROM LocalDrivingLicenseApplications LDLA
                                       WHERE LDLA.LocalDrivingLicenseApplicationID = @LDLAID;
                                      
                                       SELECT SCOPE_IDENTITY();";

                    int newLicenseID = 0;
                    using (SqlCommand cmdLicense = new SqlCommand(insertLicenseQuery, conn, transaction))
                    {
                        cmdLicense.Parameters.AddWithValue("@LDLAID", _LocalDLAppID);
                        cmdLicense.Parameters.AddWithValue("@DriverID", driverID);
                        cmdLicense.Parameters.AddWithValue("@LicenseClass", _LicenseClassID);
                        cmdLicense.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text);
                        cmdLicense.Parameters.AddWithValue("@PaidFees", _LicenseFees);
                        cmdLicense.Parameters.AddWithValue("@CreatedByUserID", clsGlobal.CurrentUserID != -1 ? clsGlobal.CurrentUserID : 1);

                        newLicenseID = Convert.ToInt32(cmdLicense.ExecuteScalar());
                    }

                    string updateAppQuery = @"UPDATE Applications 
                                      SET ApplicationStatus = 3, LastStatusDate = GETDATE() 
                                      WHERE ApplicationID = (SELECT ApplicationID FROM LocalDrivingLicenseApplications WHERE LocalDrivingLicenseApplicationID = @LDLAID)";

                    using (SqlCommand cmdUpdate = new SqlCommand(updateAppQuery, conn, transaction))
                    {
                        cmdUpdate.Parameters.AddWithValue("@LDLAID", _LocalDLAppID);
                        cmdUpdate.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    MessageBox.Show($"License Issued Successfully with ID = {newLicenseID}", "Succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    btnIssue.Enabled = false;
                    this.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error: " + ex.Message, "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void application_Basic_Info1_Load(object sender, EventArgs e)
        {

        }
    }
}
