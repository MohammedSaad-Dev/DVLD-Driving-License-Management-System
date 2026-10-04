using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using DB_Layer;


namespace Driving_License_Management
{
    
    
    
    
    public partial class Add_Person : UserControl
    {

        public delegate void DataBackEventHandler(object sender,int PersonID,string NationalNo,string FirstName,string SecondName,string ThirdName,string LastName,string Email,DateTime DateOfBirth,byte Gendor,string Address,string Phone,string CountryName,string ImagePath);
         public event DataBackEventHandler DataBack;

        public Add_Person()
        {
            InitializeComponent();
        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       



        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {


          
            string checkQuery = "SELECT COUNT(1) FROM People WHERE NationalNo = @NationalNo";

            using (SqlConnection checkConnection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, checkConnection))
                {
                    checkCommand.Parameters.AddWithValue("@NationalNo", NationalNo.Text.Trim());
                    try
                    {
                        checkConnection.Open();
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show("This national ID number already exists in the system!", "Alert - Duplicate person", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("خطأ أثناء التحقق: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(NationalNo.Text) ||
                string.IsNullOrWhiteSpace(FirstName.Text) ||
                string.IsNullOrWhiteSpace(SecondName.Text) ||
                string.IsNullOrWhiteSpace(ThirdName.Text) ||
                string.IsNullOrWhiteSpace(LastName.Text) ||
                string.IsNullOrWhiteSpace(Phone.Text) ||
                string.IsNullOrWhiteSpace(Email.Text) ||
                string.IsNullOrWhiteSpace(Address.Text))
            {
                MessageBox.Show("Please fill in all the required fields!", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int age = DateTime.Now.Year - DateOfBirth.Value.Year;
            if (DateOfBirth.Value.Date > DateTime.Now.AddYears(-age))
                age--;

            if (age < 18)
            {
                MessageBox.Show("Person must be 18 years old or older!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ImagePath.ImageLocation == null)
            {
                MessageBox.Show("Please upload a profile picture!", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. استعلام الحفظ في قاعدة البيانات
            string query = @"INSERT INTO People
               (
                   NationalNo, FirstName, SecondName, ThirdName, LastName,
                   DateOfBirth, Gendor, Address, Phone, Email, NationalityCountryID, ImagePath
               )
               VALUES
               (
                   @NationalNo, @FirstName, @SecondName, @ThirdName, @LastName,
                   @DateOfBirth, @Gendor, @Address, @Phone, @Email, @NationalityCountryID, @ImagePath
               );
               
               SELECT SCOPE_IDENTITY();";

            byte gendor = M.Checked ? (byte)0 : (byte)1;

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNo", NationalNo.Text.Trim());
                    command.Parameters.AddWithValue("@FirstName", FirstName.Text.Trim());
                    command.Parameters.AddWithValue("@SecondName", SecondName.Text.Trim());
                    command.Parameters.AddWithValue("@ThirdName", ThirdName.Text.Trim());
                    command.Parameters.AddWithValue("@LastName", LastName.Text.Trim());
                    command.Parameters.AddWithValue("@Email", Email.Text.Trim());
                    command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth.Value);
                    command.Parameters.AddWithValue("@Gendor", gendor);
                    command.Parameters.AddWithValue("@Address", Country.Text.Trim()+"-"+Address.Text.Trim());
                    command.Parameters.AddWithValue("@Phone", Phone.Text.Trim());
                    command.Parameters.AddWithValue("@NationalityCountryID", Country.SelectedIndex + 1);
                    command.Parameters.AddWithValue("@ImagePath", (object)ImagePath.ImageLocation ?? DBNull.Value);


                   

                    try
                    {
                        connection.Open();
                        int personID = Convert.ToInt32(command.ExecuteScalar());
                        string countryName = Country.Text;


                        DataBack?.Invoke(this,personID,NationalNo.Text.Trim(),FirstName.Text.Trim(),SecondName.Text.Trim(),ThirdName.Text.Trim(),LastName.Text.Trim(),Email.Text.Trim(),DateOfBirth.Value,gendor,Address.Text.Trim(),Phone.Text.Trim(),countryName,ImagePath.ImageLocation);

                        if (personID > 0)
                        {
                            MessageBox.Show("Data Saved Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            label11.Text = personID.ToString();


                        }
                        else
                        {
                            MessageBox.Show("Failed to save data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        


        





        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {


           OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string destinationFolder = @"F:\DVLD-People-Images\";

                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                if (!string.IsNullOrEmpty(ImagePath.ImageLocation) && File.Exists(ImagePath.ImageLocation))
                {
                    try
                    {
                        string oldPath = ImagePath.ImageLocation;
                        ImagePath.ImageLocation = null;
                        File.Delete(oldPath);
                    }
                    catch (Exception ex)
                    { /* في حال تعذر الحذف */ }
                }

                string sourceFile = openFileDialog1.FileName;
                string fileExtension = Path.GetExtension(sourceFile);
                string newFileName = Guid.NewGuid().ToString() + fileExtension;
                string destinationFilePath = Path.Combine(destinationFolder, newFileName);

                System.IO.File.Copy(sourceFile, destinationFilePath, true);

                ImagePath.ImageLocation = destinationFilePath;

                // llRemove.Visible = true;

              //  Parameters.AddWithValue("@ImagePath", (object)ImagePath.ImageLocation ?? DBNull.Value);
            }
        }



        
        

        private void DateOfBirth_ValueChanged(object sender, EventArgs e)
        {
        }

        private void Email_TextChanged(object sender, EventArgs e)
        {
         
            if (string.IsNullOrWhiteSpace(Email.Text))
            {
                errorProvider1.SetError(Email, "");
                return;
            }

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (!Regex.IsMatch(Email.Text.Trim(), pattern))
            {
                Email.Focus();
                errorProvider1.SetError(Email, "The email format is incorrect (e.g., example@gmail.com)");
            }
            else
            {
                errorProvider1.SetError(Email, ""); 
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

            
        }

        private void Add_Person_Load(object sender, EventArgs e)
        {

        }
    }
    
    
}