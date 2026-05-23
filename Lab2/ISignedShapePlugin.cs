using System;

namespace Lab2
{
    /// <summary>
    /// Extended plugin interface with digital signature support
    /// </summary>
    public interface ISignedShapePlugin : IShapePlugin
    {
        /// <summary>
        /// Get the digital signature of the plugin (as Base64 string)
        /// </summary>
        string GetSignatureBase64();

        /// <summary>
        /// Get expiration date of the plugin (null = never expires)
        /// </summary>
        DateTime? GetExpirationDate();

        /// <summary>
        /// Get plugin author information
        /// </summary>
        string GetAuthor();

        /// <summary>
        /// Get plugin version
        /// </summary>
        string GetVersion();
    }
}