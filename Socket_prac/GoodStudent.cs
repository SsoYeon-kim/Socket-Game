using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class GoodStudent :Student, IG
    {
        public bool flag = true;

        public override int Sleep()
        {
            if (flag)
            {
                badPoint = 3;

                return badPoint;
            }
            else
            {
                goodPoint = 4;

                return goodPoint;
            }
        }

        public int Eat()
        {
            if (flag)
            {
                badPoint = 5;

                return badPoint;
            }
            else
            {
                goodPoint = 5;
                return goodPoint;
            }
        }
    }
}
