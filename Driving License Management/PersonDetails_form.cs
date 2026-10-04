using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Driving_License_Management
{
    public partial class PersonDetails_form : Form
    {
        private int _PersonID = -1;

        public PersonDetails_form()
        {
            InitializeComponent();
        }

        public PersonDetails_form(int personID)
        {
            InitializeComponent();
            _PersonID = personID;
        }
        private void PersonDetails_form_Load(object sender, EventArgs e)
        {
            ctrlPersonCard1.LoadPersonInfo(_PersonID);


        }

        private void person_Details1_Load(object sender, EventArgs e)
        {

        }
    }
}
