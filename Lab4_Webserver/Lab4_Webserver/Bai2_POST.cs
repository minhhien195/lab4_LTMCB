using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab4_Webserver
{
    public partial class Bai2_POST : Form
    {
        public Bai2_POST()
        {
            InitializeComponent();
        }
        private string postHTML(string szURL)
        {
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create(szURL);
            request.Method = "POST";
            byte[] datapost = Encoding.UTF8.GetBytes(txtQuery.Text);
            request.ContentLength = datapost.Length;
            request.ContentType = "application/json";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(datapost, 0, datapost.Length); 
            requestStream.Close();
            // Get the response.   
            WebResponse response = request.GetResponse();
            // Get the stream containing content returned by the server.  
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.   
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.   
            string responseFromServer = reader.ReadToEnd();
            // Close the response.   
            response.Close();
            return responseFromServer;
        }
        private void btnPOST_Click(object sender, EventArgs e)
        {
            rtbContent.Text = postHTML(txtUrl.Text);
        }
    }
}
