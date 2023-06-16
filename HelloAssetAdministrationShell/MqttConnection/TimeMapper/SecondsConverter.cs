using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAssetAdministrationShell.MqttConnection.TimeMapper
{
    public class SecondsConverter
    {
        public int ConverCurrenthourstosecond(string currentTime)
        {
            string ti = currentTime;
           
            Console.WriteLine(ti);

            string[] parts = ti.Split(":");
            Console.WriteLine(parts[0]);
            Console.WriteLine(parts[1]);
            Console.WriteLine(parts[2]);

            int hh = int.Parse(parts[0]);
            int mm = int.Parse(parts[1]);
            int ss = int.Parse(parts[2]);

            int totalseconds = hh * 3600 + mm * 60 + ss;
            return totalseconds;


        }

        public string incrementedtimeformatter(int time)
         {
            int incrementedtime = time + 1;
            int hh = incrementedtime / 3600;
            int mm = (incrementedtime % 60) / 60;
            int ss = incrementedtime % 60;

            return $"{hh:D4}:{mm:D2}:{ss:D2}";

         }
    }
}
