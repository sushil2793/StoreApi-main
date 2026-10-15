using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using SmartMeterAdmin.Infrastructure.Helpers;
using Hangfire;
using SmartMeterAdmin.Infrastructure.Enumeration;

namespace SmartmeterAdmin.Hangfire.ExcelJobs
{
    public class ImportExcelDataIntoDatabase
    {
        private readonly string excelConString;
        private readonly string databaseConString;
        private TableNameEnum tableName;

        public ImportExcelDataIntoDatabase(IConfiguration configuration)
        {
            this.excelConString = configuration.GetConnectionString("ExcelConString");
            this.databaseConString = configuration.GetConnectionString("DevConnection");
        }

        [AutomaticRetry(Attempts = 0)]
        public void ImportExcelData(string filePath, int userId, TableNameEnum tableName)
        {
            try
            {
                this.tableName = tableName;
                ProcessData(filePath, userId);

                // ✅ AFTER ALL INSERTS → CALL STORED PROCEDURE
                ExecuteStoredProcedure(userId);
            }
            catch (Exception)
            {
                TruncateTempTable();
                throw;
            }
            finally
            {
                DeleteFileFromServer(filePath);
            }
        }

        private void ProcessData(string filePath, int userId)
        {
            var data = ExportDataFromExcel(filePath, userId);

            if (tableName == TableNameEnum.TempNewMeter)
            {
                string[] customerColumns = ColumnNameByTable.GetColumns(TableNameEnum.TempFullCustomerData).Keys.ToArray();
                string[] newmeterColumns = ColumnNameByTable.GetColumns(tableName).Keys.ToArray();
                string[] oldmeterColumns = ColumnNameByTable.GetColumns(TableNameEnum.TempOldMeter).Keys.ToArray();

                var customerData = new DataView(data).ToTable(false, customerColumns);
                var newMeterData = new DataView(data).ToTable(false, newmeterColumns);
                var oldMeterData = new DataView(data).ToTable(false, oldmeterColumns);

                InsertDataIntoTable(customerData, TableNameEnum.TempFullCustomerData);
                InsertDataIntoTable(newMeterData, tableName);
                InsertDataIntoTable(oldMeterData, TableNameEnum.TempOldMeter);
            }
            else
            {
                InsertDataIntoTable(data, tableName);
            }
        }

        private DataTable ExportDataFromExcel(string filePath, int userId)
        {
            DataTable dt = new();
            var excelFilePath = string.Format(excelConString, filePath);

            using (OleDbConnection connExcel = new(excelFilePath))
            {
                using OleDbCommand cmdExcel = new();
                using OleDbDataAdapter odaExcel = new();
                cmdExcel.Connection = connExcel;

                connExcel.Open();
                DataTable? dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                var sheetName = GetSheetName(dtExcelSchema);
                connExcel.Close();

                connExcel.Open();
                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                odaExcel.SelectCommand = cmdExcel;
                odaExcel.Fill(dt);

                DataColumn CreatedBy = new("CreatedBy", typeof(int))
                {
                    DefaultValue = userId
                };
                dt.Columns.Add(CreatedBy);

                foreach (DataColumn col in dt.Columns)
                {
                    col.ColumnName = col.ColumnName.Replace(" ", "");
                }

                connExcel.Close();
            }

            return dt;
        }

        private void InsertDataIntoTable(DataTable dt, TableNameEnum tableName)
        {
            if (dt.Columns.Contains("MeterNo"))
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["MeterNo"] != DBNull.Value)
                        row["MeterNo"] = row["MeterNo"].ToString().Trim();
                }

                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    if (dt.Rows[i]["MeterNo"] == DBNull.Value ||
                        string.IsNullOrWhiteSpace(dt.Rows[i]["MeterNo"].ToString()))
                    {
                        dt.Rows.RemoveAt(i);
                    }
                }

                dt.AcceptChanges();
            }

            var columns = ColumnNameByTable.GetColumns(tableName);

            using SqlConnection con = new(databaseConString);
            using SqlBulkCopy sqlBulkCopy = new(con);

            sqlBulkCopy.DestinationTableName = $"dbo.{tableName}";

            foreach (var column in columns)
            {
                sqlBulkCopy.ColumnMappings.Add(column.Key, column.Value);
            }

            con.Open();
            sqlBulkCopy.WriteToServer(dt);
        }

        // ✅ NEW METHOD (IMPORTANT)
        private void ExecuteStoredProcedure(int userId)
        {
            using SqlConnection con = new(databaseConString);
            using SqlCommand cmd = new("ImportConsumerSurveyMerged", con);

            cmd.CommandType = CommandType.StoredProcedure;

            // ✅ REQUIRED FIX
            cmd.Parameters.AddWithValue("@UserId", userId);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        private static void DeleteFileFromServer(string filePath)
        {
            File.Delete(filePath);
        }

        private void TruncateTempTable()
        {
            try
            {
                using SqlConnection con = new(databaseConString);
                using SqlCommand cmd = new("TRUNCATE_TEMP_TABLE", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch
            {
            }
        }

        private string GetSheetName(DataTable? dt)
        {
            if (tableName == TableNameEnum.TEMP_UPDATED_DATA)
                return "Data$";
            else
                return dt?.Rows[0]["TABLE_NAME"].ToString() ?? string.Empty;
        }
    }
}