using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Interpolation
{
    public partial class Form1 : Form
    {
        private double[] lastPolynomialCoeffs;
        private double[] xValues;
        private double[] yValues;
        private double lastYTarget;
        private List<(int startIndex, int endIndex)> lastIsolationIntervals;
        private List<(int startIndex, int endIndex, bool isIncreasing, double[] selectedX, double[] selectedY)> lastMonotonicIntervals;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxNewton.SelectedIndex = 0; // Lựa chọn mặc định mốc nội suy bất kì
            comboBoxNewtonFinite.SelectedIndex = 0; // Lựa chọn mặc định mốc nội suy cách đều tăng dần
            comboBoxLeastSquares.SelectedIndex = 0; // Lựa chọn mặc định phương pháp bình phương tối thiểu tuyến tính
            SetupDataGridViewColumnTypes();
        }
        #region Event Handlers
        // Tìm mốc nội suy Chebyshev
        private void btnSolveChebyshev_Click(object sender, EventArgs e)
        {
            try
            {
                double a = Convert.ToDouble(txtBoxa.Text);
                double b = Convert.ToDouble(txtBoxb.Text);
                int n = Convert.ToInt32(txtBoxn.Text);
                int precision = Convert.ToInt32(txtBoxPrecision.Text);

                double[] res = InterpolationPoints.FindOptimizedPoints(n, a, b, precision);

                string cellRes = string.Join(", ", res);
                dataChebyshev.Rows.Add(a, b, n, cellRes);
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi định dạng");
            }
        }
        // Tìm đa thức nội suy bằng Lagrange
        private void btnSolveLagrange_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Xử lý Input
                dataGridViewLagrange.Rows.Clear();
                dataGridViewLagrange.Columns.Clear();
                RemoveDuplicate(dataXYLagrange);

                double[] x = GetXValues(dataXYLagrange);
                double[] y = GetYValues(dataXYLagrange);
                int precision = Convert.ToInt32(txtBoxPrecisionLagrange.Text);

                // Tìm và in đa thức nội suy
                var lagrangePolynomial = SolveLagrange(x, y, precision, out double[] coeffsD, out double[,] productTable, out double[,] divideTable, out double[,] arrMatrix);
                lastPolynomialCoeffs = lagrangePolynomial;
                DisplayLagrangeResults(dataGridViewLagrange, x.Length, coeffsD, productTable, divideTable, lagrangePolynomial, arrMatrix);

                // In ra đa thức nội suy dạng chính tắc
                lblResult.Text = Function.PolynomialToString(lagrangePolynomial);
                lblResult.Visible = true;
                btnLagrangeToEval.Visible = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi định dạng");
            }
        }
        private void btnOpenExcelLagrange_Click(object sender, EventArgs e)
        {
            ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                    openFileDialog.Title = "Chọn file Excel chứa dữ liệu (x, y)";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        ReadExcelData(filePath, out double[] xValues, out double[] yValues);
                        dataXYLagrange.Rows.Clear();
                        for (int i = 0; i < xValues.Length; i++)
                        {
                            dataXYLagrange.Rows.Add(xValues[i], yValues[i]);
                        }

                        MessageBox.Show(
                            $"Đã nhập thành công {xValues.Length} điểm dữ liệu từ Excel!",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}",
                      "Lỗi",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
        }

        // Tìm đa thức nội suy bằng Newton
        private void btnSolveNewton_Click(object sender, EventArgs e)
        {
            try
            {
                // Xử lý Input
                dataGridViewNewton.Rows.Clear();
                dataGridViewNewton.Columns.Clear();

                if (comboBoxNewton.SelectedIndex == 0)
                {
                    // Mốc nội suy bất kì
                    MessageBox.Show("Giữ nguyên thứ tự mốc nội suy");
                }
                else if (comboBoxNewton.SelectedIndex == 1)
                {
                    // Mốc nội suy tăng dần
                    dataXYNewton.Sort(dataXYNewton.Columns["colsXNewton"], System.ComponentModel.ListSortDirection.Ascending);
                    MessageBox.Show("Sắp xếp mốc nội suy tăng dần");
                }
                else if (comboBoxNewton.SelectedIndex == 2)
                {
                    // Mốc nội suy giảm dần
                    dataXYNewton.Sort(dataXYNewton.Columns["colsXNewton"], System.ComponentModel.ListSortDirection.Descending);
                    MessageBox.Show("Sắp xếp mốc nội suy giảm dần");
                }

                RemoveDuplicate(dataXYNewton);

                double[] x = GetXValues(dataXYNewton);
                double[] y = GetYValues(dataXYNewton);
                int precision = Convert.ToInt32(txtBoxPrecisionNewton.Text);

                // Tìm và in đa thức nội suy
                var newtonPolynomial = SolveNewton(x, y, precision, out double?[,] diffTable, out double[,] productTable, out double[] dividedDiagonalDiff);
                lastPolynomialCoeffs = newtonPolynomial;
                DisplayNewtonResults(dataGridViewNewton, x.Length, diffTable, dividedDiagonalDiff, productTable, newtonPolynomial);

                // In đa thức nội suy
                lblResultNewton.Text = Function.PolynomialToString(newtonPolynomial);
                lblResultNewton.Visible = true;
                btnNewtonToEval.Visible = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi định dạng");
            }
        }
        private void btnOpenExcelNewton_Click(object sender, EventArgs e)
        {
            ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                    openFileDialog.Title = "Chọn file Excel chứa dữ liệu (x, y)";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        ReadExcelData(filePath, out double[] xValues, out double[] yValues);
                        dataXYNewton.Rows.Clear();
                        for (int i = 0; i < xValues.Length; i++)
                        {
                            dataXYNewton.Rows.Add(xValues[i], yValues[i]);
                        }

                        MessageBox.Show(
                            $"Đã nhập thành công {xValues.Length} điểm dữ liệu từ Excel!",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}",
                      "Lỗi",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
        }
        // Tìm đa thức nội suy Newton mốc cách đều
        private void btnSolveNewtonFinite_Click(object sender, EventArgs e)
        {
            try
            {
                // Xử lý Input
                if (comboBoxNewtonFinite.SelectedIndex == 0)
                {
                    dataGridViewXYNewtonFinite.Sort(dataGridViewXYNewtonFinite.Columns["colsXNewtonFinite"], System.ComponentModel.ListSortDirection.Ascending);
                    MessageBox.Show("Nội suy Newton tiến");
                }
                else if (comboBoxNewtonFinite.SelectedIndex == 1)
                {
                    MessageBox.Show("Nội suy Newton lùi");
                }
                MessageBox.Show(
                "Chương trình sẽ sử dụng phép đổi biến:\n\n" +
                "    t = (x - x_0) / h\n\n" +
                "Đa thức nội suy là: f(t)\n\n" +
                "Kết quả hiển thị theo biến t",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
                );
                RemoveDuplicate(dataGridViewXYNewtonFinite);

                double[] x = GetXValues(dataGridViewXYNewtonFinite);
                double[] y = GetYValues(dataGridViewXYNewtonFinite);
                int precision = Convert.ToInt32(txtBoxPrecisionNewtonFinite.Text);
                bool isForward = comboBoxNewtonFinite.SelectedIndex == 0;

                // Tìm và in đa thức nội suy
                var newtonPolynomial = SolveNewtonFinite(x, y, precision, isForward, out double?[,] finiteDiffTable, out double[,] productTable, out double[] dividedDiagonalDiff);
                lastPolynomialCoeffs = newtonPolynomial;
                DisplayNewtonResults(dataGridViewNewtonFinite, x.Length, finiteDiffTable, dividedDiagonalDiff, productTable, newtonPolynomial);

                // Viết đa thức nội suy dạng chính tắc
                lblNewtonFinite.Text = Function.PolynomialToString(newtonPolynomial, "t");
                lblNewtonFinite.Visible = true;
                btnNewtonFiniteToEval.Visible = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi định dạng");
            }
        }
        private void btnOpenExcelNewtonFinite_Click(object sender, EventArgs e)
        {
            ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                    openFileDialog.Title = "Chọn file Excel chứa dữ liệu (x, y)";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        ReadExcelData(filePath, out double[] xValues, out double[] yValues);
                        dataGridViewXYNewtonFinite.Rows.Clear();
                        for (int i = 0; i < xValues.Length; i++)
                        {
                            dataGridViewXYNewtonFinite.Rows.Add(xValues[i], yValues[i]);
                        }

                        MessageBox.Show(
                            $"Đã nhập thành công {xValues.Length} điểm dữ liệu từ Excel!",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}",
                      "Lỗi",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
        }
        // Tính giá trị đa thức và các đạo hàm tại 1 điểm bằng phương pháp Horner
        private void btnEval_Click(object sender, EventArgs e)
        {
            try
            {
                // Xử lý Input
                double[] coeffsP = GetCoeffsP(dataGridViewCoeffsP);
                int precision = Convert.ToInt32(txtBoxPrecisionEval.Text);
                double c = Convert.ToDouble(txtBoxC.Text);
                int k = Convert.ToInt32(txtBoxk.Text);
                int n = coeffsP.Length - 1;

                // Tính và in kết quả
                HornerResults result = SolveHorner(coeffsP, c, k, precision);
                DisplayHornerResults(result, c);
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi định dạng");
            }
        }

        // Tìm đa thức nội suy trung tâm bằng Stirling
        private void btnSolveStirling_Click(object sender, EventArgs e)
        {
            try
            {
                // Xử lý Input
                dataXYStirling.Sort(dataXYStirling.Columns["colsXStirling"], System.ComponentModel.ListSortDirection.Ascending);
                RemoveDuplicate(dataXYStirling);

                double[] x = GetXValues(dataXYStirling);
                if (x.Length % 2 == 0)
                {
                    MessageBox.Show("Số nút nội suy chẵn vui lòng chọn nội suy Bessel");
                    return;
                }
                MessageBox.Show(
                "Chương trình sẽ sử dụng phép đổi biến:\n\n" +
                "    t = (x - x_0) / h\n\n" +
                "Đa thức nội suy là: f(t)\n\n" +
                "Kết quả hiển thị theo biến t",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
                );
                double[] y = GetYValues(dataXYStirling);
                int precision = Convert.ToInt32(txtboxPrecisionStirling.Text);

                var StirlingPolynomial = SolveStirling(x, y, precision, out double?[,] diffTable, out double[,] prodTable, out double[] vectorCoeffs);
                lastPolynomialCoeffs = StirlingPolynomial;
                DisplayCentralResultsStirling(dataResultStirling, x.Length, diffTable, vectorCoeffs, prodTable, StirlingPolynomial);

                lblStirling.Text = Function.PolynomialToString(StirlingPolynomial, "t");
                lblStirling.Visible = true;
                btnStirlingToEval.Visible = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi định dạng");
            }
        }
        private void btnOpenExcelStirling_Click(object sender, EventArgs e)
        {
            ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                    openFileDialog.Title = "Chọn file Excel chứa dữ liệu (x, y)";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        ReadExcelData(filePath, out double[] xValues, out double[] yValues);
                        dataXYStirling.Rows.Clear();
                        for (int i = 0; i < xValues.Length; i++)
                        {
                            dataXYStirling.Rows.Add(xValues[i], yValues[i]);
                        }

                        MessageBox.Show(
                            $"Đã nhập thành công {xValues.Length} điểm dữ liệu từ Excel!",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}",
                      "Lỗi",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
        }
        // Tìm đa thức nội suy trung tâm bằng Bessel
        private void btnSolveBessel_Click(object sender, EventArgs e)
        {
            try
            {
                dataXYBessel.Sort(dataXYBessel.Columns["colsXBessel"], System.ComponentModel.ListSortDirection.Ascending);
                RemoveDuplicate(dataXYBessel);

                double[] x = GetXValues(dataXYBessel);
                if (x.Length % 2 != 0)
                {
                    MessageBox.Show("Số nút nội suy lẻ vui lòng chọn nội suy Stirling");
                    return;
                }
                MessageBox.Show(
                "Chương trình sẽ sử dụng phép đổi biến:\n\n" +
                "    t = ( (x - x_0) / h ) - 0.5\n\n" +
                "Đa thức nội suy là: f(t)\n\n" +
                "Kết quả hiển thị theo biến t",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
                );
                double[] y = GetYValues(dataXYBessel);
                int precision = Convert.ToInt32(txtboxPrecisionBessel.Text);

                var BesselPolynomial = SolveBessel(x, y, precision, out double?[,] diffTable, out double[,] prodTable, out double[] vectorCoeffs);
                lastPolynomialCoeffs = BesselPolynomial;
                DisplayCentralResultsBessel(dataResultBessel, x.Length, diffTable, vectorCoeffs, prodTable, BesselPolynomial);

                lblResultBessel.Text = Function.PolynomialToString(BesselPolynomial, "t");
                lblResultBessel.Visible = true;
                btnBesselToEval.Visible = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi định dạng");
            }
        }
        private void btnOpenExcelBessel_Click(object sender, EventArgs e)
        {
            ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                    openFileDialog.Title = "Chọn file Excel chứa dữ liệu (x, y)";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        ReadExcelData(filePath, out double[] xValues, out double[] yValues);
                        dataXYBessel.Rows.Clear();
                        for (int i = 0; i < xValues.Length; i++)
                        {
                            dataXYBessel.Rows.Add(xValues[i], yValues[i]);
                        }

                        MessageBox.Show(
                            $"Đã nhập thành công {xValues.Length} điểm dữ liệu từ Excel!",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}",
                      "Lỗi",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
        }
        // Tìm đa thức nội suy trung tâm bằng Gauss I
        private void btnSolveGaussI_Click(object sender, EventArgs e)
        {
            try
            {
                // Xử lý Input
                dataXYGaussI.Sort(dataXYGaussI.Columns["colsXGaussI"], System.ComponentModel.ListSortDirection.Ascending);
                RemoveDuplicate(dataXYGaussI);

                double[] x = GetXValues(dataXYGaussI);
                double[] y = GetYValues(dataXYGaussI);
                int precision = Convert.ToInt32(txtBoxPrecisionGaussI.Text);

                MessageBox.Show(
                    "Công thức Gauss I (Forward):\n\n" +
                    "- Phép đổi biến: t = (x - x_0) / h\n" +
                    "- Đa thức: f(t)\n\n" +
                    "Kết quả hiển thị theo biến t",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                var gaussIPolynomial = SolveGaussI(x, y, precision,
                    out double?[,] diffTable,
                    out double[,] prodTable,
                    out double[] vectorCoeffs);

                lastPolynomialCoeffs = gaussIPolynomial;
                DisplayGaussIResults(dataResultGaussI, x.Length, diffTable, vectorCoeffs, prodTable, gaussIPolynomial);

                lblResultGaussI.Text = Function.PolynomialToString(gaussIPolynomial, "t");
                lblResultGaussI.Visible = true;
                btnGaussIToEval.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }
        private void btnOpenExcelGaussI_Click(object sender, EventArgs e)
        {
            ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                    openFileDialog.Title = "Chọn file Excel chứa dữ liệu (x, y)";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        ReadExcelData(filePath, out double[] xValues, out double[] yValues);
                        dataXYGaussI.Rows.Clear();
                        for (int i = 0; i < xValues.Length; i++)
                        {
                            dataXYGaussI.Rows.Add(xValues[i], yValues[i]);
                        }

                        MessageBox.Show(
                            $"Đã nhập thành công {xValues.Length} điểm dữ liệu từ Excel!",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}",
                      "Lỗi",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
        }
        // Tìm đa thức nội suy trung tâm bằng Gauss II
        private void btnSolveGaussII_Click(object sender, EventArgs e)
        {
            try
            {
                // Xử lý Input
                dataXYGaussII.Sort(dataXYGaussII.Columns["colsXGaussII"], System.ComponentModel.ListSortDirection.Ascending);
                RemoveDuplicate(dataXYGaussII);

                double[] x = GetXValues(dataXYGaussII);
                double[] y = GetYValues(dataXYGaussII);
                int precision = Convert.ToInt32(txtBoxPrecisionGaussII.Text);

                MessageBox.Show(
                    "Công thức Gauss II:\n\n" +
                    "- Phép đổi biến: t = (x - x_0) / h\n" +
                    "- Đa thức: f(t)\n\n" +
                    "Kết quả hiển thị theo biến t",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                var gaussIIPolynomial = SolveGaussII(x, y, precision,
                    out double?[,] diffTable,
                    out double[,] prodTable,
                    out double[] vectorCoeffs);

                lastPolynomialCoeffs = gaussIIPolynomial;
                DisplayGaussIIResults(dataResultGaussII, x.Length, diffTable, vectorCoeffs, prodTable, gaussIIPolynomial);

                lblResultGaussII.Text = Function.PolynomialToString(gaussIIPolynomial, "t");
                lblResultGaussII.Visible = true;
                btnGaussIIToEval.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }
        private void btnOpenExcelGaussII_Click(object sender, EventArgs e)
        {
            ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                    openFileDialog.Title = "Chọn file Excel chứa dữ liệu (x, y)";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        ReadExcelData(filePath, out double[] xValues, out double[] yValues);
                        dataXYGaussII.Rows.Clear();
                        for (int i = 0; i < xValues.Length; i++)
                        {
                            dataXYGaussII.Rows.Add(xValues[i], yValues[i]);
                        }

                        MessageBox.Show(
                            $"Đã nhập thành công {xValues.Length} điểm dữ liệu từ Excel!",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}",
                      "Lỗi",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
        }
        // Phương pháp lặp
        private void btnSolveIteration_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridViewXYIteration.Sort(dataGridViewXYIteration.Columns["colsXIteration"], System.ComponentModel.ListSortDirection.Ascending);
                RemoveDuplicate(dataGridViewXYIteration);

                double[] x = GetXValues(dataGridViewXYIteration);
                double[] y = GetYValues(dataGridViewXYIteration);
                double yTarget = Convert.ToDouble(txtBoxYAvg.Text);
                int precision = Convert.ToInt32(txtBoxPrecisionIteration.Text);

                double result = SolveIterationForward(x, y, yTarget, precision, out double?[,] diffTable, out var iterationSteps);

                DisplayIterationResults(dataGridViewIteration, richTextBoxIterationResult, diffTable, iterationSteps,
            x, y, yTarget, result, precision);

                lblResultIteration.Text = $"{result}";
                lblResultIteration.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }
        private void btnOpenExcelIteration_Click(object sender, EventArgs e)
        {
            ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                    openFileDialog.Title = "Chọn file Excel chứa dữ liệu (x, y)";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        ReadExcelData(filePath, out double[] xValues, out double[] yValues);
                        dataGridViewXYIteration.Rows.Clear();
                        for (int i = 0; i < xValues.Length; i++)
                        {
                            dataGridViewXYIteration.Rows.Add(xValues[i], yValues[i]);
                        }

                        MessageBox.Show(
                            $"Đã nhập thành công {xValues.Length} điểm dữ liệu từ Excel!",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}",
                      "Lỗi",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
        }
        // Phương pháp bình phương tối thiểu
        private void btnSolveLeastSquares_Click(object sender, EventArgs e)
        {
            try
            {
                double[] x = GetXValues(dataXYLeastSquares);
                double[] y = GetYValues(dataXYLeastSquares);
                string[] phiExpressions = txtBoxLeastSquares.Text
            .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .ToArray();
                if (comboBoxLeastSquares.SelectedIndex == 0)
                {
                    SolveLinearLeastSquares(x, y, phiExpressions, out double[] coeffs, out double MSE, out string equation, out double[,] thetaMatrix, out double[,] M, out double[] b, out double[] yPred);
                    DisplayLeastSquaresResults(rtbResultLeastSquares, phiExpressions, thetaMatrix, M, b, coeffs, MSE, equation, x, y, yPred);
                    lblResultLeastSquares.Text = "Hàm tuyến tính";
                }
                else if (comboBoxLeastSquares.SelectedIndex == 1)
                {
                    SolvePowerLawLeastSquares(x, y, out double a, out double p, out double MSE, out double[,] thetaMatrix, out double[,] M, out double[] r);
                    DisplayPowerLawLeastSquaresResults(rtbResultLeastSquares, thetaMatrix, M, r, a, p, MSE, x, y);
                    lblResultLeastSquares.Text = $"Hàm dạng y = ax^b";
                }
                else if (comboBoxLeastSquares.SelectedIndex == 2)
                {
                    SolveExponentialLeastSquares(x, y, phiExpressions, out double a, out double[] b, out double MSE, out double[,] thetaMatrix, out double[,] M, out double[] r);
                    DisplayExponentialLeastSquaresResults(rtbResultLeastSquares, phiExpressions, thetaMatrix, M, r, a, b, MSE, x, y);
                    lblResultLeastSquares.Text = $"Hàm dạng y = ae^(b1φ₁(x) + b2φ₂(x) + ...)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }

        }
        private void btnOpenExcelLeastSquares_Click(object sender, EventArgs e)
        {
            ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                    openFileDialog.Title = "Chọn file Excel chứa dữ liệu (x, y)";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        ReadExcelDataWithoutDuplicateRemoval(filePath, out double[] xValues, out double[] yValues);
                        dataXYLeastSquares.Rows.Clear();
                        for (int i = 0; i < xValues.Length; i++)
                        {
                            dataXYLeastSquares.Rows.Add(xValues[i], yValues[i]);
                        }
                        MessageBox.Show(
                            $"Đã nhập thành công {xValues.Length} điểm dữ liệu từ Excel!",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}",
                      "Lỗi",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
        }
        // Spline
        //private void btnSolveSpline_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        dataXYSpline.Sort(dataXYSpline.Columns["colsXSpline"], System.ComponentModel.ListSortDirection.Ascending);
        //        double[] x = GetXValues(dataXYSpline);
        //        double[] y = GetYValues(dataXYSpline);
        //        int precision = Convert.ToInt32(txtBoxPrecisionSpline.Text);
        //        RemoveDuplicate(dataXYSpline);

        //        if (comboBoxSpline.SelectedIndex == 0)
        //        {
        //            SolveLinearSpline(x, y, precision, out double[] a, out double[] b, out double[] h);
        //            DisplayLinearSplineResults(rtbResultSpline, x, y, a, b, h, precision);
        //        }
        //        else if (comboBoxSpline.SelectedIndex == 1)
        //        {
        //            // Fix
        //        }
        //        else if (comboBoxSpline.SelectedIndex == 2)
        //        {
        //            SolveCubicSpline(x, y, precision, out double[] a, out double[] b, out double[] c, out double[] d, out double[] h, out double[] alpha);
        //            DisplayCubicSplineResults(rtbResultSpline, x, y, a, b, c, d, h, alpha, precision);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Lỗi: {ex.Message}");
        //    }
        //}
        private void btnOpenExcelSpline_Click(object sender, EventArgs e)
        {
            ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                    openFileDialog.Title = "Chọn file Excel chứa dữ liệu (x, y)";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        ReadExcelData(filePath, out double[] xValues, out double[] yValues);
                        dataXYSpline.Rows.Clear();
                        for (int i = 0; i < xValues.Length; i++)
                        {
                            dataXYSpline.Rows.Add(xValues[i], yValues[i]);
                        }
                        MessageBox.Show(
                            $"Đã nhập thành công {xValues.Length} điểm dữ liệu từ Excel!",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}",
                      "Lỗi",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
        }
        private void btnLagrangeToEval_Click(object sender, EventArgs e)
        {
            TransferCoeffsToEval(dataGridViewCoeffsP, lastPolynomialCoeffs);
            MessageBox.Show("Đã chuyển hệ số qua tính giá trị");
        }
        private void btnNewtonToEval_Click(object sender, EventArgs e)
        {
            TransferCoeffsToEval(dataGridViewCoeffsP, lastPolynomialCoeffs);
            MessageBox.Show("Đã chuyển hệ số qua tính giá trị");
        }
        private void btnNewtonFiniteToEval_Click(object sender, EventArgs e)
        {
            TransferCoeffsToEval(dataGridViewCoeffsP, lastPolynomialCoeffs);
            MessageBox.Show("Đã chuyển hệ số qua tính giá trị");
        }
        private void btnStirlingToEval_Click(object sender, EventArgs e)
        {
            TransferCoeffsToEval(dataGridViewCoeffsP, lastPolynomialCoeffs);
            MessageBox.Show("Đã chuyển hệ số qua tính giá trị");
        }
        private void btnBesselToEval_Click(object sender, EventArgs e)
        {
            TransferCoeffsToEval(dataGridViewCoeffsP, lastPolynomialCoeffs);
            MessageBox.Show("Đã chuyển hệ số qua tính giá trị");
        }
        private void btnGaussIToEval_Click(object sender, EventArgs e)
        {
            TransferCoeffsToEval(dataGridViewCoeffsP, lastPolynomialCoeffs);
            MessageBox.Show("Đã chuyển hệ số qua tính giá trị");
        }
        private void btnGaussIIToEval_Click(object sender, EventArgs e)
        {
            TransferCoeffsToEval(dataGridViewCoeffsP, lastPolynomialCoeffs);
            MessageBox.Show("Đã chuyển hệ số qua tính giá trị");
        }
        // Tìm mốc nội suy cách đều
        private void btnFindPoints_Click(object sender, EventArgs e)
        {
            if (xValues == null || yValues == null || xValues.Length == 0)
            {
                MessageBox.Show("Vui lòng mở file Excel trước");
                return;
            }

            double xAvg = Convert.ToDouble(txtBoxX.Text);
            int k = Convert.ToInt32(textBoxK.Text);

            if (k > xValues.Length)
            {
                MessageBox.Show($"Số mốc nội suy k = {k} lớn hơn số điểm dữ liệu {xValues.Length}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var result = FindInterpolationPoints(xAvg, k, xValues, yValues);
            if (result.xs == -1)
            {
                MessageBox.Show($"Giá trị x = {xAvg} nằm ngoài phạm vi dữ liệu [{xValues.Min()}, {xValues.Max()}]",
                              "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DisplayInterpolationPointsResult(xAvg, result);
            var exportResult = MessageBox.Show(
        $"Đã tìm thấy {result.selectedX.Length} mốc nội suy.\n\nBạn có muốn xuất dữ liệu ra file Excel không?",
        "Xuất dữ liệu",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

            if (exportResult == DialogResult.Yes)
            {
                ExportInterpolationPointsToExcel(xAvg, result.xs, result.selectedX, result.selectedY);
            }
        }
        private void btnOpenExcel_Click(object sender, EventArgs e)
        {
            ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                    openFileDialog.Title = "Chọn file Excel chứa dữ liệu (x, y)";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        ReadExcelData(filePath, out xValues, out yValues);

                        MessageBox.Show(
                            $"Đã đọc thành công {xValues.Length} điểm dữ liệu từ file Excel!\n\n",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}",
                      "Lỗi",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
        }
        // Tìm mốc nội suy ngược
        private void btnFindReversePoints_Click(object sender, EventArgs e)
        {
            if (xValues == null || yValues == null || xValues.Length == 0)
            {
                MessageBox.Show("Vui lòng mở file Excel trước");
                return;
            }

            try
            {
                double yTarget = Convert.ToDouble(txtBoxY.Text);

                var isolationIntervals = FindAllIsolationIntervals(yTarget, yValues);

                if (isolationIntervals.Count == 0)
                {
                    MessageBox.Show($"Không tìm thấy khoảng nào chứa y = {yTarget}\n" +
                                  $"Phạm vi dữ liệu: [{yValues.Min()}, {yValues.Max()}]",
                                  "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var monotonicIntervals = FindMonotonicIntervals(isolationIntervals, xValues, yValues);
                lastYTarget = yTarget;
                lastIsolationIntervals = isolationIntervals;
                lastMonotonicIntervals = monotonicIntervals;

                DisplayMonotonicIntervalsResult(yTarget, isolationIntervals, monotonicIntervals);
                var result = MessageBox.Show(
                    $"Đã tìm thấy {monotonicIntervals.Count} khoảng đơn điệu.\n\nBạn có muốn xuất dữ liệu ra file Excel không?",
                    "Xuất dữ liệu",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ExportMonotonicIntervalsToExcel(yTarget, monotonicIntervals);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Lỗi định dạng dữ liệu đầu vào", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnOpenExcelReverse_Click(object sender, EventArgs e)
        {
            ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                    openFileDialog.Title = "Chọn file Excel chứa dữ liệu (x, y)";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        ReadExcelData(filePath, out xValues, out yValues);

                        MessageBox.Show(
                            $"Đã đọc thành công {xValues.Length} điểm dữ liệu từ file Excel!\n\n",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}",
                      "Lỗi",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
            }
        }
        #endregion
        #region Core
        private double[] SolveLagrange(double[] x, double[] y, int precision, out double[] coeffsD, out double[,] productTable, out double[,] divideTable, out double[,] arrMatrix)
        {
            coeffsD = Lagrange.CoeffsD(x, y, precision, out arrMatrix);
            productTable = Horner.ProductTable(x, precision);
            double[] coeffsW = new double[x.Length + 1];
            for (int i = 0; i <= x.Length; i++)
            {
                coeffsW[i] = productTable[productTable.GetLength(0) - 1, i];
            }
            divideTable = Lagrange.DivideTable(x, y, coeffsW, precision);
            return Function.FindPolynomial(coeffsD, divideTable, precision);
        }
        private double[] SolveNewton(double[] x, double[] y, int precision, out double?[,] diffTable, out double[,] productTable, out double[] dividedDiagonalDiff)
        {
            diffTable = Newton.BuildDividedDifferenceTable(x, y, precision);
            double[] x_newton = x.Take(x.Length - 1).ToArray();
            productTable = Horner.ProductTable(x_newton, precision);
            dividedDiagonalDiff = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                dividedDiagonalDiff[i] = diffTable[i, i + 1] ?? 0.0;
            }
            return Function.FindPolynomial(dividedDiagonalDiff, productTable, precision);
        }
        private double[] SolveNewtonFinite(double[] x, double[] y, int precision, bool isForward, out double?[,] finiteDiffTable, out double[,] productTable, out double[] dividedDiagonalDiff)
        {
            finiteDiffTable = Newton.BuildFiniteDifferenceTable(x, y, precision);
            double[] x_newton = new double[x.Length - 1];
            if (isForward)
            {
                for (int i = 0; i < x.Length - 1; i++)
                {
                    x_newton[i] = i;
                }
            }
            else
            {
                for (int i = 0; i < x.Length - 1; i++)
                {
                    x_newton[i] = -i;
                }
            }
            productTable = Horner.ProductTable(x_newton, precision);
            dividedDiagonalDiff = new double[x.Length];
            if (isForward)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    dividedDiagonalDiff[i] = (finiteDiffTable[i, i + 1] / Function.Factorial(i)) ?? 0.0;
                }
            }
            else
            {
                int lastRow = x.Length - 1;
                for (int i = 0; i < x.Length; i++)
                {
                    dividedDiagonalDiff[i] = (finiteDiffTable[lastRow, i + 1] / Function.Factorial(i)) ?? 0.0;
                }
            }
            return Function.FindPolynomial(dividedDiagonalDiff, productTable, precision);
        }
        private HornerResults SolveHorner(double[] coeffsP, double c, int k, int precision)
        {
            int n = coeffsP.Length - 1;
            var results = new HornerResults();

            results.Evaluation = Horner.HornerEvaluate(coeffsP, c, precision);
            results.Derivatives = Horner.HornerDerivatives(coeffsP, c, n, precision);
            (results.Quotient, results.Remainder) = Horner.HornerDivide(coeffsP, c, precision);
            results.MultipliedPolynomial = Function.MultiplyPolynomial(coeffsP, c, precision);
            results.EvaluationTable = Horner.HornerEvaluationTable(coeffsP, c, precision);
            results.DerivativeTable = Horner.GetHornerDerivativesTable(coeffsP, c, k, precision);

            return results;
        }
        private double[] SolveStirling(double[] x, double[] y, int precision, out double?[,] diffTable, out double[,] prodTable, out double[] vectorCoeffs)
        {
            diffTable = Newton.BuildFiniteDifferenceTable(x, y, precision);
            vectorCoeffs = new double[x.Length];
            int idx = (x.Length - 1) / 2;
            for (int j = 1; j <= x.Length; j++)
            {
                if (j % 2 != 0)
                {
                    vectorCoeffs[j - 1] = diffTable[idx, j] / Function.Factorial(j - 1) ?? 0.0;
                    vectorCoeffs[j - 1] = Math.Round(vectorCoeffs[j - 1], precision);
                }
                else
                {
                    vectorCoeffs[j - 1] = (1.0 / 2.0) * ((diffTable[idx, j] + diffTable[idx + 1, j]) / Function.Factorial(j - 1)) ?? 0.0;
                    vectorCoeffs[j - 1] = Math.Round(vectorCoeffs[j - 1], precision);
                    idx++;
                }
            }
            double[] vectorCoeffsEven = new double[(x.Length + 1) / 2];
            double[] vectorCoeffsOdd = new double[(x.Length / 2)];
            int evenIdx = 0;
            int oddIdx = 0;
            for (int i = 0; i < x.Length; i++)
            {
                if (i % 2 == 0)
                {
                    vectorCoeffsEven[evenIdx++] = vectorCoeffs[i];
                }
                else
                {
                    vectorCoeffsOdd[oddIdx++] = vectorCoeffs[i];
                }
            }
            double[] x_new = new double[(x.Length - 1) / 2];
            for (int i = 0; i < (x.Length - 1) / 2; i++)
            {
                x_new[i] = Math.Pow(i, 2);
            }
            prodTable = Horner.ProductTable(x_new, precision);
            double[,] prodTableEven = prodTable;
            double[,] prodTableOdd = RemoveFirstRowAndLastColumn(prodTable);
            double[] finalCoeffsEven = Function.FindPolynomial(vectorCoeffsEven, prodTableEven, precision);
            double[] finalCoeffsOdd = Function.FindPolynomial(vectorCoeffsOdd, prodTableOdd, precision);

            double[] finalCoeffs = new double[x.Length];
            evenIdx = 0;
            oddIdx = 0;
            for (int i = 0; i < x.Length; i++)
            {
                if ((i % 2) == 0)
                {
                    finalCoeffs[i] = finalCoeffsEven[evenIdx++];
                }
                else
                {
                    finalCoeffs[i] = finalCoeffsOdd[oddIdx++];
                }
            }
            return finalCoeffs;
        }
        private double[] SolveBessel(double[] x, double[] y, int precision, out double?[,] diffTable, out double[,] prodTable, out double[] vectorCoeffs)
        {
            diffTable = Newton.BuildFiniteDifferenceTable(x, y, precision);
            vectorCoeffs = new double[x.Length];
            int idx = (x.Length / 2) - 1;
            for (int j = 1; j <= x.Length; j++)
            {
                if (j % 2 != 0)
                {
                    vectorCoeffs[j - 1] = (1.0 / 2.0) * ((diffTable[idx, j] + diffTable[idx + 1, j]) / Function.Factorial(j - 1)) ?? 0.0;
                    vectorCoeffs[j - 1] = Math.Round(vectorCoeffs[j - 1], precision);
                    idx++;
                }
                else
                {
                    vectorCoeffs[j - 1] = diffTable[idx, j] / Function.Factorial(j - 1) ?? 0.0;
                    vectorCoeffs[j - 1] = Math.Round(vectorCoeffs[j - 1], precision);
                }
            }
            double[] vectorCoeffsEven = new double[(x.Length / 2)];
            double[] vectorCoeffsOdd = new double[(x.Length / 2)];
            int evenIdx = 0;
            int oddIdx = 0;
            for (int i = 0; i < x.Length; i++)
            {
                if (i % 2 == 0)
                {
                    vectorCoeffsEven[evenIdx++] = vectorCoeffs[i];
                }
                else
                {
                    vectorCoeffsOdd[oddIdx++] = vectorCoeffs[i];
                }
            }
            double[] x_new = new double[((x.Length / 2) - 1)];
            for (int i = 1; i < (x.Length / 2); i++)
            {
                x_new[i - 1] = Math.Pow(i - 0.5, 2);
            }
            prodTable = Horner.ProductTable(x_new, precision);
            double[] finalCoeffsEven = Function.FindPolynomial(vectorCoeffsEven, prodTable, precision);
            double[] finalCoeffsOdd = Function.FindPolynomial(vectorCoeffsOdd, prodTable, precision);

            double[] finalCoeffs = new double[x.Length];
            evenIdx = 0;
            oddIdx = 0;
            for (int i = 0; i < x.Length; i++)
            {
                if ((i % 2) != 0)
                {
                    finalCoeffs[i] = finalCoeffsEven[evenIdx++];
                }
                else
                {
                    finalCoeffs[i] = finalCoeffsOdd[oddIdx++];
                }
            }
            return finalCoeffs;
        }
        private (int xs, double[] selectedX, double[] selectedY) FindInterpolationPoints(double x, int k, double[] dataX, double[] dataY)
        {
            int n = dataX.Length;
            int xs = -1;
            bool isRightPoint = false;
            for (int i = 0; i < n - 1; i++)
            {
                if (x >= dataX[i] && x <= dataX[i + 1])
                {
                    double distToLeft = Math.Abs(x - dataX[i]);
                    double distToRight = Math.Abs(x - dataX[i + 1]);

                    if (distToLeft <= distToRight)
                    {
                        xs = i;
                        isRightPoint = false;
                    }
                    else
                    {
                        xs = i + 1;
                        isRightPoint = true;
                    }
                    break;
                }
            }

            if (xs == -1)
            {
                if (x < dataX[0] || x > dataX[n - 1])
                {
                    return (-1, null, null);
                }
            }

            var selectedIndices = new List<int>();
            selectedIndices.Add(xs);
            int left = xs - 1;
            int right = xs + 1;
            if (isRightPoint)
            {
                while (selectedIndices.Count < k)
                {
                    if (left >= 0 && selectedIndices.Count < k)
                    {
                        selectedIndices.Add(left);
                        left--;
                    }
                    if (right < n && selectedIndices.Count < k)
                    {
                        selectedIndices.Add(right);
                        right++;
                    }
                    if (left < 0 && right >= n)
                    {
                        break;
                    }
                }
            }
            else
            {
                while (selectedIndices.Count < k)
                {
                    if (right < n && selectedIndices.Count < k)
                    {
                        selectedIndices.Add(right);
                        right++;
                    }
                    if (left >= 0 && selectedIndices.Count < k)
                    {
                        selectedIndices.Add(left);
                        left--;
                    }
                    if (left < 0 && right >= n)
                    {
                        break;
                    }
                }
            }
            double[] selectedX = selectedIndices.Select(i => dataX[i]).ToArray();
            double[] selectedY = selectedIndices.Select(i => dataY[i]).ToArray();

            return (xs, selectedX, selectedY);
        }
        private double[] SolveGaussI(double[] x, double[] y, int precision,
    out double?[,] diffTable, out double[,] prodTable, out double[] vectorCoeffs)
        {
            diffTable = Gauss.BuildGaussForwardTable(x, y, precision);

            int n = x.Length;
            vectorCoeffs = new double[n];

            int center = (n - 1) / 2;

            vectorCoeffs[0] = diffTable[center, 1] ?? 0.0;
            int currentRow = center;

            for (int j = 2; j <= n; j++)
            {
                if (j % 2 != 0)
                    currentRow--;

                if (currentRow >= 0 && currentRow < n - j + 1)
                {
                    vectorCoeffs[j - 1] = (diffTable[currentRow, j] ?? 0.0) / Function.Factorial(j - 1);
                    vectorCoeffs[j - 1] = Math.Round(vectorCoeffs[j - 1], precision);
                }
            }
            double[] tPattern = new double[n - 1];
            for (int i = 0; i < n - 1; i++)
            {
                if (i == 0)
                    tPattern[i] = 0;
                else if (i % 2 == 1)
                    tPattern[i] = (i + 1) / 2.0;
                else
                    tPattern[i] = -(i / 2.0);
            }

            prodTable = Horner.ProductTable(tPattern, precision);
            return Function.FindPolynomial(vectorCoeffs, prodTable, precision);
        }
        private double[] SolveGaussII(double[] x, double[] y, int precision,
    out double?[,] diffTable, out double[,] prodTable, out double[] vectorCoeffs)
        {
            diffTable = Gauss.BuildGaussForwardTable(x, y, precision);

            int n = x.Length;
            vectorCoeffs = new double[n];

            int center = (n - 1) / 2;

            vectorCoeffs[0] = diffTable[center, 1] ?? 0.0;
            int currentRow = center;

            for (int j = 2; j <= n; j++)
            {
                if (j % 2 == 0)
                    currentRow--;

                if (currentRow >= 0 && currentRow < n - j + 1)
                {
                    vectorCoeffs[j - 1] = (diffTable[currentRow, j] ?? 0.0) / Function.Factorial(j - 1);
                    vectorCoeffs[j - 1] = Math.Round(vectorCoeffs[j - 1], precision);
                }
            }

            double[] tPattern = new double[n - 1];
            for (int i = 0; i < n - 1; i++)
            {
                if (i == 0)
                    tPattern[i] = 0;
                else if (i % 2 == 1)
                    tPattern[i] = -(i + 1) / 2.0;
                else
                    tPattern[i] = i / 2.0;
            }
            prodTable = Horner.ProductTable(tPattern, precision);

            return Function.FindPolynomial(vectorCoeffs, prodTable, precision);
        }
        private List<(int startIndex, int endIndex)> FindAllIsolationIntervals(double y, double[] dataY)
        {
            int n = dataY.Length;
            var isolationIntervals = new List<(int startIndex, int endIndex)>();

            // Tìm các khoảng cách ly chứa y
            for (int i = 0; i < n - 1; i++)
            {
                // Kiểm tra nếu y nằm trong khoảng [dataY[i], dataY[i+1]]
                double minY = Math.Min(dataY[i], dataY[i + 1]);
                double maxY = Math.Max(dataY[i], dataY[i + 1]);

                if (y >= minY && y <= maxY)
                {
                    isolationIntervals.Add((i, i + 1));
                }
            }

            return isolationIntervals;
        }
        private List<(int startIndex, int endIndex, bool isIncreasing, double[] selectedX, double[] selectedY)> FindMonotonicIntervals(
            List<(int startIndex, int endIndex)> isolationIntervals,
            double[] dataX,
            double[] dataY)
        {
            var monotonicIntervals = new List<(int startIndex, int endIndex, bool isIncreasing, double[] selectedX, double[] selectedY)>();
            int n = dataY.Length;

            foreach (var isolation in isolationIntervals)
            {
                int isoStart = isolation.startIndex;
                int isoEnd = isolation.endIndex;

                // Xác định xu hướng của khoảng cách ly
                bool isIncreasing = dataY[isoEnd] > dataY[isoStart];

                // Mở rộng về trái để tìm khoảng đơn điệu
                int monotonicStart = isoStart;
                while (monotonicStart > 0)
                {
                    bool prevIncreasing = dataY[monotonicStart] > dataY[monotonicStart - 1];
                    if (prevIncreasing == isIncreasing)
                    {
                        monotonicStart--;
                    }
                    else
                    {
                        break;
                    }
                }

                // Mở rộng về phải để tìm khoảng đơn điệu
                int monotonicEnd = isoEnd;
                while (monotonicEnd < n - 1)
                {
                    bool nextIncreasing = dataY[monotonicEnd + 1] > dataY[monotonicEnd];
                    if (nextIncreasing == isIncreasing)
                    {
                        monotonicEnd++;
                    }
                    else
                    {
                        break;
                    }
                }

                // Lấy TẤT CẢ các điểm trong khoảng đơn điệu
                var selectedIndices = new List<int>();
                for (int idx = monotonicStart; idx <= monotonicEnd; idx++)
                {
                    selectedIndices.Add(idx);
                }

                double[] selectedX = selectedIndices.Select(idx => dataX[idx]).ToArray();
                double[] selectedY = selectedIndices.Select(idx => dataY[idx]).ToArray();

                monotonicIntervals.Add((monotonicStart, monotonicEnd, isIncreasing, selectedX, selectedY));
            }

            return monotonicIntervals;
        }
        private void ExportMonotonicIntervalsToExcel(
    double y,
    List<(int startIndex, int endIndex, bool isIncreasing, double[] selectedX, double[] selectedY)> monotonicIntervals)
        {
            ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel Files|*.xlsx";
                    saveFileDialog.Title = "Xuất dữ liệu ra Excel";
                    saveFileDialog.FileName = $"ReverseInterpolation_y_{y}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = saveFileDialog.FileName;

                        using (ExcelPackage package = new ExcelPackage())
                        {
                            // Tạo worksheet cho mỗi khoảng đơn điệu
                            for (int idx = 0; idx < monotonicIntervals.Count; idx++)
                            {
                                var monotonic = monotonicIntervals[idx];

                                // Tạo worksheet với tên đơn giản
                                string worksheetName = $"Khoang_{idx + 1}";
                                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetName);

                                // Điền dữ liệu trực tiếp từ hàng 1, không có header
                                int row = 1;
                                for (int i = 0; i < monotonic.selectedX.Length; i++)
                                {
                                    worksheet.Cells[row, 1].Value = monotonic.selectedX[i];
                                    worksheet.Cells[row, 2].Value = monotonic.selectedY[i];
                                    row++;
                                }
                                // Auto-fit columns
                                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                            }

                            // Lưu file
                            FileInfo fileInfo = new FileInfo(filePath);
                            package.SaveAs(fileInfo);

                            MessageBox.Show(
                                $"Đã xuất thành công {monotonicIntervals.Count} khoảng đơn điệu ra file:\n{filePath}",
                                "Thành công",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất file Excel: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void ExportInterpolationPointsToExcel(
    double x,
    int xs,
    double[] selectedX,
    double[] selectedY)
        {
            ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel Files|*.xlsx";
                    saveFileDialog.Title = "Xuất dữ liệu ra Excel";
                    saveFileDialog.FileName = $"InterpolationPoints_x_{x}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = saveFileDialog.FileName;

                        using (ExcelPackage package = new ExcelPackage())
                        {
                            // Tạo worksheet
                            string worksheetName = "Moc_Noi_Suy";
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetName);

                            // Điền dữ liệu trực tiếp từ hàng 1, không có header
                            int row = 1;
                            for (int i = 0; i < selectedX.Length; i++)
                            {
                                worksheet.Cells[row, 1].Value = selectedX[i];
                                worksheet.Cells[row, 2].Value = selectedY[i];
                                row++;
                            }

                            // Auto-fit columns
                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                            // Lưu file
                            FileInfo fileInfo = new FileInfo(filePath);
                            package.SaveAs(fileInfo);

                            MessageBox.Show(
                                $"Đã xuất thành công {selectedX.Length} mốc nội suy ra file:\n{filePath}",
                                "Thành công",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất file Excel: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private double SolveIterationForward(double[] x, double[] y, double yTarget, int precision,
     out double?[,] diffTable, out List<(int iteration, double t_prev, double t_n, double sum, double error, List<(int r, double deltaR, double factorial, double prod, double term)> details)> iterationSteps)
        {
            diffTable = Newton.BuildFiniteDifferenceTable(x, y, precision);
            iterationSteps = new List<(int, double, double, double, double, List<(int, double, double, double, double)>)>();
            double h = Math.Round(x[1] - x[0], precision);
            double y0 = y[0];

            // Sai phân bậc 1
            double delta1 = diffTable[1, 2] ?? 0.0;

            // Giá trị khởi tạo t_0
            double t_prev, t_n = Math.Round((yTarget - y0) / delta1, precision);
            double epsilon = 1e-8;
            int maxIterations = 1000;
            int iteration = 0;

            iterationSteps.Add((0, 0, t_n, 0, 0, new List<(int, double, double, double, double)>()));

            do
            {
                t_prev = t_n;
                double sum = 0.0;
                var stepDetails = new List<(int r, double deltaR, double factorial, double prod, double term)>();

                for (int r = 2; r < x.Length; r++)
                {
                    double deltaR = diffTable[r, r + 1] ?? 0.0;
                    double factorial = Function.Factorial(r);

                    double prod = 1.0;
                    for (int i = 0; i < r; i++)
                    {
                        prod *= (t_prev - i);
                    }
                    double term = Math.Round((deltaR / factorial) * prod, precision);
                    sum += term;

                    stepDetails.Add((r, deltaR, factorial, prod, term));
                }

                t_n = Math.Round(((yTarget - y0) / delta1) - (sum / delta1), precision);
                double error = Math.Abs(t_n - t_prev);

                iterationSteps.Add((iteration + 1, t_prev, t_n, sum, error, stepDetails));

                iteration++;

                if (iteration > maxIterations) break;
            } while (Math.Abs(t_n - t_prev) >= epsilon);

            double res = x[0] + h * t_n;
            return res;
        }
        private void SolveLinearLeastSquares(double[] x, double[] y, string[] phiExpressions,
    out double[] coeffs, out double MSE, out string equation,
    out double[,] thetaMatrix, out double[,] M, out double[] b, out double[] yPred)
        {
            int n = x.Length;
            int m = phiExpressions.Length;

            thetaMatrix = new double[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    var expr = new NCalc.Expression(phiExpressions[j]);
                    expr.Parameters["x"] = x[i];
                    thetaMatrix[i, j] = Convert.ToDouble(expr.Evaluate());
                }
            }

            var Theta = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(thetaMatrix);
            var Y = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(y);

            var MMatrix = Theta.TransposeThisAndMultiply(Theta);
            var bVec = Theta.TransposeThisAndMultiply(Y);

            M = MMatrix.ToArray();
            b = bVec.ToArray();

            var res = MMatrix.Solve(bVec);
            coeffs = res.ToArray();

            var yTarget = Theta * res;
            yPred = yTarget.ToArray();
            MSE = Math.Sqrt((Y - yTarget).PointwisePower(2).Sum() / n);

            var coeffs_copy = coeffs;
            equation = "y ≈ " + string.Join(" + ",
                phiExpressions.Select((phi, j) => $"{coeffs_copy[j]:F6}*({phi})"));
        }
        private void SolvePowerLawLeastSquares(double[] x, double[] y,
    out double a, out double p, out double MSE,
    out double[,] ThetaMatrix, out double[,] M, out double[] r)
        {
            int n = x.Length;

            bool allPositive = y.All(v => v > 0);
            bool allNegative = y.All(v => v < 0);
            if (!allPositive && !allNegative)
            {
                throw new ArgumentException("Giá trị y phải cùng dấu để áp dụng phương pháp hàm mũ.");
            }
            double[] X = x.Select(xi => Math.Log(xi)).ToArray();
            double[] Y = y.Select(yi => Math.Log(Math.Abs(yi))).ToArray();

            ThetaMatrix = new double[n, 2];
            for (int i = 0; i < n; i++)
            {
                ThetaMatrix[i, 0] = 1.0;
                ThetaMatrix[i, 1] = X[i];
            }

            var Theta = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(ThetaMatrix);
            var Yvec = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(Y);

            var MMatrix = Theta.TransposeThisAndMultiply(Theta);
            var rVec = Theta.TransposeThisAndMultiply(Yvec);

            M = MMatrix.ToArray();
            r = rVec.ToArray();

            var A = MMatrix.Solve(rVec);

            a = Math.Exp(A[0]);
            p = A[1];
            if (allNegative)
            {
                a = -a;
            }
            var Y_pred = Theta * A;
            MSE = Math.Sqrt((Yvec - Y_pred).PointwisePower(2).Sum() / n);
        }
        private void SolveExponentialLeastSquares(
    double[] x, double[] y, string[] phiExpressions,
    out double a, out double[] b, out double MSE,
    out double[,] ThetaMatrix, out double[,] M, out double[] r)
        {
            int n = x.Length;
            int m = phiExpressions.Length;

            bool allPositive = y.All(v => v > 0);
            bool allNegative = y.All(v => v < 0);
            if (!allPositive && !allNegative)
                throw new Exception("Dữ liệu y chứa cả giá trị dương và âm — không thể tuyến tính hoá bằng logarit.");

            double[] Y = y.Select(yi => Math.Log(Math.Abs(yi))).ToArray();

            ThetaMatrix = new double[n, m + 1];
            for (int i = 0; i < n; i++)
            {
                ThetaMatrix[i, 0] = 1.0;
                for (int j = 0; j < m; j++)
                {
                    var expr = new NCalc.Expression(phiExpressions[j]);
                    expr.Parameters["x"] = x[i];
                    ThetaMatrix[i, j + 1] = Convert.ToDouble(expr.Evaluate());
                }
            }

            var Theta = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(ThetaMatrix);
            var Yvec = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(Y);

            var MMatrix = Theta.TransposeThisAndMultiply(Theta);
            var rVec = Theta.TransposeThisAndMultiply(Yvec);

            M = MMatrix.ToArray();
            r = rVec.ToArray();

            var C = MMatrix.Solve(rVec).ToArray();

            a = Math.Exp(C[0]);
            if (allNegative) a = -a;

            b = C.Skip(1).ToArray();

            var Y_pred = Theta * MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(C);
            MSE = Math.Sqrt((Yvec - Y_pred).PointwisePower(2).Sum() / n);
        }
        // Spline

    //    private void SolveLinearSpline(
    //double[] x, double[] y, int precision,
    //out double[] a, out double[] b, out double[] h)
    //    {
    //        int n = x.Length;

    //        a = new double[n - 1];
    //        b = new double[n - 1];
    //        h = new double[n - 1];

    //        for (int i = 0; i < n - 1; i++)
    //        {
    //            h[i] = Math.Round(x[i + 1] - x[i], precision);
    //            a[i] = Math.Round((y[i + 1] - y[i]) / h[i], precision);
    //            b[i] = Math.Round((y[i] * x[i + 1] - y[i + 1] * x[i]) / h[i], precision);
    //        }
    //    }
    //    private void SolveQuadraticSpline(
    //double[] x, double[] y, int precision,
    //double? s_prime_start, // Điều kiện S'(x_0)
    //double? s_prime_end,   // Điều kiện S'(x_{n-1})
    //out double[] a, out double[] b, out double[] c, out double[] h,
    //out double[] m, out double[] v)
    //    {
    //        int n = x.Length;
    //        int numSplines = n - 1;

    //        // Khởi tạo các mảng output
    //        a = new double[numSplines];
    //        b = new double[numSplines];
    //        c = new double[numSplines];
    //        h = new double[numSplines];

    //        // Khởi tạo các mảng trung gian - EXPORT ra ngoài
    //        v = new double[numSplines]; // Vế phải
    //        m = new double[n];          // Các giá trị đạo hàm m[i] = S'(x[i])

    //        // --- Bước 1: Tính h[i] và v[i] cho mỗi khoảng ---
    //        for (int i = 0; i < numSplines; i++)
    //        {
    //            h[i] = Math.Round(x[i + 1] - x[i], precision);
    //            if (h[i] == 0)
    //            {
    //                throw new ArgumentException($"Các mốc x bị trùng nhau tại chỉ số {i}.");
    //            }
    //            v[i] = Math.Round(2 * (y[i + 1] - y[i]) / h[i], precision);
    //        }

    //        // --- Bước 2: Giải tìm các đạo hàm m[i] dựa trên điều kiện biên ---

    //        if (s_prime_start.HasValue)
    //        {
    //            // TRƯỜNG HỢP 1: Người dùng cung cấp S'(x_0)
    //            // Giải truy hồi TIẾN
    //            m[0] = s_prime_start.Value;
    //            for (int i = 0; i < numSplines; i++)
    //            {
    //                m[i + 1] = Math.Round(v[i] - m[i], precision);
    //            }
    //        }
    //        else if (s_prime_end.HasValue)
    //        {
    //            // TRƯỜNG HỢP 2: Người dùng cung cấp S'(x_{n-1})
    //            // Giải truy hồi LÙI
    //            m[n - 1] = s_prime_end.Value;
    //            for (int i = numSplines - 1; i >= 0; i--)
    //            {
    //                m[i] = Math.Round(v[i] - m[i + 1], precision);
    //            }
    //        }
    //        else
    //        {
    //            // TRƯỜNG HỢP 3: Người dùng KHÔNG cung cấp điều kiện nào
    //            // Sử dụng điều kiện mặc định theo yêu cầu: m[0] = 0
    //            m[0] = 0;
    //            for (int i = 0; i < numSplines; i++)
    //            {
    //                m[i + 1] = Math.Round(v[i] - m[i], precision);
    //            }
    //        }

    //        // --- Bước 3: Tính các hệ số a, b, c từ m[i] ---
    //        for (int i = 0; i < numSplines; i++)
    //        {
    //            a[i] = Math.Round((m[i + 1] - m[i]) / (2 * h[i]), precision);
    //            b[i] = Math.Round((m[i] * x[i + 1] - m[i + 1] * x[i]) / h[i], precision);
    //            c[i] = Math.Round(
    //                (-m[i] * x[i + 1] * x[i + 1] + m[i + 1] * x[i] * x[i]) / (2 * h[i]) + y[i] + (m[i] * h[i]) / 2,
    //                precision);
    //        }
    //    }
        //private void SolveCubicSpline(
        //double[] x, double[] y, int precision,
        //out double[] a, out double[] b, out double[] c, out double[] d, out double[] h, out double[] alpha)
        //{
        //    int n = x.Length;
        //    int numSplines = n - 1; // Số lượng khoảng (spline)
        //    int numEquations = n - 2; // Số phương trình cần giải

        //    a = new double[numSplines];
        //    b = new double[numSplines];
        //    c = new double[numSplines];
        //    d = new double[numSplines];
        //    h = new double[numSplines];
        //    alpha = new double[n];    // Đạo hàm cấp hai S''(x_i)

        //    double[] gamma = new double[numEquations]; // Vế phải của hệ PT

        //    // --- Bước 1: Tính h[i] ---
        //    for (int i = 0; i < numSplines; i++)
        //    {
        //        h[i] = Math.Round(x[i + 1] - x[i], precision);
        //        if (h[i] == 0)
        //        {
        //            throw new ArgumentException($"Các mốc x bị trùng nhau tại chỉ số {i}.");
        //        }
        //    }

        //    // --- Bước 2: Tính gamma[i] (vế phải) ---
        //    for (int i = 0; i < numEquations; i++)
        //    {
        //        double term1 = (y[i + 2] - y[i + 1]) / h[i + 1];
        //        double term2 = (y[i + 1] - y[i]) / h[i];
        //        gamma[i] = Math.Round(term1 - term2, precision);
        //    }

        //    // --- Bước 3: Giải hệ phương trình tam đường chéo để tìm alpha[i] ---
        //    // Giả định Spline Tự nhiên (Natural Spline): alpha[0] = 0 và alpha[n-1] = 0
        //    alpha[0] = 0;
        //    alpha[n - 1] = 0;

        //    // Xây dựng 3 đường chéo của hệ phương trình
        //    double[] subDiag = new double[numEquations];    // Đường chéo dưới
        //    double[] mainDiag = new double[numEquations];   // Đường chéo chính
        //    double[] superDiag = new double[numEquations];  // Đường chéo trên

        //    for (int i = 0; i < numEquations; i++)
        //    {
        //        subDiag[i] = Math.Round(h[i] / 6.0, precision);
        //        mainDiag[i] = Math.Round((h[i] + h[i + 1]) / 3.0, precision);
        //        superDiag[i] = Math.Round(h[i + 1] / 6.0, precision);
        //    }

        //    // Điều chỉnh cho điều kiện biên (alpha[0]=0, alpha[n-1]=0)
        //    subDiag[0] = 0;
        //    superDiag[numEquations - 1] = 0;

        //    // Giải hệ
        //    double[] solution = SolveTridiagonalSystem(subDiag, mainDiag, superDiag, gamma);

        //    // Nạp kết quả vào mảng alpha
        //    for (int i = 0; i < numEquations; i++)
        //    {
        //        alpha[i + 1] = solution[i];
        //    }

        //    // --- Bước 4: Tính các hệ số a, b, c, d  ---
        //    for (int i = 0; i < numSplines; i++)
        //    {
        //        double h_i = h[i];
        //        double x_i = x[i];
        //        double x_i1 = x[i + 1];
        //        double y_i = y[i];
        //        double y_i1 = y[i + 1];
        //        double alpha_i = alpha[i];
        //        double alpha_i1 = alpha[i + 1];

        //        // a[i]
        //        a[i] = Math.Round((alpha_i1 - alpha_i) / (6 * h_i), precision);

        //        // b[i]
        //        b[i] = Math.Round((alpha_i * x_i1 - alpha_i1 * x_i) / (2 * h_i), precision);

        //        // c[i]
        //        double c_term1 = (-alpha_i * x_i1 * x_i1 + alpha_i1 * x_i * x_i) / (2 * h_i);
        //        double c_term2 = (y_i1 - y_i) / h_i;
        //        double c_term3 = (-(alpha_i1 - alpha_i) * h_i) / 6;
        //        c[i] = Math.Round(c_term1 + c_term2 + c_term3, precision);

        //        // d[i]
        //        double d_term1 = (alpha_i * x_i1 * x_i1 * x_i1 - alpha_i1 * x_i * x_i * x_i) / (6 * h_i);
        //        double d_term2 = (y_i * x_i1 - y_i1 * x_i) / h_i;
        //        double d_term3 = ((alpha_i1 * x_i - alpha_i * x_i1) * h_i) / 6;
        //        d[i] = Math.Round(d_term1 + d_term2 + d_term3, precision);
        //    }
        //}
        #endregion 

        #region UI
        private void DisplayLagrangeResults(DataGridView dgv, int n, double[] coeffsD, double[,] productTable, double[,] divideTable, double[] lagrangePolynomial, double[,] arrMatrix) // n là số nút nội suy
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            SetupColumns(dgv, n + 1, "Ghi chú"); // n + 1 cột do bảng nhân và 1 cột ghi chú
            AddTable(dgv, arrMatrix, "Ma trận {x_j - x_i}");
            AddRow(dgv, coeffsD, "Hệ số D");
            AddSectionBreak(dgv);
            AddTable(dgv, productTable, "Bảng tích", "w_{n+1}");
            AddSectionBreak(dgv);
            AddTable(dgv, divideTable, "Bảng thương");
            AddSectionBreak(dgv);
            AddRow(dgv, lagrangePolynomial, "Hệ số đa thức nội suy");
        }

        private void DisplayNewtonResults(DataGridView dgv, int n, double?[,] diffTable, double[] dividedDiagonalDiff, double[,] productTable, double[] newtonPolynomial)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            SetupColumns(dgv, n + 1, "Ghi chú");
            AddNullableTable(dgv, diffTable, "Bảng tỷ sai phân");
            AddSectionBreak(dgv);
            AddRow(dgv, dividedDiagonalDiff, "Hệ số trích xuất từ bảng TSP");
            AddSectionBreak(dgv);
            AddTable(dgv, productTable, "Bảng tích", "w_{n+1}");
            AddSectionBreak(dgv);
            AddRow(dgv, newtonPolynomial, "Hệ số đa thức nội suy");
        }
        private void DisplayHornerResults(HornerResults results, double c)
        {
            richTextBoxResult.Clear();
            richTextBoxResult.AppendText($"Giá trị đa thức P(x) tại c:\nP({c}) = {results.Evaluation}\n\n");
            richTextBoxResult.AppendText($"Giá trị đạo hàm các cấp của đa thức P(x) tại c:\n");
            for (int m = 1; m < results.Derivatives.Length; m++)
            {
                richTextBoxResult.AppendText($"P^({m})(x = {c}) = {results.Derivatives[m]}\n");
            }
            richTextBoxResult.AppendText($"\nThương và dư của phép chia P(x) với (x-c):\n");
            richTextBoxResult.AppendText($"Q(x) = {Function.PolynomialToString(results.Quotient.Reverse().ToArray())}, R = {results.Remainder}\n\n");
            richTextBoxResult.AppendText($"Tích đa thức P(x) với (x-c):\n");
            richTextBoxResult.AppendText($"{Function.PolynomialToString(results.MultipliedPolynomial)}\n");

            dataGridViewHornerEval.DataSource = results.EvaluationTable;
            dataGridViewHornerDerivative.DataSource = results.DerivativeTable;

            StyleHornerGrids();
        }
        private void StyleHornerGrids()
        {
            if (dataGridViewHornerEval.Rows.Count > 1 && dataGridViewHornerEval.Columns.Count > 0)
            {
                var cell = dataGridViewHornerEval.Rows[1].Cells[dataGridViewHornerEval.ColumnCount - 1];
                cell.Style.BackColor = Color.LightGreen;
                cell.Style.Font = new Font(dataGridViewHornerEval.Font, FontStyle.Bold);
            }
            foreach (DataGridViewRow row in dataGridViewHornerDerivative.Rows)
            {
                if (row.Index == 0) continue;
                for (int i = row.Cells.Count - 1; i >= 0; i--)
                {
                    var cell = row.Cells[i];
                    if (cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()))
                    {
                        cell.Style.BackColor = Color.LightSkyBlue;
                        cell.Style.Font = new Font(dataGridViewHornerEval.Font, FontStyle.Bold);
                        break;
                    }
                }
            }
        }
        private void DisplayCentralResultsStirling(DataGridView dgv, int n, double?[,] diffTable, double[] coeffsVector, double[,] productTable, double[] coeffsPolynomial)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            SetupColumns(dgv, n + 1, "Ghi chú");
            AddNullableTable(dgv, diffTable, "Bảng tỷ sai phân");
            AddSectionBreak(dgv);
            AddRow(dgv, coeffsVector, "Hệ số trích xuất từ bảng TSP");
            AddSectionBreak(dgv);
            AddTable(dgv, productTable, "Bảng tích", "t = 0, 1, 4, 9, 16...");
            AddSectionBreak(dgv);
            AddRow(dgv, coeffsPolynomial, "Hệ số đa thức nội suy");
        }
        private void DisplayCentralResultsBessel(DataGridView dgv, int n, double?[,] diffTable, double[] coeffsVector, double[,] productTable, double[] coeffsPolynomial)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            SetupColumns(dgv, n + 1, "Ghi chú");
            AddNullableTable(dgv, diffTable, "Bảng tỷ sai phân");
            AddSectionBreak(dgv);
            AddRow(dgv, coeffsVector, "Hệ số trích xuất từ bảng TSP");
            AddSectionBreak(dgv);
            AddTable(dgv, productTable, "Bảng tích", "t = 0.25, 2.25, 6.25, 12.25...");
            AddSectionBreak(dgv);
            AddRow(dgv, coeffsPolynomial, "Hệ số đa thức nội suy");
        }
        private void DisplayInterpolationPointsResult(double x, (int xs, double[] selectedX, double[] selectedY) result)
        {
            var sb = new StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════════");
            sb.AppendLine($"KẾT QUẢ TÌM MỐC NỘI SUY CHO x = {x}");
            sb.AppendLine("═══════════════════════════════════════════════\n");

            sb.AppendLine($"Số mốc nội suy tìm được: {result.selectedX.Length}\n");

            sb.AppendLine("Bộ điểm (x, y) được chọn:");
            sb.AppendLine("───────────────────────────────────────────────");
            sb.AppendLine(String.Format("{0,-5} {1,15} {2,15}", "STT", "x", "y"));
            sb.AppendLine("───────────────────────────────────────────────");

            for (int i = 0; i < result.selectedX.Length; i++)
            {
                sb.AppendLine(String.Format("{0,-5} {1,15:G10} {2,15:G10}",
                    i + 1, result.selectedX[i], result.selectedY[i]));
            }

            sb.AppendLine("───────────────────────────────────────────────");
            sb.AppendLine($"\nĐiểm trung tâm xs = {xValues[result.xs]:G10}");
            sb.AppendLine("═══════════════════════════════════════════════");

            richTextBoxFindPoints.Clear();
            richTextBoxFindPoints.AppendText(sb.ToString());
        }
        private void DisplayGaussIResults(DataGridView dgv, int n, double?[,] diffTable,
    double[] coeffsVector, double[,] productTable, double[] gaussIPolynomial)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            SetupColumns(dgv, n + 1, "Ghi chú");

            AddNullableTable(dgv, diffTable, "Bảng sai phân");
            AddSectionBreak(dgv);
            AddRow(dgv, coeffsVector, "Hệ số trích xuất (Δⁿf/n!)");
            AddSectionBreak(dgv);
            AddTable(dgv, productTable, "Bảng tích", "t = 0, 1, -1, 2, -2...");
            AddSectionBreak(dgv);
            AddRow(dgv, gaussIPolynomial, "Hệ số đa thức nội suy Gauss I");
        }
        private void DisplayGaussIIResults(DataGridView dgv, int n, double?[,] diffTable,
    double[] coeffsVector, double[,] productTable, double[] gaussIPolynomial)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            SetupColumns(dgv, n + 1, "Ghi chú");

            AddNullableTable(dgv, diffTable, "Bảng sai phân");
            AddSectionBreak(dgv);
            AddRow(dgv, coeffsVector, "Hệ số trích xuất (Δⁿf/n!)");
            AddSectionBreak(dgv);
            AddTable(dgv, productTable, "Bảng tích", "t = 0, -1, 1, -2, 2...");
            AddSectionBreak(dgv);
            AddRow(dgv, gaussIPolynomial, "Hệ số đa thức nội suy Gauss II");
        }
        private void DisplayMonotonicIntervalsResult(double y,
            List<(int startIndex, int endIndex)> isolationIntervals,
            List<(int startIndex, int endIndex, bool isIncreasing, double[] selectedX, double[] selectedY)> monotonicIntervals)
        {
            var sb = new StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine($"TÌM KHOẢNG ĐƠN ĐIỆU CHỨA CÁC KHOẢNG CÁCH LY y = {y}");
            sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

            if (isolationIntervals.Count == 0)
            {
                sb.AppendLine($"Không tìm thấy khoảng nào chứa y = {y}");
                sb.AppendLine($"Phạm vi dữ liệu: [{yValues.Min():F6}, {yValues.Max():F6}]");
            }
            else
            {
                sb.AppendLine($"Tìm thấy {isolationIntervals.Count} khoảng cách ly chứa y = {y}");
                sb.AppendLine($"Tìm thấy {monotonicIntervals.Count} khoảng đơn điệu tương ứng\n");

                for (int idx = 0; idx < monotonicIntervals.Count; idx++)
                {
                    var isolation = isolationIntervals[idx];
                    var monotonic = monotonicIntervals[idx];

                    sb.AppendLine("╔═══════════════════════════════════════════════════════════════╗");
                    sb.AppendLine($"KHOẢNG {idx + 1}");
                    sb.AppendLine("╠═══════════════════════════════════════════════════════════════╣");

                    // Thông tin khoảng cách ly
                    sb.AppendLine("KHOẢNG CÁCH LY:");
                    sb.AppendLine($"f(x) ∈ [{yValues[isolation.startIndex]}, {yValues[isolation.endIndex]}]");
                    sb.AppendLine($"x    ∈ [{xValues[isolation.startIndex]}, {xValues[isolation.endIndex]}]");
                    sb.AppendLine("╠═══════════════════════════════════════════════════════════════╣");

                    // Thông tin khoảng đơn điệu
                    string monotonicType = monotonic.isIncreasing ? "TĂNG" : "GIẢM";
                    sb.AppendLine($"KHOẢNG ĐƠN ĐIỆU ({monotonicType}):");
                    sb.AppendLine($"f(x) ∈ [{monotonic.selectedY.Min()}, {monotonic.selectedY.Max()}]");
                    sb.AppendLine($"x    ∈ [{monotonic.selectedX.Min()}, {monotonic.selectedX.Max()}]");
                    sb.AppendLine($"Số điểm: {monotonic.selectedX.Length}");
                    sb.AppendLine("╠═══════════════════════════════════════════════════════════════╣");
                    sb.AppendLine("CÁC MỐC NỘI SUY:");
                    sb.AppendLine("╠═══════════════════════════════════════════════════════════════╣");
                    sb.AppendLine("           x                           f(x)                         ");
                    sb.AppendLine("╠═══════════════════════════════════════════════════════════════╣");

                    for (int i = 0; i < monotonic.selectedX.Length; i++)
                    {
                        sb.AppendLine(String.Format("  {0,18}      {1,18}       ",
                            monotonic.selectedX[i], monotonic.selectedY[i]));
                    }

                    sb.AppendLine("╚═══════════════════════════════════════════════════════════════╝\n");
                    sb.AppendLine("- Nếu sử dụng phương pháp hàm ngược để tìm đa thức nội suy thì nhập file Excel đã xuất vào chức năng tìm mốc nội suy cách đều để chọn ra các điểm nội suy sao cho khoảng cách ly nằm giữa khoảng đơn điệu");
                    sb.AppendLine("-Nếu sử dụng phương pháp lặp để tìm đa thức nội suy, hãy tự chọn ra các mốc sao cho khoảng cách ly nằm ở đầu (Nếu sử dụng Newton tiến) hoặc khoảng cách ly nằm ở cuối (Nếu sử dụng Newton lùi)");
                }
            }
            richTextBoxFindReversePoints.Clear();
            richTextBoxFindReversePoints.AppendText(sb.ToString());
        }
        private void DisplayIterationResults(DataGridView dgv, RichTextBox rtb, double?[,] diffTable,
            List<(int iteration, double t_prev, double t_n, double sum, double error, List<(int r, double deltaR, double factorial, double prod, double term)> details)> iterationSteps,
            double[] x, double[] y, double yTarget, double result, int precision)
        {
            // Hiển thị bảng tỷ sai phân
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            SetupColumns(dgv, diffTable.GetLength(1), "Ghi chú");
            AddNullableTable(dgv, diffTable, "Bảng tỷ sai phân");

            // Hiển thị quá trình lặp
            rtb.Clear();

            double h = x[1] - x[0];
            double y0 = y[0];
            double delta1 = diffTable[1, 2] ?? 0.0;

            var sb = new StringBuilder();
            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("QUÁ TRÌNH LẶP");
            sb.AppendLine("═══════════════════════════════════════════════════════════════\n");
            sb.AppendLine("Dữ liệu ban đầu:");
            sb.AppendLine($"  h = {h}");
            sb.AppendLine($"  y₀ = {y0}");
            sb.AppendLine($"  y_target = {yTarget}");
            sb.AppendLine($"  Δy₀ = {delta1}");
            sb.AppendLine($"  epsilon = 1e-8");
            sb.AppendLine($"  Max iterations = 100\n");

            foreach (var step in iterationSteps)
            {
                if (step.iteration == 0)
                {
                    sb.AppendLine("KHỞI TẠO:");
                    sb.AppendLine("───────────────────────────────────────────────────────────────");
                    sb.AppendLine($"t₀ = (y_target - y₀) / Δy₀ = ({yTarget} - {y0}) / {delta1} = {step.t_n}\n");
                }
                else
                {
                    sb.Append($"t_{step.iteration} = φ(t_{step.iteration - 1}) = {(yTarget - y0) / delta1}");

                    if (step.details.Count > 0)
                    {
                        sb.Append($" - (1/{delta1}) × [");

                        var terms = new List<string>();
                        foreach (var detail in step.details)
                        {
                            var prodTerms = new List<string>();
                            for (int i = 0; i < detail.r; i++)
                            {
                                prodTerms.Add($"({step.t_prev} - {i})");
                            }

                            string termStr = $"({detail.deltaR}/{detail.factorial}) × {string.Join(" × ", prodTerms)}";
                            terms.Add(termStr);
                        }

                        sb.Append(string.Join(" + ", terms));
                        sb.Append("]");
                    }

                    sb.AppendLine($" = {step.t_n}");
                    sb.AppendLine($"  → Sai số = |t_{step.iteration} - t_{step.iteration - 1}| = {step.error}\n");
                }
            }

            var lastStep = iterationSteps[iterationSteps.Count - 1];
            if (lastStep.iteration >= 100)
            {
                sb.AppendLine("⚠ Đã đạt số vòng lặp tối đa: 100\n");
            }

            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("KẾT QUẢ CUỐI CÙNG:");
            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine($"Số vòng lặp: {lastStep.iteration}");
            sb.AppendLine($"t_final = {lastStep.t_n}");
            sb.AppendLine($"\nTính giá trị x:");
            sb.AppendLine($"x = x₀ + h × t_n = {x[0]} + {h} × {lastStep.t_n} = {result}");
            sb.AppendLine("═══════════════════════════════════════════════════════════════");

            rtb.AppendText(sb.ToString());
        }
        private void DisplayLeastSquaresResults(RichTextBox rtb, string[] phiExpressions, double[,] thetaMatrix, double[,] M, double[] b, double[] coeffs, double MSE, string equation, double[] x, double[] y, double[] yPred)
        {
            rtb.Clear();
            var sb = new StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("KẾT QUẢ PHƯƠNG PHÁP BÌNH PHƯƠNG TỐI THIỂU");
            sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

            // 1. Hiển thị tập cơ sở
            sb.AppendLine("TẬP HÀM CƠ SỞ:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            for (int j = 0; j < phiExpressions.Length; j++)
            {
                sb.AppendLine($"  φ{j + 1}(x) = {phiExpressions[j]}");
            }
            sb.AppendLine();

            // 2. Hiển thị ma trận Theta
            sb.AppendLine("MA TRẬN Θ (THETA):");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine("Θ[i,j] = φj(xi), với i = 1..n (số điểm), j = 1..m (số hàm cơ sở)");
            sb.AppendLine();

            // Header
            sb.Append("   i |");
            for (int j = 0; j < phiExpressions.Length; j++)
            {
                sb.Append($"    φ{j + 1}(xi)    |");
            }
            sb.AppendLine();
            sb.AppendLine(new string('─', 10 + phiExpressions.Length * 16));

            // Data rows
            for (int i = 0; i < thetaMatrix.GetLength(0); i++)
            {
                sb.Append($"  {i + 1,2} |");
                for (int j = 0; j < thetaMatrix.GetLength(1); j++)
                {
                    sb.Append($" {thetaMatrix[i, j],12:F6} |");
                }
                sb.AppendLine();
            }
            sb.AppendLine();

            // 3. Hiển thị ma trận M = Θ^T × Θ
            sb.AppendLine("MA TRẬN M = Θ^T × Θ:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            for (int i = 0; i < M.GetLength(0); i++)
            {
                sb.Append("  [");
                for (int j = 0; j < M.GetLength(1); j++)
                {
                    sb.Append($" {M[i, j],12:F6}");
                }
                sb.AppendLine(" ]");
            }
            sb.AppendLine();

            // 4. Hiển thị vector b = Θ^T × y
            sb.AppendLine("VECTOR b = Θ^T × y:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.Append("  b = [");
            for (int i = 0; i < b.Length; i++)
            {
                sb.Append($" {b[i],12:F6}");
                if (i < b.Length - 1) sb.Append(",");
            }
            sb.AppendLine(" ]^T\n");

            // 5. Hiển thị hệ số (giải M × a = b)
            sb.AppendLine("HỆ SỐ TÌM ĐƯỢC (Giải M × a = b):");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            for (int i = 0; i < coeffs.Length; i++)
            {
                sb.AppendLine($"  a{i + 1} = {coeffs[i]:F8}");
            }
            sb.AppendLine();

            // 6. Hiển thị phương trình
            sb.AppendLine("PHƯƠNG TRÌNH XẤP XỈ:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  {equation}");
            sb.AppendLine();

            sb.AppendLine("GIÁ TRỊ Y SAU KHI THAY VÀO PHƯƠNG TRÌNH TÌM ĐƯỢC");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            for (int i = 0; i < x.Length; i++)
            {
                sb.AppendLine($"{yPred[i],12:F6}");
            }
            sb.AppendLine();

            // 7. Hiển thị MSE
            sb.AppendLine("SAI SỐ TRUNG BÌNH PHƯƠNG:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  MSE = {MSE:F8}");
            sb.AppendLine();

            rtb.AppendText(sb.ToString());
        }
        private void DisplayPowerLawLeastSquaresResults(RichTextBox rtb, double[,] thetaMatrix, double[,] M, double[] r, double a, double p, double MSE, double[] x, double[] y)
        {
            rtb.Clear();
            var sb = new StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("KẾT QUẢ PHƯƠNG PHÁP BÌNH PHƯƠNG TỐI THIỂU");
            sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

            // 1. Hiển thị mô hình
            sb.AppendLine("MÔ HÌNH:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine("  y = a × x^p");
            sb.AppendLine("  Biến đổi logarit: ln(y) = ln(a) + p × ln(x)");
            sb.AppendLine("  Đặt: Y = ln(y), X = ln(x), A₀ = ln(a), A₁ = p");
            sb.AppendLine("  => Y = A₀ + A₁ × X\n");

            // 2. Hiển thị ma trận Theta (sau khi ln)
            sb.AppendLine("MA TRẬN Θ SAU KHI BIẾN ĐỔI:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine("Θ = [1, ln(xᵢ)], với i = 1..n (số điểm dữ liệu)");
            sb.AppendLine();

            // Header
            sb.AppendLine("   i |      1       |    ln(xi)    |");
            sb.AppendLine(new string('─', 40));

            // Data rows
            for (int i = 0; i < thetaMatrix.GetLength(0); i++)
            {
                sb.Append($"  {i + 1,2} |");
                for (int j = 0; j < thetaMatrix.GetLength(1); j++)
                {
                    sb.Append($" {thetaMatrix[i, j],12:F6} |");
                }
                sb.AppendLine();
            }
            sb.AppendLine();

            // 3. Hiển thị dữ liệu gốc và sau biến đổi
            sb.AppendLine("DỮ LIỆU GỐC VÀ SAU BIẾN ĐỔI:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine("   i |      x       |      y       |    ln(x)     |    ln(y)     |");
            sb.AppendLine(new string('─', 70));

            for (int i = 0; i < x.Length; i++)
            {
                sb.AppendLine($"  {i + 1,2} | {x[i],12:F6} | {y[i],12:F6} | {Math.Log(x[i]),12:F6} | {Math.Log(Math.Abs(y[i])),12:F6} |");
            }
            sb.AppendLine();

            // 4. Hiển thị ma trận M = Θ^T × Θ
            sb.AppendLine("MA TRẬN M = Θ^T × Θ:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            for (int i = 0; i < M.GetLength(0); i++)
            {
                sb.Append("  [");
                for (int j = 0; j < M.GetLength(1); j++)
                {
                    sb.Append($" {M[i, j],12:F6}");
                }
                sb.AppendLine(" ]");
            }
            sb.AppendLine();

            // 5. Hiển thị vector r = Θ^T × Y
            sb.AppendLine("VECTOR r = Θ^T × Y:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.Append("  r = [");
            for (int i = 0; i < r.Length; i++)
            {
                sb.Append($" {r[i],12:F6}");
                if (i < r.Length - 1) sb.Append(",");
            }
            sb.AppendLine(" ]^T\n");

            // 6. Hiển thị hệ số (giải M × A = r)
            sb.AppendLine("HỆ SỐ TÌM ĐƯỢC (Giải M × A = r):");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  A₀ = ln(a) = {Math.Log(Math.Abs(a)):F8}");
            sb.AppendLine($"  A₁ = b     = {p:F8}");
            sb.AppendLine();

            // 7. Hiển thị tham số cuối cùng
            sb.AppendLine("THAM SỐ CUỐI CÙNG:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  a = {a:F8}");
            sb.AppendLine($"  b = {p:F8}");
            sb.AppendLine();

            // 8. Hiển thị phương trình
            sb.AppendLine("PHƯƠNG TRÌNH XẤP XỈ:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  y ≈ {a:F8} × x^({p:F8})");
            sb.AppendLine();

            // 9.Hiển thị MSE
            sb.AppendLine("SAI SỐ TRUNG BÌNH PHƯƠNG:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  MSE = {MSE:F8}");
            sb.AppendLine("  (Tính trên không gian logarit: ln(y))");
            sb.AppendLine();

            rtb.AppendText(sb.ToString());
        }
        private void DisplayExponentialLeastSquaresResults(RichTextBox rtb, string[] phiExpressions, double[,] thetaMatrix, double[,] M, double[] r, double a, double[] b, double MSE, double[] x, double[] y)
        {
            rtb.Clear();
            var sb = new StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("KẾT QUẢ PHƯƠNG PHÁP BÌNH PHƯƠNG TỐI THIỂU DẠNG MÕ");
            sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

            // 1. Hiển thị mô hình
            sb.AppendLine("MÔ HÌNH:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.Append("  y = a × e^(");
            var expTerms = new List<string>();
            for (int j = 0; j < phiExpressions.Length; j++)
            {
                expTerms.Add($"b{j + 1}×{phiExpressions[j]}");
            }
            sb.Append(string.Join(" + ", expTerms));
            sb.AppendLine(")");

            sb.Append("  Biến đổi logarit: ln(|y|) = ln(a) + ");
            var lnTerms = new List<string>();
            for (int j = 0; j < phiExpressions.Length; j++)
            {
                lnTerms.Add($"b{j + 1}×{phiExpressions[j]}");
            }
            sb.AppendLine(string.Join(" + ", lnTerms));

            sb.AppendLine("  Đặt: Y = ln(|y|), C₀ = ln(|a|), C₁ = b₁, C₂ = b₂, ...");
            sb.Append("  => Y = C₀ + C₁×φ₁(x)");
            if (phiExpressions.Length > 1)
            {
                for (int j = 1; j < phiExpressions.Length; j++)
                {
                    sb.Append($" + C{j + 1}×φ{j + 1}(x)");
                }
            }
            sb.AppendLine("\n");

            // 2. Hiển thị ma trận Theta (sau khi ln)
            sb.AppendLine("MA TRẬN Θ SAU KHI BIẾN ĐỔI:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.Append("Θ = [1");
            for (int j = 0; j < phiExpressions.Length; j++)
            {
                sb.Append($", φ{j + 1}(xᵢ)");
            }
            sb.AppendLine("], với i = 1..n (số điểm dữ liệu)");
            sb.AppendLine();

            // Header
            sb.Append("   i |      1       |");
            for (int j = 0; j < phiExpressions.Length; j++)
            {
                sb.Append($"    φ{j + 1}(xi)    |");
            }
            sb.AppendLine();
            sb.AppendLine(new string('─', 20 + phiExpressions.Length * 16));

            // Data rows
            for (int i = 0; i < thetaMatrix.GetLength(0); i++)
            {
                sb.Append($"  {i + 1,2} |");
                for (int j = 0; j < thetaMatrix.GetLength(1); j++)
                {
                    sb.Append($" {thetaMatrix[i, j],12:F6} |");
                }
                sb.AppendLine();
            }
            sb.AppendLine();

            // 3. Hiển thị dữ liệu gốc và sau biến đổi
            sb.AppendLine("DỮ LIỆU GỐC VÀ SAU BIẾN ĐỔI:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");

            // Build header dynamically
            sb.Append("   i |      x       |      y       |");
            for (int j = 0; j < phiExpressions.Length; j++)
            {
                sb.Append($"   φ{j + 1}(x)     |");
            }
            sb.AppendLine("   ln(|y|)    |");
            sb.AppendLine(new string('─', 40 + phiExpressions.Length * 14 + 14));

            for (int i = 0; i < x.Length; i++)
            {
                sb.Append($"  {i + 1,2} | {x[i],12:F6} | {y[i],12:F6} |");
                for (int j = 0; j < phiExpressions.Length; j++)
                {
                    sb.Append($" {thetaMatrix[i, j + 1],12:F6} |");
                }
                sb.AppendLine($" {Math.Log(Math.Abs(y[i])),12:F6} |");
            }
            sb.AppendLine();

            // 4. Hiển thị ma trận M = Θ^T × Θ
            sb.AppendLine("MA TRẬN M = Θ^T × Θ:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            for (int i = 0; i < M.GetLength(0); i++)
            {
                sb.Append("  [");
                for (int j = 0; j < M.GetLength(1); j++)
                {
                    sb.Append($" {M[i, j],12:F6}");
                }
                sb.AppendLine(" ]");
            }
            sb.AppendLine();

            // 5. Hiển thị vector r = Θ^T × Y
            sb.AppendLine("VECTOR r = Θ^T × Y:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.Append("  r = [");
            for (int i = 0; i < r.Length; i++)
            {
                sb.Append($" {r[i],12:F6}");
                if (i < r.Length - 1) sb.Append(",");
            }
            sb.AppendLine(" ]^T\n");

            // 6. Hiển thị hệ số (giải M × C = r)
            sb.AppendLine("HỆ SỐ TÌM ĐƯỢC (Giải M × C = r):");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  C₀ = ln(|a|) = {Math.Log(Math.Abs(a)):F8}");
            for (int j = 0; j < b.Length; j++)
            {
                sb.AppendLine($"  C{j + 1} = b{j + 1}     = {b[j]:F8}");
            }
            sb.AppendLine();

            // 7. Hiển thị tham số cuối cùng
            sb.AppendLine("THAM SỐ CUỐI CÙNG:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  a  = {a:F8}");
            for (int j = 0; j < b.Length; j++)
            {
                sb.AppendLine($"  b{j + 1} = {b[j]:F8}");
            }
            sb.AppendLine();

            // 8. Hiển thị phương trình
            sb.AppendLine("PHƯƠNG TRÌNH XẤP XỈ:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.Append($"  y ≈ {a:F8} × e^(");
            var finalTerms = new List<string>();
            for (int j = 0; j < phiExpressions.Length; j++)
            {
                string sign = b[j] >= 0 ? "+" : "";
                finalTerms.Add($"{sign}{b[j]:F8}×({phiExpressions[j]})");
            }
            sb.Append(string.Join(" ", finalTerms));
            sb.AppendLine(")");
            sb.AppendLine();

            // 9. MSE
            sb.AppendLine("SAI SỐ TRUNG BÌNH PHƯƠNG:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"  MSE = {MSE:F8}");
            sb.AppendLine("  (Tính trên không gian logarit: ln(y))");
            sb.AppendLine();
            rtb.AppendText(sb.ToString());
        }
        // Spline 

        //private void DisplayLinearSplineResults(RichTextBox rtb, double[] x, double[] y, double[] a, double[] b, double[] h, int precision)
        //{
        //    rtb.Clear();
        //    var sb = new StringBuilder();

        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine("KẾT QUẢ SPLINE TUYẾN TÍNH (SPLINE CẤP 1)");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

        //    // 1. Hiển thị dữ liệu đầu vào
        //    sb.AppendLine("DỮ LIỆU ĐẦU VÀO:");
        //    sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    sb.AppendLine("   i |      x[i]    |      y[i]    |");
        //    sb.AppendLine(new string('─', 40));

        //    for (int i = 0; i < x.Length; i++)
        //    {
        //        sb.AppendLine($"  {i,2} | {x[i],12:F6} | {y[i],12:F6} |");
        //    }
        //    sb.AppendLine();

        //    // 2. Hiển thị thông tin từng đoạn spline
        //    sb.AppendLine("CHI TIẾT CÁC ĐOẠN SPLINE:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");

        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        sb.AppendLine($"\nĐOẠN {i + 1}: [{x[i]:F6}, {x[i + 1]:F6}]");
        //        sb.AppendLine("───────────────────────────────────────────────────────────────");
        //        sb.AppendLine($"  h[{i}] = x[{i + 1}] - x[{i}]");
        //        sb.AppendLine($"      = {x[i + 1]:F6} - {x[i]:F6}");
        //        sb.AppendLine($"      = {h[i]:F6}");
        //        sb.AppendLine();

        //        sb.AppendLine($"  a[{i}] = (y[{i + 1}] - y[{i}]) / h[{i}]");
        //        sb.AppendLine($"      = ({y[i + 1]:F6} - {y[i]:F6}) / {h[i]:F6}");
        //        sb.AppendLine($"      = {a[i]:F6}");
        //        sb.AppendLine();

        //        sb.AppendLine($"  b[{i}] = (y[{i}] × x[{i + 1}] - y[{i + 1}] × x[{i}]) / h[{i}]");
        //        sb.AppendLine($"      = ({y[i]:F6} × {x[i + 1]:F6} - {y[i + 1]:F6} × {x[i]:F6}) / {h[i]:F6}");
        //        sb.AppendLine($"      = ({y[i] * x[i + 1]:F6} - {y[i + 1] * x[i]:F6}) / {h[i]:F6}");
        //        sb.AppendLine($"      = {b[i]:F6}");
        //        sb.AppendLine();

        //        string sign = b[i] >= 0 ? "+" : "";
        //        sb.AppendLine($"  Phương trình: S_{i}(x) = {a[i]:F6}×x {sign} {b[i]:F6}");
        //        sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    }

        //    // 3. Tổng hợp thông số
        //    sb.AppendLine("\nBẢNG TỔNG HỢP THÔNG SỐ:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine("  k |  Khoảng [x_k, x_{k+1}]  |    h_k     |     a_k      |     b_k      |");
        //    sb.AppendLine(new string('─', 80));

        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        sb.AppendLine($" {i,2} | [{x[i],6:F3}, {x[i + 1],6:F3}] | {h[i],10:F6} | {a[i],12:F6} | {b[i],12:F6} |");
        //    }
        //    sb.AppendLine();

        //    // 4. Danh sách phương trình
        //    sb.AppendLine("CÁC PHƯƠNG TRÌNH ĐOẠN:");
        //    sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        string sign = b[i] >= 0 ? "+" : "";
        //        sb.AppendLine($"  S_{i}(x) = {a[i]:F6}×x {sign} {b[i]:F6}  trên [{x[i]:F3}, {x[i + 1]:F3}]");
        //    }
        //    sb.AppendLine();

        //    rtb.AppendText(sb.ToString());
        //}
        //private void DisplayQuadraticSplineResults(RichTextBox rtb, double[] x, double[] y, double[] a, double[] b, double[] c, double[] h, double[] m, double[] v, int precision)
        //{
        //    rtb.Clear();
        //    var sb = new StringBuilder();

        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine("KẾT QUẢ SPLINE BẬC HAI (SPLINE CẤP 2)");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

        //    // 1. Hiển thị dữ liệu đầu vào
        //    sb.AppendLine("DỮ LIỆU ĐẦU VÀO:");
        //    sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    sb.AppendLine("   i |      x[i]    |      y[i]    |");
        //    sb.AppendLine(new string('─', 40));

        //    for (int i = 0; i < x.Length; i++)
        //    {
        //        sb.AppendLine($"  {i,2} | {x[i],12:F6} | {y[i],12:F6} |");
        //    }
        //    sb.AppendLine();

        //    // 2. Hiển thị điều kiện biên
        //    sb.AppendLine("ĐIỀU KIỆN BIÊN:");
        //    sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    sb.AppendLine($"  S'(x₀) = m[0] = {m[0]:F6}");
        //    if (m[0] == 0)
        //    {
        //        sb.AppendLine("  (Điều kiện mặc định: đạo hàm bậc nhất tại điểm đầu bằng 0)");
        //    }
        //    else
        //    {
        //        sb.AppendLine("  (Điều kiện biên do người dùng chỉ định)");
        //    }
        //    sb.AppendLine();

        //    // 3. Hiển thị tính toán h và v
        //    sb.AppendLine("TÍNH TOÁN KHOẢNG CÁCH h VÀ HỆ SỐ v:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");

        //    for (int i = 0; i < h.Length; i++)
        //    {
        //        sb.AppendLine($"\nĐOẠN {i + 1}:");
        //        sb.AppendLine("───────────────────────────────────────────────────────────────");
        //        sb.AppendLine($"  h[{i}] = x[{i + 1}] - x[{i}]");
        //        sb.AppendLine($"      = {x[i + 1]:F6} - {x[i]:F6}");
        //        sb.AppendLine($"      = {h[i]:F6}");
        //        sb.AppendLine();

        //        sb.AppendLine($"  v[{i}] = 2 × (y[{i + 1}] - y[{i}]) / h[{i}]");
        //        sb.AppendLine($"      = 2 × ({y[i + 1]:F6} - {y[i]:F6}) / {h[i]:F6}");
        //        sb.AppendLine($"      = 2 × {y[i + 1] - y[i]:F6} / {h[i]:F6}");
        //        sb.AppendLine($"      = {v[i]:F6}");
        //    }
        //    sb.AppendLine();

        //    // 4. Hiển thị tính toán m (hệ số đạo hàm)
        //    sb.AppendLine("TÍNH TOÁN HỆ SỐ ĐẠO HÀM m:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine($"  m[0] = 0 (điều kiện biên)");
        //    sb.AppendLine();

        //    for (int i = 0; i < m.Length - 1; i++)
        //    {
        //        sb.AppendLine($"  m[{i + 1}] = v[{i}] - m[{i}]");
        //        sb.AppendLine($"        = {v[i]:F6} - {m[i]:F6}");
        //        sb.AppendLine($"        = {m[i + 1]:F6}");
        //        sb.AppendLine();
        //    }

        //    // 5. Hiển thị chi tiết tính toán các hệ số a, b, c
        //    sb.AppendLine("CHI TIẾT TÍNH TOÁN CÁC HỆ SỐ:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");

        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        sb.AppendLine($"\nĐOẠN {i + 1}: [{x[i]:F6}, {x[i + 1]:F6}]");
        //        sb.AppendLine("───────────────────────────────────────────────────────────────");

        //        // Tính a[i]
        //        sb.AppendLine($"  a[{i}] = (m[{i + 1}] - m[{i}]) / (2 × h[{i}])");
        //        sb.AppendLine($"      = ({m[i + 1]:F6} - {m[i]:F6}) / (2 × {h[i]:F6})");
        //        sb.AppendLine($"      = {m[i + 1] - m[i]:F6} / {2 * h[i]:F6}");
        //        sb.AppendLine($"      = {a[i]:F6}");
        //        sb.AppendLine();

        //        // Tính b[i]
        //        sb.AppendLine($"  b[{i}] = (m[{i}] × x[{i + 1}] - m[{i + 1}] × x[{i}]) / h[{i}]");
        //        sb.AppendLine($"      = ({m[i]:F6} × {x[i + 1]:F6} - {m[i + 1]:F6} × {x[i]:F6}) / {h[i]:F6}");
        //        sb.AppendLine($"      = ({m[i] * x[i + 1]:F6} - {m[i + 1] * x[i]:F6}) / {h[i]:F6}");
        //        sb.AppendLine($"      = {b[i]:F6}");
        //        sb.AppendLine();

        //        // Tính c[i]
        //        double term1 = -m[i] * x[i + 1] * x[i + 1];
        //        double term2 = m[i + 1] * x[i] * x[i];
        //        double term3 = y[i];
        //        double term4 = m[i] * h[i] / 2;

        //        sb.AppendLine($"  c[{i}] = (-m[{i}] × x[{i + 1}]² + m[{i + 1}] × x[{i}]²) / (2 × h[{i}]) + y[{i}] + (m[{i}] × h[{i}]) / 2");
        //        sb.AppendLine($"      = ({-m[i]:F6} × {x[i + 1] * x[i + 1]:F6} + {m[i + 1]:F6} × {x[i] * x[i]:F6}) / {2 * h[i]:F6} + {y[i]:F6} + {m[i] * h[i] / 2:F6}");
        //        sb.AppendLine($"      = ({term1:F6} + {term2:F6}) / {2 * h[i]:F6} + {term3:F6} + {term4:F6}");
        //        sb.AppendLine($"      = {(term1 + term2) / (2 * h[i]):F6} + {term3 + term4:F6}");
        //        sb.AppendLine($"      = {c[i]:F6}");
        //        sb.AppendLine();

        //        sb.AppendLine($"  Phương trình: S_{i}(x) = {a[i]:F6}×x² + {b[i]:F6}×x + {c[i]:F6}");
        //        sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    }

        //    // 6. Bảng tổng hợp thông số
        //    sb.AppendLine("\nBẢNG TỔNG HỢP THÔNG SỐ:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine("  k |  Khoảng [x_k, x_{k+1}]  |    h_k     |    m_k     |    m_{k+1}   |");
        //    sb.AppendLine(new string('─', 85));

        //    for (int i = 0; i < h.Length; i++)
        //    {
        //        sb.AppendLine($" {i,2} | [{x[i],6:F3}, {x[i + 1],6:F3}] | {h[i],10:F6} | {m[i],10:F6} | {m[i + 1],12:F6} |");
        //    }
        //    sb.AppendLine();

        //    sb.AppendLine("  k |     a_k      |     b_k      |     c_k      |");
        //    sb.AppendLine(new string('─', 55));

        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        sb.AppendLine($" {i,2} | {a[i],12:F6} | {b[i],12:F6} | {c[i],12:F6} |");
        //    }
        //    sb.AppendLine();

        //    // 7. Danh sách phương trình
        //    sb.AppendLine("CÁC PHƯƠNG TRÌNH ĐOẠN:");
        //    sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        string signB = b[i] >= 0 ? "+" : "";
        //        string signC = c[i] >= 0 ? "+" : "";
        //        sb.AppendLine($"  S_{i}(x) = {a[i]:F6}×x² {signB} {b[i]:F6}×x {signC} {c[i]:F6}");
        //        sb.AppendLine($"           trên [{x[i]:F3}, {x[i + 1]:F3}]");
        //    }
        //    sb.AppendLine();
        //    rtb.AppendText(sb.ToString());
        //}
        //private void DisplayCubicSplineResults(RichTextBox rtb, double[] x, double[] y, double[] a, double[] b, double[] c, double[] d, double[] h, double[] alpha, int precision)
        //{
        //    rtb.Clear();
        //    var sb = new StringBuilder();

        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine("KẾT QUẢ SPLINE BẬC BA (SPLINE CẤP 3 - NATURAL SPLINE)");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

        //    // 1. Hiển thị dữ liệu đầu vào
        //    sb.AppendLine("DỮ LIỆU ĐẦU VÀO:");
        //    sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    sb.AppendLine("   i |      x[i]    |      y[i]    |");
        //    sb.AppendLine(new string('─', 40));

        //    for (int i = 0; i < x.Length; i++)
        //    {
        //        sb.AppendLine($"  {i,2} | {x[i],12:F6} | {y[i],12:F6} |");
        //    }
        //    sb.AppendLine();

        //    // 2. Hiển thị điều kiện biên
        //    sb.AppendLine("ĐIỀU KIỆN BIÊN (NATURAL SPLINE):");
        //    sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    sb.AppendLine($"  S''(x₀) = α[0] = 0");
        //    sb.AppendLine($"  S''(x_n) = α[n] = 0");
        //    sb.AppendLine("  (Đạo hàm cấp hai tại điểm đầu và cuối bằng 0)\n");

        //    // 3. Tính toán h
        //    sb.AppendLine("BƯỚC 1: TÍNH KHOẢNG CÁCH h:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    for (int i = 0; i < h.Length; i++)
        //    {
        //        sb.AppendLine($"  h[{i}] = x[{i + 1}] - x[{i}] = {x[i + 1]:F6} - {x[i]:F6} = {h[i]:F6}");
        //    }
        //    sb.AppendLine();

        //    // 4. Tính gamma (vế phải của hệ phương trình)
        //    sb.AppendLine("BƯỚC 2: TÍNH VẾ PHẢI γ CỦA HỆ PHƯƠNG TRÌNH:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    for (int i = 0; i < x.Length - 2; i++)
        //    {
        //        double term1 = (y[i + 2] - y[i + 1]) / h[i + 1];
        //        double term2 = (y[i + 1] - y[i]) / h[i];
        //        double gamma = term1 - term2;

        //        sb.AppendLine($"  γ[{i}] = (y[{i + 2}] - y[{i + 1}])/h[{i + 1}] - (y[{i + 1}] - y[{i}])/h[{i}]");
        //        sb.AppendLine($"       = ({y[i + 2]:F6} - {y[i + 1]:F6})/{h[i + 1]:F6} - ({y[i + 1]:F6} - {y[i]:F6})/{h[i]:F6}");
        //        sb.AppendLine($"       = {term1:F6} - {term2:F6}");
        //        sb.AppendLine($"       = {gamma:F6}");
        //        sb.AppendLine();
        //    }

        //    // 5. Hiển thị hệ phương trình tam đường chéo
        //    sb.AppendLine("BƯỚC 3: XÂY DỰNG HỆ PHƯƠNG TRÌNH TAM ĐƯỜNG CHÉO:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine();

        //    int numEq = x.Length - 2;
        //    sb.AppendLine("   i | Đường chéo dưới | Đường chéo chính | Đường chéo trên |");
        //    sb.AppendLine(new string('─', 75));

        //    for (int i = 0; i < numEq; i++)
        //    {
        //        double subDiag = (i == 0) ? 0 : h[i] / 6.0;
        //        double mainDiag = (h[i] + h[i + 1]) / 3.0;
        //        double superDiag = (i == numEq - 1) ? 0 : h[i + 1] / 6.0;

        //        sb.AppendLine($"  {i,2} | {subDiag,16:F6} | {mainDiag,16:F6} | {superDiag,15:F6} |");
        //    }
        //    sb.AppendLine();

        //    // 6. Hiển thị giải hệ và alpha
        //    sb.AppendLine("BƯỚC 4: GIẢI HỆ PHƯƠNG TRÌNH TÌM α (S''(x_i)):");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine();
        //    sb.AppendLine("   i |      α[i]     |");
        //    sb.AppendLine(new string('─', 25));

        //    for (int i = 0; i < alpha.Length; i++)
        //    {
        //        string note = "";
        //        if (i == 0) note = " (điều kiện biên)";
        //        if (i == alpha.Length - 1) note = " (điều kiện biên)";

        //        sb.AppendLine($"  {i,2} | {alpha[i],13:F6} |{note}");
        //    }
        //    sb.AppendLine();

        //    // 7. Tính các hệ số a, b, c, d
        //    sb.AppendLine("BƯỚC 5: TÍNH CÁC HỆ SỐ a, b, c, d:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");

        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        sb.AppendLine($"\nĐOẠN {i + 1}: [{x[i]:F6}, {x[i + 1]:F6}]");
        //        sb.AppendLine("───────────────────────────────────────────────────────────────");

        //        double h_i = h[i];
        //        double x_i = x[i];
        //        double x_i1 = x[i + 1];
        //        double y_i = y[i];
        //        double y_i1 = y[i + 1];
        //        double alpha_i = alpha[i];
        //        double alpha_i1 = alpha[i + 1];

        //        // a[i]
        //        sb.AppendLine($"  a[{i}] = (α[{i + 1}] - α[{i}]) / (6 × h[{i}])");
        //        sb.AppendLine($"       = ({alpha_i1:F6} - {alpha_i:F6}) / (6 × {h_i:F6})");
        //        sb.AppendLine($"       = {alpha_i1 - alpha_i:F6} / {6 * h_i:F6}");
        //        sb.AppendLine($"       = {a[i]:F6}");
        //        sb.AppendLine();

        //        // b[i]
        //        sb.AppendLine($"  b[{i}] = (α[{i}] × x[{i + 1}] - α[{i + 1}] × x[{i}]) / (2 × h[{i}])");
        //        sb.AppendLine($"       = ({alpha_i:F6} × {x_i1:F6} - {alpha_i1:F6} × {x_i:F6}) / (2 × {h_i:F6})");
        //        sb.AppendLine($"       = ({alpha_i * x_i1:F6} - {alpha_i1 * x_i:F6}) / {2 * h_i:F6}");
        //        sb.AppendLine($"       = {b[i]:F6}");
        //        sb.AppendLine();

        //        // c[i]
        //        double c_term1 = (-alpha_i * x_i1 * x_i1 + alpha_i1 * x_i * x_i) / (2 * h_i);
        //        double c_term2 = (y_i1 - y_i) / h_i;
        //        double c_term3 = (-(alpha_i1 - alpha_i) * h_i) / 6;

        //        sb.AppendLine($"  c[{i}] = (-α[{i}] × x[{i + 1}]² + α[{i + 1}] × x[{i}]²) / (2 × h[{i}]) + (y[{i + 1}] - y[{i}])/h[{i}] - (α[{i + 1}] - α[{i}]) × h[{i}]/6");
        //        sb.AppendLine($"       = {c_term1:F6} + {c_term2:F6} + ({c_term3:F6})");
        //        sb.AppendLine($"       = {c[i]:F6}");
        //        sb.AppendLine();

        //        // d[i]
        //        double d_term1 = (alpha_i * x_i1 * x_i1 * x_i1 - alpha_i1 * x_i * x_i * x_i) / (6 * h_i);
        //        double d_term2 = (y_i * x_i1 - y_i1 * x_i) / h_i;
        //        double d_term3 = ((alpha_i1 * x_i - alpha_i * x_i1) * h_i) / 6;

        //        sb.AppendLine($"  d[{i}] = (α[{i}] × x[{i + 1}]³ - α[{i + 1}] × x[{i}]³) / (6 × h[{i}]) + (y[{i}] × x[{i + 1}] - y[{i + 1}] × x[{i}])/h[{i}] + (α[{i + 1}] × x[{i}] - α[{i}] × x[{i + 1}]) × h[{i}]/6");
        //        sb.AppendLine($"       = {d_term1:F6} + {d_term2:F6} + {d_term3:F6}");
        //        sb.AppendLine($"       = {d[i]:F6}");
        //        sb.AppendLine();

        //        sb.AppendLine($"  Phương trình: S_{i}(x) = {a[i]:F6}×x³ + {b[i]:F6}×x² + {c[i]:F6}×x + {d[i]:F6}");
        //        sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    }

        //    // 8. Bảng tổng hợp
        //    sb.AppendLine("\nBẢNG TỔNG HỢP THÔNG SỐ:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine("  k |  Khoảng [x_k, x_{k+1}]  |    h_k     |    α_k     |   α_{k+1}  |");
        //    sb.AppendLine(new string('─', 85));

        //    for (int i = 0; i < h.Length; i++)
        //    {
        //        sb.AppendLine($" {i,2} | [{x[i],6:F3}, {x[i + 1],6:F3}] | {h[i],10:F6} | {alpha[i],10:F6} | {alpha[i + 1],10:F6} |");
        //    }
        //    sb.AppendLine();

        //    sb.AppendLine("  k |     a_k      |     b_k      |     c_k      |     d_k      |");
        //    sb.AppendLine(new string('─', 70));

        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        sb.AppendLine($" {i,2} | {a[i],12:F6} | {b[i],12:F6} | {c[i],12:F6} | {d[i],12:F6} |");
        //    }
        //    sb.AppendLine();

        //    // 9. Danh sách phương trình
        //    sb.AppendLine("CÁC PHƯƠNG TRÌNH ĐOẠN:");
        //    sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        string signB = b[i] >= 0 ? "+" : "";
        //        string signC = c[i] >= 0 ? "+" : "";
        //        string signD = d[i] >= 0 ? "+" : "";

        //        sb.AppendLine($"  S_{i}(x) = {a[i]:F6}×x³ {signB} {b[i]:F6}×x² {signC} {c[i]:F6}×x {signD} {d[i]:F6}");
        //        sb.AppendLine($"           trên [{x[i]:F3}, {x[i + 1]:F3}]");
        //    }
        //    sb.AppendLine();
        //    rtb.AppendText(sb.ToString());
        //}
        #endregion

        #region Utilities
        private double[] GetXValues(DataGridView dataGridView)
        {
            int rows = dataGridView.Rows.Count - 1;
            double[] x = new double[rows];
            for (int i = 0; i < rows; i++)
            {
                x[i] = Convert.ToDouble(dataGridView.Rows[i].Cells[0].Value);
            }
            return x;
        }
        private double[] GetYValues(DataGridView dataGridView)
        {
            int rows = dataGridView.Rows.Count - 1;
            double[] y = new double[rows];
            for (int i = 0; i < rows; i++)
            {
                y[i] = Convert.ToDouble(dataGridView.Rows[i].Cells[1].Value);
            }
            return y;
        }
        private double[] GetCoeffsP(DataGridView dataGridView)
        {
            int rows = dataGridView.Rows.Count - 1;
            double[] coeffsP = new double[rows];
            for (int i = 0; i < rows; i++)
            {
                coeffsP[i] = Convert.ToDouble(dataGridView.Rows[rows - 1 - i].Cells[0].Value);
            }
            return coeffsP;
        }
        private void RemoveDuplicate(DataGridView dataGridView)
        {
            var seenXValues = new HashSet<double>();
            int rowIndex = 0;
            while (rowIndex < dataGridView.Rows.Count - 1)
            {
                var cellValue = dataGridView.Rows[rowIndex].Cells[0].Value;
                double xValue = Convert.ToDouble(cellValue);
                if (seenXValues.Contains(xValue))
                {
                    dataGridView.Rows.RemoveAt(rowIndex);
                }
                else
                {
                    seenXValues.Add(xValue);
                    rowIndex++;
                }
            }
        }
        private void SetupColumns(DataGridView dgv, int dataCols, string noteHeaderText)
        {
            for (int j = 0; j < dataCols; j++)
            {
                dgv.Columns.Add($"col{j}", $"Cột {j}");
            }
            dgv.Columns.Add("note", noteHeaderText);
            dgv.Columns["note"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

        }
        private void AddRow(DataGridView dgv, double[] data, string note)
        {
            var row = new List<object>();
            for (int i = 0; i < data.Length; i++)
            {
                row.Add(data[i]);
            }
            while (row.Count < dgv.Columns.Count - 1) row.Add(""); // Thêm ô trống tới khi đến cột ghi chú
            row.Add(note);
            dgv.Rows.Add(row.ToArray());
        }
        private void AddTable(DataGridView dgv, double[,] table, string startNote, string endNote = null)
        {
            for (int i = 0; i < table.GetLength(0); i++)
            {
                double[] rowData = new double[table.GetLength(1)];
                for (int j = 0; j < table.GetLength(1); j++) rowData[j] = table[i, j];

                string note = (i == 0) ? startNote : (i == table.GetLength(0) - 1) ? endNote : "";
                AddRow(dgv, rowData, note);
            }
        }
        private void AddNullableTable(DataGridView dgv, double?[,] table, string note)
        {
            for (int i = 0; i < table.GetLength(0); i++)
            {
                object[] row = new object[dgv.Columns.Count];
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    row[j] = table[i, j]?.ToString() ?? "";
                }
                if (i == table.GetLength(0) - 1) row[dgv.Columns.Count - 1] = note;
                dgv.Rows.Add(row);
            }
        }
        private void AddSectionBreak(DataGridView dgv) => dgv.Rows.Add();
        private class HornerResults
        {
            public double Evaluation { get; set; }
            public double[] Derivatives { get; set; }
            public double[] Quotient { get; set; }
            public double Remainder { get; set; }
            public double[] MultipliedPolynomial { get; set; }
            public DataTable EvaluationTable { get; set; }
            public DataTable DerivativeTable { get; set; }
        }
        private double[,] RemoveFirstRowAndLastColumn(double[,] table)
        {
            int rows = table.GetLength(0);
            int cols = table.GetLength(1);

            double[,] res = new double[rows - 1, cols - 1];

            for (int i = 1; i < rows; i++)
            {
                for (int j = 0; j < cols - 1; j++)
                {
                    res[i - 1, j] = table[i, j];
                }
            }
            return res;
        }
        private void TransferCoeffsToEval(DataGridView dgv, double[] polynomialCoeffs)
        {
            dgv.Rows.Clear();
            for (int i = 0; i < polynomialCoeffs.Length; i++)
            {
                dgv.Rows.Add(polynomialCoeffs[i]);
            }
        }
        private void ReadExcelData(string filePath, out double[] xValues, out double[] yValues)
        {
            var xList = new List<double>();
            var yList = new List<double>();
            var seenXValues = new HashSet<double>();
            int duplicateCount = 0;

            FileInfo fileInfo = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension?.Rows ?? 0;
                int startRow = 1;
                for (int row = startRow; row <= rowCount; row++)
                {
                    var xCell = worksheet.Cells[row, 1].Value;
                    var yCell = worksheet.Cells[row, 2].Value;
                    if (xCell != null && yCell != null)
                    {
                        if (double.TryParse(xCell.ToString(), out double xValue) &&
                            double.TryParse(yCell.ToString(), out double yValue))
                        {
                            if (!seenXValues.Contains(xValue))
                            {
                                xList.Add(xValue);
                                yList.Add(yValue);
                                seenXValues.Add(xValue);
                            }
                            else
                            {
                                duplicateCount++;
                            }
                        }
                    }
                }
            }
            xValues = xList.ToArray();
            yValues = yList.ToArray();
            if (xValues.Length == 0)
            {
                throw new Exception("Không tìm thấy dữ liệu hợp lệ");
            }
            if (duplicateCount > 0)
            {
                MessageBox.Show(
                    $"Đã loại bỏ {duplicateCount} điểm có giá trị x trùng lặp.\n" +
                    $"Số điểm còn lại: {xValues.Length}",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        //private double[] SolveTridiagonalSystem(double[] a, double[] b, double[] c, double[] d)
        //{
        //    int n = d.Length;
        //    double[] c_prime = new double[n];
        //    double[] d_prime = new double[n];
        //    double[] x = new double[n];

        //    // --- Quá trình thuận ---
        //    c_prime[0] = c[0] / b[0];
        //    d_prime[0] = d[0] / b[0];

        //    for (int i = 1; i < n; i++)
        //    {
        //        double temp = b[i] - a[i] * c_prime[i - 1];
        //        c_prime[i] = c[i] / temp;
        //        d_prime[i] = (d[i] - a[i] * d_prime[i - 1]) / temp;
        //    }

        //    // --- Quá trình ngược (Back substitution) ---
        //    x[n - 1] = d_prime[n - 1];

        //    for (int i = n - 2; i >= 0; i--)
        //    {
        //        x[i] = d_prime[i] - c_prime[i] * x[i + 1];
        //    }

        //    return x;
        //}
        private void SetupDataGridViewColumnTypes()
        {
            // Gauss I
            if (dataXYGaussI.Columns.Contains("colsXGaussI"))
                dataXYGaussI.Columns["colsXGaussI"].ValueType = typeof(double);
            if (dataXYGaussI.Columns.Contains("colsYGaussI"))
                dataXYGaussI.Columns["colsYGaussI"].ValueType = typeof(double);

            // Newton
            if (dataXYNewton.Columns.Contains("colsXNewton"))
                dataXYNewton.Columns["colsXNewton"].ValueType = typeof(double);
            if (dataXYNewton.Columns.Contains("colsYNewton"))
                dataXYNewton.Columns["colsYNewton"].ValueType = typeof(double);

            // Newton Finite
            if (dataGridViewXYNewtonFinite.Columns.Contains("colsXNewtonFinite"))
                dataGridViewXYNewtonFinite.Columns["colsXNewtonFinite"].ValueType = typeof(double);
            if (dataGridViewXYNewtonFinite.Columns.Contains("colsYNewtonFinite"))
                dataGridViewXYNewtonFinite.Columns["colsYNewtonFinite"].ValueType = typeof(double);

            // Stirling
            if (dataXYStirling.Columns.Contains("colsXStirling"))
                dataXYStirling.Columns["colsXStirling"].ValueType = typeof(double);
            if (dataXYStirling.Columns.Contains("colsYStirling"))
                dataXYStirling.Columns["colsYStirling"].ValueType = typeof(double);

            // Bessel
            if (dataXYBessel.Columns.Contains("colsXBessel"))
                dataXYBessel.Columns["colsXBessel"].ValueType = typeof(double);
            if (dataXYBessel.Columns.Contains("colsYBessel"))
                dataXYBessel.Columns["colsYBessel"].ValueType = typeof(double);

            // Gauss II
            if (dataXYGaussII.Columns.Contains("colsXGaussII"))
                dataXYGaussII.Columns["colsXGaussII"].ValueType = typeof(double);
            if (dataXYGaussII.Columns.Contains("colsYGaussII"))
                dataXYGaussII.Columns["colsYGaussII"].ValueType = typeof(double);

            // Iteration
            if (dataGridViewXYIteration.Columns.Contains("colsXIteration"))
                dataGridViewXYIteration.Columns["colsXIteration"].ValueType = typeof(double);
            if (dataGridViewXYIteration.Columns.Contains("colsYIteration"))
                dataGridViewXYIteration.Columns["colsYIteration"].ValueType = typeof(double);

            // Least Squares
            if (dataXYLeastSquares != null && dataXYLeastSquares.Columns.Contains("colsXLeastSquares"))
                dataXYLeastSquares.Columns["colsXLeastSquares"].ValueType = typeof(double);
            if (dataXYLeastSquares != null && dataXYLeastSquares.Columns.Contains("colsYLeastSquares"))
                dataXYLeastSquares.Columns["colsYLeastSquares"].ValueType = typeof(double);

            // Spline
            if (dataXYSpline != null && dataXYSpline.Columns.Contains("colsXSpline"))
                dataXYSpline.Columns["colsXSpline"].ValueType = typeof(double);
            if (dataXYSpline != null && dataXYSpline.Columns.Contains("colsXSpline"))
                dataXYSpline.Columns["colsXSpline"].ValueType = typeof(double);
        }
        private void ReadExcelDataWithoutDuplicateRemoval(string filePath, out double[] xValues, out double[] yValues)
        {
            var xList = new List<double>();
            var yList = new List<double>();

            FileInfo fileInfo = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension?.Rows ?? 0;
                int startRow = 1;
                for (int row = startRow; row <= rowCount; row++)
                {
                    var xCell = worksheet.Cells[row, 1].Value;
                    var yCell = worksheet.Cells[row, 2].Value;
                    if (xCell != null && yCell != null)
                    {
                        if (double.TryParse(xCell.ToString(), out double xValue) &&
                            double.TryParse(yCell.ToString(), out double yValue))
                        {
                            xList.Add(xValue);
                            yList.Add(yValue);
                        }
                    }
                }
            }
            xValues = xList.ToArray();
            yValues = yList.ToArray();
            if (xValues.Length == 0)
            {
                throw new Exception("Không tìm thấy dữ liệu hợp lệ");
            }
        }
        #endregion
    }
}