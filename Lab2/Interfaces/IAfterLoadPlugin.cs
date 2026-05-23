namespace Lab2
{
    /// <summary>
    /// Interface for plugins that process data after loading from file
    /// </summary>
    public interface IAfterLoadPlugin
    {
        /// <summary>
        /// Returns the display name of the plugin
        /// </summary>
        string GetName();

        /// <summary>
        /// Processes data after it is loaded from file
        /// </summary>
        /// <param name="data">Raw XML data from file</param>
        /// <returns>Processed XML data (should be valid XML)</returns>
        string ProcessAfterLoad(string data);

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