using System;
using System.Text;
using System.Windows.Forms;

namespace Interpolation.Methods
{
    public enum DataPosition { Start, Center, End }

    public class Derivative
    {
        public double[] XData { get; set; }
        public double[] YData { get; set; }
        public double XTarget { get; set; }
        public int P { get; set; } // Bậc P
        public int K { get; set; }
        public double H { get; set; }

        public double Result { get; set; }
        public DataPosition Position { get; set; }

        private StringBuilder coefficientStepBuilder;
        private StringBuilder substitutionStepBuilder;
        private int startIndex;
        private int endIndex;
        private double[] coefficients;

        public Derivative(double[] x, double[] y, double xTarget, int p, int precision)
        {
            XData = x;
            YData = y;
            XTarget = xTarget;
            P = p;
            coefficientStepBuilder = new StringBuilder();
            substitutionStepBuilder = new StringBuilder();

            Solve(precision);
        }

        private void Solve(int precision)
        {
            int n = XData.Length;

            if (n < 2) throw new ArgumentException("Cần ít nhất 2 điểm dữ liệu.");
            H = XData[1] - XData[0];

            K = Array.IndexOf(XData, XTarget);
            if (K == -1) throw new ArgumentException($"Không tìm thấy giá trị x = {XTarget} trong bảng dữ liệu.");

            if (P < 1) throw new ArgumentException("Bậc p phải >= 1.");
            if (n < P + 1) throw new ArgumentException($"Không đủ dữ liệu cho bậc p={P}. Cần {P + 1} điểm.");

            startIndex = K - (P / 2);

            if (startIndex < 0) startIndex = 0;
            if (startIndex + P >= n) startIndex = n - 1 - P;

            endIndex = startIndex + P;

            if (K == startIndex) Position = DataPosition.Start;
            else if (K == endIndex) Position = DataPosition.End;
            else Position = DataPosition.Center;

            int relativeTargetIndex = K - startIndex;
            coefficients = GetLagrangeCoefficients(P, relativeTargetIndex);

            BuildSubstitutionStep();

            double sum = 0.0;
            for (int i = 0; i <= P; i++)
            {
                int dataIndex = startIndex + i;
                sum += coefficients[i] * YData[dataIndex];
            }

            Result = sum / H;
            Result = Math.Round(Result, precision);
        }

        private double[] GetLagrangeCoefficients(int p, int targetPos)
        {
            double[] coeffs = new double[p + 1];
            double[] nodes = new double[p + 1];
            for (int i = 0; i <= p; i++) nodes[i] = i;

            double targetNode = nodes[targetPos];

            coefficientStepBuilder.Clear();
            coefficientStepBuilder.AppendLine("═══ TÍNH HỆ SỐ LAGRANGE ═══");
            coefficientStepBuilder.AppendLine();
            coefficientStepBuilder.AppendLine($"Số điểm sử dụng: {p + 1} điểm");
            coefficientStepBuilder.AppendLine($"Vị trí tính đạo hàm: node[{targetPos}]");
            coefficientStepBuilder.AppendLine();

            for (int i = 0; i <= p; i++)
            {
                coefficientStepBuilder.AppendLine($"Hệ số C[{i}]:");

                coefficientStepBuilder.Append("  Mẫu = ");
                double denominator = 1.0;
                StringBuilder denomBuilder = new StringBuilder();
                for (int j = 0; j <= p; j++)
                {
                    if (i != j)
                    {
                        denominator *= (nodes[i] - nodes[j]);
                        if (denomBuilder.Length > 0) denomBuilder.Append(" × ");
                        denomBuilder.Append($"({nodes[i]} - {nodes[j]})");
                    }
                }
                coefficientStepBuilder.AppendLine($"{denomBuilder} = {denominator}");

                coefficientStepBuilder.Append("  Tử = ");
                double numeratorDeriv = 0.0;
                StringBuilder numBuilder = new StringBuilder();

                for (int m = 0; m <= p; m++)
                {
                    if (m == i) continue;

                    double term = 1.0;
                    StringBuilder termBuilder = new StringBuilder();

                    for (int j = 0; j <= p; j++)
                    {
                        if (j != i && j != m)
                        {
                            term *= (targetNode - nodes[j]);
                            if (termBuilder.Length > 0) termBuilder.Append(" × ");
                            termBuilder.Append($"({targetNode} - {nodes[j]})");
                        }
                    }

                    numeratorDeriv += term;

                    if (numBuilder.Length > 0) numBuilder.Append(" + ");
                    if (termBuilder.Length > 0)
                        numBuilder.Append($"[{termBuilder}]");
                    else
                        numBuilder.Append("1");
                }

                coefficientStepBuilder.AppendLine($"{numBuilder} = {numeratorDeriv}");

                coeffs[i] = numeratorDeriv / denominator;
                coefficientStepBuilder.AppendLine($"  => C[{i}] = {numeratorDeriv} / {denominator} = {coeffs[i]:F6}");
                coefficientStepBuilder.AppendLine();
            }

            return coeffs;
        }

