using Interpolation.Methods;
using Interpolation.Utilities;
using OfficeOpenXml;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
namespace Interpolation
{
    public partial class Interpolation : Form
    {
        private double[] lastPolynomialCoeffs;
        private double[] xValues;
        private double[] yValues;

        private double lastX0; // Mốc trung tâm
        private double lastH; // Khoảng cách
        private bool isLastMethodTransformed; // Đổi biến không?
        private string lastTransformationType; // Loại nội suy đổi biến

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
        private void dataGridViewCoeffsP_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridViewCoeffsP_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {

        }

        private void dataGridViewCoeffsP_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {

        }
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
                InputHelper.RemoveDuplicate(dataXYLagrange);

                double[] x = InputHelper.GetXValues(dataXYLagrange);
                double[] y = InputHelper.GetYValues(dataXYLagrange);
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
            InputHelper.OpenExcelAndLoadData(dataXYLagrange);
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

                InputHelper.RemoveDuplicate(dataXYNewton);

                double[] x = InputHelper.GetXValues(dataXYNewton);
                double[] y = InputHelper.GetYValues(dataXYNewton);
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
            InputHelper.OpenExcelAndLoadData(dataXYNewton);
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
                InputHelper.RemoveDuplicate(dataGridViewXYNewtonFinite);

