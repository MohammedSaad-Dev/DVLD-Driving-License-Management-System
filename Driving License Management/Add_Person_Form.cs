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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Driving_License_Management
{
    public partial class Add_Person_Form : Form
    {
        public delegate void DataBackEventHandler(object sender, int PersonID, string NationalNo, string FirstName, string SecondName, string ThirdName, string LastName, string Email, DateTime DateOfBirth, byte Gendor, string Address, string Phone, string CountryName, string ImagePath);
        public event DataBackEventHandler DataBack;

        public Add_Person_Form()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            add_Person1.DataBack += Add_Person1_DataBack;
        }

        private void Add_Person1_DataBack(object sender, int personID, string NationalNo, string FirstName, string SecondName, string ThirdName, string LastName, string Email, DateTime DateOfBirth, byte Gendor, string Address, string Phone, string CountryName, string ImagePath)
        {
            DataBack?.Invoke(this, personID, NationalNo, FirstName, SecondName, ThirdName, LastName, Email, DateOfBirth, Gendor, Address, Phone, CountryName, ImagePath);
        }


        private void add_Person1_Load(object sender, EventArgs e)
        {
            
            
        }

    }
    
}
