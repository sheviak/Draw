namespace Draw.Entities
{
    public class NumProperties : BaseEntity
    {
        public int Rotate { get; set; }
        public int ShiftX { get; set; }
        public int ShiftY { get; set; }
        public double ScaleY { get; set; }
        public double ScaleX { get; set; }
        public int SkewX { get; set;}
        public int SkewY { get; set; }

        public int NumberId { get; set; }
        public Number Number { get; set; }
    }
}