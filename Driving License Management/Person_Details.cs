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
using System.IO;
namespace Driving_License_Management
{
    public partial class Person_Details : UserControl
    {
        public Person_Details()
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
                          
                        }
                        else
                        {
                            MessageBox.Show("لم يتم العثور على الشخص بالرقم: " + PersonID, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ResetPersonInfo();
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

        public void ResetPersonInfo()
        {
            lblPersonID.Text = "[???]";
            lblFullName.Text = "[???]";
            lblNationalNo.Text = "[???]";
            lblGendor.Text = "[???]";
            lblEmail.Text = "[???]";
            lblPhone.Text = "[???]";
            lblDateOfBirth.Text = "[???]";
            lblAddress.Text = "[???]";
        //    pbPersonImage.Image = Properties.Resources.Male_User;
        }








        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Person_Details_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
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
                MessageBox.Show("لا يوجد شخص محدد لتعديله!", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



    }
    
}
