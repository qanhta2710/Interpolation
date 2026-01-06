using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interpolation.Methods
{
    // Class chứa kết quả trả về
    public class EigenResult
    {
        public List<double> Eigenvalues { get; set; }
        public string Log { get; set; }
    }

    public class EigenvalueSolver
    {
        public static EigenResult SolveSturmLiouville(
            BoundaryValueSolver.FuncX p,
            BoundaryValueSolver.FuncX q,
            BoundaryValueSolver.FuncX r,
            double a, double b, double h,
            BoundaryValueSolver.BoundaryCondition left,
            BoundaryValueSolver.BoundaryCondition right)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== GIẢI BÀI TOÁN TRỊ RIÊNG (STURM-LIOUVILLE) ===");
            sb.AppendLine("Phương trình: [p(x)u']' - q(x)u = λ·r(x)·u");
            sb.AppendLine($"Miền tính: [{a}, {b}], Bước lưới h = {h}");

            int N = (int)Math.Round((b - a) / h);
            sb.AppendLine($"Số khoảng chia N = {N}");
            sb.AppendLine();

            double[] x = new double[N + 1];
            for (int i = 0; i <= N; i++) x[i] = a + i * h;

            // 1. Xây dựng ma trận
            sb.AppendLine("1. RỜI RẠC HÓA & XÂY DỰNG MA TRẬN");
            sb.AppendLine("   Sử dụng sai phân hữu hạn để đưa về dạng ma trận tổng quát:");
            sb.AppendLine("   M·u = λ·N·u");
            sb.AppendLine("   Trong đó M là ma trận tam giác (vế trái), N là ma trận đường chéo (vế phải).");
            sb.AppendLine();

            double[,] MatA = new double[N + 1, N + 1]; // Đây là M
            double[] MatB_Diag = new double[N + 1];    // Đây là đường chéo của N

            // Ghi log cấu trúc 3 đường chéo
            sb.AppendLine(string.Format("{0,5} | {1,12} {2,12} {3,12} | {4,12}",
                "Idx", "A (u_i-1)", "B (u_i)", "C (u_i+1)", "R (Vế phải)"));
            sb.AppendLine(new string('-', 75));

            for (int i = 1; i < N; i++)
            {
                double p_plus = p(x[i] + h / 2.0);
                double p_minus = p(x[i] - h / 2.0);
                double q_val = q(x[i]);
                double r_val = r(x[i]);

                // Các hệ số của phương trình sai phân
                double val_A = p_minus / (h * h);
                double val_B = -((p_plus + p_minus) / (h * h) + q_val);
                double val_C = p_plus / (h * h);

                MatA[i, i - 1] = val_A;
                MatA[i, i] = val_B;
                MatA[i, i + 1] = val_C;
                MatB_Diag[i] = r_val;

                // Log vài dòng đại diện
                if (i <= 3 || i >= N - 2)
                {
                    sb.AppendLine(string.Format("{0,5} | {1,12:F4} {2,12:F4} {3,12:F4} | {4,12:F4}",
                        i, val_A, val_B, val_C, r_val));
                }
                else if (i == 4) sb.AppendLine("   ...   |      ...          ...          ...     |      ...");
            }

            // Xử lý biên
            MatA[0, 0] = left.Beta - left.Alpha / h;
            MatA[0, 1] = left.Alpha / h;
            MatB_Diag[0] = 0;

            MatA[N, N - 1] = -right.Alpha / h;
            MatA[N, N] = right.Beta + right.Alpha / h;
            MatB_Diag[N] = 0;

            sb.AppendLine();
            sb.AppendLine("2. CHUYỂN VỀ BÀI TOÁN TRỊ RIÊNG CHUẨN");
            sb.AppendLine("   Biến đổi: (N^-1)·M·u = λ·u  =>  C·u = λ·u");
            sb.AppendLine("   (Loại bỏ hàng/cột 0 và N do điều kiện biên Dirichlet thuần nhất u=0)");

            // Rút gọn ma trận (Lấy lõi trong)
            int innerSize = N - 1;
            double[,] Core = new double[innerSize, innerSize];

            for (int r_idx = 0; r_idx < innerSize; r_idx++)
            {
                for (int c_idx = 0; c_idx < innerSize; c_idx++)
                {
                    double r_val = r(x[r_idx + 1]);
                    if (Math.Abs(r_val) < 1e-9) r_val = 1.0;
                    Core[r_idx, c_idx] = MatA[r_idx + 1, c_idx + 1] / r_val;
                }
            }

            sb.AppendLine($"   Kích thước ma trận rút gọn C: {innerSize} x {innerSize}");
            sb.AppendLine();

            // 3. Giải QR
            sb.AppendLine("3. TÌM TRỊ RIÊNG (THUẬT TOÁN QR)");
            sb.AppendLine("   Thực hiện phân rã QR lặp lại để đưa ma trận về dạng tam giác trên.");
            sb.AppendLine("   Các phần tử đường chéo chính sẽ hội tụ về các trị riêng.");

            // Tăng số lần lặp để chính xác hơn
            var eigenvalues = QREigenvalues(Core, 100);

            sb.AppendLine($"   -> Tìm thấy {eigenvalues.Count} trị riêng.");

            return new EigenResult
            {
                Eigenvalues = eigenvalues,
                Log = sb.ToString()
            };
        }

        private static List<double> QREigenvalues(double[,] A, int maxIter)
        {
            int n = A.GetLength(0);
            double[,] Ak = (double[,])A.Clone();

            for (int iter = 0; iter < maxIter; iter++)
            {
                QRDecomposition(Ak, out double[,] Q, out double[,] R);
                Ak = MultiplyMatrices(R, Q);
            }

            List<double> lambdas = new List<double>();
            for (int i = 0; i < n; i++)
            {
                lambdas.Add(Ak[i, i]);
            }
            lambdas.Sort();
            return lambdas;
        }

        private static void QRDecomposition(double[,] A, out double[,] Q, out double[,] R)
        {
            int n = A.GetLength(0);
            Q = new double[n, n];
            R = new double[n, n];

            // Copy A to Q
            for (int j = 0; j < n; j++)
                for (int i = 0; i < n; i++) Q[i, j] = A[i, j];

            // Gram-Schmidt
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < j; k++)
                {
                    double dot = 0;
                    for (int i = 0; i < n; i++) dot += Q[i, k] * Q[i, j];
                    R[k, j] = dot;
                    for (int i = 0; i < n; i++) Q[i, j] -= R[k, j] * Q[i, k];
                }

                double norm = 0;
                for (int i = 0; i < n; i++) norm += Q[i, j] * Q[i, j];
                norm = Math.Sqrt(norm);
                R[j, j] = norm;

                if (norm > 1e-10)
                {
                    for (int i = 0; i < n; i++) Q[i, j] /= norm;
                }
            }
        }

        private static double[,] MultiplyMatrices(double[,] A, double[,] B)
        {
            int n = A.GetLength(0);
            double[,] C = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (int k = 0; k < n; k++)
                        C[i, j] += A[i, k] * B[k, j];
            return C;
        }
    }
}