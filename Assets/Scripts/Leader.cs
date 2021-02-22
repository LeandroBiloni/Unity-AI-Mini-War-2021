using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : Being
{
    

    public float leadStopDistance;

    public float buffRange;

    public MeshRenderer mesh;


    public LeaderFlags flags;

    //FSM

    LeadState<states> _leadState;
    AttackState<states> _attackState;
    BuffState<states> _buffState;
    EscapeState<states> _escapeState;
    IdleState<states> _idleState;
    ReloadState<states> _reloadState;

    //Decision Tree
    ActionNode _idleAction;
    ActionNode _leadAction;
    ActionNode _attackAction;
    ActionNode _buffAction;
    ActionNode _escapeAction;
    ActionNode _reloadAction;

    INode _treeStartPoint;


    
    public override void Awake()
    {
        base.Awake();

        mesh = GetComponent<MeshRenderer>();
        defaultMaterial = mesh.material;
        flags = new LeaderFlags();
        flags.inMiddle = false;
        
    }
    public override void StartFSM()
    {
        _leadState = new LeadState<states>(this, flags, leadStopDistance, _treeStartPoint);
        _attackState = new AttackState<states>(this, flags, _treeStartPoint);
        _buffState = new BuffState<states>(this, flags, _treeStartPoint);
        _escapeState = new EscapeState<states>(this, flags, _treeStartPoint);
        _idleState = new IdleState<states>(this, flags, _treeStartPoint);
        _reloadState = new ReloadState<states>(this, flags, _treeStartPoint);
        
        fsm = new FSM<states>(_idleState);

        _idleState.AddTransition(states.Attack, _attackState);
        _idleState.AddTransition(states.Buff, _buffState);
        _idleState.AddTransition(states.Lead, _leadState);
        _idleState.AddTransition(states.Escape, _escapeState);
        _idleState.AddTransition(states.Reload, _reloadState);

        _leadState.AddTransition(states.Attack, _attackState);
        _leadState.AddTransition(states.Buff, _buffState);
        _leadState.AddTransition(states.Escape, _escapeState);
        _leadState.AddTransition(states.Idle, _idleState);
        _leadState.AddTransition(states.Reload, _reloadState);

        _attackState.AddTransition(states.Lead, _leadState);
        _attackState.AddTransition(states.Buff, _buffState);
        _attackState.AddTransition(states.Escape, _escapeState);
        _attackState.AddTransition(states.Idle, _idleState);
        _attackState.AddTransition(states.Reload, _reloadState);

        _reloadState.AddTransition(states.Idle, _idleState);
        _reloadState.AddTransition(states.Attack, _attackState);
        _reloadState.AddTransition(states.Buff, _buffState);
        _reloadState.AddTransition(states.Lead, _leadState);
        _reloadState.AddTransition(states.Escape, _escapeState);

        _buffState.AddTransition(states.Lead, _leadState);
        _buffState.AddTransition(states.Attack, _attackState);
        _buffState.AddTransition(states.Escape, _escapeState);
        _buffState.AddTransition(states.Idle, _idleState);
        _buffState.AddTransition(states.Reload, _reloadState);

        _escapeState.AddTransition(states.Idle, _idleState);
        _escapeState.AddTransition(states.Lead, _leadState);

        fsm.SetState(_idleState);
    }



    public override void StartTree()
    {
        _idleAction = new ActionNode(IdleAction);
        _leadAction = new ActionNode(LeadAction);
        _attackAction = new ActionNode(AttackAction);
        _buffAction = new ActionNode(BuffAction);
        _escapeAction = new ActionNode(EscapeAction);
        _reloadAction = new ActionNode(ReloadAction);

        var questionCanShoot = new QuestionNode(CanShootQuestion, _attackAction, _reloadAction);
        var questionInSight = new QuestionNode(InSightQuestion, questionCanShoot, _idleAction);
        var questionIsInMid = new QuestionNode(IsInMidQuestion, questionInSight, _leadAction);
        var questionBuff = new QuestionNode(BuffAvailableQuestion, _buffAction, questionIsInMid);
        var questionSafe = new QuestionNode(SafeDistanceQuestion,_idleAction, _escapeAction);
        var questionHalfHP = new QuestionNode(HalfHPQuestion, questionBuff, questionIsInMid);
        var questionLowHP = new QuestionNode(LowHPQuestion, questionSafe, questionHalfHP);

        _treeStartPoint = questionLowHP;
    }

    #region Actions para la transición a otro estado
    public void IdleAction()
    {
        fsm.Transition(states.Idle);
    }

    public void LeadAction()
    {
        fsm.Transition(states.Lead);
    }
    public void AttackAction()
    {
        fsm.Transition(states.Attack);
    }
    public void BuffAction()
    {
        fsm.Transition(states.Buff);
    }
    public void EscapeAction()
    {
        fsm.Transition(states.Escape);
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

    public bool HalfHPQuestion()
    {
        if (currentHP <= maxHP / 2)
            flags.halfHP = true;
        else flags.halfHP = false;
        return flags.halfHP;
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

    public bool CanShootQuestion()
    {
        return flags.canShoot;
    }

    public bool BuffAvailableQuestion()
    {
        return flags.buffAvailable;
    }

    public bool IsInMidQuestion()
    {
        return flags.inMiddle;
    }

    public bool InSightQuestion()
    {
        Transform closestEnemy = GetClosestEnemy();
        flags.inSight = lineOfSight.IsInSight(closestEnemy);
        return flags.inSight;
    }
    #endregion

    public override void Move()
    {
        transform.LookAt(EnemyTeamCenter());
        Vector3 dir = Vector3.zero;
        dir += (EnemyTeamCenter() - transform.position).normalized;
        dir += obstacleAvoidance.GetDir();
        dir.y = 0;
        directionForGizmo = dir;
        transform.position += dir * speed * Time.deltaTime;
    }
    public override void Escape()
    {
        Vector3 dir = Vector3.zero;

        dir += (EnemyTeamCenter() - transform.position).normalized;
        dir += obstacleAvoidance.GetDir();
        dir = dir * -1;
        dir.y = 0;
        directionForGizmo = dir;
        transform.position += dir * speed * Time.deltaTime;
        transform.LookAt(EnemyTeamCenter() * -1);
    }
    public void BuffAllies(string action)
    {
        //Busco los aliados cercanos y aplico un buff --- Hacer varios buff para que sea random
        Collider[] allies = Physics.OverlapSphere(transform.position, buffRange);
        int count = 0;
        for (int i = 0; i < allies.Length; i++)
        {
            Being ally = allies[i].GetComponent<Being>();
            if (ally && ally.selectedTeam == selectedTeam)
            {
                count++;
                ally.ApplyBuff(action);
            }
        }
        flags.buffAvailable = false;
    }

    public override void ExtraHealth()
    {
        base.ExtraHealth();
        manager.UpdateHP(selectedTeam, maxHP, currentHP);
    }

    public void LookAtEnemy()
    {
        Transform enemy = GetClosestEnemy();
        if (enemy != null)
            transform.LookAt(enemy);
    }

    public override void Idle()
    {
        mesh.material = idleMaterial;
        rb.isKinematic = true;
        if (flags.escape == false && flags.inMiddle)
            LookAtEnemy();
    }

    public override void Dead()
    {
        manager.EndSimulation(selectedTeam);
        Destroy(gameObject);
    }

    public override void PassiveHPRegen()
    {
        base.PassiveHPRegen();
        manager.UpdateHP(selectedTeam, maxHP, currentHP);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        manager.UpdateHP(selectedTeam, maxHP, currentHP);
    }
}
