using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProtoBuf;

using VirtualFileSystem;

namespace VirtualFileSystem.Core
{

    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    class SuperBlock
    {
        public long s_inodes_count;
        public long s_blocks_count;
        public long s_free_blocks_count;
        public long s_free_inodes_count;
        public long s_first_data_block;
        public long s_log_block_size;
        public long s_blocks_per_group;
        public long s_inodes_per_group;

        public long s_mtime;
        public long s_wtime;
        public long s_mnt_count;
        public long s_max_mnt_count;
        public String s_magic;

        public SuperBlock()
        {
            this.s_inodes_count = Config.GROUPS * Config.INODES_PER_GROUP; //inode总数
            this.s_blocks_count = Config.GROUPS * Config.BLOCKS_PER_GROUP; //block总数
            this.s_free_inodes_count = this.s_inodes_count; //空闲inodes数
            this.s_free_blocks_count = this.s_blocks_count; //空闲block数
            this.s_first_data_block = 0;
            this.s_log_block_size = Config.BLOCK_SIZE; //块大小
            this.s_blocks_per_group = Config.BLOCKS_PER_GROUP; //每个块组包含的block数
            this.s_inodes_per_group = Config.INODES_PER_GROUP; //每个块组包含的inode数

            this.s_mtime = Utils.getUnixTimeStamp();
            this.s_wtime = this.s_mtime;
            this.s_mnt_count = 1;
            this.s_max_mnt_count = 1024;
            this.s_magic = "EXT2";
        }


    }
}
