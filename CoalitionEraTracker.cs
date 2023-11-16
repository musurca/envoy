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

                // Renaissance
                CoalitionEraTracker.CoalitionsByEra.Add(
                    (1500, 1617),
                    new List<string[]>() {
                        new string[] {
                            "France", "Scotland", "Dutch", "Swiss/Mercenary", 
                            "Italian States/Mercenary", "Portugal", "Ferrara", "Ottoman Turks"
                        },
                        new string[] {
                            "Imperialist/Spain", "England", "Swiss", "Italian States", 
                            "Hungary", "Mamelukes", "Navarre/Huguenots"
                        }
                    }
                );

                // Thirty Years War
                CoalitionEraTracker.CoalitionsByEra.Add(
                    (1618, 1648),
                    new List<string[]>() {
                        new string[] { 
                            "Imperialist", "Bavaria", "Hapsburg", "Poland", "Spain", "Saxon", "Catholic" 
                        },
                        new string[] { 
                            "Anti-Imperialist", "Bohemia", "Transylvania", "Dutch", "Denmark", 
                            "Saxon Rebels", "France", "Sweden", "Ottoman Turks" 
                        }
                    }
                );

                // Great Northern War
                CoalitionEraTracker.CoalitionsByEra.Add(
                    (1700, 1721),
                    new List<string[]>() {
                        new string[] {
                            "Lithuanian Crown", "Turks", "Sweden", "Poland/Pro-Swedish"
                        },
                        new string[] {
                            "Prussia", "Saxony", "Lithuanian Republic", "Poland/Pro-Russian", 
                            "Denmark", "Russia"
                        }
                    }
                );

                // Seven Years War
                CoalitionEraTracker.CoalitionsByEra.Add(
                    (1756, 1763),
                    new List<string[]>() {
                        new string[] { "Sweden", "Reichsarmee", "Russia", "France", "Austria"},
                        new string[] { "Britain", "Prussia", "Army of Observation" }
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
