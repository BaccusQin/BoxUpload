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
using System.Threading;
using System.Runtime.InteropServices;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using System.Text.RegularExpressions;

namespace BoxUpload
{
    public partial class BoxLogin : Form
    {
      
        public static string token { get; set; }

        public static string sBoxClientId { get; set; }

        public static string sBoxClientSecret { get; set; }
        public DataTable outPutCsvTable { get; set; }


        public BoxLogin()
        {
            
            InitializeComponent();
            System.Windows.Forms.TextBox.CheckForIllegalCrossThreadCalls = false;
            OnDelConnBox += new DelConnBox(afterWebChange);
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
                lbUserName.Visible = false;
                label1.Visible = false;
              


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
        private string connCode;
        public string ConnCode 
        {
            get 
            {
               
                return connCode; 
            }
            set
            {
                if (webBrowser1.Url.ToString().Contains("code"))
                {
                    connCode = webBrowser1.Url.ToString();
                    whenWebChange();
                }

               
            }
        }
        public delegate void DelConnBox(object sender, EventArgs e);
        public event DelConnBox OnDelConnBox;
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ConnCode = webBrowser1.Url.ToString();
        }
        private async void afterWebChange(object sender,EventArgs e)
        {
            await ShowUserInfo();
           // webBrowser1.Visible = false;

        }
        private void whenWebChange()
        {
            if (OnDelConnBox != null)
            {
                OnDelConnBox(this, null);
            }
        }


       // private async void ShowUser_ClickAsync(object sender, EventArgs e)]
        private async Task ShowUserInfo()
        {
          
            if (webBrowser1.Url != null)
            {
                webBrowser1.Visible = false;
              //  MessageBox.Show(webBrowser1.Url.ToString());
               // connCode = webBrowser1.Url.ToString();
                string code =this.ConnCode.Split('?')[1].Substring(5);
                BoxConn boxConn = new BoxConn(code,sBoxClientId,sBoxClientSecret);
                token=boxConn.token;
                await SetForm.GetInfo();
                lbUserName.Text = SetForm.UserName;
                lbUserName.Visible = true;
                label1.Visible = true;
                SetFileList("0");
          

            }
            else
            {
                MessageBox.Show("Boxにログインしてください。");
            }
            
        }

     

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog csvFile = new OpenFileDialog();
            csvFile.Filter = csvFile.Filter = "CSVファイル|*.csv|TXTファイル|*.txt|すべてのファイル|*.*";
            if (csvFile.ShowDialog() == DialogResult.OK)
            {
                string filePath = csvFile.FileName;
                textBox1.Text = filePath;
            }

        }
        
