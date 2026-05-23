// ShapeList.cs - изменённая версия
using Lab2;

public class ShapeList
{
    private List<Shape> shapes = new List<Shape>();

    // No more DrawerFactory! Drawers come from ShapeFactory now

    public void Add(Shape shape)
    {
        shapes.Add(shape);
    }

    public void Clear()
    {
        shapes.Clear();
    }
    public List<Shape> GetAllShapes()
    {
        return shapes;
    }

    /// Draw all shapes using drawers from the factory
    public void DrawAll(Graphics g, ShapeFactory factory)
    {
        foreach (Shape shape in shapes)
        {
            IShapeDrawer drawer = factory.GetDrawer(shape);
            if (drawer != null)
            {
                drawer.Draw(shape, g);
            }
        }
    }
}