                double[] x = InputHelper.GetXValues(dataGridViewXYNewtonFinite);
                double[] y = InputHelper.GetYValues(dataGridViewXYNewtonFinite);
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
            InputHelper.OpenExcelAndLoadData(dataGridViewXYNewtonFinite);
        }
        // Tính giá trị đa thức và các đạo hàm tại 1 điểm bằng phương pháp Horner
        private void btnEval_Click(object sender, EventArgs e)
        {
            try
            {
                // Xử lý Input
                double[] coeffsP = InputHelper.GetCoeffsP(dataGridViewCoeffsP);
                int precision = Convert.ToInt32(txtBoxPrecisionEval.Text);
                double inputValue = Convert.ToDouble(txtBoxC.Text);
                int k = Convert.ToInt32(txtBoxk.Text);

                double evaluationPoint;
                string explanation;

                if (isLastMethodTransformed)
                {
                    if (lastTransformationType == "Bessel")
                    {
                        evaluationPoint = ((inputValue - lastX0) / lastH) - 0.5;
                        explanation = $"Đổi biến Bessel:\n" +
                                    $"x = {inputValue}\n" +
                                    $"t = ((x - x₀) / h) - 0.5\n" +
                                    $"t = (({inputValue} - {lastX0}) / {lastH}) - 0.5\n" +
                                    $"t = {evaluationPoint}";
                    }
                    else // Newton, Stirling, Gauss I, Gauss II
                    {
                        evaluationPoint = (inputValue - lastX0) / lastH;
                        explanation = $"Đổi biến {lastTransformationType}:\n" +
                                    $"x = {inputValue}\n" +
                                    $"t = (x - x₀) / h\n" +
                                    $"t = ({inputValue} - {lastX0}) / {lastH}\n" +
                                    $"t = {evaluationPoint}";
                    }

                    MessageBox.Show(explanation, "Thông tin đổi biến", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    evaluationPoint = inputValue;
                    MessageBox.Show($"Không thực hiện đổi biến\nTính tại x = {inputValue}", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                var result = new Horner(coeffsP, evaluationPoint, k, precision);
                result.DisplayResults(richTextBoxResult, dataGridViewHornerEval, dataGridViewHornerDerivative);}
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
                InputHelper.RemoveDuplicate(dataXYStirling);

                double[] x = InputHelper.GetXValues(dataXYStirling);
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
                double[] y = InputHelper.GetYValues(dataXYStirling);
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
            InputHelper.OpenExcelAndLoadData(dataXYStirling);
        }
        // Tìm đa thức nội suy trung tâm bằng Bessel
        private void btnSolveBessel_Click(object sender, EventArgs e)
        {
            try
            {
                dataXYBessel.Sort(dataXYBessel.Columns["colsXBessel"], System.ComponentModel.ListSortDirection.Ascending);
                InputHelper.RemoveDuplicate(dataXYBessel);

                double[] x = InputHelper.GetXValues(dataXYBessel);
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
                double[] y = InputHelper.GetYValues(dataXYBessel);
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
            InputHelper.OpenExcelAndLoadData(dataXYBessel);
        }
        // Tìm đa thức nội suy trung tâm bằng Gauss I
        private void btnSolveGaussI_Click(object sender, EventArgs e)
        {
            try
            {
                // Xử lý Input
                dataXYGaussI.Sort(dataXYGaussI.Columns["colsXGaussI"], System.ComponentModel.ListSortDirection.Ascending);
                InputHelper.RemoveDuplicate(dataXYGaussI);

                double[] x = InputHelper.GetXValues(dataXYGaussI);
                double[] y = InputHelper.GetYValues(dataXYGaussI);
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
            InputHelper.OpenExcelAndLoadData(dataXYGaussI);
        }
        // Tìm đa thức nội suy trung tâm bằng Gauss II
        private void btnSolveGaussII_Click(object sender, EventArgs e)
        {
            try
            {
                // Xử lý Input
                dataXYGaussII.Sort(dataXYGaussII.Columns["colsXGaussII"], System.ComponentModel.ListSortDirection.Ascending);
                InputHelper.RemoveDuplicate(dataXYGaussII);

                double[] x = InputHelper.GetXValues(dataXYGaussII);
                double[] y = InputHelper.GetYValues(dataXYGaussII);
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
            InputHelper.OpenExcelAndLoadData(dataXYGaussII);
        }
        // Phương pháp lặp
        private void btnSolveIteration_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridViewXYIteration.Sort(dataGridViewXYIteration.Columns["colsXIteration"], System.ComponentModel.ListSortDirection.Ascending);
                InputHelper.RemoveDuplicate(dataGridViewXYIteration);

                double[] x = InputHelper.GetXValues(dataGridViewXYIteration);
                double[] y = InputHelper.GetYValues(dataGridViewXYIteration);
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
            InputHelper.OpenExcelAndLoadData(dataGridViewXYIteration);
        }
        // Phương pháp bình phương tối thiểu
        private void btnSolveLeastSquares_Click(object sender, EventArgs e)
        {
            try
            {
                double[] x = InputHelper.GetXValues(dataXYLeastSquares);
                double[] y = InputHelper.GetYValues(dataXYLeastSquares);
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
                        InputHelper.ReadExcelDataWithoutDuplicateRemoval(filePath, out double[] xValues, out double[] yValues);
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
        private void btnSolveSpline_Click(object sender, EventArgs e)
        {
            try
            {
                dataXYSpline.Sort(dataXYSpline.Columns[0], System.ComponentModel.ListSortDirection.Ascending);

                double[] x = InputHelper.GetXValues(dataXYSpline);
                double[] y = InputHelper.GetYValues(dataXYSpline);

                // Kiểm tra dữ liệu
                if (x.Length < 2) { MessageBox.Show("Cần ít nhất 2 điểm dữ liệu."); return; }

                int precision = Convert.ToInt32(txtBoxPrecisionSpline.Text);

                // 2. Khởi tạo đối tượng Spline
                Spline splineSolver = new Spline(x, y, precision);

                // 3. Gọi phương thức giải tùy theo ComboBox
                int type = comboBoxSpline.SelectedIndex;
                double valStart = string.IsNullOrWhiteSpace(txtSplineStart.Text) ? 0 : Convert.ToDouble(txtSplineStart.Text);
                double valEnd = string.IsNullOrWhiteSpace(txtSplineEnd.Text) ? 0 : Convert.ToDouble(txtSplineEnd.Text);

                if (type == 0) // Linear
                {
                    splineSolver.SolveLinear();
                }
                else if (type == 1) // Quadratic
                {
                    double? s0 = string.IsNullOrWhiteSpace(txtSplineStart.Text) ? (double?)null : valStart;
                    double? sn = string.IsNullOrWhiteSpace(txtSplineEnd.Text) ? (double?)null : valEnd;

                    if (s0 == null && sn == null) s0 = 0;

                    splineSolver.SolveQuadratic(s0, sn);
                }
                else if (type == 2) // Cubic General
                {
                    splineSolver.SolveCubic(false, valStart, valEnd);
                }
                else if (type == 3) // Clamped Cubic (S' biên)
                {
                    splineSolver.SolveCubic(true, valStart, valEnd);
                }

                // 4. Hiển thị kết quả
                splineSolver.DisplayResults(rtbResultSpline);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }
        private void btnOpenExcelSpline_Click(object sender, EventArgs e)
        {
            try
            {
                InputHelper.OpenExcelAndLoadData(dataXYSpline);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi mở file: " + ex.Message);
            }
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
                        InputHelper.ReadExcelData(filePath, out xValues, out yValues);

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
                        InputHelper.ReadExcelData(filePath, out xValues, out yValues);

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
            isLastMethodTransformed = false;
            lblMethod.Visible = true;
            lblMethod.Text = "Phương pháp đổi biến: Mặc định (Không đổi biến)";
            MessageBox.Show("Đã chuyển hệ số qua tính giá trị");
        }
        private void btnNewtonToEval_Click(object sender, EventArgs e)
        {
            TransferCoeffsToEval(dataGridViewCoeffsP, lastPolynomialCoeffs);
            isLastMethodTransformed = false;
            lblMethod.Visible = true;
            lblMethod.Text = "Phương pháp đổi biến: Mặc định (Không đổi biến)";
            MessageBox.Show("Đã chuyển hệ số qua tính giá trị");
        }
        private void btnNewtonFiniteToEval_Click(object sender, EventArgs e)
        {
            TransferCoeffsToEval(dataGridViewCoeffsP, lastPolynomialCoeffs);
            double[] x = InputHelper.GetXValues(dataGridViewXYNewtonFinite);
            if (comboBoxNewtonFinite.SelectedItem.ToString().Contains("tiến"))
            {
                lastX0 = x[0];
            }
            else if (comboBoxNewtonFinite.SelectedItem.ToString().Contains("lùi"))
            {
                lastX0 = x[x.Length - 1];
            }
            lastH = Math.Round(x[1] - x[0], 6);
            isLastMethodTransformed = true;
            lastTransformationType = "Newton Mốc cách đều";
            lblMethod.Visible = true;
            lblMethod.Text = $"Phương pháp đổi biến: {lastTransformationType}\n" +
                             $"x₀ = {lastX0}, h = {lastH}";
            MessageBox.Show("Đã chuyển hệ số qua tính giá trị");
        }
        private void btnStirlingToEval_Click(object sender, EventArgs e)
        {
            TransferCoeffsToEval(dataGridViewCoeffsP, lastPolynomialCoeffs);
            double[] x = InputHelper.GetXValues(dataXYStirling);
            int centerIndex = (x.Length - 1) / 2;
            lastX0 = x[centerIndex];
            lastH = Math.Round(x[1] - x[0], 6);
            isLastMethodTransformed = true;
            lastTransformationType = "Stirling";
            lblMethod.Visible = true;
            lblMethod.Text = $"Phương pháp đổi biến: {lastTransformationType}\n" +
                             $"x₀ = {lastX0}, h = {lastH}";
            MessageBox.Show("Đã chuyển hệ số qua tính giá trị");
        }
        private void btnBesselToEval_Click(object sender, EventArgs e)
        {
            TransferCoeffsToEval(dataGridViewCoeffsP, lastPolynomialCoeffs);
            double[] x = InputHelper.GetXValues(dataXYBessel);
            int centerIndex = (x.Length / 2) - 1; 
            lastX0 = x[centerIndex];
            lastH = Math.Round(x[1] - x[0], 6);
            isLastMethodTransformed = true;
            lastTransformationType = "Bessel";
            lblMethod.Visible = true;
            lblMethod.Text = $"Phương pháp đổi biến: {lastTransformationType}\n" +
                             $"x₀ = {lastX0}, h = {lastH}";
            MessageBox.Show("Đã chuyển hệ số qua tính giá trị");
        }
        private void btnGaussIToEval_Click(object sender, EventArgs e)
        {
            TransferCoeffsToEval(dataGridViewCoeffsP, lastPolynomialCoeffs);
            double[] x = InputHelper.GetXValues(dataXYGaussI);
            int centerIndex = (x.Length - 1) / 2;
            lastX0 = x[centerIndex];
            lastH = Math.Round(x[1] - x[0], 6);
            isLastMethodTransformed = true;
            lastTransformationType = "GaussI";
            lblMethod.Visible = true;
            lblMethod.Text = $"Phương pháp đổi biến: {lastTransformationType}\n" +
                             $"x₀ = {lastX0}, h = {lastH}";
            MessageBox.Show("Đã chuyển hệ số qua tính giá trị");
        }
        private void btnGaussIIToEval_Click(object sender, EventArgs e)
        {
            TransferCoeffsToEval(dataGridViewCoeffsP, lastPolynomialCoeffs);
            double[] x = InputHelper.GetXValues(dataXYGaussII);
            int centerIndex = (x.Length - 1) / 2;
            lastX0 = x[centerIndex];
            lastH = Math.Round(x[1] - x[0], 6);
            isLastMethodTransformed = true;
            lastTransformationType = "GaussII";
            lblMethod.Visible = true;
            lblMethod.Text = $"Phương pháp đổi biến: {lastTransformationType}\n" +
                             $"x₀ = {lastX0}, h = {lastH}";
            MessageBox.Show("Đã chuyển hệ số qua tính giá trị");
        }
        private void btnResetDefault_Click(object sender, EventArgs e)
        {
            if (!isLastMethodTransformed)
            {
                MessageBox.Show("Hiện tại không có thông tin đổi biến để reset.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var result = MessageBox.Show(
                $"Bạn có chắc muốn reset thông tin đổi biến?\n\n" +
                $"Thông tin hiện tại:\n" +
                $"- Phương pháp: {lastTransformationType}\n" +
                $"- x₀ = {lastX0}\n" +
                $"- h = {lastH}",
                "Xác nhận reset",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                isLastMethodTransformed = false;
                lastTransformationType = null;
                lastX0 = 0;
                lastH = 0;
                lblMethod.Text = "Phương pháp đổi biến: Mặc định (Không đổi biến)";
                MessageBox.Show("Reset về mặc định thành công");
            }
        }
        #endregion

        #region Utilities
        private void TransferCoeffsToEval(DataGridView dgv, double[] polynomialCoeffs)
        {
            dgv.Rows.Clear();
            for (int i = 0; i < polynomialCoeffs.Length; i++)
            {
                dgv.Rows.Add(polynomialCoeffs[i]);
            }
        }
        private void SetupDataGridViewColumnTypes()
        {
            InputHelper.SetupDataGridViewColumnTypes(dataXYGaussI, "colsXGaussI", "colsYGaussI");
            InputHelper.SetupDataGridViewColumnTypes(dataXYNewton, "colsXNewton", "colsYNewton");
            InputHelper.SetupDataGridViewColumnTypes(dataGridViewXYNewtonFinite, "colsXNewtonFinite", "colsYNewtonFinite");
            InputHelper.SetupDataGridViewColumnTypes(dataXYStirling, "colsXStirling", "colsYStirling");
            InputHelper.SetupDataGridViewColumnTypes(dataXYBessel, "colsXBessel", "colsYBessel");
            InputHelper.SetupDataGridViewColumnTypes(dataXYGaussII, "colsXGaussII", "colsYGaussII");
            InputHelper.SetupDataGridViewColumnTypes(dataGridViewXYIteration, "colsXIteration", "colsYIteration");
            InputHelper.SetupDataGridViewColumnTypes(dataXYLeastSquares, "colsXLeastSquares", "colsYLeastSquares");
            InputHelper.SetupDataGridViewColumnTypes(dataXYSpline, "colsXSpline", "colsYSpline");
        }
        private void comboBoxSpline_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = comboBoxSpline.SelectedIndex;

            // Mặc định hiện panel nhập liệu
            panelSplineConditions.Visible = true;

            switch (idx)
            {
                case 0: // Tuyến tính
                    panelSplineConditions.Visible = false; // Không cần biên
                    break;

                case 1: // Bậc 2 (Cần S')
                    labelSplineStart.Text = "S'(x0) =";
                    labelSplineEnd.Text = "S'(xn) =";
                    labelSplineNote.Text = "(Nhập 1 trong 2 giá trị đạo hàm cấp 1)";
                    labelSplineNote.Visible = true;
                    break;

                case 2: // Bậc 3 Tổng quát (Cần S'') - CÂU B
                    labelSplineStart.Text = "S''(x0) =";
                    labelSplineEnd.Text = "S''(xn) =";
                    labelSplineNote.Text = "(Nhập đạo hàm cấp 2. Để trống = 0 là Spline Tự nhiên)";
                    labelSplineNote.Visible = true;
                    break;

                case 3: // Bậc 3 Clamped (Cần S') - CÂU C
                    labelSplineStart.Text = "S'(x0) =";
                    labelSplineEnd.Text = "S'(xn) =";
                    labelSplineNote.Text = "(Nhập đạo hàm cấp 1 tại 2 đầu mút)";
                    labelSplineNote.Visible = true;
                    break;
            }
        }
        #endregion

        private void comboBoxLeastSquares_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLeastSquares.Text.Contains("ax^b"))
            {
                txtBoxLeastSquares.Enabled = false;
                txtBoxLeastSquares.Clear();
            }
            else
            {
                txtBoxLeastSquares.Enabled = true;
            }
        }
    }
}