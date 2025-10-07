using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Linq;
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
        public static double HornerEvaluate(double[] coeffsP, double c, int precision)
        {
            double res = coeffsP[coeffsP.Length - 1];
            for (int i = coeffsP.Length - 2; i >= 0; i--)
            {
                res = Math.Round(coeffsP[i] + c * res, precision);
            }
            return res;
        }
        public static (double[] quotinent, double remainder) HornerDivide(double[] coeffsP, double c, int precision)
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
        public static double[] HornerDerivatives(double[] coeffsP, double c, int m, int precision)
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
        public static double[] MultiplyPolynomial(double[] P, double c, int precision)
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
        public static double Factorial(int n)
        {
            double res = 1;
            for (int i = 2; i <= n; i++) res *= i;
            return res;
        }
    }
}
