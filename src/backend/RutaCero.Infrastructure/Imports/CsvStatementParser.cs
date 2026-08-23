using System.Globalization;using CsvHelper;using CsvHelper.Configuration;using RutaCero.Application.Imports;
namespace RutaCero.Infrastructure.Imports;
public sealed class CsvStatementParser:IStatementParser
{
 public bool CanParse(string contentType,string extension)=>extension==".csv"&&contentType is "text/csv" or "application/vnd.ms-excel" or "application/octet-stream";
 public async Task<IReadOnlyList<ParsedStatementRow>> ParseAsync(Stream stream,CancellationToken token)
 {using var reader=new StreamReader(stream,leaveOpen:true);using var csv=new CsvReader(reader,new CsvConfiguration(CultureInfo.InvariantCulture){DetectDelimiter=true,HasHeaderRecord=true,MissingFieldFound=null});var rows=new List<ParsedStatementRow>();await csv.ReadAsync();csv.ReadHeader();while(await csv.ReadAsync()){token.ThrowIfCancellationRequested();var fields=csv.Parser.Record??[];if(fields.Length<3||!TryDate(fields[0],out var date)||!decimal.TryParse(fields[^1],NumberStyles.Any,CultureInfo.InvariantCulture,out var amount))continue;rows.Add(new(date,fields[1],Math.Abs(amount),amount>=0));}return rows;}
 private static bool TryDate(string value,out DateOnly date)=>DateOnly.TryParse(value,CultureInfo.InvariantCulture,DateTimeStyles.None,out date)||DateOnly.TryParse(value,new CultureInfo("es-HN"),DateTimeStyles.None,out date);
}
