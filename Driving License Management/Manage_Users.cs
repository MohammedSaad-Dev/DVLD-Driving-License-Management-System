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
    public partial class Manage_Users : Form
    {

        private DataView _dtPeopleView;

        public Manage_Users()
        {
            InitializeComponent();

        }


        private void LoadData()
        {

          

            string query = "SELECT Users.UserID,  Users.PersonID,  Users.UserName, " +

                "(People.FirstName + ' ' + People.SecondName + ' ' + People.ThirdName + ' ' + People.LastName)" +" AS FullName" +
                "" +
                ",Users.IsActive FROM  Users INNER JOIN People  ON Users.PersonID = People.PersonID;";


            // استخدام using لضمان إغلاق الاتصال تلقائياً وتوفير موارد الجهاز
            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);


                    _dtPeopleView = dataTable.DefaultView;

                    dgvUsers.DataSource = _dtPeopleView;

                    dgvUsers.DataSource = dataTable;
                    dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


                    label2.Text = _dtPeopleView.Count.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching the data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }




            }


            if (dgvUsers.Columns.Contains("UserID"))
                dgvUsers.Columns["UserID"].HeaderText = "User ID";

            if (dgvUsers.Columns.Contains("PersonID"))
                dgvUsers.Columns["PersonID"].HeaderText = "Person ID";

            if (dgvUsers.Columns.Contains("FullName"))
            {
                // 1. تغيير عنوان العمود ليظهر للمستخدم بشكل جميل وبمسافة
                dgvUsers.Columns["FullName"].HeaderText = "Full Name";

                // 2. ضبط الحجم التلقائي ليتكيف مع أطول اسم
                dgvUsers.Columns["FullName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            if (dgvUsers.Columns.Contains("UserName"))
                dgvUsers.Columns["UserName"].HeaderText = "User Name";

            if (dgvUsers.Columns.Contains("IsActive"))
                dgvUsers.Columns["IsActive"].HeaderText = "Is Active";

        }



        

        private int GetPeopleCountFromDatabase()
        {
            int count = 0;
            string query = "SELECT COUNT(*) FROM Users;";

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


        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Manage_Users_Load(object sender, EventArgs e)
        {
            LoadData();
            label2.Text = GetPeopleCountFromDatabase().ToString();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
           Add_Users_Form  f2 = new Add_Users_Form();

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.ShowDialog();
            LoadData();
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            string filterColumn = "";

            // تحديد العمود المطلوب حسب اختيار المستخدم
            switch (cbFilterBy.Text)
            {
                case "User ID":
                    filterColumn = "UserID";
                    break;

                case "UserName":
                    filterColumn = "UserName";
                    break;

                case "Person ID":
                    filterColumn = "PersonID";
                    break;

                case "Full Name":
                    filterColumn = "FullName";
                    break;

                default:
                    filterColumn = "None";
                    break;
            }

            if (string.IsNullOrWhiteSpace(txtFilterValue.Text) || filterColumn == "None")
            {
                _dtPeopleView.RowFilter = "";
                label1.Text = _dtPeopleView.Count.ToString();
                return;
            }

            if (filterColumn == "UserID" || filterColumn == "PersonID")
            {
                if (int.TryParse(txtFilterValue.Text.Trim(), out int value))
                {
                    _dtPeopleView.RowFilter = $"{filterColumn} = {value}";
                }
                else
                {
                    _dtPeopleView.RowFilter = $"{filterColumn} = -1";
                }
            }
            else
            {
                _dtPeopleView.RowFilter = $"{filterColumn} LIKE '{txtFilterValue.Text.Trim()}%'";
            }

         
        

        label1.Text = _dtPeopleView.Count.ToString();


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
         
            txtFilterValue.Visible = (cbFilterBy.Text != "None" && cbFilterBy.Text != "Is Active");
            cbIsActive.Visible = (cbFilterBy.Text == "Is Active"); // ComboBox آخر اختياري لـ Is Active (All, Yes, No)

            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = "";
                txtFilterValue.Focus();
            }

            if (cbFilterBy.Text == "None")
            {
                _dtPeopleView.RowFilter = "";
                label1.Text = _dtPeopleView.Count.ToString();
            }


        }

        private void cbIsActive_SelectedIndexChanged(object sender, EventArgs e)
        {

            string filterValue = cbIsActive.Text;

            switch (filterValue)
            {
                case "All":
                    _dtPeopleView.RowFilter = "";
                    break;

                case "Yes":
                    _dtPeopleView.RowFilter = "IsActive = true";
                    break;

                case "No":
                    _dtPeopleView.RowFilter = "IsActive = false";
                    break;
            }

            label1.Text = _dtPeopleView.Count.ToString();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (dgvUsers.CurrentRow != null)
            {
                int userID = Convert.ToInt32(dgvUsers.CurrentRow.Cells["UserID"].Value);

                Update_User_Form frm = new Update_User_Form(userID);
                frm.ShowDialog();

                LoadData();
            }
        }

        private void dgvUsers_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
          
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dgvUsers.ClearSelection();

                dgvUsers.Rows[e.RowIndex].Selected = true;

                dgvUsers.CurrentCell = dgvUsers.Rows[e.RowIndex].Cells[e.ColumnIndex >= 0 ? e.ColumnIndex : 0];
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        
            if (dgvUsers.CurrentRow == null) return;

            int userID = Convert.ToInt32(dgvUsers.CurrentRow.Cells["UserID"].Value);

            if (MessageBox.Show("Are you sure you want to delete user [" + userID + "]?", "Confirm Delete",  MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                using (SqlConnection connection = new SqlConnection(DB.connectionString))
                {
                    string query = "DELETE FROM Users WHERE UserID = @UserID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userID);

                        try
                        {
                            connection.Open();
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("User Deleted Successfully.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                LoadData();

                            }
                            else
                            {
                                MessageBox.Show("User was not deleted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (SqlException ex)
                        {
                            // رقم الخطأ 547 يشير إلى Foreign Key Constraint Conflict
                            if (ex.Number == 547)
                            {
                                MessageBox.Show("User cannot be deleted because it is linked to other data in the system (e.g., International Licenses).\n\nYou can deactivate the user instead.",
                                                "Cannot Delete User", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow != null)
            {
                int PersonID = Convert.ToInt32(dgvUsers.CurrentRow.Cells["PersonID"].Value);

                PersonDetails_form frm = new PersonDetails_form(PersonID);
                frm.ShowDialog();

                LoadData();
            }
        }
    }
    
    

    
    
}
