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
        private void btnSolveLagrange_Click(object sender, EventArgs e)
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
                DisplayLagrangeResults(dataGridViewLagrange, x.Length, coeffsD, productTable, divideTable, lagrangePolynomial);

                // In ra đa thức nội suy dạng chính tắc
                lblResult.Text = Function.PolynomialToString(lagrangePolynomial);
                lblResult.Visible = true;
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
                DisplayNewtonResults(dataGridViewNewton, x.Length, diffTable, dividedDiagonalDiff, productTable, newtonPolynomial);

                // In đa thức nội suy
                lblResultNewton.Text = Function.PolynomialToString(newtonPolynomial);
                lblResultNewton.Visible = true;
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
                    MessageBox.Show("Sắp xếp mốc nội suy tăng dần");
                }
                else if (comboBoxNewtonFinite.SelectedIndex == 1)
                {
                    dataGridViewXYNewtonFinite.Sort(dataGridViewXYNewtonFinite.Columns["colsXNewtonFinite"], System.ComponentModel.ListSortDirection.Descending);
                    MessageBox.Show("Sắp xếp mốc nội suy giảm dần");
                }

                RemoveDuplicate(dataGridViewXYNewtonFinite);

                double[] x = GetXValues(dataGridViewXYNewtonFinite);
                double[] y = GetYValues(dataGridViewXYNewtonFinite);
                int precision = Convert.ToInt32(txtBoxPrecisionNewtonFinite.Text);
                bool isAscending = comboBoxNewtonFinite.SelectedIndex == 0;

                // Tìm và in đa thức nội suy
                var newtonPolynomial = SolveNewtonFinite(x, y, precision, isAscending, out double?[,] finiteDiffTable, out double[,] productTable, out double[] dividedDiagonalDiff);
                DisplayNewtonResults(dataGridViewNewtonFinite, x.Length, finiteDiffTable, dividedDiagonalDiff, productTable, newtonPolynomial);

                // Viết đa thức nội suy dạng chính tắc
                lblNewtonFinite.Text = Function.PolynomialToString(newtonPolynomial);
                lblNewtonFinite.Visible = true;
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
        private double[] SolveNewtonFinite(double[] x, double[] y, int precision, bool isAscending, out double?[,] finiteDiffTable, out double[,] productTable, out double[] dividedDiagonalDiff)
        {
            finiteDiffTable = Newton.BuildFiniteDifferenceTable(x, y, precision);
            double[] x_newton = new double[x.Length - 1];
            if (isAscending)
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
            for (int i = 0; i < x.Length; i++)
            {
                dividedDiagonalDiff[i] = (finiteDiffTable[i, i + 1] / Function.Factorial(i)) ?? 0.0;
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
        #endregion
    }
}
