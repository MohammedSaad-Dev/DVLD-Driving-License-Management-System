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
    public partial class Manage_Test_Types : Form
    {
        private DataView _dtPeopleView;

        public Manage_Test_Types()
        {
            InitializeComponent();
        }

        private void LoadData()
        {

            string query = "SELECT * FROM TestTypes ";


            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    _dtPeopleView = dataTable.DefaultView;

                    DataType.DataSource = _dtPeopleView;

                    DataType.DataSource = dataTable;
                    DataType.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


                    label3.Text = _dtPeopleView.Count.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching the data:" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }



                if (DataType.Columns.Contains("TestTypeID"))
                    DataType.Columns["TestTypeID"].HeaderText = "ID";

                if (DataType.Columns.Contains("TestTypeTitle"))
                    DataType.Columns["TestTypeTitle"].HeaderText = "Title";

                if (DataType.Columns.Contains("TestTypeDescription"))
                    DataType.Columns["TestTypeDescription"].HeaderText = "Description";

                if (DataType.Columns.Contains("TestTypeFees"))
                    DataType.Columns["TestTypeFees"].HeaderText = "Fees";



            }



        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Manage_Test_Types_Load(object sender, EventArgs e)
        {
            LoadData();
            DataType.ContextMenuStrip = contextMenuStrip1;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DataType.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(DataType.CurrentRow.Cells[0].Value);

                Update_Test_Type F1 = new Update_Test_Type(id);
                F1.ShowDialog();
                LoadData();
            }
        }

        private void DataType_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                DataType.ClearSelection();
                DataType.Rows[e.RowIndex].Selected = true;
            }
        }
    }
}
