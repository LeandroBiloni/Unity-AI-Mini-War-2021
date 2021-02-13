using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeparationBehavior : MonoBehaviour, IFlockBehavior
{
    public float separationWeight;
    public float range;
    public Vector3 GetDir(List<IFlockEntity> entities, IFlockEntity entity)
    {
        Vector3 dir = Vector3.zero;
        for (int i = 0; i < entities.Count; i++)
        {
            var currEntity = entities[i];
            if (Vector3.Distance(currEntity.Position, entity.Position) < range)
            {
                Vector3 currDir = (entity.Position - currEntity.Position);
                float distance = currDir.magnitude;
                currDir = currDir.normalized * (range - distance);
                dir += currDir;
            }
        }
        return dir.normalized * separationWeight;
    }
}
