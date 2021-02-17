
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Being : Teams
{
    public enum states
    {
        Idle, Follow, Lead, Attack, Buff, Escape
    }
    public FSM<states> fsm;
    public Team selectedTeam;
    public float maxHP;
    public float currentHP;
    public float regenTimer;
    public float passiveHPRegen;
    public float speed;
    public float damage;
    public Projectile projectile;
    public Transform projectileSpawn;
    public float cooldown;
    public float currentCooldown;

    public float dangerDetectionRange;
    public LineOfSight lineOfSight;
    public LayerMask obstacle;

    public ObstacleAvoidance obstacleAvoidance;
    public float obstacleAvoidRadius;
    public float obstacleAvoidWeight;
    public bool test;

    
    public Material idleMaterial;
    public Material defaultMaterial;

    public List<Being> enemyTeam = new List<Being>();


    public bool stopUpdate = true;

    public RouletteWheel roulette;
    delegate void RouletteAction();
    public Dictionary<string, int> rouletteActions = new Dictionary<string, int>();
    Dictionary<string, RouletteAction> _rouletteExecute = new Dictionary<string, RouletteAction>();
    public int cooldownReductionWeight;
    public float cooldownReductionValue;
    string _cooldownReduction = "Cooldown Reduction";
    public int extraSpeedWeight;
    public float extraSpeedValue;
    string _extraSpeed = "Extra Speed";
    public int extraDamageWeight;
    public float extraDamageValue;
    string _extraDamage = "Extra Damage";
    public int extraHealthWeight;
    public float extraHealthValue;
    string _extraHealth = "Extra Health";

    public Rigidbody rb;

    public GameManager manager;

    
    public virtual void Awake()
    {
        lineOfSight = GetComponent<LineOfSight>();
        
        
        obstacleAvoidance = new ObstacleAvoidance(transform, obstacleAvoidRadius, obstacleAvoidWeight, obstacle);
        rb = GetComponent<Rigidbody>();
        stopUpdate = true;

        manager = FindObjectOfType<GameManager>();
        InitializeRouletteWheel();
    }
    public virtual void Start()
    {
        currentHP = maxHP;
        
        if (test == false)
        {
            StartTree();
            StartFSM();
        }
    }

    public virtual void Update()
    {
        if (stopUpdate == false && test == false)
        {
            if (currentHP <= 0)
                Dead();
            fsm.OnUpdate();
            Debug.Log("current state: " + fsm._currentState.ToString() + "object: " + gameObject.name);
            PassiveHPRegen();
        }
    }

    public abstract void Move();

    public abstract void Escape();

    public abstract void StartFSM();

    public abstract void StartTree();

    public void InitializeRouletteWheel()
    {
        roulette = new RouletteWheel();

        //Agrego las acciones al diccionario que se va a pasar cuando se llame a la ruleta.
        rouletteActions.Add(_cooldownReduction, cooldownReductionWeight);
        rouletteActions.Add(_extraSpeed, extraSpeedWeight);
        rouletteActions.Add(_extraDamage, extraDamageWeight);
        rouletteActions.Add(_extraHealth, extraHealthWeight);

        //Agrego los métodos que se van a ejecutar según lo que devuelva la ruleta.
        _rouletteExecute.Add(_cooldownReduction, ReduceCooldown);
        _rouletteExecute.Add(_extraSpeed, ExtraSpeed);
        _rouletteExecute.Add(_extraDamage, ExtraDamage);
        _rouletteExecute.Add(_extraHealth, ExtraHealth);
    }
    public abstract void Idle();

    public void ApplyBuff(string action)
    {
        _rouletteExecute[action]();
    }
    #region Buffs
    public virtual void ReduceCooldown()
    {
        cooldown = cooldown / 2;
    }

    public virtual void ExtraSpeed()
    {
        speed += extraSpeedValue;
    }

    public virtual void ExtraDamage()
    {
        damage += extraDamageValue;
    }

    public virtual void ExtraHealth()
    {
        currentHP += extraHealthValue;
    }
    #endregion
    public void Attack()
    {
        Instantiate(projectile, projectileSpawn.position, Quaternion.identity, projectileSpawn);
        projectile.transform.parent = null;
        Transform closestEnemy = GetClosestEnemy();
        if (closestEnemy)
        {
            projectile.dir = (closestEnemy.position - transform.position).normalized;
            projectile.damage = damage;
            projectile.selectedTeam = selectedTeam;
        }
        else Destroy(gameObject);
        
    }

    public Vector3 EnemyTeamCenter()
    {
        Vector3 dir = Vector3.zero;
        int aliveEnemies = 0;
        foreach (var enemy in enemyTeam)
        {
            //Sumo las posiciones de los enemigos vivos y cuantos son
            if (enemy != null)
            {
                dir += enemy.transform.position;
                aliveEnemies++;
            }
        }

        //Punto medio (cambiar Y segun la altura del mapa)
        dir.x = dir.x / aliveEnemies;
        dir.y = 0;
        dir.z = dir.z / aliveEnemies;
        return dir;
    }

    public Transform GetClosestEnemy()
    {
        Collider[] closeEnemies = Physics.OverlapSphere(transform.position, dangerDetectionRange);
        Transform closest = null;
        float closestDist = dangerDetectionRange;
        for (int i = 1; i < closeEnemies.Length; i++)
        {
            var enemy = closeEnemies[i].GetComponent<Being>();
            if (CheckIfEnemy(enemy))
            {
                if ((enemy.transform.position - transform.position).magnitude <= closestDist)
                {
                    closest = enemy.transform;
                    closestDist = (closest.position - transform.position).magnitude;
                }
            }
        }
        return closest;
    }

    public bool CheckIfEnemy(Being enemy)
    {
        if (enemy != null && enemy.selectedTeam != selectedTeam)
            return true;
        else return false;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHP -= damage;
    }

    public abstract void Dead();

    public void StopWorking()
    {
        stopUpdate = true;
        rb.velocity = Vector3.zero;
    }

    public void StartWorking()
    {
        stopUpdate = false;
    }

    public virtual void PassiveHPRegen()
    {
        if (currentHP < maxHP)
        {
            regenTimer += Time.deltaTime;

            if (regenTimer >= 5)
            {
                regenTimer = 0;
                currentHP += passiveHPRegen;
            }
        }
    }
}
