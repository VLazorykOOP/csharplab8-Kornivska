using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EduLinkFinder
{
    public partial class Form1 : Form
    {
        string loadedText = "";
        string[] foundLinks;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                loadedText = File.ReadAllText(ofd.FileName);
                richTextBox1.Text = loadedText;
                ExtractLinks();
            }
        }

        private void ExtractLinks()
        {
            string pattern = @"https?://[\w\-\.]+\.edu\.ua(/\S*)?";
            MatchCollection matches = Regex.Matches(loadedText, pattern);
            foundLinks = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
                foundLinks[i] = matches[i].Value;

            lblCount.Text = $"Знайдено {foundLinks.Length} посилань";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(sfd.FileName, foundLinks);
                MessageBox.Show("Збережено!");
            }
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReplace.Text)) return;
            foreach (string link in foundLinks)
                loadedText = loadedText.Replace(link, txtReplace.Text);

            richTextBox1.Text = loadedText;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (string link in foundLinks)
                loadedText = loadedText.Replace(link, "");

            richTextBox1.Text = loadedText;
        }
    }
}
