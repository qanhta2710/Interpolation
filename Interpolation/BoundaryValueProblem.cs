using Interpolation.Methods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Interpolation
{
    public partial class BoundaryValueProblem : Form
    {
        public BoundaryValueProblem()
        {
            InitializeComponent();
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Lấy hàm p, q, f
                var p = BoundaryValueSolver.CompileFunction(txtP.Text);
                var q = BoundaryValueSolver.CompileFunction(txtQ.Text);
                var f = BoundaryValueSolver.CompileFunction(txtF.Text);

                // 2. Lấy tham số lưới
                double a = double.Parse(txtStart.Text);
                double b = double.Parse(txtEnd.Text);
                double h = double.Parse(txtH.Text);

                // 3. Lấy điều kiện biên Trái (tại a)
                var leftBC = new BoundaryValueSolver.BoundaryCondition
                {
                    Alpha = double.Parse(txtAlpha1.Text),
                    Beta = double.Parse(txtBeta1.Text),
                    Gamma = double.Parse(txtGamma1.Text)
                };

                // 4. Lấy điều kiện biên Phải (tại b)
                var rightBC = new BoundaryValueSolver.BoundaryCondition
                {
                    Alpha = double.Parse(txtAlpha2.Text),
                    Beta = double.Parse(txtBeta2.Text),
                    Gamma = double.Parse(txtGamma2.Text)
                };

                // 5. Gọi Solver
                var result = BoundaryValueSolver.SolveBVP(p, q, f, a, b, h, leftBC, rightBC);

                // 6. Hiển thị bảng
                DisplayTable(result.DataPoints);

                // 7. Hiển thị log
                rtbLog.Text = result.Log;

                // 8. Vẽ đồ thị
                DrawGraph(result.DataPoints);
                lblMaxMin.Text = $"GTLN: u({result.MaxX:F4}) = {result.MaxVal:F6}\n" +
                                 $"GTNN: u({result.MinX:F4}) = {result.MinVal:F6}";

                tabControlOutput.SelectedTab = tabLog;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        private void btnSolveEigen_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Lấy dữ liệu (Tận dụng p, q từ Tab BVP)
                var p = BoundaryValueSolver.CompileFunction(txtEigenP.Text);
                var q = BoundaryValueSolver.CompileFunction(txtEigenQ.Text);
                var r = BoundaryValueSolver.CompileFunction(txtEigenR.Text);

                double a = double.Parse(txtEigenA.Text);
                double b = double.Parse(txtEigenB.Text);
                double h = double.Parse(txtEigenH.Text);

                var leftBC = new BoundaryValueSolver.BoundaryCondition
                {
                    Alpha = double.Parse(txtEigenAlpha1.Text),
                    Beta = double.Parse(txtEigenBeta1.Text),
                    Gamma = 0
                };

                var rightBC = new BoundaryValueSolver.BoundaryCondition
                {
                    Alpha = double.Parse(txtEigenAlpha2.Text),
                    Beta = double.Parse(txtEigenBeta2.Text),
                    Gamma = 0
                };

                // 2. Gọi Solver
                EigenResult result = EigenvalueSolver.SolveSturmLiouville(p, q, r, a, b, h, leftBC, rightBC);

                // 3. Hiển thị
                dgvEigen.Rows.Clear();
                dgvEigen.Columns.Clear();
                dgvEigen.Columns.Add("idx", "k");
                dgvEigen.Columns.Add("val", "Lambda (Trị riêng)");

                var lambdas = result.Eigenvalues;
                for (int i = 0; i < lambdas.Count; i++)
                {
                    // Lọc bớt giá trị nhiễu quá lớn do thuật toán số
                    if (Math.Abs(lambdas[i]) < 1e6)
                    {
                        dgvEigen.Rows.Add(i + 1, Math.Round(lambdas[i], 6));
                    }
                }

                // 4. Hiển thị Log
                rtbEigenLog.Text = result.Log;

                // Tự động chuyển sang tab Log để người dùng xem thông tin trước
                tabControlEigenOutput.SelectedTab = tabEigenLog;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void DisplayTable(List<double[]> data)
        {
            dgvResult.Rows.Clear();
            dgvResult.Columns.Clear();
            dgvResult.Columns.Add("x", "x");
            dgvResult.Columns.Add("u", "u(x)");

            dgvResult.SuspendLayout();

            // Giới hạn hiển thị nếu quá nhiều dòng (tránh treo máy)
            int step = data.Count > 1000 ? data.Count / 1000 : 1;

            for (int i = 0; i < data.Count; i += step)
            {
                if (i >= 3 && i < data.Count - 3)
                {
                    continue;
                }
                else
                {
                    object[] row = { Math.Round(data[i][0], 4), Math.Round(data[i][1], 6) };
                    dgvResult.Rows.Add(row);
                }
            }

            dgvResult.ResumeLayout();
        }

        private void DrawGraph(List<double[]> data)
        {
            chartResult.Series[0].Points.Clear();
            chartResult.Titles.Clear();
            chartResult.Titles.Add("Đồ thị nghiệm u(x)");

            // Cấu hình trục
            var chartArea = chartResult.ChartAreas[0];
            chartArea.AxisX.Title = "x";
            chartArea.AxisY.Title = "u(x)";
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;

            // Vẽ
            int step = data.Count > 500 ? data.Count / 500 : 1;
            for (int i = 0; i < data.Count; i += step)
            {
                chartResult.Series[0].Points.AddXY(data[i][0], data[i][1]);
            }
        }
    }
}