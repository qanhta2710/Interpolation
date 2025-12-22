using Interpolation.Utilities;
using System;
using System.Windows.Forms;

namespace Interpolation
{
    public class Lagrange
    {
        // Properties
        public double[] Polynomial { get; set; }
        public double[] CoeffsD { get; set; }
        public double[,] DivideTable { get; set; }
        public double[,] ProductTable { get; set; }
        public double[,] DifferenceMatrix { get; set; }
        public int NodeCount { get; set; }
        // Methods
        public Lagrange(double[] x, double[] y, int precision)
        {
            NodeCount = x.Length;
            Solve(x, y, precision);
        }
        private void Solve(double[] x, double[] y, int precision)
        {
            var result = FindCoeffsD(x, y, precision);
            CoeffsD = result.coeffsD;
            DifferenceMatrix = result.matrix;
            ProductTable = Horner.ProductTable(x, precision);
            double[] coeffsW = new double[x.Length + 1];
            for (int i = 0; i <= x.Length; i++)
            {
                coeffsW[i] = ProductTable[ProductTable.GetLength(0) - 1, i];
            }
            DivideTable = FindDivideTable(x, y, coeffsW, precision);
            Polynomial = Function.FindPolynomial(CoeffsD, DivideTable, precision);
        }
        private static (double[] coeffsD, double[,] matrix) FindCoeffsD(double[] x, double[] y, int precision)
        {
            int n = x.Length;
            double[,] arr = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        arr[i, j] = 1;
                    }
                    else
                    {
                        arr[i, j] = x[j] - x[i];
                    }
                }
            }
            double[] productNotation = new double[arr.GetLength(0)];
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                double prod = 1;
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    if (i != j)
                    {
                        prod *= arr[i, j];
                    }
                }
                productNotation[j] = prod;
            }
            double[] coeffsD = new double[productNotation.Length];
            for (int i = 0; i < n; i++)
            {
                coeffsD[i] = y[i] / productNotation[i];
                coeffsD[i] = Math.Round(coeffsD[i], precision);
            }
            return (coeffsD, arr);
        }
        private static double[,] FindDivideTable(double[] x, double[] y, double[] coeffsW, int precision)
        {
            int n = x.Length;
            double[,] divideTable = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (j == 0)
                    {
                        divideTable[i, j] = 1;
                    }
                    else
                    {
                        divideTable[i, j] = x[i] * divideTable[i, j - 1] + coeffsW[j];
                        divideTable[i, j] = Math.Round(divideTable[i, j], precision);
                    }
                }
            }
            return divideTable;
        }
        public void DisplayResults(DataGridView dgv)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            DataGridViewHelper.SetupColumns(dgv, NodeCount + 1, "Ghi chú");
            DataGridViewHelper.AddTable(dgv, DifferenceMatrix, "Ma trận {x_j - x_i}");
            DataGridViewHelper.AddSectionBreak(dgv);
            DataGridViewHelper.AddRow(dgv, CoeffsD, "Hệ số D");
            DataGridViewHelper.AddSectionBreak(dgv);
            DataGridViewHelper.AddTable(dgv, ProductTable, "Bảng tích", "w_{n+1}");
            DataGridViewHelper.AddSectionBreak(dgv);
            DataGridViewHelper.AddTable(dgv, DivideTable, "Bảng thương");
            DataGridViewHelper.AddSectionBreak(dgv);
            DataGridViewHelper.AddRow(dgv, Polynomial, "Hệ số đa thức nội suy");
        }
    }
}
