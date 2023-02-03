using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[JsonObject]
public class AssemblyDefinitionAsset
{
    [JsonProperty] public string name;
    [JsonProperty] public string rootNamespace;
    [JsonProperty] public string[] references;
    [JsonProperty] public string[] includePlatforms;
    [JsonProperty] public string[] excludePlatforms;
    [JsonProperty] public bool allowUnsafeCode;
    [JsonProperty] public bool overrideReferences;
    [JsonProperty] public string[] precompiledReferences;
    [JsonProperty] public bool autoReferenced;
    [JsonProperty] public string[] defineConstraints;
    [JsonProperty] public VersionDefine[] versionDefines;
    [JsonProperty] public bool noEngineReferences;

    public static void UpdateAsset(string assemblyDefinitionFilePath, AssemblyDefinitionAsset assemblyDefinitionAsset)
    {
        string jsonString = JToken.FromObject(assemblyDefinitionAsset).ToString();
        File.WriteAllText(assemblyDefinitionFilePath, jsonString);
    }

    public static AssemblyDefinitionAsset Get(string assemblyDefinitionFilePath)
    {
        if(File.Exists(assemblyDefinitionFilePath) == false)
        {
            throw new System.Exception($"File {assemblyDefinitionFilePath} not found");
        }
        if (assemblyDefinitionFilePath.EndsWith(".asmdef") == false)
        {
            throw new System.Exception($"File {assemblyDefinitionFilePath} is not an Assembly Definition file");
        }
        string fileContents = File.ReadAllText(assemblyDefinitionFilePath);
        JToken jToken = JToken.Parse(fileContents);
        AssemblyDefinitionAsset assemblyDefinitionAsset = jToken.ToObject<AssemblyDefinitionAsset>();
        return assemblyDefinitionAsset;
    }

    public class VersionDefine
    {
        [JsonProperty] public string name;
        [JsonProperty] public string expression;
        [JsonProperty] public string define;
    }
}
