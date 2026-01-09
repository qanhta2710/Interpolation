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
                var p = BoundaryValueSolver.CompileFunction(txtEigenP.Text);
                var q = BoundaryValueSolver.CompileFunction(txtEigenQ.Text);
                var r = BoundaryValueSolver.CompileFunction(txtEigenR.Text);

                double a = double.Parse(txtEigenA.Text);
                double b = double.Parse(txtEigenB.Text);
                double h = double.Parse(txtEigenH.Text);

                EigenResult result = EigenvalueSolver.SolveSturmLiouville(p, q, r, a, b, h);

                rtbEigenLog.Text = result.Log;

                dgvEigen.Rows.Clear();
                dgvEigen.Columns.Clear();
                dgvEigen.Columns.Add("idx", "k");
                dgvEigen.Columns.Add("val", "Lambda (λ)");

                var lambdas = result.Eigenvalues;

                int minIdx = -1;
                int maxIdx = -1;
                double minVal = double.MaxValue;
                double maxVal = double.MinValue;

                for (int i = 0; i < lambdas.Count; i++)
                {
                    dgvEigen.Rows.Add(i + 1, Math.Round(lambdas[i], 6));

                    double absVal = Math.Abs(lambdas[i]);
                    if (absVal < minVal) { minVal = absVal; minIdx = i; }
                    if (absVal > maxVal) { maxVal = absVal; maxIdx = i; }
                }

                if (minIdx != -1) dgvEigen.Rows[minIdx].DefaultCellStyle.BackColor = Color.LightGreen;
                if (maxIdx != -1) dgvEigen.Rows[maxIdx].DefaultCellStyle.BackColor = Color.LightPink;

                DrawEigenChart(result, minIdx, maxIdx);

                tabControlEigenOutput.SelectedTab = tabEigenLog;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        private void DrawEigenChart(EigenResult result, int idxMin, int idxMax)
        {
            chartEigen.Series.Clear();
            chartEigen.Titles.Clear();
            chartEigen.Legends.Clear();

            var title = chartEigen.Titles.Add("Đồ thị Hàm riêng");
            title.Font = new Font("Consolas", 12, FontStyle.Bold);

            var legend = chartEigen.Legends.Add("Legend");
            legend.Docking = Docking.Right;
            legend.Font = new Font("Consolas", 9);

            var chartArea = chartEigen.ChartAreas[0];
            chartArea.AxisX.Title = "x";
            chartArea.AxisX.TitleFont = new Font("Consolas", 10, FontStyle.Bold);
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.Title = "u(x)";
            chartArea.AxisY.TitleFont = new Font("Consolas", 10, FontStyle.Bold);
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;

            // Vẽ hàm riêng gần 0 nhất
            if (idxMin != -1 && idxMin < result.Eigenfunctions.Count)
            {
                var seriesMin = new Series();
                seriesMin.Name = $"λ[{idxMin + 1}] = {result.Eigenvalues[idxMin]:F4} (gần 0)";
                seriesMin.ChartType = SeriesChartType.Spline;
                seriesMin.BorderWidth = 3;
                seriesMin.Color = Color.Green;

                double[] yData = result.Eigenfunctions[idxMin];
                for (int i = 0; i < result.X.Length; i++)
                {
                    seriesMin.Points.AddXY(result.X[i], yData[i]);
                }

                chartEigen.Series.Add(seriesMin);
            }

            // Vẽ hàm riêng xa 0 nhất
            if (idxMax != -1 && idxMax != idxMin && idxMax < result.Eigenfunctions.Count)
            {
                var seriesMax = new Series();
                seriesMax.Name = $"λ[{idxMax + 1}] = {result.Eigenvalues[idxMax]:F4} (xa 0)";
                seriesMax.ChartType = SeriesChartType.Line;
                seriesMax.BorderWidth = 2;
                seriesMax.Color = Color.Red;

                double[] yData = result.Eigenfunctions[idxMax];
                for (int i = 0; i < result.X.Length; i++)
                {
                    seriesMax.Points.AddXY(result.X[i], yData[i]);
                }

                chartEigen.Series.Add(seriesMax);
            }

            // Tự động điều chỉnh trục Y
            chartArea.RecalculateAxesScale();
        }
        private void DisplayTable(List<double[]> data)
        {
            dgvResult.Rows.Clear();
            dgvResult.Columns.Clear();
            dgvResult.Columns.Add("x", "x");
            dgvResult.Columns.Add("u", "u(x)");

            dgvResult.SuspendLayout();

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

            var title = chartResult.Titles.Add("Đồ thị nghiệm u(x)");
            title.Font = new Font("Consolas", 12, FontStyle.Bold);

            // Cấu hình trục
            var chartArea = chartResult.ChartAreas[0];
            chartArea.AxisX.Title = "x";
            chartArea.AxisX.TitleFont = new Font("Consolas", 10, FontStyle.Bold);
            chartArea.AxisY.Title = "u(x)";
            chartArea.AxisY.TitleFont = new Font("Consolas", 10, FontStyle.Bold);
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