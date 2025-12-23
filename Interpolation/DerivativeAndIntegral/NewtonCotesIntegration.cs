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
        public NewtonCotesIntegration(double[] x, double[] y, double a, double b, double epsilon, int order)
        {
            XData = x;
            YData = y;
            A = a;
            B = b;
            Epsilon = epsilon;
            Order = order;
            IsFromData = true;

            Solve();
            EstimateErrorRunge(); // Ước lượng sai số bằng Runge
        }

        // --- CONSTRUCTOR 2: Hàm F(x) với Sai số cho trước (Tính N) ---
        // Lưu ý: Việc tính N tối ưu cho Newton-Cotes tổng quát rất phức tạp vì phụ thuộc đạo hàm bậc cao
        // Ở đây ta sẽ dùng công thức xấp xỉ hoặc yêu cầu người dùng nhập N nếu quá phức tạp.
        // Tuy nhiên để đồng bộ, ta sẽ thử ước lượng sơ bộ.
        public NewtonCotesIntegration(string functionStr, double a, double b, double epsilon, int order)
        {
            try { FunctionExpr = functionStr; }
            catch (Exception ex) { throw new ArgumentException($"Lỗi phân tích hàm: {ex.Message}"); }

            A = a;
            B = b;
            Epsilon = epsilon;
            Order = order;
            IsFromData = false;

            // Vì tính đạo hàm bậc cao (cấp n) tự động rất tốn kém và khó chính xác
            // Ta sẽ sử dụng một N mặc định đủ lớn (ví dụ 10 * order) hoặc 
            // dùng thuật toán lặp tăng dần N cho đến khi sai số Runge < Epsilon.
            // Ở đây tôi chọn phương án: Lặp tìm N thỏa mãn Runge.

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
            // Newton-Cotes bậc cao tính đạo hàm lý thuyết rất khó, ta dùng Runge để đánh giá
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
            // Kiểm tra dữ liệu
            if (XData.Length < Order + 1)
                throw new ArgumentException($"Cần ít nhất {Order + 1} điểm dữ liệu cho Newton-Cotes bậc {Order}.");

            int indexA = Array.IndexOf(XData, A);
            int indexB = Array.IndexOf(XData, B);

            if (indexA == -1 || indexB == -1 || indexA >= indexB)
                throw new ArgumentException("Cận a, b không hợp lệ trong dữ liệu.");

            TotalSteps = indexB - indexA;

            if (TotalSteps % Order != 0)
            {
                // Nếu không chia hết, thông báo cảnh báo hoặc lỗi
                // Trong thực tế có thể kết hợp các phương pháp khác, nhưng ở đây ta báo lỗi strict.
                throw new ArgumentException($"Số khoảng chia trong dữ liệu ({TotalSteps}) không chia hết cho bậc {Order}.\nKhông thể áp dụng công thức tổng hợp (Composite).");
            }

            H = (B - A) / TotalSteps;

            calculationSteps.AppendLine($"Dữ liệu rời rạc: [{A}, {B}]");
            calculationSteps.AppendLine($"Tổng số khoảng chia (N): {TotalSteps}");
            calculationSteps.AppendLine($"Bước nhảy h: {H}");
            calculationSteps.AppendLine($"Công thức Newton-Cotes bậc {Order} yêu cầu chia thành {TotalSteps / Order} đoạn lớn.");

            // 1. Lấy hệ số Cotes
            double[] weights = NewtonCotesHelper.GetWeights(Order);
            calculationSteps.Append("Hệ số Cotes chuẩn hóa (trên đoạn [0,n]): [ ");
            foreach (var w in weights) calculationSteps.Append($"{w:F4} ");
            calculationSteps.AppendLine("]");

            // 2. Tính toán
            double sum = 0;

            // Duyệt từng "đoạn lớn" (Mỗi đoạn lớn gồm 'Order' khoảng nhỏ)
            int numSegments = TotalSteps / Order;

            for (int seg = 0; seg < numSegments; seg++)
            {
                int startIndex = indexA + (seg * Order);
                double segmentSum = 0;

                for (int j = 0; j <= Order; j++)
                {
                    double yVal = YData[startIndex + j];
                    segmentSum += weights[j] * yVal;
                }

                sum += segmentSum;

                // Hiển thị chi tiết cho vài đoạn đầu
                if (seg < 3)
                    calculationSteps.AppendLine($"  Đoạn {seg + 1} (x[{startIndex}]..x[{startIndex + Order}]): Tổng trọng số = {segmentSum:F6}");
            }
            if (numSegments > 3) calculationSteps.AppendLine("  ...");

            // Công thức chuẩn: I ≈ h * Sum(Weights * Y) / Divisor? 
            // KHÔNG. Hàm GetWeights của ta đã trả về tích phân trên [0, n].
            // Tích phân thực tế trên [a, b] = (H/1) * Sum(Weights_Normalized * Y) ???
            // Hãy check lại Simpson (n=2): Weights = {1/3, 4/3, 1/3}.
            // CT Simpson: h/3 * (y0 + 4y1 + y2). 
            // -> h * (1/3*y0 + 4/3*y1 + 1/3*y2). -> ĐÚNG.
            // Vậy ta chỉ cần nhân Sum với H.

            Result = sum * H;

            calculationSteps.AppendLine();
            calculationSteps.AppendLine($"Tổng trọng số: {sum:F8}");
            calculationSteps.AppendLine($"I ≈ h × Tổng = {H} × {sum:F8} = {Result}");
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
            // Tìm N sao cho sai số Runge < Epsilon
            // Bắt đầu với N = Order (1 đoạn)
            int currentN = Order;

            // Loop tìm N (đơn giản hóa để tránh treo máy)
            // Chiến thuật: Nhân đôi số đoạn lớn (currentN *= 2) sau mỗi lần lặp

            calculationSteps.AppendLine("Tự động tìm N tối ưu dựa trên sai số Runge...");

            while (currentN < 10000) // Giới hạn an toàn
            {
                // Tính tích phân với currentN
                var calc = new NewtonCotesIntegration(FunctionExpr.ToString(), A, B, currentN, Order);
                double valN = calc.Result;

                // Tính tích phân với 2*currentN (để so sánh)
                var calc2N = new NewtonCotesIntegration(FunctionExpr.ToString(), A, B, currentN * 2, Order);
                double val2N = calc2N.Result;

                // Runge Error
                // Bậc chính xác của Newton-Cotes bậc n là:
                // - n nếu n lẻ
                // - n+1 nếu n chẵn (Ví dụ Simpson n=2 chính xác bậc 3)
                int accuracyOrder = (Order % 2 == 0) ? Order + 1 : Order; // Hoặc dùng quy tắc chung
                                                                          // Thực tế CT Runge mẫu số là 2^p - 1. Ta lấy p = Order + 1 cho an toàn.

                double error = Math.Abs(val2N - valN) / (Math.Pow(2, Order) - 1); // Ước lượng

                if (error < Epsilon)
                {
                    TotalSteps = currentN * 2; // Chọn mức lưới mịn hơn để đảm bảo
                    calculationSteps.AppendLine($"-> Chọn N = {TotalSteps} (Sai số ước lượng: {error} < {Epsilon})");
                    return;
                }

                currentN *= 2;
            }

            // Fallback nếu không hội tụ
            TotalSteps = currentN;
            calculationSteps.AppendLine($"-> Đạt giới hạn lặp, chọn N = {TotalSteps}");
        }

        private void EstimateErrorRunge()
        {
            // 1. Xác định bậc chính xác p
            // Nếu n chẵn (VD Simpson n=2), chính xác bậc n+2. Nếu n lẻ, chính xác bậc n+1
            int p = (Order % 2 == 0) ? Order + 2 : Order + 1;

            // 2. Tìm tỷ lệ m (scale factor) phù hợp
            int m = -1;

            // Thử m từ 2 trở lên. Giới hạn m sao cho n*m <= TotalSteps
            for (int scale = 2; scale * Order <= TotalSteps; scale++)
            {
                if (TotalSteps % (Order * scale) == 0)
                {
                    m = scale;
                    break; // Tìm thấy m nhỏ nhất thỏa mãn
                }
            }

            if (m == -1)
            {
                EstimatedError = 0;
                calculationSteps.AppendLine($"\n(!) Không tìm được lưới thô phù hợp để tính sai số (Dữ liệu không chia hết cho n*m).");
                return;
            }

            // --- SỬA LỖI TẠI ĐÂY: BIÊN DỊCH HÀM SỐ TRƯỚC VÒNG LẶP ---
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
            // --------------------------------------------------------

            // 3. Tính I_tho
            double[] weights = NewtonCotesHelper.GetWeights(Order);
            double sumCoarse = 0;
            double H_coarse = m * H; // Bước nhảy lưới thô tăng m lần
            int numSegmentsCoarse = TotalSteps / (Order * m);

            for (int seg = 0; seg < numSegmentsCoarse; seg++)
            {
                int startBaseIdx = seg * (Order * m);

                for (int j = 0; j <= Order; j++)
                {
                    // Các điểm cách nhau m đơn vị index
                    int idx = startBaseIdx + (j * m);
                    double yVal = 0;

                    if (IsFromData)
                    {
                        yVal = YData[idx];
                    }
                    else
                    {
                        // Nếu là hàm số, ta tính x dựa trên index và H ban đầu
                        double xVal = A + idx * H;

                        // GỌI HÀM VỚI ĐỦ 2 THAM SỐ
                        yVal = EvaluateFunction(compiledFunc, xVal);
                    }

                    sumCoarse += weights[j] * yVal;
                }
            }

            double I_coarse = sumCoarse * H_coarse;

            // 4. Tính sai số Runge tổng quát
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