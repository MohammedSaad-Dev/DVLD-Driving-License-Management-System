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
    public partial class Update_User_Form : Form
    {

        private int _UserID;
        public Update_User_Form()
        {
            InitializeComponent();
           
        }
        public Update_User_Form(int userID)
        {
            InitializeComponent();
            _UserID = userID;
        }


        private void _LoadUserData()
        {

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                string query = "SELECT * FROM Users WHERE UserID = @UserID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", _UserID);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            lblUserID.Text = reader["UserID"].ToString();
                            txtUserName.Text = reader["UserName"].ToString();
                            txtPassword.Text = reader["Password"].ToString();
                            txtConfirmPassword.Text = reader["Password"].ToString();
                            chkIsActive.Checked = Convert.ToBoolean(reader["IsActive"]);

                            int personID = Convert.ToInt32(reader["PersonID"]);


                        }
                        else
                        {
                            MessageBox.Show("User not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void Update_User_Load(object sender, EventArgs e)
        {
            _LoadUserData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
         
            if (string.IsNullOrWhiteSpace(txtUserName.Text.Trim()))
            {
                MessageBox.Show("Please enter a User Name!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUserName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter a Password!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Password and Confirm Password do not match!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConfirmPassword.Focus();
                return;
            }

            // 3. تحديث البيانات في قاعدة البيانات
            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                string query = @"UPDATE Users 
                         SET UserName = @UserName, 
                             Password = @Password, 
                             IsActive = @IsActive 
                         WHERE UserID = @UserID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", txtUserName.Text.Trim());
                    command.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
                    command.Parameters.AddWithValue("@IsActive", chkIsActive.Checked);
                    command.Parameters.AddWithValue("@UserID", _UserID);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Data Saved Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close(); 
                        }
                        else
                        {
                            MessageBox.Show("Data was not saved!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
         
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
            txtConfirmPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {


            if (int.TryParse(lblUserID.Text, out int personID))
            {
                Update_Form frm = new Update_Form(personID);
                frm.ShowDialog();

            }
            else
            {
                MessageBox.Show("There's no specific person to edit it!", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }
    }
    
}
