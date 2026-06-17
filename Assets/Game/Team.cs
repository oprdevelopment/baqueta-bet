using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Team : ScriptableObject
{
    public List<Player> players = new();
    public void AddPlayers(List<string> players) => players.ForEach((name) => this.players.Add(new Player(name, this)));
    public static List<string> names, lastNames = new();
    public static Team GenerateRandomTeam()
    {
        Team newTeam = CreateInstance<Team>();
        newTeam.AddPlayers(LoadRandomName(11));

        return newTeam;
    }

    public static List<string> LoadRandomName(int amount)
    {
        if(names == null || names.Count == 0)
        {
            string namesFilePath = Path.Combine(Application.streamingAssetsPath, "NameGenerator", "names.txt");
            names = File.ReadAllLines(namesFilePath).ToList();
        }

        if(lastNames == null || lastNames.Count == 0)
        {   
            string lastNamesfilePath = Path.Combine(Application.streamingAssetsPath, "NameGenerator", "lastnames.txt");
            lastNames = File.ReadAllLines(lastNamesfilePath).ToList();
        }

        List<string> fullNames = new();
        for(int i = 0; i < amount; i++)
        {
            if(names.Count > 0 && lastNames.Count > 0)
            {
                int nameRandomIndex = UnityEngine.Random.Range(0, names.Count);
                int lastNameRandomIndex = UnityEngine.Random.Range(0, lastNames.Count);

                var fullName = $"{names[nameRandomIndex]} {lastNames[lastNameRandomIndex]}";
                names.RemoveAt(nameRandomIndex);

                fullNames.Add(fullName);
            }
        }
        return fullNames;
    }
}