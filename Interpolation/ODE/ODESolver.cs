using System;
using System.Collections.Generic;
using AngouriMath; // Thư viện parse hàm
using System.Linq; // Hỗ trợ xử lý mảng

namespace Interpolation
{
    public class ODESolver
    {
        // Delegate đại diện cho hàm vector F(t, Y)
        // Input: thời gian t, mảng trạng thái hiện tại y[]
        // Output: mảng đạo hàm dy[]
        public delegate double[] DerivativeFunc(double t, double[] y);

        /// <summary>
        /// Hàm hỗ trợ biên dịch chuỗi hàm thành delegate để tính toán nhanh
        /// </summary>
        public static DerivativeFunc CompileSystem(string[] strFuncs)
        {
            int dim = strFuncs.Length;
            var compiledExprs = new AngouriMath.Core.FastExpression[dim];

            // Biên dịch từng phương trình. Hỗ trợ biến t, x, y, z
            for (int i = 0; i < dim; i++)
            {
                // Parse chuỗi thành biểu thức AngouriMath
                Entity expr = strFuncs[i];
                // Compile sang FastExpression để tính toán cực nhanh
                // Lưu ý: Thứ tự tham số compile phải cố định
                compiledExprs[i] = expr.Compile("t", "x", "y", "z");
            }

            // Trả về hàm delegate thực thi các biểu thức đã biên dịch
            return (t, yVal) =>
            {
                double[] dy = new double[dim];
                // Map mảng yVal sang x, y, z. Nếu mảng ko đủ dài thì gán bằng 0
                double x = yVal.Length > 0 ? yVal[0] : 0;
                double y = yVal.Length > 1 ? yVal[1] : 0;
                double z = yVal.Length > 2 ? yVal[2] : 0;

                for (int i = 0; i < dim; i++)
                {
                    // Gọi hàm đã compile
                    dy[i] = (double)compiledExprs[i].Call(t, x, y, z).Real;
                }
                return dy;
            };
        }

        // ==========================================================
        // 1. THUẬT TOÁN EULER HIỆN (EULER EXPLICIT)
        // ==========================================================
        public static List<double[]> SolveEulerExplicit(DerivativeFunc f, double[] y0, double t0, double tend, double h)
        {
            var result = new List<double[]>();
            SaveState(result, t0, y0); // Lưu trạng thái ban đầu (B0)

            double t = t0;
            double[] y = (double[])y0.Clone();
            int n = y.Length;

            // Vòng lặp theo thời gian (B1 -> B4)
            while (t < tend - 1e-9)
            {
                // B1: Tính các đạo hàm (Fx, Fy, Fz...) tại (t, y)
                double[] dy = f(t, y);

                // B2: Cập nhật trạng thái mới: y_new = y + h * dy
                for (int i = 0; i < n; i++)
                {
                    y[i] = y[i] + h * dy[i];
                }

                // Cập nhật t
                t += h;

                // Lưu kết quả
                SaveState(result, t, y);
            }
            return result;
        }

        // ==========================================================
        // 2. THUẬT TOÁN EULER ẨN (EULER IMPLICIT) - Dùng Lặp đơn
        // ==========================================================
        public static List<double[]> SolveEulerImplicit(DerivativeFunc f, double[] y0, double t0, double tend, double h, double epsilon = 1e-4)
        {
            var result = new List<double[]>();
            SaveState(result, t0, y0);

            double t = t0;
            double[] y = (double[])y0.Clone();
            int n = y.Length;
            int maxIter = 1000; // Giới hạn số lần lặp để tránh treo máy

            while (t < tend - 1e-9)
            {
                double t_next = t + h;

                // B1: Dự báo (Predictor) bằng Euler Hiện để lấy giá trị khởi tạo cho y*
                double[] dy_curr = f(t, y);
                double[] y_star = new double[n]; // Đây là y* trong vở
                for (int i = 0; i < n; i++) y_star[i] = y[i] + h * dy_curr[i];

                // B3: Vòng lặp hội tụ (Corrector)
                for (int k = 0; k < maxIter; k++)
                {
                    // 1. Tính đạo hàm tại điểm dự báo: F* = f(t_next, y*)
                    double[] dy_next = f(t_next, y_star);

                    double[] y_temp = new double[n]; // Đây là y_temp trong vở
                    double error = 0;

                    // 2. Tính y_temp = y + h * F*
                    for (int i = 0; i < n; i++)
                    {
                        y_temp[i] = y[i] + h * dy_next[i];

                        // 3. Tính tổng sai số: |x_temp - x*| + ...
                        error += Math.Abs(y_temp[i] - y_star[i]);
                    }

                    // Cập nhật y* = y_temp cho lần lặp sau
                    Array.Copy(y_temp, y_star, n);

                    // 4. Kiểm tra điều kiện dừng
                    if (error < epsilon) break;
                }

                // B4: Cập nhật giá trị chính thức
                y = y_star;
                t = t_next;
                SaveState(result, t, y);
            }
            return result;
        }

        // ==========================================================
        // 3. THUẬT TOÁN HÌNH THANG (TRAPEZOIDAL) - Dùng Lặp đơn
        // ==========================================================
        public static List<double[]> SolveTrapezoidal(DerivativeFunc f, double[] y0, double t0, double tend, double h, double epsilon = 1e-4)
        {
            var result = new List<double[]>();
            SaveState(result, t0, y0);

            double t = t0;
            double[] y = (double[])y0.Clone();
            int n = y.Length;
            int maxIter = 1000;

            while (t < tend - 1e-9)
            {
                double t_next = t + h;

                // B1: Tính F tại điểm hiện tại: F_curr = f(t, y)
                double[] dy_curr = f(t, y);

                // B2: Dự báo y* bằng Euler Hiện
                double[] y_star = new double[n];
                for (int i = 0; i < n; i++) y_star[i] = y[i] + h * dy_curr[i];

                // B3: Vòng lặp hội tụ
                for (int k = 0; k < maxIter; k++)
                {
                    // 1. Tính đạo hàm tại điểm tương lai: F* = f(t_next, y*)
                    double[] dy_next = f(t_next, y_star);

                    double[] y_temp = new double[n];
                    double error = 0;

                    // 2. Công thức Hình thang: y_temp = y + (h/2) * (F_curr + F*)
                    for (int i = 0; i < n; i++)
                    {
                        y_temp[i] = y[i] + (h / 2.0) * (dy_curr[i] + dy_next[i]);

                        // 3. Tính sai số
                        error += Math.Abs(y_temp[i] - y_star[i]);
                    }

                    // Cập nhật
                    Array.Copy(y_temp, y_star, n);

                    // 4. Kiểm tra sai số
                    if (error < epsilon) break;
                }

                // Cập nhật chính thức
                y = y_star;
                t = t_next;
                SaveState(result, t, y);
            }
            return result;
        }

        // Hàm tiện ích để lưu dòng kết quả: [t, x, y, z...]
        private static void SaveState(List<double[]> list, double t, double[] y)
        {
            double[] row = new double[y.Length + 1];
            row[0] = t;
            Array.Copy(y, 0, row, 1, y.Length);
            list.Add(row);
        }
    }
}