using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Regul.OlibKey.Structures;

namespace Regul.OlibKey.General
{
    public static class DatabaseExtensions
    {
        public static void Save(this Database database, string path, string masterPassword)
        {
            string file = database.Settings.Iterations + ":" + database.Settings.NumberOfEncryptionProcedures + ":";

            string databaseXml = database.ToXml(true);
            
            string encryptString = Encryptor.EncryptString(
                database.Settings.UseCompress ? Compressing.Compress(databaseXml) : databaseXml, masterPassword,
                database.Settings.NumberOfEncryptionProcedures, database.Settings.Iterations);

            file += encryptString + ":" + database.Settings.UseCompress + ":" + database.Settings.UseTrash;

            File.WriteAllText(path, file);
        }

        public static string ToXml(this Database database, bool removeEmptyAndDefaultNodes)
        {
            using (StringWriter writer = new StringWriter())
            {
                new XmlSerializer(typeof(Database)).Serialize(writer, database);
                XElement doc = XElement.Parse(writer.ToString());

                if (removeEmptyAndDefaultNodes)
                {
                    foreach (XElement element in doc.Descendants().Reverse())
                    {
                        if (!element.HasElements && string.IsNullOrEmpty(element.Value) && !element.HasAttributes)
                        {
                            element.Remove();
                            continue;
                        }

                        if (element.Value == "0" || element.Value == "false")
                        {
                            element.Remove();
                            continue;
                        }

                        foreach (XAttribute attribute in element.Attributes())
                        {
                            if (attribute.Value == "false")
                                attribute.Remove();
                        }
                    }
                }

                return doc.ToString();
            }
        }
    }
}