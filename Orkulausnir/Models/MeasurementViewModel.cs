using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Orkulausnir.Models
{
    public class MeasurementViewModel
    {
        [Display(Name = "Meðaltal")]
        public bool IncludeAverage { get; set; }

        [Display(Name = "Hámark")]
        public bool IncludeMax { get; set; }
        
        [Display(Name = "1-16")]
        public bool IncludeHarmonics1_16 { get; set; }

        [Display(Name = "17-31")]
        public bool IncludeHarmonics17_31 { get; set; }

        [Display(Name = "32-47")]
        public bool IncludeHarmonics32_47 { get; set; }

        [Display(Name = "48-63")]
        public bool IncludeHarmonics48_63 { get; set; }
        
        public IEnumerable<SelectListItem> DataSets { get; set; }

        public DatasetFilter DatasetFilter1 { get; set; }

        public DatasetFilter DatasetFilter2 { get; set; }
    }
}
