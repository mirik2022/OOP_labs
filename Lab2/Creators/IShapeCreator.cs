using Lab2;
using System.Drawing;

namespace Lab2
{
    /// <summary>
    /// Factory interface for creating shape instances.
    /// Each shape type has its own creator that constructs the shape
    /// from two mouse points (drag-start and drag-end).
    /// </summary>
    public interface IShapeCreator
    {
        /// <summary>
        /// Returns the display name shown in the UI shape dropdown.
        /// Must be unique across all registered creators.
        /// </summary>
        string GetName();

        /// <summary>
        /// Creates and returns a new shape instance from two mouse points.
        /// </summary>
        /// <param name="start">Mouse-down point (drag origin).</param>
        /// <param name="end">Mouse-up point (drag destination).</param>
        Shape Create(Point start, Point end);
    }
}