namespace ClassLibrary
{
    public struct SplineDataItem
    {
        public double x { get; set; }
        public double y { get; set; }
        public double y1 { get; set; }
        public double y2 { get; set; }

        public SplineDataItem(double x, double y, double y1, double y2)
        {
            this.x = x;
            this.y = y;
            this.y1 = y1;
            this.y2 = y2;
        }
        public string ToLongString(string format, bool first = true)
        {
            return $"x = {x.ToString(format)}, y = {y.ToString(format)}, " +
                   (first? $"\ny' = {y1.ToString(format)}, " : "") +
                   $"y'' = {y2.ToString(format)}";
        }

        public override string ToString()
        {
            return $"x = {x}, y = {y}, y' = {y1}, y'' = {y2}";
        }
    }
}
