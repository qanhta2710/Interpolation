using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Interpolation
{
    public partial class ODE : Form
    {
        public ODE()
        {
            InitializeComponent();

            cmbMethod.SelectedIndex = 0;
            cmbOrder.SelectedIndex = 1;
            UpdateSystemUI();
            UpdateHighOrderUI();
        }
        private void rdoSys1_CheckedChanged(object sender, EventArgs e) { UpdateSystemUI(); }
        private void rdoSys2_CheckedChanged(object sender, EventArgs e) { UpdateSystemUI(); }
        private void rdoSys3_CheckedChanged(object sender, EventArgs e) { UpdateSystemUI(); }
        private void cmbOrder_SelectedIndexChanged(object sender, EventArgs e) { UpdateHighOrderUI(); }

        private void UpdateSystemUI()
        {
            bool is2D = rdoSys2.Checked || rdoSys3.Checked;
            label6.Visible = is2D; txtFuncY.Visible = is2D;
            label9.Visible = is2D; txtInitY.Visible = is2D;
            bool is3D = rdoSys3.Checked;
            label7.Visible = is3D; txtFuncZ.Visible = is3D;
            label10.Visible = is3D; txtInitZ.Visible = is3D;
        }

        private void UpdateHighOrderUI()
        {
            int order = cmbOrder.SelectedIndex + 1;
            label12.Visible = (order >= 2); txtInitDy.Visible = (order >= 2);
            label13.Visible = (order >= 3); txtInitD2y.Visible = (order >= 3);
        }
        private void btnSolve_Click(object sender, EventArgs e)
        {
            try
            {
                int dim = 1;
                string[] strFuncs = null;
                double[] y0 = null;

                if (tabInput.SelectedTab == tabSystem)
                {
                    if (rdoSys1.Checked) dim = 1;
                    else if (rdoSys2.Checked) dim = 2;
                    else if (rdoSys3.Checked) dim = 3;

                    y0 = new double[dim];
                    strFuncs = new string[dim];
                    y0[0] = double.Parse(txtInitX.Text);
                    strFuncs[0] = txtFuncX.Text;
                    if (dim >= 2)
                    {
                        y0[1] = double.Parse(txtInitY.Text);
                        strFuncs[1] = txtFuncY.Text;
                    }
                    if (dim >= 3)
                    {
                        y0[2] = double.Parse(txtInitZ.Text);
                        strFuncs[2] = txtFuncZ.Text;
                    }
                }
                else
                {
                    int order = cmbOrder.SelectedIndex + 1;
                    dim = order;
                    y0 = new double[dim];

                    y0[0] = double.Parse(txtInitY_High.Text);
                    if (order >= 2) y0[1] = double.Parse(txtInitDy.Text);
                    if (order >= 3) y0[2] = double.Parse(txtInitD2y.Text);

                    strFuncs = new string[dim];
                    if (order == 1) strFuncs[0] = txtFuncHigh.Text.Replace("y", "x");
                    else if (order == 2) { strFuncs[0] = "y"; strFuncs[1] = ReplaceHighOrderVars(txtFuncHigh.Text, 2); }
                    else if (order == 3) { strFuncs[0] = "y"; strFuncs[1] = "z"; strFuncs[2] = ReplaceHighOrderVars(txtFuncHigh.Text, 3); }
                }

                var derivativeFunc = ODESolver.CompileSystem(strFuncs);
                double t0 = double.Parse(txtT0.Text);
                double tend = double.Parse(txtTend.Text);
                double h = double.Parse(txtH.Text);
                double epsilon = string.IsNullOrWhiteSpace(txtEpsilon.Text) ? 1e-6 : double.Parse(txtEpsilon.Text);

                List<double[]> results = null;
                int methodIndex = cmbMethod.SelectedIndex;

                if (methodIndex == 0)
                {
                    results = ODESolver.SolveEulerExplicit(derivativeFunc, y0, t0, tend, h); // Euler hiện
                }
                else if (methodIndex == 1)
                {
                    results = ODESolver.SolveEulerImplicit(derivativeFunc, y0, t0, tend, h, epsilon); // Euler ẩn
                }
                else if (methodIndex == 2)
                {
                    results = ODESolver.SolveTrapezoidal(derivativeFunc, y0, t0, tend, h, epsilon); // Hình thang
                }
                else if (methodIndex == 3)
                {
                    results = ODESolver.SolveRK2(derivativeFunc, y0, t0, tend, h); // RK2
                }
                else if (methodIndex == 4)
                {
                    results = ODESolver.SolveRK3(derivativeFunc, y0, t0, tend, h); // RK3
                }
                else if (methodIndex == 5)
                {
                    results = ODESolver.SolveRK4(derivativeFunc, y0, t0, tend, h); // RK4
                }

                DisplayTable(results, dim);

                GenerateReport(dim, strFuncs, y0, t0, tend, h, cmbMethod.SelectedItem.ToString());

                DrawGraph(results, dim);

                tabOutput.SelectedTab = tabLog;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void GenerateReport(int dim, string[] funcs, double[] y0, double t0, double tend, double h, string method)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("1. Đưa bài toán về dạng tổng quát:");
            sb.AppendLine("   u' = F(t, u), với u thuộc R^" + dim);
            sb.AppendLine("   Biến độc lập: t thuộc [" + t0 + ", " + tend + "]");
            sb.AppendLine();

            int N = (int)((tend - t0) / h);
            sb.AppendLine("2. Xác định lưới:");
            sb.AppendLine($"   Bước nhảy h = {h}");
            sb.AppendLine($"   Thời điểm đầu t0 = {t0}");
            sb.AppendLine($"   Các điểm nút: tk = t0 + k*h, k = 0...{N}");
            sb.AppendLine($"   (Tổng số bước tính: {N})");
            sb.AppendLine();

            sb.AppendLine("3. Xác định hàm vector F(t, u) và giá trị đầu u0:");
            sb.AppendLine("   Vector hàm F:");
            string[] vars = (tabInput.SelectedTab == tabSystem) ? new[] { "x", "y", "z" } : new[] { "y", "y'", "y''" };
            for (int i = 0; i < dim; i++)
            {
                sb.AppendLine($"     F[{i}] ({vars[i]}') = {funcs[i]}");
            }

            sb.AppendLine("   Vector giá trị đầu u(t0):");
            sb.Append("     u0 = [ ");
            foreach (var val in y0) sb.Append($"{val}  ");
            sb.AppendLine("]^T");
            sb.AppendLine();

            sb.AppendLine("4. Công thức giải số (" + method + "):");
            if (method == "Euler hiện")
            {
                sb.AppendLine("   u[k+1] = u[k] + h * F(t[k], u[k])");
            }
            else if (method == "Euler ẩn")
            {
                sb.AppendLine("   u* = u[k] + h * F(t[k], u[k])");
                sb.AppendLine("   CT Lặp: u[k+1] = u[k] + h * F(t[k+1], u*)");
            }
            else if (method == "Hình thang")
            {
                sb.AppendLine("   u* = u[k] + h * F(t[k], u[k])");
                sb.AppendLine("   CT Lặp: u[k+1] = u[k] + (h/2) * [F(t[k], u[k]) + F(t[k+1], u*)]");
            }
            else if (method.Contains("Runge-Kutta 2"))
            {
                sb.AppendLine("   k1 = h * F(t[k], u[k])");
                sb.AppendLine("   k2 = h * F(t[k] + h, u[k] + k1)");
                sb.AppendLine("   u[k+1] = u[k] + 0.5 * (k1 + k2)");
            }
            else if (method.Contains("Runge-Kutta 3"))
            {
                sb.AppendLine("   k1 = h * F(t[k], u[k])");
                sb.AppendLine("   k2 = h * F(t[k] + h/2, u[k] + k1/2)");
                sb.AppendLine("   k3 = h * F(t[k] + h, u[k] - k1 + 2k2)");
                sb.AppendLine("   u[k+1] = u[k] + (1/6) * (k1 + 4k2 + k3)");
            }
            else if (method.Contains("Runge-Kutta 4"))
            {
                sb.AppendLine("   k1 = h * F(t[k], u[k])");
                sb.AppendLine("   k2 = h * F(t[k] + h/2, u[k] + k1/2)");
                sb.AppendLine("   k3 = h * F(t[k] + h/2, u[k] + k2/2)");
                sb.AppendLine("   k4 = h * F(t[k] + h, u[k] + k3)");
                sb.AppendLine("   u[k+1] = u[k] + (1/6) * (k1 + 2k2 + 2k3 + k4)");
            }
            sb.AppendLine();
            sb.AppendLine("5. Bảng giá trị (Xem Tab 'Bảng kết quả')");
            sb.AppendLine("6. Đồ thị nghiệm (Xem Tab 'Đồ thị nghiệm')");

            rtbLog.Text = sb.ToString();
        }

        private void DrawGraph(List<double[]> data, int dim)
        {
            chartResult.Series.Clear();
            chartResult.Titles.Clear();
            var title = chartResult.Titles.Add("Đồ thị nghiệm u(t)");
            title.Font = new Font("Consolas", 12, FontStyle.Bold);
            var chartArea = chartResult.ChartAreas[0];
            chartArea.AxisX.Title = "t";
            chartArea.AxisX.TitleFont = new Font("Consolas", 10, FontStyle.Bold);
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.Title = "(x, y, z)";
            chartArea.AxisY.TitleFont = new Font("Consolas", 10, FontStyle.Bold);
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            string[] seriesNames;
            if (tabInput.SelectedTab == tabSystem)
                seriesNames = new[] { "x(t)", "y(t)", "z(t)" };
            else
                seriesNames = new[] { "y(t)", "y'(t)", "y''(t)" };

            for (int i = 0; i < dim; i++)
            {
                Series series = new Series(seriesNames[i]);
                series.ChartType = SeriesChartType.Line;
                series.BorderWidth = 2;
                chartResult.Series.Add(series);
            }
            int step = data.Count > 1000 ? data.Count / 1000 : 1;

            for (int i = 0; i < data.Count; i += step)
            {
                double t = data[i][0];
                for (int j = 0; j < dim; j++)
                {
                    chartResult.Series[j].Points.AddXY(t, data[i][j + 1]);
                }
            }
        }
        private void DisplayTable(List<double[]> data, int dim)
        {
            dgvResult.Rows.Clear();
            dgvResult.Columns.Clear();
            dgvResult.Columns.Add("t", "t (x_k)");

            string[] headers;
            if (tabInput.SelectedTab == tabSystem) headers = new[] { "u1 (x)", "u2 (y)", "u3 (z)" };
            else headers = new[] { "u1 (y)", "u2 (y')", "u3 (y'')" };

            for (int i = 0; i < dim; i++) dgvResult.Columns.Add("u" + i, headers[i]);

            dgvResult.SuspendLayout();
            int step = 1;

            for (int i = 0; i < data.Count; i += step)
            {
                if (i >= 3 && i < data.Count - 3)
                {
                    continue;
                }
                else
                {
                    object[] row = new object[dim + 1];
                    row[0] = Math.Round(data[i][0], 4);
                    for (int j = 0; j < dim; j++) row[j + 1] = Math.Round(data[i][j + 1], 6);
                    dgvResult.Rows.Add(row);
                }
            }
            dgvResult.ResumeLayout();
        }

        private string ReplaceHighOrderVars(string input, int order)
        {
            string temp = input.Replace(" ", "").ToLower();

            if (order >= 3)
            {
                temp = temp.Replace("y''", "##Z##");
                temp = temp.Replace("d2y", "##Z##");
            }
            if (order >= 2)
            {
                temp = temp.Replace("y'", "##Y##");
                temp = temp.Replace("dy", "##Y##");
            }
            temp = temp.Replace("y", "##X##");

            if (order == 1)
            {
                // Cấp 1: y -> x
                temp = temp.Replace("##X##", "x");
            }
            else if (order == 2)
            {
                // Cấp 2: y -> x, y' -> y
                temp = temp.Replace("##X##", "x");
                temp = temp.Replace("##Y##", "y");
            }
            else if (order == 3)
            {
                // Cấp 3: y -> x, y' -> y, y'' -> z
                temp = temp.Replace("##X##", "x");
                temp = temp.Replace("##Y##", "y");
                temp = temp.Replace("##Z##", "z");
            }
            return temp;
        }

    }
}