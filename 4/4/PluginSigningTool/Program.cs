using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PluginSigningTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Plugin Signing Tool ===");
            Console.WriteLine();

            if (args.Length < 1)
            {
                Console.WriteLine("Usage: PluginSigningTool <plugin.dll> [expiration_days]");
                Console.WriteLine("Example: PluginSigningTool StarPlugin.dll 365");
                Console.WriteLine();
                Console.WriteLine("If no private key exists, a new key pair will be generated.");
                return;
            }

            string dllPath = args[0];
            int expirationDays = args.Length > 1 ? int.Parse(args[1]) : 365;

            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"Error: File not found - {dllPath}");
                return;
            }

            try
            {
                // Load or generate private key
                RSACryptoServiceProvider rsa = LoadOrGeneratePrivateKey();

                // Compute hash of the DLL
                byte[] dllHash = ComputeFileHash(dllPath);

                // Sign the hash
                byte[] signature = rsa.SignData(dllHash, SHA256.Create());

                // Convert signature to Base64
                string signatureBase64 = Convert.ToBase64String(signature);

                // Save signature to .sig file
                string sigPath = dllPath + ".sig";
                File.WriteAllText(sigPath, signatureBase64);

                // Create metadata
                DateTime expirationDate = DateTime.Now.AddDays(expirationDays);
                string metaPath = dllPath + ".meta";
                string metadata = $@"Plugin Metadata
================================
Plugin: {Path.GetFileName(dllPath)}
Signature: {signatureBase64.Substring(0, Math.Min(50, signatureBase64.Length))}...
Expiration Date: {expirationDate:yyyy-MM-dd HH:mm:ss}
Signed On: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
Days Valid: {expirationDays}
";

                File.WriteAllText(metaPath, metadata);

                Console.WriteLine($"✅ Plugin signed successfully!");
                Console.WriteLine($"   File: {Path.GetFileName(dllPath)}");
                Console.WriteLine($"   Signature: {sigPath}");
                Console.WriteLine($"   Expires: {expirationDate:yyyy-MM-dd}");
                Console.WriteLine($"   Metadata: {metaPath}");
                Console.WriteLine();
                Console.WriteLine("IMPORTANT: Copy public_key.xml to your Lab2 project!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static RSACryptoServiceProvider LoadOrGeneratePrivateKey()
        {
            string privateKeyPath = "private_key.xml";
            string publicKeyPath = "public_key.xml";

            if (File.Exists(privateKeyPath))
            {
                Console.WriteLine("Loading existing private key...");
                var rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(File.ReadAllText(privateKeyPath));
                return rsa;
            }

            Console.WriteLine("No private key found. Generating new key pair...");
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                string publicKey = rsa.ToXmlString(false);
                string privateKey = rsa.ToXmlString(true);

                File.WriteAllText(publicKeyPath, publicKey);
                File.WriteAllText(privateKeyPath, privateKey);

                Console.WriteLine($"   Generated: {publicKeyPath}");
                Console.WriteLine($"   Generated: {privateKeyPath}");
                Console.WriteLine("   ⚠️  Keep private_key.xml secure!");
                Console.WriteLine("   📋 Copy public_key.xml to your Lab2 project!");

                var result = new RSACryptoServiceProvider();
                result.FromXmlString(privateKey);
                return result;
            }
        }

        private static byte[] ComputeFileHash(string filePath)
        {
            using (SHA256 sha256 = SHA256.Create())
            using (FileStream stream = File.OpenRead(filePath))
            {
                return sha256.ComputeHash(stream);
            }
        }
    }
}