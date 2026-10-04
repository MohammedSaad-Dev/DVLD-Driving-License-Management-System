using DB_Layer;
using Driving_License_Management.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Driving_License_Management
{
    public partial class New_Local_DLA : Form
    {
        public delegate void DataBackEventHandler(object sender);
        public event DataBackEventHandler DataBack;

        private int _SelectedPersonID = -1; 
        public New_Local_DLA()
        {
            InitializeComponent();
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
                            // تعبئة البيانات
                            lblPersonID.Text = reader["PersonID"].ToString();
                            _SelectedPersonID = Convert.ToInt32(reader["PersonID"]);
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
                            MessageBox.Show($"No person was found with the number/statement: {textBox1.Text.Trim()}", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }



        public void LoadPersonInfo(int PersonID)
        {
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

        private void label16_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Add_Person_Form F = new Add_Person_Form();
            F.ShowDialog();

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

        private void New_Local_DLA_Load(object sender, EventArgs e)
        {
            _ResetPersonInfo();
            _ResetDefaultValues();

        }

        private void _ResetDefaultValues()
        {
            label19.Text = "[???]";

            label18.Text = DateTime.Now.ToString("dd/MM/yyyy");


            _FillLicenseClassesInComboBox();

            if (comboBox2.Items.Count > 0)
                comboBox2.SelectedIndex = 0;


            label20.Text = clsGlobal.CurrentUserName;
        }


        private void _FillLicenseClassesInComboBox()
        {
            comboBox2.Items.Clear();

            string query = "SELECT ClassName FROM LicenseClasses";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBox2.Items.Add(reader["ClassName"].ToString());
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
                textBox1.Text = "";
                _ResetPersonInfo();
            }
            else
            {
                textBox1.Visible = true;
                textBox1.Focus();

                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    _FindAndLoadPersonInfo();
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                _ResetPersonInfo();
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

        private void btnSave_Click(object sender, EventArgs e)
        {

            int selectedPersonID = _SelectedPersonID; 

            if (selectedPersonID == -1)
            {
                MessageBox.Show("Please select a person first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int licenseClassID = _GetLicenseClassID(comboBox2.Text);

            if (_IsApplicationExistForLicenseClass(selectedPersonID, licenseClassID))
            {
                MessageBox.Show("Choose another License Class, the selected person already has an active application for this class!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int newApplicationID = _SaveBaseApplication(selectedPersonID);

            if (newApplicationID != -1)
            {
                int newLocalDLAppID = _SaveLocalDrivingLicenseApplication(newApplicationID, licenseClassID);

                if (newLocalDLAppID != -1)
                {
                    label19.Text = newLocalDLAppID.ToString();
                    MessageBox.Show("Data Saved Successfully with ID = " + newLocalDLAppID, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DataBack?.Invoke(this);
                    btnSave.Enabled = false; 
                }
                else
                {
                    MessageBox.Show("Failed to save Local Driving License Application!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Failed to save Base Application!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _ResetDefaultValues();
          
        }



        private int _SaveBaseApplication(int applicantPersonID)
        {
            int insertedID = -1;
            string query = @"INSERT INTO Applications 
                    (ApplicantPersonID, ApplicationDate, ApplicationTypeID, ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID)
                    VALUES 
                    (@ApplicantPersonID, @ApplicationDate, @ApplicationTypeID, @ApplicationStatus, @LastStatusDate, @PaidFees, @CreatedByUserID);
                    SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicantPersonID", applicantPersonID);
                    command.Parameters.AddWithValue("@ApplicationDate", DateTime.Now);
                    command.Parameters.AddWithValue("@ApplicationTypeID", 1); // 1 = New Local Driving License
                    command.Parameters.AddWithValue("@ApplicationStatus", 1); // 1 = New
                    command.Parameters.AddWithValue("@LastStatusDate", DateTime.Now);
                    command.Parameters.AddWithValue("@PaidFees", Convert.ToDecimal(label17.Text));
                    command.Parameters.AddWithValue("@CreatedByUserID", clsGlobal.CurrentUserID);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int id))
                    {
                        insertedID = id;
                    }
                }
            }
            return insertedID;
        }

        private int _SaveLocalDrivingLicenseApplication(int applicationID, int licenseClassID)
        {
            int insertedID = -1;
            string query = @"INSERT INTO LocalDrivingLicenseApplications (ApplicationID, LicenseClassID)
                    VALUES (@ApplicationID, @LicenseClassID);
                    SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", applicationID);
                    command.Parameters.AddWithValue("@LicenseClassID", licenseClassID);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int id))
                    {
                        insertedID = id;
                    }
                }
            }
            return insertedID;
        }

        private int _GetLicenseClassID(string className)
        {
            int classID = -1;
            string query = "SELECT LicenseClassID FROM LicenseClasses WHERE ClassName = @ClassName";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClassName", className);
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                        classID = Convert.ToInt32(result);
                }
            }
            return classID;
        }

        private bool _IsApplicationExistForLicenseClass(int personID, int licenseClassID)
        {
            bool isExist = false;
            string query = @"SELECT Found = 1 FROM LocalDrivingLicenseApplications LDLA
                    INNER JOIN Applications A ON LDLA.ApplicationID = A.ApplicationID
                    WHERE A.ApplicantPersonID = @PersonID 
                      AND LDLA.LicenseClassID = @LicenseClassID 
                      AND A.ApplicationStatus = 1"; // Status 1 = New

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", personID);
                    command.Parameters.AddWithValue("@LicenseClassID", licenseClassID);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                        isExist = true;
                }
            }
            return isExist;
        }









        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        
            string selectedClassName = comboBox2.Text;

            string query = "SELECT ClassFees FROM LicenseClasses WHERE ClassName = @ClassName";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClassName", selectedClassName);
                    connection.Open();

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        // تحديث الـ Label الخاص بالرسوم
                        label17.Text = Convert.ToDecimal(result).ToString("0.##");
                    }
                }
            }
        }
    }
    
}
