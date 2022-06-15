using System;
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

        public Form3()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "")
            {
                MessageBox.Show("Profile Name cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (this.Text == "Create Profile")
                {
                    try
                    {
                        string folderPath = System.IO.Path.Combine(Application.StartupPath, "Profiles");
                        folderPath = System.IO.Path.Combine(folderPath, textBox1.Text);
                        if (!System.IO.Directory.Exists(folderPath))
                        {
                            System.IO.Directory.CreateDirectory(folderPath);
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
                else if (this.Text == "Edit Profile")
                {
                    string folderPath = System.IO.Path.Combine(Application.StartupPath, "Profiles");
                    string fromPath = System.IO.Path.Combine(folderPath, profileName);
                    string toPath = System.IO.Path.Combine(folderPath, textBox1.Text);
                    if (!System.IO.Directory.Exists(toPath))
                    {
                        try
                        {
                            System.IO.Directory.Move(fromPath, toPath);
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
                else if (this.Text == "Create Save")
                {
                    try
                    {
                        string toPath = System.IO.Path.Combine(Application.StartupPath, "Profiles");
                        toPath = System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(toPath, profileName), textBox1.Text),"11");
                        if (!System.IO.Directory.Exists(toPath))
                        {
                            CopyDirectory(savePath, toPath, true);
                            System.IO.File.Delete(System.IO.Path.Combine(toPath, "1.save"));
                            System.IO.File.Delete(System.IO.Path.Combine(toPath, "1.save.upload"));
                            System.IO.File.Delete(System.IO.Path.Combine(System.IO.Path.Combine(toPath, "uplay_backup"),"1.save"));
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
                else if (this.Text == "Edit Save")
                {
                    string folderPath = System.IO.Path.Combine(Application.StartupPath, "Profiles");
                    string fromPath = System.IO.Path.Combine(System.IO.Path.Combine(folderPath, profileName),savePath);
                    string toPath = System.IO.Path.Combine(System.IO.Path.Combine(folderPath, profileName), textBox1.Text);
                    if (!System.IO.Directory.Exists(toPath))
                    {
                        try
                        {
                            System.IO.Directory.Move(fromPath, toPath);
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(fromPath+"\n"+toPath+"\n"+ex.ToString()+"Save name contains speacial characters or reserved keywords which cannot be a windows folder name! Try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Another Save of the same name already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
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
