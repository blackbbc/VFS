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

using VirtualFileSystem.Core;

namespace VirtualFileSystem
{

    public partial class Form1 : Form
    {
        private void PopulateTreeView()
        {
            TreeNode rootNode;
            TreeNode node1, node2, node3, node4, node5, node6;

            rootNode = new TreeNode("/");
            node1 = new TreeNode("bin");
            node1.ImageKey = "folder";
            node2 = new TreeNode("etc");
            node2.ImageKey = "folder";
            node3 = new TreeNode("lib");
            node3.ImageKey = "folder";
            node4 = new TreeNode("lib");
            node4.ImageKey = "folder";
            node5 = new TreeNode("lib");
            node5.ImageKey = "folder";
            node6 = new TreeNode("lib");
            node6.ImageKey = "folder";

            rootNode.Nodes.Add(node1);
            rootNode.Nodes.Add(node2);
            rootNode.Nodes.Add(node3);

            treeView1.Nodes.Add(rootNode);
        }

        public Form1()
        {
            InitializeComponent();
            //初始化全局块组
            for (int i = 0; i < Config.GROUPS; i++)
                VFS.BLOCK_GROUPS[i] = new BlockGroup(i);

            //初始化目录树

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateTreeView();

        }

        private void treeView1_NodeMouseClick_1(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            listView1.Items.Clear();

            ListViewItem directory = new ListViewItem("sweet", 0);
            ListViewItem file = new ListViewItem("1.txt", 1);
            ListViewItem.ListViewSubItem[] subItems;

            subItems = new ListViewItem.ListViewSubItem[]
            {
                new ListViewItem.ListViewSubItem(file, "Directory"),
                new ListViewItem.ListViewSubItem(file, "今天"),
                new ListViewItem.ListViewSubItem(file, "1kb")
            };

            directory.SubItems.AddRange(subItems);

            listView1.Items.Add(directory);
            listView1.Items.Add(file);


            //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize); //自动修改宽度

            //MessageBox.Show("Hello World");

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
