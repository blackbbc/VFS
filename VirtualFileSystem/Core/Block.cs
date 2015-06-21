using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualFileSystem.Core
{
    class Block
    {
        public char[] data;
        public Block[] block;

        public Block() { }

        public Block(Boolean flag)
        {
            if (flag)
            {
                data = new char[1024];
            }
            else
            {
                block = new Block[512];
            }
        }
        
    }
}
