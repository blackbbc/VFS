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


        //发送消息依赖-------------------------------------------------------------
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwnd, uint wMsg, int wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint RegisterWindowMessage(string lpString);
        uint MSG_SHOW = RegisterWindowMessage("TextEditor Closed");

        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        //发送消息依赖------------------------------------------------------------

        public TextEditor()
        {
            InitializeComponent();
        }

        public TextEditor(File file)
        {
            InitializeComponent();

            this.file = file;
        }
        private void TextEditor_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = file.getContent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                String content = richTextBox1.Text;
                file.save(content);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void 保存SToolStripButton_Click(object sender, EventArgs e)
        {
            String content = richTextBox1.Text;
            file.save(content);
        }

        //关闭窗口
        private void TextEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            IntPtr form_name = FindWindow(null, "Form1");//找B的IntPtr 用來代表指標或控制代碼

            if (form_name != IntPtr.Zero)
            {
                try
                {
                    int iNum = 1000;
                    SendMessage(form_name, MSG_SHOW, iNum, IntPtr.Zero);
                }
                catch (Exception)
                {

                }

            }

        }


    }
}
