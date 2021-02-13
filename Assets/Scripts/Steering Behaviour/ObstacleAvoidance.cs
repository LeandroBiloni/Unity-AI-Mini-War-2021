using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : ISteering
{
    Transform _from;
    float _radius;
    LayerMask _mask;
    public float _avoidWeight;
    public ObstacleAvoidance(Transform from, float radius, float avoidWeight, LayerMask mask)
    {
        _avoidWeight = avoidWeight;
        _radius = radius;
        _mask = mask;
        _from = from;
    }
    public Vector3 GetDir()
    {
        Vector3 dir = Vector3.zero;

        Collider[] obstacles = Physics.OverlapSphere(_from.position, _radius, _mask);
        if (obstacles.Length > 0)
        {
            float distance = Vector3.Distance(obstacles[0].transform.position, _from.position);
            int indexSave = 0;
            for (int i = 1; i < obstacles.Length; i++)
            {
                float currDistance = Vector3.Distance(obstacles[i].transform.position, _from.position);
                if (currDistance < distance)
                {
                    distance = currDistance;
                    indexSave = i;
                }
            }
            if (distance >= _radius)
                distance = _radius - 0.1f;
            Vector3 dirFromObs = (_from.position - obstacles[indexSave].transform.position).normalized * ((_radius - distance) / _radius) * _avoidWeight;
            dir += dirFromObs;
        }
        return dir.normalized;
    }
}
