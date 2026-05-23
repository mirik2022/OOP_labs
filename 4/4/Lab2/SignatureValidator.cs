using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Lab2
{
    /// <summary>
    /// Validates digital signatures of plugins
    /// </summary>
    public class SignatureValidator
    {
        // Public key for verifying signatures
        // In production, this would be loaded from a certificate store
        private const string PUBLIC_KEY_XML = @"
            <RSAKeyValue>
                <Modulus>t6X8r9Yv2wN4mQ7kL3pR5sT1uV6xZ9bC2fG4hJ7kL9nM1pQ3rS5tU7vW9xY2zA4bC6dE8fG0hJ2kL4mN6oP8qR0sT2uV4wX6yZ8</Modulus>
                <Exponent>AQAB</Exponent>
            </RSAKeyValue>";

        private RSACryptoServiceProvider rsa;

        public SignatureValidator()
        {
            try
            {
                rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(PUBLIC_KEY_XML);
            }
            catch
            {
                // If key is invalid, generate a new key pair for testing
                GenerateNewKeyPair();
            }
        }

        /// <summary>
        /// Generate a new RSA key pair (for development only)
        /// </summary>
        public void GenerateNewKeyPair()
        {
            using (var tempRsa = new RSACryptoServiceProvider(2048))
            {
                string publicKey = tempRsa.ToXmlString(false);
                string privateKey = tempRsa.ToXmlString(true);

                // Save keys to files for the signing tool to use
                File.WriteAllText("public_key.xml", publicKey);
                File.WriteAllText("private_key.xml", privateKey);

                rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(publicKey);

                Console.WriteLine("New key pair generated. Save private_key.xml for signing plugins.");
            }
        }

        /// <summary>
        /// Verify plugin signature and integrity
        /// </summary>
        /// <param name="dllPath">Path to the plugin DLL</param>
        /// <param name="signatureBase64">The signature as Base64 string</param>
        /// <param name="expirationDate">Optional expiration date</param>
        /// <returns>True if signature is valid and plugin is not expired</returns>
        public bool VerifyPlugin(string dllPath, string signatureBase64, DateTime? expirationDate)
        {
            // Check expiration
            if (expirationDate.HasValue && expirationDate.Value < DateTime.Now)
            {
                throw new Exception($"Plugin has expired on {expirationDate.Value.ToShortDateString()}");
            }

            // Convert signature from Base64
            byte[] signature;
            try
            {
                signature = Convert.FromBase64String(signatureBase64);
            }
            catch
            {
                throw new Exception("Invalid signature format");
            }

            // Compute hash of the DLL
            byte[] dllHash = ComputeFileHash(dllPath);

            // Verify signature
            bool isValid = rsa.VerifyData(dllHash, SHA256.Create(), signature);

            if (!isValid)
            {
                throw new Exception("Plugin signature is invalid. File may have been tampered with.");
            }

            return true;
        }

        /// <summary>
        /// Compute SHA256 hash of a file
        /// </summary>
        private byte[] ComputeFileHash(string filePath)
        {
            using (SHA256 sha256 = SHA256.Create())
            using (FileStream stream = File.OpenRead(filePath))
            {
                return sha256.ComputeHash(stream);
            }
        }

        /// <summary>
        /// Get the public key (for the signing tool)
        /// </summary>
        public string GetPublicKeyXml()
        {
            return PUBLIC_KEY_XML;
        }
    }
}