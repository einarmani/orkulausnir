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
    // todo: cache file names and files when they are fetched
    public class HomeController : Controller
    {
        private const string GraphType = "column";

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
            //model.UseCurrent = true;
            //model.UseAverage = true;
            model.IncludeHarmonics1_16 = true;
            
            return View(model);
        }
        
        [HttpPost]
        public IActionResult Compare([FromBody] MeasurementViewModel model)
        {
            bool isSameMeasurementComparison = string.IsNullOrEmpty(model.FileName2);
            List<int> harmonics = DomainLogic.DomainLogic.GetHarmonics(model.IncludeHarmonics1_16, model.IncludeHarmonics17_31, model.IncludeHarmonics32_47, model.IncludeHarmonics48_63);
            var phaseEnums = new List<PhaseEnum>();
            if (model.IncludePhaseA) phaseEnums.Add(PhaseEnum.A);
            if (model.IncludePhaseB) phaseEnums.Add(PhaseEnum.B);
            if (model.IncludePhaseC) phaseEnums.Add(PhaseEnum.C);
            if (model.IncludePhaseNeutral) phaseEnums.Add(PhaseEnum.Neutral);
            
            var graphData = new List<GraphData>();

            byte[] file1 = AzureDataAccess.GetFile(model.FileName1).Result;
            if (file1.Length == 0)
            {
                throw new ArgumentException($"Skjalið {model.FileName1} fannst ekki");
            }
            
            if (isSameMeasurementComparison)
            {
                // we are comparing the same file so each phase is a datapoint
                foreach (var phase in phaseEnums)
                {
                    var measurement1 = DomainLogic.DomainLogic.FilterData(file1, model.UseCurrent, new List<PhaseEnum> {phase});
                    List<DataPoint> dataPoints = DomainLogic.DomainLogic.GetDataPoints(harmonics, model.UseAverage, measurement1);
                    graphData.Add(new GraphData
                    {
                        DataPoints = dataPoints,
                        Type = GraphType,
                        Label = "ble-" + Guid.NewGuid().ToString().Substring(0,4)
                    });
                }
            }
            else
            {
                // we are comparing two different measurements so the sum of XXxx xxxx TBD!!!!
                byte[] file2 = model.FileName1 == model.FileName2 ? file1 : AzureDataAccess.GetFile(model.FileName2).Result;
                if (file2.Length == 0)
                {
                    throw new ArgumentException($"Skjalið {model.FileName2} fannst ekki");
                }

                var measurement1 = DomainLogic.DomainLogic.FilterData(file1, model.UseCurrent, phaseEnums);
                List<DataPoint> dataPoints = DomainLogic.DomainLogic.GetDataPoints(harmonics, model.UseAverage, measurement1);
                graphData.Add(new GraphData
                {
                    DataPoints = dataPoints,
                    Type = GraphType,
                    Label = "ble-" + Guid.NewGuid().ToString().Substring(0, 4)
                });

                var measurement2 = DomainLogic.DomainLogic.FilterData(file2, model.UseCurrent, phaseEnums);
                List<DataPoint> dataPoints2 = DomainLogic.DomainLogic.GetDataPoints(harmonics, model.UseAverage, measurement2);
                graphData.Add(new GraphData
                {
                    DataPoints = dataPoints2,
                    Type = GraphType,
                    Label = "some label"
                });
            }
            
            return Json(graphData);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}