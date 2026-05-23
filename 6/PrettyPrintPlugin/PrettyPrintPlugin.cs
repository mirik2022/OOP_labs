using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using Lab2;

namespace PrettyPrintPlugin
{
    // Эти интерфейсы должны ТОЧНО совпадать с теми, что в Lab2

    public class PrettyPrintPlugin : IBeforeSavePlugin, IAfterLoadPlugin
    {
        private bool isEnabled = true;

        public string GetName()
        {
            return "Pretty Print XML";
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        public string ProcessBeforeSave(string data)
        {
            if (!isEnabled) return data;
            return FormatXml(data);
        }

        public string ProcessAfterLoad(string data)
        {
            return data; // No reverse transformation needed
        }

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
                "Pretty Print Plugin\n\nFormats XML with proper indentation.",
                "Plugin Settings",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}