using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platform_drop : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(waiter());
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(15);
        Object.Destroy(this.gameObject);
    }
}
