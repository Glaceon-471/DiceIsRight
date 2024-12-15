using System.IO;
using System.Reflection;
using System;

namespace DiceIsRight;

public class Harmony_Patch
{
    public static string DirectoryPath;
    public static string LogPath => $"{DirectoryPath}/Log.txt";
    
    public Harmony_Patch()
    {
        DirectoryPath = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
        File.WriteAllText(LogPath, "");
    }

    public static void LogWrite(string text)
    {
        File.AppendAllText(LogPath, $"{text}\n");
    }
}
