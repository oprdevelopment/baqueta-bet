using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectMenuConfigurator
{
    [MenuItem("Assets/Nations/Generate Nations from File")]
    public static void GenerateAllNations()
    {
        string countryFilePath = Path.Combine(Application.dataPath, "Game", "TeamInfo", "Countries.txt");
        var countryNames = File.ReadAllLines(countryFilePath).ToList();

        string folderPath = "Assets/Game/TeamInfo/AllTeams";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        foreach(var country in countryNames)
        {
            string fullPath = $"{folderPath}/{country}.asset";  
            if(File.Exists(fullPath)) return;

            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

            TeamInfo newTeam = ScriptableObject.CreateInstance<TeamInfo>();
            newTeam.name = country;

            AssetDatabase.CreateAsset(newTeam, uniquePath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    [MenuItem("Assets/Nations/Randomize Strenght")]
    public static void RandomizeNationsStrenght()
    {
        string folderPath = "Assets/Game/TeamInfo/AllTeams";
        string filtro = "t:TeamInfo";

        string[] guids = AssetDatabase.FindAssets(filtro, new[] {folderPath});

        foreach(var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            TeamInfo team = AssetDatabase.LoadAssetAtPath<TeamInfo>(assetPath);

            team.strenght = UnityEngine.Random.Range(20, 95);
        }
    }
}