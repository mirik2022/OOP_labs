using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab2
{
    public partial class Form1 : Form
    {
        // Core components
        private ShapeList shapeList;        // Stores all drawn shapes
        private ShapeFactory factory;        // Creates new shapes
        private Shape currentShape;          // Shape being drawn (preview)
        private Point startPoint;             // Starting point for drawing
        private bool isDrawing = false;       // Drawing state flag
        private string selectedShape;         // Currently selected shape type
        private Color selectedColor = Color.Black; // Selected color
        private PluginManager pluginManager;
        private Button statusButton;
        private Label pluginStatusLabel;

        // UI Controls
        private ComboBox shapeBox;
        private ComboBox colorBox;            // Color selection dropdown
        private Button clearButton;
        private Label statusLabel;

        public Form1()
        {
            InitializeComponent();

            // Form settings
            this.Text = "Simple Graphic Editor - Lab 2";
            this.Size = new Size(900, 600);
            this.DoubleBuffered = true;
            this.BackColor = Color.White;

            // Initialize components
            shapeList = new ShapeList();

            // NEW: Use PluginManager instead of ShapeFactory
            pluginManager = new PluginManager();
            pluginManager.LoadPlugins();  // Loads both built-in shapes and plugins

            // OLD factory - keep for backward compatibility or remove
            // factory = new ShapeFactory();

            // Setup UI and event handlers
            CreateControls();
            SetupEventHandlers();
        }

        /// <summary>
        /// Create and position all UI controls
        /// </summary>
        private void CreateControls()
        {
            // Shape selection label
            Label shapeLabel = new Label();
            shapeLabel.Text = "Shape:";
            shapeLabel.Location = new Point(10, 10);
            shapeLabel.Size = new Size(50, 25);
            this.Controls.Add(shapeLabel);

            // Shape dropdown - populated from PluginManager
            shapeBox = new ComboBox();
            shapeBox.Location = new Point(70, 10);
            shapeBox.Size = new Size(120, 25);
            shapeBox.DropDownStyle = ComboBoxStyle.DropDownList;

            // NEW: Get shapes from PluginManager instead of factory
            string[] shapeNames = pluginManager.GetAllShapeNames();
            foreach (string name in shapeNames)
            {
                shapeBox.Items.Add(name);
            }

            if (shapeBox.Items.Count > 0)
                shapeBox.SelectedIndex = 0;

            this.Controls.Add(shapeBox);

            // Color selection label
            Label colorLabel = new Label();
            colorLabel.Text = "Color:";
            colorLabel.Location = new Point(200, 10);
            colorLabel.Size = new Size(40, 25);
            this.Controls.Add(colorLabel);

            // Color dropdown (same as before)
            colorBox = new ComboBox();
            colorBox.Location = new Point(240, 10);
            colorBox.Size = new Size(100, 25);
            colorBox.DropDownStyle = ComboBoxStyle.DropDownList;

            colorBox.Items.Add("Black");
            colorBox.Items.Add("Red");
            colorBox.Items.Add("Blue");
            colorBox.Items.Add("Green");
            colorBox.Items.Add("Yellow");
            colorBox.Items.Add("Orange");
            colorBox.Items.Add("Purple");
            colorBox.Items.Add("Brown");
            colorBox.Items.Add("Gray");

            colorBox.SelectedIndex = 0;

            colorBox.SelectedIndexChanged += (s, e) =>
            {
                switch (colorBox.SelectedItem.ToString())
                {
                    case "Black": selectedColor = Color.Black; break;
                    case "Red": selectedColor = Color.Red; break;
                    case "Blue": selectedColor = Color.Blue; break;
                    case "Green": selectedColor = Color.Green; break;
                    case "Yellow": selectedColor = Color.Yellow; break;
                    case "Orange": selectedColor = Color.Orange; break;
                    case "Purple": selectedColor = Color.Purple; break;
                    case "Brown": selectedColor = Color.Brown; break;
                    case "Gray": selectedColor = Color.Gray; break;
                }
            };

            this.Controls.Add(colorBox);

            // Clear canvas button
            clearButton = new Button();
            clearButton.Text = "Clear";
            clearButton.Location = new Point(350, 10);
            clearButton.Size = new Size(80, 25);
            this.Controls.Add(clearButton);

            // NEW: Plugin status button
            statusButton = new Button();
            statusButton.Text = "Plugin Status";
            statusButton.Location = new Point(440, 10);
            statusButton.Size = new Size(100, 25);
            statusButton.Click += (s, e) =>
            {
                MessageBox.Show(pluginManager.GetStatusReport(), "Plugin Status",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            this.Controls.Add(statusButton);

            // NEW: Plugin status label
            pluginStatusLabel = new Label();
            pluginStatusLabel.Text = $"Loaded {pluginManager.GetAllShapeNames().Length} shapes";
            pluginStatusLabel.Location = new Point(550, 10);
            pluginStatusLabel.Size = new Size(300, 25);
            pluginStatusLabel.ForeColor = Color.Green;
            this.Controls.Add(pluginStatusLabel);

            // Status bar at bottom
            statusLabel = new Label();
            statusLabel.Text = "Select a shape and draw with mouse";
            statusLabel.Location = new Point(10, this.Height - 40);
            statusLabel.Size = new Size(500, 20);
            statusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(statusLabel);
        }

        /// <summary>
        /// Setup all event handlers for UI and mouse
        /// </summary>
        private void SetupEventHandlers()
        {
            // Shape selection changed
            shapeBox.SelectedIndexChanged += (s, e) =>
            {
                selectedShape = shapeBox.SelectedItem.ToString();
                UpdateStatus("Selected: " + selectedShape);
            };

            // Clear button - remove all shapes
            clearButton.Click += (s, e) =>
            {
                shapeList.Clear();
                Refresh();
                UpdateStatus("Canvas cleared");
            };

            // Mouse events for drawing
            this.MouseDown += Form1_MouseDown;
            this.MouseMove += Form1_MouseMove;
            this.MouseUp += Form1_MouseUp;
            this.Paint += Form1_Paint;
        }

        private void UpdateStatus(string text)
        {
            statusLabel.Text = text;
        }

        /// <summary>
        /// Mouse down - start drawing a new shape
        /// </summary>
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && shapeBox.SelectedItem != null)
            {
                startPoint = e.Location;
                isDrawing = true;
            }
        }

        /// <summary>
        /// Mouse move - update shape preview while dragging
        /// </summary>
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                // Use PluginManager to create shape
                IShapeCreator creator = pluginManager.GetCreator(selectedShape);
                if (creator != null)
                {
                    currentShape = creator.Create(startPoint, e.Location);
                    if (currentShape != null)
                    {
                        currentShape.Color = selectedColor;
                    }
                }
                Refresh();
            }
        }

        /// <summary>
        /// Mouse up - finalize and add the shape to the list
        /// </summary>
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.Button == MouseButtons.Left)
            {
                // NEW: Use PluginManager to create final shape
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

        /// <summary>
        /// Paint event - draw all shapes and preview if drawing
        /// </summary>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw all saved shapes using PluginManager
            foreach (Shape shape in shapeList.GetAllShapes())
            {
                IShapeDrawer drawer = pluginManager.GetDrawer(shape);
                if (drawer != null)
                {
                    drawer.Draw(shape, g);
                }
            }

            // Draw preview shape while dragging
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