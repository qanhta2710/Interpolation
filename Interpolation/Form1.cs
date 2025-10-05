using System;
using System.Linq;
using System.Text;
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

                // Sắp xếp dữ liệu theo thứ tự tăng dần
                dataXYLagrange.Sort(dataXYLagrange.Columns["colsXLagrange"], System.ComponentModel.ListSortDirection.Ascending);

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

                // Hệ số đa thức nội suy
                double[] dividedDiffDiagonal = new double[x.Length];
                for (int i = 0; i < x.Length; i++)
                {
                    dividedDiffDiagonal[i] = dividedDifferenceTable[i, i + 1] ?? 0.0;
                }

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
    }
}
