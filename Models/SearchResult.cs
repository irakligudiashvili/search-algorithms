namespace SearchAlgorithms.Models {
    public class SearchResult {
        public List<string> Path { get; set; } = new();
        public List<string> VisitedOrder { get; set; } = new();
        public TimeSpan ExecutionTime { get; set; }
        public int NodesExploredCount => VisitedOrder.Count;
        public double TotalDistance { get; set; }
    }
}
