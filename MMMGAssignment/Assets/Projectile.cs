using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update

    public float moveSpeed = 5f;

    public float timetolive = 2f;

    private float timeSinceSpawned = 0f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveSpeed * transform.forward * Time.deltaTime;

        timeSinceSpawned += Time.deltaTime;

        if(timeSinceSpawned > timetolive)
        {
            Destroy(gameObject);
        }
    }
}
