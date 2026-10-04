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
using DB_Layer;

namespace Driving_License_Management
{
    public partial class Update_Test_Type : Form
    {

        private int _TestTypeID;

        public Update_Test_Type()
        {
            InitializeComponent();
        }

        public Update_Test_Type(int TestTypeID)
        {
            InitializeComponent();
            _TestTypeID = TestTypeID;
        }


        private void DataLoad()
        {
            label2.Text = _TestTypeID.ToString();

            string query = "SELECT * FROM TestTypes WHERE TestTypeID = @ID";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", _TestTypeID);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtTitle.Text = reader["TestTypeTitle"].ToString();
                            txtDescription.Text = reader["TestTypeDescription"].ToString();
                            txtFees.Text = reader["TestTypeFees"].ToString();
                        }
                    }
                }
            }
        }


        private void Update_Test_Type_Load(object sender, EventArgs e)
        {
            DataLoad();
           
        }

        private void txtTitle_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string query = @"UPDATE TestTypes 
                             SET TestTypeTitle = @Title, 
                                 TestTypeDescription = @Description,
                                 TestTypeFees = @Fees 
                             WHERE TestTypeID = @ID";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", txtTitle.Text.Trim());
                    command.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                    command.Parameters.AddWithValue("@Fees", Convert.ToDecimal(txtFees.Text.Trim()));
                    command.Parameters.AddWithValue("@ID", _TestTypeID);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Test Type Updated Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
