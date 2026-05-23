using Lab2.Adapters;
using Lab2.Creators;
using Lab2.Drawers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Lab2
{
    /// <summary>
    /// Manages plugin loading with signature verification
    /// </summary>
    public class PluginManager
    {
        private Dictionary<string, IShapeCreator> creators = new Dictionary<string, IShapeCreator>();
        private Dictionary<string, IShapeDrawer> drawers = new Dictionary<string, IShapeDrawer>();
        private List<string> loadedPlugins = new List<string>();
        private List<string> rejectedPlugins = new List<string>();

        private SignatureValidator validator;
        private string pluginsPath;

        public PluginManager()
        {
            LoadBuiltInShapes();
            LoadPlugins();                      // Load shape plugins (Star, Hexagon)
            LoadComradePluginsWithAdapter();    // Load and adapt comrade's plugin
            validator = new SignatureValidator();
            pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

            // Create plugins folder if it doesn't exist
            if (!Directory.Exists(pluginsPath))
            {
                Directory.CreateDirectory(pluginsPath);
            }
        }

        /// <summary>
        /// Load all plugins with signature verification
        /// </summary>
        /// 
        /// <summary>
        /// Load and adapt comrade's student plugins
        /// </summary>
        private void LoadAdaptedStudentPlugins()
        {
            string pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "student_plugins");

            if (!Directory.Exists(pluginsPath))
                Directory.CreateDirectory(pluginsPath);

            foreach (string dllPath in Directory.GetFiles(pluginsPath, "*.dll"))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dllPath);

                    foreach (Type type in assembly.GetTypes())
                    {
                        // Look for IPlugin interface (from comrade's code)
                        var pluginInterface = type.GetInterface("IPlugin");
                        if (pluginInterface != null && !type.IsInterface && !type.IsAbstract)
                        {
                            object plugin = Activator.CreateInstance(type);

                            // Get plugin name
                            var nameProp = type.GetProperty("PluginName");
                            string pluginName = nameProp != null ? (string)nameProp.GetValue(plugin) : type.Name;
                            string shapeName = pluginName.Replace("Plugin", "").Trim();

                            // Register as shape
                            if (!creators.ContainsKey(shapeName))
                            {
                                creators[shapeName] = new StudentCreator(shapeName, plugin);
                                drawers[shapeName] = new StudentDrawer();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load comrade's plugin: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Load comrade's plugins and adapt them using Adapter pattern
        /// </summary>
        private void LoadComradePluginsWithAdapter()
        {
            string pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "comrade_plugins");

            if (!Directory.Exists(pluginsPath))
                Directory.CreateDirectory(pluginsPath);

            foreach (string dllPath in Directory.GetFiles(pluginsPath, "*.dll"))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dllPath);

                    foreach (Type type in assembly.GetTypes())
                    {
                        // Look for IPlugin interface (comrade's interface)
                        var pluginInterface = type.GetInterface("IPlugin");
                        if (pluginInterface != null && !type.IsInterface && !type.IsAbstract)
                        {
                            object plugin = Activator.CreateInstance(type);

                            // Get plugin name
                            var nameProp = type.GetProperty("PluginName");
                            string pluginName = nameProp != null ? (string)nameProp.GetValue(plugin) : type.Name;
                            string shapeName = pluginName.Replace("Plugin", "").Trim();

                            // Wrap with Adapter pattern
                            var adaptedPlugin = new StudentPluginAdapter(plugin, shapeName);

                            // Register adapted plugin
                            if (!creators.ContainsKey(shapeName))
                            {
                                creators[shapeName] = adaptedPlugin.GetCreator();
                                drawers[shapeName] = adaptedPlugin.GetDrawer();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load comrade plugin: {ex.Message}");
                }
            }
        }
        public void LoadPlugins()
        {
            // FIRST: Load built-in shapes (always available)
            LoadBuiltInShapes();

            // THEN: Load plugins from folder
            if (!Directory.Exists(pluginsPath)) return;

            string[] dllFiles = Directory.GetFiles(pluginsPath, "*.dll");

            foreach (string dllPath in dllFiles)
            {
                string pluginName = Path.GetFileName(dllPath);

                try
                {
                    // Check if signature file exists (for signed plugins)
                    string sigPath = dllPath + ".sig";
                    bool hasSignature = File.Exists(sigPath);

                    // Load assembly
                    Assembly assembly = Assembly.LoadFrom(dllPath);

                    // Find plugin classes
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (typeof(IShapePlugin).IsAssignableFrom(type) &&
                            !type.IsInterface && !type.IsAbstract)
                        {
                            // For signed plugins, verify signature
                            if (hasSignature && typeof(ISignedShapePlugin).IsAssignableFrom(type))
                            {
                                string signatureBase64 = File.ReadAllText(sigPath).Trim();
                                var signedPlugin = (ISignedShapePlugin)Activator.CreateInstance(type);

                                try
                                {
                                    if (validator.VerifyPlugin(dllPath, signatureBase64, signedPlugin.GetExpirationDate()))
                                    {
                                        RegisterPlugin(signedPlugin);
                                        loadedPlugins.Add($"{pluginName} (signed, expires: {(signedPlugin.GetExpirationDate()?.ToShortDateString() ?? "never")})");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    rejectedPlugins.Add($"{pluginName} - Signature verification failed: {ex.Message}");
                                }
                            }
                            else if (!hasSignature)
                            {
                                // Unsigned plugin - load anyway (or reject if you want strict)
                                var plugin = (IShapePlugin)Activator.CreateInstance(type);
                                RegisterPlugin(plugin);
                                loadedPlugins.Add($"{pluginName} (unsigned)");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    rejectedPlugins.Add($"{pluginName} - {ex.Message}");
                }
            }
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
                    loadedPlugins.Add($"Built-in: {name}");
                }
            }
        }

        /// <summary>
        /// Register a plugin
        /// </summary>
        private void RegisterPlugin(IShapePlugin plugin)
        {
            string shapeName = plugin.GetShapeName();

            if (!creators.ContainsKey(shapeName))
            {
                creators[shapeName] = plugin.GetCreator();
                drawers[shapeName] = plugin.GetDrawer();
            }
        }

        /// <summary>
        /// Get all shape names (built-in + plugins)
        /// </summary>
        public string[] GetAllShapeNames()
        {
            string[] names = new string[creators.Count];
            creators.Keys.CopyTo(names, 0);
            return names;
        }

        /// <summary>
        /// Get creator by shape name
        /// </summary>
        public IShapeCreator GetCreator(string name)
        {
            if (creators.ContainsKey(name))
            {
                return creators[name];
            }
            return null;
        }

        /// <summary>
        /// Get drawer for shape
        /// </summary>
        public IShapeDrawer GetDrawer(Shape shape)
        {
            if (shape != null && drawers.ContainsKey(shape.Name))
            {
                return drawers[shape.Name];
            }

            // Fallback: create drawer on the fly
            return CreateBuiltInDrawer(shape);
        }

        /// <summary>
        /// Create built-in drawer for a shape
        /// </summary>
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

        /// <summary>
        /// Get list of successfully loaded plugins
        /// </summary>
        public string[] GetLoadedPlugins()
        {
            return loadedPlugins.ToArray();
        }

        /// <summary>
        /// Get list of rejected plugins with reasons
        /// </summary>
        public string[] GetRejectedPlugins()
        {
            return rejectedPlugins.ToArray();
        }

        /// <summary>
        /// Get plugin load status summary
        /// </summary>
        public string GetStatusReport()
        {
            string report = $"Loaded shapes ({loadedPlugins.Count}):\n";
            foreach (var plugin in loadedPlugins)
            {
                report += $"  ✅ {plugin}\n";
            }

            report += $"\nRejected ({rejectedPlugins.Count}):\n";
            foreach (var plugin in rejectedPlugins)
            {
                report += $"  ❌ {plugin}\n";
            }

            return report;
        }
    }
}