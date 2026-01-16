using AngouriMath;
using System;
using System.Text;
using System.Windows.Forms;

namespace Interpolation.Methods
{
    public class NewtonCotesIntegration
    {
        // Properties
        public double[] XData { get; set; }
        public double[] YData { get; set; }
        public Entity FunctionExpr { get; set; }

        public double A { get; set; }  // Cận dưới
        public double B { get; set; }  // Cận trên
        public double H { get; set; }  // Bước nhảy
        public int TotalSteps { get; set; } // Tổng số khoảng chia nhỏ (phải chia hết cho Order)
        public int Order { get; set; } // Bậc của Newton-Cotes (n = 4, 5, 6...)

        public double Epsilon { get; set; }  // Sai số cho phép
        public double MaxDerivative { get; set; } // Max |f^(n)| trên [a,b] để ước lượng (nếu có)

        public double Result { get; set; }
        public double EstimatedError { get; set; }
        public bool IsFromData { get; private set; }

        private StringBuilder calculationSteps;

        // --- CONSTRUCTOR 1: Dữ liệu rời rạc ---
        public NewtonCotesIntegration(double[] x, double[] y, double a, double b, int order)
        {
            XData = x;
            YData = y;
            A = a;
            B = b;
            Order = order;
            IsFromData = true;

            Solve();
            EstimateErrorRunge(); // Ước lượng sai số bằng Runge
        }

        // --- CONSTRUCTOR 2: Hàm F(x) với Sai số cho trước (Tính N) ---
        public NewtonCotesIntegration(string functionStr, double a, double b, double epsilon, int order)
        {
            try { FunctionExpr = functionStr; }
            catch (Exception ex) { throw new ArgumentException($"Lỗi phân tích hàm: {ex.Message}"); }

            A = a;
            B = b;
            Epsilon = epsilon;
            Order = order;
            IsFromData = false;

            CalculateOptimalN_ByRunge();
            Solve();
        }

        // --- CONSTRUCTOR 3: Hàm F(x) với N cho trước ---
        public NewtonCotesIntegration(string functionStr, double a, double b, int n, int order)
        {
            try { FunctionExpr = functionStr; }
            catch (Exception ex) { throw new ArgumentException($"Lỗi phân tích hàm: {ex.Message}"); }

            A = a;
            B = b;
            TotalSteps = n;
            Order = order;
            IsFromData = false;

            Solve();
            EstimateErrorRunge();
        }

        private void Solve()
        {
            calculationSteps = new StringBuilder();
            calculationSteps.AppendLine($"═══ NEWTON-COTES (BẬC {Order}) ═══");

            if (IsFromData) SolveFromData();
            else SolveFromFunction();
        }

