using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        private void button1_Click(object sender, EventArgs e)
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
            string[] profiles = System.IO.Directory.GetDirectories(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "TFSSaveOrganiser"));
            foreach (string profile in profiles)
            {
                string pr = profile.Replace(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "TFSSaveOrganiser")+ @"\", "");
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

        private void button2_Click(object sender, EventArgs e)
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

        private void button3_Click(object sender, EventArgs e)
        {
            string folderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "TFSSaveOrganiser");
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
        
    }
}
