namespace sqlc.Config;




public class ConfigManager
{
    private static object _lock = new();
    private static ConfigManager? _instance;
    private Dictionary<string /* key */,string /* value */> _dic = new();

    
    
    
    public static ConfigManager Instance()
    {
        lock (_lock)
        {
            return _instance ??= new ConfigManager();
        }
    }




    public bool ContainsKey(string key) => _dic.ContainsKey(key);
    public bool ContainsKey(ConfigKeys configKey) => ContainsKey(configKey.ToString());
    

    

    public void Register(string key, string val)
    {
        _dic[key] = val;
    }




    public void Unregister(string key)
    {
        _dic.Remove(key);
    }




    public string? FindValOrNull(string key)
    {
        if (_dic.TryGetValue(key, out var val))
        {
            return val;
        }

        return null;
    }




    public string? FindValOrNull(ConfigKeys configKey) => FindValOrNull(configKey.ToString());
    

    

    public string FindValOrException(string key)
    {
        var val = FindValOrNull(key);
        if (val == null)
        {
            Console.WriteLine($"Does not exist config key ... (Key: {key})");
            throw new AppException(ErrorCodes.ERR_CODE_CFG_KEY_NOT_FOUND);
        }

        return val;
    }




    public string FindValOrException(ConfigKeys key) => FindValOrException(key.ToString());

    
    

    public void ParseArgs(string[] args)
    {
        foreach (var arg in args)
        {
            if (arg.StartsWith("-")) OnParseSingleType(arg);
            else if (arg.StartsWith("--")) OnParseKeyValType(arg);
            else Register(arg, "");
        }
    }




    private void OnParseSingleType(string arg)
    {
        if (arg.Length <= 1)
        {
            Console.WriteLine($"[Error] Invalid argument ... ({arg.Substring(1)})");
            throw new AppException(ErrorCodes.ERR_CODE_INVALID_ARGUMENT);
        }

        Register(arg.Substring(1), "");
    }




    private void OnParseKeyValType(string arg)
    {
        if (arg.Length <= 2)
        {
            Console.WriteLine($"[Error] Invalid argument ... ({arg.Substring(2)})");
            throw new AppException(ErrorCodes.ERR_CODE_INVALID_ARGUMENT);
        }

        var tokens = arg.Substring(2).Split("=");
        if (tokens.Length != 2)
        {
            Console.WriteLine($"[Error] Invalid argument ... ({arg.Substring(2)})");
            throw new AppException(ErrorCodes.ERR_CODE_INVALID_ARGUMENT);
        }

        Register(tokens[0], tokens[1]);        
    }
}