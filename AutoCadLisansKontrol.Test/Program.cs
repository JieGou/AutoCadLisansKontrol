using AutoCadLisansKontrol.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCadLisansKontrol.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            CheckAutoCadLicense license = new CheckAutoCadLicense();
            var list = license.GetComputerInfoOnNetwork();

            foreach (var item in list)
            {
                license.ExecuteComputer(item);
            }
        }
    }
}
