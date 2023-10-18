using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDS_Dispatches {
    internal class CoalitionEraTracker {
        static SortedDictionary<
            (int StartYear, int EndYear), 
            List<string[]>
        > CoalitionsByEra;

        public static bool AreNationsAllied(int year, string nation1, string nation2) {
            if (nation1 == nation2) { return true; }

            if (CoalitionEraTracker.CoalitionsByEra == null) {
                CoalitionEraTracker.CoalitionsByEra = new SortedDictionary<
                    (int StartYear, int EndYear),
                    List<string[]>
                >();

                // Seven Years War
                CoalitionEraTracker.CoalitionsByEra.Add(
                    (1756, 1761),
                    new List<string[]>() {
                        new string[] { "French", "Austrian", "Russian" },
                        new string[] { "British", "Prussian" }
                    }
                );
                CoalitionEraTracker.CoalitionsByEra.Add(
                    (1762, 1763),
                    new List<string[]>() {
                        new string[] { "French", "Austrian", "Spanish" },
                        new string[] { "British", "Prussian", "Russian", "Portuguese" }
                    }
                );

                // American War of Independence
                CoalitionEraTracker.CoalitionsByEra.Add(
                    (1775, 1783),
                    new List<string[]>() {
                         new string[] { "American", "French" }
                    }
                );

                // French Revolutionary & Napoleonic Wars
                CoalitionEraTracker.CoalitionsByEra.Add(
                    (1792, 1815),
                    new List<string[]>() {
                        new string[] { "British", "Spanish", "Portuguese", "Prussian", "Austrian", "Russian", "Dutch" }
                    }
                );
            }

            // Determine if both nations are in a coalition in the year of this scenario
            foreach (var key in CoalitionEraTracker.CoalitionsByEra.Keys) {
                if (year >= key.StartYear && year <= key.EndYear) {
                    List<string[]> coalitions = CoalitionEraTracker.CoalitionsByEra[key];
                    foreach (string[] coalition in coalitions) {
                        if (coalition.Contains(nation1) && coalition.Contains(nation2)) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
