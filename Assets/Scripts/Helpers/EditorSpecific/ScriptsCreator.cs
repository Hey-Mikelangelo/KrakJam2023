using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities
{
    [Serializable]
    public static class ScriptsCreator 
    {
        private static StringBuilder sb = new StringBuilder();
      
        public static string CreateScript(string folderPath, string scriptName, string scriptContent)
        {
            var assetsPath = Application.dataPath;
            var directoryAbsolutePath = sb.Clear().Append(assetsPath).Append('/').Append(folderPath).ToString();
            var fullScriptName = scriptName + ".cs";
            var scriptFullPath = sb.Clear().Append(directoryAbsolutePath).Append('/').Append(fullScriptName).ToString();

            if(Directory.Exists(directoryAbsolutePath) == false)
            {
                Directory.CreateDirectory(directoryAbsolutePath);
            }

            using (StreamWriter sw = new StreamWriter(scriptFullPath))
            {
                sw.Write(scriptContent);
            }
            return scriptFullPath;
        }

    }

}

