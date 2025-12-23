using Interpolation.Utilities;
using System;
using System.Windows.Forms;
namespace Interpolation
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private async void btnCheckUpdate_Click(object sender, EventArgs e)
        {
            btnCheckUpdate.Enabled = false;
            btnCheckUpdate.Text = "Đang kiểm tra...";

            await UpdateManager.CheckForUpdateAsync(true);

            btnCheckUpdate.Text = "Kiểm tra cập nhật";
            btnCheckUpdate.Enabled = true;
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

        private void btnODE_Click(object sender, EventArgs e)
        {
            this.Hide();
            ODE form3 = new ODE();
            form3.ShowDialog();
            this.Show();
        }
    }
}
