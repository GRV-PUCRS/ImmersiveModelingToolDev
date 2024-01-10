using System.IO;

public class ObjectFile
{
    private string name;
    private string extension;

    public ObjectFile(string file)
    {
        string[] tokens = Path.GetFileName(file).Split('.');

        name = tokens[0];
        extension = tokens[1];
    }

    public string Name { get => name; }
    public string Extension { get => extension; }
    public string NameWithExtension { get => $"{name}.{extension}"; }
    public ExtensionAllowed.ExtensionValue ExtensionValue { get => ExtensionAllowed.GetExtensionValue(extension); }
}