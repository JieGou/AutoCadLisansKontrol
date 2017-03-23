using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialDesignDemo.Model
{
    public class Excel
    {
        private Workbook xlWorkBook;

        public string ExportExcel(string path, string filename) {

            xlWorkBook = new Workbook();
            xlWorkBook.Open(path);
            var xlWorkSheet = xlWorkBook.Worksheets[0];
            var cells = xlWorkSheet.Cells;
          
            
            xlWorkSheet.AutoFitColumns();
            path = path.Substring(0, path.LastIndexOf("\\", System.StringComparison.Ordinal));

            var fullpath = path + "\\File\\" + filename;
            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
            }
            xlWorkBook.Save(fullpath);
            return fullpath;
        }

    }
}
