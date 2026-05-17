using System.Diagnostics;
using SearchAlgorithms.Data;
using SearchAlgorithms.Models;

namespace SearchAlgorithms.Services {
    public class IterativeDeepeningBidirectionalDFS : ISearchAlgorithm {
        public string Name => "ID-Bidirectional DFS";
        public SearchResult FindPath(string startCity, string targetCity) {
            var result = new SearchResult();
            var stopwatch = Stopwatch.StartNew();

            if(startCity == targetCity) {
                result.Path = new List<string> { startCity };
                result.VisitedOrder.Add(startCity);
                stopwatch.Stop();
                
                return result;
            }

            int depthLimit = 0;
            bool intersectionFound = false;
            string intersectionNode = null;

            var forwardParentMap = new Dictionary<string, string>();
            var backwardParentMap = new Dictionary<string, string>();

            var chronologicalOrder = new List<string>();
            var visitCounts = new Dictionary<string, int>();

            int maxPossibleDepth = GeorgiaMap.Cities.Count;

            while(depthLimit <= maxPossibleDepth && !intersectionFound) {
                var forwardVisited = new HashSet<string>();
                var backwardVisited = new HashSet<string>();

                forwardParentMap.Clear();
                backwardParentMap.Clear();

                DepthLimitedStackSearch(startCity, depthLimit, forwardVisited, forwardParentMap, chronologicalOrder, visitCounts);

                DepthLimitedStackSearch(targetCity, depthLimit, backwardVisited, backwardParentMap, chronologicalOrder, visitCounts);

                var intersection = forwardVisited.Intersect(backwardVisited).FirstOrDefault();

                if(intersection != null) {
                    intersectionFound = true;
                    intersectionNode = intersection;
                    break;
                }

                depthLimit++;
            }

            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;

            result.VisitedOrder = chronologicalOrder.Distinct().ToList();

            if(intersectionFound && intersectionNode != null) {
                result.Path = ReconstructBidirectionalPath(forwardParentMap, backwardParentMap, startCity, targetCity, intersectionNode);
                result.TotalDistance = CalculatePathDistance(result.Path);
            }

            return result;
        }

        private void DepthLimitedStackSearch(string current, int limit, HashSet<string> visitedThisIteration, Dictionary<string, string> parentMap, List<string> chronologicalOrder, Dictionary<string, int> visitCounts) {
            var stack = new Stack<(string Node, int Depth, string Parent)>();

            var bestDepthThisIteration = new Dictionary<string, int>();

            stack.Push((current, 0, null));
            bestDepthThisIteration[current] = 0;

            while(stack.Count > 0) {
                var (node, depth, parent) = stack.Pop();

                if(visitedThisIteration.Contains(node) && depth > bestDepthThisIteration[node]) {
                    continue;
                }

                chronologicalOrder.Add(node);

                if (visitCounts.ContainsKey(node)) {
                    visitCounts[node]++;
                } else {
                    visitCounts[node] = 1;
                }

                visitedThisIteration.Add(node);

                if(parent != null) {
                    if(!parentMap.ContainsKey(node) || depth <= bestDepthThisIteration[node]) {
                        parentMap[node] = parent;
                        bestDepthThisIteration[node] = depth;
                    }
                }

                if (depth < limit) {
                    foreach (var neighbor in GetNeighbors(node)) {
                        if(!bestDepthThisIteration.ContainsKey(neighbor) || (depth + 1) < bestDepthThisIteration[neighbor]) {
                            bestDepthThisIteration[neighbor] = depth + 1;
                            stack.Push((neighbor, depth + 1, node));
                        }
                    }
                }
            }
        }

        private List<string> GetNeighbors(string city) {
            return GeorgiaMap.Roads
                .Where(r => r.StartCity == city)
                .Select(r => r.EndCity)
                .Concat(GeorgiaMap.Roads.Where(r => r.EndCity == city).Select(r => r.StartCity))
                .Distinct()
                .ToList();
        }

        private List<string> ReconstructBidirectionalPath(Dictionary<string, string> forwardMap, Dictionary<string, string> backwardMap, string start, string target, string intersection) {
            var pathFromStart = new List<string>();
            string curr = intersection;
            
            while(curr != start && curr != null) {
                pathFromStart.Add(curr);
                forwardMap.TryGetValue(curr, out curr);
            }

            pathFromStart.Add(start);
            pathFromStart.Reverse();

            var pathToTarget = new List<string>();
            curr = intersection;
            backwardMap.TryGetValue(curr, out curr);

            while(curr != target && curr != null) {
                pathToTarget.Add(curr);
                backwardMap.TryGetValue(curr, out curr);
            }

            if(intersection != target) {
                pathToTarget.Add(target);
            }

            pathFromStart.AddRange(pathToTarget);
            return pathFromStart;
        }

        private double CalculatePathDistance(List<string> path) {
            double distance = 0;

            for(int i = 0; i < path.Count - 1; i++) {
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
