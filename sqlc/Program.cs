using System.Text;
using Newtonsoft.Json;
using sqlc;
using sqlc.Config;

internal class MainProcess
{
    private static string _cfgIdlPath = "";
    private static readonly Dictionary<IScriptGenerator, string /* out_path */> _scriptGenerators = new();



    public static async Task Main(string[] args)
    {
        Console.WriteLine("< SQL idl compiler v1.0 >");
        ConfigManager.Instance().ParseArgs(args);
        CheckRequiredConfigOrExit();

        _cfgIdlPath = ConfigManager.Instance().FindValOrException(ConfigKeys.idl_path);
        var tableInfos = await LoadIdlFileOrExitAsync(_cfgIdlPath);

        await GenerateDtoAsync(tableInfos);
        Console.WriteLine("Complete.");
    }




    private static async Task GenerateDtoAsync(List<TableInfo> tableInfos)
    {
        GenerateTypescriptDtoIfDefined(tableInfos);

        foreach (var kv in _scriptGenerators)
        {
            var scriptGenerator = kv.Key;
            var outPath = kv.Value;
            
            scriptGenerator.Open();
            foreach (var tableInfo in tableInfos) scriptGenerator.GenerateTable(tableInfo);
            scriptGenerator.Close();
            
            var outFullPath = Path.Join(outPath, $"{Path.GetFileNameWithoutExtension(_cfgIdlPath)}.dto.ts");
            await File.WriteAllTextAsync(outFullPath, scriptGenerator.GetGeneratedCode(), Encoding.UTF8);
        }
    }




    private static void GenerateTypescriptDtoIfDefined(List<TableInfo> tableInfos)
    {
        var cfgTsOut = ConfigManager.Instance().FindValOrNull(ConfigKeys.ts_out);
        if (cfgTsOut == null) return;
        
        _scriptGenerators.Add(new GeneratorForTypescript(), cfgTsOut);
    }




    private static async Task<List<TableInfo>> LoadIdlFileOrExitAsync(string idlPath)
    {
        if (File.Exists(idlPath) == false)
        {
            Console.WriteLine($"[Error] Does not exist idl file path ... (Path: {idlPath})");
            Environment.Exit(1);
        }

        var idlText = await File.ReadAllTextAsync(idlPath, Encoding.UTF8);
        try
        {
            var tablesObj = JsonConvert.DeserializeObject<List<TableInfo>>(idlText);
            return tablesObj ?? new List<TableInfo>();
        }
        catch (Exception e)
        {
            Console.WriteLine($"[Error] Json file deserialization fail ... (Path: {idlPath}, Error: {e.Message})");
            Environment.Exit(2);
        }

        // 주의 : 코드 흐름에 따라 여기까지 도달할 수 없음
        throw new AppException(ErrorCodes.ERR_CODE_UNEXPECTED);
    }




    // 필수 환경변수 체크
    // 주의 : 오류 발생시 중단된다.
    private static void CheckRequiredConfigOrExit()
    {
        var requiredCfgKeys = new List<ConfigKeys> { ConfigKeys.idl_path, ConfigKeys.ts_out };

        foreach (var requiredCfgKey in requiredCfgKeys)
        {
            if (ConfigManager.Instance().ContainsKey(requiredCfgKey.ToString())) continue;
            
            Console.WriteLine($"[Error] Please specify config key ... (Required key: '{requiredCfgKey.ToString()}')");
            Environment.Exit(1);
        }
    }
}