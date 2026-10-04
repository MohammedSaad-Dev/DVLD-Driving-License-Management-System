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
    public partial class List_Detained_Lice : Form
    {
        private DataTable _dtDetainedLicenses = new DataTable();
        public List_Detained_Lice()
        {
            InitializeComponent();
        }

        private void _RefreshDetainedLicensesList()
        {

            string query = @"
            SELECT 
                DL.DetainID,
                DL.LicenseID,
                DL.DetainDate,
                DL.IsReleased,
                DL.FineFees,
                DL.ReleaseDate,
                P.NationalNo AS [N.No.],
                (P.FirstName + ' ' + P.SecondName + ' ' + ISNULL(P.ThirdName, '') + ' ' + P.LastName) AS [Full Name],
                DL.ReleaseApplicationID AS [Rlease App.ID]
            FROM DetainedLicenses DL
            INNER JOIN Licenses L ON DL.LicenseID = L.LicenseID
            INNER JOIN Drivers D ON L.DriverID = D.DriverID
            INNER JOIN People P ON D.PersonID = P.PersonID
            ORDER BY DL.DetainID DESC;";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                try
                {
                    _dtDetainedLicenses.Clear();
                    adapter.Fill(_dtDetainedLicenses);
                    dgvDetainedLicenses.DataSource = _dtDetainedLicenses;

                    lblRecordsCount.Text = dgvDetainedLicenses.Rows.Count.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading the data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void List_Detained_Lice_Load(object sender, EventArgs e)
        {
            _RefreshDetainedLicensesList();
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string filterColumn = "";

            switch (cbFilterBy.Text)
            {
                case "Detain ID":
                    filterColumn = "DetainID";
                    break;
                case "National No.":
                    filterColumn = "[N.No.]";
                    break;
                case "Full Name":
                    filterColumn = "[Full Name]";
                    break;
                case "Release Application ID":
                    filterColumn = "[Rlease App.ID]";
                    break;
                default:
                    filterColumn = "None";
                    break;
            }

            if (txtFilterValue.Text.Trim() == "" || filterColumn == "None")
            {
                _dtDetainedLicenses.DefaultView.RowFilter = "";
                lblRecordsCount.Text = dgvDetainedLicenses.Rows.Count.ToString();
                return;
            }

            if (filterColumn == "DetainID" || filterColumn == "[Rlease App.ID]")
            {
                if (int.TryParse(txtFilterValue.Text.Trim(), out int id))
                {
                    _dtDetainedLicenses.DefaultView.RowFilter = string.Format("[{0}] = {1}", filterColumn.Replace("[", "").Replace("]", ""), id);
                }
                else
                {
                    _dtDetainedLicenses.DefaultView.RowFilter = "1=0";
                }
            }
            else 
            {
                _dtDetainedLicenses.DefaultView.RowFilter = string.Format("{0} LIKE '{1}%'", filterColumn, txtFilterValue.Text.Trim());
            }

            lblRecordsCount.Text = dgvDetainedLicenses.Rows.Count.ToString();
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isIsReleased = (cbFilterBy.Text == "Is Released");
            bool isNone = (cbFilterBy.Text == "None");

            txtFilterValue.Visible = !isIsReleased && !isNone;
            cbIsReleased.Visible = isIsReleased;

            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = "";
                txtFilterValue.Focus();
            }

            _dtDetainedLicenses.DefaultView.RowFilter = "";
            lblRecordsCount.Text = dgvDetainedLicenses.Rows.Count.ToString();
        }

        private void cbIsActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filterValue = cbIsReleased.Text;

            switch (filterValue)
            {
                case "All":
                    _dtDetainedLicenses.DefaultView.RowFilter = "";
                    break;
                case "Yes":
                    _dtDetainedLicenses.DefaultView.RowFilter = "IsReleased = true";
                    break;
                case "No":
                    _dtDetainedLicenses.DefaultView.RowFilter = "IsReleased = false";
                    break;
            }

            lblRecordsCount.Text = dgvDetainedLicenses.Rows.Count.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Detain_L f2 = new Detain_L();

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Release_Detained_License f2 = new Release_Detained_License();

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();
        }
    }
}
