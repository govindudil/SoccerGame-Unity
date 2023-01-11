using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIPlayer : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 4.0f;
    [SerializeField] private float shootingPower = 0.7f;
    private Transform transformBall;
    private Player scriptPlayer;
    private Animator animator;
    private Transform playerBallPosition;
    private Vector3 targetGoalPosition;
    private Vector3 ownGoalPosition;
    private Vector3[] attackTargetLocation = new Vector3[5]; private AIPlayer[] allOtherPlayers;
    private AIPlayer NearestPlayer;
    float distFromNearestPlayer = 100000;
    float distanceFromPlayers;

    void Awake()
    {
        scriptPlayer = GetComponent<Player>();
    }

    void Start()
    {
        transformBall = GameObject.Find("Ball").transform;
        animator = GetComponent<Animator>();
        playerBallPosition = transform.Find("BallPosition");
        targetGoalPosition = new Vector3(33.07f, Game.PLAYER_Y_POSITION, 0f);
        ownGoalPosition = new Vector3(-33.07f, Game.PLAYER_Y_POSITION, 0f);
        attackTargetLocation[0] = new Vector3(30f, Game.PLAYER_Y_POSITION, 10f);
        attackTargetLocation[1] = new Vector3(20f, Game.PLAYER_Y_POSITION, 5f);
        attackTargetLocation[2] = new Vector3(10f, Game.PLAYER_Y_POSITION, 0f);
        attackTargetLocation[3] = new Vector3(20f, Game.PLAYER_Y_POSITION, -5f);
        attackTargetLocation[4] = new Vector3(30f, Game.PLAYER_Y_POSITION, -10f);/*
        List<GridPoint> path = AStar.Instance.FindPath(scriptPlayer.Team.Graph, new GridPoint(scriptPlayer.transform.position), new GridPoint(targetGoalPosition));
        Debug.Log(path.Count);
        foreach (var x in path)
        {
            Debug.Log(string.Format("{0} {1}", x.X, x.Y));
        }*/
    }

    void Update()
    {
        if (Game.Instance.WaitingForKickOff)
        {
            return;
        }

        if (Game.Instance.TeamWithBall != scriptPlayer.Team.Number)
        {
            // If the ball is with an opponent go to defend state
            DefendMode();
        }
        else
        {
            // If the ball is with an ally go to attack state
            AttackMode();
        }
    }

    // players move towards enemy goal
    private void AttackMode()
    {
        if (scriptPlayer.HasBall)
        {
            List<GridPoint> path = AStar.Instance.FindPath(Game.Instance.Teams[0].Graph, new GridPoint(transform.position), new GridPoint(targetGoalPosition));
            //Debug.Log(path.Count);
            string str = "";
            foreach (var x in path)
            {
                str += string.Format("[{0}:{1}], ", x.X, x.Y);
            }

            Vector3 movedirection;
            if (path.Count == 0)
            {
                movedirection = targetGoalPosition - new Vector3(playerBallPosition.position.x, Game.PLAYER_Y_POSITION, playerBallPosition.position.z);
            }
            else
            {
                movedirection = new GamePoint(path[0].X, path[0].Y).vector - new GamePoint(transform.position).vector;
            }

            //Debug.Log(string.Format("Vector: {0} {1}", movedirection.x, movedirection.z));
            float distanceToGoal = (targetGoalPosition - transform.position).magnitude;
            //Debug.Log(string.Format("targetGoalPosition: {0} {1}", targetGoalPosition.x, targetGoalPosition.z));
            float speed = movementSpeed;
            speed *= Game.HAVING_BALL_SLOWDOWN_FACTOR;
            Vector3 moveSpeed = new Vector3(movedirection.normalized.x * speed * Time.deltaTime, 0, movedirection.normalized.z * speed * Time.deltaTime);
            //Debug.Log(string.Format("Speed: {0} {1}", moveSpeed.x, moveSpeed.z));
            transform.position += moveSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movedirection), Time.deltaTime * 10f);
            animator.SetFloat("Speed", speed);
            animator.SetFloat("MotionSpeed", 1);
            //Debug.Log(string.Format("distanceToGoal: {0}", distanceToGoal));

            // shoot
            if (distanceToGoal < 10)
            {
                transform.LookAt(targetGoalPosition);
                scriptPlayer.ShootingPower = shootingPower;
                animator.SetFloat("Speed", 0);
                animator.SetFloat("MotionSpeed", 0);
                scriptPlayer.Shoot();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //Debug.Log("Ball is Passed");
                scriptPlayer.ShootingPower = shootingPower;
                animator.SetFloat("Speed", 0);
                animator.SetFloat("MotionSpeed", 0);
                AIPass();

            }

        } 
        else
        {
            // move to target goal
            Vector3 movedirection = attackTargetLocation[scriptPlayer.Number] - new Vector3(playerBallPosition.position.x, 0, playerBallPosition.position.z);
            float distanceToGoal = movedirection.magnitude;
            float speed = movementSpeed;
            Vector3 moveSpeed = new Vector3(movedirection.normalized.x * speed * Time.deltaTime, 0, movedirection.normalized.z * speed * Time.deltaTime);
            transform.position += moveSpeed;
            transform.LookAt(targetGoalPosition);
            animator.SetFloat("Speed", speed);
            animator.SetFloat("MotionSpeed", 1);
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
        Vector3 targetLocation;
        if (Game.Instance.PlayerWithBall == null)
        {
            targetLocation = Vector3.Lerp(ownGoalPosition, Game.Instance.Teams[0].Players[scriptPlayer.Number].transform.position, 0.5f);
        }
        else if (Game.Instance.PlayerWithBall.Number == scriptPlayer.Number)
        {
            targetLocation = Vector3.Lerp(ownGoalPosition, Game.Instance.PlayerWithBall.Team.Players[Game.Instance.PlayerClosestToBall(scriptPlayer.Team.Number).Number].transform.position, 0.5f);
        }
        else
        {
            targetLocation = Vector3.Lerp(ownGoalPosition, Game.Instance.Teams[0].Players[scriptPlayer.Number].transform.position, 0.5f);
        }
        //Vector3 mostDangerousEnemyPlayer = Game.Instance.PlayerClosestToLocation(0, ownGoalPosition).transform.position;
        //Vector3 targetLocation = Vector3.Lerp(ownGoalPosition, mostDangerousEnemyPlayer, 0.5f);
        Vector3 movedirection = targetLocation - playerBallPosition.position;
        Vector3 moveSpeed = new Vector3(movedirection.normalized.x * movementSpeed * Time.deltaTime, 0, movedirection.normalized.z * movementSpeed * Time.deltaTime);
        transform.position += moveSpeed;

        if (moveSpeed.magnitude > 0.005)
        {
            transform.LookAt(targetLocation);
        }
        else
        {
            if (Game.Instance.PlayerWithBall == null)
            {
                transform.LookAt(Game.Instance.Teams[0].Players[scriptPlayer.Number].transform.position);
            }
            else if (Game.Instance.PlayerWithBall.Number == scriptPlayer.Number)
            {
                transform.LookAt(Game.Instance.PlayerWithBall.Team.Players[Game.Instance.PlayerClosestToBall(scriptPlayer.Team.Number).Number].transform.position);
            }
            else
            {
                transform.LookAt(Game.Instance.Teams[0].Players[scriptPlayer.Number].transform.position);
            }
            //transform.LookAt(mostDangerousEnemyPlayer);
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
    public void AIPass()
    {
        //Debug.Log("In AIPass Function");

        //find all other ai players on team
        allOtherPlayers = FindObjectsOfType<AIPlayer>().Where(t => t != this).ToArray();
        //for every team member, find direction of closest one
        for (int i = 0; i < allOtherPlayers.Length; i++)
        {
            distanceFromPlayers = Vector3.Distance(this.transform.position, allOtherPlayers[i].transform.position);
            if (distanceFromPlayers < distFromNearestPlayer)
            {
                NearestPlayer = allOtherPlayers[i];
                distFromNearestPlayer = distanceFromPlayers;
            }
        }
        // Pass in the direction of closest team member
        Vector3 passDirection = NearestPlayer.transform.position - this.transform.position;
        //Debug.Log("Direction = " + passDirection);
        Quaternion rotation = Quaternion.LookRotation(passDirection);
        transform.rotation = rotation;
        // use shoot method for convience
        scriptPlayer.Shoot();

    }
}
