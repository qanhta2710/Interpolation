using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
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
            cmbMethod.SelectedIndexChanged += CmbMethod_SelectedIndexChanged;
            cmbAdamsOrder.SelectedIndexChanged += cmbAdamsOrder_SelectedIndexChanged;
            cmbMethod.SelectedIndex = 0;
            cmbOrder.SelectedIndex = 1;
            grpCustomRK.Visible = false;
            grpCustomRK3.Visible = false;
            UpdateSystemUI();
            UpdateHighOrderUI();
        }
        private void CmbMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = cmbMethod.SelectedItem != null ? cmbMethod.SelectedItem.ToString() : "";
            bool isAdams = selected.Contains("ABs-AMs");

            lblAdamsS.Visible = isAdams;
            cmbAdamsOrder.Visible = isAdams;

            // Xử lý RK Tùy chỉnh
            bool isCustomRK = selected.Contains("Runge-Kutta 2 (Custom)");
            grpCustomRK.Visible = isCustomRK;

            bool isCustomRK3 = selected.Contains("Runge-Kutta 3 (Custom)");
            grpCustomRK3.Visible = isCustomRK3;
        }
        private void cmbAdamsOrder_SelectedIndexChanged(object sender, EventArgs e) { }
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
                else if (methodIndex == 6 || cmbMethod.Text.Contains("ABs-AMs")) // Adams
                {
                    int s = int.Parse(cmbAdamsOrder.SelectedItem.ToString());
                    results = ODESolver.SolveAdams(derivativeFunc, y0, t0, tend, h, s, epsilon);
                }
                else if (cmbMethod.Text.Contains("Runge-Kutta 2 (Custom)"))
                {
                    string typeStr = cmbRKParamType.SelectedItem.ToString();
                    ODESolver.RK2ParamType pType;

                    if (typeStr == "Alpha2") pType = ODESolver.RK2ParamType.Alpha2;
                    else if (typeStr == "Beta11") pType = ODESolver.RK2ParamType.Beta11;
                    else if (typeStr == "R1") pType = ODESolver.RK2ParamType.R1;
                    else pType = ODESolver.RK2ParamType.R2;

                    double val = ParseFraction(txtRKParamValue.Text);
                    results = ODESolver.SolveCustomRK2(derivativeFunc, y0, t0, tend, h, pType, val);
                }
                else if (cmbMethod.Text.Contains("Runge-Kutta 3 (Custom)"))
                {
                    double alpha2 = ParseFraction(txtAlpha2_RK3.Text);
                    double alpha3 = ParseFraction(txtAlpha3_RK3.Text);

                    results = ODESolver.SolveCustomRK3(derivativeFunc, y0, t0, tend, h, alpha2, alpha3);
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
            else if (method.Contains("Runge-Kutta 2 (Custom)"))
            {
                double val = ParseFraction(txtRKParamValue.Text);
                string paramType = cmbRKParamType.Text;

                double r1 = 0, r2 = 0, alpha2 = 0, beta11 = 0;

                if (paramType == "Alpha2")
                {
                    alpha2 = val;
                    beta11 = alpha2;
                    r2 = 1.0 / (2.0 * alpha2);
                    r1 = 1.0 - r2;
                }
                else if (paramType == "Beta11")
                {
                    beta11 = val;
                    alpha2 = beta11;
                    r2 = 1.0 / (2.0 * alpha2);
                    r1 = 1.0 - r2;
                }
                else if (paramType == "R2")
                {
                    r2 = val;
                    r1 = 1.0 - r2;
                    alpha2 = 1.0 / (2.0 * r2);
                    beta11 = alpha2;
                }
                else if (paramType == "R1")
                {
                    r1 = val;
                    r2 = 1.0 - r1;
                    if (r2 != 0)
                    {
                        alpha2 = 1.0 / (2.0 * r2);
                        beta11 = alpha2;
                    }
                }

                sb.AppendLine($"   Phương pháp RK2 Tùy chỉnh (Input: {paramType} = {val:F4})");
                sb.AppendLine();

                sb.AppendLine("   a) Hệ phương trình ràng buộc các hệ số:");
                sb.AppendLine("      (1) r1 + r2 = 1");
                sb.AppendLine("      (2) r2 * alpha2 = 1/2");
                sb.AppendLine("      (3) r2 * beta11 = 1/2");
                sb.AppendLine();

                sb.AppendLine("   b) Các hệ số tính được:");
                sb.AppendLine($"      r1      = {r1:F6}");
                sb.AppendLine($"      r2      = {r2:F6}");
                sb.AppendLine($"      alpha2  = {alpha2:F6}");
                sb.AppendLine($"      beta11  = {beta11:F6}");
                sb.AppendLine();

                sb.AppendLine("   c) Công thức thực hiện:");
                sb.AppendLine("      k1 = h * F(t[k], u[k])");
                sb.AppendLine($"      k2 = h * F(t[k] + {alpha2:F4}h, u[k] + {beta11:F4}k1)");
                sb.AppendLine($"      u[k+1] = u[k] + {r1:F4}k1 + {r2:F4}k2");
            }
            else if (method.Contains("Runge-Kutta 3 (Custom)"))
            {
                double alpha2 = ParseFraction(txtAlpha2_RK3.Text);
                double alpha3 = ParseFraction(txtAlpha3_RK3.Text);

                sb.AppendLine($"   Phương pháp RK3 Tùy chỉnh (Input: α2={alpha2:F4}, α3={alpha3:F4})");
                sb.AppendLine();

                sb.AppendLine("   a) Hệ phương trình ràng buộc các hệ số:");
                sb.AppendLine("      (1) r1 + r2 + r3 = 1");
                sb.AppendLine("      (2) r2*α2 + r3*α3 = 1/2");
                sb.AppendLine("      (3) r2*β11 + r3*(β21 + β22) = 1/2");
                sb.AppendLine("      (4) 1/2*r2*α2² + 1/2*r3*α3² = 1/6");
                sb.AppendLine("      (5) r2*α2*β11 + r3*α3*(β21 + β22) = 1/3");
                sb.AppendLine("      (6) r2*β11² + r3*(β21 + β22)² = 1/3");
                sb.AppendLine("      (7) r3*β22*α2 = 1/6");
                sb.AppendLine("      (8) r3*β11*β22 = 1/6");
                sb.AppendLine();

                sb.AppendLine("   b) Rút gọn và giải hệ:");
                sb.AppendLine("      Áp dụng điều kiện: β11 = α2 và (β21 + β22) = α3");
                sb.AppendLine("      Ta thu được hệ phương trình để tìm r2, r3:");
                sb.AppendLine($"      (I)   {alpha2:F4}*r2 + {alpha3:F4}*r3 = 0.5");
                sb.AppendLine($"      (II)  {alpha2 * alpha2:F4}*r2 + {alpha3 * alpha3:F4}*r3 = 0.3333");
                sb.AppendLine();

                double r2 = (3 * alpha3 - 2) / (6 * alpha2 * (alpha3 - alpha2));
                double r3 = (2 - 3 * alpha2) / (6 * alpha3 * (alpha3 - alpha2));
                double r1 = 1 - r2 - r3;
                double beta11 = alpha2;
                double beta22 = 1.0 / (6 * r3 * alpha2);
                double beta21 = alpha3 - beta22;

                sb.AppendLine($"   Phương pháp RK3 Tùy chỉnh (Alpha2={alpha2}, Alpha3={alpha3})");
                sb.AppendLine("   a) Hệ số tính được:");
                sb.AppendLine($"      r1={r1:F4}, r2={r2:F4}, r3={r3:F4}");
                sb.AppendLine($"      beta11={beta11:F4}, beta21={beta21:F4}, beta22={beta22:F4}");
                sb.AppendLine();
                sb.AppendLine("   b) Công thức:");
                sb.AppendLine("      k1 = h * f(t, y)");
                sb.AppendLine($"      k2 = h * f(t + {alpha2:F4}h, y + {beta11:F4}k1)");
                sb.AppendLine($"      k3 = h * f(t + {alpha3:F4}h, y + {beta21:F4}k1 + {beta22:F4}k2)");
                sb.AppendLine($"      y_new = y + {r1:F4}k1 + {r2:F4}k2 + {r3:F4}k3");
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
            else if (method.Contains("ABs-AMs"))
            {
                // Lấy bậc s từ ComboBox
                int s = int.Parse(cmbAdamsOrder.SelectedItem.ToString());

                sb.AppendLine($"   Phương pháp đa bước Adams-Bashforth-Moulton (s = {s})");
                sb.AppendLine();

                // a) Khởi tạo
                sb.AppendLine("   a) Khởi tạo:");
                sb.AppendLine($"      Dùng RK4 tính {s - 1} điểm đầu tiên: u[0] ... u[{s - 1}]");
                sb.AppendLine();

                // b) Hệ số Beta (Dự báo)
                double[] beta = ODESolver.GetAdamsBashforthCoeffs(s);
                sb.AppendLine("   b) Hệ số Beta (Adams-Bashforth - Dự báo):");
                sb.AppendLine("      ─────────────────────────────────────────");
                for (int j = 0; j < beta.Length; j++)
                {
                    // In căn lề đẹp: Index 2 số, Giá trị 15 ký tự, 10 số lẻ
                    sb.AppendLine($"      β[{j}] = {beta[j],15:F10}");
                }
                sb.AppendLine();

                // c) Công thức dự báo
                sb.AppendLine("   c) Công thức dự báo (Adams-Bashforth):");
                sb.Append("      u_p = u[n] + h * (");
                List<string> betaTerms = new List<string>();
                for (int j = 0; j < beta.Length; j++)
                {
                    // Xử lý dấu + - cho đẹp
                    string valStr = Math.Abs(beta[j]).ToString("F6");
                    string sign = beta[j] < 0 ? "- " : (j == 0 ? "" : "+ ");

                    // F[n] với j=0, F[n-1] với j=1...
                    string indexStr = (j == 0) ? "n" : $"n-{j}";

                    betaTerms.Add($"{sign}{valStr}*F[{indexStr}]");
                }
                sb.Append(string.Join(" ", betaTerms));
                sb.AppendLine(")");
                sb.AppendLine();

                // d) Hệ số Gamma (Hiệu chỉnh)
                double[] gamma = ODESolver.GetAdamsMoultonCoeffs(s);
                sb.AppendLine("   d) Hệ số Gamma (Adams-Moulton - Hiệu chỉnh):");
                sb.AppendLine("      ─────────────────────────────────────────");
                for (int j = 0; j < gamma.Length; j++)
                {
                    sb.AppendLine($"      γ[{j}] = {gamma[j],15:F10}");
                }
                sb.AppendLine();

                // e) Công thức hiệu chỉnh
                sb.AppendLine("   e) Công thức hiệu chỉnh (Adams-Moulton):");
                sb.Append("      u[n+1] = u[n] + h * (");
                List<string> gammaTerms = new List<string>();

                // Phần tử đầu tiên (j=0) là F tương lai: F(t[n+1], u_p)
                string valG0 = Math.Abs(gamma[0]).ToString("F6");
                gammaTerms.Add($"{valG0}*F_p");

                // Các phần tử sau (j=1..s-1) là F quá khứ: F[n], F[n-1]...
                for (int j = 1; j < gamma.Length; j++)
                {
                    string valStr = Math.Abs(gamma[j]).ToString("F6");
                    string sign = gamma[j] < 0 ? "- " : "+ ";

                    // j=1 -> F[n], j=2 -> F[n-1] ... => công thức n-(j-1)
                    int offset = j - 1;
                    string indexStr = (offset == 0) ? "n" : $"n-{offset}";

                    gammaTerms.Add($"{sign}{valStr}*F[{indexStr}]");
                }
                sb.Append(string.Join(" ", gammaTerms));
                sb.AppendLine(")");
                sb.AppendLine("      (Với F_p = F(t[n+1], u_p))");
                sb.AppendLine();
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
                object[] row = new object[dim + 1];
                row[0] = Math.Round(data[i][0], 4);
                for (int j = 0; j < dim; j++) row[j + 1] = Math.Round(data[i][j + 1], 6);
                dgvResult.Rows.Add(row);
            }

            int totalRows = dgvResult.Rows.Count;
            Color highlightColor = Color.LightYellow; 

            for (int i = 0; i < Math.Min(3, totalRows); i++)
            {
                dgvResult.Rows[i].DefaultCellStyle.BackColor = highlightColor;
            }

            for (int i = Math.Max(0, totalRows - 3); i < totalRows; i++)
            {
                dgvResult.Rows[i].DefaultCellStyle.BackColor = highlightColor;
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
        private double ParseFraction(string input)
        {
            try
            {
                return Convert.ToDouble(new System.Data.DataTable().Compute(input, null));
            }
            catch
            {
                return double.Parse(input); 
            }
        }
    }
}