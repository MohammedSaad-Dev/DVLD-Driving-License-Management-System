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
    public partial class Street_Test_Appointments : Form
    {
        private int _LocalDLAppID = -1;

        public Street_Test_Appointments()
        {
            InitializeComponent();
        }

        public Street_Test_Appointments(int localDLAppID)
        {
            InitializeComponent();
            _LocalDLAppID = localDLAppID;
        }

        private void _LoadTestAppointments(int localDLAppID, int testTypeID = 3)
        {
            string query = @"
        SELECT 
            TestAppointmentID as [Appointment ID],
            AppointmentDate as [Appointment Date],
            PaidFees as [Paid Fees],
            IsLocked as [Is Locked]
        FROM TestAppointments
        WHERE LocalDrivingLicenseApplicationID = @LocalDLAppID
          AND TestTypeID = @TestTypeID
        ORDER BY TestAppointmentID DESC";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDLAppID", localDLAppID);
                    command.Parameters.AddWithValue("@TestTypeID", testTypeID);

                    try
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dataGridView1.DataSource = dt;
                        label2.Text = dt.Rows.Count.ToString();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void Street_Test_Appointments_Load(object sender, EventArgs e)
        {

            driving_License_Application_Info1.LoadApplicationInfo(_LocalDLAppID);

            application_Basic_Info1.LoadApplicationBasicInfo(_LocalDLAppID);
            _LoadTestAppointments(_LocalDLAppID, 3);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        private void button2_Click(object sender, EventArgs e)
        {


            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();

                string checkPassQuery = @"SELECT TOP 1 Tests.TestResult 
                                             FROM Tests 
                                             INNER JOIN TestAppointments ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID
                                             WHERE TestAppointments.LocalDrivingLicenseApplicationID = @LocalDLAppID 
                                             AND TestAppointments.TestTypeID = @_TestTypeID 
                                             AND Tests.TestResult = 1";

                using (SqlCommand cmdPass = new SqlCommand(checkPassQuery, conn))
                {
                    cmdPass.Parameters.AddWithValue("@LocalDLAppID", _LocalDLAppID);
                    cmdPass.Parameters.AddWithValue("@_TestTypeID", 3);

                    object isPassed = cmdPass.ExecuteScalar();
                    if (isPassed != null)
                    {
                        MessageBox.Show("This person has already passed this test. You cannot add a new appointment!", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                string checkActiveQuery = @"SELECT TOP 1 1 FROM TestAppointments 
                                               WHERE LocalDrivingLicenseApplicationID = @LocalDLAppID 
                                               AND TestTypeID = @_TestTypeID 
                                               AND IsLocked = 0";

                using (SqlCommand cmdActive = new SqlCommand(checkActiveQuery, conn))
                {
                    cmdActive.Parameters.AddWithValue("@LocalDLAppID", _LocalDLAppID);
                    cmdActive.Parameters.AddWithValue("@_TestTypeID", 3);

                    object isActive = cmdActive.ExecuteScalar();
                    if (isActive != null)
                    {
                        MessageBox.Show("Person already has an active appointment for this test. You cannot add a new appointment.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            Schedule_Test_Form frm = new Schedule_Test_Form(_LocalDLAppID, 3);
            frm.StartPosition = FormStartPosition.CenterScreen;

            frm.ShowDialog();

            _LoadTestAppointments(_LocalDLAppID, 3);

        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[e.RowIndex].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isLocked = Convert.ToBoolean(dataGridView1.CurrentRow.Cells["Is Locked"].Value);

            if (isLocked)
            {
                MessageBox.Show("Person already sat for the test, appointment locked.", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }



            int appointmentID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Appointment ID"].Value);

            Schedule_Test_Form frm = new Schedule_Test_Form(_LocalDLAppID, 3, appointmentID);

            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
            _LoadTestAppointments(_LocalDLAppID, 3);
        }

        private void takeTestToolStripMenuItem_Click(object sender, EventArgs e)
        {

            bool isLocked = Convert.ToBoolean(dataGridView1.CurrentRow.Cells["Is Locked"].Value);

            if (isLocked)
            {
                MessageBox.Show("Person already sat for the test ", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int appointmentID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Appointment ID"].Value);

            Take_Test_Form frm = new Take_Test_Form(appointmentID);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();

            _LoadTestAppointments(_LocalDLAppID, 3);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
