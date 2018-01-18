using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Orkulausnir.Models
{
    public class MeasurementViewModel
    {
        public string FileName1 { get; set; }

        public string FileName2 { get; set; }

        public bool UseCurrent { get; set; }
        
        public bool UseAverage { get; set; }
        
        [Display(Name = "Fasi A")]
        public bool IncludePhaseA { get; set; }

        [Display(Name = "Fasi B")]
        public bool IncludePhaseB { get; set; }

        [Display(Name = "Fasi C")]
        public bool IncludePhaseC { get; set; }

        [Display(Name = "Neutral")]
        public bool IncludePhaseNeutral { get; set; }

        [Display(Name = "1-16")]
        public bool IncludeHarmonics1_16 { get; set; }

        [Display(Name = "17-31")]
        public bool IncludeHarmonics17_31 { get; set; }

        [Display(Name = "32-47")]
        public bool IncludeHarmonics32_47 { get; set; }

        [Display(Name = "48-63")]
        public bool IncludeHarmonics48_63 { get; set; }
        
        public IEnumerable<SelectListItem> DataSets { get; set; }
        
        public List<string> Filters { get; set; }
    }
}
