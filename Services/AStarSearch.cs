using System.Diagnostics;
using SearchAlgorithms.Data;
using SearchAlgorithms.Models;

namespace SearchAlgorithms.Services {
    public class AStarSearch : ISearchAlgorithm {
        public string Name => "A* Search";

        public SearchResult FindPath(string startCity, string targetCity) {
            var result = new SearchResult();
            var stopwatch = Stopwatch.StartNew();

            var startNode = GeorgiaMap.Cities.FirstOrDefault(c => c.Name == startCity);
            var targetNode = GeorgiaMap.Cities.FirstOrDefault(c => c.Name == targetCity);

            if(startNode == null || targetNode == null) {
                stopwatch.Stop();
                return result;
            }

            var openSet = new SortedSet<(string Name, double FScore)>(new FScoreComparer());
            var parentMap = new Dictionary<string, string>();

            var gScore = new Dictionary<string, double>();

            foreach(var city in GeorgiaMap.Cities) {
                gScore[city.Name] = double.PositiveInfinity;
            }

            gScore[startCity] = 0;
            double initialFScore = CalculateHeuristic(startNode, targetNode);
            openSet.Add((startCity, initialFScore));

            while(openSet.Count > 0) {
                var current = openSet.Min;
                openSet.Remove(current);

                result.VisitedOrder.Add(current.Name);

                if(current.Name == targetCity) {
                    break;
                }

                var currentCityData = GeorgiaMap.Cities.First(c => c.Name == current.Name);

                foreach(var neighborName in GetNeighbors(current.Name)) {
                    var neighborCityData = GeorgiaMap.Cities.First(c => c.Name == neighborName);

                    double edgeWeight = CalculateDistance(currentCityData, neighborCityData);
                    double tentativeGScore = gScore[current.Name] + edgeWeight;

                    if(tentativeGScore < gScore[neighborName]) {
                        parentMap[neighborName] = current.Name;
                        gScore[neighborName] = tentativeGScore;

                        double fScore = tentativeGScore + CalculateHeuristic(neighborCityData, targetNode);

                        openSet.RemoveWhere(item => item.Name == neighborName);
                        openSet.Add((neighborName, fScore));
                    }
                }
            }

            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;

            if (parentMap.ContainsKey(targetCity) || startCity == targetCity) {
                result.Path = ReconstructPath(parentMap, startCity, targetCity);
                result.TotalDistance = CalculatePathDistance(result.Path);
            }

            return result;
        }

        private List<string> GetNeighbors(string city) {
            return GeorgiaMap.Roads
                .Where(r => r.StartCity == city).Select(r => r.EndCity)
                .Concat(GeorgiaMap.Roads.Where(r => r.EndCity == city).Select(r => r.StartCity))
                .Distinct()
                .ToList();
        }

        private double CalculateHeuristic(City current, City target) {
            return CalculateDistance(current, target);
        }

        private double CalculateDistance(City a, City b) {
            double dx = a.Lon - b.Lon;
            double dy = a.Lat - b.Lat;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        private List<string> ReconstructPath(Dictionary<string, string> parentMap, string start, string target) {
            var path = new List<string>();
            string current = target;

            while(current != start) {
                path.Add(current);
                current = parentMap[current];
            }

            path.Add(start);
            path.Reverse();

            return path;
        }

        private double CalculatePathDistance(List<string> path) {
            double distance = 0;

            for(int i = 0; i < path.Count - 1; i++) {
                var cityA = GeorgiaMap.Cities.First(c => c.Name == path[i]);
                var cityB = GeorgiaMap.Cities.First(c => c.Name == path[i + 1]);

                distance += CalculateDistance(cityA, cityB);
            }

            return distance;
        }

        private class FScoreComparer : IComparer<(string Name, double FScore)> {
            public int Compare((string Name, double FScore) x, (string Name, double FScore) y) {
                int result = x.FScore.CompareTo(y.FScore);

                if (result == 0) {
                    return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
                }

                return result;
            }
        }
    }
}
