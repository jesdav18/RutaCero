using System.Globalization;using ClosedXML.Excel;using RutaCero.Application.Imports;
namespace RutaCero.Infrastructure.Imports;
public sealed class XlsxStatementParser:IStatementParser
{
 public bool CanParse(string contentType,string extension)=>extension==".xlsx"&&contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" or "application/octet-stream";
 public Task<IReadOnlyList<ParsedStatementRow>> ParseAsync(Stream stream,CancellationToken token){using var book=new XLWorkbook(stream);var rows=new List<ParsedStatementRow>();foreach(var row in book.Worksheet(1).RowsUsed().Skip(1)){token.ThrowIfCancellationRequested();if(!DateOnly.TryParse(row.Cell(1).GetString(),CultureInfo.InvariantCulture,out var date)||!decimal.TryParse(row.LastCellUsed()?.GetString(),NumberStyles.Any,CultureInfo.InvariantCulture,out var amount))continue;rows.Add(new(date,row.Cell(2).GetString(),Math.Abs(amount),amount>=0));}return Task.FromResult<IReadOnlyList<ParsedStatementRow>>(rows);}
}
