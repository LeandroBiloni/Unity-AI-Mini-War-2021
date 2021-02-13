using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffState<T> : States<T>
{
    Leader _leader;
    LeaderFlags _flags;
    INode _start;
    public BuffState(Leader leader, LeaderFlags flags, INode start)
    {
        _leader = leader;
        _flags = flags;
        _start = start;
    }

    public override void Execute()
    {
        _leader.mesh.material = _leader.defaultMaterial;
        if (_flags.buffAvailable)
        {
            string action = _leader.roulette.ExecuteAction(_leader.rouletteActions);
            _leader.BuffAllies(action);
        }

        _start.Execute();
    }
}
