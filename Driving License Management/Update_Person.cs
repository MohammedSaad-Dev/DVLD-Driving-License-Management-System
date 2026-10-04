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
using DB_Layer;

namespace Driving_License_Management
{
    public partial class Update_Person : UserControl
    {
        private int _PersonID=-1;

        public Update_Person()
        {
            InitializeComponent();

        }
        public void LoadPersonInfo(int personID)
        {
            _PersonID = personID;

            label17.Text = _PersonID.ToString();

            _LoadDataFromDatabase();
        }


        private void _LoadDataFromDatabase()
        {




            string query = "SELECT * FROM People WHERE PersonID = @PersonID";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", _PersonID);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            FirstName.Text = reader["FirstName"].ToString();
                            SecondName.Text = reader["SecondName"].ToString();
                            ThirdName.Text = reader["ThirdName"].ToString();
                            LastName.Text = reader["LastName"].ToString();
                            NationalNo.Text = reader["NationalNo"].ToString();
                            DateOfBirth.Value = Convert.ToDateTime(reader["DateOfBirth"]);

                            if (Convert.ToByte(reader["Gendor"]) == 0)
                                M.Checked = true;
                            else
                                F.Checked = true;

                            Phone.Text = reader["Phone"].ToString();
                            Email.Text = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";
                            Address.Text = reader["Address"].ToString();
                            Country.SelectedValue = reader["NationalityCountryID"];

                            if (reader["ImagePath"] != DBNull.Value && !string.IsNullOrEmpty(reader["ImagePath"].ToString()))
                            {
                                ImagePath.ImageLocation = reader["ImagePath"].ToString();
                            }
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {



            string query = @"UPDATE People 
                    SET NationalNo = @NationalNo,
                        FirstName = @FirstName,
                        SecondName = @SecondName,
                        ThirdName = @ThirdName,
                        LastName = @LastName,
                        DateOfBirth = @DateOfBirth,
                        Gendor = @Gendor,
                        Address = @Address,
                        Phone = @Phone,
                        Email = @Email,
                        NationalityCountryID = @NationalityCountryID,
                        ImagePath = @ImagePath
                    WHERE PersonID = @PersonID";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", _PersonID);

                    command.Parameters.AddWithValue("@NationalNo", NationalNo.Text.Trim());
                    command.Parameters.AddWithValue("@FirstName", FirstName.Text.Trim());
                    command.Parameters.AddWithValue("@SecondName", SecondName.Text.Trim());
                    command.Parameters.AddWithValue("@ThirdName", ThirdName.Text.Trim());
                    command.Parameters.AddWithValue("@LastName", LastName.Text.Trim());
                    command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth.Value);
                    command.Parameters.AddWithValue("@Gendor", M.Checked ? 0 : 1);
                    command.Parameters.AddWithValue("@Address", Address.Text.Trim());
                    command.Parameters.AddWithValue("@Phone", Phone.Text.Trim());
                    command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email.Text.Trim()) ? DBNull.Value : (object)Email.Text.Trim());
                    command.Parameters.AddWithValue("@NationalityCountryID", Country.SelectedValue ?? 1);
                    command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(ImagePath.ImageLocation) ? DBNull.Value : (object)ImagePath.ImageLocation);
                     
                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Error: Data Is Not Saved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        

        private void Update_Person_Load(object sender, EventArgs e)
        {

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
                    catch { /* في حال تعذر الحذف */ }
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }
    }

}
