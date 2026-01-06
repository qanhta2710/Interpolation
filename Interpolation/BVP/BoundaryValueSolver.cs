using AngouriMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interpolation.Methods
{
    public class BoundaryValueSolver
    {
        public delegate double FuncX(double x);

        // Điều kiện biên: Alpha*u' + Beta*u = Gamma
        public struct BoundaryCondition
        {
            public double Alpha;
            public double Beta;
            public double Gamma;
        }
        public class BVPResult
        {
            public List<double[]> DataPoints { get; set; } // {x, u}
            public string Log { get; set; } // Chi tiết thuật toán
            public double MaxVal { get; set; }
            public double MaxX { get; set; }
            public double MinVal { get; set; }
            public double MinX { get; set; }
        }
        public static FuncX CompileFunction(string funcStr)
        {
            try
            {
                Entity expr = funcStr;
                var compiled = expr.Compile("x");
                return (x) => (double)compiled.Call(x).Real;
            }
            catch
            {
                throw new ArgumentException($"Lỗi cú pháp hàm: {funcStr}");
            }
        }
        public static BVPResult SolveBVP(FuncX p, FuncX q, FuncX f,
                                              double a, double b, double h,
                                              BoundaryCondition left, BoundaryCondition right)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== GIẢI BÀI TOÁN BIÊN BẰNG SAI PHÂN HỮU HẠN ===");
            sb.AppendLine($"Phương trình: [p(x)u']' - q(x)u = -f(x)");
            sb.AppendLine($"Miền tính: [{a}, {b}], Bước lưới h = {h}");

            int N = (int)Math.Round((b - a) / h);

            sb.AppendLine($"Số khoảng chia N = {N}");
            sb.AppendLine();

            double[] x = new double[N + 1];

            for (int i = 0; i <= N; i++) x[i] = a + i * h;

            double[] A_arr = new double[N + 1]; // Hệ số u_{i-1}
            double[] B_arr = new double[N + 1]; // Hệ số u_i
            double[] C_arr = new double[N + 1]; // Hệ số u_{i+1}
            double[] D_arr = new double[N + 1]; // -f_i

            // Sai phân hoá PTVP

            for (int i = 1; i < N; i++)
            {
                double x_half_plus = x[i] + h / 2.0;
                double x_half_minus = x[i] - h / 2.0;

                double p_plus = p(x_half_plus);   // p_{i+1/2}
                double p_minus = p(x_half_minus); // p_{i-1/2}
                double q_val = q(x[i]);
                double f_val = f(x[i]);

                A_arr[i] = p_minus / (h * h);
                B_arr[i] = -p_plus / (h * h) - p_minus / (h * h) - q_val;
                C_arr[i] = p_plus / (h * h);
                D_arr[i] = -f_val;
            }

            // Biên trái
            sb.AppendLine("--- XỬ LÝ BIÊN TRÁI (x0) ---");
            sb.AppendLine($"Điều kiện: {left.Alpha}*u'(a) + {left.Beta}*u(a) = {left.Gamma}");
            if (Math.Abs(left.Alpha) < 1e-9) // Dirichlet
            {
                A_arr[0] = 0;
                B_arr[0] = 1.0;
                C_arr[0] = 0;
                D_arr[0] = left.Gamma / left.Beta;
                sb.AppendLine("-> Loại 1 (Dirichlet): u0 = Gamma/Beta");
            }
            else // Neumann / Robin 
            {
                double p_half = p(x[0] + h / 2.0);
                double q_0 = q(x[0]);
                double f_0 = f(x[0]);

                A_arr[0] = 0;

                B_arr[0] = -(p_half + (h * h / 2.0) * q_0) + (h * left.Beta / left.Alpha) * p_half;
                C_arr[0] = p_half;

                D_arr[0] = -(h * h / 2.0) * f_0 + (h * left.Gamma / left.Alpha) * p_half;

                sb.AppendLine("-> Loại 2/3 (Neumann/Robin)");
            }

            sb.AppendLine($"   PT biên trái: {B_arr[0]:F4}*u0 + {C_arr[0]:F4}*u1 = {D_arr[0]:F4}");
            sb.AppendLine();

            // Biên phải
            sb.AppendLine("--- XỬ LÝ BIÊN PHẢI (xN) ---");
            sb.AppendLine($"Điều kiện: {right.Alpha}*u'(b) + {right.Beta}*u(b) = {right.Gamma}");
            if (Math.Abs(right.Alpha) < 1e-9) // Dirichlet
            {
                A_arr[N] = 0;
                B_arr[N] = 1.0;
                C_arr[N] = 0;
                D_arr[N] = right.Gamma / right.Beta;
                sb.AppendLine("-> Loại 1 (Dirichlet): uN = Gamma/Beta");
            }
            else // Neumann / Robin
            {
                double p_half_left = p(x[N] - h / 2.0);
                double q_N = q(x[N]);
                double f_N = f(x[N]);

                A_arr[N] = p_half_left;

                B_arr[N] = -(p_half_left + (h * h / 2.0) * q_N) - (h * right.Beta / right.Alpha) * p_half_left;
                C_arr[N] = 0;

                D_arr[N] = -(h * h / 2.0) * f_N - (h * right.Gamma / right.Alpha) * p_half_left;
                sb.AppendLine("-> Loại 2/3 (Neumann/Robin - Chính xác O(h^2))");
            }

            sb.AppendLine($"   PT biên phải: {A_arr[N]:F4}*u_{N - 1} + {B_arr[N]:F4}*u_{N} = {D_arr[N]:F4}");
            sb.AppendLine();

            // Giải hệ phương trình
            sb.AppendLine("--- HỆ PHƯƠNG TRÌNH ĐẠI SỐ (MA TRẬN 3 ĐƯỜNG CHÉO) ---");
            sb.AppendLine("Dạng: A_i*u_{i-1} + B_i*u_i + C_i*u_{i+1} = D_i");
            sb.AppendLine(string.Format("{0,5} | {1,12} {2,12} {3,12} | {4,12}", "Idx", "A (u_i-1)", "B (u_i)", "C (u_i+1)", "D (VP)"));
            sb.AppendLine(new string('-', 70));

            for (int i = 0; i <= N; i++)
            {
                sb.AppendLine(string.Format("{0,5} | {1,12:F4} {2,12:F4} {3,12:F4} | {4,12:F4}",
                    i, A_arr[i], B_arr[i], C_arr[i], D_arr[i]));
            }
            sb.AppendLine();

            double[] u = ThomasAlgorithm(A_arr, B_arr, C_arr, D_arr, N + 1);

            double maxVal = double.MinValue, minVal = double.MaxValue;
            double maxX = 0, minX = 0;

            List<double[]> resultList = new List<double[]>();
            for (int i = 0; i <= N; i++)
            {
                resultList.Add(new double[] { x[i], u[i] });

                if (u[i] > maxVal) { maxVal = u[i]; maxX = x[i]; }
                if (u[i] < minVal) { minVal = u[i]; minX = x[i]; }
            }

            return new BVPResult
            {
                DataPoints = resultList,
                Log = sb.ToString(),
                MaxVal = maxVal,
                MaxX = maxX,
                MinVal = minVal,
                MinX = minX
            };
        }

        private static double[] ThomasAlgorithm(double[] a, double[] b, double[] c, double[] d, int n)
        {
            double[] c_prime = new double[n];
            double[] d_prime = new double[n];
            double[] x = new double[n];

            c_prime[0] = c[0] / b[0];
            d_prime[0] = d[0] / b[0];

            for (int i = 1; i < n; i++)
            {
                double temp = b[i] - a[i] * c_prime[i - 1];
                if (i < n - 1)
                {
                    c_prime[i] = c[i] / temp;
                }
                d_prime[i] = (d[i] - a[i] * d_prime[i - 1]) / temp;
            }

            x[n - 1] = d_prime[n - 1];
            for (int i = n - 2; i >= 0; i--)
            {
                x[i] = d_prime[i] - c_prime[i] * x[i + 1];
            }

            return x;
        }
    }
}