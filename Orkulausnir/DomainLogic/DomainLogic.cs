using System;
using System.Collections.Generic;
using System.Linq;
using Orkulausnir.Models;

namespace Orkulausnir.DomainLogic
{
    public static class DomainLogic
    {
        public static IList<MeasurementParent> ParseFile(byte[] file)
        {
            var dataItems = new List<MeasurementParent>();
            
            var text = System.Text.Encoding.UTF8.GetString(file);
            string[] lines = text.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
            );

            // the index of the item array
            int listIndex = -1;
            int lineNumber = 0;

            foreach (string line in lines)
            {
                lineNumber = lineNumber + 1;

                // the first line above each phase/voltage measurement
                if (line.Contains("Harmonics Summary"))
                {
                    if (!line.Contains("\t"))
                    {
                        throw new ArgumentException($"Villa í línu {lineNumber}. Tab þarf að vera í línunni þar sem 'Harmonics Summary er'");
                    }

                    dataItems.Add(new MeasurementParent());

                    var indexOfTab = line.IndexOf("\t", StringComparison.Ordinal);

                    listIndex = listIndex + 1;
                    dataItems[listIndex].Name = "'" + line.Substring(0, indexOfTab) + "'";
                    dataItems[listIndex].Quantities = line.Contains("Current Harmonics")
                        ? ElectricalQuantities.Current
                        : ElectricalQuantities.Voltage;
                    dataItems[listIndex].Phase = line.Contains("Phase A")
                        ? PhaseEnum.A
                        : line.Contains("Phase B")
                            ? PhaseEnum.B
                            : line.Contains("Phase C")
                                ? PhaseEnum.C
                                : PhaseEnum.Neutral;
                }
                else if (line.Length > 0 && !line.StartsWith("Harm")) // it's a measurement
                {
                    var measurementLine = line.Split("\t");
                    if (measurementLine.Length != 3)
                    {
                        throw new ArgumentException($"Villa í línu {lineNumber}. Tab þarf að splitta upp línunni í samtals þrjár einingar");
                    }

                    dataItems[listIndex].Measurements.Add(new MeasurementItem
                    {
                        Harmonic = Convert.ToInt32(measurementLine[0]),
                        Average = Convert.ToDouble(measurementLine[1].Replace('.', ',')),
                        Max = Convert.ToDouble(measurementLine[2].Replace('.', ','))
                    });
                }
            }

            foreach (var dataItem in dataItems)
            {
                if (dataItem.Measurements.Count != 63)
                {
                    throw new ArgumentException($"Harmoníur eiga að vera 63 fyrir hverja mælingu. Mæling {dataItem.Name} {dataItem.Phase} {dataItem.Quantities} contains {dataItem.Measurements.Count}");
                }
            }

            return dataItems;
        }

        public static IList<MeasurementParent> FilterData(byte[] file, bool useCurrent, List<PhaseEnum> phase)
        {
            IList<MeasurementParent> measurementParent1 = ParseFile(file);
            
            List<MeasurementParent> filteredData = measurementParent1.Where(x =>
                ((useCurrent && x.Quantities == ElectricalQuantities.Current) ||
                 (!useCurrent && x.Quantities == ElectricalQuantities.Voltage)) &&
                (
                phase.Contains(x.Phase)
                //(filter.IncludePhaseA && x.Phase == PhaseEnum.A) ||
                // (filter.IncludePhaseB && x.Phase == PhaseEnum.B) ||
                // (filter.IncludePhaseC && x.Phase == PhaseEnum.C) ||
                // (filter.IncludePhaseNeutral && x.Phase == PhaseEnum.Neutral)
                 )
            ).ToList();

            return filteredData;
        }

        public static List<int> GetHarmonics(bool includeHarmonics116, bool includeHarmonics1731, bool includeHarmonics3247, bool includeHarmonics4863)
        {
            var harmonics = new List<int>();
            if (includeHarmonics116)
            {
                for (int i = 1; i < 17; i++)
                {
                    harmonics.Add(i);
                }
            }
            if (includeHarmonics1731)
            {
                for (int i = 17; i < 32; i++)
                {
                    harmonics.Add(i);
                }
            }
            if (includeHarmonics3247)
            {
                for (int i = 32; i < 48; i++)
                {
                    harmonics.Add(i);
                }
            }
            if (includeHarmonics4863)
            {
                for (int i = 48; i < 64; i++)
                {
                    harmonics.Add(i);
                }
            }

            return harmonics;
        }

        public static List<DataPoint> GetDataPoints(List<int> harmonics, bool useAverage, IList<MeasurementParent> measurements)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();
            foreach (var h in harmonics)
            {
                var harmonicMeasures = measurements.SelectMany(x => x.Measurements).Where(x => x.Harmonic == h).ToList();
                if (useAverage)
                {
                    var value = harmonicMeasures.Sum(x => x.Average);
                    dataPoints.Add(new DataPoint(h, value));
                }
                else
                {
                    var value = harmonicMeasures.Sum(x => x.Max);
                    dataPoints.Add(new DataPoint(h, value));
                }
            }

            return dataPoints;
        }
    }
}
