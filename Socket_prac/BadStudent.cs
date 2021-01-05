using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class BadStudent:Student, IB
    {
        public bool flag = true;

        public override int Sleep()
        {
            if (flag)
            {
                badPoint = 5;

                return badPoint;
            }
            else
            {
                goodPoint = 6;
                return goodPoint;
            }
        }

        public int Dance()
        {
            if (flag)
            {
                badPoint = 7;
                return badPoint;
            }
            else
            {
                goodPoint = 8;
                return goodPoint;
            }
        }
    }
}
