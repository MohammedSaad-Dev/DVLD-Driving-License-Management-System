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
    public partial class Application_Basic_Info : UserControl
    {

        private int _PersonID = -1;
        private int _LocalDLAppID = -1;


        public Application_Basic_Info()
        {
            InitializeComponent();
        }




        public int LocalDLAppID
        {
            get { return _LocalDLAppID; }
            set
            {
                _LocalDLAppID = value;
                LoadApplicationBasicInfo(_LocalDLAppID);
            }
        }

        public void LoadApplicationBasicInfo(int localDLAppID)
        {
            string query = @"
            SELECT 
                A.ApplicationID,
                A.ApplicantPersonID,
                Case 
                    when A.ApplicationStatus = 1 then 'New'
                    when A.ApplicationStatus = 2 then 'Cancelled'
                    when A.ApplicationStatus = 3 then 'Completed'
                End as Status,
                A.PaidFees,
                AT.ApplicationTypeTitle,
                P.FirstName + ' ' + P.SecondName + ' ' + P.ThirdName + ' ' + P.LastName as FullName,
                A.ApplicationDate,
                A.LastStatusDate,
                U.UserName
            FROM LocalDrivingLicenseApplications LDLA
            INNER JOIN Applications A ON LDLA.ApplicationID = A.ApplicationID
            INNER JOIN ApplicationTypes AT ON A.ApplicationTypeID = AT.ApplicationTypeID
            INNER JOIN People P ON A.ApplicantPersonID = P.PersonID
            INNER JOIN Users U ON A.CreatedByUserID = U.UserID
            WHERE LDLA.LocalDrivingLicenseApplicationID = @LocalDLAppID";

            using (SqlConnection connection = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDLAppID", localDLAppID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // 3. تخزين الـ PersonID
                                _PersonID = Convert.ToInt32(reader["ApplicantPersonID"]);

                                lblApplicationID.Text = reader["ApplicationID"].ToString();
                                lblStatus.Text = reader["Status"].ToString();
                                lblFees.Text = reader["PaidFees"].ToString();
                                lblType.Text = reader["ApplicationTypeTitle"].ToString();
                                lblApplicant.Text = reader["FullName"].ToString();
                                lblDate.Text = Convert.ToDateTime(reader["ApplicationDate"]).ToString("dd/MMM/yyyy");
                                lblStatusDate.Text = Convert.ToDateTime(reader["LastStatusDate"]).ToString("dd/MMM/yyyy");
                                lblCreatedBy.Text = reader["UserName"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }



       

     


        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void Application_Basic_Info_Load(object sender, EventArgs e)
        {
                LoadApplicationBasicInfo(_LocalDLAppID);
            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            if (_PersonID != -1)
            {
                PersonDetails_form frm = new PersonDetails_form(_PersonID);
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("No person selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    
    }
}
