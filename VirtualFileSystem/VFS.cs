using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

using ProtoBuf;

using VirtualFileSystem.Core;

namespace VirtualFileSystem
{
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    class VFS
    {
        public static Directory rootDir;
        public static BlockGroup[] BLOCK_GROUPS = new BlockGroup[Config.GROUPS];

        public Directory rootDir_Serialize;
        public BlockGroup[] BLOCK_GROUPS_Serialize;

        public VFS()
        {
            rootDir_Serialize = rootDir;
            BLOCK_GROUPS_Serialize = BLOCK_GROUPS;
        }

        public void update()
        {
            rootDir = rootDir_Serialize;
            BLOCK_GROUPS = BLOCK_GROUPS_Serialize;
        }

        //获取空闲空间
        public static ArrayList getFreeBlocks(int num)
        {
            ArrayList result = new ArrayList();

            foreach (BlockGroup block_group in BLOCK_GROUPS)
                if (block_group.hasFreeBlock())
                {
                    ArrayList tempArr = block_group.getFreeBlocks();
                    result.AddRange(tempArr);
                    if (result.Count >= num )
                        return result;
                }

            return null;
        }

        //格式化
        public static void format()
        {
            //初始化全局块组
            for (int i = 0; i < Config.GROUPS; i++)
                VFS.BLOCK_GROUPS[i] = new BlockGroup(i);

            //初始化目录树
            VFS.rootDir = new Directory("/", null);

        }
    }
}
