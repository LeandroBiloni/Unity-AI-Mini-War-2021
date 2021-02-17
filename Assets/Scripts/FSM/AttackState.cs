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
            if (_leaderFlags.inCooldown == false)
            {
                _leader.Attack();
                _leaderFlags.inCooldown = true;
            }
            else
            {
                _leader.currentCooldown += Time.deltaTime;
                if (_leader.currentCooldown >= _leader.cooldown)
                {
                    _leader.currentCooldown = 0;
                    _leaderFlags.inCooldown = false;
                }
            }
        }
        
        if (_follower)
        {
            _follower.mesh.material = _follower.defaultMaterial;
            _follower.transform.LookAt(_follower.GetClosestEnemy());
            if (_followerFlags.inCooldown == false)
            {
                _follower.Attack();
                _followerFlags.inCooldown = true;
            }
            else
            {
                _follower.currentCooldown += Time.deltaTime;
                if (_follower.currentCooldown >= _follower.cooldown)
                {
                    _follower.currentCooldown = 0;
                    _followerFlags.inCooldown = false;
                }
            }
        }
        _treeStart.Execute();
    }
}
