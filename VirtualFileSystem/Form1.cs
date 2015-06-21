using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using VirtualFileSystem.Core;

namespace VirtualFileSystem
{

    public partial class Form1 : Form
    {
        private File file;

        public Form1()
        {
            InitializeComponent();

            //初始化全局块组
            for (int i = 0; i < Config.GROUPS; i++)
                VFS.BLOCK_GROUPS[i] = new BlockGroup(i);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            file = new File("Hello");

            MessageBox.Show("创建成功");
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(file.getContent());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String content = textBox1.Text;

            file.save(content);

            MessageBox.Show(content);
        }
    }
}
