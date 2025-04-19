using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Potycznik_Backend.Data;
using Potycznik_Backend.DTo;
using OfficeOpenXml;
using System.IO;
using System.ComponentModel;
using System.IO.Packaging;
using OfficeOpenXml.Style;

namespace Potycznik_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            _context = context;
        }

        [HttpGet("full")]
        public async Task<IActionResult> GetFullReport()
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            var lastInventories = await _context.Inventories
                .OrderByDescending(i => i.Date)
                .Take(2)
                .Include(i => i.InventoryRecords)
                .ToListAsync()
                .ConfigureAwait(false);

            if (lastInventories.Count < 2)
                return BadRequest("Wymagane są przynajmniej dwie inwentaryzacje.");

            var last = lastInventories[0];
            var previous = lastInventories[1];

            var lossesBetween = await _context.Losses
                .Where(l => l.Date >= previous.Date && l.Date <= last.Date)
                .ToListAsync()
                .ConfigureAwait(false);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Raport");

            // Dodanie wiersza tytułowego nad tabelą
            worksheet.Cells[1, 1].Value = $"Raport z inwentaryzacji z dni: {previous.Date:yyyy-MM-dd} - {last.Date:yyyy-MM-dd}";
            worksheet.Cells[1, 1, 1, 8].Merge = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 1].Style.Font.Size = 14;
            worksheet.Cells[1, 1].Style.Font.Bold = true;

            // Nagłówki tabeli
            worksheet.Cells[2, 1].Value = "Nazwa produktu";
            worksheet.Cells[2, 2].Value = "Ilość na poprzednim stanie";
            worksheet.Cells[2, 3].Value = "Ilość przed stratami";
            worksheet.Cells[2, 4].Value = "Ilość utracona";
            worksheet.Cells[2, 5].Value = "Aktualna ilość";
            worksheet.Cells[2, 6].Value = "Różnica ilości";
            worksheet.Cells[2, 7].Value = "Minimalny zapas";
            worksheet.Cells[2, 8].Value = "Stan zapasów";

            // Styl nagłówków
            using (var headerRange = worksheet.Cells[2, 1, 2, 8])
            {
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            int row = 3;

            var previousRecords = previous.InventoryRecords
                .ToDictionary(r => r.ProductName, r => r.Quantity);

            var lastRecords = last.InventoryRecords
                .ToDictionary(r => r.ProductName, r => (r.Quantity, r.PreviousQuantity));

            var lossesGrouped = lossesBetween
                .GroupBy(l => l.ProductName)
                .ToDictionary(g => g.Key, g => g.Sum(l => l.Quantity));

            var productNames = await _context.Products
                .Select(p => p.Name)
                .Union(previousRecords.Keys)
                .Union(lastRecords.Keys)
                .Union(lossesGrouped.Keys)
                .Distinct()
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var name in productNames)
            {
                decimal previousQty = 0, quantityBeforeLoss = 0, currentQty = 0, lossQty = 0;

                var productDetails = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Name == name)
                    .ConfigureAwait(false);

                // 1. Poprzednia ilość
                if (lastRecords.TryGetValue(name, out var lastWithPrev) && lastWithPrev.PreviousQuantity > 0)
                {
                    previousQty = lastWithPrev.PreviousQuantity;
                }
                else if (previousRecords.TryGetValue(name, out var prevQty))
                {
                    previousQty = prevQty;
                }
                else
                {
                    previousQty = productDetails?.Quantity ?? 0;
                }

                // 2. Ilość bez strat
                if (lastRecords.TryGetValue(name, out var lastRec))
                {
                    quantityBeforeLoss = lastRec.Quantity;
                }
                else
                {
                    quantityBeforeLoss = productDetails?.Quantity ?? 0;
                }

                // 3. Strata
                if (lossesGrouped.TryGetValue(name, out var loss))
                {
                    lossQty = loss;
                }

                // 4. Aktualna ilość
                currentQty = quantityBeforeLoss - lossQty;

                // 5. Zmiana
                var change = quantityBeforeLoss > previousQty
                    ? quantityBeforeLoss - previousQty
                    : previousQty - quantityBeforeLoss;

                var changePrefix = quantityBeforeLoss > previousQty ? "+" : "-";

                // 6. Minimalna ilość
                var minQty = productDetails?.MinimalQuantity ?? 0;

                // 7. Status
                var status = currentQty < minQty ? "DO ZAKUPU" : "";

                // Wstaw dane
                worksheet.Cells[row, 1].Value = name;
                worksheet.Cells[row, 2].Value = previousQty;
                worksheet.Cells[row, 3].Value = quantityBeforeLoss;

                // Formatowanie kolumny "Strata" – liczba zawsze z "-", jeśli większa od zera, inaczej 0
                worksheet.Cells[row, 4].Value = lossQty > 0 ? lossQty : 0;
                worksheet.Cells[row, 4].Style.Numberformat.Format = lossQty > 0 ? "-0" : "0";

                // Obliczenie różnicy między aktualną a poprzednią ilością
                var actualQtyDiff = currentQty - previousQty;
                worksheet.Cells[row, 5].Value = actualQtyDiff;
                worksheet.Cells[row, 5].Style.Numberformat.Format = actualQtyDiff == 0 ? "0" : "+0;-0;0";

                // Zmiana – różnica między ilością bez strat a poprzednią ilością
                worksheet.Cells[row, 6].Value = change;
                worksheet.Cells[row, 6].Style.Numberformat.Format = change == 0 ? "0" : "+0;-0;0";

                // Minimalna ilość
                worksheet.Cells[row, 7].Value = minQty;

                // Status do zakupu
                worksheet.Cells[row, 8].Value = status;


                // Kolorowanie: aktualna ilość
                worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(204, 255, 204));

                // Kolorowanie: wiersz DO ZAKUPU
                if (status == "DO ZAKUPU")
                {
                    for (int col = 1; col <= 8; col++)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 204, 204));
                    }
                }

                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Raport_{DateTime.Now:yyyy-MM-dd}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

    }
}
