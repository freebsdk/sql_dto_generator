using System.Text;

namespace sqlc;

public abstract class IScriptGenerator
{
    protected readonly Dictionary<IdlFieldType, string /* native type */> _nativeTypes = new();
    protected readonly StringBuilder _genCode = new();
    
    
    
    public abstract void Open();
    public abstract void Close();
    public abstract void GenerateTable(TableInfo tableInfo);




    public string GetGeneratedCode()
    {
        return _genCode.ToString();
    }
}