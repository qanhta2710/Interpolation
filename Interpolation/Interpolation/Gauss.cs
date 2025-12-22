using Interpolation.Utilities;
using System;
using System.Windows.Forms;
namespace Interpolation
{
    public abstract class Gauss
    {
        public double[] Polynomial { get; protected set; }
        public double?[,] DiffTable { get; protected set; }
        public double[,] ProductTable { get; protected set; }
        public double[] ExtractedCoeffs { get; protected set; }
        public int NodeCount { get; protected set; }
        protected Gauss(double[] x, double[] y, int precision)
        {
            NodeCount = x.Length;
            Solve(x, y, precision);
        }
        private void Solve(double[] x, double[] y, int precision)
        {
            DiffTable = BuildGaussForwardTable(x, y, precision);

            ExtractedCoeffs = ExtractCoefficients(DiffTable, NodeCount, precision);

            double[] tPattern = GenerateTPattern(NodeCount);

            ProductTable = Horner.ProductTable(tPattern, precision);

            Polynomial = Function.FindPolynomial(ExtractedCoeffs, ProductTable, precision);
        }
        protected abstract double[] ExtractCoefficients(double?[,] diffTable, int n, int precision);
        protected abstract double[] GenerateTPattern(int n);
        protected abstract string GetDisplayName();
        protected abstract string GetPatternNote();
        private static double?[,] BuildGaussForwardTable(double[] x, double[] y, int precision)
        {
            int n = x.Length;
            double?[,] table = new double?[n, n + 1];

            // Cột đầu tiên: giá trị x
            for (int i = 0; i < n; i++)
                table[i, 0] = Math.Round(x[i], precision);

            // Cột thứ hai: giá trị y
            for (int i = 0; i < n; i++)
                table[i, 1] = Math.Round(y[i], precision);

            // Các cột sai phân Δy, Δ²y, Δ³y, ...
            for (int j = 2; j <= n; j++)
            {
                for (int i = 0; i <= n - j; i++)
                {
                    // Sai phân tiến: Δ^k y_i = Δ^(k-1) y_(i+1) - Δ^(k-1) y_i
                    if (table[i + 1, j - 1] != null && table[i, j - 1] != null)
                        table[i, j] = table[i + 1, j - 1] - table[i, j - 1];
                    else
                        table[i, j] = null;

                    if (table[i, j] != null)
                        table[i, j] = Math.Round(table[i, j].Value, precision);
                }
            }
            return table;
        }
        public void DisplayResults(DataGridView dgv)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            DataGridViewHelper.SetupColumns(dgv, NodeCount + 1, "Ghi chú");

            DataGridViewHelper.AddNullableTable(dgv, DiffTable, $"Bảng sai phân {GetDisplayName()}");
            DataGridViewHelper.AddSectionBreak(dgv);
            DataGridViewHelper.AddRow(dgv, ExtractedCoeffs, "Hệ số trích xuất (Δⁿf/n!)");
            DataGridViewHelper.AddSectionBreak(dgv);
            DataGridViewHelper.AddTable(dgv, ProductTable, "Bảng tích", GetPatternNote());
            DataGridViewHelper.AddSectionBreak(dgv);
            DataGridViewHelper.AddRow(dgv, Polynomial, $"Hệ số đa thức nội suy {GetDisplayName()}");
        }
    }

    public class GaussI : Gauss
    {
        public GaussI(double[] x, double[] y, int precision) : base(x, y, precision) { }
        protected override double[] ExtractCoefficients(double?[,] diffTable, int n, int precision)
        {
            double[] coeffs = new double[n];
            int center = (n - 1) / 2;

            coeffs[0] = diffTable[center, 1] ?? 0.0;
            int currentRow = center;

            for (int j = 2; j <= n; j++)
            {
                if (j % 2 != 0)
                    currentRow--;

                if (currentRow >= 0 && currentRow < n - j + 1)
                {
                    coeffs[j - 1] = (diffTable[currentRow, j] ?? 0.0) / Function.Factorial(j - 1);
                    coeffs[j - 1] = Math.Round(coeffs[j - 1], precision);
                }
            }

            return coeffs;
        }
        protected override double[] GenerateTPattern(int n)
        {
            double[] pattern = new double[n - 1];

            for (int i = 0; i < n - 1; i++)
            {
                if (i == 0)
                    pattern[i] = 0;
                else if (i % 2 == 1)
                    pattern[i] = (i + 1) / 2.0;
                else
                    pattern[i] = -(i / 2.0);
            }

            return pattern;
        }

        protected override string GetDisplayName() => "Gauss I";
        protected override string GetPatternNote() => "t = 0, 1, -1, 2, -2, 3, -3...";
    }
    public class GaussII : Gauss
    {
        public GaussII(double[] x, double[] y, int precision) : base(x, y, precision) { }
        protected override double[] ExtractCoefficients(double?[,] diffTable, int n, int precision)
        {
            double[] coeffs = new double[n];
            int center = (n - 1) / 2;

            coeffs[0] = diffTable[center, 1] ?? 0.0;
            int currentRow = center;

            for (int j = 2; j <= n; j++)
            {
                if (j % 2 == 0)
                    currentRow--;

                if (currentRow >= 0 && currentRow < n - j + 1)
                {
                    coeffs[j - 1] = (diffTable[currentRow, j] ?? 0.0) / Function.Factorial(j - 1);
                    coeffs[j - 1] = Math.Round(coeffs[j - 1], precision);
                }
            }

            return coeffs;
        }

        protected override double[] GenerateTPattern(int n)
        {
            double[] pattern = new double[n - 1];
            for (int i = 0; i < n - 1; i++)
            {
                if (i == 0)
                    pattern[i] = 0;
                else if (i % 2 == 1)
                    pattern[i] = -(i + 1) / 2.0;
                else
                    pattern[i] = i / 2.0;
            }
            return pattern;
        }
        protected override string GetDisplayName() => "Gauss II";
        protected override string GetPatternNote() => "t = 0, -1, 1, -2, 2, -3, 3...";
    }
}
