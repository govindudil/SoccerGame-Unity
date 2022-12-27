using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private Player scriptPlayer;
    [SerializeField] private TextMeshProUGUI textGoal;
    private float goalTextColourAlpha;
    private AudioSource soundCheer;

    // Start is called before the first frame update
    void Start()
    {
        soundCheer = GameObject.Find("Sound/cheer").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (goalTextColourAlpha > 0)
        {
            goalTextColourAlpha -= Time.deltaTime * 0.5f;
            textGoal.alpha = goalTextColourAlpha;
            textGoal.fontSize = 200 - (goalTextColourAlpha * 100);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Ball"))
        {
            soundCheer.Play();
            if (name.Equals("GoalDetector1"))
            {
                scriptPlayer.IncreaseMyScore();
                textGoal.color = Color.green;
                goalTextColourAlpha = 1f;
            }
            else
            {
                scriptPlayer.IncreaseOtherScore();
                textGoal.color = Color.red;
                goalTextColourAlpha = 1f;
            }
        }
    }
}
