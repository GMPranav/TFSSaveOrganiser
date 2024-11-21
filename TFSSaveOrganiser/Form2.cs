using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace TFSSaveOrganiser
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void OpenCreateForm(object sender, EventArgs e)
        {
            var form3 = new Form3();
            form3.Text = "Create Profile";
            form3.Location = Control.MousePosition;
            form3.ShowDialog();
            form3.Dispose();
        }

        private void Form2_Enter(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string[] profiles = System.IO.Directory.GetDirectories(System.IO.Path.Combine(Application.StartupPath, "Profiles"));
            foreach (string profile in profiles)
            {
                string pr = profile.Replace(System.IO.Path.Combine(Application.StartupPath, "Profiles") + @"\", "");
                listBox1.Items.Add(pr);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex != -1)
            {
                button2.Enabled = true;
                button3.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
                button3.Enabled = false;
            }
        }

        private void OpenEditForm(object sender, EventArgs e)
        {
            var form3 = new Form3();
            form3.Text = "Edit Profile";
            form3.Location = Control.MousePosition;
            form3.textBox1.Text = listBox1.Text;
            form3.ShowDialog();
            button2.Enabled = false;
            button3.Enabled = false;
            form3.Dispose();
        }

        private void DeleteProfile(object sender, EventArgs e)
        {
            string folderPath = System.IO.Path.Combine(Application.StartupPath, "Profiles");
            folderPath = System.IO.Path.Combine(folderPath, listBox1.Text);
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete the Profile?", "Confirm Profile Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    System.IO.Directory.Delete(folderPath, true);
                    button2.Enabled = false;
                    button3.Enabled = false;
                    this.Enabled = false;
                    this.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (dialogResult == DialogResult.No)
            {
                button2.Enabled = false;
                button3.Enabled = false;
            }
        }

        private void DownloadCommunityProfile(object sender, EventArgs e)
        {
            button4.Enabled = false;
            progressBar1.Visible = true;

            int bytesProcessed = 0;
            string downloadLink = "https://github.com/GMPranav/TFSSaveOrganiser/raw/master/Any%25_Saves.zip";
            System.IO.Stream remoteStream = null;
            System.IO.Stream localStream = null;
            WebResponse response = null;
            string filePath = System.IO.Path.Combine(Application.StartupPath, "Profiles");
            filePath = System.IO.Path.Combine(filePath, "Any%_Saves.zip");
            string folderPath = System.IO.Path.Combine(Application.StartupPath, @"Profiles\Any%_Saves");
            
            if (!System.IO.Directory.Exists(folderPath))
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to download the community profile (70MB) ?", "Confirm Download", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        WebRequest request = WebRequest.Create(downloadLink);
                        if (request != null)
                        {
                            double totalBytesToRecieve = 0;
                            var sizeWebRequest = HttpWebRequest.Create(new Uri(downloadLink));
                            sizeWebRequest.Method = "HEAD";

                            using (var webResponse = sizeWebRequest.GetResponse())
                            {
                                var fileSize = webResponse.Headers.Get("Content-Length");
                                totalBytesToRecieve = Convert.ToDouble(fileSize);
                            }

                            response = request.GetResponse();
                            if (response != null)
                            {
                                remoteStream = response.GetResponseStream();
                                localStream = System.IO.File.Create(filePath);

                                byte[] buffer = new byte[1024];
                                int bytesRead = 0;

                                do
                                {
                                    bytesRead = remoteStream.Read(buffer, 0, buffer.Length);
                                    localStream.Write(buffer, 0, bytesRead);
                                    bytesProcessed += bytesRead;

                                    double bytesIn = double.Parse(bytesProcessed.ToString());
                                    double percentage = bytesIn / totalBytesToRecieve * 100;
                                    percentage = Math.Round(percentage, 0);

                                    progressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());
                                }
                                while (bytesRead > 0);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if (response != null) response.Close();
                        if (remoteStream != null) remoteStream.Close();
                        if (localStream != null) localStream.Close();
                    }

                    System.IO.Compression.ZipFile.ExtractToDirectory(filePath, folderPath);
                    System.IO.File.Delete(filePath);
                }
                else if (dialogResult == DialogResult.No)
                {
                    //Do Nothing
                }
            }
            else
            {
                MessageBox.Show("Community profile already downloaded.\nDelete or rename the existed profile named \"Any%_Saves\" if you want to download it again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            progressBar1.Visible = false;
            button4.Enabled = true;
            this.Enabled = false;
            this.Enabled = true;
        }
    }
}
