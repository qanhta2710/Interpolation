using Interpolation.Methods;
using Interpolation.Utilities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
namespace Interpolation
{
    public partial class Interpolation : Form
    {
        private double[] lastPolynomialCoeffs;
        private double[] xValues;
        private double[] yValues;
        private double lastYTarget;
        private List<(int startIndex, int endIndex)> lastIsolationIntervals;
        private List<(int startIndex, int endIndex, bool isIncreasing, double[] selectedX, double[] selectedY)> lastMonotonicIntervals;
        public Interpolation()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxNewton.SelectedIndex = 0; // Lựa chọn mặc định mốc nội suy bất kì
            comboBoxNewtonFinite.SelectedIndex = 0; // Lựa chọn mặc định mốc nội suy cách đều tăng dần
            comboBoxLeastSquares.SelectedIndex = 0; // Lựa chọn mặc định phương pháp bình phương tối thiểu tuyến tính
            comboBoxIteration.SelectedIndex = 0; // Lựa chọn mặc định phương pháp lặp tiến
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

                double[] res = Chebyshev.FindOptimizedPoints(n, a, b, precision);

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
                var lagrange = new Lagrange(x, y, precision);
                lastPolynomialCoeffs = lagrange.Polynomial;
                lagrange.DisplayResults(dataGridViewLagrange);

                // In ra đa thức nội suy dạng chính tắc
                lblResult.Text = Function.PolynomialToString(lagrange.Polynomial);
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
            OpenExcelAndLoadData(dataXYLagrange);
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
                var newton = new Newton(x, y, precision);
                lastPolynomialCoeffs = newton.Polynomial;
                newton.DisplayResults(dataGridViewNewton);

                // In đa thức nội suy
                lblResultNewton.Text = Function.PolynomialToString(newton.Polynomial);
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
            OpenExcelAndLoadData(dataXYNewton);
        }
        // Tìm đa thức nội suy Newton mốc cách đều
        private void btnSolveNewtonFinite_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridViewXYNewtonFinite.Sort(dataGridViewXYNewtonFinite.Columns["colsXNewtonFinite"], System.ComponentModel.ListSortDirection.Ascending);

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
                var newton = new Newton(x, y, isForward, precision);
                lastPolynomialCoeffs = newton.Polynomial;
                newton.DisplayResults(dataGridViewNewtonFinite);

                // Viết đa thức nội suy dạng chính tắc
                lblNewtonFinite.Text = Function.PolynomialToString(newton.Polynomial, "t");
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
            OpenExcelAndLoadData(dataGridViewXYNewtonFinite);
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
                var result = new Horner(coeffsP, c, k, precision);
                result.DisplayResults(richTextBoxResult, dataGridViewHornerEval, dataGridViewHornerDerivative);
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

                var stirling = new Stirling(x, y, precision);
                lastPolynomialCoeffs = stirling.Polynomial;
                stirling.DisplayResults(dataResultStirling);

                lblStirling.Text = Function.PolynomialToString(stirling.Polynomial, "t");
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
            OpenExcelAndLoadData(dataXYStirling);
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

                var bessel = new Bessel(x, y, precision);
                lastPolynomialCoeffs = bessel.Polynomial;
                bessel.DisplayResults(dataResultBessel);

                lblResultBessel.Text = Function.PolynomialToString(bessel.Polynomial, "t");
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
            OpenExcelAndLoadData(dataXYBessel);
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

                var gaussI = new GaussI(x, y, precision);

                lastPolynomialCoeffs = gaussI.Polynomial;
                gaussI.DisplayResults(dataResultGaussI);

                lblResultGaussI.Text = Function.PolynomialToString(gaussI.Polynomial, "t");
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
            OpenExcelAndLoadData(dataXYGaussI);
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

                var gaussII = new GaussII(x, y, precision);

                lastPolynomialCoeffs = gaussII.Polynomial;
                gaussII.DisplayResults(dataResultGaussII);

                lblResultGaussII.Text = Function.PolynomialToString(gaussII.Polynomial, "t");
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
            OpenExcelAndLoadData(dataXYGaussII);
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

                IterationDirection direction = comboBoxIteration.SelectedIndex == 0 ? IterationDirection.Forward : IterationDirection.Backward;

                var iteration = new NewtonIteration(x, y, yTarget, direction, precision);
                iteration.DisplayResults(dataGridViewIteration, richTextBoxIterationResult);