        private void SolveFromData()
        {
            // Kiểm tra dữ liệu cơ bản
            if (XData.Length < 2)
                throw new ArgumentException("Cần ít nhất 2 điểm dữ liệu.");

            int indexA = Array.IndexOf(XData, A);
            int indexB = Array.IndexOf(XData, B);

            if (indexA == -1 || indexB == -1 || indexA >= indexB)
                throw new ArgumentException("Cận a, b không hợp lệ trong dữ liệu.");

            TotalSteps = indexB - indexA;
            H = (B - A) / TotalSteps;

            calculationSteps.AppendLine($"Dữ liệu rời rạc: [{A}, {B}]");
            calculationSteps.AppendLine($"Tổng số khoảng chia (N): {TotalSteps}");
            calculationSteps.AppendLine($"Bước nhảy h: {H}");

            int numMainSegments = TotalSteps / Order; 
            int remainder = TotalSteps % Order;      

            int mainPartEndIndex = indexA + (numMainSegments * Order);

            double sumMain = 0;
            double sumRem = 0;

            if (numMainSegments > 0)
            {
                calculationSteps.AppendLine($"--- PHẦN CHÍNH (Bậc {Order}) ---");
                calculationSteps.AppendLine($"Sử dụng Newton-Cotes bậc {Order} từ x[{indexA}] đến x[{mainPartEndIndex}]");

                double[] weights = NewtonCotesHelper.GetWeights(Order);

                calculationSteps.Append($"Hệ số Cotes bậc {Order}: [ ");
                foreach (var w in weights) calculationSteps.Append($"{w:F4} ");
                calculationSteps.AppendLine("]");

                for (int seg = 0; seg < numMainSegments; seg++)
                {
                    int startIndex = indexA + (seg * Order);
                    double segmentSum = 0;

                    for (int j = 0; j <= Order; j++)
                    {
                        double yVal = YData[startIndex + j];
                        segmentSum += weights[j] * yVal;
                    }
                    sumMain += segmentSum;

                    if (seg < 3)
                        calculationSteps.AppendLine($"  Đoạn {seg + 1} (x[{startIndex}]..x[{startIndex + Order}]): Tổng trọng số = {segmentSum:F6}");
                }
                if (numMainSegments > 3) calculationSteps.AppendLine("  ...");
                calculationSteps.AppendLine($"Tổng trọng số phần chính: {sumMain:F8}");
            }

            if (remainder > 0)
            {
                calculationSteps.AppendLine();
                calculationSteps.AppendLine($"--- PHẦN DƯ ({remainder} khoảng cuối) ---");
                calculationSteps.AppendLine($"Số khoảng còn lại không chia hết cho {Order}.");
                calculationSteps.AppendLine($"Giảm bậc xuống: Sử dụng Newton-Cotes bậc {remainder} từ x[{mainPartEndIndex}] đến x[{indexB}]");

                double[] remWeights = NewtonCotesHelper.GetWeights(remainder);

                calculationSteps.Append($"Hệ số Cotes bậc {remainder}: [ ");
                foreach (var w in remWeights) calculationSteps.Append($"{w:F4} ");
                calculationSteps.AppendLine("]");

                double segmentSum = 0;
                for (int j = 0; j <= remainder; j++)
                {
                    double yVal = YData[mainPartEndIndex + j];
                    segmentSum += remWeights[j] * yVal;
                }
                sumRem += segmentSum;
                calculationSteps.AppendLine($"Tổng trọng số phần dư: {sumRem:F8}");
            }

            double totalSum = sumMain + sumRem;
            Result = totalSum * H;

            calculationSteps.AppendLine();
            calculationSteps.AppendLine("------------------------------------------------");
            calculationSteps.AppendLine($"Tổng trọng số toàn bộ = {sumMain:F8} + {sumRem:F8} = {totalSum:F8}");
            calculationSteps.AppendLine($"I ≈ h × Tổng = {H} × {totalSum:F8} = {Result}");
        }

        private void SolveFromFunction()
        {
            // Kiểm tra tính chia hết
            if (TotalSteps % Order != 0)
            {
                // Tự động điều chỉnh N lên bội số gần nhất
                int oldN = TotalSteps;
                TotalSteps = ((TotalSteps + Order - 1) / Order) * Order;
                calculationSteps.AppendLine($"(!) Đã điều chỉnh N từ {oldN} lên {TotalSteps} để chia hết cho bậc {Order}.");
            }

            H = (B - A) / TotalSteps;
            XData = new double[TotalSteps + 1];
            YData = new double[TotalSteps + 1];

            calculationSteps.AppendLine($"Hàm số: f(x) = {FunctionExpr}");
            calculationSteps.AppendLine($"Đoạn: [{A}, {B}], N = {TotalSteps}, h = {H}");

            var compiledFunc = FunctionExpr.Compile("x");

            // Tính giá trị các điểm
            for (int i = 0; i <= TotalSteps; i++)
            {
                XData[i] = A + i * H;
                YData[i] = EvaluateFunction(compiledFunc, XData[i]);
            }

            // Lấy hệ số
            double[] weights = NewtonCotesHelper.GetWeights(Order);
            double sum = 0;
            int numSegments = TotalSteps / Order;

            for (int seg = 0; seg < numSegments; seg++)
            {
                int startIndex = seg * Order;
                double segmentSum = 0;
                for (int j = 0; j <= Order; j++)
                {
                    segmentSum += weights[j] * YData[startIndex + j];
                }
                sum += segmentSum;
            }

            Result = sum * H;
            calculationSteps.AppendLine($"\nKết quả tính toán: I ≈ {Result}");
        }

