using System;
using System.Windows.Forms;
using Interpolation.Utilities;
namespace Interpolation
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }
        private async void Dashboard_Load(object sender, EventArgs e)
        {
            chkAutoUpdate.Checked = Properties.Settings.Default.IsAutoUpdate;

            if (chkAutoUpdate.Checked)
            {
                await UpdateManager.CheckForUpdateAsync(false);
            }
        }

        private async void btnCheckUpdate_Click(object sender, EventArgs e)
        {
            btnCheckUpdate.Enabled = false;
            btnCheckUpdate.Text = "Đang kiểm tra...";

            await UpdateManager.CheckForUpdateAsync(true);

            btnCheckUpdate.Text = "Kiểm tra cập nhật";
            btnCheckUpdate.Enabled = true;
        }

        private void chkAutoUpdate_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IsAutoUpdate = chkAutoUpdate.Checked;
            Properties.Settings.Default.Save();
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
