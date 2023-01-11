using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team
{
    int number;
    int score;
    List<Player> players = new List<Player>();
    bool isHuman;
    Player playerClosestToBall;
    //public bool[,] Graph;
    bool[,] graph;
    public bool[,] Graph { get => graph; set => graph = value; }

    public Team(int number, bool isHuman)
    {
        this.number = number;
        this.isHuman = isHuman;
        //this.graph = new bool[75 / 3, 39 / 3];
        this.graph = new bool[75, 39];
    }

    public int Number { get => number; set => number = value; }
    public bool IsHuman { get => isHuman; set => isHuman = value; }
    public List<Player> Players { get => players; set => players = value; }
    public Player PlayerClosestToBall { get => playerClosestToBall; set => playerClosestToBall = value; }
    public int Score { get => score; set => score = value; }
}
