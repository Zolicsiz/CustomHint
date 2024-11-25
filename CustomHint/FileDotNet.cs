using Exiled.API.Features;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CustomHintPlugin
{
    public class FileDotNet
    {
        public static string GetPath(string fileName)
        {
            return Path.Combine(Paths.Configs, "CustomHint", fileName);
        }

        public static object LoadFile<F>(string fileName, object objectDefault)
        {
            if (File.Exists(fileName))
            {
                IDeserializer deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                return deserializer.Deserialize<F>(File.ReadAllText(fileName));
            }
            else
            {
                SaveFile(fileName, objectDefault);
                return objectDefault;
            }
        }

        public static void SaveFile(string fileName, object text)
        {
            string directory = Path.GetDirectoryName(fileName);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            ISerializer serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            File.WriteAllText(fileName, serializer.Serialize(text));
        }
    }
}
