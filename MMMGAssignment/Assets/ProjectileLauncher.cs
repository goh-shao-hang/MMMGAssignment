using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject projectile;

    public Transform spawnlocation;

    public Quaternion spawnRotation;

    public float spawnTime = 16f;

    private float timeSinceSpawned = 0f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        timeSinceSpawned += Time.deltaTime;

        if(timeSinceSpawned >= spawnTime)
        {
            Instantiate(projectile, spawnlocation.position, spawnRotation);
            timeSinceSpawned = 0;
        }
    }
}
