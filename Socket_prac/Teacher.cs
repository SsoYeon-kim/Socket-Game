using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Teacher : Human
    {
        public Teacher()
        {
            hp = this.CommonHp();
        }

        public override int CommonHp()
        {
            return hp = 60;
        }

        public string Teaching()
        {
            return "수업중";
        }

        public string Angry()
        {
            return "집중해!!!";
        }

        public string Bump()
        {
            return "뭐하냐!!!!";
        }
    }
}

