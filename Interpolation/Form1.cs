using System;
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
            dataGridViewHornerEval.ClearSelection();
            dataGridViewHornerDerivative.ClearSelection();
            dataGridViewLagrange.ClearSelection();
            dataGridViewNewton.ClearSelection();
            dataGridViewNewtonFinite.ClearSelection();
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
                dataGridViewLagrange.Rows.Clear();
                dataGridViewLagrange.Columns.Clear();
                RemoveDuplicate(dataXYLagrange);

                double[] x = GetXValues(dataXYLagrange);
                double[] y = GetYValues(dataXYLagrange);
                int precision = Convert.ToInt32(txtBoxPrecisionLagrange.Text);

                // Tạo bảng
                int cols = x.Length + 1;
                for (int j = 0; j < cols; j++)
                {
                    dataGridViewLagrange.Columns.Add($"col{j}", $"Cột {j}");
                }
                dataGridViewLagrange.Columns.Add("note", "Ghi chú");
                dataGridViewLagrange.Columns["note"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                foreach (DataGridViewColumn col in dataGridViewLagrange.Columns)
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;

                // Tính hệ số D
                double[] coeffsD = Lagrange.CoeffsD(x, y, precision);
                object[] coeffsDRow = new object[cols + 1];
                for (int j = 0; j < coeffsD.Length; j++)
                {
                    coeffsDRow[j] = coeffsD[j];
                }
                coeffsDRow[cols - 1] = "";
                coeffsDRow[cols] = "Hệ số D";
                dataGridViewLagrange.Rows.Add(coeffsDRow);

                dataGridViewLagrange.Rows.Add();

                // Tính bảng tích
                double[,] productTable = Horner.ProductTable(x, precision);
                int tableProductRows = productTable.GetLength(0);
                int tableProductCols = productTable.GetLength(1);
                for (int i = 0; i < tableProductRows; i++)
                {
                    object[] row = new object[tableProductCols + 1];
                    for (int j = 0; j < tableProductCols; j++)
                    {
                        row[j] = productTable[i, j];
                    }
                    if (i == 0)
                    {
                        row[tableProductCols] = "Bảng tích";
                    }
                    if (i == tableProductRows - 1)
                    {
                        row[tableProductCols] = "w_{n+1}";
                    }
                    dataGridViewLagrange.Rows.Add(row);
                }

                dataGridViewLagrange.Rows.Add();

                double[] coeffsW = new double[cols];
                for (int i = 0; i < cols; i++)
                {
                    coeffsW[i] = productTable[productTable.GetLength(0) - 1, i];
                }

                // Tính bảng thương
                double[,] divideTable = Lagrange.DivideTable(x, y, coeffsW, precision);
                int tableDivideRows = divideTable.GetLength(0);
                int tableDivideCols = divideTable.GetLength(1);
                for (int i = 0; i < tableDivideRows; i++)
                {
                    object[] row = new object[tableDivideCols + 2];
                    for (int j = 0; j < tableDivideCols; j++)
                    {
                        row[j] = divideTable[i, j];
                    }
                    if (i == 0)
                    {
                        row[tableDivideCols] = "";
                        row[tableDivideCols + 1] = "Bảng thương";
                    }
                    dataGridViewLagrange.Rows.Add(row);
                }
                dataGridViewLagrange.Rows.Add();

                // Tính hệ số đa thức nội suy
                double[] lagrangePolynomial = Function.FindPolynomial(coeffsD, divideTable, precision);
                object[] lagrangePolynomialRow = new object[cols + 1];
                for (int j = 0; j < lagrangePolynomial.Length; j++)
                {
                    lagrangePolynomialRow[j] = lagrangePolynomial[j];
                }
                lagrangePolynomialRow[cols - 1] = "";
                lagrangePolynomialRow[cols] = "Hệ số đa thức nội suy";
                dataGridViewLagrange.Rows.Add(lagrangePolynomialRow);

                // Chỉnh label để in ra màn hình đa thức hoàn chỉnh
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

                // Tạo bảng
                dataGridViewNewton.Columns.Add("X", "X");
                dataGridViewNewton.Columns.Add("Y", "Y");
                for (int j = 1; j < x.Length; j++)
                {
                    dataGridViewNewton.Columns.Add($"TSP{j}", $"TSP {j}");
                }
                dataGridViewNewton.Columns.Add("note", "Ghi chú");
                dataGridViewNewton.Columns["note"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                foreach (DataGridViewColumn col in dataGridViewNewton.Columns)
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;

                double?[,] dividedDifferenceTable = Newton.BuildDividedDifferenceTable(x, y, precision);

                // Thêm bảng tỷ sai phân
                int rows = dividedDifferenceTable.GetLength(0);
                int cols = dividedDifferenceTable.GetLength(1);
                for (int i = 0; i < rows; i++)
                {
                    object[] row = new object[cols + 2];
                    for (int j = 0; j < cols; j++)
                    {
                        row[j] = dividedDifferenceTable[i, j]?.ToString() ?? "";
                    }
                    if (i == rows - 1)
                    {
                        row[cols] = "Bảng tỷ sai phân";
                    }
                    dataGridViewNewton.Rows.Add(row);
                }

                dataGridViewNewton.Rows.Add();

                // Bảng tích
                double[] x_newton = x.Take(x.Length - 1).ToArray();
                double[,] productTable = Horner.ProductTable(x_newton, precision);
                int tableProductRows = productTable.GetLength(0);
                int tableProductCols = productTable.GetLength(1);
                for (int i = 0; i < tableProductRows; i++)
                {
                    object[] row = new object[tableProductCols + 2];
                    for (int j = 0; j < tableProductCols; j++)
                    {
                        row[j] = productTable[i, j];
                    }
                    if (i == 0)
                    {
                        row[tableProductCols] = "";
                        row[tableProductCols + 1] = "Bảng tích";
                    }
                    dataGridViewNewton.Rows.Add(row);
                }

                dataGridViewNewton.Rows.Add();

                // Hệ số
                double[] dividedDiffDiagonal = new double[x.Length];
                for (int i = 0; i < x.Length; i++)
                {
                    dividedDiffDiagonal[i] = dividedDifferenceTable[i, i + 1] ?? 0.0;
                }
                object[] colsDividedDiffDiagonal = new object[x.Length + 2];
                for (int j = 0; j < x.Length; j++)
                {
                    colsDividedDiffDiagonal[j] = dividedDiffDiagonal[j];
                }
                colsDividedDiffDiagonal[x.Length] = "";
                colsDividedDiffDiagonal[x.Length + 1] = "Hệ số";
                dataGridViewNewton.Rows.Add(colsDividedDiffDiagonal);

                dataGridViewNewton.Rows.Add();

                // Hệ số đa thức nội suy
                double[] newtonPolynomial = Function.FindPolynomial(dividedDiffDiagonal, productTable, precision);
                object[] newtonPolynomialRow = new object[cols + 1];
                for (int j = 0; j < newtonPolynomial.Length; j++)
                {
                    newtonPolynomialRow[j] = newtonPolynomial[j];
                }
                newtonPolynomialRow[cols - 1] = "";
                newtonPolynomialRow[cols] = "Hệ số đa thức nội suy";
                dataGridViewNewton.Rows.Add(newtonPolynomialRow);

                // In đa thức nội suy
                lblResultNewton.Text = Function.PolynomialToString(newtonPolynomial);
                lblResultNewton.Visible = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi định dạng");
            }
        }

        // Tính giá trị đa thức và các đạo hàm tại điểm c bằng phương pháp Horner
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBoxResult.Clear();
                double[] coeffsP = GetCoeffsP(dataGridViewCoeffsP);
                int precision = Convert.ToInt32(txtBoxPrecisionEval.Text);
                double c = Convert.ToDouble(txtBoxC.Text);
                int k = Convert.ToInt32(txtBoxk.Text);
                int n = coeffsP.Length - 1;

                // Tính giá trị đa thức P(x) tại c
                richTextBoxResult.AppendText($"Giá trị đa thức P(x) tại c:\n");
                double hornerEval = Horner.HornerEvaluate(coeffsP, c, precision);
                richTextBoxResult.AppendText($"P({c}) = {hornerEval}\n");

                // Tính giá trị đạo hàm các cấp của đa thức P(x) tại c
                richTextBoxResult.AppendText($"Giá trị đạo hàm các cấp của đa thức P(x) tại c:\n");
                double[] hornerDerivatives = Horner.HornerDerivatives(coeffsP, c, n, precision);
                for (int m = 1; m <= k; m++)
                {
                    richTextBoxResult.AppendText($"P^({m})(x = {c}) = {hornerDerivatives[m]}\n");
                }

                // Tính thương và phần dư của phép chia P(x) với (x-c)
                richTextBoxResult.AppendText($"Thương và dư của phép chia P(x) với (x-c):\n");
                (double[] quotinent, double remainder) = Horner.HornerDivide(coeffsP, c, precision);
                richTextBoxResult.AppendText($"Q(x) = {Function.PolynomialToString(quotinent.Reverse().ToArray())}, R = {remainder}\n");

                // Tính tích đa thức P(x) với (x-c)
                richTextBoxResult.AppendText($"Tích đa thức P(x) với (x-c):\n");
                double[] multiplyPolynomial = Function.MultiplyPolynomial(coeffsP, c, precision);
                richTextBoxResult.AppendText($"({Function.PolynomialToString(coeffsP.Reverse().ToArray())})(x - {c}) = {Function.PolynomialToString(multiplyPolynomial)}\n");

                dataGridViewHornerEval.DataSource = null;
                DataTable evalTable = Horner.HornerEvaluationTable(coeffsP, c, precision);
                dataGridViewHornerEval.DataSource = evalTable;

                DataTable derivativeTable = Horner.GetHornerDerivativesTable(coeffsP, c, k, precision);
                dataGridViewHornerDerivative.DataSource = derivativeTable;

                if (dataGridViewHornerEval.Rows.Count > 1 && dataGridViewHornerEval.Columns.Count > 0)
                {
                    int lastColIndex = dataGridViewHornerEval.ColumnCount - 1;
                    dataGridViewHornerEval.Rows[1].Cells[lastColIndex].Style.BackColor = Color.LightGreen;
                    dataGridViewHornerEval.Rows[1].Cells[lastColIndex].Style.Font = new Font(dataGridViewHornerEval.Font, FontStyle.Bold);
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
            catch (Exception)
            {
                MessageBox.Show("Lỗi định dạng");
            }
        }
        private void btnSolveNewtonFinite_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridViewNewtonFinite.Rows.Clear();
                dataGridViewNewtonFinite.Columns.Clear();

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

                dataGridViewNewtonFinite.Columns.Add("X", "X");
                dataGridViewNewtonFinite.Columns.Add("Y", "Y");
                for (int j = 1; j < x.Length; j++)
                {
                    dataGridViewNewtonFinite.Columns.Add($"TSP{j}", $"TSP {j}");
                }
                dataGridViewNewtonFinite.Columns.Add("note", "Ghi chú");
                dataGridViewNewtonFinite.Columns["note"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                foreach (DataGridViewColumn col in dataGridViewNewtonFinite.Columns)
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;

                double?[,] finiteDifferenceTable = Newton.BuildFiniteDifferenceTable(x, y, precision);
                int rows = finiteDifferenceTable.GetLength(0);
                int cols = finiteDifferenceTable.GetLength(1);
                for (int i = 0; i < rows; i++)
                {
                    object[] row = new object[cols + 2];
                    for (int j = 0; j < cols; j++)
                    {
                        row[j] = finiteDifferenceTable[i, j]?.ToString() ?? "";
                    }
                    if (i == rows - 1)
                    {
                        row[cols] = "Bảng tỷ sai phân";
                    }
                    dataGridViewNewtonFinite.Rows.Add(row);
                }

                dataGridViewNewtonFinite.Rows.Add();

                double[] x_newton_finite = new double[x.Length - 1];
                if (comboBoxNewtonFinite.SelectedIndex == 0)
                {
                    for (int i = 0; i < x.Length - 1; i++)
                    {
                        x_newton_finite[i] = i;
                    }
                }
                else if (comboBoxNewtonFinite.SelectedIndex == 1)
                {
                    for (int i = 0; i < x.Length - 1; i++)
                    {
                        x_newton_finite[i] = -i;
                    }
                }
                double[,] productTable = Horner.ProductTable(x_newton_finite, precision);
                int tableProductRows = productTable.GetLength(0);
                int tableProductCols = productTable.GetLength(1);
                for (int i = 0; i < tableProductRows; i++)
                {
                    object[] row = new object[tableProductCols + 2];
                    for (int j = 0; j < tableProductCols; j++)
                    {
                        row[j] = productTable[i, j];
                    }
                    if (i == 0)
                    {
                        row[tableProductCols] = "";
                        row[tableProductCols + 1] = "Bảng tích";
                    }
                    dataGridViewNewtonFinite.Rows.Add(row);
                }

                dataGridViewNewtonFinite.Rows.Add();

                double[] finiteDifferenceDiagonal = new double[x.Length];
                for (int i = 0; i < x.Length; i++)
                {
                    finiteDifferenceDiagonal[i] = finiteDifferenceTable[i, i + 1] / Function.Factorial(i) ?? 0.0;
                }
                object[] colsFiniteDifferenceDiagonal = new object[x.Length + 2];
                for (int j = 0; j < x.Length; j++)
                {
                    colsFiniteDifferenceDiagonal[j] = finiteDifferenceDiagonal[j];
                }
                colsFiniteDifferenceDiagonal[x.Length] = "";
                colsFiniteDifferenceDiagonal[x.Length + 1] = "Hệ số";
                dataGridViewNewtonFinite.Rows.Add(colsFiniteDifferenceDiagonal);

                dataGridViewNewtonFinite.Rows.Add();

                double[] newtonPolynomial = Function.FindPolynomial(finiteDifferenceDiagonal, productTable, precision);
                object[] newtonPolynomialRow = new object[cols + 1];
                for (int j = 0; j < newtonPolynomial.Length; j++)
                {
                    newtonPolynomialRow[j] = newtonPolynomial[j];
                }
                newtonPolynomialRow[cols - 1] = "";
                newtonPolynomialRow[cols] = "Hệ số đa thức nội suy";
                dataGridViewNewtonFinite.Rows.Add(newtonPolynomialRow);

                lblNewtonFinite.Text = Function.PolynomialToString(newtonPolynomial);
                lblNewtonFinite.Visible = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi định dạng");
            }
        }

        // Các phương thức hỗ trợ 
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
            var seenXValues = new System.Collections.Generic.HashSet<double>();
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
    }
}
