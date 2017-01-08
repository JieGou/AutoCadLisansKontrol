
using AutoCadLisansKontrol.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCadLisansKontrol
{
    class Program
    {
        static void Main(string[] args)
        {
            DataAccess dbaccess = new DataAccess();
            var list =dbaccess.ListFirm();
            var firm = list;
        }
    }
}
