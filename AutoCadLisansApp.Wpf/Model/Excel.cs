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

        public static void ExportExcel(List<CheckLicenseModel> data, string path)
        {

            Workbook book = new Workbook();
            //Clear all the worksheets
            book.Worksheets.Clear();
            //Add a new Sheet "Data";
            Worksheet sheet = book.Worksheets.Add("Result of Sniff");

            sheet.Cells.ImportCustomObjects((System.Collections.ICollection)data,
                                                                            new string[] {
                                                                            "Installed",
                                                                            "Uninstalled",
                                                                            "Ip",
                                                                            "Name",
                                                                            "IsFound",
                                                                            "Output",
                                                                            "CheckDate",
                                                                            "App.AppName",
                                                                            "App.Id"
                                                                            },
                                                                            true,
                                                                            0,
                                                                            0,
                                                                            data.Count,
                                                                            true,
                                                                            "dd/mm/yyyy",
                                                                            false);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            book.Worksheets[0].AutoFitColumns();
            //Save the Excel file
            book.Save(path);
        }

    }
}
