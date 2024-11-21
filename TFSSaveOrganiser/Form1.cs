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
        string selectedSlot = "2";

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

        private void SelectSaveFolder(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                ShowNewFolderButton = false,
                RootFolder = Environment.SpecialFolder.MyDocuments
            };
            DialogResult dr = fbd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                folderpath = fbd.SelectedPath;
            }

            if (folderpath != "")
            {
                bool validFolder = false;
                foreach (string s in System.IO.Directory.GetFiles(folderpath, "*.*", SearchOption.AllDirectories))
                {
                    if (RegularExpressions.Regex.IsMatch(s, @"SaveSlot[1-3]\\lastSavedFolder\.txt"))
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
                    MessageBox.Show("Not a valid TLC save folder. Make sure to select the root folder and that contains slot folders.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            fbd.Dispose();
        }

        private void OpenProfileForm(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = -1;
            var form2 = new Form2();
            form2.Location = Control.MousePosition;
            form2.ShowDialog();
            form2.Dispose();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            string[] profiles = System.IO.Directory.GetDirectories(System.IO.Path.Combine(Application.StartupPath, "Profiles"));
            foreach (string profile in profiles)
            {
                string pr = profile.Replace(System.IO.Path.Combine(Application.StartupPath, "Profiles") + @"\", "");
                comboBox1.Items.Add(pr);
            }
            if(!System.IO.Directory.Exists(System.IO.Path.Combine(System.IO.Path.Combine(Application.StartupPath, "Profiles"),comboBox1.Text))){
                comboBox1.Text = "";
            }
            listBox1.SelectedIndex = -1;
            listBox1.Items.Clear();
            if (comboBox1.Text != "")
            {
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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string profilesPath = System.IO.Path.Combine(Application.StartupPath, "Profiles");

            if (!System.IO.Directory.Exists(profilesPath)){
                System.IO.Directory.CreateDirectory(profilesPath);
            }
        }

        private void OpenSaveForm(object sender, EventArgs e)
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

        private void HandleProfileChange(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = -1;
            listBox1.Items.Clear();
            if (comboBox1.Text != "")
            {
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

        private void CorrectSaveSlotNames(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            string popSavePattern = @"PopSaveGameSlot[1-3]Index0\.AlkSave";
            string progressionPattern = @"ProgressionSlot[1-3]Index0\.AlkContent";

            string[] files = Directory.GetFiles(directoryPath);

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                if (Regex.IsMatch(fileName, popSavePattern))
                {
                    string newFileName = Regex.Replace(
                        fileName,
                        @"(?<=PopSaveGameSlot)[1-3](?=Index0\.AlkSave)",
                        selectedSlot
                    );
                    string newFilePath = Path.Combine(directoryPath, newFileName);
                    File.Move(file, newFilePath);
                }
                else if (Regex.IsMatch(fileName, progressionPattern))
                {
                    string newFileName = Regex.Replace(
                        fileName,
                        @"(?<=ProgressionSlot)[1-3](?=Index0\.AlkContent)",
                        selectedSlot
                    );
                    string newFilePath = Path.Combine(directoryPath, newFileName);
                    File.Move(file, newFilePath);
                }
            }
        }

        private void LoadSave(object sender, EventArgs e)
        {
            if (listBox1.Text != "")
            {
                string savePath = Path.Combine(Application.StartupPath, "Profiles", comboBox1.Text, listBox1.Text);
                CorrectSaveSlotNames(savePath);
                string saveSlotPath = Path.Combine(textBox1.Text, $"SaveSlot{selectedSlot}");
                string lastSave = System.IO.File.ReadAllText(Path.Combine(saveSlotPath, "lastSavedFolder.txt"));
                string lastSavePath = Path.Combine(saveSlotPath, $"Save_{lastSave}");
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
            bool itemSelected = (listBox1.SelectedIndex != -1);
            addImageToolStripMenuItem.Enabled = itemSelected;
            renameToolStripMenuItem.Enabled = itemSelected;
            deleteToolStripMenuItem.Enabled = itemSelected;
            openInExplorerToolStripMenuItem.Enabled = itemSelected;
        }

        private void ReplaceSave(object sender, EventArgs e)
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

        private void HandleImageChange(object sender, EventArgs e)
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

        private void FreezeSave(object sender, EventArgs e)
        {
            button6.Enabled = false;
            if (button6.Text == "Freeze Save")
            {
                if (listBox1.Text != "")
                {
                    button4.Enabled = false;
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
                button4.Enabled = true;
                button6.Text = "Freeze Save";
                backgroundWorker1.CancelAsync();
            } 
            button6.Enabled = true;
        }

        private void FreezeSaveWorker(object sender, DoWorkEventArgs e)
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

        private void OpenSaveInExplorer(object sender, EventArgs e)
        {
            string savePath = System.IO.Path.Combine(Application.StartupPath, "Profiles");
            savePath = System.IO.Path.Combine(System.IO.Path.Combine(savePath, comboBox1.Text), listBox1.Text);
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
            {
                Arguments = savePath,
                FileName = "explorer.exe"
            };

            try
            {
                System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
