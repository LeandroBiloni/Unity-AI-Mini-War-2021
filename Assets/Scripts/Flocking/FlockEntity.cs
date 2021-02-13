using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockEntity : MonoBehaviour, ISteering, IFlockEntity
{
    public LayerMask maskEntity;
    public List<IFlockBehavior> _behaviors = new List<IFlockBehavior>();
    public float radius;
    Vector3 _dir = Vector3.zero;
    private void Awake()
    {
        IFlockBehavior[] behaviors = GetComponents<IFlockBehavior>();
        _behaviors = new List<IFlockBehavior>(behaviors);
    }
    public Vector3 Direction => _dir;

    public Vector3 Position => transform.position;

    public Vector3 GetDir()
    {
        Collider[] objs = Physics.OverlapSphere(transform.position, radius, maskEntity);
        List<IFlockEntity> entities = new List<IFlockEntity>();
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].transform.position == transform.position) continue;
            if (objs[i].GetComponent<Follower>().selectedTeam != this.gameObject.GetComponent<Follower>().selectedTeam) continue;
            var currEntity = objs[i].GetComponent<IFlockEntity>();
            if (currEntity != null)
                entities.Add(currEntity);
        }
        _dir = Vector3.zero;
        for (int i = 0; i < _behaviors.Count; i++)
        {
            _dir += _behaviors[i].GetDir(entities, this);
        }
        return _dir.normalized;
    }
}
