using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GridPoint
{
    public int X, Y;

    public GridPoint(int x, int y)
    {
        X = x;
        Y = y;
    }

    // Create grid point from real world position
    public GridPoint(Vector3 vector)
    {
        X = 37 + (int)vector.x;
        Y = 19 + (int)vector.z;
    }
}

public class GamePoint
{
    public Vector3 vector;

    // Create real world position from grid point coordinates
    public GamePoint(int x, int y)
    {
        vector.x = x -37;
        vector.z = y - 19;
        vector.y = Game.PLAYER_Y_POSITION;
    }
    public GamePoint(Vector3 vectorIn)
    {
        vector.x = (int)vectorIn.x;
        vector.z = (int)vectorIn.z;
        vector.y = Game.PLAYER_Y_POSITION;
    }
}

public class AStar : MonoBehaviour
{
    public static AStar Instance;
    Dictionary<GridPoint, bool> closedSet = new Dictionary<GridPoint, bool>();
    Dictionary<GridPoint, bool> openSet = new Dictionary<GridPoint, bool>();

    //cost of start to this key node
    Dictionary<GridPoint, int> gScore = new Dictionary<GridPoint, int>();
    //cost of start to goal, passing through key node
    Dictionary<GridPoint, int> fScore = new Dictionary<GridPoint, int>();

    Dictionary<GridPoint, GridPoint> nodeLinks = new Dictionary<GridPoint, GridPoint>();

    int count;
    int count2;

    public void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        count = 0;
        count2 = 0;

        // Generate initial cost map of each team seperately
        foreach (Team team in Game.Instance.Teams)
        {
            foreach (Player player in team.Players)
            {
                int X = 37 + (int)player.transform.position.x;
                int Y = 19 + (int)player.transform.position.z;

                team.Graph[X, Y] = true;
                for (int value2 = 1; value2 < 3; value2++)
                {
                    team.Graph[X, Y + value2] = true;
                    team.Graph[X, Y - value2] = true;
                }
                for (int value1 = 1; value1 < 3; value1++)
                {
                    team.Graph[X + value1, Y] = true;
                    team.Graph[X - value1, Y] = true;
                    for (int value2 = 1; value2 < 3; value2++)
                    {
                        team.Graph[X + value1, Y + value2] = true;
                        team.Graph[X - value1, Y + value2] = true;
                        team.Graph[X + value1, Y - value2] = true;
                        team.Graph[X - value1, Y - value2] = true;
                    }
                }
            }
        }

        // Print the cost map for debugging
        /*string print1 = "";
        string print2 = "";
        for (int value1 = 0; value1 < Game.Instance.Teams[0].Graph.GetLength(0); value1++)
        {
            for (int value2 = 0; value2 < Game.Instance.Teams[0].Graph.GetLength(1); value2++)
            {
                if (Game.Instance.Teams[0].Graph[value1, value2])
                {
                    print1 += "1,";
                }
                else
                {
                    print1 += "0,";
                }
                if (Game.Instance.Teams[1].Graph[value1, value2])
                {
                    print2 += "1,";
                }
                else
                {
                    print2 += "0,";
                }
            }
            print1 += "\n";
            print2 += "\n";
        }
        Debug.Log(print1);
        Debug.Log(print2);*/



