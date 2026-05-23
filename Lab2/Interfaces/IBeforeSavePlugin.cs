namespace Lab2
{
    /// <summary>
    /// Interface for plugins that process data before saving to file
    /// </summary>
    public interface IBeforeSavePlugin
    {
        /// <summary>
        /// Returns the display name of the plugin
        /// </summary>
        string GetName();

        /// <summary>
        /// Processes data before it is saved to file
        /// </summary>
        /// <param name="data">Original XML data</param>
        /// <returns>Processed XML data</returns>
        string ProcessBeforeSave(string data);

        /// <summary>
        /// Whether the plugin is currently enabled
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Shows settings dialog for this plugin
        /// </summary>
        void ShowSettings();
    }
}