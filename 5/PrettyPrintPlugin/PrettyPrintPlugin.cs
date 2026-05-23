using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using Lab2;

namespace PrettyPrintPlugin
{
    /// <summary>
    /// Plugin that formats XML with proper indentation
    /// </summary>
    public class PrettyPrintPlugin : IBeforeSavePlugin, IAfterLoadPlugin
    {
        private bool isEnabled = true;

        public string GetName() => "Pretty Print XML";

        public bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }

        /// <summary>
        /// Format XML before saving
        /// </summary>
        public string ProcessBeforeSave(string data)
        {
            if (!isEnabled) return data;
            return FormatXml(data);
        }

        /// <summary>
        /// No reverse transformation needed for formatting
        /// </summary>
        public string ProcessAfterLoad(string data)
        {
            return data;  // Форматирование не требует обратного преобразования
        }

        /// <summary>
        /// Format XML string with indentation
        /// </summary>
        private string FormatXml(string xml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                using (StringWriter writer = new StringWriter())
                using (XmlTextWriter xmlWriter = new XmlTextWriter(writer))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.Indentation = 2;
                    doc.WriteTo(xmlWriter);
                    return writer.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PrettyPrint error: {ex.Message}");
                return xml;
            }
        }

        public void ShowSettings()
        {
            MessageBox.Show(
                "Pretty Print Plugin\n\nFormats XML with proper indentation.\n\nEnabled: " + isEnabled,
                "Plugin Settings",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}