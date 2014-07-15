using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Employee_Manager.Classes
{
    class ADTimeConvert
    {

        public Boolean[] DecodeTime(int hours)
        {
            Boolean[] dayHour = new Boolean[8];
            dayHour[0] = true;

            if (hours == 255)
            {
                dayHour[0] = true;
                dayHour[1] = true;
                dayHour[2] = true;
                dayHour[3] = true;
                dayHour[4] = true;
                dayHour[5] = true;
                dayHour[6] = true;
                dayHour[7] = true;
            }
            else if (hours == 254)
            {
                dayHour[0] = false;
                dayHour[1] = true;
                dayHour[2] = true;
                dayHour[3] = true;
                dayHour[4] = true;
                dayHour[5] = true;
                dayHour[6] = true;
                dayHour[7] = true;
            }
            else if (hours == 252)
            {
                dayHour[0] = false;
                dayHour[1] = false;
                dayHour[2] = true;
                dayHour[3] = true;
                dayHour[4] = true;
                dayHour[5] = true;
                dayHour[6] = true;
                dayHour[7] = true;
            }
            else if (hours == 248)
            {
                dayHour[0] = false;
                dayHour[1] = false;
                dayHour[2] = false;
                dayHour[3] = true;
                dayHour[4] = true;
                dayHour[5] = true;
                dayHour[6] = true;
                dayHour[7] = true;
            }
            else if (hours == 240)
            {
                dayHour[0] = false;
                dayHour[1] = false;
                dayHour[2] = false;
                dayHour[3] = false;
                dayHour[4] = true;
                dayHour[5] = true;
                dayHour[6] = true;
                dayHour[7] = true;
            }
            else if (hours == 224)
            {
                dayHour[0] = false;
                dayHour[1] = false;
                dayHour[2] = false;
                dayHour[3] = false;
                dayHour[4] = false;
                dayHour[5] = true;
                dayHour[6] = true;
                dayHour[7] = true;
            }
            else if (hours == 192)
            {
                dayHour[0] = false;
                dayHour[1] = false;
                dayHour[2] = false;
                dayHour[3] = false;
                dayHour[4] = false;
                dayHour[5] = false;
                dayHour[6] = true;
                dayHour[7] = true;
            }
            else if (hours == 128)
            {
                dayHour[0] = false;
                dayHour[1] = false;
                dayHour[2] = false;
                dayHour[3] = false;
                dayHour[4] = false;
                dayHour[5] = false;
                dayHour[6] = false;
                dayHour[7] = true;
            }
            else if (hours == 127)
            {
                dayHour[0] = true;
                dayHour[1] = true;
                dayHour[2] = true;
                dayHour[3] = true;
                dayHour[4] = true;
                dayHour[5] = true;
                dayHour[6] = true;
                dayHour[7] = false;
            }
            else if (hours == 64)
            {
                dayHour[0] = false;
                dayHour[1] = false;
                dayHour[2] = false;
                dayHour[3] = false;
                dayHour[4] = false;
                dayHour[5] = false;
                dayHour[6] = true;
                dayHour[7] = false;
            }
            else if (hours == 63)
            {
                dayHour[0] = true;
                dayHour[1] = true;
                dayHour[2] = true;
                dayHour[3] = true;
                dayHour[4] = true;
                dayHour[5] = true;
                dayHour[6] = false;
                dayHour[7] = false;
            }
            else if (hours == 32)
            {
                dayHour[0] = false;
                dayHour[1] = false;
                dayHour[2] = false;
                dayHour[3] = false;
                dayHour[4] = false;
                dayHour[5] = true;
                dayHour[6] = false;
                dayHour[7] = false;
            }
            else if (hours == 31)
            {
                dayHour[0] = true;
                dayHour[1] = true;
                dayHour[2] = true;
                dayHour[3] = true;
                dayHour[4] = true;
                dayHour[5] = false;
                dayHour[6] = false;
                dayHour[7] = false;
            }
            else if (hours == 16)
            {
                dayHour[0] = false;
                dayHour[1] = false;
                dayHour[2] = false;
                dayHour[3] = false;
                dayHour[4] = true;
                dayHour[5] = false;
                dayHour[6] = false;
                dayHour[7] = false;
            }
            else if (hours == 15)
            {
                dayHour[0] = true;
                dayHour[1] = true;
                dayHour[2] = true;
                dayHour[3] = true;
                dayHour[4] = false;
                dayHour[5] = false;
                dayHour[6] = false;
                dayHour[7] = false;
            }
            else if (hours == 8)
            {
                dayHour[0] = false;
                dayHour[1] = false;
                dayHour[2] = false;
                dayHour[3] = true;
                dayHour[4] = false;
                dayHour[5] = false;
                dayHour[6] = false;
                dayHour[7] = false;
            }
            else if (hours == 7)
            {
                dayHour[0] = true;
                dayHour[1] = true;
                dayHour[2] = true;
                dayHour[3] = false;
                dayHour[4] = false;
                dayHour[5] = false;
                dayHour[6] = false;
                dayHour[7] = false;
            }
            else if (hours == 4)
            {
                dayHour[0] = false;
                dayHour[1] = false;
                dayHour[2] = true;
                dayHour[3] = false;
                dayHour[4] = false;
                dayHour[5] = false;
                dayHour[6] = false;
                dayHour[7] = false;
            }
            else if (hours == 3)
            {
                dayHour[0] = true;
                dayHour[1] = true;
                dayHour[2] = false;
                dayHour[3] = false;
                dayHour[4] = false;
                dayHour[5] = false;
                dayHour[6] = false;
                dayHour[7] = false;
            }
            else if (hours == 2)
            {
                dayHour[0] = false;
                dayHour[1] = true;
                dayHour[2] = false;
                dayHour[3] = false;
                dayHour[4] = false;
                dayHour[5] = false;
                dayHour[6] = false;
                dayHour[7] = false;
            }
            else if (hours == 1)
            {
                dayHour[0] = true;
                dayHour[1] = false;
                dayHour[2] = false;
                dayHour[3] = false;
                dayHour[4] = false;
                dayHour[5] = false;
                dayHour[6] = false;
                dayHour[7] = false;
            }
            else if (hours == 0)
            {
                dayHour[0] = false;
                dayHour[1] = false;
                dayHour[2] = false;
                dayHour[3] = false;
                dayHour[4] = false;
                dayHour[5] = false;
                dayHour[6] = false;
                dayHour[7] = false;
            }


            return dayHour;
        }
    }
}
