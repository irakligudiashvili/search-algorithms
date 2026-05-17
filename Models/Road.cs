namespace SearchAlgorithms.Models {
    public class Road {
        public string StartCity { get; set; }
        public string EndCity { get; set; }
        public double ControlX { get; set; }
        public double ControlY { get; set; }

        public Road(string startCity, string endCity) {
            StartCity = startCity;
            EndCity = endCity;
        }
    }
}
