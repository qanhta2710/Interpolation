using Interpolation.Methods;
using Interpolation.Utilities;
using AngouriMath;
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
            comboBoxDerivative.SelectedIndex = 0;
            cmbMethod.SelectedIndex = 0;
        }
        #region Derivative
        private void btnOpenExcelDerivative_Click(object sender, EventArgs e)
        {
            InputHelper.OpenExcelAndLoadData(dataXYDerivative);
        }

        private void btnDerivative_Click(object sender, EventArgs e)
        {
            try
            {
                dataXYDerivative.Sort(dataXYDerivative.Columns[0], System.ComponentModel.ListSortDirection.Ascending);
                InputHelper.RemoveDuplicate(dataXYDerivative);

                double xTarget = Convert.ToDouble(txtBoxXDerivative.Text);
                int precision = Convert.ToInt32(txtBoxPrecisionDerivative.Text);
                double[] x = InputHelper.GetXValues(dataXYDerivative);
                double[] y = InputHelper.GetYValues(dataXYDerivative);
                
                DerivativeMethod method;
                switch (comboBoxDerivative.SelectedIndex)
                {
                    case 0:
                        method = DerivativeMethod.TwoPoints;
                        break;
                    case 1:
                        method = DerivativeMethod.ThreePoints;
                        break;
                    case 2:
                        method = DerivativeMethod.FourPoints;
                        break;
                    default:
                        MessageBox.Show("Vui lòng chọn công thức tính!");
                        return;
                }

                string functionStr = string.IsNullOrWhiteSpace(txtBoxFunction.Text) ? null : txtBoxFunction.Text.Trim();
                
                Derivative derivative = new Derivative(x, y, xTarget, method, precision, functionStr);
                derivative.DisplayResults(rtbDerivativeResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Integral
        private void btnOpenExcelIntegral_Click(object sender, EventArgs e)
        {
            InputHelper.OpenExcelAndLoadData(dgvIntegralData);
        }
        private void btnCalculateIntegral_Click(object sender, EventArgs e)
        {
            try
            {
                double a, b;

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

                if (rdoFunction.Checked)
                {
                    string func = txtFunction.Text.Trim();

                    if (string.IsNullOrEmpty(func))
                    {
                        MessageBox.Show("Vui lòng nhập hàm f(x)!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Kiểm tra phương pháp: 0 = Hình thang, 1 = Simpson, 2 = ...
                    bool isSimpson = cmbMethod.SelectedIndex == 1;

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

                        if (isSimpson && n % 2 != 0)
                        {
                            MessageBox.Show("Công thức Simpson yêu cầu n chẵn!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (isSimpson)
                        {
                            // Công thức hình thang
                            double m4 = CalculateM4(func, a, b);
                            var simpson = new SimpsonIntegration(func, a, b, n, m4);
                            simpson.DisplayResults(rtbIntegralResult);
                            lblIntegralResult.Text = $"Kết quả: ∫f(x)dx ≈ {simpson.Result}";
                        }
                        else
                        {
                            // Công thức Simpson
                            double m2 = CalculateM2(func, a, b);
                            var trapezoidal = new TrapezoidalIntegration(func, a, b, n, m2);
                            trapezoidal.DisplayResults(rtbIntegralResult);
                            lblIntegralResult.Text = $"Kết quả: ∫f(x)dx ≈ {trapezoidal.Result}";
                        }
                    }
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

                        if (isSimpson)
                        {
                            // Công thức Simpson với epsilon
                            double m4 = CalculateM4(func, a, b);
                            
                            if (m4 <= 0)
                            {
                                MessageBox.Show("Không thể tính M₄ tự động. Hàm có thể không có đạo hàm bậc 4 hoặc công thức không hợp lệ.", 
                                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            var simpson = new SimpsonIntegration(func, a, b, epsilon, m4);
                            simpson.DisplayResults(rtbIntegralResult);
                            lblIntegralResult.Text = $"Kết quả: ∫[{a}, {b}] f(x)dx ≈ {simpson.Result}";
                        }
                        else
                        {
                            // Công thức Hình thang với epsilon
                            double m2 = CalculateM2(func, a, b);
                            
                            if (m2 <= 0)
                            {
                                MessageBox.Show("Không thể tính M₂ tự động. Hàm có thể không có đạo hàm bậc 2 hoặc công thức không hợp lệ.", 
                                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            var trapezoidal = new TrapezoidalIntegration(func, a, b, epsilon, m2);
                            trapezoidal.DisplayResults(rtbIntegralResult);
                            lblIntegralResult.Text = $"Kết quả: ∫[{a}, {b}] f(x)dx ≈ {trapezoidal.Result}";
                        }
                    }

                    lblIntegralResult.Visible = true;
                }
                else
                {
                    // Tập dữ liệu rời rạc
                    InputHelper.RemoveDuplicate(dgvIntegralData);

                    if (dgvIntegralData.Rows.Count <= 1)
                    {
                        MessageBox.Show("Vui lòng nhập dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    double[] x = InputHelper.GetXValues(dgvIntegralData);
                    double[] y = InputHelper.GetYValues(dgvIntegralData);
                    double epsilon = Convert.ToDouble(txtDataEpsilon.Text);

                    bool isSimpson = cmbMethod.SelectedIndex == 1;

                    if (isSimpson)
                    {
                        var simpson = new SimpsonIntegration(x, y, a, b, epsilon);
                        simpson.DisplayResults(rtbIntegralResult);
                        lblIntegralResult.Text = $"Kết quả: ∫f(x)dx ≈ {simpson.Result}";
                    }
                    else
                    {
                        var trapezoidal = new TrapezoidalIntegration(x, y, a, b, epsilon);
                        trapezoidal.DisplayResults(rtbIntegralResult);
                        lblIntegralResult.Text = $"Kết quả: ∫f(x)dx ≈ {trapezoidal.Result}";
                    }

                    lblIntegralResult.Visible = true;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Lỗi định dạng dữ liệu. Vui lòng kiểm tra lại các giá trị nhập vào!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            catch (TypeInitializationException typeEx)
            {
                // Lấy lỗi gốc nằm bên trong
                Exception inner = typeEx.InnerException;
                while (inner.InnerException != null)
                {
                    inner = inner.InnerException;
                }

                MessageBox.Show($"Lỗi GỐC: {inner.Message}\n\nĐề nghị kiểm tra lại App.config hoặc thư mục bin.",
                                "Tìm ra nguyên nhân", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
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
            panelFunction.Visible = rdoFunction.Checked;
            panelData.Visible = !rdoFunction.Checked;
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