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
    public partial class Update_Form : Form
    {
        private int __PersonID = -1;
        public Update_Form()
        {
            InitializeComponent();
        }

        public Update_Form(int personID)
        {
            InitializeComponent();
            __PersonID = personID;
        }

        private void update_Person1_Load(object sender, EventArgs e)
        {

        }

        private void Update_Form_Load(object sender, EventArgs e)
        {
            
            update_Person1.LoadPersonInfo(__PersonID);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;




        }
    }
}
