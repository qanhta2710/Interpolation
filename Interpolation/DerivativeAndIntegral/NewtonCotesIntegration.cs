using AngouriMath;
using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Interpolation.Methods
{
    public class NewtonCotesIntegration
    {
        // Properties
        public double[] XData { get; set; }
        public double[] YData { get; set; }
        public Entity FunctionExpr { get; set; }

        public double A { get; set; }
        public double B { get; set; }
        public double H { get; set; }
        public int TotalSteps { get; set; }
        public int Order { get; set; } // Bậc chính

        public double Epsilon { get; set; }
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
            if (EstimatedError == 0) EstimateErrorRunge();
        }

        // --- CONSTRUCTOR 2: Hàm F(x) với Sai số cho trước ---
        public NewtonCotesIntegration(string functionStr, double a, double b, double epsilon, int order)
        {
            try { FunctionExpr = functionStr; }
            catch (Exception ex) { throw new ArgumentException($"Lỗi phân tích hàm: {ex.Message}"); }

            A = a; B = b; Epsilon = epsilon; Order = order; IsFromData = false;
            CalculateOptimalN_ByRunge();
            Solve();
        }

        // --- CONSTRUCTOR 3: Hàm F(x) với N cho trước ---
        public NewtonCotesIntegration(string functionStr, double a, double b, int n, int order)
        {
            try { FunctionExpr = functionStr; }
            catch (Exception ex) { throw new ArgumentException($"Lỗi phân tích hàm: {ex.Message}"); }

            A = a; B = b; TotalSteps = n; Order = order; IsFromData = false;
            Solve();
            EstimateErrorRunge();
        }

        private void Solve()
        {
            calculationSteps = new StringBuilder();
            calculationSteps.AppendLine($"═══ NEWTON-COTES HỖN HỢP (BẬC CHÍNH {Order}) ═══");

            if (IsFromData) SolveFromData();
            else SolveFromFunction();
        }

        private void SolveFromData()
        {
            // Kiểm tra dữ liệu
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

            // --- XỬ LÝ CHIA ĐOẠN ---
            int numMainSegments = TotalSteps / Order; // Số đoạn lớn dùng bậc chính
            int remainder = TotalSteps % Order;       // Số khoảng dư

            // Index kết thúc của phần chính
            int mainPartEndIndex = indexA + (numMainSegments * Order);

            double sumMain = 0;
            double sumRem = 0;

            // 1. TÍNH PHẦN CHÍNH (Bậc Order)
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

            // 2. TÍNH PHẦN DƯ (Bậc remainder)
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

                // Phần dư là 1 đoạn lớn duy nhất có bậc = remainder
                double segmentSum = 0;
                for (int j = 0; j <= remainder; j++)
                {
                    double yVal = YData[mainPartEndIndex + j];
                    segmentSum += remWeights[j] * yVal;
                }
                sumRem += segmentSum;
                calculationSteps.AppendLine($"Tổng trọng số phần dư: {sumRem:F8}");
            }

            // Tổng kết
            double totalSum = sumMain + sumRem;
            Result = totalSum * H;

            calculationSteps.AppendLine();
            calculationSteps.AppendLine("------------------------------------------------");
            calculationSteps.AppendLine($"Tổng trọng số toàn bộ = {sumMain:F8} + {sumRem:F8} = {totalSum:F8}");
            calculationSteps.AppendLine($"I ≈ h × Tổng = {H} × {totalSum:F8} = {Result}");
        }

        private void SolveFromFunction()
        {
            // Tương tự như SolveFromData nhưng tự sinh dữ liệu
            H = (B - A) / TotalSteps;
            XData = new double[TotalSteps + 1];
            YData = new double[TotalSteps + 1];
            var compiledFunc = FunctionExpr.Compile("x");

            for (int i = 0; i <= TotalSteps; i++)
            {
                XData[i] = A + i * H;
                YData[i] = EvaluateFunction(compiledFunc, XData[i]);
            }

            // Gọi lại hàm xử lý dữ liệu để tránh viết lặp logic
            // Cần gán A, B khớp với XData[0] và XData[end] để IndexOf chạy đúng
            // Hoặc chỉnh sửa nhẹ SolveFromData để không phụ thuộc IndexOf nếu muốn tối ưu hơn.
            // Ở đây để đơn giản ta gọi lại SolveFromData vì mảng đã có.
            SolveFromData();
        }

        private void EstimateErrorRunge()
        {
            // Chiến thuật: Tính sai số Runge cho phần chính (Main) và phần dư (Rem) riêng biệt
            // Sau đó cộng dồn sai số tuyệt đối.

            int numMainSegments = TotalSteps / Order;
            int remainder = TotalSteps % Order;

            double errorMain = 0;
            double errorRem = 0;

            AngouriMath.Core.FastExpression compiledFunc = null;
            if (!IsFromData)
            {
                try { compiledFunc = FunctionExpr.Compile("x"); } catch { EstimatedError = 0; return; }
            }

            // --- 1. ĐÁNH GIÁ SAI SỐ PHẦN CHÍNH (Bậc = Order) ---
            if (numMainSegments > 0)
            {
                // Tìm tỷ lệ m để tạo lưới thô cho phần chính
                int pMain = (Order % 2 == 0) ? Order + 2 : Order + 1;
                int m = -1;
                // Thử tìm m nhỏ nhất >= 2 sao cho số đoạn lớn chia hết cho m
                for (int scale = 2; scale <= numMainSegments; scale++)
                {
                    if (numMainSegments % scale == 0) { m = scale; break; }
                }

                if (m != -1)
                {
                    // Tính lại I_fine (mịn) cho phần chính
                    // (Lấy lại kết quả từ mảng YData để đỡ tính lại hàm)
                    int indexA = IsFromData ? Array.IndexOf(XData, A) : 0;
                    double[] weights = NewtonCotesHelper.GetWeights(Order);
                    double sumFine = 0;
                    for (int seg = 0; seg < numMainSegments; seg++)
                    {
                        int sIdx = indexA + seg * Order;
                        for (int j = 0; j <= Order; j++) sumFine += weights[j] * YData[sIdx + j];
                    }
                    double I_fine = sumFine * H;

                    // Tính I_coarse (thô) với bước nhảy m*H
                    double sumCoarse = 0;
                    int numSegsCoarse = numMainSegments / m;

                    for (int seg = 0; seg < numSegsCoarse; seg++)
                    {
                        int startBaseIdx = indexA + seg * (Order * m);
                        for (int j = 0; j <= Order; j++)
                        {
                            int idx = startBaseIdx + (j * m);
                            double yVal = 0;
                            if (IsFromData) yVal = YData[idx];
                            else yVal = EvaluateFunction(compiledFunc, A + idx * H);
                            sumCoarse += weights[j] * yVal;
                        }
                    }
                    double I_coarse = sumCoarse * (m * H);

                    double denom = Math.Pow(m, pMain) - 1;
                    errorMain = Math.Abs(I_fine - I_coarse) / denom;
                }
                else
                {
                    calculationSteps.AppendLine("(!) Không đủ dữ liệu để tính Runge cho phần chính (Không chia được lưới thô).");
                }
            }

            // --- 2. ĐÁNH GIÁ SAI SỐ PHẦN DƯ (Bậc = remainder) ---
            if (remainder > 0)
            {
                // Phần dư dùng NC bậc 'remainder'.
                // Để tính Runge, ta cần lưới thô với bước 2H (m=2).
                // Điều kiện: Nếu là hàm số, ta luôn tính được.
                // Nếu là dữ liệu: Cần đảm bảo các điểm cách nhau 2H có tồn tại trong dữ liệu.
                // Phần dư hiện tại là [x_k ... x_N] với bước H.
                // Muốn tính bước 2H, ta cần các điểm x_k, x_{k+2}, x_{k+4}...
                // Tức là số khoảng dư 'remainder' phải chia hết cho 2.

                int pRem = (remainder % 2 == 0) ? remainder + 2 : remainder + 1;
                int m = 2; // Thử lưới đôi

                if (!IsFromData || remainder % 2 == 0)
                {
                    int indexA = IsFromData ? Array.IndexOf(XData, A) : 0;
                    int startRemIdx = indexA + numMainSegments * Order;

                    // I_fine (đã có hoặc tính lại cho chắc)
                    double[] remWeights = NewtonCotesHelper.GetWeights(remainder);
                    double sumFine = 0;
                    for (int j = 0; j <= remainder; j++)
                        sumFine += remWeights[j] * YData[startRemIdx + j];
                    double I_fine = sumFine * H;

                    // I_coarse (bước 2H)
                    // Ta coi phần dư này là 1 đoạn lớn bậc 'remainder'.
                    // Nếu dùng bước 2H, ta cần 'remainder' khoảng 2H.
                    // Tức là cần độ dài dữ liệu thực tế là remainder * 2 * H.
                    // Nhưng dữ liệu phần dư chỉ có độ dài remainder * H.
                    // => VỚI DỮ LIỆU CỐ ĐỊNH, KHÔNG THỂ TÍNH RUNGE CHO PHẦN DƯ BẰNG CÁCH GỘP BẬC CAO.
                    // => Trừ khi ta giảm bậc của lưới thô? Không, Runge yêu cầu cùng công thức.

                    // TUY NHIÊN: Nếu là HÀM SỐ (!IsFromData), ta có thể tính thoải mái.
                    if (!IsFromData)
                    {
                        double sumCoarse = 0;
                        double H_coarse = m * H;
                        double xStartRem = A + (numMainSegments * Order) * H;

                        // Tính I_thô dùng cùng công thức bậc 'remainder' trên lưới thưa
                        for (int j = 0; j <= remainder; j++)
                        {
                            double yVal = EvaluateFunction(compiledFunc, xStartRem + j * H_coarse);
                            sumCoarse += remWeights[j] * yVal;
                        }
                        double I_coarse = sumCoarse * H_coarse;

                        double denom = Math.Pow(m, pRem) - 1;
                        errorRem = Math.Abs(I_fine - I_coarse) / denom;
                    }
                    else
                    {
                        // Với dữ liệu rời rạc, phần dư thường quá ngắn để lập lưới thô cùng bậc.
                        // Ta chấp nhận errorRem = 0 hoặc cảnh báo.
                        calculationSteps.AppendLine("(*) Phần dư dữ liệu rời rạc không đủ điểm để tính sai số Runge.");
                    }
                }
            }

            // --- TỔNG HỢP ---
            EstimatedError = errorMain + errorRem;

            calculationSteps.AppendLine();
            calculationSteps.AppendLine("═══ TỔNG SAI SỐ LƯỚI PHỦ (RUNGE) ═══");
            if (numMainSegments > 0) calculationSteps.AppendLine($"Sai số phần chính (Bậc {Order}): {errorMain:E5}");
            if (remainder > 0 && !IsFromData) calculationSteps.AppendLine($"Sai số phần dư (Bậc {remainder}): {errorRem:E5}");
            calculationSteps.AppendLine($"Tổng sai số ước lượng: {EstimatedError}");
        }

        private void CalculateOptimalN_ByRunge()
        {
            // Tìm N sao cho sai số Runge < Epsilon
            // Chỉ áp dụng cho trường hợp đồng nhất (chia hết cho Order) để đơn giản hóa việc tìm kiếm
            int currentN = Order;
            while (currentN < 100000)
            {
                var calc = new NewtonCotesIntegration(FunctionExpr.ToString(), A, B, currentN, Order);
                var calc2 = new NewtonCotesIntegration(FunctionExpr.ToString(), A, B, currentN * 2, Order);

                // Công thức Runge: |I_2n - I_n| / (2^p - 1)
                // p phụ thuộc bậc n chẵn/lẻ
                int p = (Order % 2 == 0) ? Order + 2 : Order + 1;
                double error = Math.Abs(calc2.Result - calc.Result) / (Math.Pow(2, p) - 1);

                if (error < Epsilon)
                {
                    TotalSteps = currentN * 2; // Chọn mức lưới mịn hơn
                    return;
                }
                currentN *= 2;
            }
            TotalSteps = currentN;
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