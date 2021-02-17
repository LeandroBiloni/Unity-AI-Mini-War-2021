using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeState<T> : States<T>
{
    Leader _leader;
    LeaderFlags _leaderFlags;

    Follower _follower;
    FollowerFlags _followerFlags;

    INode _treeStart;

    public EscapeState(Leader leader, LeaderFlags flags, INode treeStart)
    {
        _leader = leader;
        _leaderFlags = flags;
        _treeStart = treeStart;
    }

    public EscapeState(Follower follower, FollowerFlags flags, INode treeStart)
    {
        _follower = follower;
        _followerFlags = flags;
        _treeStart = treeStart;
    }
    public override void Execute()
    {
        
        Vector3 dir = Vector3.zero;
        if (_leader)
        {
            _leader.mesh.material = _leader.defaultMaterial;
            
            _leader.Escape();
        }
        
        if (_follower)
        {
            _follower.mesh.material = _follower.defaultMaterial;
            _follower.Escape();
        }
        _treeStart.Execute();
    }
}
