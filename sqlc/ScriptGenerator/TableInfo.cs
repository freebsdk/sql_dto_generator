using System.Data;

namespace sqlc;




public class ColumnInfo
{
    public string Name = "";
    public string Type = "";
    public int? Length;
    public bool NotNull;
    public string? DefaultValue;
    public string Desc = "";
}




public class TableInfo
{
    public string Database = "";
    public string Name = "";
    public List<ColumnInfo> Columns = new();
    public List<string> PrimaryKey = new();
    public List<List<string>> UniqueKeys = new();
    public List<List<string>> MultiKeys = new();
    public string Desc = "";



    // return : 단일 필드 PK일경우 0 리턴, 복합키인경우 몇번째에 위치하는지 index 리턴
    // 해당 필드가 PK를 구성하는 필드가 아닌경우 -1
    public (bool isPrimaryKey, int posIndex) IsPrimaryKey(string fieldName)
    {
        for (var i = 0; i < PrimaryKey.Count; i++)
        {
            if (PrimaryKey[i] == fieldName) return (true, i);
        }

        return (false, -1);
    }




    public string UpperTableName() => Name.ToUpper();
}