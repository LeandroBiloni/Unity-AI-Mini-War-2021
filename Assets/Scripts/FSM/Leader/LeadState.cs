using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeadState<T> : States<T>
{
    Leader _leader;
    LeaderFlags _flags;
    float _leadStopDistance;
    INode _treeStart;
    public LeadState(Leader leader, LeaderFlags flags, float leadStopDistance, INode treeStart)
    {
        _leader = leader;
        _flags = flags;
        _leadStopDistance = leadStopDistance;
        _treeStart = treeStart;
    }
    public override void Execute()
    {
        _leader.mesh.material = _leader.defaultMaterial;
        //Obtengo el punto para poder calcular la distancia
        Vector3 enemyTeamCenter = _leader.EnemyTeamCenter();
        if ((enemyTeamCenter - _leader.transform.position).magnitude <= _leadStopDistance)
        {
            _flags.inMiddle = true;
        }
        else _flags.inMiddle = false;

        if (_flags.inMiddle == false)
        {
            //Paso la direccion
            Vector3 dir = (enemyTeamCenter - _leader.transform.position).normalized;
            _leader.Move(dir);
        }
        else
        {
            _treeStart.Execute();
        }

        
    }
}
