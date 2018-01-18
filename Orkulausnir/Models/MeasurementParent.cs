using System.Collections.Generic;

namespace Orkulausnir.Models
{
    public class MeasurementParent
    {
        public MeasurementParent()
        {
            Measurements = new List<MeasurementItem>();
        }

        /// <summary>
        /// The name of the measurement
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The phase of the measurement
        /// </summary>
        public PhaseEnum Phase { get; set; }

        /// <summary>
        /// The electrical quantities
        /// </summary>
        public ElectricalQuantities Quantities { get; set; }

        /// <summary>
        /// The list of the 63 measurments
        /// </summary>
        public List<MeasurementItem> Measurements { get; set; }
    }
}