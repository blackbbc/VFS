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
using System.Collections;

using VirtualFileSystem.Core;

namespace VirtualFileSystem
{

    public partial class Form1 : Form
    {
        private Directory currentDir;

        //剪贴板
        private Entry sharedEntry;

        private ArrayList history;
        private int pointer;

        private bool isCut = false;

        private void InitialVFS()
        {
            if (System.IO.File.Exists("./vfs.bin"))
                //System.Console.WriteLine("文件存在");
                Utils.DeSerializeNow();
            else
                //System.Console.WriteLine("文件不存在");
                //初始化
                debugMode();
        }

        //关闭窗口
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Utils.SerializeNow();
        }
        public void debugMode()
        {
            //初始化全局块组
            for (int i = 0; i < Config.GROUPS; i++)
                VFS.BLOCK_GROUPS[i] = new BlockGroup(i);

            //初始化目录树
            VFS.rootDir = new Directory("/", null);

            Directory bootDir = new Directory("boot", VFS.rootDir);
            Directory etcDir = new Directory("etc", VFS.rootDir);
            Directory libDir = new Directory("lib", VFS.rootDir);
            Directory homeDir = new Directory("home", VFS.rootDir);
            Directory rootDir = new Directory("root", VFS.rootDir);
            Directory tempDir = new Directory("temp", VFS.rootDir);
            VFS.rootDir.add(bootDir);
            VFS.rootDir.add(etcDir);
            VFS.rootDir.add(homeDir);
            VFS.rootDir.add(libDir);
            VFS.rootDir.add(rootDir);
            VFS.rootDir.add(tempDir);

            File file1 = new File("bashrc", etcDir);
            File file2 = new File("shadowsocks", etcDir);
            etcDir.add(file1);
            etcDir.add(file2);
        }

        private void PopulateTreeView()
        {
            TreeNode rootNode = VFS.rootDir.getTreeNode();

            treeView1.Nodes.Add(rootNode);
        }

