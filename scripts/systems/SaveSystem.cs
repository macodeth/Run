using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
public partial class SaveSystem
{
    private const string SAVE_PATH = "res://data/userdata.json";
    // 0 is tutorial
    public static void SaveProgress (int level)
    {
        var saveFile = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.ReadWrite);
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(saveFile.GetAsText());        
        data["level"] = level;
        var res = JsonSerializer.Serialize(data);
        saveFile.StoreString(res);
    }
    public static int GetLevel () 
    {
        var saveFile = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Read);
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(saveFile.GetAsText());        
        return Int32.Parse(data["level"].ToString());
    }
}