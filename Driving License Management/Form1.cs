using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Driving_License_Management.Global; 

namespace Driving_License_Management
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void pepleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void signOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();

            Login_Form loginForm = new Login_Form();
            loginForm.ShowDialog();

            this.Close();
        }

        private void manageUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Manage_Users f2 = new Manage_Users();

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();

        }

        private void currentUserInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PersonDetails_form f2 = new PersonDetails_form(clsGlobal.CurrentPersonID);

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Update_User_Form f2 = new Update_User_Form(clsGlobal.CurrentUserID);

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();
        }

        private void drivingLicensesServicesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void manageApplicationTyesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Manage_Application_Types f2 = new Manage_Application_Types();

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();

        }

        private void manageTestTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Manage_Test_Types f2 = new Manage_Test_Types();

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();
        }

        private void localDrivingLicenseApplicationsToolStripMenuItem_Click(object sender, EventArgs e)
        {


            Loacl_DLA f2 = new Loacl_DLA();

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();


        }

        private void manageApplicationsToolStripMenuItem_Click(object sender, EventArgs e)
        {

          

        }

        private void driversToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Drivers f2 = new Drivers();

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();
        }

        private void internationalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            International_Lic f2 = new International_Lic();

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();
            
        }

        private void renewDrivingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Renew_L_A f2 = new Renew_L_A();

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();
        }

        private void replacementForLostDamagedLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            R_F_D_OR_L_L f2 = new R_F_D_OR_L_L();

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();
        }

        private void manageDetainedLicensesToolStripMenuItem_Click(object sender, EventArgs e)
        {
           List_Detained_Lice f2 = new List_Detained_Lice();

           f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();
        }

        private void detainLincenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Detain_L f2 = new Detain_L();

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();
        }

        private void releaseDetainedLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Release_Detained_License f2 = new Release_Detained_License();

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();
        }

        private void releaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Release_Detained_License f2 = new Release_Detained_License();

            f2.StartPosition = FormStartPosition.CenterScreen;
            f2.Show();
        }

        private void applicationsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
