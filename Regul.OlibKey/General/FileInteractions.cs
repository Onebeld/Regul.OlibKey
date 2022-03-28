using System;
using System.IO;

namespace Regul.OlibKey.General;

public static class FileInteractions
{
    public static string ImportFile(string path) => 
        Compressing.Compress(File.ReadAllBytes(path));

    public static void ExportFile(string file, string path) => 
        File.WriteAllBytes(path, Convert.FromBase64String(Compressing.Decompress(file)));
}