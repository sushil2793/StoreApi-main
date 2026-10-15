using Hangfire;
using Microsoft.Extensions.Configuration;
using System.Data.OleDb;
using System.Data;
using Microsoft.AspNetCore.Hosting;
using ClosedXML.Excel;
using SmartMeterAdmin.Services.Master;

namespace SmartmeterAdmin.Hangfire.ExcelJobs
{
    public class ExportDataExcel
    {
        private readonly string excelConString;
        private readonly string databaseConString;
        private readonly IWebHostEnvironment env;
        private readonly IMaster master;

        public ExportDataExcel(IConfiguration configuration, IWebHostEnvironment env, IMaster master)
        {
            //Read the connection string for the Excel file.
            this.excelConString = configuration.GetConnectionString("ExcelConString");
            //Insert the Data read from the Excel file to Database Table.
            this.databaseConString = configuration.GetConnectionString("OleDevConnection");
            this.env = env;
            this.master = master;
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task ExportExcelData(string dataType)
        {
            try
            {
                var data = ExportDataFromDatabase(dataType);
                DumpDataIntoExcel(data, dataType);
                await master.ManageExportDataExcel("Generate", dataType,$"/ExportExcel/{dataType}_All_Data.xlsx");
            }
            catch
            {
                throw;
            }
        }

        private DataTable ExportDataFromDatabase(string dataType)
        {
            DataTable dt = new();
            using (OleDbConnection connection = new(databaseConString))
            using (OleDbCommand cmd = new("GET_DATA_FOR_EXCEL_EXPORT", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DataType", dataType);

                connection.Open();
                using (OleDbDataAdapter adapter = new(cmd))
                {
                    adapter.Fill(dt);
                }
                connection.Close();
            }
            return dt;
        }



        private void DumpDataIntoExcel(DataTable tbl, string sheetName)
        {
            using XLWorkbook wb = new();
            // Add DataTable in worksheet
            wb.Worksheets.Add(tbl, sheetName);
            using FileStream stream = new(GetFilePath(sheetName), FileMode.Create);
            wb.SaveAs(stream);
        }

        private string GetFilePath(string sheetName)
        {
            string path = Path.Combine(env.WebRootPath, "ExportExcel");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = Path.Combine(path, $"{sheetName}_All_Data.xlsx");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            return path;
        }
    }
}
