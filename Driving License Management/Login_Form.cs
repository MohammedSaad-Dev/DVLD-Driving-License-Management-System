using DB_Layer;
using Driving_License_Management.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Driving_License_Management
{
    public partial class Login_Form : Form
    {
        public Login_Form()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
        
            string username = txtUserName.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter your username and password !", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"SELECT UserID, PersonID, IsActive FROM Users WHERE UserName = @Username AND Password = @Password";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", username);
                    command.Parameters.AddWithValue("@Password", password);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            bool isActive = Convert.ToBoolean(reader["IsActive"]);

                            if (!isActive)
                            {
                                MessageBox.Show("Your account is disabled, please contact the administrator !", "Your account is inactive", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            clsGlobal.CurrentUserID = Convert.ToInt32(reader["UserID"]);
                            clsGlobal.CurrentPersonID = Convert.ToInt32(reader["PersonID"]);
                            clsGlobal.CurrentUserName = username;


                            clsGlobal.CurrentUserName = txtUserName.Text.Trim(); 
                            if (chkRememberMe.Checked)
                            {
                                Properties.Settings.Default.UserName = username;
                                Properties.Settings.Default.Password = password;
                            }
                            else
                            {
                                Properties.Settings.Default.UserName = "";
                                Properties.Settings.Default.Password = "";
                            }
                            Properties.Settings.Default.Save();

                            this.Hide();
                            Form1 f2 = new Form1();
                            f2.ShowDialog();

                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("The username or password is incorrect!", "Login failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while connecting to the database: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        

        private void Login_Form_Load(object sender, EventArgs e)
        {

            txtUserName.Text = Properties.Settings.Default.UserName;
            txtPassword.Text = Properties.Settings.Default.Password;
            chkRememberMe.Checked = !string.IsNullOrEmpty(txtUserName.Text);
          //  txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }


        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
