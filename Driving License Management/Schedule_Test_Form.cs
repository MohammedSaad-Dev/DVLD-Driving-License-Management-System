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
using DB_Layer;

namespace Driving_License_Management
{
    public partial class Schedule_Test_Form : Form
    {
        private int _LocalDLAppID = -1;
        private int _TestTypeID = 1; // 1 = Vision, 2 = Written, 3 = Street
        private int _TestAppointmentID = -1;
        private float _TestTypeFees = 0;
        public Schedule_Test_Form()
        {
            InitializeComponent();
        }
        public Schedule_Test_Form(int localDLAppID, int testTypeID = 1, int appointmentID = -1)
        {
            InitializeComponent();
            _LocalDLAppID = localDLAppID;
            _TestTypeID = testTypeID;
            _TestAppointmentID = appointmentID;
        }



        private void _LoadData()
        {

            if (_TestAppointmentID != -1)
            {
                string editQuery = "SELECT AppointmentDate FROM TestAppointments WHERE TestAppointmentID = @TestAppointmentID";

                using (SqlConnection conn = new SqlConnection(DB.connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(editQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@TestAppointmentID", _TestAppointmentID);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DateTime appointmentDate = Convert.ToDateTime(reader["AppointmentDate"]);

                                if (appointmentDate < dateTimePicker1.MinDate)
                                {
                                    dateTimePicker1.MinDate = appointmentDate;
                                }

                                dateTimePicker1.Value = appointmentDate;


                            }
                        }
                    }
                }
            }

            // 2. جلب بيانات الطلب والشخص
            string query = @"
        SELECT 
            LDLA.LocalDrivingLicenseApplicationID,
            LC.ClassName,
            P.FirstName + ' ' + P.SecondName + ' ' + P.ThirdName + ' ' + P.LastName AS FullName,
            TT.TestTypeFees,
            (SELECT COUNT(1) FROM TestAppointments TA 
             WHERE TA.LocalDrivingLicenseApplicationID = LDLA.LocalDrivingLicenseApplicationID 
               AND TA.TestTypeID = @TestTypeID) AS Trials
        FROM LocalDrivingLicenseApplications LDLA
        INNER JOIN LicenseClasses LC ON LDLA.LicenseClassID = LC.LicenseClassID
        INNER JOIN Applications A ON LDLA.ApplicationID = A.ApplicationID
        INNER JOIN People P ON A.ApplicantPersonID = P.PersonID
        CROSS JOIN TestTypes TT
        WHERE LDLA.LocalDrivingLicenseApplicationID = @LocalDLAppID 
          AND TT.TestTypeID = @TestTypeID";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDLAppID", _LocalDLAppID);
                    command.Parameters.AddWithValue("@TestTypeID", _TestTypeID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblLocalDLAppID.Text = reader["LocalDrivingLicenseApplicationID"].ToString();
                                lblClassName.Text = reader["ClassName"].ToString();
                                lblFullName.Text = reader["FullName"].ToString();

                                int trials = Convert.ToInt32(reader["Trials"]);
                                lblTrial.Text = trials.ToString();

                                _TestTypeFees = Convert.ToSingle(reader["TestTypeFees"]);
                                lblFees.Text = _TestTypeFees.ToString("0.##");

                                // التحكم بإعدادات إعادة الاختبار (Retake Test)
                                if (trials > 0)
                                {
                                    gbRetakeTestInfo.Enabled = true;
                                    lblRetakeAppFees.Text = "5";
                                    lblTotalFees.Text = (_TestTypeFees + 5).ToString("0.##");
                                    lblRetakeTestAppID.Text = "N/A"; // سيتولى زر الحفظ (Save) وضع الـ ID المولد عند الإنشاء
                                }
                                else
                                {
                                    gbRetakeTestInfo.Enabled = false;
                                    lblRetakeAppFees.Text = "0";
                                    lblTotalFees.Text = _TestTypeFees.ToString("0.##");
                                    lblRetakeTestAppID.Text = "N/A";
                                }
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




        private void Schedule_Test_Form_Load(object sender, EventArgs e)
        {
            dateTimePicker1.MinDate = DateTime.Now.Date; ;
            dateTimePicker1.Value = DateTime.Now.Date; ;
            _LoadData();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (dateTimePicker1.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Cannot set appointment date in the past!", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_TestAppointmentID == -1)
            {
                string checkQuery = @"SELECT 1 FROM TestAppointments 
                              WHERE LocalDrivingLicenseApplicationID = @LocalDLAppID 
                              AND TestTypeID = @TestTypeID 
                              AND IsLocked = 0";

                using (SqlConnection connection = new SqlConnection(DB.connectionString))
                {
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@LocalDLAppID", _LocalDLAppID);
                        checkCmd.Parameters.AddWithValue("@TestTypeID", _TestTypeID);

                        try
                        {
                            connection.Open();
                            object activeAppointment = checkCmd.ExecuteScalar();

                            if (activeAppointment != null)
                            {
                                MessageBox.Show("Person already has an active appointment for this test, You cannot add new appointment.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }

            string query = "";
            if (_TestAppointmentID == -1)
            {
                query = @"INSERT INTO TestAppointments 
                  (TestTypeID, LocalDrivingLicenseApplicationID, AppointmentDate, PaidFees, CreatedByUserID, IsLocked)
                  VALUES 
                  (@TestTypeID, @LocalDLAppID, @AppointmentDate, @PaidFees, @CreatedByUserID, 0);
                  SELECT SCOPE_IDENTITY();";
            }
            else
            {
                query = @"UPDATE TestAppointments 
                  SET AppointmentDate = @AppointmentDate 
                  WHERE TestAppointmentID = @TestAppointmentID";
            }

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@AppointmentDate", dateTimePicker1.Value);

                    if (_TestAppointmentID == -1)
                    {
                        cmd.Parameters.AddWithValue("@TestTypeID", _TestTypeID);
                        cmd.Parameters.AddWithValue("@LocalDLAppID", _LocalDLAppID);
                        cmd.Parameters.AddWithValue("@PaidFees", Convert.ToSingle(lblTotalFees.Text));
                        cmd.Parameters.AddWithValue("@CreatedByUserID", 1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@TestAppointmentID", _TestAppointmentID);
                    }

                    try
                    {
                        connection.Open();

                        if (_TestAppointmentID == -1)
                        {
                            object result = cmd.ExecuteScalar();
                            if (result != null)
                            {
                                _TestAppointmentID = Convert.ToInt32(result);
                                lblRetakeTestAppID.Text = _TestAppointmentID.ToString();
                            }
                        }
                        else
                        {
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}
