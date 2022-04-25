using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public bool canDraw;
    public bool newSketch;
    Vector3 lastpos;

    private void Start()
    {
        canDraw = true;
        newSketch = false;
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

    public void TurnOffNewSketch()
    {
        newSketch = false;
    }
}
