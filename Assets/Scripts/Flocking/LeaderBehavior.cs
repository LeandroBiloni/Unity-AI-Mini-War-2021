using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBehavior : MonoBehaviour, IFlockBehavior
{
    public float leaderWeight;
    public Transform target;

    public Vector3 GetDir(List<IFlockEntity> entities, IFlockEntity entity)
    {
        return (target.position - entity.Position).normalized * leaderWeight;
    }

}
