using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Text;

namespace Interpolation.Utilities
{
    public static class Function
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
        public static string PolynomialToString(double[] coeffs, string variable = "x")
        {
            var sb = new StringBuilder();
            int n = coeffs.Length;

            for (int i = 0; i < n; i++)
            {
                double c = coeffs[i];
                int power = n - i - 1;

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
                    sb.Append(variable);
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
