using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Regul.OlibKey.General;

public static class Compressing
{
    public static string Compress(string text) => Compress(Encoding.UTF8.GetBytes(text));

    public static string Compress(byte[] buffer)
    {
        MemoryStream memoryStream = new();
        using (GZipStream gZipStream = new(memoryStream, CompressionMode.Compress, true))
            gZipStream.Write(buffer, 0, buffer.Length);

        memoryStream.Position = 0;

        byte[] compressedData = new byte[memoryStream.Length];
        _ = memoryStream.Read(compressedData, 0, compressedData.Length);

        byte[] gZipBuffer = new byte[compressedData.Length + 4];
        Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);

        return Convert.ToBase64String(gZipBuffer);
    }

    public static string Decompress(string text)
    {
        byte[] gZipBuffer = Convert.FromBase64String(text);
        using MemoryStream memoryStream = new();
        
        int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
        memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

        byte[] buffer = new byte[dataLength];

        memoryStream.Position = 0;
        using (GZipStream gZipStream = new(memoryStream, CompressionMode.Decompress))
            _ = gZipStream.Read(buffer, 0, buffer.Length);

        return Encoding.UTF8.GetString(buffer);
    }
}