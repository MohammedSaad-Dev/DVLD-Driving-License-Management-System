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
    public partial class Form2 : Form
    {
        private DataView _dtPeopleView;

        public Form2()
        {
            InitializeComponent();
        }



        private void Form2_Load(object sender, EventArgs e)
        {
            LoadData();
                this.FormBorderStyle = FormBorderStyle.FixedSingle;

            label2.Text = GetPeopleCountFromDatabase().ToString();

        }

        private void LoadData()
        {

            string query = "SELECT * FROM People ";


            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    _dtPeopleView = dataTable.DefaultView;

                    dataGridView1.DataSource = _dtPeopleView;

                    dataGridView1.DataSource = dataTable;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                   

                    label2.Text = _dtPeopleView.Count.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching the data:" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }




            }



        }
        private int GetPeopleCountFromDatabase()
        {
            int count = 0;
            string query = "SELECT COUNT(*) FROM People;";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int tempCount))
                        {
                            count = tempCount;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while fetching the number:" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            return count;
        }


        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Add_Person_Form f2 = new Add_Person_Form();

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.ShowDialog();
            LoadData();
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

            if (dataGridView1.CurrentRow != null)
            {
                int selectedPersonID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["PersonID"].Value);

                Update_Form frm = new Update_Form(selectedPersonID);
                frm.ShowDialog();

                LoadData();
            }


        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {


            if (dataGridView1.CurrentRow != null)
            {
                int personID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["PersonID"].Value);

                if (MessageBox.Show("Are you sure you want to delete Person [ " + personID + " ]?", "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    string query = "DELETE FROM People WHERE PersonID = @PersonID";

                    using (SqlConnection connection = new SqlConnection(DB.connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@PersonID", personID);

                            try
                            {
                                connection.Open();
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Person Deleted Successfully.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    LoadData();
                                }
                                else
                                {
                                    MessageBox.Show("Person was not deleted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("This person can't be deleted because they're linked to requests or licenses in the system.", "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


            if (comboBox1.Text == "None")
            {
                textBox1.Visible = false;
                _dtPeopleView.RowFilter = "";
            }
            else
            {
                textBox1.Visible = true;
                textBox1.Focus();
            }

            label1.Text = _dtPeopleView.Count.ToString();
        
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {


            if (string.IsNullOrEmpty(textBox1.Text.Trim()) || comboBox1.Text == "None")
            {
                _dtPeopleView.RowFilter = "";
                label1.Text = _dtPeopleView.Count.ToString();
                return;
            }

            string filterColumn = comboBox1.Text.Replace(" ", "");

            if (filterColumn == "PersonID")
            {
                if (int.TryParse(textBox1.Text.Trim(), out int personID))
                {
                    _dtPeopleView.RowFilter = string.Format("[{0}] = {1}", filterColumn, personID);
                }
                else
                {
                    _dtPeopleView.RowFilter = "[PersonID] = -1";
                }
            }
            else
            {
                _dtPeopleView.RowFilter = string.Format("[{0}] LIKE '{1}%'", filterColumn, textBox1.Text.Trim());
            }

            label2.Text = _dtPeopleView.Count.ToString();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (comboBox1.Text == "Person ID" || comboBox1.Text == "PersonID")
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true; 
                }
            }
        


    }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int selectedPersonID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["PersonID"].Value);

                PersonDetails_form frm = new PersonDetails_form(selectedPersonID);
                frm.ShowDialog();

            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }


    
    
    
}