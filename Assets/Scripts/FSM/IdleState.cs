using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState<T> : States<T>
{
    Leader _leader;
    LeaderFlags _leaderFlags;

    Follower _follower;
    FollowerFlags _followerFlags;
    INode _treeStart;
    public IdleState(Leader leader, LeaderFlags leaderFlags, INode treeStart)
    {
        _leader = leader;
        _leaderFlags = leaderFlags;
        _treeStart = treeStart;
    }

    public IdleState(Follower follower, FollowerFlags followerFlags, INode treeStart)
    {
        _follower = follower;
        _followerFlags = followerFlags;
        _treeStart = treeStart;
    }

    public override void Execute()
    {
        if (_leader)
        {
            _leader.Idle();

            if (_leader.currentHP <= _leader.currentHP / 4)
                _leaderFlags.lowHP = true;
            else
            {
                _leaderFlags.lowHP = false;
                Vector3 enemyTeamCenter = _leader.EnemyTeamCenter();
                if ((enemyTeamCenter - _leader.transform.position).magnitude > _leader.leadStopDistance)
                    _leaderFlags.inMiddle = false;
            }
        }

        if (_follower)
        {
            _follower.Idle();

            if (_follower.currentHP <= _follower.currentHP / 4)
                _followerFlags.lowHP = true;
            else
            {
                _followerFlags.lowHP = false;
                if (_follower.DistanceToLeader() <= _follower.stopDist)
                    _followerFlags.isCloseToLeader = true;

                else _followerFlags.isCloseToLeader = false;
            }
        }
        _treeStart.Execute();
    }
}
