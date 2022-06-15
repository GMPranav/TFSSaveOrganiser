using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace TFSSaveOrganiser
{

    public partial class Form1 : Form
    {
        string folderpath = "";

        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new System.IO.DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new System.IO.DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            System.IO.DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            System.IO.Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (System.IO.FileInfo file in dir.GetFiles())
            {
                string targetFilePath = System.IO.Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (System.IO.DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = System.IO.Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            textBox1.Text = TFSSaveOrganiser.Properties.Settings.Default.SavePath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = false;
            fbd.RootFolder = Environment.SpecialFolder.ProgramFilesX86;
            DialogResult dr = fbd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                folderpath = fbd.SelectedPath;
            }

            if (folderpath != "")
            {
                bool validFolder = false;
                foreach (string s in System.IO.Directory.GetFiles(folderpath))
                {
                    if (s.Contains("4.save"))
                    {
                        validFolder = true;
                    }
                }
                if (validFolder)
                {
                    textBox1.Text = folderpath;
                    TFSSaveOrganiser.Properties.Settings.Default.SavePath = textBox1.Text;
                    TFSSaveOrganiser.Properties.Settings.Default.Save();
                }
                else
                {
                    MessageBox.Show("Not a valid TFS save folder. Make sure to select the \"11\" folder and it contains the \"4.save\" file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            fbd.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var form2 = new Form2();
            form2.Location = Control.MousePosition;
            form2.ShowDialog();
            form2.Dispose();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if(comboBox1.Text != "")
            {
                listBox1.SelectedIndex = -1;
                listBox1.Items.Clear();
                string profilesPath = System.IO.Path.Combine(Application.StartupPath, "Profiles");
                string savesPath = System.IO.Path.Combine(profilesPath, comboBox1.Text);
                string[] saves = System.IO.Directory.GetDirectories(savesPath);
                saves = saves.OrderBy(x => x.Length).ToArray();
                foreach (string save in saves)
                {
                    string sr = save.Replace(savesPath + @"\", "");
                    listBox1.Items.Add(sr);
                }
            }

            comboBox1.Items.Clear();
            string[] profiles = System.IO.Directory.GetDirectories(System.IO.Path.Combine(Application.StartupPath, "Profiles"));
            foreach (string profile in profiles)
            {
                string pr = profile.Replace(System.IO.Path.Combine(Application.StartupPath, "Profiles") + @"\", "");
                comboBox1.Items.Add(pr);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string profilesPath = System.IO.Path.Combine(Application.StartupPath, "Profiles");

            if (!System.IO.Directory.Exists(profilesPath)){
                System.IO.Directory.CreateDirectory(profilesPath);
            }

            toolTip1.SetToolTip(button3, "Imports the save from the game directory into the selected save organiser profile.");
            toolTip2.SetToolTip(button4, "Copies the selected save into the game directory.");
            toolTip3.SetToolTip(button5, "Replaces the selected save with the one from the game directory.");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != "")
            {
                if (comboBox1.Text != "")
                {
                    var form3 = new Form3();
                    form3.Text = "Create Save";
                    form3.label1.Text = "Save Name:";
                    form3.Location = Control.MousePosition;
                    form3.textBox1.Text = listBox1.Text;
                    form3.profileName = comboBox1.Text;
                    form3.savePath = textBox1.Text;
                    form3.ShowDialog();
                    form3.Dispose();
                }
                else
                {
                    MessageBox.Show("Select a profile before proceeding.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Select the save folder before proceeding.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = -1;
            listBox1.Items.Clear();
            string profilesPath = System.IO.Path.Combine(Application.StartupPath, "Profiles");
            string savesPath = System.IO.Path.Combine(profilesPath, comboBox1.Text);
            string[] saves = System.IO.Directory.GetDirectories(savesPath);
            saves = saves.OrderBy(x => x.Length).ToArray();
            foreach (string save in saves)
            {
                string sr = save.Replace(savesPath + @"\", "");
                listBox1.Items.Add(sr);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string profilesPath = System.IO.Path.Combine(Application.StartupPath, "Profiles");
            string savesPath = System.IO.Path.Combine(profilesPath, comboBox1.Text);
            string savePath = System.IO.Path.Combine(savesPath, listBox1.Text);
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete the save?", "Confirm Save Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    System.IO.Directory.Delete(savePath, true);
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
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form3 = new Form3();
            form3.Text = "Edit Save";
            form3.Location = Control.MousePosition;
            form3.label1.Text = "Save Name:";
            form3.profileName = comboBox1.Text;
            form3.textBox1.Text = listBox1.Text;
            form3.ShowDialog();
            form3.Dispose();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(listBox1.Text != "")
            {
                string savePath = System.IO.Path.Combine(Application.StartupPath, "Profiles");
                savePath = System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(savePath, comboBox1.Text), listBox1.Text),"11");
                try
                {
                    CopyDirectory(savePath, textBox1.Text, true);
                    MessageBox.Show("Save loaded successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Select a save to load before proceeding.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void contextMenuStrip1_Opened(object sender, EventArgs e)
        {
                addImageToolStripMenuItem.Enabled = (listBox1.SelectedIndex != -1);
                renameToolStripMenuItem.Enabled = (listBox1.SelectedIndex != -1);
                deleteToolStripMenuItem.Enabled = (listBox1.SelectedIndex != -1);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.Text != "")
            {
                string savePath = System.IO.Path.Combine(Application.StartupPath, "Profiles");
                savePath = System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(savePath, comboBox1.Text), listBox1.Text), "11");
                try
                {
                    CopyDirectory(textBox1.Text, savePath, true);
                    MessageBox.Show("Save replaced successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Select a save to replace before proceeding.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string savename = listBox1.Text;
            OpenFileDialog fbd = new OpenFileDialog();

            fbd.InitialDirectory = System.Environment.SpecialFolder.MyComputer.ToString();

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = string.Empty;

            foreach (var c in codecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                fbd.Filter = String.Format("{0}{1}{2} ({3})|{3}", fbd.Filter, sep, codecName, c.FilenameExtension);
                sep = "|";
            }

            fbd.Filter = String.Format("{0}{1}{2} ({3})|{3}", fbd.Filter, sep, "All Files", "*.*");

            fbd.DefaultExt = ".png"; // Default file extension 

            DialogResult dr = fbd.ShowDialog();

            if (dr == DialogResult.OK)
            {

                string imgPath = System.IO.Path.Combine(Application.StartupPath, "Profiles");
                imgPath = System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(imgPath, comboBox1.Text), savename), "image");
                if (System.IO.File.Exists(imgPath))
                {
                    pictureBox1.BackgroundImage = Properties.Resources.SaveImgBackground;
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                }
                System.IO.File.Copy(fbd.FileName, imgPath, true);
            }
            fbd.Dispose();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string imgPath = System.IO.Path.Combine(Application.StartupPath, "Profiles");
                imgPath = System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(imgPath, comboBox1.Text), listBox1.Text), "image");
                if (System.IO.File.Exists(imgPath))
                {
                    pictureBox1.BackgroundImage = null;
                    pictureBox1.Image = Image.FromFile(imgPath);
                }
                else
                {
                    pictureBox1.BackgroundImage = null;
                    pictureBox1.Image = Properties.Resources.NoSaveImg;
                }
            }
            else if (pictureBox1.Image != null)
            {
                pictureBox1.BackgroundImage = Properties.Resources.SaveImgBackground;
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button6.Enabled = false;
            if (button6.Text == "Freeze Save")
            {
                if (listBox1.Text != "")
                {
                    button6.Text = "Unfreeze Save";
                    backgroundWorker1.RunWorkerAsync();
                }
                else
                {
                    MessageBox.Show("Select a save to replace before proceeding.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                button6.Text = "Freeze Save";
                backgroundWorker1.CancelAsync();
            } 
            button6.Enabled = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string savePath = System.IO.Path.Combine(Application.StartupPath, "Profiles");
            comboBox1.Invoke(new MethodInvoker(delegate ()
            {
                savePath = System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(savePath, comboBox1.Text), listBox1.Text), "11");
            }));
            string fourHash = CalculateMD5(System.IO.Path.Combine(savePath, "4.save"));
            string fourPath = System.IO.Path.Combine(textBox1.Text, "4.save");
            while (button6.Text == "Unfreeze Save")
            {
                if (System.IO.File.Exists(fourPath))
                {
                    if (fourHash != CalculateMD5(fourPath))
                    {
                        CopyDirectory(savePath, textBox1.Text, true);
                    }
                }
                else
                {
                    CopyDirectory(savePath, textBox1.Text, true);
                }
            }
        }
    }
}
