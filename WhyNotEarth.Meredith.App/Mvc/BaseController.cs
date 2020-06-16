using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;

namespace WhyNotEarth.Meredith.App.Mvc
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        protected CreateResult Created()
        {
            return new CreateResult();
        }

        [NonAction]
        protected CreateObjectResult Created(object value)
        {
            return new CreateObjectResult(value);
        }

        [NonAction]
        protected async Task<IActionResult> Csv<T>(IEnumerable<T> records, bool isDevelopment)
        {
            var csvData = await GetCsvData(records);

            if (isDevelopment)
            {
                return File(csvData, "text/csv", Guid.NewGuid() + ".csv");
            }

            return Ok(Convert.ToBase64String(csvData));
        }

        [NonAction]
        private async Task<byte[]> GetCsvData<T>(IEnumerable<T> records)
        {
            await using var memoryStream = new MemoryStream();
            await using var streamWriter = new StreamWriter(memoryStream);
            await using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            await csvWriter.WriteRecordsAsync(records);
            await streamWriter.FlushAsync();

            return memoryStream.ToArray();
        }
    }
}