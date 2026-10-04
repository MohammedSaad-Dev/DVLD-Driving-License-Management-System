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
    public partial class Loacl_DLA : Form
    {

        private DataTable _dtLocalDrivingLicenseApplications;
        public Loacl_DLA()
        {
            InitializeComponent();
        }

        private void _RefreshApplicationsList()
        {
            string query = "SELECT * FROM LocalDrivingLicenseApplications_View";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        _dtLocalDrivingLicenseApplications = new DataTable();
                        adapter.Fill(_dtLocalDrivingLicenseApplications);

                        dataGridView1.DataSource = _dtLocalDrivingLicenseApplications;
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        label3.Text = dataGridView1.Rows.Count.ToString();
                    }
                }
            }


            if (dataGridView1.Columns.Contains("LocalDrivingLicenseApplicationID"))
                dataGridView1.Columns["LocalDrivingLicenseApplicationID"].HeaderText = "L.D.L.AppID";

            if (dataGridView1.Columns.Contains("NationalNo"))
                dataGridView1.Columns["NationalNo"].HeaderText = "National No";

            if (dataGridView1.Columns.Contains("FullName"))
                dataGridView1.Columns["FullName"].HeaderText = "Full Name";

            if (dataGridView1.Columns.Contains("ApplicationDate"))
                dataGridView1.Columns["ApplicationDate"].HeaderText = " Application Date";

            if (dataGridView1.Columns.Contains("PassedTestCount"))
                dataGridView1.Columns["PassedTestCount"].HeaderText = "Passed Test";

            if (dataGridView1.Columns.Contains("ClassName"))
                dataGridView1.Columns["ClassName"].HeaderText = "Driving Name";

        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

            New_Local_DLA f2 = new New_Local_DLA();

            f2.StartPosition = FormStartPosition.CenterScreen;

            f2.DataBack += Frm_DataBack;

            f2.ShowDialog();


        }
        private void Frm_DataBack(object sender)
        {
            _RefreshApplicationsList();
        }

        private void Loacl_DLA_Load(object sender, EventArgs e)
        {
            _RefreshApplicationsList();
            _FillFilterComboBox();
        }

        private void _FillFilterComboBox()
        {
            comboBox1.Items.Clear();

            comboBox1.Items.Add("None");
            comboBox1.Items.Add("L.D.L.AppID");
            comboBox1.Items.Add("National No.");
            comboBox1.Items.Add("Full Name");
            comboBox1.Items.Add("Status");

            comboBox1.SelectedIndex = 0; 
        }
       
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


            textBox1.Visible = (comboBox1.Text != "None");

            if (textBox1.Visible)
            {
                textBox1.Text = "";
                textBox1.Focus();
            }
            else
            {
                if (_dtLocalDrivingLicenseApplications != null)
                    _dtLocalDrivingLicenseApplications.DefaultView.RowFilter = "";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {


            string filterColumn = "";

            switch (comboBox1.Text)
            {
                case "L.D.L.AppID":
                    filterColumn = "LocalDrivingLicenseApplicationID";
                    break;

                case "National No.":
                    filterColumn = "NationalNo";
                    break;

                case "Full Name":
                    filterColumn = "FullName";
                    break;

                case "Status":
                    filterColumn = "Status";
                    break;

                default:
                    filterColumn = "None";
                    break;
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text) || filterColumn == "None")
            {
                _dtLocalDrivingLicenseApplications.DefaultView.RowFilter = "";

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                label3.Text = dataGridView1.Rows.Count.ToString();
                return;
            }

            if (filterColumn == "LocalDrivingLicenseApplicationID")
            {
                if (int.TryParse(textBox1.Text.Trim(), out int id))
                    _dtLocalDrivingLicenseApplications.DefaultView.RowFilter = string.Format("[{0}] = {1}", filterColumn, id);
                else
                    _dtLocalDrivingLicenseApplications.DefaultView.RowFilter = string.Format("[{0}] = -1", filterColumn);
            }
            else
            {
                _dtLocalDrivingLicenseApplications.DefaultView.RowFilter = string.Format("[{0}] LIKE '%{1}%'", filterColumn, textBox1.Text.Trim());
            }

            label3.Text = dataGridView1.Rows.Count.ToString();
        }

        private void DataGV_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
      
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[e.RowIndex].Selected = true;

                dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex >= 0 ? e.ColumnIndex : 0];
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void sechduleTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cancelApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (dataGridView1.CurrentRow == null) return;


            int localDLAppID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value);


            if (MessageBox.Show("Are you sure you want to cancel this application?", "Confirm Cancel",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {


                string query = @"UPDATE Applications 
                        SET ApplicationStatus = 2, LastStatusDate = @LastStatusDate 
                        WHERE ApplicationID = (
                            SELECT ApplicationID 
                            FROM LocalDrivingLicenseApplications 
                            WHERE LocalDrivingLicenseApplicationID = @LocalDLAppID
                        )";

                using (SqlConnection connection = new SqlConnection(DB.connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@LocalDLAppID", localDLAppID);
                        command.Parameters.AddWithValue("@LastStatusDate", DateTime.Now);

                        try
                        {
                            connection.Open();
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Application Cancelled Successfully.", "Cancelled",
                                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                                _RefreshApplicationsList();
                            }
                            else
                            {
                                MessageBox.Show("Failed to cancel application.", "Error",
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message, "Database Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void deleteApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (dataGridView1.CurrentRow == null) return;

            int localDLAppID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value);

            if (MessageBox.Show("Are you sure you want to delete this application?", "Confirm Delete",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection connection = new SqlConnection(DB.connectionString))
                {
                    connection.Open();

                    int applicationID = -1;
                    string getAppIDQuery = "SELECT ApplicationID FROM LocalDrivingLicenseApplications WHERE LocalDrivingLicenseApplicationID = @LocalDLAppID";
                    using (SqlCommand cmdGet = new SqlCommand(getAppIDQuery, connection))
                    {
                        cmdGet.Parameters.AddWithValue("@LocalDLAppID", localDLAppID);
                        object res = cmdGet.ExecuteScalar();
                        if (res != null) applicationID = Convert.ToInt32(res);
                    }

                    if (applicationID == -1)
                    {
                        MessageBox.Show("Application not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        string deleteLocalQuery = "DELETE FROM LocalDrivingLicenseApplications WHERE LocalDrivingLicenseApplicationID = @LocalDLAppID";
                        using (SqlCommand cmdLocal = new SqlCommand(deleteLocalQuery, connection, transaction))
                        {
                            cmdLocal.Parameters.AddWithValue("@LocalDLAppID", localDLAppID);
                            cmdLocal.ExecuteNonQuery();
                        }

                        string deleteBaseQuery = "DELETE FROM Applications WHERE ApplicationID = @ApplicationID";
                        using (SqlCommand cmdBase = new SqlCommand(deleteBaseQuery, connection, transaction))
                        {
                            cmdBase.Parameters.AddWithValue("@ApplicationID", applicationID);
                            cmdBase.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        MessageBox.Show("Application Deleted Successfully.", "Deleted",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                        _RefreshApplicationsList();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Could not delete application. It might have linked records (like tests).\nDetails: " + ex.Message,
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void scheduleVisionTestToolStripMenuItem_Click(object sender, EventArgs e)
        {


            int localDLAppID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value);

            Driving_License_Application_Info_Form frm = new Driving_License_Application_Info_Form(localDLAppID);
            frm.StartPosition = FormStartPosition.CenterScreen;

            frm.ShowDialog();

            _RefreshApplicationsList();

        }

        private void scheduleWrittenTestToolStripMenuItem_Click(object sender, EventArgs e)
        {


            int localDLAppID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value);

            Written_Test_Appointments frm = new Written_Test_Appointments(localDLAppID);
            frm.StartPosition = FormStartPosition.CenterScreen;

            frm.ShowDialog();

            _RefreshApplicationsList();

            
        }

        private void sheduleStreetTestToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int localDLAppID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value);

            Street_Test_Appointments frm = new Street_Test_Appointments(localDLAppID);
            frm.StartPosition = FormStartPosition.CenterScreen;

            frm.ShowDialog();

            _RefreshApplicationsList();

        }

        private void issueDrivingLicenseFirstTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int localDLAppID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value);

            Lssus_D frm = new Lssus_D(localDLAppID);
            frm.StartPosition = FormStartPosition.CenterScreen;

            frm.ShowDialog();

            _RefreshApplicationsList();
        }

        private void showLiceseToolStripMenuItem_Click(object sender, EventArgs e)
        {
        
            int localDLAppID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value);
            int licenseID = -1;

            string query = @"
        SELECT Licenses.LicenseID 
        FROM Licenses 
        INNER JOIN LocalDrivingLicenseApplications LDLA ON Licenses.ApplicationID = LDLA.ApplicationID 
        WHERE LDLA.LocalDrivingLicenseApplicationID = @LDLAID";

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LDLAID", localDLAppID);
                conn.Open();

                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    licenseID = Convert.ToInt32(result);
                }
            }

            if (licenseID != -1)
            {
                Show_Lic frm = new Show_Lic(licenseID);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();

                _RefreshApplicationsList();
            }
            else
            {
                MessageBox.Show("No active license found for this application!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void showPersonLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {


            int localDLAppID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value);

            int driverID = -1;

            string query = @"
        SELECT Drivers.DriverID 
        FROM LocalDrivingLicenseApplications LDLA
        INNER JOIN Applications ON LDLA.ApplicationID = Applications.ApplicationID
        INNER JOIN Drivers ON Applications.ApplicantPersonID = Drivers.PersonID
        WHERE LDLA.LocalDrivingLicenseApplicationID = @LDLAID";

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LDLAID", localDLAppID);
                conn.Open();

                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    driverID = Convert.ToInt32(result);
                }
            }

            if (driverID != -1)
            {
                License_History frm = new License_History(driverID);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("This person is not registered as a driver yet!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }







        








        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

            if (dataGridView1.CurrentRow == null) return;

            int passedTestCount = Convert.ToInt32(dataGridView1.CurrentRow.Cells["PassedTestCount"].Value);
            string status = dataGridView1.CurrentRow.Cells["Status"].Value.ToString().Trim();

            if (status == "Completed")
            {
                editApplicationToolStripMenuItem.Enabled = false;
                deleteApplicationToolStripMenuItem.Enabled = false;
                cancelApplicationToolStripMenuItem.Enabled = false;
                scheduleTestsToolStripMenuItem.Enabled = false;
                issueDrivingLicenseToolStripMenuItem.Enabled = false;

                showLicenseToolStripMenuItem.Enabled = true; 
                return;
            }

            if (status == "Cancelled")
            {
                editApplicationToolStripMenuItem.Enabled = false;
                deleteApplicationToolStripMenuItem.Enabled = false;
                cancelApplicationToolStripMenuItem.Enabled = false;
                scheduleTestsToolStripMenuItem.Enabled = false;
                issueDrivingLicenseToolStripMenuItem.Enabled = false;
                showLicenseToolStripMenuItem.Enabled = false;
                return;
            }

            editApplicationToolStripMenuItem.Enabled = (passedTestCount == 0);
            cancelApplicationToolStripMenuItem.Enabled = true;
            deleteApplicationToolStripMenuItem.Enabled = true;

            scheduleTestsToolStripMenuItem.Enabled = (passedTestCount < 3);

            scheduleVisionTestToolStripMenuItem.Enabled = (passedTestCount == 0);
            scheduleWrittenTestToolStripMenuItem.Enabled = (passedTestCount == 1);
            scheduleStreetTestToolStripMenuItem.Enabled = (passedTestCount == 2);

            issueDrivingLicenseToolStripMenuItem.Enabled = (passedTestCount == 3);

            showLicenseToolStripMenuItem.Enabled = false;
        }
        

        private void editApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
    
    
    
    
    
}
