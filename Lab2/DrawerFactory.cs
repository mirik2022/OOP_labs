using System;


namespace Lab2
{
    /// Factory class that provides drawers for shapes
    /// Separates drawing logic from shape classes
    public class DrawerFactory
    {
        // Array of drawer types - matches shape types
        private Type[] drawerTypes = new Type[]
        {
            typeof(LineDrawer),
            typeof(RectangleDrawer),
            typeof(SquareDrawer),
            typeof(EllipseDrawer),
            typeof(CircleDrawer),
            typeof(TriangleDrawer)
        };

        /// Get appropriate drawer for a shape
        /// Uses shape name to find matching drawer
        public IShapeDrawer GetDrawer(Shape shape)
        {
            string shapeName = shape.Name;

            // Find drawer by shape name
            foreach (Type type in drawerTypes)
            {
                if (type.Name.StartsWith(shapeName))
                {
                    return (IShapeDrawer)Activator.CreateInstance(type);
                }
            }

            return null;
        }
    }
}