using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    public ColorPickerTriangle CP;
    private LineRenderer currLine;
    private Vector3 lastPos, curPos;

    public int numClicks = 0;

    void Start()
    {
        lastPos = transform.position;
        currLine = this.gameObject.AddComponent<LineRenderer>();
        currLine.startWidth = .1f;
        currLine.endWidth = .1f;

        numClicks = 0;
    }


    void Update()
    {
        curPos = transform.position;

        if (curPos != lastPos) {  // when the controller is held
            currLine.positionCount = numClicks + 1;
            currLine.SetPosition(numClicks, curPos);
            numClicks++;
        }

        currLine.material.color = ColorManager.Instance.GetColor();
    

    }

    
}
