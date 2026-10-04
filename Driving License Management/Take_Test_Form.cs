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
    public partial class Take_Test_Form : Form
    {

        private int _TestAppointmentID = -1;

        public Take_Test_Form()
        {
            InitializeComponent();
        }


        public Take_Test_Form(int testAppointmentID)
        {
            InitializeComponent();
            _TestAppointmentID = testAppointmentID;
        }
        private void _LoadAppointmentInfo()
        {
            string query = @"
        SELECT 
            TA.LocalDrivingLicenseApplicationID,
            LicenseClasses.ClassName,
            People.FirstName + ' ' + People.SecondName + ' ' + People.ThirdName + ' ' + People.LastName AS FullName,
            TA.AppointmentDate,
            TA.PaidFees,
            (SELECT COUNT(*) FROM Tests T 
             INNER JOIN TestAppointments TA2 ON T.TestAppointmentID = TA2.TestAppointmentID
             WHERE TA2.LocalDrivingLicenseApplicationID = TA.LocalDrivingLicenseApplicationID 
               AND TA2.TestTypeID = TA.TestTypeID) AS TrialCount
        FROM TestAppointments TA
        INNER JOIN LocalDrivingLicenseApplications LDLA ON TA.LocalDrivingLicenseApplicationID = LDLA.LocalDrivingLicenseApplicationID
        INNER JOIN Applications App ON LDLA.ApplicationID = App.ApplicationID
        INNER JOIN People ON App.ApplicantPersonID = People.PersonID
        INNER JOIN LicenseClasses ON LDLA.LicenseClassID = LicenseClasses.LicenseClassID
        WHERE TA.TestAppointmentID = @TestAppointmentID";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", _TestAppointmentID);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            lblLocalDLAppID.Text = reader["LocalDrivingLicenseApplicationID"].ToString();
                            lblLicenseClass.Text = reader["ClassName"].ToString();
                            lblFullName.Text = reader["FullName"].ToString();
                            lblTrial.Text = reader["TrialCount"].ToString();
                            lblDate.Text = Convert.ToDateTime(reader["AppointmentDate"]).ToString("dd/MMM/yyyy");
                            lblFees.Text = Convert.ToDecimal(reader["PaidFees"]).ToString("0.##");
                            lblLocalDLAppID.Text = "Not Taken Yet";
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            {
                if (MessageBox.Show("Are you sure you want to save this test result? You cannot change it later.",
                                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }

                bool testResult = rbPass.Checked;
                string notes = txtNotes.Text.Trim();

                using (SqlConnection connection = new SqlConnection(DB.connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        string insertTestQuery = @"INSERT INTO Tests (TestAppointmentID, TestResult, Notes, CreatedByUserID)
                                       VALUES (@TestAppointmentID, @TestResult, @Notes, @CreatedByUserID);
                                       SELECT SCOPE_IDENTITY();";

                        SqlCommand cmdTest = new SqlCommand(insertTestQuery, connection, transaction);
                        cmdTest.Parameters.AddWithValue("@TestAppointmentID", _TestAppointmentID);
                        cmdTest.Parameters.AddWithValue("@TestResult", testResult);
                        cmdTest.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(notes) ? (object)DBNull.Value : notes);
                        cmdTest.Parameters.AddWithValue("@CreatedByUserID", 1);

                        int testID = Convert.ToInt32(cmdTest.ExecuteScalar());

                        string updateAppointmentQuery = @"UPDATE TestAppointments 
                                             SET IsLocked = 1 
                                             WHERE TestAppointmentID = @TestAppointmentID";

                        SqlCommand cmdAppointment = new SqlCommand(updateAppointmentQuery, connection, transaction);
                        cmdAppointment.Parameters.AddWithValue("@TestAppointmentID", _TestAppointmentID);
                        cmdAppointment.ExecuteNonQuery();


                        transaction.Commit();

                        lblLocalDLAppID.Text = testID.ToString();
                        MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        btnSave.Enabled = false;

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Take_Test_Form_Load(object sender, EventArgs e)
        {
            _LoadAppointmentInfo();
        }

        private void Take_Test_Form_Load_1(object sender, EventArgs e)
        {
            _LoadAppointmentInfo();
        }
    }
}
