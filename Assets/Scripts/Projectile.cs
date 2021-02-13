using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Teams
{
    public float speed;
    public Vector3 dir;
    public float damage;
    public Team selectedTeam;

    private void Start()
    {
        transform.parent = null;
    }
    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.position += dir * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Being being = collision.gameObject.GetComponent<Being>();
        if (being && being.selectedTeam != selectedTeam)
            being.TakeDamage(damage);

        Destroy(gameObject);
    }

}
