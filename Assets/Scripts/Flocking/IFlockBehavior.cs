using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFlockBehavior
{
    Vector3 GetDir(List<IFlockEntity> entities, IFlockEntity entity);
}
