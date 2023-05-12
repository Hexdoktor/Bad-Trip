using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missiles : MonoBehaviour
{
    public float damage;
    public float moveSpeed;
    private float angle;
    public float lifetime;
    Vector3 curpos;
    Vector3 targetpos;

    public void SetAngle(float ang)
    {
        angle = ang;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(damage == 0)
            damage = 10f;
        if(moveSpeed == 0)
            moveSpeed = 10f;
        if(lifetime == 0)
            lifetime = 3;
        transform.Rotate(0, 0, angle);
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-moveSpeed * Time.deltaTime, 0, 0);
    }

    void Explode()
    {
        // spawn some particle effects here?
        Destroy(gameObject);
    }

    // hit something, do damage and explode
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Enemy")
            return;
        if(col.gameObject.tag == "Player")
            col.gameObject.SendMessage("TakeDamage", damage);
        Explode();
    }
}
