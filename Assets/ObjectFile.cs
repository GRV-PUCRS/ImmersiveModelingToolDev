using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ObjectFile
{
    private string name;
    private string extension;

    public ObjectFile(string file)
    {
        string[] tokens = file.Split('.');

        name = tokens[0];
        extension = tokens[1];
    }

    public string Name { get => name; }
    public string Extension { get => extension; }
    public string FileName { get => name + '.' + extension; }
    public ExtensionAllowed.ExtensionValue ExtensionValue { get => ExtensionAllowed.GetExtensionValue(extension); }
}