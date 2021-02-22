using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState<T> : States<T>
{
    Leader _leader;
    LeaderFlags _leaderFlags;
    Follower _follower;
    FollowerFlags _followerFlags;
    INode _treeStart;

    public AttackState(Leader leader, LeaderFlags leaderFlags,  INode treeStart)
    {
        _leader = leader;
        _leaderFlags = leaderFlags;
        _treeStart = treeStart;
    }

    public AttackState(Follower follower, FollowerFlags followerFlags, INode treeStart)
    {
        _follower = follower;
        _followerFlags = followerFlags;
        _treeStart = treeStart;
    }
    public override void Execute()
    {
        if (_leader)
        {
            
            _leader.mesh.material = _leader.defaultMaterial;
            _leader.transform.LookAt(_leader.GetClosestEnemy());
            _leader.Attack();
            _leaderFlags.canShoot = false;
        }
        
        if (_follower)
        {
            Debug.Log("ataco");
            _follower.mesh.material = _follower.defaultMaterial;
            _follower.transform.LookAt(_follower.GetClosestEnemy());
            _follower.Attack();
            _followerFlags.canShoot = false;
        }
        _treeStart.Execute();
    }
}
