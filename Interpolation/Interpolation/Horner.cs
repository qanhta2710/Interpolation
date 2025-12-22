using Interpolation.Utilities;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Interpolation
{
    public class Horner
    {
        // Properties
        public double[] Coeffs { get; set; }
        public double C { get; set; }
        public int Deg { get; set; }
        public double Evaluation { get; set; }
        public double[] Derivatives { get; set; }
        public double[] Quotient { get; set; }
        public double Remainder { get; set; }
        public double[] MultipliedPolynomial { get; set; }
        public DataTable EvaluationTable { get; set; }
        public DataTable DerivativeTable { get; set; }
        // Methods
        public Horner(double[] coeffsP, double c, int k, int precision)
        {
            Coeffs = coeffsP;
            C = c;
            Deg = coeffsP.Length - 1;
            Calculate(k, precision);
        }
        private void Calculate(int k, int precision)
        {
            Evaluation = HornerEvaluate(Coeffs, C, precision);
            Derivatives = HornerDerivatives(Coeffs, C, k, precision);
            (Quotient, Remainder) = HornerDivide(Coeffs, C, precision);
            MultipliedPolynomial = MultiplyPolynomial(Coeffs, C, precision);
            EvaluationTable = HornerEvaluationTable(Coeffs, C, precision);
            DerivativeTable = GetHornerDerivativesTable(Coeffs, C, k, precision);
        }
        public void DisplayResults(RichTextBox rtb, DataGridView dgvEval, DataGridView dgvDeriv)
        {
            rtb.Clear();
            var sb = new StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("KẾT QUẢ TÍNH TOÁN BẰNG SƠ ĐỒ HORNER");
            sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

            sb.AppendLine($"Giá trị đa thức P(x) tại c = {C}:");
            sb.AppendLine($"P({C}) = {Evaluation}\n");

            sb.AppendLine($"Giá trị đạo hàm các cấp của đa thức P(x) tại c = {C}:");
            for (int m = 1; m < Derivatives.Length; m++)
            {
                sb.AppendLine($"P^({m})({C}) = {Derivatives[m]}");
            }
            sb.AppendLine();

            sb.AppendLine($"Thương và dư của phép chia P(x) với (x - {C}):");
            sb.AppendLine($"Q(x) = {Function.PolynomialToString(Quotient.Reverse().ToArray())}");
            sb.AppendLine($"R = {Remainder}\n");

            sb.AppendLine($"Tích đa thức P(x) với (x - {C}):");
            sb.AppendLine($"{Function.PolynomialToString(MultipliedPolynomial)}\n");

            rtb.AppendText(sb.ToString());

            dgvEval.DataSource = EvaluationTable;
            dgvDeriv.DataSource = DerivativeTable;
            StyleGrids(dgvEval, dgvDeriv);
        }
        // Static Methods
        private static double[] MultiplyPolynomial(double[] P, double c, int precision)
        {
            int degP = P.Length - 1;
            double[] result = new double[degP + 2];
            result[degP + 1] = Math.Round(P[degP], precision);
            for (int i = degP; i > 0; i--)
            {
                result[i] = Math.Round(-c * P[i] + P[i - 1], precision);
            }
            result[0] = Math.Round(-c * P[0], precision);
            return result.Reverse().ToArray();
        }
        private static double HornerEvaluate(double[] coeffsP, double c, int precision)
        {
            double res = coeffsP[coeffsP.Length - 1];
            for (int i = coeffsP.Length - 2; i >= 0; i--)
            {
                res = Math.Round(coeffsP[i] + c * res, precision);
            }
            return res;
        }
        private static (double[] quotinent, double remainder) HornerDivide(double[] coeffsP, double c, int precision)
        {
            int n = coeffsP.Length - 1;
            double[] quotient = new double[n];
            quotient[n - 1] = Math.Round(coeffsP[n], precision);
            for (int i = n - 2; i >= 0; i--)
            {
                quotient[i] = Math.Round(coeffsP[i + 1] + c * quotient[i + 1], precision);
            }
            double remainder = Math.Round(coeffsP[0] + c * quotient[0], precision);
            return (quotient, remainder);
        }
        private static double[] HornerDerivatives(double[] coeffsP, double c, int m, int precision)
        {
            double[] R = coeffsP;
            double[] result = new double[m + 1];
            for (int k = 0; k <= m; k++)
            {
                double Rc = HornerEvaluate(R, c, precision);
                result[k] = Math.Round(Function.Factorial(k) * Rc, precision);

                if (R.Length <= 1)
                {
                    break;
                }

                (double[] quotient, _) = HornerDivide(R, c, precision);
                R = quotient;
            }
            return result;
        }
        private static DataTable HornerEvaluationTable(double[] coeffsP, double c, int precision)
        {
            var dt = new DataTable();
            var displayCoeffs = coeffsP.Reverse().ToArray();
            int n = displayCoeffs.Length;
            for (int i = 0; i < n; i++)
            {
                dt.Columns.Add($"Col{i}");
            }
            dt.Rows.Add(displayCoeffs.Cast<object>().ToArray());
            var steps = new double[n];
            steps[0] = displayCoeffs[0];
            for (int i = 1; i < n; i++)
            {
                steps[i] = Math.Round(displayCoeffs[i] + c * steps[i - 1], precision);
            }
            dt.Rows.Add(steps.Cast<object>().ToArray());

            return dt;
        }
        private static DataTable GetHornerDerivativesTable(double[] coeffsP, double c, int m, int precision)
        {
            var dt = new DataTable();
            var displayCoeffs = coeffsP.Reverse().ToArray();
            int n = displayCoeffs.Length;

            for (int i = 0; i < n; i++)
            {
                dt.Columns.Add($"Col{i}");
            }
            dt.Rows.Add(displayCoeffs.Cast<object>().ToArray());
            var currentCoeffs = displayCoeffs;
            for (int k = 0; k <= m; k++)
            {
                if (currentCoeffs.Length < 1) break;

                var stepsRowValues = new double[currentCoeffs.Length];
                var nextQuotient = new double[currentCoeffs.Length - 1];

                stepsRowValues[0] = currentCoeffs[0];
                if (nextQuotient.Length > 0)
                {
                    nextQuotient[0] = currentCoeffs[0];
                }

                for (int i = 1; i < currentCoeffs.Length; i++)
                {
                    stepsRowValues[i] = Math.Round(currentCoeffs[i] + c * stepsRowValues[i - 1], precision);
                    if (i < currentCoeffs.Length - 1)
                    {
                        nextQuotient[i] = stepsRowValues[i];
                    }
                }
                object[] fullRowData = new object[n];
                Array.Copy(stepsRowValues.Cast<object>().ToArray(), fullRowData, stepsRowValues.Length);
                dt.Rows.Add(fullRowData);
                currentCoeffs = nextQuotient;
            }
            return dt;
        }
        private void StyleGrids(DataGridView dgvEval, DataGridView dgvDeriv)
        {
            if (dgvEval.Rows.Count > 1 && dgvEval.Columns.Count > 0)
            {
                int lastRow = 1; // Hàng thứ 2 (index 1)
                int lastCol = dgvEval.Columns.Count - 1;

                var cell = dgvEval.Rows[lastRow].Cells[lastCol];
                cell.Style.BackColor = Color.LightGreen;
                cell.Style.Font = new Font(dgvEval.Font, FontStyle.Bold);
                cell.Style.ForeColor = Color.DarkGreen;
            }
            foreach (DataGridViewRow row in dgvDeriv.Rows)
            {
                if (row.Index == 0) continue;

                for (int col = dgvDeriv.Columns.Count - 1; col >= 0; col--)
                {
                    var cell = row.Cells[col];
                    if (cell.Value != null &&
                        cell.Value != DBNull.Value &&
                        !string.IsNullOrEmpty(cell.Value.ToString()))
                    {
                        cell.Style.BackColor = Color.LightSkyBlue;
                        cell.Style.Font = new Font(dgvDeriv.Font, FontStyle.Bold);
                        cell.Style.ForeColor = Color.DarkBlue;
                        break;
                    }
                }
            }
        }
        public static double[,] ProductTable(double[] x, int precision)
        {
            int cols = x.Length + 1;
            int rows = x.Length + 1;
            double[,] productTable = new double[rows, cols];
            productTable[0, cols - 1] = 1;
            for (int i = 1; i < rows; i++)
            {
                for (int j = cols - 1; j >= cols - i - 1; j--)
                {
                    if (j == cols - 1)
                    {
                        productTable[i, j] = -x[i - 1] * productTable[i - 1, j];
                        productTable[i, j] = Math.Round(productTable[i, j], precision);
                    }
                    else if (j == cols - i - 1)
                    {
                        productTable[i, j] = 1;
                    }
                    else
                    {
                        productTable[i, j] = -x[i - 1] * productTable[i - 1, j] + productTable[i - 1, j + 1];
                        productTable[i, j] = Math.Round(productTable[i, j], precision);
                    }
                }
            }
            return productTable;
        }
    }
}
