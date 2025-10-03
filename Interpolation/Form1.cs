using System;
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
                double[] lagrangePolynomial = Lagrange.FindPolynomial(coeffsD, divideTable, precision);
                object[] lagrangePolynomialRow = new object[cols + 1];
                for (int j = 0; j < lagrangePolynomial.Length; j++)
                {
                    lagrangePolynomialRow[j] = lagrangePolynomial[j];
                }
                lagrangePolynomialRow[cols - 1] = "";
                lagrangePolynomialRow[cols] = "Hệ số đa thức nội suy";
                dataGridViewLagrange.Rows.Add(lagrangePolynomialRow);

                // Chỉnh label để in ra màn hình đa thức hoàn chỉnh
                lblResult.Text = PolynomialToString(lagrangePolynomial);
                lblResult.Visible = true;


            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi định dạng");
                throw;
            }
        }
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
        private string PolynomialToString(double[] coeffs)
        {
            var sb = new StringBuilder();
            int n = coeffs.Length;

            for (int i = 0; i < n; i++)
            {
                double c = coeffs[i];
                int power = n - i - 1;

                if (Math.Abs(c) < 1e-9) continue;

                if (sb.Length > 0)
                {
                    sb.Append(c >= 0 ? " + " : " - ");
                }
                else if (c < 0)
                {
                    sb.Append("-");
                }

                double absC = Math.Abs(c);
                if (!(Math.Abs(absC - 1.0) < 1e-9 && power > 0))
                {
                    sb.Append(absC);
                }
                if (power > 0)
                {
                    sb.Append("x");
                    if (power > 1) sb.Append("^" + power);
                }
            }
            return sb.ToString();
        }
    }
}
