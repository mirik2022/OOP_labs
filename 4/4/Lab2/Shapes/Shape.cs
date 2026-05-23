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
        /// <summary>Display name of the shape, set by each concrete subclass.</summary>
        public string Name { get; protected set; } = string.Empty;

        /// <summary>Drawing color applied when rendering this shape.</summary>
        public Color Color { get; set; } = Color.Black;
    }
}