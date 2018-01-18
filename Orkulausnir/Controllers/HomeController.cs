using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Orkulausnir.DataAccess;
using Orkulausnir.Models;

namespace Orkulausnir.Controllers
{
    // todo:
    // cache file names and files when they are fetched
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var files = AzureDataAccess.GetFileNames();
            var model = new MeasurementViewModel();
            var fileList = new List<SelectListItem>();
            foreach (var file in files)
            {
                fileList.Add(new SelectListItem
                {
                    Text = file.Key,
                    Value = file.Value
                });
            }
            
            model.DataSets = fileList;
            model.IncludeAverage = true;
            model.IncludeHarmonics1_16 = true;
            
            return View(model);
        }
        
        [HttpPost]
        public IActionResult Compare([FromBody] MeasurementViewModel model)
        {
            byte[] file1 = AzureDataAccess.GetFile(model.DatasetFilter1.FileName).Result;
            if (file1.Length == 0)
            {
                throw new ArgumentException($"Skjalið {model.DatasetFilter1.FileName} fannst ekki");
            }

            // we are comparing the same file
            byte[] file2 = model.DatasetFilter1.FileName == model.DatasetFilter2.FileName ? file1 : AzureDataAccess.GetFile(model.DatasetFilter2.FileName).Result;
            if (file2.Length == 0)
            {
                throw new ArgumentException($"Skjalið {model.DatasetFilter1.FileName} fannst ekki");
            }

            var measurement1 = DomainLogic.DomainLogic.FilterData(file1, model.DatasetFilter1);
            var measurement2 = DomainLogic.DomainLogic.FilterData(file2, model.DatasetFilter2);

            List<int> harmonics = DomainLogic.DomainLogic.GetHarmonics(model.IncludeHarmonics1_16, model.IncludeHarmonics17_31, model.IncludeHarmonics32_47, model.IncludeHarmonics48_63);
            
            List<DataPoint> dataPoints1 = DomainLogic.DomainLogic.GetDataPoints(harmonics, model.IncludeAverage, measurement1);
            List<DataPoint> dataPoints2 = DomainLogic.DomainLogic.GetDataPoints(harmonics, model.IncludeAverage, measurement2);
            
            var graphData = new List<GraphData>();
            graphData.Add(new GraphData
            {
                Type = "column",
                DataPoints = dataPoints1
            });

            graphData.Add(new GraphData
            {
                Type = "column",
                DataPoints = dataPoints2
            });

            return Json(graphData);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}