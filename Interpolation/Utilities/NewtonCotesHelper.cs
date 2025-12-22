using System;

namespace Interpolation.Methods
{
    public static class NewtonCotesHelper
    {
        /// <summary>
        /// Tính hệ số Newton-Cotes đã chuẩn hóa cho bậc n
        /// Kết quả trả về là mảng C[i] sao cho Tích phân trên [0, n] = Sum(C[i] * y[i])
        /// Lưu ý: Tổng các C[i] sẽ bằng n (chiều dài đoạn cơ sở)
        /// </summary>
        public static double[] GetWeights(int n)
        {
            double[] weights = new double[n + 1];

            for (int i = 0; i <= n; i++)
            {
                double denominator = 1.0;
                for (int j = 0; j <= n; j++)
                {
                    if (i != j) denominator *= (i - j);
                }

                double[] polyCoeffs = { 1.0 };

                for (int j = 0; j <= n; j++)
                {
                    if (i != j)
                    {
                        polyCoeffs = MultiplyPolyWithBinomial(polyCoeffs, -j);
                    }
                }

                double integralValue = 0;
                for (int k = 0; k < polyCoeffs.Length; k++)
                {
                    integralValue += polyCoeffs[k] * Math.Pow(n, k + 1) / (k + 1);
                }

                weights[i] = integralValue / denominator;
            }

            return weights;
        }

        private static double[] MultiplyPolyWithBinomial(double[] poly, double c)
        {
            double[] newPoly = new double[poly.Length + 1];
            for (int i = 0; i < poly.Length; i++)
            {
                newPoly[i + 1] += poly[i];
                newPoly[i] += poly[i] * c;
            }
            return newPoly;
        }
    }
}