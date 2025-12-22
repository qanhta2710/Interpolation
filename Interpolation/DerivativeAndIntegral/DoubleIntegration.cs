using AngouriMath;
using System;
using System.Text;
using System.Windows.Forms;

namespace Interpolation.Methods
{
    public class DoubleIntegration
    {
        public Entity FunctionExpr { get; set; }
        public double X0 { get; set; }
        public double Xn { get; set; }
        public double Y0 { get; set; }
        public double Ym { get; set; }

        // Không bắt buộc nhập Nx, Ny từ ngoài nữa, class sẽ tự tính
        public double Epsilon { get; set; }

        public double Result { get; private set; }
        public int FinalNx { get; private set; } // Để hiển thị xem máy đã chia bao nhiêu khoảng
        public int FinalNy { get; private set; }

        private StringBuilder calculationSteps;
        private AngouriMath.Core.FastExpression compiledFunc;

        public DoubleIntegration(string funcStr, double x0, double xn, double y0, double ym, double epsilon)
        {
            try
            {
                FunctionExpr = funcStr;
                compiledFunc = FunctionExpr.Compile("x", "y");
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Lỗi phân tích hàm: {ex.Message}");
            }

            X0 = x0; Xn = xn;
            Y0 = y0; Ym = ym;
            Epsilon = epsilon;
            calculationSteps = new StringBuilder();
        }

        // --- PHƯƠNG PHÁP HÌNH THANG TỰ ĐỘNG ---
        public void CalculateTrapezoidalAuto()
        {
            calculationSteps.Clear();
            calculationSteps.AppendLine("═══ TÍCH PHÂN KÉP - HÌNH THANG ═══");
            calculationSteps.AppendLine($"Hàm: {FunctionExpr}");
            calculationSteps.AppendLine($"Miền: X[{X0}, {Xn}], Y[{Y0}, {Ym}]");
            calculationSteps.AppendLine($"Sai số cho phép: {Epsilon}");
            calculationSteps.AppendLine("------------------------------------------------");

            // Bắt đầu với số khoảng chia nhỏ (ví dụ 10)
            int nX = 10;
            int nY = 10;
            double prevResult = double.MaxValue;
            double currentResult = 0;
            int maxIter = 10; // Giới hạn số lần tăng lưới để tránh treo máy

            for (int k = 1; k <= maxIter; k++)
            {
                currentResult = ComputeTrapezoidalOnce(nX, nY);

                // Đánh giá sai số Runge: |I_2n - I_n| / 3
                double error = Math.Abs(currentResult - prevResult) / 3.0;

                calculationSteps.AppendLine($"Lần lặp {k}: Nx={nX}, Ny={nY} => I = {currentResult:F9} (Sai số ước lượng: {error:E})");

                if (error < Epsilon)
                {
                    calculationSteps.AppendLine("-> Sai số nhỏ hơn mức cho phép. DỪNG.");
                    break;
                }

                prevResult = currentResult;
                // Tăng gấp đôi số khoảng chia cho lần sau
                nX *= 2;
                nY *= 2;
            }

            Result = currentResult;
            FinalNx = nX;
            FinalNy = nY;

            calculationSteps.AppendLine("------------------------------------------------");
            calculationSteps.AppendLine($"KẾT QUẢ CUỐI CÙNG: I ≈ {Result}");
        }

        // --- PHƯƠNG PHÁP SIMPSON TỰ ĐỘNG ---
        public void CalculateSimpsonAuto()
        {
            calculationSteps.Clear();
            calculationSteps.AppendLine("═══ TÍCH PHÂN KÉP - SIMPSON ═══");
            calculationSteps.AppendLine($"Hàm: {FunctionExpr}");

            // Simpson yêu cầu N chẵn, bắt đầu với 10
            int nX = 10;
            int nY = 10;
            double prevResult = double.MaxValue;
            double currentResult = 0;
            int maxIter = 10;

            for (int k = 1; k <= maxIter; k++)
            {
                currentResult = ComputeSimpsonOnce(nX, nY);

                // Đánh giá sai số Runge Simpson: |I_2n - I_n| / 15
                double error = Math.Abs(currentResult - prevResult) / 15.0;

                calculationSteps.AppendLine($"Lần lặp {k}: Nx={nX}, Ny={nY} => I = {currentResult:F9} (Sai số ước lượng: {error:E})");

                if (error < Epsilon)
                {
                    calculationSteps.AppendLine("-> Sai số nhỏ hơn mức cho phép. DỪNG.");
                    break;
                }

                prevResult = currentResult;
                nX *= 2;
                nY *= 2;
            }

            Result = currentResult;
            FinalNx = nX;
            FinalNy = nY;

            calculationSteps.AppendLine("------------------------------------------------");
            calculationSteps.AppendLine($"KẾT QUẢ CUỐI CÙNG: I ≈ {Result}");
        }

        // Hàm tính toán lõi cho 1 lần chạy (Hình thang)
        private double ComputeTrapezoidalOnce(int nx, int ny)
        {
            double hx = (Xn - X0) / nx;
            double hy = (Ym - Y0) / ny;
            double sum = 0;

            for (int i = 0; i <= nx; i++)
            {
                for (int j = 0; j <= ny; j++)
                {
                    double x = X0 + i * hx;
                    double y = Y0 + j * hy;
                    double val = Evaluate(x, y);

                    double weight = 0;
                    bool isCorner = (i == 0 || i == nx) && (j == 0 || j == ny);
                    bool isEdge = (i == 0 || i == nx || j == 0 || j == ny);

                    if (isCorner) weight = 1;
                    else if (isEdge) weight = 2;
                    else weight = 4;

                    sum += weight * val;
                }
            }
            return (hx * hy / 4.0) * sum;
        }

        // Hàm tính toán lõi cho 1 lần chạy (Simpson)
        private double ComputeSimpsonOnce(int nx, int ny)
        {
            double hx = (Xn - X0) / nx;
            double hy = (Ym - Y0) / ny;
            double sum = 0;

            for (int i = 0; i <= nx; i++)
            {
                for (int j = 0; j <= ny; j++)
                {
                    double x = X0 + i * hx;
                    double y = Y0 + j * hy;
                    double val = Evaluate(x, y);

                    double wx = (i == 0 || i == nx) ? 1 : (i % 2 == 1 ? 4 : 2);
                    double wy = (j == 0 || j == ny) ? 1 : (j % 2 == 1 ? 4 : 2);
                    sum += (wx * wy) * val;
                }
            }
            return (hx * hy / 9.0) * sum;
        }

        private double Evaluate(double x, double y)
        {
            return (double)compiledFunc.Call(x, y).Real;
        }

        public void DisplayResults(RichTextBox rtb)
        {
            rtb.Text = calculationSteps.ToString();
        }
    }
}