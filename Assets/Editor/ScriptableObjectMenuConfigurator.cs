using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectMenuConfigurator
{
    [MenuItem("Assets/Create/Create Random Team")]
    public static void CreateRandomTeam()
    {
        Team newTeam = ScriptableObject.CreateInstance<Team>();
        newTeam.players = LoadRandomName(11);

        string folderPath = "Assets/Game/Teams";
        string assetName = "RandomTeam";
        string fullPath = $"{folderPath}/{assetName}.asset";

        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }

        string uniquePath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

        AssetDatabase.CreateAsset(newTeam, uniquePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newTeam;
    }

    public static List<string> LoadRandomName(int amount)
    {
        string namesFilePath = Path.Combine(Application.dataPath, "Game", "NameGenerator", "names.txt");
        var names = File.ReadAllLines(namesFilePath).ToList();

        string lastNamesfilePath = Path.Combine(Application.dataPath, "Game", "NameGenerator", "lastnames.txt");
        var lastNames = File.ReadAllLines(lastNamesfilePath).ToList();

        List<string> fullNames = new();
        for(int i = 0; i < amount; i++)
        {
            if(names.Count > 0 && lastNames.Count > 0)
            {
                int nameRandomIndex = Random.Range(0, names.Count);
                int lastNameRandomIndex = Random.Range(0, lastNames.Count);

                var fullName = $"{names[nameRandomIndex]} {lastNames[lastNameRandomIndex]}";
                names.RemoveAt(nameRandomIndex);

                fullNames.Add(fullName);
            }
        }

        return fullNames;
    }
}