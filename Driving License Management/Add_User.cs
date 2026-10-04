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
using System.IO;

namespace Driving_License_Management
{
    public partial class Add_User : UserControl
    {

        public Add_User()
        {
            InitializeComponent();

        }

        public void LoadPersonInfo(int PersonID)
        {
            // استعلام يجلب كافة بيانات الشخص والدولة المرتبطة به
            string query = @"SELECT People.*, Countries.CountryName 
                         FROM People 
                         LEFT JOIN Countries ON People.NationalityCountryID = Countries.CountryID 
                         WHERE People.PersonID = @PersonID";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            // تعبئة الـ Labels بالبيانات من داتابيز مباشرة
                            lblPersonID.Text = reader["PersonID"].ToString();

                            // تجميع الاسم الكامل
                            string firstName = reader["FirstName"].ToString();
                            string secondName = reader["SecondName"] != DBNull.Value ? reader["SecondName"].ToString() : "";
                            string thirdName = reader["ThirdName"] != DBNull.Value ? reader["ThirdName"].ToString() : "";
                            string lastName = reader["LastName"].ToString();
                            lblFullName.Text = $"{firstName} {secondName} {thirdName} {lastName}".Replace("  ", " ");

                            lblNationalNo.Text = reader["NationalNo"] != DBNull.Value ? reader["NationalNo"].ToString() : "N/A";

                            // الجنس (0 = Male, 1 = Female)
                            byte gendor = Convert.ToByte(reader["Gendor"]);
                            lblGendor.Text = (gendor == 0) ? "Male" : "Female";

                            lblEmail.Text = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "N/A";
                            lblPhone.Text = reader["Phone"].ToString();

                            if (reader["DateOfBirth"] != DBNull.Value)
                            {
                                lblDateOfBirth.Text = Convert.ToDateTime(reader["DateOfBirth"]).ToString("dd/MM/yyyy");
                            }

                            lblAddress.Text = reader["Address"].ToString();

                            // عرض الصورة الشخصية
                            string imagePath = reader["ImagePath"] != DBNull.Value ? reader["ImagePath"].ToString() : "";
                            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                            {
                                pbPersonImage.ImageLocation = imagePath;
                            }

                        }
                        else
                        {
                            MessageBox.Show("لم يتم العثور على الشخص بالرقم: " + PersonID, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                          
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("حدث خطأ أثناء الاتصال بقاعدة البيانات: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                _ResetPersonInfo();
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox1.Text == "None")
            {
                textBox1.Visible = false;
                textBox1.Text = "";
                _ResetPersonInfo();
            }
            else
            {
                textBox1.Visible = true;
                textBox1.Focus();

                // البحث مجدداً بالقيم الموجودة حالياً إن وجدت
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    _FindAndLoadPersonInfo();
                }
            }

        }
        

        private void Add_User_Load(object sender, EventArgs e)
        {

            _ResetPersonInfo();
        }

        private void _ResetPersonInfo()
        {
            lblPersonID.Text = "[ ??? ]";
            lblFullName.Text = "[ ??? ]";
            lblNationalNo.Text = "[ ??? ]";
            lblGendor.Text = "[ ??? ]";
            lblEmail.Text = "[ ??? ]";
            lblPhone.Text = "[ ??? ]";
            lblDateOfBirth.Text = "[ ??? ]";
            lblAddress.Text = "[ ??? ]";
        }

        private void _FindAndLoadPersonInfo()
        {
            string searchValue = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(searchValue))
            {
                _ResetPersonInfo();
                return;
            }

            string filterColumn = "";

            switch (comboBox1.Text)
            {
                case "Person ID":
                    filterColumn = "People.PersonID";
                    break;
                case "National No":
                    filterColumn = "People.NationalNo";
                    break;
                case "First Name":
                    filterColumn = "People.FirstName";
                    break;
                case "Phone":
                    filterColumn = "People.Phone";
                    break;
                case "Email":
                    filterColumn = "People.Email";
                    break;
                default:
                    return; 
            }

            string query = $@"SELECT People.*, Countries.CountryName 
                             FROM People 
                             LEFT JOIN Countries ON People.NationalityCountryID = Countries.CountryID 
                             WHERE {filterColumn} = @SearchValue";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SearchValue", searchValue);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            lblPersonID.Text = reader["PersonID"].ToString();

                            string firstName = reader["FirstName"].ToString();
                            string secondName = reader["SecondName"] != DBNull.Value ? reader["SecondName"].ToString() : "";
                            string thirdName = reader["ThirdName"] != DBNull.Value ? reader["ThirdName"].ToString() : "";
                            string lastName = reader["LastName"].ToString();
                            lblFullName.Text = $"{firstName} {secondName} {thirdName} {lastName}".Replace("  ", " ");

                            lblNationalNo.Text = reader["NationalNo"] != DBNull.Value ? reader["NationalNo"].ToString() : "N/A";

                            byte gendor = Convert.ToByte(reader["Gendor"]);
                            lblGendor.Text = (gendor == 0) ? "Male" : "Female";

                            lblEmail.Text = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "N/A";
                            lblPhone.Text = reader["Phone"].ToString();

                            if (reader["DateOfBirth"] != DBNull.Value)
                            {
                                lblDateOfBirth.Text = Convert.ToDateTime(reader["DateOfBirth"]).ToString("dd/MM/yyyy");
                            }

                            lblAddress.Text = reader["Address"].ToString();

