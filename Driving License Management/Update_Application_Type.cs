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
    public partial class Update_Application_Type : Form
    {
        private int _ApplicationTypeID;
        public Update_Application_Type()
        {
            InitializeComponent();
        }

        public Update_Application_Type(int applicationTypeID)
        {
            InitializeComponent();
            _ApplicationTypeID = applicationTypeID;
        }

        private void  DataLoad()
        {
            label2.Text = _ApplicationTypeID.ToString();

            string query = "SELECT * FROM ApplicationTypes WHERE ApplicationTypeID = @ID";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", _ApplicationTypeID);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtTitle.Text = reader["ApplicationTypeTitle"].ToString();
                            txtFees.Text = reader["ApplicationFees"].ToString();
                        }
                    }
                }
            }
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Update_Application_Type_Load(object sender, EventArgs e)
        {


            DataLoad();



        }

        private void button1_Click(object sender, EventArgs e)
        {



            string query = @"UPDATE ApplicationTypes 
                             SET ApplicationTypeTitle = @Title, 
                                 ApplicationFees = @Fees 
                             WHERE ApplicationTypeID = @ID";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", txtTitle.Text.Trim());
                    command.Parameters.AddWithValue("@Fees", Convert.ToDecimal(txtFees.Text.Trim()));
                    command.Parameters.AddWithValue("@ID", _ApplicationTypeID);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Data Saved Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to save data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }



    }
    
        
    
}
