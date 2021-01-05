using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Human
    {
        public int hp;

        public virtual int CommonHp()
        {
            return hp = 0;
        }
    }
}
