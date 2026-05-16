namespace SearchAlgorithms.Models {
    public class City {
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }

        public City(string name, double lat, double lon) {
            Name = name;
            Lat = lat;
            Lon = lon;
        }
    }
}
