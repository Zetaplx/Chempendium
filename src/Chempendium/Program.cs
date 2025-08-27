using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using YamlDotNet.RepresentationModel;


public class Program
{
    public static void Main(string[] args)
    {
        string rootPath = Path.GetFullPath(Directory.GetCurrentDirectory()) + "/data";
        var files = Directory.EnumerateFiles(rootPath, "*.yml", SearchOption.AllDirectories)
                        .Concat(Directory.EnumerateFiles(rootPath, "*.yaml", SearchOption.AllDirectories))
                        .Distinct();


        var yaml = new YamlStream();
        foreach (var filePath in files)
        {
            if (!File.Exists(filePath)) continue;

            yaml.Load(new StreamReader(filePath));

            foreach (var doc in yaml.Documents)
            {
                if (doc.RootNode is not YamlSequenceNode node) continue;

                foreach (var item in node)
                {
                    if (item is not YamlMappingNode components) continue;
                    if(TryGetNode<YamlScalarNode>(components, "id", out var id)) {
                        Console.WriteLine($"{id}");
                    }
                }
            }
        }
    }
    
    static bool TryGetNode<T>(YamlMappingNode root, string key, out T? value) where T : YamlNode
    {
        foreach (var keyValuePair in root.Children)
        {
            if (keyValuePair.Key is YamlScalarNode keyNode && string.Equals(keyNode?.Value ?? "invalid", key, StringComparison.Ordinal))
            {
                if (keyValuePair.Value is not T t) continue;
                value = t;
                return true;
            }
        }

        value = default!;
        return false;
    }
}

public class ReagentData {
    public string ID { get; }
    public string Name { get; }
    public string ColorHex { get; }
    public string Description { get; }
}