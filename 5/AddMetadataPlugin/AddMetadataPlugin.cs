using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using Lab2;

namespace AddMetadataPlugin
{
    /// <summary>
    /// Plugin that adds timestamp and shape count to XML
    /// </summary>
    public class AddMetadataPlugin : IBeforeSavePlugin, IAfterLoadPlugin
    {
        private bool isEnabled = true;
        private bool includeTimestamp = true;
        private bool includeShapeCount = true;

        public string GetName() => "Add Metadata";

        public bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }

        /// <summary>
        /// Add metadata before saving
        /// </summary>
        public string ProcessBeforeSave(string data)
        {
            if (!isEnabled) return data;
            return AddMetadata(data);
        }

        /// <summary>
        /// Remove metadata after loading
        /// </summary>
        public string ProcessAfterLoad(string data)
        {
            if (!isEnabled) return data;
            return RemoveMetadata(data);
        }

        private string AddMetadata(string xml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                // Создаём элемент Metadata
                XmlElement metadata = doc.CreateElement("Metadata");

                // Добавляем дату и время
                if (includeTimestamp)
                {
                    XmlElement timestamp = doc.CreateElement("Timestamp");
                    timestamp.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    metadata.AppendChild(timestamp);
                }

                // Добавляем количество фигур
                if (includeShapeCount)
                {
                    XmlElement shapeCount = doc.CreateElement("ShapeCount");
                    int count = doc.GetElementsByTagName("Shape").Count;
                    shapeCount.InnerText = count.ToString();
                    metadata.AppendChild(shapeCount);
                }

                // Добавляем метаданные в корневой элемент
                if (doc.DocumentElement != null && metadata.HasChildNodes)
                {
                    doc.DocumentElement.AppendChild(metadata);
                }

                return doc.OuterXml;
            }
            catch
            {
                return xml;
            }
        }

        private string RemoveMetadata(string xml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                // Удаляем элемент Metadata
                XmlNode metadata = doc.SelectSingleNode("//Metadata");
                if (metadata != null && metadata.ParentNode != null)
                {
                    metadata.ParentNode.RemoveChild(metadata);
                }

                return doc.OuterXml;
            }
            catch
            {
                return xml;
            }
        }

        public void ShowSettings()
        {
            // Создаём диалог настроек
            Form dialog = new Form();
            dialog.Text = "Metadata Plugin Settings";
            dialog.Size = new System.Drawing.Size(350, 180);
            dialog.FormBorderStyle = FormBorderStyle.FixedDialog;

            CheckBox chkTimestamp = new CheckBox();
            chkTimestamp.Text = "Include timestamp";
            chkTimestamp.Location = new System.Drawing.Point(20, 20);
            chkTimestamp.Checked = includeTimestamp;

            CheckBox chkShapeCount = new CheckBox();
            chkShapeCount.Text = "Include shape count";
            chkShapeCount.Location = new System.Drawing.Point(20, 50);
            chkShapeCount.Checked = includeShapeCount;

            CheckBox chkEnabled = new CheckBox();
            chkEnabled.Text = "Enable plugin";
            chkEnabled.Location = new System.Drawing.Point(20, 80);
            chkEnabled.Checked = isEnabled;

            Button btnOk = new Button();
            btnOk.Text = "OK";
            btnOk.Location = new System.Drawing.Point(150, 120);
            btnOk.Click += (s, e) =>
            {
                includeTimestamp = chkTimestamp.Checked;
                includeShapeCount = chkShapeCount.Checked;
                isEnabled = chkEnabled.Checked;
                dialog.Close();
            };

            dialog.Controls.Add(chkTimestamp);
            dialog.Controls.Add(chkShapeCount);
            dialog.Controls.Add(chkEnabled);
            dialog.Controls.Add(btnOk);

            dialog.ShowDialog();
        }
    }
}