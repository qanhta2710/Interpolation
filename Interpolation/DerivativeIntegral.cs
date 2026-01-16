using AngouriMath;
using Interpolation.Methods;
using Interpolation.Utilities;
using System;
using System.Windows.Forms;

namespace Interpolation
{
    public partial class DerivativeIntegral : Form
    {
        public DerivativeIntegral()
        {
            InitializeComponent();
            SetupDataGridViewColumns();
        }
        private void DerivativeIntegral_Load(object sender, EventArgs e)
        {
            cmbMethod.SelectedIndex = 0;
        }

        #region Derivative
        private void btnOpenExcelDerivative_Click_1(object sender, EventArgs e)
        {
            InputHelper.OpenExcelAndLoadData(dataXYDerivative);
        }

        private void btnDerivative_Click_1(object sender, EventArgs e)
        {

            try
            {
                dataXYDerivative.Sort(dataXYDerivative.Columns[0], System.ComponentModel.ListSortDirection.Ascending);
                InputHelper.RemoveDuplicate(dataXYDerivative);

                double xTarget = Convert.ToDouble(txtBoxXDerivative.Text);
                int precision = Convert.ToInt32(txtBoxPrecisionDerivative.Text);
                double[] x = InputHelper.GetXValues(dataXYDerivative);
                double[] y = InputHelper.GetYValues(dataXYDerivative);
                int p = Convert.ToInt32(txtOrderP.Text);

                Derivative derivative = new Derivative(x, y, xTarget, p, precision);
                derivative.DisplayResults(rtbDerivativeResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Integral
        private void cmbMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Kiểm tra nếu mục được chọn là "Newton-Cotes"
            if (cmbMethod.SelectedItem != null && cmbMethod.SelectedItem.ToString() == "Newton-Cotes")
            {
                lblNewtonOrder.Visible = true;
                txtNewtonOrder.Visible = true;
            }
            else
            {
                lblNewtonOrder.Visible = false;
                txtNewtonOrder.Visible = false;
            }
        }
        private void btnOpenExcelIntegral_Click(object sender, EventArgs e)
        {
            InputHelper.OpenExcelAndLoadData(dgvIntegralData);
        }
        private void btnCalculateIntegral_Click(object sender, EventArgs e)
        {
            try
            {
                double a, b;

                // 1. Lấy và kiểm tra cận tích phân
                if (rdoFunction.Checked)
                {
                    if (string.IsNullOrWhiteSpace(txtLowerBound.Text) || string.IsNullOrWhiteSpace(txtUpperBound.Text))
                    {
                        MessageBox.Show("Vui lòng nhập cận tích phân!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    a = Convert.ToDouble(txtLowerBound.Text);
                    b = Convert.ToDouble(txtUpperBound.Text);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(txtDataLowerBound.Text) || string.IsNullOrWhiteSpace(txtDataUpperBound.Text))
                    {
                        MessageBox.Show("Vui lòng nhập cận tích phân!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    a = Convert.ToDouble(txtDataLowerBound.Text);
                    b = Convert.ToDouble(txtDataUpperBound.Text);
                }

                if (a >= b)
                {
                    MessageBox.Show("Cận dưới phải nhỏ hơn cận trên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Xử lý trường hợp nhập HÀM SỐ
                if (rdoFunction.Checked)
                {
                    string func = txtFunction.Text.Trim();
                    if (string.IsNullOrEmpty(func))
                    {
                        MessageBox.Show("Vui lòng nhập hàm f(x)!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    int methodIndex = cmbMethod.SelectedIndex; // 0: Hình thang, 1: Simpson, 2: Điểm giữa, 3: Newton-Cotes

                    // Xử lý Newton-Cotes
                    if (cmbMethod.SelectedItem != null && cmbMethod.SelectedItem.ToString() == "Newton-Cotes")
                    {
                        if (string.IsNullOrWhiteSpace(txtNewtonOrder.Text))
                        {
                            MessageBox.Show("Vui lòng nhập bậc Newton-Cotes!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        int order = Convert.ToInt32(txtNewtonOrder.Text);

                        if (order < 1 || order > 8)
                        {
                            MessageBox.Show("Bậc Newton-Cotes phải từ 1 đến 8!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // TRƯỜNG HỢP: TÍNH N TỪ EPSILON
                        if (rdoCalculateN.Checked)
                        {
                            if (string.IsNullOrWhiteSpace(txtEpsilon.Text))
                            {
                                MessageBox.Show("Vui lòng nhập epsilon!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            double epsilon = Convert.ToDouble(txtEpsilon.Text);
                            if (epsilon <= 0)
                            {
                                MessageBox.Show("Epsilon phải lớn hơn 0!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            var nc = new NewtonCotesIntegration(func, a, b, epsilon, order);
                            nc.DisplayResults(rtbIntegralResult);
                        }
                        // TRƯỜNG HỢP: N CHO TRƯỚC
                        else
                        {
                            if (string.IsNullOrWhiteSpace(txtN.Text))
                            {
                                MessageBox.Show("Vui lòng nhập số khoảng chia n!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            int n = Convert.ToInt32(txtN.Text);

                            if (n <= 0)
                            {
                                MessageBox.Show("Số khoảng chia phải lớn hơn 0!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // Kiểm tra n phải chia hết cho order
                            if (n % order != 0)
                            {
                                MessageBox.Show($"Số khoảng chia N phải chia hết cho bậc {order}!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            var nc = new NewtonCotesIntegration(func, a, b, n, order);
                            nc.DisplayResults(rtbIntegralResult);
                        }

                        lblIntegralResult.Visible = true;
                        return;
                    }

                    // TRƯỜNG HỢP: BIẾT TRƯỚC N
                    if (rdoFixedN.Checked)
                    {
                        if (string.IsNullOrWhiteSpace(txtN.Text))
                        {
                            MessageBox.Show("Vui lòng nhập số khoảng chia n!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        int n = Convert.ToInt32(txtN.Text);
                        if (n <= 0)
                        {
                            MessageBox.Show("Số khoảng chia phải lớn hơn 0!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        switch (methodIndex)
                        {
                            case 0: // Hình thang
                                double m2Trap = CalculateM2(func, a, b);
                                var trapezoidal = new TrapezoidalIntegration(func, a, b, n, m2Trap);
                                trapezoidal.DisplayResults(rtbIntegralResult);
                                break;

                            case 1: // Simpson
                                if (n % 2 != 0)
                                {
                                    MessageBox.Show("Công thức Simpson yêu cầu n chẵn!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                double m4Simp = CalculateM4(func, a, b);
                                var simpson = new SimpsonIntegration(func, a, b, n, m4Simp);
                                simpson.DisplayResults(rtbIntegralResult);
                                break;

                            case 2: // Điểm giữa
                                var midpoint = new MidpointIntegration(func, a, b, n);
                                midpoint.DisplayResults(rtbIntegralResult);
                                break;

                            default:
                                MessageBox.Show("Vui lòng chọn phương pháp tính!");
                                return;
                        }
                    }
                    // TRƯỜNG HỢP: TÍNH N TỪ EPSILON
                    else
                    {
                        if (string.IsNullOrWhiteSpace(txtEpsilon.Text))
                        {
                            MessageBox.Show("Vui lòng nhập epsilon!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        double epsilon = Convert.ToDouble(txtEpsilon.Text);
                        if (epsilon <= 0)
                        {
                            MessageBox.Show("Epsilon phải lớn hơn 0!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (methodIndex == 1) // Simpson dùng M4
                        {
                            double m4 = CalculateM4(func, a, b);
                            if (m4 <= 0) { MessageBox.Show("Không thể tính M4 tự động."); return; }
                            var simpson = new SimpsonIntegration(func, a, b, epsilon, m4);
                            simpson.DisplayResults(rtbIntegralResult);
                        }
                        else if (methodIndex == 0) // Hình thang dùng M2
                        {
                            double m2 = CalculateM2(func, a, b);
                            if (m2 <= 0) { MessageBox.Show("Không thể tính M2 tự động."); return; }
                            var trapezoidal = new TrapezoidalIntegration(func, a, b, epsilon, m2);
                            trapezoidal.DisplayResults(rtbIntegralResult);
                        }
                        else if (methodIndex == 2) // Điểm giữa dùng M2
                        {
                            var midpoint = new MidpointIntegration(func, a, b, epsilon);
                            midpoint.DisplayResults(rtbIntegralResult);
                        }
                    }
                    lblIntegralResult.Visible = true;
                }
                // 3. Xử lý trường hợp DỮ LIỆU RỜI RẠC
                else
                {
                    InputHelper.RemoveDuplicate(dgvIntegralData);
                    if (dgvIntegralData.Rows.Count <= 1)
                    {
                        MessageBox.Show("Vui lòng nhập dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    double[] x = InputHelper.GetXValues(dgvIntegralData);
                    double[] y = InputHelper.GetYValues(dgvIntegralData);

                    // Xử lý Newton-Cotes cho dữ liệu rời rạc
                    if (cmbMethod.SelectedItem != null && cmbMethod.SelectedItem.ToString() == "Newton-Cotes")
                    {
                        if (string.IsNullOrWhiteSpace(txtNewtonOrder.Text))
                        {
                            MessageBox.Show("Vui lòng nhập bậc Newton-Cotes!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        int order = Convert.ToInt32(txtNewtonOrder.Text);

                        if (order < 1 || order > 8)
                        {
                            MessageBox.Show("Bậc Newton-Cotes phải từ 1 đến 8!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        var nc = new NewtonCotesIntegration(x, y, a, b, order);
                        nc.DisplayResults(rtbIntegralResult);
                        return;
                    }

                    int methodIndex = cmbMethod.SelectedIndex;

                    if (methodIndex == 1) // Simpson
                    {
                        var simpson = new SimpsonIntegration(x, y, a, b);
                        simpson.DisplayResults(rtbIntegralResult);
                    }
                    else if (methodIndex == 2) // Điểm giữa
                    {
                        var midpoint = new MidpointIntegration(x, y, a, b);
                        midpoint.DisplayResults(rtbIntegralResult);
                    }
                    else // Hình thang (mặc định hoặc index 0)
                    {
                        var trapezoidal = new TrapezoidalIntegration(x, y, a, b);
                        trapezoidal.DisplayResults(rtbIntegralResult);
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Lỗi định dạng dữ liệu. Vui lòng kiểm tra lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnCalcDouble_Click(object sender, EventArgs e)
        {
            try
            {
                string func = txtFunction2D.Text;

                // Lấy cận
                double x0 = double.Parse(txtX0.Text);
                double xn = double.Parse(txtXn.Text);
                double y0 = double.Parse(txtY0.Text);
                double ym = double.Parse(txtYm.Text);

                double epsilon = Convert.ToDouble(txtBoxErrorGeneral.Text);

                // Khởi tạo class với Epsilon thay vì Nx, Ny
                var solver = new DoubleIntegration(func, x0, xn, y0, ym, epsilon);

                if (cmbMethod2D.SelectedIndex == 0) // Hình thang
                {
                    solver.CalculateTrapezoidalAuto();
                }
                else // Simpson
                {
                    solver.CalculateSimpsonAuto();
                }

                solver.DisplayResults(rtbResult2D);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        #endregion 
        private double CalculateM2(string functionStr, double a, double b)
        {
            try
            {
                // Parse hàm f(x)
                Entity function = functionStr;

                // Tính đạo hàm bậc 1: f'(x)
                var firstDerivative = function.Differentiate("x");

                // Tính đạo hàm bậc 2: f''(x)
                var secondDerivative = firstDerivative.Differentiate("x");

                // Compile f''(x) để tính giá trị
                var compiledSecondDerivative = secondDerivative.Compile("x");

                // Tìm max |f''(x)| trên [a, b] bằng cách lấy mẫu
                int samplePoints = 100; // Số điểm lấy mẫu
                double step = (b - a) / samplePoints;
                double maxValue = 0;

                for (int i = 0; i <= samplePoints; i++)
                {
                    double x = a + i * step;
                    try
                    {
                        double value = Math.Abs((double)compiledSecondDerivative.Call(x).Real);
                        if (!double.IsNaN(value) && !double.IsInfinity(value) && value > maxValue)
                        {
                            maxValue = value;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (maxValue < 1e-10)
                {
                    maxValue = 1e-6;
                }

                return maxValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tính M₂: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
        private double CalculateM4(string functionStr, double a, double b)
        {
            try
            {
                Entity function = functionStr;

                // Tính đạo hàm bậc 4
                var d1 = function.Differentiate("x");
                var d2 = d1.Differentiate("x");
                var d3 = d2.Differentiate("x");
                var d4 = d3.Differentiate("x");

                var compiledFourthDerivative = d4.Compile("x");

                int samplePoints = 100;
                double step = (b - a) / samplePoints;
                double maxValue = 0;

                for (int i = 0; i <= samplePoints; i++)
                {
                    double x = a + i * step;
                    try
                    {
                        double value = Math.Abs((double)compiledFourthDerivative.Call(x).Real);
                        if (!double.IsNaN(value) && !double.IsInfinity(value) && value > maxValue)
                        {
                            maxValue = value;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (maxValue < 1e-10)
                {
                    maxValue = 1e-6;
                }

                return maxValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tính M₄: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
        private void SetupDataGridViewColumns()
        {
            InputHelper.SetupDataGridViewColumnTypes(dataXYDerivative, "colsXDerivative", "colsYDerivative");
            InputHelper.SetupDataGridViewColumnTypes(dgvIntegralData, "colsXIntegral", "colsYIntegral");
        }
        private void rdoFunction_CheckedChanged(object sender, EventArgs e)
        {
            bool isFunctionMode = rdoFunction.Checked;

            panelFunction.Visible = rdoFunction.Checked;
            panelData.Visible = !rdoFunction.Checked;
            if (isFunctionMode)
            {
                rdoFixedN.Enabled = true;
                rdoCalculateN.Enabled = true;
                txtN.Enabled = rdoFixedN.Checked;
                txtEpsilon.Enabled = rdoCalculateN.Checked;
            }
            else
            {
                rdoFixedN.Enabled = false;
                rdoCalculateN.Enabled = false;
                txtN.Enabled = false;
                txtEpsilon.Enabled = false;

                txtN.BackColor = System.Drawing.SystemColors.Control;
                txtEpsilon.BackColor = System.Drawing.SystemColors.Control;
            }
        }

        private void rdoFixedN_CheckedChanged(object sender, EventArgs e)
        {
            txtN.Enabled = rdoFixedN.Checked;
            txtEpsilon.Enabled = !rdoFixedN.Checked;
        }
        private void btnClearDataIntegral_Click(object sender, EventArgs e)
        {
            dgvIntegralData.Rows.Clear();
        }
    }
}