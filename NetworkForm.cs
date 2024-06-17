using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;


namespace MultiReqHTTP
{
    public class NetworkForm : Form
    {
        private TextBox ipTextBox;
        private Button submitButton;
        private Button localSessionButton;

        public NetworkForm()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Create Controls
            Label ipLabel = new Label();
            ipTextBox = new TextBox();
            submitButton = new Button();
            localSessionButton = new Button();

            // Set Properties
            ipLabel.Text = "Enter Network IP:";
            ipLabel.Location = new Point(10, 10);
            ipLabel.AutoSize = true;

            ipTextBox.Location = new Point(10, 30);
            ipTextBox.Size = new Size(200, 20);

            submitButton.Text = "Submit";
            submitButton.Location = new Point(10, 60);
            submitButton.Size = new Size(100, 30);
            submitButton.Click += SubmitButton_Click;

            localSessionButton.Text = "Local Session";
            localSessionButton.Location = new Point(120, 60);
            localSessionButton.Size = new Size(100, 30);
            localSessionButton.Click += LocalSessionButton_Click;

            // Add Controls to Form
            Controls.Add(ipLabel);
            Controls.Add(ipTextBox);
            Controls.Add(submitButton);
            Controls.Add(localSessionButton);
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            string ipAddress = ipTextBox.Text;
            if (PingHost(ipAddress))
            {
                Form1 apiForm = new Form1(ipAddress);
                apiForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("IP address is not accessible. Please try again.");
                ipTextBox.Clear();
            }
        }

        private void LocalSessionButton_Click(object sender, EventArgs e)
        {
            Form1 apiForm = new Form1("Local");
            apiForm.Show();
            this.Hide();
        }

        private bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                pinger?.Dispose();
            }
            return pingable;
        }
    }
}

