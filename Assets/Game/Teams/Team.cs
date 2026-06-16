using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Team : ScriptableObject
{
    public List<Player> players = new();
    public void AddPlayers(List<string> players) => players.ForEach((name) => this.players.Add(new Player(name, this)));
}