        private void CalculateOptimalN_ByRunge()
        {
            int currentN = Order;

            calculationSteps.AppendLine("Tự động tìm N tối ưu dựa trên sai số Runge...");

            while (currentN < 10000) 
            {
                var calc = new NewtonCotesIntegration(FunctionExpr.ToString(), A, B, currentN, Order);
                double valN = calc.Result;

                var calc2N = new NewtonCotesIntegration(FunctionExpr.ToString(), A, B, currentN * 2, Order);
                double val2N = calc2N.Result;

                int accuracyOrder = (Order % 2 == 0) ? Order + 1 : Order;

                double error = Math.Abs(val2N - valN) / (Math.Pow(2, Order) - 1); // Ước lượng

                if (error < Epsilon)
                {
                    TotalSteps = currentN * 2; // Chọn mức lưới mịn hơn để đảm bảo
                    calculationSteps.AppendLine($"-> Chọn N = {TotalSteps} (Sai số ước lượng: {error} < {Epsilon})");
                    return;
                }

                currentN *= 2;
            }

            TotalSteps = currentN;
            calculationSteps.AppendLine($"-> Đạt giới hạn lặp, chọn N = {TotalSteps}");
        }

        private void EstimateErrorRunge()
        {
            int p = (Order % 2 == 0) ? Order + 2 : Order + 1;

            int m = -1;

            for (int scale = 2; scale * Order <= TotalSteps; scale++)
            {
                if (TotalSteps % (Order * scale) == 0)
                {
                    m = scale;
                    break;
                }
            }

            if (m == -1)
            {
                EstimatedError = 0;
                calculationSteps.AppendLine($"\n(!) Không tìm được lưới thô phù hợp để tính sai số.");
                return;
            }

            AngouriMath.Core.FastExpression compiledFunc = null;
            if (!IsFromData)
            {
                try
                {
                    compiledFunc = FunctionExpr.Compile("x");
                }
                catch
                {
                    EstimatedError = 0; return;
                }
            }
            double[] weights = NewtonCotesHelper.GetWeights(Order);
            double sumCoarse = 0;
            double H_coarse = m * H; 
            int numSegmentsCoarse = TotalSteps / (Order * m);

            for (int seg = 0; seg < numSegmentsCoarse; seg++)
            {
                int startBaseIdx = seg * (Order * m);

                for (int j = 0; j <= Order; j++)
                {
                    int idx = startBaseIdx + (j * m);
                    double yVal = 0;

                    if (IsFromData)
                    {
                        yVal = YData[idx];
                    }
                    else
                    {
                        double xVal = A + idx * H;

                        yVal = EvaluateFunction(compiledFunc, xVal);
                    }

                    sumCoarse += weights[j] * yVal;
                }
            }

            double I_coarse = sumCoarse * H_coarse;

            double denominator = Math.Pow(m, p) - 1;
            EstimatedError = Math.Abs(Result - I_coarse) / denominator;

            calculationSteps.AppendLine("\n═══ SAI SỐ LƯỚI PHỦ (RUNGE TỔNG QUÁT) ═══");
            calculationSteps.AppendLine($"Tỷ lệ lưới s = m = {m}");
            calculationSteps.AppendLine($"I_mịn (h)      = {Result}");
            calculationSteps.AppendLine($"I_thô ({m}h)     = {I_coarse}");
            calculationSteps.AppendLine($"Công thức: R ≈ |I - I_thô| / ({m}^{p} - 1)");
            calculationSteps.AppendLine($"R ≈ {EstimatedError}");
        }

        private double EvaluateFunction(AngouriMath.Core.FastExpression compiledFunc, double x)
        {
            return (double)compiledFunc.Call(x).Real;
        }

        public void DisplayResults(RichTextBox rtb)
        {
            rtb.Clear();
            rtb.Text = calculationSteps.ToString() +
                       $"\n════════════════════════════════════\nKẾT QUẢ: {Result}";
        }
    }
}