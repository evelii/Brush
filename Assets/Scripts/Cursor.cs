using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public bool canDraw;
    Vector3 lastpos;

    private void Start()
    {
        canDraw = true;
        lastpos = gameObject.transform.position;
    }

    private void Update()
    {
        if(lastpos != gameObject.transform.position)
        {
            //Debug.Log(gameObject.transform.position.ToString("F4"));
        }
        lastpos = gameObject.transform.position;
    }
}
