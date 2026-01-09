using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Interpolation.Methods
{
    public class Spline
    {
        public StringBuilder ResultLog { get; private set; }
        private double[] _x;
        private double[] _y;
        private int _precision;
        private int _n;

        public Spline(double[] x, double[] y, int precision)
        {
            _x = x;
            _y = y;
            _n = x.Length; // Số điểm
            _precision = precision;
            ResultLog = new StringBuilder();
        }

        public void SolveLinear()
        {
            ResultLog.Clear();
            double[] a, b, h;
            CalculateLinearInternal(out a, out b, out h);
            FormatLinearOutput(a, b, h);
        }

        public void SolveQuadratic(double? startDeriv, double? endDeriv)
        {
            ResultLog.Clear();
            double[] a, b, c, h, m, v;
            CalculateQuadraticInternal(startDeriv, endDeriv, out a, out b, out c, out h, out m, out v);
            FormatQuadraticOutput(a, b, c, h, m, v);
        }

        public void SolveCubic(bool isClamped, double valStart, double valEnd)
        {
            ResultLog.Clear();
            double[] a, b, c, d, h, alpha;
            if (isClamped)
            {
                CalculateCubicClampedInternal(valStart, valEnd, out a, out b, out c, out d, out h, out alpha);
                FormatCubicOutput(a, b, c, d, h, alpha, true, valStart, valEnd);
            }
            else
            {
                CalculateCubicGeneralDeriv2(valStart, valEnd, out a, out b, out c, out d, out h, out alpha);
                FormatCubicOutput(a, b, c, d, h, alpha, false, valStart, valEnd);
            }
        }

        public void DisplayResults(RichTextBox rtb)
        {
            rtb.Clear();
            rtb.AppendText(ResultLog.ToString());
        }

        private void CalculateLinearInternal(out double[] a, out double[] b, out double[] h)
        {
            int numSplines = _n - 1;
            a = new double[numSplines];
            b = new double[numSplines];
            h = new double[numSplines];

            for (int i = 0; i < numSplines; i++)
            {
                h[i] = Math.Round(_x[i + 1] - _x[i], _precision);
                a[i] = Math.Round((_y[i + 1] - _y[i]) / h[i], _precision);
                b[i] = Math.Round((_y[i] * _x[i + 1] - _y[i + 1] * _x[i]) / h[i], _precision);
            }
        }

        private void CalculateQuadraticInternal(double? s_prime_start, double? s_prime_end,
            out double[] a, out double[] b, out double[] c, out double[] h,
            out double[] m, out double[] v)
        {
            int numSplines = _n - 1;
            a = new double[numSplines];
            b = new double[numSplines];
            c = new double[numSplines];
            h = new double[numSplines];
            v = new double[numSplines];
            m = new double[_n];

            for (int i = 0; i < numSplines; i++)
            {
                h[i] = Math.Round(_x[i + 1] - _x[i], _precision);
                v[i] = Math.Round(2 * (_y[i + 1] - _y[i]) / h[i], _precision);
            }

            if (s_prime_start.HasValue)
            {
                m[0] = s_prime_start.Value;
                for (int i = 0; i < numSplines; i++)
                    m[i + 1] = Math.Round(v[i] - m[i], _precision);
            }
            else if (s_prime_end.HasValue)
            {
                m[_n - 1] = s_prime_end.Value;
                for (int i = numSplines - 1; i >= 0; i--)
                    m[i] = Math.Round(v[i] - m[i + 1], _precision);
            }
            else
            {
                m[0] = 0;
                for (int i = 0; i < numSplines; i++)
                    m[i + 1] = Math.Round(v[i] - m[i], _precision);
            }

            for (int i = 0; i < numSplines; i++)
            {
                a[i] = Math.Round((m[i + 1] - m[i]) / (2 * h[i]), _precision);
                b[i] = Math.Round((m[i] * _x[i + 1] - m[i + 1] * _x[i]) / h[i], _precision);
                double term = (-m[i] * Math.Pow(_x[i + 1], 2) + m[i + 1] * Math.Pow(_x[i], 2)) / (2 * h[i]);
                c[i] = Math.Round(term + _y[i] + (m[i] * h[i]) / 2, _precision);
            }
        }

        private void CalculateCubicGeneralDeriv2(
            double m_start, double m_end,
            out double[] a, out double[] b, out double[] c, out double[] d,
            out double[] h, out double[] alpha)
        {
            int numSplines = _n - 1;
            int numEq = _n - 2;

            a = new double[numSplines];
            b = new double[numSplines];
            c = new double[numSplines];
            d = new double[numSplines];
            h = new double[numSplines];
            alpha = new double[_n];

            for (int i = 0; i < numSplines; i++)
                h[i] = Math.Round(_x[i + 1] - _x[i], _precision);

            alpha[0] = m_start;
            alpha[_n - 1] = m_end;

            if (_n > 2)
            {
                double[] sub = new double[numEq];
                double[] main = new double[numEq];
                double[] super = new double[numEq];
                double[] rhs = new double[numEq];

                for (int i = 0; i < numEq; i++)
                {
                    int realIdx = i + 1;
                    sub[i] = Math.Round(h[realIdx - 1] / 6.0, _precision);
                    main[i] = Math.Round((h[realIdx - 1] + h[realIdx]) / 3.0, _precision);
                    super[i] = Math.Round(h[realIdx] / 6.0, _precision);

                    double term1 = (_y[realIdx + 1] - _y[realIdx]) / h[realIdx];
                    double term2 = (_y[realIdx] - _y[realIdx - 1]) / h[realIdx - 1];
                    rhs[i] = Math.Round(term1 - term2, _precision);
                }

                rhs[0] -= Math.Round((h[0] / 6.0) * alpha[0], _precision);
                sub[0] = 0;

                rhs[numEq - 1] -= Math.Round((h[numSplines - 1] / 6.0) * alpha[_n - 1], _precision);
                super[numEq - 1] = 0;

                double[] solution = SolveTridiagonalSystem(sub, main, super, rhs);
                for (int i = 0; i < numEq; i++) alpha[i + 1] = solution[i];
            }

            CalculateCoeffsCubic(numSplines, h, alpha, out a, out b, out c, out d);
        }

        private void CalculateCubicClampedInternal(
            double startDeriv, double endDeriv,
            out double[] a, out double[] b, out double[] c, out double[] d,
            out double[] h, out double[] alpha)
        {
            int numSplines = _n - 1;
            int numEq = _n;

            a = new double[numSplines];
            b = new double[numSplines];
            c = new double[numSplines];
            d = new double[numSplines];
            h = new double[numSplines];
            alpha = new double[_n];

            for (int i = 0; i < numSplines; i++)
                h[i] = Math.Round(_x[i + 1] - _x[i], _precision);

            double[] sub = new double[_n];
            double[] main = new double[_n];
            double[] super = new double[_n];
            double[] rhs = new double[_n];

            main[0] = 2 * h[0];
            super[0] = h[0];
            rhs[0] = 6 * ((_y[1] - _y[0]) / h[0] - startDeriv);

            for (int i = 1; i < _n - 1; i++)
            {
                sub[i] = h[i - 1];
                main[i] = 2 * (h[i - 1] + h[i]);
                super[i] = h[i];
                rhs[i] = 6 * ((_y[i + 1] - _y[i]) / h[i] - (_y[i] - _y[i - 1]) / h[i - 1]);
            }

            sub[_n - 1] = h[_n - 2];
            main[_n - 1] = 2 * h[_n - 2];
            rhs[_n - 1] = 6 * (endDeriv - (_y[_n - 1] - _y[_n - 2]) / h[_n - 2]);

            double[] solution = SolveTridiagonalSystem(sub, main, super, rhs);
            for (int i = 0; i < _n; i++) alpha[i] = solution[i];

            CalculateCoeffsCubic(numSplines, h, alpha, out a, out b, out c, out d);
        }

        private void CalculateCoeffsCubic(int numSplines, double[] h, double[] M,
            out double[] a, out double[] b, out double[] c, out double[] d)
        {
            a = new double[numSplines];
            b = new double[numSplines];
            c = new double[numSplines];
            d = new double[numSplines];

            for (int i = 0; i < numSplines; i++)
            {
                double hi = h[i];
                double mi = M[i];
                double mi1 = M[i + 1];

                a[i] = Math.Round((mi1 - mi) / (6 * hi), _precision);
                b[i] = Math.Round((mi * _x[i + 1] - mi1 * _x[i]) / (2 * hi), _precision);

                double term1 = (-mi * Math.Pow(_x[i + 1], 2) + mi1 * Math.Pow(_x[i], 2)) / (2 * hi);
                double term2 = (_y[i + 1] - _y[i]) / hi;
                double term3 = -(mi1 - mi) * hi / 6.0;
                c[i] = Math.Round(term1 + term2 + term3, _precision);

                double term4 = (mi * Math.Pow(_x[i + 1], 3) - mi1 * Math.Pow(_x[i], 3)) / (6 * hi);
                double term5 = (_y[i] * _x[i + 1] - _y[i + 1] * _x[i]) / hi;
                double term6 = (mi1 * _x[i] - mi * _x[i + 1]) * hi / 6.0;
                d[i] = Math.Round(term4 + term5 + term6, _precision);
            }
        }

        private double[] SolveTridiagonalSystem(double[] a, double[] b, double[] c, double[] d)
        {
            int n = d.Length;
            double[] c_prime = new double[n];
            double[] d_prime = new double[n];
            double[] x = new double[n];

            c_prime[0] = c[0] / b[0];
            d_prime[0] = d[0] / b[0];

            for (int i = 1; i < n; i++)
            {
                double temp = b[i] - a[i] * c_prime[i - 1];
                if (i < n - 1) c_prime[i] = c[i] / temp;
                d_prime[i] = (d[i] - a[i] * d_prime[i - 1]) / temp;
            }

            x[n - 1] = d_prime[n - 1];
            for (int i = n - 2; i >= 0; i--)
            {
                x[i] = d_prime[i] - c_prime[i] * x[i + 1];
            }
            return x;
        }

        private void FormatLinearOutput(double[] a, double[] b, double[] h)
        {
            ResultLog.AppendLine("KẾT QUẢ SPLINE TUYẾN TÍNH (CẤP 1)");
            ResultLog.AppendLine("--------------------------------------------------");
            for (int i = 0; i < a.Length; i++)
            {
                ResultLog.AppendLine($"Đoạn {i + 1} [{_x[i]}, {_x[i + 1]}]:");
                string sign = b[i] >= 0 ? "+" : "";
                ResultLog.AppendLine($"  S(x) = {a[i]}x {sign} {b[i]}");
            }
        }

        private void FormatQuadraticOutput(double[] a, double[] b, double[] c, double[] h, double[] m, double[] v)
        {
            ResultLog.AppendLine("=== KẾT QUẢ SPLINE BẬC 2 (CẤP 2) ===");
            ResultLog.AppendLine($"Số điểm n = {_n}");
            ResultLog.AppendLine();

            // 1. Hiển thị h_i và v_i
            ResultLog.AppendLine("1. BƯỚC LƯỚI h_i VÀ HỆ SỐ TRUNG GIAN v_i:");
            ResultLog.AppendLine("   Công thức: v_i = 2 * (y_{i+1} - y_i) / h_i");
            for (int i = 0; i < h.Length; i++)
            {
                ResultLog.AppendLine($"   Đoạn {i + 1}: h_{i} = {h[i]}, v_{i} = {v[i]:F6}");
            }
            ResultLog.AppendLine();

            // 2. Tính toán đạo hàm m_i
            ResultLog.AppendLine("2. TÍNH ĐẠO HÀM CẤP 1 TẠI CÁC NÚT (m_i):");
            ResultLog.AppendLine("   Sử dụng công thức truy hồi: m_{i+1} = v_i - m_i");

            ResultLog.Append("   Vector m = [ ");
            foreach (var val in m) ResultLog.Append($"{val:F6}  ");
            ResultLog.AppendLine("]^T");
            ResultLog.AppendLine();

            // 3. Công thức hệ số
            ResultLog.AppendLine("3. CÔNG THỨC TÍNH HỆ SỐ ĐA THỨC S(x) = ax^2 + bx + c:");
            ResultLog.AppendLine("   a_i = (m_{i+1} - m_i) / (2h_i)");
            ResultLog.AppendLine("   b_i = (m_i*x_{i+1} - m_{i+1}*x_i) / h_i");
            ResultLog.AppendLine("   c_i = [ -m_i*x_{i+1}^2 + m_{i+1}*x_i^2 ] / (2h_i) + y_i + m_i*h_i/2");
            ResultLog.AppendLine();

            // 4. Kết quả
            ResultLog.AppendLine("5. KẾT QUẢ ĐA THỨC TRÊN CÁC ĐOẠN:");
            ResultLog.AppendLine("--------------------------------------------------");
            for (int i = 0; i < a.Length; i++)
            {
                ResultLog.AppendLine($"Đoạn {i + 1} [{_x[i]}, {_x[i + 1]}]:");
                string sb = b[i] >= 0 ? "+" : "";
                string sc = c[i] >= 0 ? "+" : "";
                ResultLog.AppendLine($"  S_{i + 1}(x) = {a[i]}x^2 {sb} {b[i]}x {sc} {c[i]}");
            }
        }
        private void FormatCubicOutput(double[] a, double[] b, double[] c, double[] d, double[] h, double[] alpha, bool isClamped, double valStart, double valEnd)
        {
            string title = isClamped ? "TRƯỜNG HỢP S' Biên" : "TRƯỜNG HỢP S'' biên";
            ResultLog.AppendLine($"=== KẾT QUẢ {title} ===");
            ResultLog.AppendLine($"Số điểm n = {_n}");
            ResultLog.AppendLine();

            // 1. Hiển thị h_i
            ResultLog.AppendLine("1. BƯỚC LƯỚI h_i:");
            for (int i = 0; i < h.Length; i++)
            {
                ResultLog.Append($"   h_{i + 1} = {h[i]}");
                if (i < h.Length - 1) ResultLog.Append(" ; ");
            }
            ResultLog.AppendLine("\n");

            // 2. Điều kiện biên
            ResultLog.AppendLine("2. ĐIỀU KIỆN BIÊN:");
            if (isClamped)
            {
                ResultLog.AppendLine($"   S'(x_0) = {valStart}");
                ResultLog.AppendLine($"   S'(x_{_n - 1}) = {valEnd}");
            }
            else
            {
                ResultLog.AppendLine($"   M_0 = S''(x_0) = {valStart}");
                ResultLog.AppendLine($"   M_{_n - 1} = S''(x_{_n - 1}) = {valEnd}");
            }
            ResultLog.AppendLine();

            // 3. Hệ phương trình
            ResultLog.AppendLine("3. HỆ PHƯƠNG TRÌNH XÁC ĐỊNH M_i:");

            double[,] matrix;
            double[] rhsVec;
            int size = _n; // Kích thước hiển thị từ 0 đến n-1

            if (!isClamped) // Natural / General
            {
                ResultLog.AppendLine($"   Phương trình tổng quát tại nút i (1 <= i <= {_n - 2}):");
                ResultLog.AppendLine("   (h_{i-1}/6)*M_{i-1} + (h_{i-1} + h_i)/3 * M_i + (h_i/6)*M_{i+1} = (y_{i+1}-y_i)/h_i - (y_i-y_{i-1})/h_{i-1}");
                ResultLog.AppendLine();

                matrix = new double[size, size];
                rhsVec = new double[size];

                matrix[0, 0] = 1.0;
                rhsVec[0] = valStart;

                for (int i = 1; i < size - 1; i++)
                {
                    matrix[i, i - 1] = h[i - 1] / 6.0;           // h_{i-1}/6
                    matrix[i, i] = (h[i - 1] + h[i]) / 3.0;  // (h_{i-1}+h_i)/3
                    matrix[i, i + 1] = h[i] / 6.0;               // h_i/6

                    double term1 = (_y[i + 1] - _y[i]) / h[i];
                    double term2 = (_y[i] - _y[i - 1]) / h[i - 1];
                    rhsVec[i] = term1 - term2;
                }

                matrix[size - 1, size - 1] = 1.0;
                rhsVec[size - 1] = valEnd;
            }
            else // Clamped
            {
                ResultLog.AppendLine("   PT Biên đầu: 2*h0*M0 + h0*M1 = 6*((y1-y0)/h0 - S'(x0))");
                ResultLog.AppendLine();

                ResultLog.AppendLine($"   PT Biên cuối: h_{_n - 2}*M_{_n - 2} + 2*h_{_n - 2}*M_{_n - 1} = 6*(S'(x_{_n - 1}) - (y_{_n - 1}-y_{_n - 2})/h_{_n - 2})");
                ResultLog.AppendLine();

                ResultLog.AppendLine($"   Phương trình tổng quát tại nút i (1 <= i <= {_n - 2}):");
                ResultLog.AppendLine("   (h_{i-1}/6)*M_{i-1} + (h_{i-1} + h_i)/3 * M_i + (h_i/6)*M_{i+1} = (y_{i+1}-y_i)/h_i - (y_i-y_{i-1})/h_{i-1}");
                ResultLog.AppendLine();

                matrix = new double[size, size];
                rhsVec = new double[size];

                matrix[0, 0] = 2 * h[0];
                matrix[0, 1] = h[0];
                rhsVec[0] = 6 * ((_y[1] - _y[0]) / h[0] - valStart);

                for (int i = 1; i < size - 1; i++)
                {
                    matrix[i, i - 1] = h[i - 1];
                    matrix[i, i] = 2 * (h[i - 1] + h[i]);
                    matrix[i, i + 1] = h[i];
                    rhsVec[i] = 6 * ((_y[i + 1] - _y[i]) / h[i] - (_y[i] - _y[i - 1]) / h[i - 1]);
                }

                matrix[size - 1, size - 2] = h[size - 2];
                matrix[size - 1, size - 1] = 2 * h[size - 2];
                rhsVec[size - 1] = 6 * (valEnd - (_y[size - 1] - _y[size - 2]) / h[size - 2]);
            }

            ResultLog.AppendLine();
            ResultLog.AppendLine("   Ma trận hệ số:");
            for (int r = 0; r < size; r++)
            {
                ResultLog.Append("   | ");
                for (int m = 0; m < size; m++)
                {
                    ResultLog.Append($"{matrix[r, m],8:F4} ");
                }

                ResultLog.Append($" |  | M_{r} |");

                if (r == size / 2) ResultLog.Append(" = "); else ResultLog.Append("   ");
                ResultLog.AppendLine($"| {rhsVec[r],8:F4} |");
            }
            ResultLog.AppendLine();

            // 4. Kết quả M
            ResultLog.AppendLine("4. Nghiệm Đạo hàm cấp 2:");
            ResultLog.Append("   Vector M = [ ");
            for (int i = 0; i < alpha.Length; i++) ResultLog.Append($"{alpha[i]:F6}  ");
            ResultLog.AppendLine("]^T");
            ResultLog.AppendLine();

            // 5. Chi tiết tính toán
            ResultLog.AppendLine("5. CÔNG THỨC TÍNH HỆ SỐ ĐA THỨC:");
            ResultLog.AppendLine("   Công thức tính hệ số cùa phương trình dạng S(x) = ax^3 + bx^2 + cx + d:");
            ResultLog.AppendLine("   a_i = (M_{i+1} - M_i) / (6*h_i)");
            ResultLog.AppendLine("   b_i = (M_i*x_{i+1} - M_{i+1}*x_i) / (2*h_i)");
            ResultLog.AppendLine("   c_i = [ -M_i*x_{i+1}^2 + M_{i+1}*x_i^2 ] / (2h_i)");
            ResultLog.AppendLine("         + (y_{i+1} - y_i) / h_i");
            ResultLog.AppendLine("         - h_i * (M_{i+1} - M_i) / 6");
            ResultLog.AppendLine("   d_i = [ M_i*x_{i+1}^3 - M_{i+1}*x_i^3 ] / (6h_i)");
            ResultLog.AppendLine("         + (y_i*x_{i+1} - y_{i+1}*x_i) / h_i");
            ResultLog.AppendLine("         + h_i * (M_{i+1}*x_i - M_i*x_{i+1}) / 6");

            ResultLog.AppendLine();

            // 6. Phương trình các đoạn
            ResultLog.AppendLine("6. CÁC ĐA THỨC NỘI SUY TRÊN TỪNG ĐOẠN:");
            ResultLog.AppendLine("--------------------------------------------------");
            for (int i = 0; i < a.Length; i++)
            {
                ResultLog.AppendLine($"Đoạn {i + 1} [{_x[i]}, {_x[i + 1]}]:");
                string sb = b[i] >= 0 ? "+" : "";
                string sc = c[i] >= 0 ? "+" : "";
                string sd = d[i] >= 0 ? "+" : "";
                ResultLog.AppendLine($"  S_{i + 1}(x) = {a[i]}x^3 {sb} {b[i]}x^2 {sc} {c[i]}x {sd} {d[i]}");
            }
        }
    }
}