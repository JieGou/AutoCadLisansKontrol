using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignColors.WpfExample.Domain;
namespace MaterialDesignDemo.Model
{
    public class Excel
    {

        public static void ExportExcel(List<CheckLicenseModel> data, string path)
        {
            try
            {
                Workbook book = new Workbook();
                //Clear all the worksheets
                book.Worksheets.Clear();
                //Add a new Sheet "Data";
                Worksheet sheet = book.Worksheets.Add("Result of Sniff");

                sheet.Cells.ImportDataTable(data.ToDataTable(),
                                                                                true,
                                                                                0,
                                                                                0,
                                                                                data.Count,
                                                                                5,
                                                                                true
                                                                                );

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                book.Worksheets[0].AutoFitColumns();
                //Save the Excel file
                book.Save(path);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }

        }

        public static void ExporttoExcel(List<CheckLicenseModel> data, string path)
        {

            try
            {
                Workbook book = new Workbook();
                var filename = System.IO.Directory.GetCurrentDirectory() + @"\BatFile\Output Template.xlsx";
                book.Open(filename);
                //Add a new Sheet "Data";
                Worksheet sheet = book.Worksheets[0];
                //sheet.Name = "Result of Sniff";
                string[] columns = { "AppName", "Ip", "MachineName", "Description", "Installed", "Uninstalled", "InstallDate", "UnInstalledDate", "IsFound", "Output", "CheckDate", };


                //for (int j = 0; j < columns.Length; j++)
                //{
                //    book.Worksheets[0].Cells[0, j].PutValue(columns[j]);
                //}


                for (int i = 0; i < data.Count; i++)
                {
                    try
                    {
                        book.Worksheets[0].Cells[i + 1, 0].PutValue(data[i].Ip + "\\" + data[i].Name);
                        book.Worksheets[0].Cells[i + 1, 1].PutValue(data[i].App.AppName);
                        book.Worksheets[0].Cells[i + 1, 2].PutValue(data[i].SerialNumber);
                        book.Worksheets[0].Cells[i + 1, 3].PutValue(ConverttoEnum(data[i].State).ToString());
                        book.Worksheets[0].Cells[i + 1, 4].PutValue(data[i].CheckDate);
                        book.Worksheets[0].Cells[i + 1, 5].PutValue(data[i].Installed);
                        book.Worksheets[0].Cells[i + 1, 6].PutValue(data[i].Uninstalled);
                        book.Worksheets[0].Cells[i + 1, 7].PutValue(data[i].InstallDate.ToString());
                        book.Worksheets[0].Cells[i + 1, 8].PutValue(data[i].UnInstallDate.ToString());
                        book.Worksheets[0].Cells[i + 1, 9].PutValue(data[i].Output);
                    }
                    catch (Exception ex)
                    {
                        var message = ex.Message;
                        book.Worksheets[0].Cells[i + 1, 9].PutValue(message);
                    }
                    
                }

                book.Worksheets[0].AutoFitColumns();


                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                book.Save(path);
            }
            catch (Exception ex)
            {
                var messaage = ex.Message;
            }

        }
        public static QueryState ConverttoEnum(int? value)
        {
            if (value == 0)
            {
                return QueryState.User_not_authorized_to_login_computer;
            }
            else if (value == 1)
            {
                return QueryState.Remote_computer_doesnt_respond_Maybe_it_is_switched_off_or_WMI_service_is_not_running_on_it;
            }
            else if (value == 2)
            {
                return QueryState.Product_found;
            }
            else if (value == 3)
            {
                return QueryState.No_Product_found;
            }
            else if (value == 4)
            {
                return QueryState.UnKnownState;
            }
            return QueryState.UnKnownState;
        }




    }
}
