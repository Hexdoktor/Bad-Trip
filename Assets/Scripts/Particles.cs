using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    public float lifetime; // time to remove in seconds

    void Start()
    {
        if(lifetime == 0)
            lifetime = 1;
        Destroy(gameObject, lifetime);
    }
}
