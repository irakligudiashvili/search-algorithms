namespace SearchAlgorithms.Models {
    public class Road {
        public string StartCity { get; set; }
        public string EndCity { get; set; }
        public double ControlX { get; set; }
        public double ControlY { get; set; }
        public double Distance { get; set; }

        public Road(string startCity, string endCity) {
            StartCity = startCity;
            EndCity = endCity;
        }

        public void RecalculateBezierDistance(double startX, double startY, double endX, double endY) {
            double length = 0;
            int segments = 10;
            double prevX = startX;
            double prevY = startY;

            for(int i = 1; i <= segments; i++) {
                double t = (double)i / segments;

                double currentX = Math.Pow(1 - t, 2) * startX + 2 * (1 - t) * t * ControlX + Math.Pow(t, 2) * endX;
                double currentY = Math.Pow(1 - t, 2) * startY + 2 * (1 - t) * t * ControlY + Math.Pow(t, 2) * endY;

                double dx = currentX - prevX;
                double dy = currentY - prevY;
                length += Math.Sqrt(dx * dx + dy * dy);

                prevX = currentX;
                prevY = currentY;
            }

            double pixelToKmScale = 0.306;
            this.Distance = length * pixelToKmScale;
        }
    }
}
