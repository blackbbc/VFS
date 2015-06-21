using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualFileSystem.Core
{

    class SuperBlock
    {
        public int s_inodes_count;
        public int s_blocks_count;
        public int s_free_blocks_count;
        public int s_free_inodes_count;
        public int s_first_data_block;
        public int s_log_block_size;
        public int s_blocks_per_group;
        public int s_inodes_per_group;

        public long s_mtime;
        public long s_wtime;
        public int s_mnt_count;
        public int s_max_mnt_count;
        public String s_magic;

        public SuperBlock()
        {
            this.s_inodes_count = 10240; //inode总数
            this.s_blocks_count = 102400; //block总数
            this.s_free_inodes_count = 10240; //空闲inodes数
            this.s_free_blocks_count = 102400; //空闲block数
            this.s_first_data_block = 0;
            this.s_log_block_size = 1024; //块大小
            this.s_blocks_per_group = 100; //每个块组包含的block数
            this.s_inodes_per_group = 10; //每个块组包含的inode数

            DateTime timeStamp = new DateTime(1970, 1, 1);
            this.s_mtime = (DateTime.UtcNow.Ticks - timeStamp.Ticks) / 10000000;
            this.s_wtime = this.s_mtime;
            this.s_mnt_count = 1;
            this.s_max_mnt_count = 1024;
            this.s_magic = "EXT2";
        }


    }
}