                            string imagePath = reader["ImagePath"] != DBNull.Value ? reader["ImagePath"].ToString() : "";
                            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                            {
                                pbPersonImage.ImageLocation = imagePath;
                            }
                            else
                            {
                                pbPersonImage.Image = null;
                            }
                        }
                        else
                        {
                            _ResetPersonInfo();
                            MessageBox.Show($"No person was found with the number/statement: {textBox1.Text.Trim()}", "Alert", MessageBoxButtons.OK,MessageBoxIcon.Warning);
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        // يتجاهل الأخطاء البسيطة أثناء الكتابة أو يمكنك عرض الرسالة عند اللزوم
                    }
                }
            }
        }












        private void lblEmail_Click(object sender, EventArgs e)
        {

        }

        private void lblGendor_Click(object sender, EventArgs e)
        {

        }

        private void lblAddress_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (comboBox1.Text == "Person ID" || comboBox1.Text == "Phone")
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please enter a value to search!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _FindAndLoadPersonInfo();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            if (int.TryParse(lblPersonID.Text, out int personID))
            {
                Update_Form frm = new Update_Form(personID);
                frm.ShowDialog();

                LoadPersonInfo(personID);
            }
            else
            {
                MessageBox.Show("There's no specific person to edit it!", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (lblPersonID.Text == "[ ??? ]" || string.IsNullOrEmpty(lblPersonID.Text))
            {
                MessageBox.Show("Please select a person first!", "Select Person", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int personID = Convert.ToInt32(lblPersonID.Text);

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                string checkPersonQuery = "SELECT COUNT(1) FROM Users WHERE PersonID = @PersonID";
                using (SqlCommand command = new SqlCommand(checkPersonQuery, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", personID);

                    try
                    {
                        connection.Open();
                        int userCount = Convert.ToInt32(command.ExecuteScalar());

                        if (userCount > 0)
                        {
                            MessageBox.Show("This person is already a user in the system! Choose another person.", "User Exists", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            tabControl1.SelectedIndex = 1;
        }
        

        private void button4_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {

            if (lblPersonID.Text == "[ ??? ]" || string.IsNullOrEmpty(lblPersonID.Text))
            {
                MessageBox.Show("Please select a person first!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tabControl1.SelectedIndex = 0; 
                return;
            }

            int personID = Convert.ToInt32(lblPersonID.Text);
            string userName = txtUserName.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();
            bool isActive = chkIsActive.Checked;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("UserName and Password cannot be empty!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. التحقق من تطابق كلمتي السر
            if (password != confirmPassword)
            {
                MessageBox.Show("Password Confirmation does not match!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConfirmPassword.Focus();
                return;
            }

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                try
                {
                    connection.Open();

                    string checkPersonQuery = "SELECT COUNT(1) FROM Users WHERE PersonID = @PersonID";
                    using (SqlCommand checkPersonCmd = new SqlCommand(checkPersonQuery, connection))
                    {
                        checkPersonCmd.Parameters.AddWithValue("@PersonID", personID);
                        int count = Convert.ToInt32(checkPersonCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show("This person is already a user in the system!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // 5. التحقق مما إذا كان اسم المستخدم مستخدماً من قبل
                    string checkUserQuery = "SELECT COUNT(1) FROM Users WHERE UserName = @UserName";
                    using (SqlCommand checkUserCmd = new SqlCommand(checkUserQuery, connection))
                    {
                        checkUserCmd.Parameters.AddWithValue("@UserName", userName);
                        int count = Convert.ToInt32(checkUserCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show("UserName is already taken, please choose another one!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtUserName.Focus();
                            return;
                        }
                    }

                    // 6. إضافة المستخدم الجديد في جدول Users واسترجاع الـ UserID
                    string insertQuery = @"INSERT INTO Users (PersonID, UserName, Password, IsActive) 
                                   VALUES (@PersonID, @UserName, @Password, @IsActive);
                                   SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", personID);
                        command.Parameters.AddWithValue("@UserName", userName);
                        command.Parameters.AddWithValue("@Password", password); // يُفضل تشفيره مستقبلاً
                        command.Parameters.AddWithValue("@IsActive", isActive);

                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int newUserID))
                        {
                            label12.Text = newUserID.ToString(); // عرض الـ UserID الجديد
                            MessageBox.Show("User Data Saved Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // تعطيل زر الحفظ لمنع التكرار
                            btnSave.Enabled = false;
                        }
                        else
                        {
                            MessageBox.Show("Failed to save user data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Add_Person_Form F=new Add_Person_Form();
            F.DataBack += Frm_DataBack;
            F.ShowDialog();
        }
        private void Frm_DataBack(object sender, int PersonID, string NationalNo, string FirstName, string SecondName, string ThirdName, string LastName, string Email, DateTime DateOfBirth, byte Gendor, string Address, string Phone, string CountryName, string ImagePath)

        {
            lblPersonID.Text = PersonID.ToString();
            lblFullName.Text = $"{FirstName} {SecondName} {ThirdName} {LastName}";
            lblNationalNo.Text = NationalNo;
            lblEmail.Text = Email;
            lblDateOfBirth.Text = DateOfBirth.ToShortDateString();
            lblGendor.Text = (Gendor == 0) ? "Male" : "Female";
            lblAddress.Text = Address;
            lblPhone.Text = Phone;

            if (!string.IsNullOrEmpty(ImagePath) && System.IO.File.Exists(ImagePath))
            {
                pbPersonImage.ImageLocation = ImagePath;
            }
        }


    }
}
