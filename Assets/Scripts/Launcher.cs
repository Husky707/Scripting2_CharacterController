using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{

    [SerializeField] GameObject obj = null;
    [SerializeField] float launchTimer = 5f;

    float launchTic = 0f;
    void Update()
    {
        launchTic += Time.deltaTime;
        if(launchTic >= launchTimer)
        {
            launchTic = 0f;
            Launch();
        }
    }


    private void Launch()
    {
        Instantiate(obj, gameObject.transform.position, Quaternion.identity);
    }
}
