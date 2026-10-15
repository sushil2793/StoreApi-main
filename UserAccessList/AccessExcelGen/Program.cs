using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Linq;

// users_clean.csv -> UserAccessList.xlsx (VIEW?/EDIT?/DELETE? blank columns)
var csvPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "users_clean.csv");
var lines = File.ReadAllLines(Path.GetFullPath(csvPath)).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
var workbook = new XSSFWorkbook();
var sheet = workbook.CreateSheet("User Access List");

// Header style (bold)
var headerFont = workbook.CreateFont();
headerFont.IsBold = true;
var headerStyle = workbook.CreateCellStyle();
headerStyle.SetFont(headerFont);

string[] header = { "UserID", "UserName (Login ID)", "FullName", "Role", "Status", "VIEW?", "EDIT?", "DELETE?" };
var hRow = sheet.CreateRow(0);
for (int c = 0; c < header.Length; c++)
{
    var cell = hRow.CreateCell(c);
    cell.SetCellValue(header[c]);
    cell.CellStyle = headerStyle;
}

int rowIdx = 1;
foreach (var line in lines)
{
    var parts = line.Split(',');
    if (parts.Length < 5) continue;
    var row = sheet.CreateRow(rowIdx++);
    for (int c = 0; c < 5; c++)
        row.CreateCell(c).SetCellValue(parts[c].Trim());
    // VIEW? / EDIT? / DELETE? blank - user bhar dega
    row.CreateCell(5).SetCellValue("");
    row.CreateCell(6).SetCellValue("");
    row.CreateCell(7).SetCellValue("");
}

for (int c = 0; c < 8; c++) sheet.AutoSizeColumn(c);
var outPath = Path.Combine(Path.GetFullPath(csvPath), "..", "UserAccessList.xlsx");
using var fs = new FileStream(Path.GetFullPath(outPath), FileMode.Create);
workbook.Write(fs);
Console.WriteLine($"OK: UserAccessList.xlsx generated with {rowIdx - 1} users.");
