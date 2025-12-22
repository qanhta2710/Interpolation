using Interpolation.Utilities;
using System;
using System.Windows.Forms;

namespace Interpolation.Methods
{
    public class Bessel
    {
        public double[] Polynomial { get; set; }
        public double?[,] DiffTable { get; set; }
        public double[] ExtractedCoeffs { get; set; }
        public double[,] ProductTable { get; set; }
        public int NodeCount { get; set; }

        public Bessel(double[] x, double[] y, int precision)
        {
            NodeCount = x.Length;
            Solve(x, y, precision);
        }
        private void Solve(double[] x, double[] y, int precision)
        {
            DiffTable = DifferenceTable.BuildFiniteDifferenceTable(x, y, precision);
            ExtractedCoeffs = new double[x.Length];
            int idx = (x.Length / 2) - 1;
            for (int j = 1; j <= x.Length; j++)
            {
                if (j % 2 != 0)
                {
                    ExtractedCoeffs[j - 1] = (1.0 / 2.0) * ((DiffTable[idx, j] + DiffTable[idx + 1, j]) / Function.Factorial(j - 1)) ?? 0.0;
                    ExtractedCoeffs[j - 1] = Math.Round(ExtractedCoeffs[j - 1], precision);
                    idx++;
                }
                else
                {
                    ExtractedCoeffs[j - 1] = DiffTable[idx, j] / Function.Factorial(j - 1) ?? 0.0;
                    ExtractedCoeffs[j - 1] = Math.Round(ExtractedCoeffs[j - 1], precision);
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
                    vectorCoeffsEven[evenIdx++] = ExtractedCoeffs[i];
                }
                else
                {
                    vectorCoeffsOdd[oddIdx++] = ExtractedCoeffs[i];
                }
            }
            double[] x_new = new double[((x.Length / 2) - 1)];
            for (int i = 1; i < (x.Length / 2); i++)
            {
                x_new[i - 1] = Math.Pow(i - 0.5, 2);
            }
            ProductTable = Horner.ProductTable(x_new, precision);
            double[] finalCoeffsEven = Function.FindPolynomial(vectorCoeffsEven, ProductTable, precision);
            double[] finalCoeffsOdd = Function.FindPolynomial(vectorCoeffsOdd, ProductTable, precision);

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
            Polynomial = finalCoeffs;
        }
        public void DisplayResults(DataGridView dgv)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            DataGridViewHelper.SetupColumns(dgv, NodeCount + 1, "Ghi chú");
            DataGridViewHelper.AddNullableTable(dgv, DiffTable, "Bảng tỷ sai phân");
            DataGridViewHelper.AddSectionBreak(dgv);
            DataGridViewHelper.AddRow(dgv, ExtractedCoeffs, "Hệ số trích xuất từ bảng TSP");
            DataGridViewHelper.AddSectionBreak(dgv);
            DataGridViewHelper.AddTable(dgv, ProductTable, "Bảng tích", "t = 0.25, 2.25, 6.25, 12.25...");
            DataGridViewHelper.AddSectionBreak(dgv);
            DataGridViewHelper.AddRow(dgv, Polynomial, "Hệ số đa thức nội suy");
        }
    }
}
