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
    public partial class TextEditor : Form
    {
        private File file;

        public TextEditor()
        {
            InitializeComponent();
        }

        public TextEditor(File file)
        {
            InitializeComponent();


            this.file = file;
        }
    }
}
