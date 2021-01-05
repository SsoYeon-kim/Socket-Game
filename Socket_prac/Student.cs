using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    abstract class Student:Human
    {
        public int badPoint;
        public int goodPoint;

        public Student()
        {
            hp = this.CommonHp();
            badPoint = 0;
            goodPoint = 0;
        }

        public override int CommonHp()
        {
            return hp = 40;
        }

        public void CalHp(int bp, int gp)
        {
            if (this.hp >= 0)
            {
                if(this.badPoint >= 3)
                {
                    while (true)
                    {
                        this.hp -= 5;
                        this.badPoint -= 3;

                        if (this.badPoint < 3.1) break;

                    }
                }

                if (this.goodPoint >= 4)
                {
                    while (true)
                    {
                        this.hp += 3;
                        this.goodPoint -= 4;
                        
                        if (this.goodPoint < 4.1) break;
                    }
                }

            }

            
        }

        public abstract int Sleep();

    }
}
