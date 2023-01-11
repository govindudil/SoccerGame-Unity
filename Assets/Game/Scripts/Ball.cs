using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Transform transformPlayer;
    private bool stickToPlayer;
    private Transform playerBallPosition;
    float speed;
    Vector3 previousLocation;
    Player scriptPlayer;

    public bool StickToPlayer { get => stickToPlayer; set => stickToPlayer = value; }

    // Start is called before the first frame update
    void Start()
    {
<<<<<<< Updated upstream
        playerBallPosition = transformPlayer.Find("Geometry").Find("BallLocation");
        scriptPlayer = transformPlayer.GetComponent<Player>();
=======
        timePassedBall = Time.time;
        Game.Instance.PassDestinationPlayer = player.FellowPlayer;
    }

    public void PutOnGround()
    {
        transform.position = new Vector3(transform.position.x, BALL_GROUND_POSITION_Y, transform.position.z);
    }
    public void PutOnCenterSpot()
    {
        transform.position = new Vector3(0f, BALL_GROUND_POSITION_Y, 0f);
    }

    private void TakeThrowIn()
    {
        if (Game.Instance.PlayerWithBall != null)
        {
            Game.Instance.PlayerWithBall.HasBall = false;
            Game.Instance.SetPlayerWithBall(null);
        }
        transform.position = ballOutOfFieldposition;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        Player player = Game.Instance.GetPlayerToThrowIn();
        player.SetPosition(new Vector3(ballOutOfFieldposition.x, player.transform.position.y, ballOutOfFieldposition.z));
        // Look in the directon of the field. Otherwise the ball will be out of field again.
        player.transform.LookAt(Game.Instance.KickOffPosition);
        Game.Instance.SetPlayerWithBall(player);
        if (isThrowIn)
        {
            player.TakeThrowIn = true;
        }
        else
        {
            player.TakeFreeKick = true;
        }
        // move players that are too close
        Game.Instance.SetMinimumDistanceOtherPlayers(player);
    }

    private void CheckBallOutOfField()
    {
        // ball out of field
        if (transform.position.z < Game.FIELD_BOUNDARY_LOWER_Z)
        {
            soundWhistle.Play();
            isThrowIn = true;
            ballOutOfFieldTimeOut = 1.0f;
            ballOutOfFieldposition = new Vector3(transform.position.x, BALL_GROUND_POSITION_Y, Game.FIELD_BOUNDARY_LOWER_Z);
        }
        if (transform.position.z > Game.FIELD_BOUNDARY_UPPER_Z)
        {
            soundWhistle.Play();
            isThrowIn = true;
            ballOutOfFieldTimeOut = 1.0f;
            ballOutOfFieldposition = new Vector3(transform.position.x, BALL_GROUND_POSITION_Y, Game.FIELD_BOUNDARY_UPPER_Z);
        }

        if (transform.position.x < Game.FIELD_BOUNDARY_LOWER_X)
        {
            soundWhistle.Play();
            isThrowIn = false;
            ballOutOfFieldTimeOut = 1.0f;
            if (Game.Instance.TeamLastTouched == 0)
            {
                // goal kick
                ballOutOfFieldposition = new Vector3(-29.56f, BALL_GROUND_POSITION_Y, -4.84f);
            }
            else
            {
                // corner
                if (transform.position.z > 0)
                {
                    ballOutOfFieldposition = new Vector3(Game.FIELD_BOUNDARY_LOWER_X, BALL_GROUND_POSITION_Y, Game.FIELD_BOUNDARY_UPPER_Z);
                }
                else
                {
                    ballOutOfFieldposition = new Vector3(Game.FIELD_BOUNDARY_LOWER_X, BALL_GROUND_POSITION_Y, Game.FIELD_BOUNDARY_LOWER_Z);
                }
            }
        }
        if (transform.position.x > Game.FIELD_BOUNDARY_UPPER_X)
        {
            soundWhistle.Play();
            isThrowIn = false;
            ballOutOfFieldTimeOut = 1.0f;
            if (Game.Instance.TeamLastTouched == 1)
            {
                // goal kick
                ballOutOfFieldposition = new Vector3(29.432f, BALL_GROUND_POSITION_Y, 4.76f);
            }
            else
            {
                // corner
                if (transform.position.z > 0)
                {
                    ballOutOfFieldposition = new Vector3(Game.FIELD_BOUNDARY_UPPER_X, BALL_GROUND_POSITION_Y, Game.FIELD_BOUNDARY_UPPER_Z);
                }
                else
                {
                    ballOutOfFieldposition = new Vector3(Game.FIELD_BOUNDARY_UPPER_X, BALL_GROUND_POSITION_Y, Game.FIELD_BOUNDARY_LOWER_Z);
                }
            }
        }
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
        if (!stickToPlayer)
        {
            float distanceToPlayer = Vector3.Distance(transformPlayer.position, transform.position);
            //Debug.Log("Distance: " + distanceToPlayer);
            if (distanceToPlayer < 0.5)
            {
                stickToPlayer = true;
                scriptPlayer.BallAttachedToPlayer = this;
            }
        }
        else
        {
            Vector2 currentLocation = new Vector2(transform.position.x, transform.position.z);
            speed = Vector2.Distance(currentLocation, previousLocation) / Time.deltaTime;
            transform.position = playerBallPosition.position;
            transform.Rotate(new Vector3(transformPlayer.right.x, 0, transformPlayer.right.z), speed, Space.World);
            previousLocation = currentLocation;
        }
        if (transform.position.y < -1)
        {
            transform.position = new Vector3(0, -0.555f, 0);
            Rigidbody rigidBody = GetComponent<Rigidbody>();
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
    }
}
