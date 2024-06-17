using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace MultiReqHTTP
{
    public class Form1 : Form
    {
        private string sessionType;
        private string logFolder;
        private TextBox urlTextBox;
        private ComboBox methodComboBox;
        private TextBox responseTextBox;
        private TextBox logFolderTextBox;
        private Button setLogFolderButton;
        private Button sendRequestButton;
        private Button copyResponseButton;
        private DataGridView headersDataGridView;
        private TextBox bodyTextBox;

        public Form1(string sessionType)
        {
            this.sessionType = sessionType;
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Create Controls
            Label urlLabel = new Label();
            urlTextBox = new TextBox();
            methodComboBox = new ComboBox();
            sendRequestButton = new Button();
            copyResponseButton = new Button();
            Label responseLabel = new Label();
            responseTextBox = new TextBox();
            logFolderTextBox = new TextBox();
            setLogFolderButton = new Button();
            Label headersLabel = new Label();
            headersDataGridView = new DataGridView();
            Label bodyLabel = new Label();
            bodyTextBox = new TextBox();

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            // Set Properties
            urlLabel.Text = "URL:";
            urlLabel.Location = new Point(10, 10);
            urlLabel.AutoSize = true;

            urlTextBox.Location = new Point(50, 10);
            urlTextBox.Size = new Size(400, 20);

            methodComboBox.Location = new Point(460, 10);
            methodComboBox.Size = new Size(100, 20);
            methodComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            headersLabel.Text = "Headers:";
            headersLabel.Location = new Point(10, 40);
            headersLabel.AutoSize = true;

            headersDataGridView.Location = new Point(10, 60);
            headersDataGridView.Size = new Size(550, 150);
            headersDataGridView.ColumnCount = 2;
            headersDataGridView.Columns[0].Name = "Header";
            headersDataGridView.Columns[1].Name = "Value";

            bodyLabel.Text = "Body:";
            bodyLabel.Location = new Point(10, 220);
            bodyLabel.AutoSize = true;

            bodyTextBox.Location = new Point(10, 240);
            bodyTextBox.Size = new Size(550, 100);
            bodyTextBox.Multiline = true;

            sendRequestButton.Text = "Send Request";
            sendRequestButton.Location = new Point(10, 350);
            sendRequestButton.Size = new Size(100, 30);

            copyResponseButton.Text = "Copy Response";
            copyResponseButton.Location = new Point(120, 350);
            copyResponseButton.Size = new Size(100, 30);
            copyResponseButton.Click += CopyResponseButton_Click;

            responseLabel.Text = "Response:";
            responseLabel.Location = new Point(10, 390);
            responseLabel.AutoSize = true;

            responseTextBox.Location = new Point(10, 410);
            responseTextBox.Size = new Size(550, 200);
            responseTextBox.Multiline = true;
            responseTextBox.ScrollBars = ScrollBars.Vertical;
            responseTextBox.ReadOnly = true;

            logFolderTextBox.Location = new Point(10, 620);
            logFolderTextBox.Size = new Size(400, 20);
            logFolderTextBox.Enabled = false;

            setLogFolderButton.Text = "Set Log Folder";
            setLogFolderButton.Location = new Point(420, 620);
            setLogFolderButton.Size = new Size(100, 30);
            setLogFolderButton.Click += SetLogFolderButton_Click;

            // Add Controls to Form
            Controls.Add(urlLabel);
            Controls.Add(urlTextBox);
            Controls.Add(methodComboBox);
            Controls.Add(sendRequestButton);
            Controls.Add(copyResponseButton);
            Controls.Add(responseLabel);
            Controls.Add(responseTextBox);
            Controls.Add(logFolderTextBox);
            Controls.Add(setLogFolderButton);
            Controls.Add(headersLabel);
            Controls.Add(headersDataGridView);
            Controls.Add(bodyLabel);
            Controls.Add(bodyTextBox);

            // Add Items to ComboBox
            methodComboBox.Items.Add("GET");
            methodComboBox.Items.Add("POST");
            methodComboBox.Items.Add("PUT");
            methodComboBox.Items.Add("DELETE");

            // Attach Event Handler
            sendRequestButton.Click += SendRequestButton_Click;
            EnableControls(false); // Initially disable the controls
        }

        private async void SendRequestButton_Click(object sender, EventArgs e)
        {
            string url = urlTextBox.Text;
            string method = methodComboBox.SelectedItem.ToString();
            string response = await SendRequestAsync(url, method);

            responseTextBox.Text = response;

            LogRequestResponse(url, method, response);
        }

        private async Task<string> SendRequestAsync(string url, string method)
        {
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest();

                switch (method)
                {
                    case "GET":
                        request.Method = Method.Get;
                        break;
                    case "POST":
                        request.Method = Method.Post;
                        break;
                    case "PUT":
                        request.Method = Method.Put;
                        break;
                    case "DELETE":
                        request.Method = Method.Delete;
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported HTTP method.");
                }

                foreach (DataGridViewRow row in headersDataGridView.Rows)
                {
                    if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                    {
                        request.AddHeader(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString());
                    }
                }

                if (method == "POST" || method == "PUT")
                {
                    request.AddJsonBody(bodyTextBox.Text);
                }

                var response = await client.ExecuteAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed)
                {
                    return "405 Method Not Allowed: The method is not allowed for the requested URL.";
                }
                return response.Content;
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }

        private void CopyResponseButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(responseTextBox.Text);
        }

        private void SetLogFolderButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    logFolder = dialog.SelectedPath;
                    logFolderTextBox.Text = logFolder;
                    EnableControls(true);
                }
            }
        }

        private void EnableControls(bool enabled)
        {
            urlTextBox.Enabled = enabled;
            methodComboBox.Enabled = enabled;
            headersDataGridView.Enabled = enabled;
            bodyTextBox.Enabled = enabled;
            sendRequestButton.Enabled = enabled;
            setLogFolderButton.Enabled = !enabled;
        }

        private void LogRequestResponse(string url, string method, string response)
        {
            if (string.IsNullOrEmpty(logFolder))
                return;

            string ipAddress = sessionType == "Local" ? "Local" : "Network";
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string logDir = Path.Combine(logFolder, date);

            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);

            string logFile = Path.Combine(logDir, $"log_{ipAddress}_{date}.json");

            var logEntry = new
            {
                IpAddress = ipAddress,
                Request = new { Url = url, Method = method },
                Response = response,
                Timestamp = DateTime.Now.ToString("o")
            };

            string logContent = JsonConvert.SerializeObject(logEntry, Formatting.Indented);
            File.AppendAllText(logFile, logContent + Environment.NewLine);
        }
    }
}
