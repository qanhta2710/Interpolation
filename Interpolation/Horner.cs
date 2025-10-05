using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Text;

namespace Interpolation
{
    public class Horner
    {
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
    public class Function
    {
        public static double[] FindPolynomial(double[] basisCoeffs, double[,] basisTable, int precision)
        {
            double[] coeffsPolynomial = new double[basisCoeffs.Length];

            var basisCoeffsVector = DenseVector.OfArray(basisCoeffs);
            var basisMatrix = DenseMatrix.OfArray(basisTable);
            var basisCoeffsMatrix = basisCoeffsVector.ToRowMatrix();
            var result = basisCoeffsMatrix.Multiply(basisMatrix);

            coeffsPolynomial = result.Row(0).ToArray();
            for (int i = 0; i < coeffsPolynomial.Length; i++)
            {
                coeffsPolynomial[i] = Math.Round(coeffsPolynomial[i], precision);
            }
            return coeffsPolynomial;
        }
        public static string PolynomialToString(double[] coeffs)
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
