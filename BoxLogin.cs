using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using Box.V2.Config;
using Box.V2.Auth;
using Box.V2;
using Box.V2.Models;

namespace BoxUpload
{
    public partial class BoxLogin : Form
    {
        public string connCode { get; set; }
        public static string token { get; set; }

        public static string sBoxClientId { get; set; }

        public static string sBoxClientSecret { get; set; }

        
        public BoxLogin()
        {
            InitializeComponent();

        }

        private void BoxLogin_Load(object sender, EventArgs e)
        {
            try
            {
                sBoxClientId = "7lbnr90zubwiz6ddjdvjko3h8ytk67f8";
                sBoxClientSecret = "iI4AfSkyHVdFJQYbei2gVUXGg3iEUSaX";
                string strHtml = LoginPage_Load();
            
                webBrowser1.ScriptErrorsSuppressed= true;
                webBrowser1.Navigate("about:blank");
                webBrowser1.Document.OpenNew(true);
                webBrowser1.Document.Write(strHtml);

        

            }
            catch(Exception ex)
            {
                MessageBox.Show("エラー："+ex.Message);
                Application.Exit();
            }
          

        }
        private string LoginPage_Load( )
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
        
            string sUrl = "https://account.box.com/api/oauth2/authorize";
            string sPostData = "response_type=code&client_id=" +
            HttpUtility.UrlEncode(sBoxClientId);

            try
            {
                System.Text.Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                byte[] postDataBytes = System.Text.Encoding.ASCII.GetBytes(sPostData);
                System.Net.WebRequest req = System.Net.WebRequest.Create(sUrl);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = postDataBytes.Length;
                System.IO.Stream reqStream = req.GetRequestStream();
                reqStream.Write(postDataBytes, 0, postDataBytes.Length);
                reqStream.Close();
                System.Net.WebResponse res = req.GetResponse();
                System.IO.Stream resStream = res.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(resStream, enc);
                string js = sr.ReadToEnd();
                sr.Close();
                return js;

            }
            catch(Exception ex)
            {
                return ex.Message;
            }

        }

        private void ShowUser_Click(object sender, EventArgs e)
        {
            
            if (webBrowser1.Url != null)
            {
                webBrowser1.Visible = false;
                MessageBox.Show(webBrowser1.Url.ToString());
                connCode = webBrowser1.Url.ToString();
                string code =connCode.Split('?')[1].Substring(5);
                BoxConn boxConn = new BoxConn(code,sBoxClientId,sBoxClientSecret);
                token=boxConn.token;
                SetForm.GetInfo();
                lbUserName.Text = SetForm.UserName;
            }
            else
            {
                MessageBox.Show("Boxにログインしてください。");
            }
            
        }

       
    }
    public class SetForm
    {
        public static string UserName { get; set; }
        public static async Task GetInfo()
        {
            var config = new BoxConfig(BoxLogin.sBoxClientId, BoxLogin.sBoxClientSecret, new Uri("http://localhost"));
            var session = new OAuthSession(BoxLogin.token, "NOT_NEEDED", 3600, "bearer");
            var client = new BoxClient(config, session);
            BoxUser currentUser = await client.UsersManager.GetCurrentUserInformationAsync();
            UserName = currentUser.Name;
            
        }
       

    }
    public class BoxConn
    {
        public string token { get; set; }
        public BoxConn(string code,string sBoxClientId,string sBoxClientSecret)
        {
      
            string sUrl = "https://api.box.com/oauth2/token";
            string accessToken;
            try
            {

                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(sUrl);
                System.Text.Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                string strRequest = "client_id=" + sBoxClientId + "&" + "client_secret=" + sBoxClientSecret + "&code=" + code + "&grant_type=authorization_code";
                byte[] pd = new UTF8Encoding().GetBytes(strRequest);
                req.ContentLength = pd.Length;
                Stream ps = req.GetRequestStream();
                ps.Write(pd, 0, pd.Length);
                ps.Close();
                System.Net.WebResponse res = req.GetResponse();
                System.IO.Stream resStream = res.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(resStream, enc);
                string js = sr.ReadToEnd();
                JObject Json = JObject.Parse(js);
                accessToken = Json["access_token"].ToString();
                sr.Close();
                token= accessToken;
            

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
              
            }

        }

    }
}
