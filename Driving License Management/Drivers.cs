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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Driving_License_Management
{
    public partial class Drivers : Form
    {
        public Drivers()
        {
            InitializeComponent();
        }

        private void _RefreshDriversList()
        {
            DataTable dtDrivers = new DataTable();

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                string query = @"
            SELECT 
                Drivers.DriverID,
                Drivers.PersonID,
                People.NationalNo,
                (People.FirstName + ' ' + People.SecondName + ' ' + ISNULL(People.ThirdName, '') + ' ' + People.LastName) AS FullName,
                Drivers.CreatedDate AS Date,
                COUNT(Licenses.LicenseID) AS ActiveLicenses
            FROM Drivers 
            INNER JOIN People ON Drivers.PersonID = People.PersonID
            LEFT JOIN Licenses ON Drivers.DriverID = Licenses.DriverID AND Licenses.IsActive = 1
            GROUP BY 
                Drivers.DriverID, 
                Drivers.PersonID, 
                People.NationalNo, 
                People.FirstName, 
                People.SecondName, 
                People.ThirdName, 
                People.LastName, 
                Drivers.CreatedDate
            ORDER BY Drivers.DriverID;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dtDrivers);
                    }
                }
            }

            dgvDrivers.DataSource = dtDrivers;
            dgvDrivers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            lblRecordsCount.Text = dgvDrivers.Rows.Count.ToString();

            if (dgvDrivers.Rows.Count > 0)
            {
                dgvDrivers.Columns[0].HeaderText = "Driver ID";
                dgvDrivers.Columns[1].HeaderText = "Person ID";
                dgvDrivers.Columns[2].HeaderText = "National No.";
                dgvDrivers.Columns[3].HeaderText = "Full Name";
                dgvDrivers.Columns[4].HeaderText = "Date";
                dgvDrivers.Columns[5].HeaderText = "Active Licenses";
            }
        }

        private void Drivers_Load(object sender, EventArgs e)
        {
            _RefreshDriversList();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

         
            string filterColumn = comboBox1.Text;
            string filterValue = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(filterValue) || filterColumn == "None")
            {
                (dgvDrivers.DataSource as DataTable).DefaultView.RowFilter = "";
                lblRecordsCount.Text = dgvDrivers.Rows.Count.ToString();
                return;
            }

            if (filterColumn == "Driver ID" || filterColumn == "Person ID")
            {
                if (int.TryParse(filterValue, out int parsedValue))
                {
                    (dgvDrivers.DataSource as DataTable).DefaultView.RowFilter = string.Format("[{0}] = {1}", filterColumn.Replace(" ", ""), parsedValue);
                }
                else
                {
                    (dgvDrivers.DataSource as DataTable).DefaultView.RowFilter = "1 = 0";
                }
            }
            else 
            {
                (dgvDrivers.DataSource as DataTable).DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", filterColumn.Replace(" ", ""), filterValue);
            }

            lblRecordsCount.Text = dgvDrivers.Rows.Count.ToString();
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
         
            if (comboBox1.Text == "None")
            {
                textBox1.Visible = false;

                if (dgvDrivers.DataSource != null)
                {
                    ((DataTable)dgvDrivers.DataSource).DefaultView.RowFilter = "";
                }
            }
            else
            {
                textBox1.Visible = true;
                textBox1.Focus();
            }

            lblRecordsCount.Text = dgvDrivers.Rows.Count.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
    
}
