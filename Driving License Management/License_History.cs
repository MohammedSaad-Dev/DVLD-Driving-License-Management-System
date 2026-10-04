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
    public partial class License_History : Form
    {
        private int _DriverID = -1;
        private int _NewLicenseID = -1;

        public License_History()
        {
            InitializeComponent();
        }

        public License_History(int driverID)
        {
            InitializeComponent();
            _DriverID = driverID;
        }

        public void LoadLocalLicensesHistory(int driverID)
        {
            string query = @"
        SELECT 
            Licenses.LicenseID AS [Lic.ID],
            Licenses.ApplicationID AS [App.ID],
            LicenseClasses.ClassName AS [Class Name],
            Licenses.IssueDate AS [Issue Date],
            Licenses.ExpirationDate AS [Expiration Date],
            Licenses.IsActive AS [Is Active]
        FROM Licenses
        INNER JOIN LicenseClasses ON Licenses.LicenseClass = LicenseClasses.LicenseClassID
        WHERE Licenses.DriverID = @DriverID
        ORDER BY Licenses.LicenseID DESC;";

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DriverID", driverID);

                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dt);

                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                lblRecordsCount.Text = dt.Rows.Count.ToString();
            }
        }

        public void LoadInternationalLicensesHistory(int driverID)
        {
            string query = @"
        SELECT 
            InternationalLicenseID AS [Int.License ID],
            ApplicationID AS [App.ID],
            IssuedUsingLocalLicenseID AS [L.License ID],
            IssueDate AS [Issue Date],
            ExpirationDate AS [Expiration Date],
            IsActive AS [Is Active]
        FROM InternationalLicenses
        WHERE DriverID = @DriverID
        ORDER BY InternationalLicenseID DESC;";

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DriverID", driverID);

                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);

                dataGridView2.DataSource = dt;
                dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                lblRecordsCountInt.Text = dt.Rows.Count.ToString();
            }
        }
        private void License_History_Load(object sender, EventArgs e)
        {
            
            groupBox1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);

            tabControl1.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Regular);

            LoadLocalLicensesHistory(_DriverID);
            LoadInternationalLicensesHistory(_DriverID);
        }

        private void showLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
        
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Lic.ID"].Value);

                Show_Lic F1 = new Show_Lic(id);
                F1.ShowDialog();
            }

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

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }
    }
}
