using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Interpolation
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void btnInterpolation_Click(object sender, EventArgs e)
        {
            this.Hide();
            Interpolation form1 = new Interpolation();
            form1.ShowDialog();
            this.Show();
        }

        private void btnDerivativeIntegral_Click(object sender, EventArgs e)
        {
            this.Hide();
            DerivativeIntegral form2 = new DerivativeIntegral();
            form2.ShowDialog();
            this.Show();
        }
    }
}
