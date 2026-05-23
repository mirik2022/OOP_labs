using System.Drawing;

namespace Lab2
{
    /// <summary>
    /// Abstract base class for all geometric shapes.
    /// Contains only data properties — no drawing logic.
    /// Drawing is delegated to IShapeDrawer implementations.
    /// </summary>
    public abstract class Shape
    {
        public string Name { get; protected set; }

        // Color needs to be serializable - use ARGB int
        public int ColorArgb { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public Color Color
        {
            get => Color.FromArgb(ColorArgb);
            set => ColorArgb = value.ToArgb();
        }

        protected Shape()
        {
            ColorArgb = Color.Black.ToArgb();
        }
    }
}