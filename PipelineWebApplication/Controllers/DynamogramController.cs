using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using monitoring;
using monitoring.Data;
using monitoring.Models;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;


namespace monitoring.Controllers
{
    [Authorize]
    public class DynamogramController : Controller
    {
        private readonly MonitoringSystemContext _context;

        public DynamogramController(MonitoringSystemContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("ExportToExcel")]
        public IActionResult ExportToExcel()
        {
            var dynamograms = _context.Dynamograms.Include(d => d.Well).Include(d => d.User).ToList(); // Получение данных для экспорта

            byte[] excelData = ExportToExcel(dynamograms); // Генерация Excel файла

            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "dynamograms.xlsx"); // Возврат файла клиенту
        }
        private byte[] ExportToExcel(IEnumerable<Dynamogram> data)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Data");

                // Запись заголовков таблицы
                var headers = new[] { "Название качалки", "Напряжение штанговой колоны", "Дата добавления", "Дебит жидкости", " Максимальная нагрузка на головку  насосного балансира назесного привода", "Pmax", "Pmin", "Тип прибора", " Число качаний наземного привода", "Длина хода наземного привода", "Кпод_нас", "Кнап_нас", "Работник", "Заключение" /* ... */ };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                }

                // Запись данных из модели в ячейки таблицы
                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = item.Well.Name;
                    worksheet.Cell(row, 2).Value = item.VarG;
                    worksheet.Cell(row, 3).Value = item.Date;
                    worksheet.Cell(row, 4).Value = item.VarQ;
                    worksheet.Cell(row, 5).Value = item.VarPmax;
                    worksheet.Cell(row, 6).Value = item.VarPmin;
                    worksheet.Cell(row, 7).Value = item.TypeDevice;
                    worksheet.Cell(row, 8).Value = item.VarN;
                    worksheet.Cell(row, 9).Value = item.VarL;
                    worksheet.Cell(row, 10).Value = item.VarKpod;
                    worksheet.Cell(row, 11).Value = item.VarKnap;
                    worksheet.Cell(row, 12).Value = item.User.FirstName;
                    worksheet.Cell(row, 13).Value = item.Opinion;



                    // ... Запись остальных полей в соответствующие ячейки

                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }


        [HttpPost]
        [Route("ImportFromExcel")]
        public async Task<IActionResult> ImportFromExcel(IFormFile excelFile, int userId, [Bind("WellId")] Dynamogram dynamograms)
        {
            if (excelFile != null && excelFile.Length > 0)
            {
                using (var stream = excelFile.OpenReadStream())
                {
                    using (var spreadsheetDoc = SpreadsheetDocument.Open(stream, false))
                    {
                        var workbookPart = spreadsheetDoc.WorkbookPart;
                        var worksheetPart = workbookPart.WorksheetParts.First();
                        var worksheet = worksheetPart.Worksheet;
                        var sheetData = worksheet.GetFirstChild<SheetData>();

                        var rows = sheetData.Elements<Row>().Skip(1); // Пропуск заголовка, начало с 2-й строки

                        foreach (var row in rows)
                        {
                            // Чтение значений ячеек и сохранение данных


                            var date = DateTime.Now.ToString();
                            var varQ = int.Parse(row.Elements<Cell>().ElementAt(0).CellValue.Text);
                            var varPmax = int.Parse(row.Elements<Cell>().ElementAt(1).CellValue.Text);
                            var varPmin = int.Parse(row.Elements<Cell>().ElementAt(2).CellValue.Text);
                            var typeDevice = row.Elements<Cell>().ElementAt(3).CellValue.Text;
                            var varN = int.Parse(row.Elements<Cell>().ElementAt(4).CellValue.Text);
                            var varL = int.Parse(row.Elements<Cell>().ElementAt(5).CellValue.Text);
                            var varKpod = int.Parse(row.Elements<Cell>().ElementAt(6).CellValue.Text);
                            var varKnap = int.Parse(row.Elements<Cell>().ElementAt(7).CellValue.Text);
                            var opinion = row.Elements<Cell>().ElementAt(8).CellValue.Text;
                            var varG = int.Parse(row.Elements<Cell>().ElementAt(9).CellValue.Text);

                            // Сохранение данных в базе данных или другое действие

                            // Пример сохранения данных в базе данных
                            var dynamogram = new Dynamogram
                            {
                                WellId = dynamograms.WellId,
                                Date = date,
                                VarQ = varQ,
                                VarPmax = varPmax,
                                VarPmin = varPmin,
                                TypeDevice = typeDevice,
                                VarN = varN,
                                VarL = varL,
                                VarKpod = varKpod,
                                VarKnap = varKnap,
                                Opinion = opinion,
                                VarG = varG,
                                UserId = userId
                            };
                            int VarQ_G = _context.Guides.First(q => q.WellId == dynamograms.WellId).VarQ;
                            int VarPMax_G = _context.Guides.First(q => q.WellId == dynamograms.WellId).VarPmax;
                            int VarPMin_G = _context.Guides.First(q => q.WellId == dynamograms.WellId).VarPmin;
                            int VarN_G = _context.Guides.First(q => q.WellId == dynamograms.WellId).VarN;
                            int VarL_G = _context.Guides.First(q => q.WellId == dynamograms.WellId).VarL;
                            int VarKPod_G = _context.Guides.First(q => q.WellId == dynamograms.WellId).VarKpod;
                            int VarKNap_G = _context.Guides.First(q => q.WellId == dynamograms.WellId).VarKnap;
                            int VarG_G = _context.Guides.First(q => q.WellId == dynamograms.WellId).VarG;

                            _context.Dynamograms.Add(dynamogram);
                            await _context.SaveChangesAsync();
                            if (varQ > VarQ_G)
                            {
                                Advice advice = new Advice()
                                {
                                    DynamogramId = dynamogram.DynamogramId,
                                    AdviceTextId = 1,
                                    RoleId = 1,
                                };

                                _context.Advices.Add(advice);



                            }

                            _context.SaveChanges();
                        }
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Index_advice(int id)
        {
            var advices = await _context.Advices.Include(d => d.Role).Include(d => d.AdviceText).Include(d => d.Dynamogram.User).Include(d => d.Dynamogram.Well.Workshop.Ngdu).Include(d => d.Dynamogram.Well).Where(d => d.Dynamogram.Well.Workshop.Ngdu.NgduId == id).ToListAsync();


            return View(advices);

        }

        // GET: Dynamogram
        public async Task<IActionResult> Index()
        {
            var dynamograms = await _context.Dynamograms
                .Include(d => d.Well)
                .Include(d => d.User)
                .ToListAsync();

            return View(dynamograms);
        }

        // GET: Dynamogram/Edit/5
        // GET: Dynamogram/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Dynamograms == null)

            {
                return NotFound();
            }

            var dynamogram = await _context.Dynamograms.FindAsync(id);
            if (dynamogram == null)
            {
                return NotFound();
            }

            ViewData["WellId"] = new SelectList(_context.Wells, "WellId", "WellId", dynamogram.WellId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", dynamogram.UserId);

            return View(dynamogram);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("DynamogramId, WellId, Date, VarQ, VarPmax, VarPmin, TypeDevice, VarN, VarL, VarKpod, VarKnap, Opinion, VarG, UserId")] Dynamogram dynamogram)
        {
            if (id != dynamogram.DynamogramId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dynamogram);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DynamogramExists(dynamogram.DynamogramId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["WellId"] = new SelectList(_context.Wells, "WellId", "WellId", dynamogram.WellId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", dynamogram.UserId);

            return View(dynamogram);
        }

        // GET: Dynamogram/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dynamogram = await _context.Dynamograms
                .Include(d => d.Well)
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.DynamogramId == id);

            if (dynamogram == null)
            {
                return NotFound();
            }

            return View(dynamogram);
        }

        // POST: Dynamogram/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var dynamogram = await _context.Dynamograms.FindAsync(id);
            _context.Dynamograms.Remove(dynamogram);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: PipelineDatums/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["WellId"] = new SelectList(_context.Wells, "WellId", "Name");


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DynamogramId,WellId,Date,VarQ,VarPmax,VarPmin,TypeDevice,VarN,VarL,VarKpod,VarKnap,Opinion,VarG,UserId")] Dynamogram dynamogram)
        {
            if (ModelState.IsValid)
            {
                int VarQ_G = _context.Guides.First(q => q.WellId == dynamogram.WellId).VarQ;
                int VarPMax_G = _context.Guides.First(q => q.WellId == dynamogram.WellId).VarPmax;
                int VarPMin_G = _context.Guides.First(q => q.WellId == dynamogram.WellId).VarPmin;
                int VarN_G = _context.Guides.First(q => q.WellId == dynamogram.WellId).VarN;
                int VarL_G = _context.Guides.First(q => q.WellId == dynamogram.WellId).VarL;
                int VarKPod_G = _context.Guides.First(q => q.WellId == dynamogram.WellId).VarKpod;
                int VarKNap_G = _context.Guides.First(q => q.WellId == dynamogram.WellId).VarKnap;
                int VarG_G = _context.Guides.First(q => q.WellId == dynamogram.WellId).VarG;


                dynamogram.Date = DateTime.Now.ToString();
                _context.Add(dynamogram);
                await _context.SaveChangesAsync();
                if (dynamogram.VarQ > VarQ_G)
                {
                    Advice advice = new Advice()
                    {
                        DynamogramId = dynamogram.DynamogramId,
                        AdviceTextId = 1,
                        RoleId = 1,
                    };

                    _context.Advices.Add(advice);
                    _context.SaveChanges();


                }
                return RedirectToAction(nameof(Index));
            }


            ViewData["WellId"] = new SelectList(_context.Wells, "WellId", "WellId", dynamogram.WellId);

            return View(dynamogram);
        }

        private bool DynamogramExists(long id)
        {
            return _context.Dynamograms.Any(d => d.DynamogramId == id);
        }
    }
}