        private void enterDirectory(Directory dir, bool flag = true)
        {
            currentDir = dir;
            listView1.Items.Clear();

            List<Entry> entries = dir.getEntries();

            foreach (Entry entry in entries)
            {
                listView1.Items.Add(entry.getListViewItem());
            }

            if (flag)
            {
                //修改历史表
                for (int i = history.Count - 1; i > pointer; i--)
                    history.RemoveAt(i);

                pointer++;
                history.Add(dir);
            }

            imageButton1.Enabled = true;
            imageButton2.Enabled = true;
            if (pointer <= 0)
                imageButton1.Enabled = false;

            if (pointer >= history.Count - 1)
                imageButton2.Enabled = false;

            comboBox1.Text = currentDir.getPath();

            UpdateGlobalStatus();
        }
        private bool IsGlassEnabled()
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                MessageBox.Show(
                  "How about trying this on Vista?");
                return false;
            }

            //Check if DWM is enabled
            bool isGlassSupported = false;
            VistaApi.DwmIsCompositionEnabled(ref isGlassSupported);
            return isGlassSupported;
        }

        public Form1()
        {
            InitializeComponent();

            InitialVFS();

            if (!this.IsGlassEnabled())
                return;

            VistaApi.Margins marg;

            marg.Top = panel1.Height; // extend from the top
            marg.Left = 0;  // not used in this sample but could be
            marg.Right = 0; // not used in this sample but could be
            marg.Bottom = 0;// not used in this sample but could be

            // call the function that extends the sides, 
            // passing a reference to our inset Margins
            VistaApi.DwmExtendFrameIntoClientArea(this.Handle, ref marg);   

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.currentDir = VFS.rootDir;
            PopulateTreeView();

            history = new ArrayList();
            pointer = -1;

            enterDirectory(currentDir);

            Color c = Color.FromArgb(255, 221, 220, 220);
            this.TransparencyKey = c;
            panel1.BackColor = c;
        }




        private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {

            if (e.Label == null)
                return;

            if (e.Label == "")
            {
                e.CancelEdit = true;
                MessageBox.Show("必须键入文件名", "重命名");
                return;
            }

            //检查文件重名
            List<Entry> entries = currentDir.getEntries();

            foreach (Entry entry in entries)
                if (entry.getName() == e.Label)
                {
                    e.CancelEdit = true;
                    MessageBox.Show("文件名重复", "重命名");
                    return;
                }

            ListViewItem selectedItem = listView1.Items[e.Item];

            Entry selectedEntry = (Entry)selectedItem.Tag;
            selectedEntry.setName(e.Label);

            //刷新listview
            enterDirectory(currentDir, false);

            //刷新treeview
            if (selectedEntry.getType() == "文件夹")
            {
                Directory selectedDir = (Directory)selectedEntry;
                selectedDir.getLinkedTreeNode().Text = e.Label;
            }
        }

        public void OnOpen()
        {
            Entry selectedEntry = (Entry)listView1.SelectedItems[0].Tag;

            if (selectedEntry.getType() == "文本文件")
            {
                //打开编辑器
                File file = (File)selectedEntry;
                TextEditor textEditor = new TextEditor(file);
                textEditor.ShowDialog();
                enterDirectory(currentDir, false);
            }
            else
            {
                //进入目录
                Directory directory = (Directory)selectedEntry;
                enterDirectory(directory);

            }
        }

        //打开
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnOpen();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            OnOpen();
        }

        //设置快捷键
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
                enterDirectory(currentDir, false);
            else if (keyData == Keys.F2)
                OnRename();
            else if (keyData == Keys.Delete)
                OnDelete();
            else if (keyData == (Keys.Control | Keys.X))
                OnCut();
            else if (keyData == (Keys.Control | Keys.C))
                OnCopy();
            else if (keyData == (Keys.Control | Keys.V))
                OnPaste();
            return base.ProcessCmdKey(ref msg, keyData);
        }

        //刷新
        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enterDirectory(currentDir, false);
        }


        //右键菜单
        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            bool match = false;

            if (e.Button == MouseButtons.Right)
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.Bounds.Contains(e.Location))
                    {
                        match = true;
                        break;
                    }
                }
                if (match)
                {
                    contextMenuStrip2.Show(Cursor.Position);
                }
                else
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        //新建文件夹
        public void OnNewFolder()
        {
            String newName = Utils.getLegalNewName("新建文件夹", currentDir);
            Directory newDir = new Directory(newName, currentDir);
            currentDir.add(newDir);

            //刷新listview
            enterDirectory(currentDir, false);

            //刷新treeview
            currentDir.getLinkedTreeNode().Nodes.Add(newDir.getTreeNode());

            ListViewItem newItem = null;
            foreach (ListViewItem item in listView1.Items)
                if (item.Text == newDir.getName())
                {
                    newItem = item;
                    break;
                }

            if (newItem != null)
                newItem.BeginEdit();
        }

        //新建文件夹
        private void 文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnNewFolder();
        }

        private void OnNewFile()
        {
            String newName = Utils.getLegalNewName("新建文本文件", currentDir);
            File newFile = new File(newName, currentDir);
            currentDir.add(newFile);

            //刷新listview
            enterDirectory(currentDir, false);

            ListViewItem newItem = null;
            foreach (ListViewItem item in listView1.Items)
                if (item.Text == newFile.getName())
                {
                    newItem = item;
                    break;
                }

            if (newItem != null)
                newItem.BeginEdit();
        }

        //新建文本文件
        private void 文本文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnNewFile();
        }

        private void OnRename()
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            ListViewItem selectedItem = (ListViewItem)listView1.SelectedItems[0];
            selectedItem.BeginEdit();
        }

        //重命名
        private void 重命名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnRename();
        }

        private void OnSort(int column)
        {
            int index = 0;
            foreach (ToolStripMenuItem item in (contextMenuStrip1.Items[0] as ToolStripDropDownItem).DropDownItems)
            {
                if (index == column)
                    item.Checked = true;
                else
                    item.Checked = false;
                index++;
            }
            listView1.ListViewItemSorter = new ListViewItemComparer(column);
        }
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            OnSort(e.Column);
        }

        //按名称排序
        private void 名称ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnSort(0);
        }

        //按修改日期排序
        private void 修改日期ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnSort(1);
        }

        //按修改日期排序
        private void 类型ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnSort(2);
        }

        //按大小排序
        private void 大小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnSort(3);
        }

        private void OnDelete()
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            Entry deletedEntry = listView1.SelectedItems[0].Tag as Entry;
            var result = MessageBox.Show("确定要删除"+deletedEntry.getName()+"吗", "删除"+deletedEntry.getType(), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                //删除硬盘数据
                deletedEntry.deleteData();

                //删除目录树数据
                currentDir.deleteEntry(deletedEntry);
                if (deletedEntry.getType() == "文件夹")
                {
                    //刷新TreeNode
                    Directory deletedDir = deletedEntry as Directory;
                    TreeNode deletedTreeNode = deletedDir.getLinkedTreeNode();
                    deletedTreeNode.Parent.Nodes.Remove(deletedTreeNode);
                }

                enterDirectory(currentDir, false);
            }
        }

        //删除
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnDelete();
        }

        public void OnCopy()
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            Entry copiedEntry = listView1.SelectedItems[0].Tag as Entry;
            sharedEntry = copiedEntry;

            isCut = false;

            粘贴ToolStripMenuItem.Enabled = true;

        }

        //复制
        private void 复制ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OnCopy();
        }


        public void OnCut()
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            OnCopy();

            //从目录树中删除
            sharedEntry.getParent().deleteEntry(sharedEntry);
            isCut = true;

            enterDirectory(currentDir, false);
        }

        //剪切
        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnCut();
        }

        public void OnPaste()
        {
            if (!粘贴ToolStripMenuItem.Enabled)
                return;

            //处理剪切
            if (isCut)
            {
                isCut = false;
                粘贴ToolStripMenuItem.Enabled = false;

                currentDir.add(sharedEntry);

                enterDirectory(currentDir, false);

                return;
            }

            //处理复制
            String newName = Utils.getLegalCopyName(sharedEntry.getName(), currentDir);
            if (sharedEntry.getType() == "文本文件")
            {
                File newFile = new File(newName, sharedEntry as File, currentDir);
                currentDir.add(newFile);

                enterDirectory(currentDir, false);
            }
            else
            {
                //复制文件夹，递归复制，Fuck!
                Directory newDir = new Directory(newName, sharedEntry as Directory, currentDir);
                currentDir.add(newDir);

                //刷新treeview
                currentDir.getLinkedTreeNode().Nodes.Add(newDir.getTreeNode());

                enterDirectory(currentDir, false);
            }

        }

        //粘贴
        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnPaste();
        }

        //后退
        private void imageButton1_Click(object sender, EventArgs e)
        {
            pointer--;
            currentDir = history[pointer] as Directory;
            if (pointer == 0)
                imageButton1.Enabled = false;
            enterDirectory(currentDir, false);
        }


        //前进
        private void imageButton2_Click(object sender, EventArgs e)
        {
            pointer++;
            currentDir = history[pointer] as Directory;
            if (pointer == history.Count - 1)
                imageButton2.Enabled = false;
            enterDirectory(currentDir, false);

        }

        private void HideStatus()
        {
            label1.Hide();
            label2.Hide();
            label3.Hide();
            label4.Hide();
            label5.Hide();
            label6.Hide();
            label7.Hide();
            label8.Hide();
            label10.Hide();
            progressBar1.Hide();
            pictureBox1.Hide();
        }

        //设置Entry状态栏信息
        private void UpdateEntryStatus()
        {
            HideStatus();

            label1.Show();
            label2.Show();
            label3.Text = "修改日期";
            label3.Show();
            label6.Show();
            pictureBox1.Show();

            Entry selectedEntry = listView1.SelectedItems[0].Tag as Entry;
            label1.Text = selectedEntry.getName();
            label2.Text = selectedEntry.getType();
            label6.Text = selectedEntry.getModifiedTime();

            if (selectedEntry.getType() == "文本文件")
            {
                label4.Text = "大小";
                label4.Show();
                label5.Show();
                label8.Show();
                label10.Show();

                label5.Text = selectedEntry.getSize();
                label8.Text = (selectedEntry as File).getCreatedTime();

                //设置图片
                pictureBox1.Image = Properties.Resources.fileB;

            }
            else
            {
                label4.Text = "大小";
                label4.Show();

                label5.Text = selectedEntry.getSize();
                label5.Show();
                //设置图片
                pictureBox1.Image = Properties.Resources.folderB;
            }

        }

        private void UpdateGlobalStatus()
        {
            HideStatus();

            label1.Text = "虚拟文件系统";
            label1.Show();
            label2.Text = "总大小";
            label2.Show();
            label3.Text = "已用空间";
            label3.Show();
            label4.Text = "可用空间";
            label4.Show();

            //设置总大小1024MB
            label7.Show();
            label7.Text = Config.GROUPS.ToString() + "MB";

            //获取超级块
            SuperBlock superBlock = VFS.BLOCK_GROUPS[0].super_block;

            //设置可用空间
            label5.Show();
            long free_blocks = superBlock.s_free_blocks_count;
            free_blocks /= 1024;
            label5.Text = free_blocks.ToString() + "MB";

            //设置已用空间
            progressBar1.Show();
            long used_blocks = superBlock.s_blocks_count - superBlock.s_free_blocks_count;
            float percent = used_blocks / superBlock.s_blocks_count;
            int step = (int)(percent * 100);
            progressBar1.Value = step;

            pictureBox1.Image = Properties.Resources.disk;
            pictureBox1.Show();

        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
                UpdateEntryStatus();
            else
                UpdateGlobalStatus();
        }

        public void OnSearch(String name)
        {
            ArrayList searchResultList = currentDir.search(name);
            listView1.Items.Clear();

            Directory tempDir = new Directory("搜索结果", currentDir);


            foreach (Entry entry in searchResultList)
            {
                tempDir.add(entry);
                listView1.Items.Add(entry.getListViewItem());
            }

            //修改历史表
            for (int i = history.Count - 1; i > pointer; i--)
                history.RemoveAt(i);

            pointer++;
            history.Add(tempDir);

            imageButton1.Enabled = true;
            imageButton2.Enabled = true;

            if (pointer <= 0)
                imageButton1.Enabled = false;

            if (pointer >= history.Count - 1)
                imageButton2.Enabled = false;

            comboBox1.Text = "搜索结果";
        }

        private void comboBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                if (comboBox2.Text != null && comboBox2.Text != "")
                    OnSearch(comboBox2.Text);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = e.Node;

            if (selectedNode.IsSelected)
            {
                Directory selectedDir = (Directory)selectedNode.Tag;
                enterDirectory(selectedDir);
            }
        }

        //格式化磁盘
        private void 格式化磁盘ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("您确定要格式化磁盘吗？", "格式化", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                VFS.format();

                //刷新treeview
                TreeNode rootNode = VFS.rootDir.getTreeNode();
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(rootNode);

                //刷新history
                history.Clear();


                enterDirectory(VFS.rootDir);
            }
        }

    }
}