        List<GridPoint> path = FindPath(Game.Instance.Teams[0].Graph, new GridPoint(Game.Instance.Teams[1].Players[0].transform.position), new GridPoint(new Vector3(33.07f, Game.PLAYER_Y_POSITION, 0f)));
        Debug.Log(path.Count);
        string str = "";
        foreach (var x in path)
        {
            str += string.Format("[{0}:{1}], ", x.X, x.Y);
        }
        Debug.Log(string.Format("Path: {0}", str));
    }

    void Update()
    {
        // Update the costmap of each team seperately
        foreach (Team team in Game.Instance.Teams)
        {
            ((System.Collections.IList)team.Graph).Clear();
            foreach (Player player in team.Players)
            {
                int X = 37 + (int)player.transform.position.x;
                int Y = 19 + (int)player.transform.position.z;

                team.Graph[X, Y] = true;
                for (int value2 = 1; value2 < 3; value2++)
                {
                    team.Graph[X, Y + value2] = true;
                    team.Graph[X, Y - value2] = true;
                }
                for (int value1 = 1; value1 < 3; value1++)
                {
                    team.Graph[X + value1, Y] = true;
                    team.Graph[X - value1, Y] = true;
                    for (int value2 = 1; value2 < 3; value2++)
                    {
                        team.Graph[X + value1, Y + value2] = true;
                        team.Graph[X - value1, Y + value2] = true;
                        team.Graph[X + value1, Y - value2] = true;
                        team.Graph[X - value1, Y - value2] = true;
                    }
                }
            }
        }

        // Print the cost map for debugging
        /*
        count++;
        if (count > 1000)
        {


            string print1 = "";
            string print2 = "";
            for (int value1 = 0; value1 < Game.Instance.Teams[0].Graph.GetLength(0); value1++)
            {
                for (int value2 = 0; value2 < Game.Instance.Teams[0].Graph.GetLength(1); value2++)
                {
                    if (Game.Instance.Teams[0].Graph[value1, value2])
                    {
                        print1 += "1,";
                    }
                    else
                    {
                        print1 += "0,";
                    }
                    if (Game.Instance.Teams[1].Graph[value1, value2])
                    {
                        print2 += "1,";
                    }
                    else
                    {
                        print2 += "0,";
                    }
                }
                print1 += "\n";
                print2 += "\n";
            }
            count = 0;
        }
        */
    }

    public List<GridPoint> FindPath(bool[,] graph, GridPoint start, GridPoint goal)
    {
        count2 = 0;
        openSet.Clear();
        //Debug.Log(string.Format("FindPath {0} {1} | {2} {3} | {4}", start.X, start.Y, goal.X, goal.Y, openSet.Count));
        openSet[start] = true;
        gScore[start] = 0;
        fScore[start] = Heuristic(start, goal);

        //Debug.Log(string.Format("openSet.Count {0}", openSet.Count));
        while (openSet.Count > 0)
        {
            //Debug.Log(string.Format("openSet.Count {0}", openSet.Count));
            var current = nextBest();
            if (current.Equals(goal))
            {
                return Reconstruct(current);
            }
            //Debug.Log(string.Format("current {0} {1}", current.X, current.Y));

            // Limit the number of iterations to save processing
            count2++;
            if (count2 > 500)
            {
                //Debug.Log(string.Format("FindPath openSet.Count loop breaked {0}", openSet.Count));
                count2 = 0;
                return Reconstruct(current);
            }

            openSet.Remove(current);
            closedSet[current] = true;

            foreach (var neighbor in Neighbors(graph, current))
            {
                //Debug.Log(string.Format("neighbor {0} {1}", neighbor.X, neighbor.Y));
                if (closedSet.ContainsKey(neighbor))
                    continue;

                var projectedG = getGScore(current) + 1;

                if (!openSet.ContainsKey(neighbor))
                    openSet[neighbor] = true;
                else if (projectedG >= getGScore(neighbor))
                    continue;

                //record it
                nodeLinks[neighbor] = current;
                gScore[neighbor] = projectedG;
                fScore[neighbor] = projectedG + Heuristic(neighbor, goal);

            }
        }
        return new List<GridPoint>();
    }

    // Calculate Heuristic cost from start to goal
    private int Heuristic(GridPoint start, GridPoint goal)
    {
        //Debug.Log(string.Format("Heuristic {0} {1} | {2} {3}", start.X, start.Y, goal.X, goal.Y));
        var dx = goal.X - start.X;
        var dy = goal.Y - start.Y;
        return Math.Abs(dx) + Math.Abs(dy);
    }

    private int getGScore(GridPoint pt)
    {
        int score = int.MaxValue;
        gScore.TryGetValue(pt, out score);
        return score;
    }


    private int getFScore(GridPoint pt)
    {
        int score = int.MaxValue;
        fScore.TryGetValue(pt, out score);
        return score;
    }

    public static IEnumerable<GridPoint> Neighbors(bool[,] graph, GridPoint center)
    {
        GridPoint pt = new GridPoint(center.X - 1, center.Y - 1);
        if (IsValidNeighbor(graph, pt))
            yield return pt;

        pt = new GridPoint(center.X, center.Y - 1);
        if (IsValidNeighbor(graph, pt))
            yield return pt;

        pt = new GridPoint(center.X + 1, center.Y - 1);
        if (IsValidNeighbor(graph, pt))
            yield return pt;

        //middle row
        pt = new GridPoint(center.X - 1, center.Y);
        if (IsValidNeighbor(graph, pt))
            yield return pt;

        pt = new GridPoint(center.X + 1, center.Y);
        if (IsValidNeighbor(graph, pt))
            yield return pt;


        //bottom row
        pt = new GridPoint(center.X - 1, center.Y + 1);
        if (IsValidNeighbor(graph, pt))
            yield return pt;

        pt = new GridPoint(center.X, center.Y + 1);
        if (IsValidNeighbor(graph, pt))
            yield return pt;

        pt = new GridPoint(center.X + 1, center.Y + 1);
        if (IsValidNeighbor(graph, pt))
            yield return pt;
    }

    public static bool IsValidNeighbor(bool[,] matrix, GridPoint pt)
    {
        int x = pt.X;
        int y = pt.Y;
        if (x < 10 || x >= 65)
            return false;

        if (y < 8 || y >= 30)
            return false;

        //Debug.Log(string.Format("IsValidNeighbor {0} {1} {2}", x, y, !matrix[x, y]));
        return !matrix[x,y];

    }

    private List<GridPoint> Reconstruct(GridPoint current)
    {
        List<GridPoint> path = new List<GridPoint>();
        while (nodeLinks.ContainsKey(current))
        {
            path.Add(current);
            current = nodeLinks[current];
        }

        path.Reverse();
        return path;
    }

    private GridPoint nextBest()
    {
        int best = int.MaxValue;
        GridPoint bestPt = null;
        foreach (var node in openSet.Keys)
        {
            var score = getFScore(node);
            if (score < best)
            {
                bestPt = node;
                best = score;
            }
        }


        return bestPt;
    }

}