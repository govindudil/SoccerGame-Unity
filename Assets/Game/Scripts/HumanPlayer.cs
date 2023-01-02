using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : MonoBehaviour
{
    Player scriptPlayer;
    private StarterAssetsInputs starterAssetsInputs;

    [SerializeField] private float movementSpeed = 7.0f;
    [SerializeField] private float shootingPower = 0.7f;
    private Transform transformBall;
    private Animator animator;
    private Transform playerBallPosition;
    private Vector3 targetGoalPosition;
    private Vector3 ownGoalPosition;
    private Vector3[] attackTargetLocation = new Vector3[2];

    void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        scriptPlayer = GetComponent<Player>();
    }

    void Start()
    {
        transformBall = GameObject.Find("Ball").transform;
        animator = GetComponent<Animator>();
        playerBallPosition = transform.Find("BallPosition");
        targetGoalPosition = new Vector3(-33.07f, Game.PLAYER_Y_POSITION, 0f);
        ownGoalPosition = new Vector3(33.07f, Game.PLAYER_Y_POSITION, 0f);
        attackTargetLocation[0] = new Vector3(-30f, Game.PLAYER_Y_POSITION, 10f);
        attackTargetLocation[1] = new Vector3(-30f, Game.PLAYER_Y_POSITION, -10f);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(string.Format("{0} | {1} | {2}", scriptPlayer.name, scriptPlayer.CharacterController.enabled, scriptPlayer.PlayerInput.enabled));
        //Debug.Log(scriptPlayer.HumanControlled);
        if (scriptPlayer.HumanControlled)
        {

            /*if (starterAssetsInputs.pass)
            {
                starterAssetsInputs.pass = false;
                scriptPlayer.Pass();
            }*/

            if (starterAssetsInputs.shoot)
            {
                if (scriptPlayer.HasBall)
                {
                    scriptPlayer.ShootingPower += 1.5f * Time.deltaTime;
                    Game.Instance.SetPowerBar(scriptPlayer.ShootingPower);
                    if (scriptPlayer.ShootingPower > 1)
                    {
                        scriptPlayer.ShootingPower = 1;
                    }
                }
            }
            else if (scriptPlayer.ShootingPower > 0)
            {
                scriptPlayer.Shoot();
            }
        }
        else
        {
            if (Game.Instance.WaitingForKickOff)
            {
                return;
            }

            if (Game.Instance.TeamWithBall != scriptPlayer.Team.Number)
            {
                DefendMode();
            }
            else
            {
                AttackMode();
            }
        }
    }

    // players move towards enemy goal, one to z=12, one to z=-12
    private void AttackMode()
    {
        // move to target goal
        Vector3 movedirection = attackTargetLocation[scriptPlayer.Number] - new Vector3(playerBallPosition.position.x, 0, playerBallPosition.position.z);
        float distanceToGoal = movedirection.magnitude;
        float speed = movementSpeed;
        if (scriptPlayer.HasBall)
        {
            speed *= Game.HAVING_BALL_SLOWDOWN_FACTOR;
        }
        Vector3 moveSpeed = new Vector3(movedirection.normalized.x * speed * Time.deltaTime, 0, movedirection.normalized.z * speed * Time.deltaTime);
        transform.position += moveSpeed;
        transform.LookAt(targetGoalPosition);
        animator.SetFloat("Speed", speed);
        animator.SetFloat("MotionSpeed", 1);
        // shoot
        if (scriptPlayer.HasBall && distanceToGoal < 15)
        {
            scriptPlayer.ShootingPower = shootingPower;
            animator.SetFloat("Speed", 0);
            animator.SetFloat("MotionSpeed", 0);
            scriptPlayer.Shoot();
        }
    }

    // player closest to ball tries to steal it
    // player farthest from ball moves between goal and player closest to goal
    private void DefendMode()
    {
        if (Game.Instance.PlayerClosestToBall(scriptPlayer.Team.Number) == scriptPlayer)
        {
            MoveToBall();
        }
        else
        {
            MoveToBetweenGoalAndPlayerClosestToGoal();
        }
    }

    private void MoveToBetweenGoalAndPlayerClosestToGoal()
    {
        Vector3 mostDangerousEnemyPlayer = Game.Instance.PlayerClosestToLocation(0, ownGoalPosition).transform.position;
        Vector3 targetLocation = Vector3.Lerp(ownGoalPosition, mostDangerousEnemyPlayer, 0.5f);
        Vector3 movedirection = targetLocation - playerBallPosition.position;
        Vector3 moveSpeed = new Vector3(movedirection.normalized.x * movementSpeed * Time.deltaTime, 0, movedirection.normalized.z * movementSpeed * Time.deltaTime);
        transform.position += moveSpeed;

        if (moveSpeed.magnitude > 0.005)
        {
            transform.LookAt(targetLocation);
        }
        else
        {
            transform.LookAt(mostDangerousEnemyPlayer);
        }

        animator.SetFloat("Speed", moveSpeed.magnitude * 200);
        animator.SetFloat("MotionSpeed", 1);
    }

    private void MoveToBall()
    {
        Vector3 lookAtPosition = transformBall.position;
        lookAtPosition.y = transform.position.y;
        transform.LookAt(lookAtPosition);
        Vector3 movedirection = transformBall.position - playerBallPosition.position;
        Vector3 moveSpeed = new Vector3(movedirection.normalized.x * movementSpeed * Time.deltaTime, 0, movedirection.normalized.z * movementSpeed * Time.deltaTime);
        transform.position += moveSpeed;
        animator.SetFloat("Speed", moveSpeed.magnitude * 200);
        animator.SetFloat("MotionSpeed", 1);
    }
}
