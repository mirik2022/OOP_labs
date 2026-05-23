using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using Lab2;

namespace AddMetadataPlugin
{

    public class AddMetadataPlugin : IBeforeSavePlugin, IAfterLoadPlugin
    {
        private bool isEnabled = true;
        private bool includeTimestamp = true;

        public string GetName()
        {
            return "Add Metadata";
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        public string ProcessBeforeSave(string data)
        {
            if (!isEnabled) return data;
            return AddMetadata(data);
        }

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

                // Check if metadata already exists
                XmlNode existingMeta = doc.SelectSingleNode("//Metadata");
                if (existingMeta != null) return xml;

                // Create metadata element
                XmlElement metadata = doc.CreateElement("Metadata");

                if (includeTimestamp)
                {
                    XmlElement timestamp = doc.CreateElement("Timestamp");
                    timestamp.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    metadata.AppendChild(timestamp);
                }

                // Add metadata after root
                XmlNode root = doc.DocumentElement;
                if (root != null && metadata.HasChildNodes)
                {
                    root.InsertAfter(metadata, root.FirstChild);
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
            Form dialog = new Form();
            dialog.Text = "Metadata Plugin Settings";
            dialog.Size = new System.Drawing.Size(350, 150);
            dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            dialog.StartPosition = FormStartPosition.CenterParent;

            CheckBox chkTimestamp = new CheckBox();
            chkTimestamp.Text = "Include timestamp";
            chkTimestamp.Location = new System.Drawing.Point(20, 20);
            chkTimestamp.Size = new System.Drawing.Size(250, 25);
            chkTimestamp.Checked = includeTimestamp;

            CheckBox chkEnabled = new CheckBox();
            chkEnabled.Text = "Enable plugin";
            chkEnabled.Location = new System.Drawing.Point(20, 50);
            chkEnabled.Size = new System.Drawing.Size(250, 25);
            chkEnabled.Checked = isEnabled;

            Button btnOk = new Button();
            btnOk.Text = "OK";
            btnOk.Location = new System.Drawing.Point(150, 90);
            btnOk.Size = new System.Drawing.Size(80, 30);
            btnOk.Click += (s, e) =>
            {
                includeTimestamp = chkTimestamp.Checked;
                isEnabled = chkEnabled.Checked;
                dialog.DialogResult = DialogResult.OK;
                dialog.Close();
            };

            dialog.Controls.Add(chkTimestamp);
            dialog.Controls.Add(chkEnabled);
            dialog.Controls.Add(btnOk);

            dialog.ShowDialog();
        }
    }
}