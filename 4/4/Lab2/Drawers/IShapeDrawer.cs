using Lab2;
using System.Drawing;

namespace Lab2
{
    /// <summary>
    /// Renderer interface for shape types.
    /// Each shape type has a dedicated drawer that knows how to
    /// paint that shape onto a GDI+ Graphics surface.
    /// Keeps rendering logic separate from shape data (SRP).
    /// </summary>
    public interface IShapeDrawer
    {
        /// <summary>
        /// Renders the given shape onto the provided Graphics context.
        /// Implementations must cast <paramref name="shape"/> to their
        /// concrete type before accessing type-specific properties.
        /// </summary>
        void Draw(Shape shape, Graphics g);
    }
}