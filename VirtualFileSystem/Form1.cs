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
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Block block1 = new Block();

            Block block2 = block1;

            block2.data[0] = 'a';

            MessageBox.Show(block2.data[1].ToString());

            
        }
    }
}
