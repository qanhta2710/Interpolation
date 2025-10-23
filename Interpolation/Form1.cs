using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Interpolation
{
    public partial class Form1 : Form
    {
        private double[] lastPolynomialCoeffs; 
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxNewton.SelectedIndex = 0; // Lựa chọn mặc định mốc nội suy bất kì
            comboBoxNewtonFinite.SelectedIndex = 0; // Lựa chọn mặc định mốc nội suy cách đều tăng dần
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
                var lagrangePolynomial = SolveLagrange(x, y, precision, out double[] coeffsD, out double[,] productTable, out double[,] divideTable);
                lastPolynomialCoeffs = lagrangePolynomial;
                DisplayLagrangeResults(dataGridViewLagrange, x.Length, coeffsD, productTable, divideTable, lagrangePolynomial);

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
        // Tính giá trị đa thức và các đạo hàm tại điểm c bằng phương pháp Horner
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
                DisplayCentralResults(dataResultStirling, x.Length, diffTable, vectorCoeffs, prodTable, StirlingPolynomial);

                lblStirling.Text = Function.PolynomialToString(StirlingPolynomial, "t");
                lblStirling.Visible = true;
                btnStirlingToEval.Visible = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi định dạng");
            }
        }
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
                DisplayCentralResults(dataResultBessel, x.Length, diffTable, vectorCoeffs, prodTable, BesselPolynomial);

                lblResultBessel.Text = Function.PolynomialToString(BesselPolynomial, "t");
                lblResultBessel.Visible = true;
                btnBesselToEval.Visible = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi định dạng");
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
        #endregion
        #region Core
        private double[] SolveLagrange(double[] x, double[] y, int precision, out double[] coeffsD, out double[,] productTable, out double[,] divideTable)
        {
            coeffsD = Lagrange.CoeffsD(x, y, precision);
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
        #endregion

        #region UI
        private void DisplayLagrangeResults(DataGridView dgv, int n, double[] coeffsD, double[,] productTable, double[,] divideTable, double[] lagrangePolynomial) // n là số nút nội suy
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            SetupColumns(dgv, n + 1, "Ghi chú"); // n + 1 cột do bảng nhân và 1 cột ghi chú
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
        private void DisplayCentralResults(DataGridView dgv, int n, double?[,] diffTable, double[] coeffsVector, double[,] productTable, double[] coeffsPolynomial)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            SetupColumns(dgv, n + 1, "Ghi chú");
            AddNullableTable(dgv, diffTable, "Bảng tỷ sai phân");
            AddSectionBreak(dgv);
            AddRow(dgv, coeffsVector, "Hệ số trích xuất từ bảng TSP");
            AddSectionBreak(dgv);
            AddTable(dgv, productTable, "Bảng tích", "");
            AddSectionBreak(dgv);
            AddRow(dgv, coeffsPolynomial, "Hệ số đa thức nội suy");
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
        #endregion
    }
}
