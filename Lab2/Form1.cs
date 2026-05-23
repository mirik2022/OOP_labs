using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Lab2
{
    public partial class Form1 : Form
    {
        // Core components
        private List<IBeforeSavePlugin> beforeSavePlugins = new List<IBeforeSavePlugin>();
        private List<IAfterLoadPlugin> afterLoadPlugins = new List<IAfterLoadPlugin>();
        private ShapeList shapeList;
        private Shape currentShape;
        private Point startPoint;
        private bool isDrawing = false;
        private string selectedShape;
        private Color selectedColor = Color.Black;
        private PluginManager pluginManager;
        private Button statusButton;
        private Label pluginStatusLabel;

        // UI Controls
        private ComboBox shapeBox;
        private ComboBox colorBox;
        private Button clearButton;
        private Label statusLabel;
        private Label shapeLabel;
        private Label colorLabel;

        public Form1()
        {
            InitializeComponent();

            // Form settings
            this.Text = "Simple Graphic Editor - Lab 5 (XSLT Transformations)";
            this.Size = new Size(900, 600);
            this.DoubleBuffered = true;
            this.BackColor = Color.White;

            // Initialize components
            shapeList = new ShapeList();

            // Use PluginManager
            pluginManager = new PluginManager();
            pluginManager.LoadPlugins();

            // Load XSLT data processing plugins
            LoadDataProcessingPlugins();

            // Setup UI
            CreateMenuBar();
            CreateControls();
            SetupEventHandlers();
        }

        private void LoadDataProcessingPlugins()
        {
            string pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data_plugins");

            // Show debug message
            MessageBox.Show($"Looking for plugins in: {pluginsPath}");

            if (!Directory.Exists(pluginsPath))
            {
                MessageBox.Show($"Creating plugins folder: {pluginsPath}");
                Directory.CreateDirectory(pluginsPath);
            }

            string[] dllFiles = Directory.GetFiles(pluginsPath, "*.dll");
            MessageBox.Show($"Found {dllFiles.Length} DLL files");

            foreach (string dllPath in dllFiles)
            {
                try
                {
                    MessageBox.Show($"Loading: {Path.GetFileName(dllPath)}");
                    Assembly assembly = Assembly.LoadFrom(dllPath);

                    foreach (Type type in assembly.GetTypes())
                    {
                        if (typeof(IBeforeSavePlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        {
                            IBeforeSavePlugin plugin = (IBeforeSavePlugin)Activator.CreateInstance(type);
                            beforeSavePlugins.Add(plugin);
                            MessageBox.Show($"Loaded before-save plugin: {plugin.GetName()}");
                        }
                        if (typeof(IAfterLoadPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        {
                            IAfterLoadPlugin plugin = (IAfterLoadPlugin)Activator.CreateInstance(type);
                            afterLoadPlugins.Add(plugin);
                            MessageBox.Show($"Loaded after-load plugin: {plugin.GetName()}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load {Path.GetFileName(dllPath)}: {ex.Message}");
                }
            }

            MessageBox.Show($"Total plugins loaded: {beforeSavePlugins.Count}");
        }

        /// <summary>
        /// Save shapes to XML file
        /// </summary>
        private void SaveToFile()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "XML Files|*.xml";
                sfd.Title = "Save Shapes";
                sfd.FileName = "shapes.xml";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // 1. Конвертируем фигуры в XML
                    string xmlData = SerializeToXml();

                    // Apply XSLT plugins
                    foreach (var plugin in beforeSavePlugins)
                    {
                        if (plugin.IsEnabled)
                        {
                            xmlData = plugin.ProcessBeforeSave(xmlData);
                        }
                    }

                    File.WriteAllText(sfd.FileName, xmlData);
                    UpdateStatus($"Saved {shapeList.GetAllShapes().Count} shapes to {Path.GetFileName(sfd.FileName)}");
                }
            }
        }

        /// <summary>
        /// Serialize shapes to simple XML format
        /// </summary>
        /// <summary>
        /// Serialize shapes to simple XML format
        /// </summary>
        private string SerializeToXml()
        {
            var shapes = shapeList.GetAllShapes();

            XmlDocument doc = new XmlDocument();
            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(decl);

            XmlElement root = doc.CreateElement("Shapes");
            doc.AppendChild(root);

            foreach (var shape in shapes)
            {
                XmlElement shapeElem = doc.CreateElement("Shape");

                // Save type
                shapeElem.SetAttribute("Type", shape.Name);

                // Save color as NAME, not number!
                XmlElement colorElem = doc.CreateElement("Color");
                colorElem.InnerText = GetColorName(shape.Color);  // ← Исправлено
                shapeElem.AppendChild(colorElem);

                // Save shape data based on type
                if (shape is Line line)
                {
                    XmlElement startElem = doc.CreateElement("Start");
                    startElem.SetAttribute("X", line.Start.X.ToString());
                    startElem.SetAttribute("Y", line.Start.Y.ToString());
                    shapeElem.AppendChild(startElem);

                    XmlElement endElem = doc.CreateElement("End");
                    endElem.SetAttribute("X", line.End.X.ToString());
                    endElem.SetAttribute("Y", line.End.Y.ToString());
                    shapeElem.AppendChild(endElem);
                }
                else if (shape is Rectangle rect)
                {
                    AddIntElement(doc, shapeElem, "X", rect.X);
                    AddIntElement(doc, shapeElem, "Y", rect.Y);
                    AddIntElement(doc, shapeElem, "Width", rect.Width);
                    AddIntElement(doc, shapeElem, "Height", rect.Height);
                }
                else if (shape is Square square)
                {
                    AddIntElement(doc, shapeElem, "X", square.X);
                    AddIntElement(doc, shapeElem, "Y", square.Y);
                    AddIntElement(doc, shapeElem, "Side", square.Side);
                }
                else if (shape is Ellipse ellipse)
                {
                    AddIntElement(doc, shapeElem, "X", ellipse.X);
                    AddIntElement(doc, shapeElem, "Y", ellipse.Y);
                    AddIntElement(doc, shapeElem, "Width", ellipse.Width);
                    AddIntElement(doc, shapeElem, "Height", ellipse.Height);
                }
                else if (shape is Circle circle)
                {
                    AddIntElement(doc, shapeElem, "X", circle.X);
                    AddIntElement(doc, shapeElem, "Y", circle.Y);
                    AddIntElement(doc, shapeElem, "Diameter", circle.Diameter);
                }
                else if (shape is Triangle triangle)
                {
                    XmlElement pointsElem = doc.CreateElement("Points");
                    AddPointElement(doc, pointsElem, "P1", triangle.Points[0]);
                    AddPointElement(doc, pointsElem, "P2", triangle.Points[1]);
                    AddPointElement(doc, pointsElem, "P3", triangle.Points[2]);
                    shapeElem.AppendChild(pointsElem);
                }

                root.AppendChild(shapeElem);
            }

            return doc.OuterXml;
        }

        /// <summary>
        /// Get color name from Color object
        /// </summary>
        private string GetColorName(Color color)
        {
            // Known colors
            if (color == Color.Black) return "Black";
            if (color == Color.Red) return "Red";
            if (color == Color.Blue) return "Blue";
            if (color == Color.Green) return "Green";
            if (color == Color.Yellow) return "Yellow";
            if (color == Color.Orange) return "Orange";
            if (color == Color.Purple) return "Purple";
            if (color == Color.Brown) return "Brown";
            if (color == Color.Gray) return "Gray";

            // Default
            return "Black";
        }

        private void AddIntElement(XmlDocument doc, XmlElement parent, string name, int value)
        {
            XmlElement elem = doc.CreateElement(name);
            elem.InnerText = value.ToString();
            parent.AppendChild(elem);
        }

        private void AddPointElement(XmlDocument doc, XmlElement parent, string name, Point point)
        {
            XmlElement elem = doc.CreateElement(name);
            elem.SetAttribute("X", point.X.ToString());
            elem.SetAttribute("Y", point.Y.ToString());
            parent.AppendChild(elem);
        }

        /// <summary>
        /// Load shapes from XML file
        /// </summary>
        private void LoadFromFile()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "XML Files|*.xml";
                ofd.Title = "Load Shapes";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string xmlData = File.ReadAllText(ofd.FileName);

                    // Apply reverse XSLT plugins
                    for (int i = afterLoadPlugins.Count - 1; i >= 0; i--)
                    {
                        if (afterLoadPlugins[i].IsEnabled)
                        {
                            xmlData = afterLoadPlugins[i].ProcessAfterLoad(xmlData);
                        }
                    }

                    // 3. Конвертируем XML обратно в фигуры
                    if (DeserializeFromXml(xmlData))
                    {
                        Refresh();
                        UpdateStatus($"Loaded {shapeList.GetAllShapes().Count} shapes from {Path.GetFileName(ofd.FileName)}");
                    }
                    else
                    {
                        MessageBox.Show("Failed to load shapes. File format may be incorrect.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Deserialize XML back to shapes
        /// </summary>
        /// <summary>
        /// Deserialize XML back to shapes
        /// </summary>
        private bool DeserializeFromXml(string xmlData)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlData);

                shapeList.Clear();

                XmlNodeList shapeNodes = doc.SelectNodes("//Shape");
                if (shapeNodes == null || shapeNodes.Count == 0)
                {
                    MessageBox.Show("No shapes found in file.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                foreach (XmlNode shapeNode in shapeNodes)
                {
                    string type = shapeNode.Attributes?["Type"]?.Value;
                    if (string.IsNullOrEmpty(type)) continue;

                    // Get color from NAME string
                    Color color = Color.Black;
                    XmlNode colorNode = shapeNode.SelectSingleNode("Color");
                    if (colorNode != null)
                    {
                        string colorName = colorNode.InnerText.Trim();

                        // Convert color name to Color
                        switch (colorName)
                        {
                            case "Black": color = Color.Black; break;
                            case "Red": color = Color.Red; break;
                            case "Blue": color = Color.Blue; break;
                            case "Green": color = Color.Green; break;
                            case "Yellow": color = Color.Yellow; break;
                            case "Orange": color = Color.Orange; break;
                            case "Purple": color = Color.Purple; break;
                            case "Brown": color = Color.Brown; break;
                            case "Gray": color = Color.Gray; break;
                            default: color = Color.Black; break;
                        }
                    }

                    Shape shape = null;

                    switch (type)
                    {
                        case "Line":
                            Point start = GetPointFromNode(shapeNode, "Start");
                            Point end = GetPointFromNode(shapeNode, "End");
                            shape = new Line(start, end);
                            break;

                        case "Rectangle":
                            int x = GetIntFromNode(shapeNode, "X");
                            int y = GetIntFromNode(shapeNode, "Y");
                            int w = GetIntFromNode(shapeNode, "Width");
                            int h = GetIntFromNode(shapeNode, "Height");
                            shape = new Rectangle(x, y, w, h);
                            break;

                        case "Square":
                            int sx = GetIntFromNode(shapeNode, "X");
                            int sy = GetIntFromNode(shapeNode, "Y");
                            int side = GetIntFromNode(shapeNode, "Side");
                            shape = new Square(sx, sy, side);
                            break;

                        case "Ellipse":
                            int ex = GetIntFromNode(shapeNode, "X");
                            int ey = GetIntFromNode(shapeNode, "Y");
                            int ew = GetIntFromNode(shapeNode, "Width");
                            int eh = GetIntFromNode(shapeNode, "Height");
                            shape = new Ellipse(ex, ey, ew, eh);
                            break;

                        case "Circle":
                            int cx = GetIntFromNode(shapeNode, "X");
                            int cy = GetIntFromNode(shapeNode, "Y");
                            int diam = GetIntFromNode(shapeNode, "Diameter");
                            shape = new Circle(cx, cy, diam);
                            break;

                        case "Triangle":
                            Point p1 = GetPointFromNode(shapeNode, "Points/P1");
                            Point p2 = GetPointFromNode(shapeNode, "Points/P2");
                            Point p3 = GetPointFromNode(shapeNode, "Points/P3");
                            shape = new Triangle(p1, p2, p3);
                            break;
                    }

                    if (shape != null)
                    {
                        shape.Color = color;
                        shapeList.Add(shape);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Deserialize error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private Point GetPointFromNode(XmlNode parent, string xpath)
        {
            XmlNode node = parent.SelectSingleNode(xpath);
            if (node == null) return new Point(0, 0);

            int x = 0, y = 0;
            if (node.Attributes?["X"] != null)
                int.TryParse(node.Attributes["X"].Value, out x);
            if (node.Attributes?["Y"] != null)
                int.TryParse(node.Attributes["Y"].Value, out y);
            return new Point(x, y);
        }

        private int GetIntFromNode(XmlNode parent, string xpath)
        {
            XmlNode node = parent.SelectSingleNode(xpath);
            if (node == null) return 0;

            int value = 0;
            int.TryParse(node.InnerText, out value);
            return value;
        }

        private void CreateMenuBar()
        {
            MenuStrip menuStrip = new MenuStrip();

            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");

            ToolStripMenuItem saveItem = new ToolStripMenuItem("Save");
            saveItem.Click += (s, e) => SaveToFile();
            saveItem.ShortcutKeys = Keys.Control | Keys.S;

            ToolStripMenuItem loadItem = new ToolStripMenuItem("Load");
            loadItem.Click += (s, e) => LoadFromFile();
            loadItem.ShortcutKeys = Keys.Control | Keys.O;

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => Application.Exit();

            fileMenu.DropDownItems.Add(saveItem);
            fileMenu.DropDownItems.Add(loadItem);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(exitItem);

            ToolStripMenuItem pluginsMenu = new ToolStripMenuItem("Data Plugins (XSLT)");

            foreach (var plugin in beforeSavePlugins)
            {
                ToolStripMenuItem pluginItem = new ToolStripMenuItem(plugin.GetName());
                pluginItem.Checked = plugin.IsEnabled;
                pluginItem.CheckOnClick = true;
                pluginItem.Tag = plugin;
                pluginItem.Click += (sender, e) =>
                {
                    var item = sender as ToolStripMenuItem;
                    var p = item.Tag as IBeforeSavePlugin;
                    if (p != null)
                    {
                        p.IsEnabled = item.Checked;
                        UpdateStatus($"{p.GetName()} is {(p.IsEnabled ? "enabled" : "disabled")}");
                    }
                };

                ToolStripMenuItem settingsItem = new ToolStripMenuItem("Settings...");
                settingsItem.Click += (s, e) => plugin.ShowSettings();
                pluginItem.DropDownItems.Add(settingsItem);
                pluginsMenu.DropDownItems.Add(pluginItem);
            }

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(pluginsMenu);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void CreateControls()
        {
            int bottomMargin = 80;

            shapeLabel = new Label();
            shapeLabel.Text = "Shape:";
            shapeLabel.Location = new Point(10, this.Height - bottomMargin);
            shapeLabel.Size = new Size(50, 25);
            this.Controls.Add(shapeLabel);

            shapeBox = new ComboBox();
            shapeBox.Location = new Point(70, this.Height - bottomMargin);
            shapeBox.Size = new Size(120, 25);
            shapeBox.DropDownStyle = ComboBoxStyle.DropDownList;

            string[] shapeNames = pluginManager.GetAllShapeNames();
            if (shapeNames.Length == 0)
            {
                shapeBox.Items.Add("Line");
                shapeBox.Items.Add("Rectangle");
                shapeBox.Items.Add("Square");
                shapeBox.Items.Add("Ellipse");
                shapeBox.Items.Add("Circle");
                shapeBox.Items.Add("Triangle");
            }
            else
            {
                foreach (string name in shapeNames)
                    shapeBox.Items.Add(name);
            }

            if (shapeBox.Items.Count > 0)
                shapeBox.SelectedIndex = 0;
            this.Controls.Add(shapeBox);

            colorLabel = new Label();
            colorLabel.Text = "Color:";
            colorLabel.Location = new Point(200, this.Height - bottomMargin);
            colorLabel.Size = new Size(40, 25);
            this.Controls.Add(colorLabel);

            colorBox = new ComboBox();
            colorBox.Location = new Point(240, this.Height - bottomMargin);
            colorBox.Size = new Size(100, 25);
            colorBox.DropDownStyle = ComboBoxStyle.DropDownList;

            string[] colors = { "Black", "Red", "Blue", "Green", "Yellow", "Orange", "Purple", "Brown", "Gray" };
            foreach (string color in colors)
                colorBox.Items.Add(color);
            colorBox.SelectedIndex = 0;
            colorBox.SelectedIndexChanged += (s, e) =>
            {
                selectedColor = Color.FromName(colorBox.SelectedItem.ToString());
            };
            this.Controls.Add(colorBox);

            clearButton = new Button();
            clearButton.Text = "Clear";
            clearButton.Location = new Point(350, this.Height - bottomMargin);
            clearButton.Size = new Size(80, 25);
            clearButton.Click += (s, e) =>
            {
                shapeList.Clear();
                Refresh();
                UpdateStatus("Canvas cleared");
            };
            this.Controls.Add(clearButton);

            statusButton = new Button();
            statusButton.Text = "Plugin Status";
            statusButton.Location = new Point(440, this.Height - bottomMargin);
            statusButton.Size = new Size(100, 25);
            statusButton.Click += (s, e) =>
            {
                string report = "=== XSLT Plugins ===\n";
                foreach (var p in beforeSavePlugins)
                {
                    report += $"\n• {p.GetName()}: {(p.IsEnabled ? "Enabled" : "Disabled")}";
                }
                MessageBox.Show(report, "Plugin Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            this.Controls.Add(statusButton);

            pluginStatusLabel = new Label();
            pluginStatusLabel.Text = $"Shapes: {shapeBox.Items.Count} | XSLT Plugins: {beforeSavePlugins.Count}";
            pluginStatusLabel.Location = new Point(550, this.Height - bottomMargin);
            pluginStatusLabel.Size = new Size(350, 25);
            pluginStatusLabel.ForeColor = Color.Green;
            this.Controls.Add(pluginStatusLabel);

            statusLabel = new Label();
            statusLabel.Text = "Draw shapes | File → Save/Load";
            statusLabel.Location = new Point(10, this.Height - 35);
            statusLabel.Size = new Size(500, 25);
            statusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(statusLabel);
        }

        private void SetupEventHandlers()
        {
            shapeBox.SelectedIndexChanged += (s, e) =>
            {
                if (shapeBox.SelectedItem != null)
                {
                    selectedShape = shapeBox.SelectedItem.ToString();
                    UpdateStatus("Selected: " + selectedShape);
                }
            };

            this.Resize += (s, e) =>
            {
                int bottomMargin = 80;
                shapeLabel.Location = new Point(10, this.Height - bottomMargin);
                shapeBox.Location = new Point(70, this.Height - bottomMargin);
                colorLabel.Location = new Point(200, this.Height - bottomMargin);
                colorBox.Location = new Point(240, this.Height - bottomMargin);
                clearButton.Location = new Point(350, this.Height - bottomMargin);
                statusButton.Location = new Point(440, this.Height - bottomMargin);
                pluginStatusLabel.Location = new Point(550, this.Height - bottomMargin);
                statusLabel.Location = new Point(10, this.Height - 35);
            };

            this.MouseDown += Form1_MouseDown;
            this.MouseMove += Form1_MouseMove;
            this.MouseUp += Form1_MouseUp;
            this.Paint += Form1_Paint;
        }

        private void UpdateStatus(string text)
        {
            statusLabel.Text = text;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && shapeBox.SelectedItem != null)
            {
                startPoint = e.Location;
                isDrawing = true;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                IShapeCreator creator = pluginManager.GetCreator(selectedShape);
                if (creator != null)
                {
                    currentShape = creator.Create(startPoint, e.Location);
                    if (currentShape != null)
                        currentShape.Color = selectedColor;
                }
                Refresh();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.Button == MouseButtons.Left)
            {
                IShapeCreator creator = pluginManager.GetCreator(selectedShape);
                if (creator != null)
                {
                    Shape newShape = creator.Create(startPoint, e.Location);
                    if (newShape != null)
                    {
                        newShape.Color = selectedColor;
                        shapeList.Add(newShape);
                        UpdateStatus("Added " + selectedShape);
                    }
                }
                isDrawing = false;
                currentShape = null;
                Refresh();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            foreach (Shape shape in shapeList.GetAllShapes())
            {
                IShapeDrawer drawer = pluginManager.GetDrawer(shape);
                drawer?.Draw(shape, g);
            }

            if (isDrawing && currentShape != null)
            {
                IShapeDrawer drawer = pluginManager.GetDrawer(currentShape);
                if (drawer != null)
                {
                    Color oldColor = currentShape.Color;
                    currentShape.Color = Color.Gray;
                    drawer.Draw(currentShape, g);
                    currentShape.Color = oldColor;
                }
            }
        }
    }
}