        public async Task SetColl(string sFod, string shareMail)
        {
            try
            {
                /*
                var config = new BoxConfig(sBoxClientId, sBoxClientSecret, new Uri("http://localhost"));
                var session = new OAuthSession(token, "NOT_NEEDED", 3600, "bearer");
                var client = new BoxClient(config, session);
                */



                BoxCollaborationRequest requestParams = new BoxCollaborationRequest()
                {
                    Item = new BoxRequestEntity()
                    {
                        Type = BoxType.folder,
                        Id = sFod
                    },
                    Role = "editor",
                    AccessibleBy = new BoxCollaborationUserRequest()
                    {
                        Type= BoxType.user,
                        Login = shareMail
                    }
                };
                if (shareMail != "" && shareMail != null)
                {
                    BoxCollaboration collab = await SetForm.client.CollaborationsManager.AddCollaborationAsync(requestParams);
                }
                else
                {
                    return;
                }




            }
            catch (Exception e)
            {
                MessageBox.Show(sFod+"_"+shareMail+"_"+e.Message);
            }


        }
        /*
        public string GetFileMail(string sToken, string folderCode, int Flag)
        {
            try
            {
                string sUrl = "https://api.box.com/2.0/folders/" + folderCode + "/collaborations";
                Common folderTable = new Common();
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(sUrl);
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                byte[] ba = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                System.Text.Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                StringBuilder mailList = new StringBuilder();
                StringBuilder csvMailList = new StringBuilder();
                req.Method = "GET";
                req.Headers.Add("Authorization: Bearer " + sToken);

                System.Net.WebResponse res = req.GetResponse();
                System.IO.Stream resStream = res.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(resStream, enc);
                string js = sr.ReadToEnd();
                JObject Json = JObject.Parse(js);
                folderTable.json = Json["entries"].ToString();
                var folerInfor = Json["entries"].AsJEnumerable();
                foreach (var item in folerInfor)
                {
                    string a = item["accessible_by"].ToString();
                    string b = JObject.Parse(a)["login"].ToString();
                    mailList.Append(b).Append("\n");
                    csvMailList.Append(b);
                }
                if (Flag == 1)
                {
                    return csvMailList.ToString();
                }
                else
                {
                    return mailList.ToString();
                }



            }
            catch (Exception e)
            {
                if(e.Message!= "リモート サーバーがエラーを返しました: (404) 見つかりません")
                {
                    MessageBox.Show(e.Message.ToString());
                }  
                return null;
            }

        }
        */
        public  async void SetFileList(string folderCode)
        {
            try
            {
                
                BoxCollection<BoxItem> items = await SetForm.client.FoldersManager.GetFolderItemsAsync(folderCode, 1000);
               
              
                DataTable blindData = new DataTable();
                blindData.Columns.Add("id", Type.GetType("System.String"));
                blindData.Columns.Add("name", Type.GetType("System.String"));
                blindData.Columns.Add("Email", Type.GetType("System.String"));
                foreach (BoxItem x in items.Entries)
                {
                    DataRow row = blindData.NewRow();
                    row["id"] = x.Id.ToString();
                  
                    row["name"] = x.Name.ToString();

                    BoxCollection < BoxCollaboration > collaborations = await SetForm.client.FoldersManager.GetCollaborationsAsync(x.Id.ToString());
                    string strEmail="";
                    foreach (BoxCollaboration y in collaborations.Entries)
                    {
                        strEmail = strEmail + ((Box.V2.Models.BoxUser)y.AccessibleBy).Login+";";
                    }
                    row["Email"] = strEmail;
                    blindData.Rows.Add(row);
                }       
                DataTable csvTable = blindData.Copy();
                /*
                foreach (DataRow myrow in blindData.Rows)
                {
                    myrow[2] = GetFileMail(BoxLogin.token, myrow[0].ToString(), 2);
                }
                foreach (DataRow myrow in csvTable.Rows)
                {
                    myrow[2] = GetFileMail(BoxLogin.token, myrow[0].ToString(), 1);
                }
                */
                
                if (blindData.Rows.Count != 0)
                {
                    dataGridView1.AutoGenerateColumns = false;
                    dataGridView1.DataSource = blindData;
                    dataGridView1.Columns["id"].DataPropertyName = "id";
                    dataGridView1.Columns["name"].DataPropertyName = "name";
                    dataGridView1.Columns["Email"].DataPropertyName = "Email";
                }
          
                outPutCsvTable = csvTable;
       
               

            }
            catch (Exception e)
            {
               
                MessageBox.Show(e.Message.ToString());
                
               
                
            }



        }
        private void dataGridView1_CellClick(object sender,DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                try
                {
                    SetFileList(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

            }
            

        }


        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog csvFile = new SaveFileDialog();
            csvFile.Title = "保存位置を選択してください。";
            csvFile.Filter = "CSVファイル|*.csv|すべてのファイル|*.*";
            if (csvFile.ShowDialog() == DialogResult.OK)
            {
                string filePath = csvFile.FileName;
                SaveCsv(outPutCsvTable, filePath);
            }

            
        }
        public void SaveCsv(DataTable dt, string filePath)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs, Encoding.Default);
                var data = string.Empty;
                //写出列名称
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    data += dt.Columns[i].ColumnName;
                    if (i < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
                //写出各行数据
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    data = string.Empty;
                    for (var j = 0; j < dt.Columns.Count; j++)
                    {
                        data += dt.Rows[i][j].ToString();
                        if (j < dt.Columns.Count - 1)
                        {
                            data += ",";
                        }
                    }
                    sw.WriteLine(data);
                }
            }
            catch (IOException ex)
            {
                throw new IOException(ex.Message, ex);
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
        }
     //   public delegate  Task TakesWhileDelegate(string ID, string mail);
        private async void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                DataTable dtMailList = OpenCSV(textBox1.Text);
                foreach (DataRow myRow in dtMailList.Rows)
                {

                    string ID = myRow[0].ToString();
                    string mail = myRow[2].ToString().Split(';')[0];
                    await SetColl(ID, mail);
                    /*
                    TakesWhileDelegate d1 = SetColl;
                    IAsyncResult ar = d1.BeginInvoke(ID, mail,null,null);
                    while (!ar.IsCompleted)
                    {
                        Thread.Sleep(50);
                    }
                    */

                }
                MessageBox.Show("設定完了しました。");
            }
            else
            {
                MessageBox.Show("CSVファイルを参照してください。");
            }
            

        }
        public DataTable OpenCSV(string filePath)

        {

            DataTable dt = new DataTable();

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);

            //记录每次读取的一行记录

            string strLine = "";

            //记录每行记录中的各字段内容

            string[] aryLine;

            //标示列数

            int columnCount = 0;

            //标示是否是读取的第一行

            bool IsFirst = true;

            //逐行读取CSV中的数据

            while ((strLine = sr.ReadLine()) != null)

            {

                aryLine = strLine.Split(',');

                if (IsFirst == true)

                {

                    IsFirst = false;

                    columnCount = aryLine.Length;

                    for (int i = 0; i < columnCount; i++)

                    {

                        DataColumn dc = new DataColumn(aryLine[i]);

                        dt.Columns.Add(dc);

                    }

                }

                else

                {

                    DataRow dr = dt.NewRow();

                    for (int j = 0; j < columnCount; j++)

                    {

                        dr[j] = aryLine[j];

                    }

                    dt.Rows.Add(dr);

                }

            }

            sr.Close();

            fs.Close();


            return dt;

        }

        private  void button3_Click(object sender, EventArgs e)
        {
            SetFileList("0");
        }

        private void btnFromPath_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            OpenFileDialog csvFile = new OpenFileDialog();
            csvFile.Multiselect = true;
            string[] filePath;
            csvFile.Filter = csvFile.Filter = "PDFファイル|*.pdf|TXTファイル|*.txt|すべてのファイル|*.*";
            if (csvFile.ShowDialog() == DialogResult.OK)
            {
                
                filePath = csvFile.FileNames;
                foreach(string x in filePath)
                {
                    
                    textBox2.Text += x;
                    textBox2.Text += ",";
                }
                
            }

        }

        private void btnToPath_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = folderBrowserDialog1.SelectedPath;
            }

        }

        private void btnInvoice_Click(object sender, EventArgs e)
        {

            textBox4.Clear();
            if (textBox2.Text.Length < 1)
            {
                MessageBox.Show("ソースファイルを選択してください。");
                return;
            }
            else if (textBox3.Text.Length < 1)
            {
                MessageBox.Show("目標フォルダを選択してください。");
                return;
            }
            else
            {
                string allLog="";
                DateTime dt = DateTime.Now;
                try
                {
                    string sourceFileName = textBox2.Text.Split(',')[0];
                    string TargetPath = textBox3.Text + @"\\";
                    SetRectangle JudgePDFType = new SetRectangle(100, 0, 50, 300, sourceFileName);
                    int cx1 = 0, cy1 = 0, cx2 = 0, cy2 = 0, dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
                    if (JudgePDFType.GetPreName(1).Length >= 7)
                    {
                        if (Regex.IsMatch(JudgePDFType.GetPreName(1).Substring(7).Split(' ')[0], @"^[+-]?\d*[.]?\d*$"))
                        {
                            cx1 = 100; cy1 = 0; cx2 = 50; cy2 = 300;
                            dx1 = 50; dy1 = 350; dx2 = 10; dy2 = 20;
                        }
                        else
                        {
                            cx1 = 0; cy1 = 450; cx2 = 400; cy2 = 50;
                            dx1 = 330; dy1 = 540; dx2 = 150; dy2 = 15;
                        }

                    }
                    else
                    {
                        cx1 = 0; cy1 = 450; cx2 = 400; cy2 = 50;
                        dx1 = 330; dy1 = 540; dx2 = 150; dy2 = 15;
                    }

                    SetRectangle invoiceCustRec = new SetRectangle(cx1, cy1, cx2, cy2, sourceFileName);
                    SetRectangle invoiceDateRec = new SetRectangle(dx1, dy1, dx2, dy2, sourceFileName);

                    int times = 1;


                    for (int page = 1; page <= invoiceCustRec.pdfDoc.GetNumberOfPages();)
                    {

                        string strCustRec = invoiceCustRec.GetPreName(page);
                        string customerCode = strCustRec.Substring(7).Split(' ')[0];
                        string customerName = strCustRec.Substring(strCustRec.IndexOf(' ', 9) + 1).Trim();
                        string date = invoiceDateRec.GetPreName(page).Trim();

                        //名前設定
                        string targetFolderName = customerCode + "_" + customerName;
                        string fileName = date + "_" + targetFolderName + ".pdf";

                        PdfReader pdfReader = new PdfReader(sourceFileName);


                        //--------------------------------
                        if (!Directory.Exists(TargetPath + targetFolderName))
                        {
                            Directory.CreateDirectory(TargetPath + targetFolderName);
                        }
                        string filePath = TargetPath + targetFolderName + "\\" + fileName;


                        PdfDocument sourceDocument = new PdfDocument(pdfReader);

                        PdfDocument targetDocument = new PdfDocument(new PdfWriter(filePath));
                        PdfMerger merger = new PdfMerger(targetDocument);





                        allLog += "PageNo: "+page + "\n";
                        allLog += fileName+"作成しました。\n";
          
                        textBox4.AppendText( "PageNo: " + page + "\r\n");
                        textBox4.AppendText( fileName + "作成しました。\r\n");
                   
                    

                        do
                        {

                            Console.WriteLine(invoiceCustRec.GetPreName(page));

                            merger.Merge(sourceDocument, page, page);
                            page++;
                            if (page > invoiceCustRec.pdfDoc.GetNumberOfPages())
                            {
                                break;
                            }

                        }
                        while (invoiceCustRec.GetPreName(page - 1) == invoiceCustRec.GetPreName(page));


                        times++;

                        sourceDocument.Close();
                        merger.Close();
                        targetDocument.Close();


                    }
                    allLog += "処理完成しました。" + (times - 1) + "個PDFを処理しました。";


                    MessageBox.Show("処理完成しました。" + (times - 1) + "個PDFを処理しました。");

                }
                catch(Exception ex)
                {
                    allLog += ex.Message+"\n";
                }
                finally
                {
                    File.AppendAllText(".\\LOG\\" + dt.ToString("yyyy-MM-dd H-mm-ss") + "log.txt", "\r\n" + allLog);
                }
                // string result = text.ToString();
               
            }
        }

        
    }
    public class SetForm
    {
        public static string UserName { get; set; }
        public static BoxClient client;
        public static async Task GetInfo()
        {
            var config = new BoxConfig(BoxLogin.sBoxClientId, BoxLogin.sBoxClientSecret, new Uri("http://localhost"));
            var session = new OAuthSession(BoxLogin.token, "NOT_NEEDED", 3600, "bearer");
            client = new BoxClient(config, session);
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
