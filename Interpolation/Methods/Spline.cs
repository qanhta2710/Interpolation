namespace Interpolation.Methods
{
    internal class Spline
    {
        // Spline

        //    private void SolveLinearSpline(
        //double[] x, double[] y, int precision,
        //out double[] a, out double[] b, out double[] h)
        //    {
        //        int n = x.Length;

        //        a = new double[n - 1];
        //        b = new double[n - 1];
        //        h = new double[n - 1];

        //        for (int i = 0; i < n - 1; i++)
        //        {
        //            h[i] = Math.Round(x[i + 1] - x[i], precision);
        //            a[i] = Math.Round((y[i + 1] - y[i]) / h[i], precision);
        //            b[i] = Math.Round((y[i] * x[i + 1] - y[i + 1] * x[i]) / h[i], precision);
        //        }
        //    }
        //    private void SolveQuadraticSpline(
        //double[] x, double[] y, int precision,
        //double? s_prime_start, // Điều kiện S'(x_0)
        //double? s_prime_end,   // Điều kiện S'(x_{n-1})
        //out double[] a, out double[] b, out double[] c, out double[] h,
        //out double[] m, out double[] v)
        //    {
        //        int n = x.Length;
        //        int numSplines = n - 1;

        //        // Khởi tạo các mảng output
        //        a = new double[numSplines];
        //        b = new double[numSplines];
        //        c = new double[numSplines];
        //        h = new double[numSplines];

        //        // Khởi tạo các mảng trung gian - EXPORT ra ngoài
        //        v = new double[numSplines]; // Vế phải
        //        m = new double[n];          // Các giá trị đạo hàm m[i] = S'(x[i])

        //        // --- Bước 1: Tính h[i] và v[i] cho mỗi khoảng ---
        //        for (int i = 0; i < numSplines; i++)
        //        {
        //            h[i] = Math.Round(x[i + 1] - x[i], precision);
        //            if (h[i] == 0)
        //            {
        //                throw new ArgumentException($"Các mốc x bị trùng nhau tại chỉ số {i}.");
        //            }
        //            v[i] = Math.Round(2 * (y[i + 1] - y[i]) / h[i], precision);
        //        }

        //        // --- Bước 2: Giải tìm các đạo hàm m[i] dựa trên điều kiện biên ---

        //        if (s_prime_start.HasValue)
        //        {
        //            // TRƯỜNG HỢP 1: Người dùng cung cấp S'(x_0)
        //            // Giải truy hồi TIẾN
        //            m[0] = s_prime_start.Value;
        //            for (int i = 0; i < numSplines; i++)
        //            {
        //                m[i + 1] = Math.Round(v[i] - m[i], precision);
        //            }
        //        }
        //        else if (s_prime_end.HasValue)
        //        {
        //            // TRƯỜNG HỢP 2: Người dùng cung cấp S'(x_{n-1})
        //            // Giải truy hồi LÙI
        //            m[n - 1] = s_prime_end.Value;
        //            for (int i = numSplines - 1; i >= 0; i--)
        //            {
        //                m[i] = Math.Round(v[i] - m[i + 1], precision);
        //            }
        //        }
        //        else
        //        {
        //            // TRƯỜNG HỢP 3: Người dùng KHÔNG cung cấp điều kiện nào
        //            // Sử dụng điều kiện mặc định theo yêu cầu: m[0] = 0
        //            m[0] = 0;
        //            for (int i = 0; i < numSplines; i++)
        //            {
        //                m[i + 1] = Math.Round(v[i] - m[i], precision);
        //            }
        //        }

        //        // --- Bước 3: Tính các hệ số a, b, c từ m[i] ---
        //        for (int i = 0; i < numSplines; i++)
        //        {
        //            a[i] = Math.Round((m[i + 1] - m[i]) / (2 * h[i]), precision);
        //            b[i] = Math.Round((m[i] * x[i + 1] - m[i + 1] * x[i]) / h[i], precision);
        //            c[i] = Math.Round(
        //                (-m[i] * x[i + 1] * x[i + 1] + m[i + 1] * x[i] * x[i]) / (2 * h[i]) + y[i] + (m[i] * h[i]) / 2,
        //                precision);
        //        }
        //    }
        //private void SolveCubicSpline(
        //double[] x, double[] y, int precision,
        //out double[] a, out double[] b, out double[] c, out double[] d, out double[] h, out double[] alpha)
        //{
        //    int n = x.Length;
        //    int numSplines = n - 1; // Số lượng khoảng (spline)
        //    int numEquations = n - 2; // Số phương trình cần giải

        //    a = new double[numSplines];
        //    b = new double[numSplines];
        //    c = new double[numSplines];
        //    d = new double[numSplines];
        //    h = new double[numSplines];
        //    alpha = new double[n];    // Đạo hàm cấp hai S''(x_i)

        //    double[] gamma = new double[numEquations]; // Vế phải của hệ PT

        //    // --- Bước 1: Tính h[i] ---
        //    for (int i = 0; i < numSplines; i++)
        //    {
        //        h[i] = Math.Round(x[i + 1] - x[i], precision);
        //        if (h[i] == 0)
        //        {
        //            throw new ArgumentException($"Các mốc x bị trùng nhau tại chỉ số {i}.");
        //        }
        //    }

        //    // --- Bước 2: Tính gamma[i] (vế phải) ---
        //    for (int i = 0; i < numEquations; i++)
        //    {
        //        double term1 = (y[i + 2] - y[i + 1]) / h[i + 1];
        //        double term2 = (y[i + 1] - y[i]) / h[i];
        //        gamma[i] = Math.Round(term1 - term2, precision);
        //    }

        //    // --- Bước 3: Giải hệ phương trình tam đường chéo để tìm alpha[i] ---
        //    // Giả định Spline Tự nhiên (Natural Spline): alpha[0] = 0 và alpha[n-1] = 0
        //    alpha[0] = 0;
        //    alpha[n - 1] = 0;

        //    // Xây dựng 3 đường chéo của hệ phương trình
        //    double[] subDiag = new double[numEquations];    // Đường chéo dưới
        //    double[] mainDiag = new double[numEquations];   // Đường chéo chính
        //    double[] superDiag = new double[numEquations];  // Đường chéo trên

        //    for (int i = 0; i < numEquations; i++)
        //    {
        //        subDiag[i] = Math.Round(h[i] / 6.0, precision);
        //        mainDiag[i] = Math.Round((h[i] + h[i + 1]) / 3.0, precision);
        //        superDiag[i] = Math.Round(h[i + 1] / 6.0, precision);
        //    }

        //    // Điều chỉnh cho điều kiện biên (alpha[0]=0, alpha[n-1]=0)
        //    subDiag[0] = 0;
        //    superDiag[numEquations - 1] = 0;

        //    // Giải hệ
        //    double[] solution = SolveTridiagonalSystem(subDiag, mainDiag, superDiag, gamma);

        //    // Nạp kết quả vào mảng alpha
        //    for (int i = 0; i < numEquations; i++)
        //    {
        //        alpha[i + 1] = solution[i];
        //    }

        //    // --- Bước 4: Tính các hệ số a, b, c, d  ---
        //    for (int i = 0; i < numSplines; i++)
        //    {
        //        double h_i = h[i];
        //        double x_i = x[i];
        //        double x_i1 = x[i + 1];
        //        double y_i = y[i];
        //        double y_i1 = y[i + 1];
        //        double alpha_i = alpha[i];
        //        double alpha_i1 = alpha[i + 1];

        //        // a[i]
        //        a[i] = Math.Round((alpha_i1 - alpha_i) / (6 * h_i), precision);

        //        // b[i]
        //        b[i] = Math.Round((alpha_i * x_i1 - alpha_i1 * x_i) / (2 * h_i), precision);

        //        // c[i]
        //        double c_term1 = (-alpha_i * x_i1 * x_i1 + alpha_i1 * x_i * x_i) / (2 * h_i);
        //        double c_term2 = (y_i1 - y_i) / h_i;
        //        double c_term3 = (-(alpha_i1 - alpha_i) * h_i) / 6;
        //        c[i] = Math.Round(c_term1 + c_term2 + c_term3, precision);

        //        // d[i]
        //        double d_term1 = (alpha_i * x_i1 * x_i1 * x_i1 - alpha_i1 * x_i * x_i * x_i) / (6 * h_i);
        //        double d_term2 = (y_i * x_i1 - y_i1 * x_i) / h_i;
        //        double d_term3 = ((alpha_i1 * x_i - alpha_i * x_i1) * h_i) / 6;
        //        d[i] = Math.Round(d_term1 + d_term2 + d_term3, precision);
        //    }
        //}

        // Spline 

        //private void DisplayLinearSplineResults(RichTextBox rtb, double[] x, double[] y, double[] a, double[] b, double[] h, int precision)
        //{
        //    rtb.Clear();
        //    var sb = new StringBuilder();

        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine("KẾT QUẢ SPLINE TUYẾN TÍNH (SPLINE CẤP 1)");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

        //    // 1. Hiển thị dữ liệu đầu vào
        //    sb.AppendLine("DỮ LIỆU ĐẦU VÀO:");
        //    sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    sb.AppendLine("   i |      x[i]    |      y[i]    |");
        //    sb.AppendLine(new string('─', 40));

        //    for (int i = 0; i < x.Length; i++)
        //    {
        //        sb.AppendLine($"  {i,2} | {x[i],12:F6} | {y[i],12:F6} |");
        //    }
        //    sb.AppendLine();

        //    // 2. Hiển thị thông tin từng đoạn spline
        //    sb.AppendLine("CHI TIẾT CÁC ĐOẠN SPLINE:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");

        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        sb.AppendLine($"\nĐOẠN {i + 1}: [{x[i]:F6}, {x[i + 1]:F6}]");
        //        sb.AppendLine("───────────────────────────────────────────────────────────────");
        //        sb.AppendLine($"  h[{i}] = x[{i + 1}] - x[{i}]");
        //        sb.AppendLine($"      = {x[i + 1]:F6} - {x[i]:F6}");
        //        sb.AppendLine($"      = {h[i]:F6}");
        //        sb.AppendLine();

        //        sb.AppendLine($"  a[{i}] = (y[{i + 1}] - y[{i}]) / h[{i}]");
        //        sb.AppendLine($"      = ({y[i + 1]:F6} - {y[i]:F6}) / {h[i]:F6}");
        //        sb.AppendLine($"      = {a[i]:F6}");
        //        sb.AppendLine();

        //        sb.AppendLine($"  b[{i}] = (y[{i}] × x[{i + 1}] - y[{i + 1}] × x[{i}]) / h[{i}]");
        //        sb.AppendLine($"      = ({y[i]:F6} × {x[i + 1]:F6} - {y[i + 1]:F6} × {x[i]:F6}) / {h[i]:F6}");
        //        sb.AppendLine($"      = ({y[i] * x[i + 1]:F6} - {y[i + 1] * x[i]:F6}) / {h[i]:F6}");
        //        sb.AppendLine($"      = {b[i]:F6}");
        //        sb.AppendLine();

        //        string sign = b[i] >= 0 ? "+" : "";
        //        sb.AppendLine($"  Phương trình: S_{i}(x) = {a[i]:F6}×x {sign} {b[i]:F6}");
        //        sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    }

        //    // 3. Tổng hợp thông số
        //    sb.AppendLine("\nBẢNG TỔNG HỢP THÔNG SỐ:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine("  k |  Khoảng [x_k, x_{k+1}]  |    h_k     |     a_k      |     b_k      |");
        //    sb.AppendLine(new string('─', 80));

        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        sb.AppendLine($" {i,2} | [{x[i],6:F3}, {x[i + 1],6:F3}] | {h[i],10:F6} | {a[i],12:F6} | {b[i],12:F6} |");
        //    }
        //    sb.AppendLine();

        //    // 4. Danh sách phương trình
        //    sb.AppendLine("CÁC PHƯƠNG TRÌNH ĐOẠN:");
        //    sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        string sign = b[i] >= 0 ? "+" : "";
        //        sb.AppendLine($"  S_{i}(x) = {a[i]:F6}×x {sign} {b[i]:F6}  trên [{x[i]:F3}, {x[i + 1]:F3}]");
        //    }
        //    sb.AppendLine();

        //    rtb.AppendText(sb.ToString());
        //}
        //private void DisplayQuadraticSplineResults(RichTextBox rtb, double[] x, double[] y, double[] a, double[] b, double[] c, double[] h, double[] m, double[] v, int precision)
        //{
        //    rtb.Clear();
        //    var sb = new StringBuilder();

        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine("KẾT QUẢ SPLINE BẬC HAI (SPLINE CẤP 2)");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

        //    // 1. Hiển thị dữ liệu đầu vào
        //    sb.AppendLine("DỮ LIỆU ĐẦU VÀO:");
        //    sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    sb.AppendLine("   i |      x[i]    |      y[i]    |");
        //    sb.AppendLine(new string('─', 40));

        //    for (int i = 0; i < x.Length; i++)
        //    {
        //        sb.AppendLine($"  {i,2} | {x[i],12:F6} | {y[i],12:F6} |");
        //    }
        //    sb.AppendLine();

        //    // 2. Hiển thị điều kiện biên
        //    sb.AppendLine("ĐIỀU KIỆN BIÊN:");
        //    sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    sb.AppendLine($"  S'(x₀) = m[0] = {m[0]:F6}");
        //    if (m[0] == 0)
        //    {
        //        sb.AppendLine("  (Điều kiện mặc định: đạo hàm bậc nhất tại điểm đầu bằng 0)");
        //    }
        //    else
        //    {
        //        sb.AppendLine("  (Điều kiện biên do người dùng chỉ định)");
        //    }
        //    sb.AppendLine();

        //    // 3. Hiển thị tính toán h và v
        //    sb.AppendLine("TÍNH TOÁN KHOẢNG CÁCH h VÀ HỆ SỐ v:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");

        //    for (int i = 0; i < h.Length; i++)
        //    {
        //        sb.AppendLine($"\nĐOẠN {i + 1}:");
        //        sb.AppendLine("───────────────────────────────────────────────────────────────");
        //        sb.AppendLine($"  h[{i}] = x[{i + 1}] - x[{i}]");
        //        sb.AppendLine($"      = {x[i + 1]:F6} - {x[i]:F6}");
        //        sb.AppendLine($"      = {h[i]:F6}");
        //        sb.AppendLine();

        //        sb.AppendLine($"  v[{i}] = 2 × (y[{i + 1}] - y[{i}]) / h[{i}]");
        //        sb.AppendLine($"      = 2 × ({y[i + 1]:F6} - {y[i]:F6}) / {h[i]:F6}");
        //        sb.AppendLine($"      = 2 × {y[i + 1] - y[i]:F6} / {h[i]:F6}");
        //        sb.AppendLine($"      = {v[i]:F6}");
        //    }
        //    sb.AppendLine();

        //    // 4. Hiển thị tính toán m (hệ số đạo hàm)
        //    sb.AppendLine("TÍNH TOÁN HỆ SỐ ĐẠO HÀM m:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine($"  m[0] = 0 (điều kiện biên)");
        //    sb.AppendLine();

        //    for (int i = 0; i < m.Length - 1; i++)
        //    {
        //        sb.AppendLine($"  m[{i + 1}] = v[{i}] - m[{i}]");
        //        sb.AppendLine($"        = {v[i]:F6} - {m[i]:F6}");
        //        sb.AppendLine($"        = {m[i + 1]:F6}");
        //        sb.AppendLine();
        //    }

        //    // 5. Hiển thị chi tiết tính toán các hệ số a, b, c
        //    sb.AppendLine("CHI TIẾT TÍNH TOÁN CÁC HỆ SỐ:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");

        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        sb.AppendLine($"\nĐOẠN {i + 1}: [{x[i]:F6}, {x[i + 1]:F6}]");
        //        sb.AppendLine("───────────────────────────────────────────────────────────────");

        //        // Tính a[i]
        //        sb.AppendLine($"  a[{i}] = (m[{i + 1}] - m[{i}]) / (2 × h[{i}])");
        //        sb.AppendLine($"      = ({m[i + 1]:F6} - {m[i]:F6}) / (2 × {h[i]:F6})");
        //        sb.AppendLine($"      = {m[i + 1] - m[i]:F6} / {2 * h[i]:F6}");
        //        sb.AppendLine($"      = {a[i]:F6}");
        //        sb.AppendLine();

        //        // Tính b[i]
        //        sb.AppendLine($"  b[{i}] = (m[{i}] × x[{i + 1}] - m[{i + 1}] × x[{i}]) / h[{i}]");
        //        sb.AppendLine($"      = ({m[i]:F6} × {x[i + 1]:F6} - {m[i + 1]:F6} × {x[i]:F6}) / {h[i]:F6}");
        //        sb.AppendLine($"      = ({m[i] * x[i + 1]:F6} - {m[i + 1] * x[i]:F6}) / {h[i]:F6}");
        //        sb.AppendLine($"      = {b[i]:F6}");
        //        sb.AppendLine();

        //        // Tính c[i]
        //        double term1 = -m[i] * x[i + 1] * x[i + 1];
        //        double term2 = m[i + 1] * x[i] * x[i];
        //        double term3 = y[i];
        //        double term4 = m[i] * h[i] / 2;

        //        sb.AppendLine($"  c[{i}] = (-m[{i}] × x[{i + 1}]² + m[{i + 1}] × x[{i}]²) / (2 × h[{i}]) + y[{i}] + (m[{i}] × h[{i}]) / 2");
        //        sb.AppendLine($"      = ({-m[i]:F6} × {x[i + 1] * x[i + 1]:F6} + {m[i + 1]:F6} × {x[i] * x[i]:F6}) / {2 * h[i]:F6} + {y[i]:F6} + {m[i] * h[i] / 2:F6}");
        //        sb.AppendLine($"      = ({term1:F6} + {term2:F6}) / {2 * h[i]:F6} + {term3:F6} + {term4:F6}");
        //        sb.AppendLine($"      = {(term1 + term2) / (2 * h[i]):F6} + {term3 + term4:F6}");
        //        sb.AppendLine($"      = {c[i]:F6}");
        //        sb.AppendLine();

        //        sb.AppendLine($"  Phương trình: S_{i}(x) = {a[i]:F6}×x² + {b[i]:F6}×x + {c[i]:F6}");
        //        sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    }

        //    // 6. Bảng tổng hợp thông số
        //    sb.AppendLine("\nBẢNG TỔNG HỢP THÔNG SỐ:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine("  k |  Khoảng [x_k, x_{k+1}]  |    h_k     |    m_k     |    m_{k+1}   |");
        //    sb.AppendLine(new string('─', 85));

        //    for (int i = 0; i < h.Length; i++)
        //    {
        //        sb.AppendLine($" {i,2} | [{x[i],6:F3}, {x[i + 1],6:F3}] | {h[i],10:F6} | {m[i],10:F6} | {m[i + 1],12:F6} |");
        //    }
        //    sb.AppendLine();

        //    sb.AppendLine("  k |     a_k      |     b_k      |     c_k      |");
        //    sb.AppendLine(new string('─', 55));

        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        sb.AppendLine($" {i,2} | {a[i],12:F6} | {b[i],12:F6} | {c[i],12:F6} |");
        //    }
        //    sb.AppendLine();

        //    // 7. Danh sách phương trình
        //    sb.AppendLine("CÁC PHƯƠNG TRÌNH ĐOẠN:");
        //    sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        string signB = b[i] >= 0 ? "+" : "";
        //        string signC = c[i] >= 0 ? "+" : "";
        //        sb.AppendLine($"  S_{i}(x) = {a[i]:F6}×x² {signB} {b[i]:F6}×x {signC} {c[i]:F6}");
        //        sb.AppendLine($"           trên [{x[i]:F3}, {x[i + 1]:F3}]");
        //    }
        //    sb.AppendLine();
        //    rtb.AppendText(sb.ToString());
        //}
        //private void DisplayCubicSplineResults(RichTextBox rtb, double[] x, double[] y, double[] a, double[] b, double[] c, double[] d, double[] h, double[] alpha, int precision)
        //{
        //    rtb.Clear();
        //    var sb = new StringBuilder();

        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine("KẾT QUẢ SPLINE BẬC BA (SPLINE CẤP 3 - NATURAL SPLINE)");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

        //    // 1. Hiển thị dữ liệu đầu vào
        //    sb.AppendLine("DỮ LIỆU ĐẦU VÀO:");
        //    sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    sb.AppendLine("   i |      x[i]    |      y[i]    |");
        //    sb.AppendLine(new string('─', 40));

        //    for (int i = 0; i < x.Length; i++)
        //    {
        //        sb.AppendLine($"  {i,2} | {x[i],12:F6} | {y[i],12:F6} |");
        //    }
        //    sb.AppendLine();

        //    // 2. Hiển thị điều kiện biên
        //    sb.AppendLine("ĐIỀU KIỆN BIÊN (NATURAL SPLINE):");
        //    sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    sb.AppendLine($"  S''(x₀) = α[0] = 0");
        //    sb.AppendLine($"  S''(x_n) = α[n] = 0");
        //    sb.AppendLine("  (Đạo hàm cấp hai tại điểm đầu và cuối bằng 0)\n");

        //    // 3. Tính toán h
        //    sb.AppendLine("BƯỚC 1: TÍNH KHOẢNG CÁCH h:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    for (int i = 0; i < h.Length; i++)
        //    {
        //        sb.AppendLine($"  h[{i}] = x[{i + 1}] - x[{i}] = {x[i + 1]:F6} - {x[i]:F6} = {h[i]:F6}");
        //    }
        //    sb.AppendLine();

        //    // 4. Tính gamma (vế phải của hệ phương trình)
        //    sb.AppendLine("BƯỚC 2: TÍNH VẾ PHẢI γ CỦA HỆ PHƯƠNG TRÌNH:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    for (int i = 0; i < x.Length - 2; i++)
        //    {
        //        double term1 = (y[i + 2] - y[i + 1]) / h[i + 1];
        //        double term2 = (y[i + 1] - y[i]) / h[i];
        //        double gamma = term1 - term2;

        //        sb.AppendLine($"  γ[{i}] = (y[{i + 2}] - y[{i + 1}])/h[{i + 1}] - (y[{i + 1}] - y[{i}])/h[{i}]");
        //        sb.AppendLine($"       = ({y[i + 2]:F6} - {y[i + 1]:F6})/{h[i + 1]:F6} - ({y[i + 1]:F6} - {y[i]:F6})/{h[i]:F6}");
        //        sb.AppendLine($"       = {term1:F6} - {term2:F6}");
        //        sb.AppendLine($"       = {gamma:F6}");
        //        sb.AppendLine();
        //    }

        //    // 5. Hiển thị hệ phương trình tam đường chéo
        //    sb.AppendLine("BƯỚC 3: XÂY DỰNG HỆ PHƯƠNG TRÌNH TAM ĐƯỜNG CHÉO:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine();

        //    int numEq = x.Length - 2;
        //    sb.AppendLine("   i | Đường chéo dưới | Đường chéo chính | Đường chéo trên |");
        //    sb.AppendLine(new string('─', 75));

        //    for (int i = 0; i < numEq; i++)
        //    {
        //        double subDiag = (i == 0) ? 0 : h[i] / 6.0;
        //        double mainDiag = (h[i] + h[i + 1]) / 3.0;
        //        double superDiag = (i == numEq - 1) ? 0 : h[i + 1] / 6.0;

        //        sb.AppendLine($"  {i,2} | {subDiag,16:F6} | {mainDiag,16:F6} | {superDiag,15:F6} |");
        //    }
        //    sb.AppendLine();

        //    // 6. Hiển thị giải hệ và alpha
        //    sb.AppendLine("BƯỚC 4: GIẢI HỆ PHƯƠNG TRÌNH TÌM α (S''(x_i)):");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine();
        //    sb.AppendLine("   i |      α[i]     |");
        //    sb.AppendLine(new string('─', 25));

        //    for (int i = 0; i < alpha.Length; i++)
        //    {
        //        string note = "";
        //        if (i == 0) note = " (điều kiện biên)";
        //        if (i == alpha.Length - 1) note = " (điều kiện biên)";

        //        sb.AppendLine($"  {i,2} | {alpha[i],13:F6} |{note}");
        //    }
        //    sb.AppendLine();

        //    // 7. Tính các hệ số a, b, c, d
        //    sb.AppendLine("BƯỚC 5: TÍNH CÁC HỆ SỐ a, b, c, d:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");

        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        sb.AppendLine($"\nĐOẠN {i + 1}: [{x[i]:F6}, {x[i + 1]:F6}]");
        //        sb.AppendLine("───────────────────────────────────────────────────────────────");

        //        double h_i = h[i];
        //        double x_i = x[i];
        //        double x_i1 = x[i + 1];
        //        double y_i = y[i];
        //        double y_i1 = y[i + 1];
        //        double alpha_i = alpha[i];
        //        double alpha_i1 = alpha[i + 1];

        //        // a[i]
        //        sb.AppendLine($"  a[{i}] = (α[{i + 1}] - α[{i}]) / (6 × h[{i}])");
        //        sb.AppendLine($"       = ({alpha_i1:F6} - {alpha_i:F6}) / (6 × {h_i:F6})");
        //        sb.AppendLine($"       = {alpha_i1 - alpha_i:F6} / {6 * h_i:F6}");
        //        sb.AppendLine($"       = {a[i]:F6}");
        //        sb.AppendLine();

        //        // b[i]
        //        sb.AppendLine($"  b[{i}] = (α[{i}] × x[{i + 1}] - α[{i + 1}] × x[{i}]) / (2 × h[{i}])");
        //        sb.AppendLine($"       = ({alpha_i:F6} × {x_i1:F6} - {alpha_i1:F6} × {x_i:F6}) / (2 × {h_i:F6})");
        //        sb.AppendLine($"       = ({alpha_i * x_i1:F6} - {alpha_i1 * x_i:F6}) / {2 * h_i:F6}");
        //        sb.AppendLine($"       = {b[i]:F6}");
        //        sb.AppendLine();

        //        // c[i]
        //        double c_term1 = (-alpha_i * x_i1 * x_i1 + alpha_i1 * x_i * x_i) / (2 * h_i);
        //        double c_term2 = (y_i1 - y_i) / h_i;
        //        double c_term3 = (-(alpha_i1 - alpha_i) * h_i) / 6;

        //        sb.AppendLine($"  c[{i}] = (-α[{i}] × x[{i + 1}]² + α[{i + 1}] × x[{i}]²) / (2 × h[{i}]) + (y[{i + 1}] - y[{i}])/h[{i}] - (α[{i + 1}] - α[{i}]) × h[{i}]/6");
        //        sb.AppendLine($"       = {c_term1:F6} + {c_term2:F6} + ({c_term3:F6})");
        //        sb.AppendLine($"       = {c[i]:F6}");
        //        sb.AppendLine();

        //        // d[i]
        //        double d_term1 = (alpha_i * x_i1 * x_i1 * x_i1 - alpha_i1 * x_i * x_i * x_i) / (6 * h_i);
        //        double d_term2 = (y_i * x_i1 - y_i1 * x_i) / h_i;
        //        double d_term3 = ((alpha_i1 * x_i - alpha_i * x_i1) * h_i) / 6;

        //        sb.AppendLine($"  d[{i}] = (α[{i}] × x[{i + 1}]³ - α[{i + 1}] × x[{i}]³) / (6 × h[{i}]) + (y[{i}] × x[{i + 1}] - y[{i + 1}] × x[{i}])/h[{i}] + (α[{i + 1}] × x[{i}] - α[{i}] × x[{i + 1}]) × h[{i}]/6");
        //        sb.AppendLine($"       = {d_term1:F6} + {d_term2:F6} + {d_term3:F6}");
        //        sb.AppendLine($"       = {d[i]:F6}");
        //        sb.AppendLine();

        //        sb.AppendLine($"  Phương trình: S_{i}(x) = {a[i]:F6}×x³ + {b[i]:F6}×x² + {c[i]:F6}×x + {d[i]:F6}");
        //        sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    }

        //    // 8. Bảng tổng hợp
        //    sb.AppendLine("\nBẢNG TỔNG HỢP THÔNG SỐ:");
        //    sb.AppendLine("═══════════════════════════════════════════════════════════════");
        //    sb.AppendLine("  k |  Khoảng [x_k, x_{k+1}]  |    h_k     |    α_k     |   α_{k+1}  |");
        //    sb.AppendLine(new string('─', 85));

        //    for (int i = 0; i < h.Length; i++)
        //    {
        //        sb.AppendLine($" {i,2} | [{x[i],6:F3}, {x[i + 1],6:F3}] | {h[i],10:F6} | {alpha[i],10:F6} | {alpha[i + 1],10:F6} |");
        //    }
        //    sb.AppendLine();

        //    sb.AppendLine("  k |     a_k      |     b_k      |     c_k      |     d_k      |");
        //    sb.AppendLine(new string('─', 70));

        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        sb.AppendLine($" {i,2} | {a[i],12:F6} | {b[i],12:F6} | {c[i],12:F6} | {d[i],12:F6} |");
        //    }
        //    sb.AppendLine();

        //    // 9. Danh sách phương trình
        //    sb.AppendLine("CÁC PHƯƠNG TRÌNH ĐOẠN:");
        //    sb.AppendLine("───────────────────────────────────────────────────────────────");
        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        string signB = b[i] >= 0 ? "+" : "";
        //        string signC = c[i] >= 0 ? "+" : "";
        //        string signD = d[i] >= 0 ? "+" : "";

        //        sb.AppendLine($"  S_{i}(x) = {a[i]:F6}×x³ {signB} {b[i]:F6}×x² {signC} {c[i]:F6}×x {signD} {d[i]:F6}");
        //        sb.AppendLine($"           trên [{x[i]:F3}, {x[i + 1]:F3}]");
        //    }
        //    sb.AppendLine();
        //    rtb.AppendText(sb.ToString());
        //}

        //private double[] SolveTridiagonalSystem(double[] a, double[] b, double[] c, double[] d)
        //{
        //    int n = d.Length;
        //    double[] c_prime = new double[n];
        //    double[] d_prime = new double[n];
        //    double[] x = new double[n];

        //    // --- Quá trình thuận ---
        //    c_prime[0] = c[0] / b[0];
        //    d_prime[0] = d[0] / b[0];

        //    for (int i = 1; i < n; i++)
        //    {
        //        double temp = b[i] - a[i] * c_prime[i - 1];
        //        c_prime[i] = c[i] / temp;
        //        d_prime[i] = (d[i] - a[i] * d_prime[i - 1]) / temp;
        //    }

        //    // --- Quá trình ngược (Back substitution) ---
        //    x[n - 1] = d_prime[n - 1];

        //    for (int i = n - 2; i >= 0; i--)
        //    {
        //        x[i] = d_prime[i] - c_prime[i] * x[i + 1];
        //    }

        //    return x;
        //}
    }
}
