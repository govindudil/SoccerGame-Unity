using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<Ball>() != null)
        {
            if (name.Equals("GoalDetector2"))
            {
                Game.Instance.ScoreGoal(1);
            }
            else
            {
                Game.Instance.ScoreGoal(0);
            }
        }
    }
}
