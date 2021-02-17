using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowState<T> : States<T>
{
    Transform _leader;
    Follower _follower;
    FollowerFlags _flags;
    INode _treeStart;
    float _stopDistance;
    public FollowState(Follower follower, Transform leader, FollowerFlags flags, INode treeStart, float stopDistance)
    {
        _follower = follower;
        _leader = leader;
        _flags = flags;
        _treeStart = treeStart;
        _stopDistance = stopDistance;
    }

    public override void Execute()
    {
        _follower.mesh.material = _follower.defaultMaterial;
        if (_follower.DistanceToLeader() <= _stopDistance)
        {
            _flags.isCloseToLeader = true;
        }
        else _flags.isCloseToLeader = false;

        if (_flags.isCloseToLeader == false)
        {
            _follower.Move();
        }
        else _treeStart.Execute();
    }
}
