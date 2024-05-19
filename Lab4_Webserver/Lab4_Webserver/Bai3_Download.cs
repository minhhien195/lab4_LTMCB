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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab4_Webserver
{
    public partial class Bai3_Download : Form
    {
        public Bai3_Download()
        {
            InitializeComponent();
        }
        private string downloadHTML(string szURL)
        {
            WebClient myClient = new WebClient();
            Stream response = myClient.OpenRead(szURL);
            myClient.DownloadFile(szURL, txtSave.Text);
            StreamReader reader = new StreamReader(response);
            // Read the content.   
            string responseFromServer = reader.ReadToEnd();
            // Close the response.   
            response.Close();
            return responseFromServer;
        }
        private void btnDownload_Click(object sender, EventArgs e)
        {
            rtbContent.Text = downloadHTML(txtUrl.Text);
        }
    }
}
