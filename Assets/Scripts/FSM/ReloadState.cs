using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadState<T> : States<T>
{
    Leader _leader;
    LeaderFlags _leaderFlags;
    Follower _follower;
    FollowerFlags _followerFlags;
    INode _treeStart;

    public ReloadState(Leader leader, LeaderFlags leaderFlags, INode treeStart)
    {
        _leader = leader;
        _leaderFlags = leaderFlags;
        _treeStart = treeStart;
    }

    public ReloadState(Follower follower, FollowerFlags followerFlags, INode treeStart)
    {
        _follower = follower;
        _followerFlags = followerFlags;
        _treeStart = treeStart;
    }
    public override void Execute()
    {
        if (_leader)
        {
            if (_leaderFlags.canShoot == false)
            {
                _leader.currentCooldown += Time.deltaTime;
                if (_leader.currentCooldown >= _leader.cooldown)
                {
                    _leaderFlags.canShoot = true;
                    _leader.currentCooldown = 0;
                }
            }
        }

        if (_follower)
        {
            if (_followerFlags.canShoot == false)
            {
                Debug.Log("reloading");
                _follower.currentCooldown += Time.deltaTime;
                if (_follower.currentCooldown >= _follower.cooldown)
                {
                    Debug.Log("finished reloading");
                    _followerFlags.canShoot = true;
                    _follower.currentCooldown = 0;
                }
            }
        }

        _treeStart.Execute();
    }
}
