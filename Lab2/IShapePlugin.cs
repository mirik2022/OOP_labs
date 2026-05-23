// IShapePlugin.cs - поместить в основной проект
namespace Lab2
{
    /// <summary>
    /// Main interface that every shape plugin must implement.
    /// A plugin provides both a creator and a drawer for a shape.
    /// </summary>
    public interface IShapePlugin
    {
        /// <summary>
        /// Returns the name of the shape (e.g., "Star", "Hexagon")
        /// </summary>
        string GetShapeName();

        /// <summary>
        /// Creates a new instance of the shape creator
        /// </summary>
        IShapeCreator GetCreator();

        /// <summary>
        /// Creates a new instance of the shape drawer
        /// </summary>
        IShapeDrawer GetDrawer();
    }
}