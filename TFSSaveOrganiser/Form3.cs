using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace TFSSaveOrganiser
{
    public partial class Form3 : Form
    {
        public string profileName = "";
        public string savePath = "";

        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        public Form3()
        {
            InitializeComponent();
        }

        private void CloseForm(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DoRelevantOperation(object sender, EventArgs e)
        {
            if(textBox1.Text == "")
            {
                MessageBox.Show("Profile Name cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                switch(this.Text)
                {
                    case "Create Profile": CreateProfile(); break;
                    case "Edit Profile": EditProfile(); break;
                    case "Create Save": CreateSave(); break;
                    case "Edit Save": EditSave(); break;
                    
                }
            }
        }

        private void EditSave()
        {
            string folderPath = Path.Combine(Application.StartupPath, "Profiles");
            string fromPath = Path.Combine(Path.Combine(folderPath, profileName), savePath);
            string toPath = Path.Combine(Path.Combine(folderPath, profileName), textBox1.Text);
            if (!Directory.Exists(toPath))
            {
                try
                {
                    Directory.Move(fromPath, toPath);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(fromPath + "\n" + toPath + "\n" + ex.ToString() + "Save name contains speacial characters or reserved keywords which cannot be a windows folder name! Try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Another Save of the same name already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateSave()
        {
            try
            {
                string toPath = Path.Combine(Application.StartupPath, "Profiles");
                toPath = Path.Combine(Path.Combine(Path.Combine(toPath, profileName), textBox1.Text), "11");
                if (!Directory.Exists(toPath))
                {
                    CopyDirectory(savePath, toPath, true);
                    File.Delete(Path.Combine(toPath, "1.save"));
                    File.Delete(Path.Combine(toPath, "1.save.upload"));
                    File.Delete(Path.Combine(Path.Combine(toPath, "uplay_backup"), "1.save"));
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Another save of the same name already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Save name contains speacial characters or reserved keywords which cannot be a windows folder name! Try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditProfile()
        {
            string folderPath = Path.Combine(Application.StartupPath, "Profiles");
            string fromPath = Path.Combine(folderPath, profileName);
            string toPath = Path.Combine(folderPath, textBox1.Text);
            if (!Directory.Exists(toPath))
            {
                try
                {
                    Directory.Move(fromPath, toPath);
                    this.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Profile name contains speacial characters or reserved keywords which cannot be a windows folder name! Try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Another Profile of the same name already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateProfile()
        {
            try
            {
                string folderPath = Path.Combine(Application.StartupPath, "Profiles");
                folderPath = Path.Combine(folderPath, textBox1.Text);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Another Profile of the same name already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Profile name contains speacial characters or reserved keywords which cannot be a windows folder name! Try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            if (this.Text == "Edit Profile")
            {
                profileName = textBox1.Text;
            }
            else if (this.Text == "Edit Save")
            {
                savePath = textBox1.Text;
            }
        }
    }
}
