using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Lab4_Webserver
{
    public partial class Bai4_webBrowser : Form
    {
        public Bai4_webBrowser()
        {
            InitializeComponent();
        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            if (webBrowserContent.Visible == false)
            {
                rtbPagesource.Visible = false;
                webBrowserContent.Visible = true;
            }
            try
            {
                webBrowserContent.Navigate(txtUrl.Text);
                webBrowserContent.ScriptErrorsSuppressed = true;
            }
            
            catch 
            {
                MessageBox.Show("Enter a valid URL");
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (webBrowserContent.CanGoBack)
                webBrowserContent.GoBack();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (webBrowserContent.CanGoForward)
                webBrowserContent.GoForward();
        }   

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            webBrowserContent.Refresh();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            string folderPath = "E:/HK4_UIT/LTMCB/Lab/Lab4/Bai4";
            MessageBox.Show(webBrowserContent.Url.ToString());
            string htmlFilePath = Regex.Replace(webBrowserContent.Url.ToString(), "^(http:\\/\\/www.|https:\\/\\/www.|https:\\/\\/|http:\\/\\/)", string.Empty);
            MessageBox.Show(htmlFilePath);

            using (WebClient myClient = new WebClient())
            {
                var html = webBrowserContent.DocumentText;

                HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(html);
                string downloadFolder = Path.Combine(folderPath, Regex.Replace(webBrowserContent.Url.ToString(), "^(http:\\/\\/www.|https:\\/\\/www.|https:\\/\\/|http:\\/\\/)", string.Empty));
                Directory.CreateDirectory(downloadFolder);
                // Lưu nội dung HTML đã chỉnh sửa vào tệp mới trong thư mục đầu ra
                myClient.DownloadFile(txtUrl.Text, Path.Combine(downloadFolder, htmlFilePath.Replace("/", "_") + ".html"));
                

                // Hàm để xử lý và tải xuống các tệp trong thẻ a, css, script, img
                void Process(string Url)
                {
                    // Tải xuống tệp và lưu vào thư mục đầu ra
                    Uri fileUrl = new Uri(new Uri(txtUrl.Text), Url);
                    // Xây dựng đường dẫn tệp đầy đủ
                    string fileName = Path.GetFileName(fileUrl.LocalPath);
                    string filePath = Path.Combine(downloadFolder, fileName);
                    try
                    {
                        myClient.DownloadFile(fileUrl, filePath);
                    }
                    catch( Exception ex )
                    {
                        MessageBox.Show($"Error downloading: {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                // Hàm để xử lý và tải xuống các tệp trong thẻ a, css, script, img
                void Processimg(string imgUrl)
                {
                    // Tải xuống tệp và lưu vào thư mục đầu ra
                    Uri fileUrl = new Uri(new Uri(txtUrl.Text), imgUrl);
                    string query = fileUrl.Query;
                    string queryName = "";
                    if (query != "")
                        queryName = query.Substring(1);
                    // Xây dựng đường dẫn tệp đầy đủ
                    string fileName = Path.GetFileName(fileUrl.LocalPath);
                    string name = queryName + fileName;

                    string filePath = Path.Combine(downloadFolder, name);
                    try
                    {
                        myClient.DownloadFile(fileUrl, filePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error downloading: {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // Tìm tất cả các thẻ <a> trong HTML và tải xuống các tệp liên quan
                HtmlNodeCollection anchorNodes = document.DocumentNode.SelectNodes("//a");
                if (anchorNodes != null)
                {
                    foreach (HtmlNode anchorNode in anchorNodes)
                    {
                        string hrefUrl = anchorNode.GetAttributeValue("href", "");
                        
                        if (!string.IsNullOrWhiteSpace(hrefUrl))
                        {
                            if (!hrefUrl.ToLower().EndsWith(".pdf"))
                            {
                                hrefUrl = "";
                            }
                            if (hrefUrl == "")
                                continue;
                            // Kiểm tra xem liên kết có phải là liên kết tới tệp không
                            if (Path.HasExtension(hrefUrl))
                            {
                                Process(hrefUrl);

                                // Thay thế đường dẫn trong liên kết <a> bằng đường dẫn tới tệp đã tải xuống
                                anchorNode.SetAttributeValue("href", Path.GetFileName(hrefUrl));
                            }
                        }
                    }
                }

                // Tìm và xử lý tất cả các thẻ <link> để tải xuống các tệp CSS
                HtmlNodeCollection linkNodes = document.DocumentNode.SelectNodes("//link[@rel='stylesheet']");
                if (linkNodes != null)
                {
                    foreach (HtmlNode linkNode in linkNodes)
                    {
                        string hrefUrl = linkNode.GetAttributeValue("href", "");
                        if (!string.IsNullOrWhiteSpace(hrefUrl))
                        {
                            Process(hrefUrl);

                            // Thay thế đường dẫn trong thẻ <link> bằng đường dẫn tới tệp đã tải xuống
                            linkNode.SetAttributeValue("href", Path.GetFileName(hrefUrl));
                        }
                    }
                }

                // Tìm và xử lý tất cả các thẻ <script> để tải xuống các tệp JavaScript
                HtmlNodeCollection scriptNodes = document.DocumentNode.SelectNodes("//script[@src]");
                if (scriptNodes != null)
                {
                    foreach (HtmlNode scriptNode in scriptNodes)
                    {
                        string srcUrl = scriptNode.GetAttributeValue("src", "");
                        if (!string.IsNullOrWhiteSpace(srcUrl))
                        {
                            Process(srcUrl);

                            // Thay thế đường dẫn trong thẻ <script> bằng đường dẫn tới tệp đã tải xuống
                            scriptNode.SetAttributeValue("src", Path.GetFileName(srcUrl));
                        }
                    }
                }

                // Tìm và xử lý tất cả các thẻ <img> để tải xuống các hình ảnh
                HtmlNodeCollection imgNodes = document.DocumentNode.SelectNodes("//img");
                if (imgNodes != null)
                {
                    foreach (HtmlNode imgNode in imgNodes)
                    {
                        string srcUrl = imgNode.GetAttributeValue("src", "");
                        if (!string.IsNullOrWhiteSpace(srcUrl))
                        {
                            Processimg(srcUrl);

                            // Thay thế đường dẫn trong thẻ <img> bằng đường dẫn tới tệp đã tải xuống
                            imgNode.SetAttributeValue("src", Path.GetFileName(srcUrl));
                        }
                    }
                }
                MessageBox.Show("Downloaded successfully!", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                

            }
        }
        private void btnOpensource_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            
            if (!string.IsNullOrEmpty(ofd.FileName))
            {
                string filePath = ofd.FileName;

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string htmlContent = reader.ReadToEnd();
                    webBrowserContent.Visible = false;
                    rtbPagesource.Visible = true;
                    rtbPagesource.Text = htmlContent;
                }
            }
            else
            {
                MessageBox.Show("Please choose valid HTML file!");
            }
        }

    }
}
