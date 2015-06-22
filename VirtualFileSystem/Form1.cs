using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.WindowsAPICodePack.Shell;
using System.Collections;

using VirtualFileSystem.Core;

namespace VirtualFileSystem
{

    public partial class Form1 : Form
    {
        private void InitialVFS()
        {
            //初始化全局块组
            for (int i = 0; i < Config.GROUPS; i++)
                VFS.BLOCK_GROUPS[i] = new BlockGroup(i);

            //初始化目录树
            VFS.rootDir = new Directory("/");

            Directory bootDir = new Directory("boot");
            Directory etcDir = new Directory("etc");
            Directory libDir = new Directory("lib");
            Directory homeDir = new Directory("home");
            Directory rootDir = new Directory("root");
            Directory tempDir = new Directory("temp");
            VFS.rootDir.add(bootDir);
            VFS.rootDir.add(etcDir);
            VFS.rootDir.add(libDir);
            VFS.rootDir.add(homeDir);
            VFS.rootDir.add(rootDir);
            VFS.rootDir.add(tempDir);

            File file1 = new File("bashrc");
            File file2 = new File("shadowsocks");
            etcDir.add(file1);
            etcDir.add(file2);
        }

        private void PopulateTreeView()
        {
            TreeNode rootNode = VFS.rootDir.getTreeNode();

            treeView1.Nodes.Add(rootNode);
        }

        public Form1()
        {
            InitializeComponent();

            InitialVFS();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateTreeView();
        }

        private void treeView1_NodeMouseClick_1(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode selectedNode = e.Node;
            listView1.Items.Clear();

            Directory selectedDir = (Directory)selectedNode.Tag;
            ArrayList entries = selectedDir.getEntries();

            foreach (Entry entry in entries)
            {
                listView1.Items.Add(entry.getListViewItem());
            }

            //ListViewItem directory = new ListViewItem("sweet", 0);
            //ListViewItem file = new ListViewItem("1.txt", 1);
            //ListViewItem.ListViewSubItem[] subItems;

            //subItems = new ListViewItem.ListViewSubItem[]
            //{
            //    new ListViewItem.ListViewSubItem(file, "Directory"),
            //    new ListViewItem.ListViewSubItem(file, "今天"),
            //    new ListViewItem.ListViewSubItem(file, "1kb")
            //};

            //directory.SubItems.AddRange(subItems);

            //listView1.Items.Add(directory);
            //listView1.Items.Add(file);


            //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize); //自动修改宽度

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TextEditor textEditor = new TextEditor();
            textEditor.Show();
        }

        private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            MessageBox.Show("修改成功！");
        }

    }
}
