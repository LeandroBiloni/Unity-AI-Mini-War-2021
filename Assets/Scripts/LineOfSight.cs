using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
	public float attackRange;
	public float angle;
	public LayerMask mask;
	public bool IsInSight(Transform target)
	{
		if (target == null)
			return false;
		Vector3 dis = target.position - transform.position;
		float distance = dis.magnitude;

		if (distance > attackRange) return false;
		if (Vector3.Angle(transform.forward, dis) > angle / 2) return false;
		if (Physics.Raycast(transform.position, dis.normalized, distance, mask)) return false;

		return true;
	}

	
    private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(transform.position, transform.forward * attackRange);
		Gizmos.DrawWireSphere(transform.position, attackRange);
		Gizmos.DrawRay(transform.position, Quaternion.Euler(0, angle / 2, 0) * transform.forward * attackRange);
		Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -angle / 2, 0) * transform.forward * attackRange);
	}
}
