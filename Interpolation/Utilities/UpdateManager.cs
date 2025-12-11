using System;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Interpolation.Utilities
{
    public static class UpdateManager
    {
        private static readonly string RemoteVersionUrl = "https://raw.githubusercontent.com/qanhta2710/Interpolation/master/Interpolation/Properties/AssemblyInfo.cs";

        private static readonly string DownloadUrl = "https://github.com/qanhta2710/Interpolation/releases";

        private static readonly HttpClient client = new HttpClient();
        public static async Task CheckForUpdateAsync(bool isManualCheck)
        {
            try
            {
                if (!client.DefaultRequestHeaders.Contains("User-Agent"))
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Interpolation-App");
                }
                string urlWithNoCache = $"{RemoteVersionUrl}?t={DateTime.Now.Ticks}";
                string fileContent = await client.GetStringAsync(urlWithNoCache);

                var match = Regex.Match(fileContent, @"AssemblyVersion\s*\(\s*""([\d\.]+)""\s*\)");

                if (!match.Success)
                {
                    if (isManualCheck)
                        MessageBox.Show("Không tìm thấy thông tin phiên bản trên Server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string onlineVerStr = match.Groups[1].Value;
                Version onlineV = new Version(onlineVerStr);

                Version currentV = Assembly.GetExecutingAssembly().GetName().Version;
                string currentVerStr = $"{currentV.Major}.{currentV.Minor}.{currentV.Build}.{currentV.Revision}";
                if (onlineV > currentV)
                {
                    DialogResult dialogResult = MessageBox.Show(
                        $"Phát hiện phiên bản mới: {onlineVerStr}\n" +
                        $"Phiên bản hiện tại: {currentVerStr}\n\n" +
                        "Bạn có muốn tải về ngay không?",
                        "Cập nhật",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (dialogResult == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(DownloadUrl);
                    }
                }
                else
                {
                    if (isManualCheck)
                    {
                        MessageBox.Show($"Bạn đang dùng bản mới nhất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                if (isManualCheck)
                {
                    MessageBox.Show("Lỗi kết nối GitHub: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}