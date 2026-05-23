using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace Lab2
{
    public class ShapeFactory
    {
        private Dictionary<string, IShapeCreator> creators = new Dictionary<string, IShapeCreator>();
        private Dictionary<string, IShapeDrawer> drawers = new Dictionary<string, IShapeDrawer>();
        private string pluginsPath;

        public ShapeFactory()
        {
            // Set plugins folder
            pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

            // Create plugins folder if it doesn't exist
            if (!Directory.Exists(pluginsPath))
            {
                Directory.CreateDirectory(pluginsPath);
                System.Windows.Forms.MessageBox.Show($"Created plugins folder at: {pluginsPath}");
            }

            // Load built-in shapes FIRST (so they always appear)
            LoadBuiltInShapes();

            // Then load plugins (adds new shapes)
            LoadPlugins();
        }

        /// <summary>
        /// Load built-in shapes (Line, Rectangle, etc.)
        /// </summary>
        private void LoadBuiltInShapes()
        {
            // Built-in creators
            var builtInCreators = new IShapeCreator[]
            {
                new LineCreator(),
                new RectangleCreator(),
                new SquareCreator(),
                new EllipseCreator(),
                new CircleCreator(),
                new TriangleCreator()
            };

            foreach (var creator in builtInCreators)
            {
                string name = creator.GetName();
                if (!creators.ContainsKey(name))
                {
                    creators[name] = creator;
                }
            }

            // Built-in drawers - we need to register them too
            // For now, we'll create them on demand in GetDrawer
        }

        /// <summary>
        /// Load plugins from the plugins folder
        /// </summary>
        private void LoadPlugins()
        {
            if (!Directory.Exists(pluginsPath)) return;

            string[] dllFiles = Directory.GetFiles(pluginsPath, "*.dll");

            foreach (string dllPath in dllFiles)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dllPath);

                    foreach (Type type in assembly.GetTypes())
                    {
                        if (typeof(IShapePlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        {
                            IShapePlugin plugin = (IShapePlugin)Activator.CreateInstance(type);
                            string shapeName = plugin.GetShapeName();

                            if (!creators.ContainsKey(shapeName))
                            {
                                creators[shapeName] = plugin.GetCreator();
                                drawers[shapeName] = plugin.GetDrawer();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Failed to load {Path.GetFileName(dllPath)}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Get all shape names for the dropdown menu
        /// </summary>
        public string[] GetAllNames()
        {
            string[] names = new string[creators.Count];
            creators.Keys.CopyTo(names, 0);
            return names;
        }

        /// <summary>
        /// Create a shape by name using two points
        /// </summary>
        public Shape CreateShape(string name, Point start, Point end)
        {
            if (creators.ContainsKey(name))
            {
                return creators[name].Create(start, end);
            }
            return null;
        }

        /// <summary>
        /// Get drawer for a shape
        /// </summary>
        public IShapeDrawer GetDrawer(Shape shape)
        {
            if (shape != null && drawers.ContainsKey(shape.Name))
            {
                return drawers[shape.Name];
            }

            // If not found in plugin drawers, create built-in drawer on the fly
            return CreateBuiltInDrawer(shape);
        }

        private IShapeDrawer CreateBuiltInDrawer(Shape shape)
        {
            switch (shape.Name)
            {
                case "Line": return new LineDrawer();
                case "Rectangle": return new RectangleDrawer();
                case "Square": return new SquareDrawer();
                case "Ellipse": return new EllipseDrawer();
                case "Circle": return new CircleDrawer();
                case "Triangle": return new TriangleDrawer();
                default: return null;
            }
        }
    }
}