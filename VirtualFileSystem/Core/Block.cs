using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using VirtualFileSystem;

namespace VirtualFileSystem.Core
{
    class Block
    {
        public char[] data;
        public Block[] block;
        private Boolean flag;

        public Block() { }

        public Block(Boolean flag)
        {
            this.flag = flag;
            if (flag)
            {
                data = new char[Config.BLOCK_SIZE];
            }
            else
            {
                block = new Block[Config.BLOCK_SIZE / 2];
            }
        }
        
    }
}
