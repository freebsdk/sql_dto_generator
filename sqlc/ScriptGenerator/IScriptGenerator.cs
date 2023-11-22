namespace sqlc;

public abstract class IScriptGenerator
{
    public abstract void Open();
    public abstract void Close();
    public abstract void GenerateTable(TableInfo tableInfo);
    public abstract string GetGeneratedCode();
}