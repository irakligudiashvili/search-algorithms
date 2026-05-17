using System.Diagnostics;
using SearchAlgorithms.Data;
using SearchAlgorithms.Models;

namespace SearchAlgorithms.Services {
    public class BreadthFirstSearch : ISearchAlgorithm {
        public string Name => "Breadth-First Search (BFS)";

        public SearchResult FindPath(string startCity, string targetCity) {
            var result = new SearchResult();
            var stopwatch = Stopwatch.StartNew();

            if(!GeorgiaMap.Cities.Any(c => c.Name == startCity) || !GeorgiaMap.Cities.Any(c => c.Name == targetCity)) {
                stopwatch.Stop();
                return result;
            }

            var queue = new Queue<string>();
            var visited = new HashSet<string>();
            var parentMap = new Dictionary<string, string>();

            queue.Enqueue(startCity);
            visited.Add(startCity);

            while (queue.Count > 0) {
                string current = queue.Dequeue();
                result.VisitedOrder.Add(current);

                if(current == targetCity) {
                    break;
                }

                var neighbors = GetNeighbors(current);

                foreach(var neighbor in neighbors) {
                    if (!visited.Contains(neighbor)) {
                        visited.Add(neighbor);
                        parentMap[neighbor] = current;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;

            if (result.VisitedOrder.Contains(targetCity)) {
                result.Path = ReconstructPath(parentMap, startCity, targetCity);
                result.TotalDistance = CalculatePathDistance(result.Path);
            }

            return result;
        }

        private List<string> GetNeighbors(string city) {
            var neighbors = GeorgiaMap.Roads
                .Where(r => r.StartCity == city)
                .Select(r => r.EndCity)
                .Concat(GeorgiaMap.Roads.Where(r => r.EndCity == city).Select(r => r.StartCity))
                .Distinct()
                .ToList();

            return neighbors;
        }

        private List<string> ReconstructPath(Dictionary<string, string> parentMap, string start, string target) {
            var path = new List<string>();
            string current = target;

            while (current != start) {
                path.Add(current);
                current = parentMap[current];
            }

            path.Add(start);
            path.Reverse();
            return path;
        }

        private double CalculatePathDistance(List<string> path) {
            double distance = 0;

            for(int i = 1; i < path.Count - 1; i++) {
                var cityA = GeorgiaMap.Cities.First(c => c.Name == path[i]);
                var cityB = GeorgiaMap.Cities.First(c => c.Name == path[i + 1]);

                double dx = cityA.Lon - cityB.Lon;
                double dy = cityA.Lat - cityB.Lat;
                distance += Math.Sqrt(dx * dx + dy * dy);
            }

            return distance;
        }
    }
}
