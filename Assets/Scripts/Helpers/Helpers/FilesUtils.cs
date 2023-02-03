using System;
using System.IO;

public static class FilesUtils
{
    public static string GetRelativePath(string filePath, string folder)
    {
        Uri pathUri = new Uri(filePath);
        // Folders must end in a slash
        if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            folder += Path.DirectorySeparatorChar;
        }
        Uri folderUri = new Uri(folder);
        return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
    }
}
