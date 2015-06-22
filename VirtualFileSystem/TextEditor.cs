using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;

using VirtualFileSystem.Core;

namespace VirtualFileSystem
{
    public partial class TextEditor : Form
    {
        private File file;

        private bool isSave;


        public TextEditor()
        {
            InitializeComponent();
        }

        public TextEditor(File file)
        {
            InitializeComponent();

            this.isSave = true;
            this.file = file;
        }
        private void TextEditor_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = file.getContent();
        }

        public void OnSave()
        {
            isSave = true;
            String content = richTextBox1.Text;
            file.save(content);
        }

        //设置快捷键
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
                OnSave();

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void 保存SToolStripButton_Click(object sender, EventArgs e)
        {
            OnSave();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            isSave = false;
        }

        private void TextEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isSave)
            {
                var result = MessageBox.Show("是否保存"+file.getName(), "记事本", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    OnSave();
                }
                else if (result == DialogResult.No)
                {

                }
                else
                {
                    e.Cancel = true;
                }

            }
        }

    }
}
