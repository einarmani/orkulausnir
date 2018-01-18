using System.ComponentModel.DataAnnotations;

namespace Orkulausnir.Models
{
    public class DatasetFilter
    {
        public string FileName { get; set; }

        [Display(Name = "Current")]
        public bool IncludeCurrent { get; set; }

        [Display(Name = "Voltage")]
        public bool IncludeVoltage { get; set; }

        [Display(Name = "Fasi A")]
        public bool IncludePhaseA { get; set; }

        [Display(Name = "Fasi B")]
        public bool IncludePhaseB { get; set; }

        [Display(Name = "Fasi C")]
        public bool IncludePhaseC { get; set; }

        [Display(Name = "Neutral")]
        public bool IncludeNeutral { get; set; }
    }
}