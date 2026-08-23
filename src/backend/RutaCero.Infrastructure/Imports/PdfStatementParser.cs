using System.Globalization;using System.Text.RegularExpressions;using UglyToad.PdfPig;using RutaCero.Application.Imports;
namespace RutaCero.Infrastructure.Imports;
public sealed partial class PdfStatementParser:IStatementParser
{
 public bool CanParse(string contentType,string extension)=>extension==".pdf"&&contentType=="application/pdf";
 public Task<IReadOnlyList<ParsedStatementRow>> ParseAsync(Stream stream,CancellationToken token){using var document=PdfDocument.Open(stream);var rows=new List<ParsedStatementRow>();foreach(var page in document.GetPages()){token.ThrowIfCancellationRequested();foreach(var line in page.Text.Split('\n')){var match=Line().Match(line);if(!match.Success||!DateOnly.TryParse(match.Groups[1].Value,new CultureInfo("es-HN"),out var date)||!decimal.TryParse(match.Groups[3].Value,NumberStyles.Any,CultureInfo.InvariantCulture,out var amount))continue;rows.Add(new(date,match.Groups[2].Value.Trim(),Math.Abs(amount),amount>=0));}}return Task.FromResult<IReadOnlyList<ParsedStatementRow>>(rows);}
 [GeneratedRegex(@"^(\d{1,2}/\d{1,2}/\d{2,4})\s+(.+?)\s+(-?[\d,]+\.\d{2})$")]private static partial Regex Line();
}