        private void BuildSubstitutionStep()
        {
            substitutionStepBuilder.Clear();
            substitutionStepBuilder.AppendLine("═══ CÔNG THỨC VỚI HỆ SỐ ═══");
            substitutionStepBuilder.AppendLine();
            substitutionStepBuilder.Append("f'(x) ≈ (1/h) × [ ");

            for (int i = 0; i <= P; i++)
            {
                string sign = (coefficients[i] >= 0 && i > 0) ? " + " : " ";
                substitutionStepBuilder.Append($"{sign}{coefficients[i]:F6} × y[{startIndex + i}]");
            }
            substitutionStepBuilder.AppendLine(" ]");
            substitutionStepBuilder.AppendLine();

            substitutionStepBuilder.AppendLine("═══ THAY SỐ VÀO CÔNG THỨC ═══");
            substitutionStepBuilder.AppendLine();
            substitutionStepBuilder.Append($"f'({XTarget}) ≈ (1/{H}) × [ ");

            double sum = 0.0;
            for (int i = 0; i <= P; i++)
            {
                int dataIndex = startIndex + i;
                double yValue = YData[dataIndex];
                double c = coefficients[i];
                sum += c * yValue;

                string sign = (c >= 0 && i > 0) ? " + " : " ";
                substitutionStepBuilder.Append($"{sign}({c:F6} × {yValue})");
            }
            substitutionStepBuilder.AppendLine(" ]");
            substitutionStepBuilder.AppendLine();
            substitutionStepBuilder.AppendLine($"         = (1/{H}) × [ {sum:F8} ]");
            substitutionStepBuilder.AppendLine($"         = {sum / H:F8}");
        }

        public void DisplayResults(RichTextBox rtb)
        {
            rtb.Clear();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("           ĐẠO HÀM SỬ DỤNG CÔNG THỨC p+1 ĐIỂM");
            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine();

            sb.AppendLine("1. THÔNG TIN:");
            sb.AppendLine("──────────────────────────────────────────────────");
            sb.AppendLine($" - Điểm tính:     x = {XTarget} (index k={K})");
            sb.AppendLine($" - Cấu hình:      p = {P} (Dùng {P + 1} điểm)");
            sb.AppendLine($" - Vị trí dữ liệu: {Position}");
            sb.AppendLine($" - Bước nhảy h:   {H}");
            sb.AppendLine();

            sb.AppendLine("2. BẢNG DỮ LIỆU SỬ DỤNG:");
            sb.AppendLine("──────────────────────────────────────────────────");
            for (int i = 0; i <= P; i++)
            {
                int dataIndex = startIndex + i;
                sb.AppendLine($" y[{dataIndex}] = f(x[{dataIndex}]) = f({XData[dataIndex]}) = {YData[dataIndex]}");
            }
            sb.AppendLine();

            sb.AppendLine("3. TÍNH HỆ SỐ:");
            sb.AppendLine("──────────────────────────────────────────────────");
            sb.AppendLine(coefficientStepBuilder.ToString());

            sb.AppendLine("4. ÁP DỤNG CÔNG THỨC:");
            sb.AppendLine("──────────────────────────────────────────────────");
            sb.AppendLine(substitutionStepBuilder.ToString());

            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine($"KẾT QUẢ: f'({XTarget}) ≈ {Result}");
            sb.AppendLine("═══════════════════════════════════════════════════════════════");

            rtb.Text = sb.ToString();
        }
    }
}