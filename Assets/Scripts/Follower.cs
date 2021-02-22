using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : Being
{
    public FlockEntity flock;
    public Transform myLeader;
    public float stopDist;

    public FollowerFlags flags;

    public float instanceNumber;
    //FSM
    IdleState<states> _idleState;
    FollowState<states> _followState;
    AttackState<states> _attackState;
    EscapeState<states> _escapeState;
    ReloadState<states> _reloadState;
    //Decision Tree
    ActionNode _idleAction;
    ActionNode _followAction;
    ActionNode _attackAction;
    ActionNode _escapeAction;
    ActionNode _reloadAction;

    INode _treeStartPoint;

    public SkinnedMeshRenderer mesh;

    public override void Awake()
    {
        base.Awake();
        flags = new FollowerFlags();
        Leader[] leader = FindObjectsOfType<Leader>();

        for (int i = 0; i < leader.Length; i++)
        {
            if (leader[i].selectedTeam == selectedTeam)
            {
                myLeader = leader[i].transform;
                enemyTeam = leader[i].enemyTeam;
                return;
            }
        }
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        flock = GetComponent<FlockEntity>();
    }
    public override void Move()
    {
        Vector3 dir = Vector3.zero;
        dir += flock.GetDir();
        dir += obstacleAvoidance.GetDir();
        dir.y = 0;
        directionForGizmo = dir;
        transform.position += dir * speed * ( DistanceToLeader() / 10) * Time.deltaTime;
        if (flags.isCloseToLeader == false)
            transform.LookAt(myLeader);
    }

    public override void Escape()
    {
        Vector3 dir = Vector3.zero;

        dir += (EnemyTeamCenter() - transform.position).normalized;
        dir += flock.GetDir();
        dir += obstacleAvoidance.GetDir();
        dir = dir * -1;
        dir.y = 0;
        directionForGizmo = dir;
        transform.position += dir * speed * Time.deltaTime;
        transform.LookAt(EnemyTeamCenter() * -1);
    }
    public override void StartFSM()
    {
        _idleState = new IdleState<states>(this, flags, _treeStartPoint);
        _followState = new FollowState<states>(this, myLeader, flags, _treeStartPoint, stopDist);
        _attackState = new AttackState<states>(this, flags, _treeStartPoint);
        _escapeState = new EscapeState<states>(this, flags, _treeStartPoint);
        _reloadState = new ReloadState<states>(this, flags, _treeStartPoint);

        fsm = new FSM<states>(_idleState);

        _idleState.AddTransition(states.Follow, _followState);
        _idleState.AddTransition(states.Attack, _attackState);
        _idleState.AddTransition(states.Escape, _escapeState);
        _idleState.AddTransition(states.Reload, _reloadState);

        _followState.AddTransition(states.Idle, _idleState);
        _followState.AddTransition(states.Attack, _attackState);
        _followState.AddTransition(states.Escape, _escapeState);
        _followState.AddTransition(states.Reload, _reloadState);

        _attackState.AddTransition(states.Idle, _idleState);
        _attackState.AddTransition(states.Follow, _followState);
        _attackState.AddTransition(states.Escape, _escapeState);
        _attackState.AddTransition(states.Reload, _reloadState);

        _reloadState.AddTransition(states.Idle, _idleState);
        _reloadState.AddTransition(states.Attack, _attackState);
        _reloadState.AddTransition(states.Follow, _followState);
        _reloadState.AddTransition(states.Escape, _escapeState);

        _escapeState.AddTransition(states.Idle, _idleState);
        _escapeState.AddTransition(states.Follow, _followState);

        fsm.SetState(_idleState);
    }

    public override void StartTree()
    {
        _idleAction = new ActionNode(IdleAction);
        _followAction = new ActionNode(FollowAction);
        _attackAction = new ActionNode(AttackAction);
        _escapeAction = new ActionNode(EscapeAction);
        _reloadAction = new ActionNode(ReloadAction);

        var questionCanShoot = new QuestionNode(CanShootQuestion, _attackAction, _reloadAction);
        var questionInSight = new QuestionNode(InSightQuestion, questionCanShoot, _idleAction);
        var questionSafe = new QuestionNode(SafeDistanceQuestion, _idleAction, _escapeAction);
        var questionCloseToLeader = new QuestionNode(IsCloseToLeaderQuestion, questionInSight, _followAction);
        var questionLowHP = new QuestionNode(LowHPQuestion, questionSafe, questionCloseToLeader);

        _treeStartPoint = questionLowHP;
    }

    #region Actions para la transición a otro estado
    public void IdleAction()
    {
        fsm.Transition(states.Idle);
    }

    public void AttackAction()
    {
        fsm.Transition(states.Attack);
    }

    public void EscapeAction()
    {
        fsm.Transition(states.Escape);
    }
    
    public void FollowAction()
    {
        fsm.Transition(states.Follow);
    }

    public void ReloadAction()
    {
        fsm.Transition(states.Reload);
    }
    #endregion

    #region Preguntas del árbol
    public bool LowHPQuestion()
    {
        if (currentHP <= maxHP / 4)
            flags.lowHP = true;
        else flags.lowHP = false;
        return flags.lowHP;
    }

    public bool EscapeQuestion()
    {
        return flags.escape;
    }

    public bool SafeDistanceQuestion()
    {
        Collider[] closeEnemies = Physics.OverlapSphere(transform.position, dangerDetectionRange);
        for (int i = 0; i < closeEnemies.Length; i++)
        {
            Being enemy = closeEnemies[i].GetComponent<Being>();
            if (CheckIfEnemy(enemy))
            {
                flags.safeDistance = false;
                return flags.safeDistance;
            }
        }
        flags.safeDistance = true;
        return flags.safeDistance;
    }

    public bool InSightQuestion()
    {
        Transform closestEnemy = GetClosestEnemy();
        transform.LookAt(closestEnemy);
        flags.inSight = lineOfSight.IsInSight(closestEnemy);
        if (flags.inSight)
            Debug.Log("insight");
        return flags.inSight;
    }

    public bool CanShootQuestion()
    {
        return flags.canShoot;
    }

    public bool IsCloseToLeaderQuestion()
    {
        return flags.isCloseToLeader;
    }
    #endregion

    public float DistanceToLeader()
    {
        if (myLeader)
            return (myLeader.transform.position - transform.position).magnitude;
        else return 0;
    }

    public override void Dead()
    {
        manager.ReduceCounter(selectedTeam);
        Destroy(gameObject);
    }

    public override void Idle()
    {
        mesh.material = idleMaterial;
        rb.isKinematic = true;
        if (DistanceToLeader() <= stopDist)
        {
            Vector3 dir = Vector3.zero;
            if (transform != null)
            {
                dir += (myLeader.position - transform.position).normalized;
                dir += flock.GetDir();
                dir.y = 0;
                dir = dir * -1;
                directionForGizmo = dir;
                transform.position += dir * speed * Time.deltaTime;
            }
        }
    }
}