                lblResultIteration.Text = $"x = {iteration.Result}";
                lblResultIteration.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }
        private void btnOpenExcelIteration_Click(object sender, EventArgs e)
        {
            OpenExcelAndLoadData(dataGridViewXYIteration);
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
                    var leastSquares = new LinearLeastSquares(x, y, phiExpressions);
                    leastSquares.Solve();
                    leastSquares.DisplayResults(rtbResultLeastSquares);
                    lblResultLeastSquares.Text = "Hàm tuyến tính";
                }
                else if (comboBoxLeastSquares.SelectedIndex == 1)
                {
                    var leastSquares = new PowerLawLeastSquares(x, y);
                    leastSquares.Solve();
                    leastSquares.DisplayResults(rtbResultLeastSquares);
                    lblResultLeastSquares.Text = $"Hàm dạng y = ax^b";
                }
                else if (comboBoxLeastSquares.SelectedIndex == 2)
                {
                    var leastSquares = new ExponentialLeastSquares(x, y, phiExpressions);
                    leastSquares.Solve();
                    leastSquares.DisplayResults(rtbResultLeastSquares);
                    lblResultLeastSquares.Text = $"Hàm dạng y = ae^(b1φ₁(x) + ...)";
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
            //ExcelPackage.License.SetNonCommercialPersonal("qanhta2710");
            //try
            //{
            //    using (OpenFileDialog openFileDialog = new OpenFileDialog())
            //    {
            //        openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
            //        openFileDialog.Title = "Chọn file Excel chứa dữ liệu (x, y)";

            //        if (openFileDialog.ShowDialog() == DialogResult.OK)
            //        {
            //            string filePath = openFileDialog.FileName;
            //            ReadExcelData(filePath, out double[] xValues, out double[] yValues);
            //            dataXYSpline.Rows.Clear();
            //            for (int i = 0; i < xValues.Length; i++)
            //            {
            //                dataXYSpline.Rows.Add(xValues[i], yValues[i]);
            //            }
            //            MessageBox.Show(
            //                $"Đã nhập thành công {xValues.Length} điểm dữ liệu từ Excel!",
            //                "Thành công",
            //                MessageBoxButtons.OK,
            //                MessageBoxIcon.Information);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}",
            //          "Lỗi",
            //          MessageBoxButtons.OK,
            //          MessageBoxIcon.Error);
            //}
        }
        // Tìm mốc nội suy cách đều
        private void btnFindPoints_Click(object sender, EventArgs e)
        {
            if (xValues == null || yValues == null || xValues.Length == 0)
            {
                MessageBox.Show("Vui lòng mở file Excel trước");
                return;
            }

            try
            {
                double xAvg = Convert.ToDouble(txtBoxX.Text);
                int k = Convert.ToInt32(textBoxK.Text);

                var finder = new FindInterpolationPoints(xValues, yValues);
                finder.FindEvenlySpacedPoints(xAvg, k);

                if (finder.CentralPointIndex == -1)
                {
                    MessageBox.Show($"Giá trị x = {xAvg} nằm ngoài phạm vi dữ liệu [{xValues.Min()}, {xValues.Max()}]",
                                  "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                finder.DisplayEvenlySpacedResult(richTextBoxFindPoints);

                var exportResult = MessageBox.Show(
                    $"Đã tìm thấy {finder.SelectedX.Length} mốc nội suy.\n\nBạn có muốn xuất dữ liệu ra file Excel không?",
                    "Xuất dữ liệu",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (exportResult == DialogResult.Yes)
                {
                    finder.ExportEvenlySpacedToExcel();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
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

                var finder = new FindInterpolationPoints(xValues, yValues);
                finder.FindReverseInterpolationPoints(yTarget);

                if (finder.IsolationIntervals.Count == 0)
                {
                    MessageBox.Show($"Không tìm thấy khoảng nào chứa y = {yTarget}\n" +
                                  $"Phạm vi dữ liệu: [{yValues.Min()}, {yValues.Max()}]",
                                  "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                finder.DisplayReverseInterpolationResult(richTextBoxFindReversePoints);

                var result = MessageBox.Show(
                    $"Đã tìm thấy {finder.MonotonicIntervals.Count} khoảng đơn điệu.\n\nBạn có muốn xuất dữ liệu ra file Excel không?",
                    "Xuất dữ liệu",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    finder.ExportReverseInterpolationToExcel();
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
        private void OpenExcelAndLoadData(DataGridView dgv)
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
                        dgv.Rows.Clear();
                        for (int i = 0; i < xValues.Length; i++)
                        {
                            dgv.Rows.Add(xValues[i], yValues[i]);
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
        #endregion
    }
}