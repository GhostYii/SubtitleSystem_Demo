using System.IO;
using YamlDotNet.Serialization;

namespace Utility
{
    public class YamlHelper
    {
        /// <summary>
        /// 序列化
        /// </summary>
        public static string ToYaml<T>(T obj)
        {
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(obj);

            return yaml;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        public static T FromYaml<T>(string yaml)
        {
            var deserializer = new Deserializer();
            T obj = deserializer.Deserialize<T>(yaml);

            return obj;
        }

        /// <summary>
        /// 从文件反序列化
        /// </summary>
        public static T FromYamlFile<T>(string filename)
        {
            string yaml = File.ReadAllText(filename);

            return FromYaml<T>(yaml);
        }

        /// <summary>
        /// 序列化至文件
        /// </summary>
        public static void ToYamlFile<T>(T obj, string path)
        {
            File.WriteAllText(path, ToYaml(obj));
        }

    }
}