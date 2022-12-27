using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textScore;
    private StarterAssetsInputs starterAssetsInputs;
    private Animator animator;
    private Ball ballAttachedToPlayer;
    private float timeShot = -1f;
    public const int ANIMATION_LAYER_SHOOT = 1;
    private int myScore, otherScore;
    private AudioSource soundKick;
    private AudioSource soundDribble;
    private CharacterController controller;
    private float distanceSinceLastDribble;

    public Ball BallAttachedToPlayer { get => ballAttachedToPlayer; set => ballAttachedToPlayer = value; }

    // Start is called before the first frame update
    void Start()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
        soundKick = GameObject.Find("Sound/kick").GetComponent<AudioSource>();
        soundDribble = GameObject.Find("Sound/dribble").GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float speed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
        if (starterAssetsInputs.shoot)
        {
            starterAssetsInputs.shoot = false;
            timeShot = Time.time;
            animator.Play("Shoot", ANIMATION_LAYER_SHOOT, 0f);
            animator.SetLayerWeight(ANIMATION_LAYER_SHOOT, 1f);
            //Debug.Log("Shoot!");
            //Debug.Log(timeShot);
        }
        if (timeShot > 0)
        {
            //Debug.Log("Outer loop: " + (Time.time - timeShot));
            // Shoot ball
            if (ballAttachedToPlayer != null && Time.time - timeShot > 0.4)
            {
                ballAttachedToPlayer.StickToPlayer = false;
                soundKick.Play();

                Rigidbody rigidBody = ballAttachedToPlayer.transform.gameObject.GetComponent<Rigidbody>();
                Vector3 shootDirection = transform.forward;
                shootDirection.y += 0.5f;
                rigidBody.AddForce(shootDirection * 20f, ForceMode.Impulse);
                //Debug.Log(shootDirection * 20f);

                ballAttachedToPlayer = null;
                //Debug.Log("Shoot.........................................: " + (Time.time - timeShot));
            }

            // Finish kicking animation
            if (Time.time - timeShot > 0.5)
            {
                timeShot = -1f;
                //Debug.Log("Finish kicking animation: " + timeShot);
            }
        }
        else
        {
            animator.SetLayerWeight(ANIMATION_LAYER_SHOOT, Mathf.Lerp(animator.GetLayerWeight(ANIMATION_LAYER_SHOOT), 0f, Time.deltaTime * 10f));
        }

        if (ballAttachedToPlayer != null)
        {
            distanceSinceLastDribble += speed * Time.deltaTime;
            if (distanceSinceLastDribble > 3)
            {
                soundDribble.Play();
                distanceSinceLastDribble = 0;
            }
        }
    }

    public void IncreaseMyScore()
    {
        myScore++;
        UpdateScore();
    }

    public void IncreaseOtherScore()
    {
        otherScore++;
        UpdateScore();
    }

    private void UpdateScore()
    {
        textScore.text = "Score: " + myScore.ToString() + "-" + otherScore.ToString();
    }
}
