using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

using VirtualFileSystem.Core;

namespace VirtualFileSystem
{
    class VFS
    {
        public static Directory rootDir;
        public static BlockGroup[] BLOCK_GROUPS = new BlockGroup[Config.GROUPS];

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
    }
}
