using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public Transform pathSetter; // a path cursor user used to defince the movement path
    private LineRenderer currLine;
    private Vector3 lastPos, curPos;
    public int numClicks = 0;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
        currLine = gameObject.AddComponent<LineRenderer>();
        currLine.startWidth = .1f;
        currLine.endWidth = .1f;
        numClicks = 0;
    }

    // Update is called once per frame
    void Update()
    {
        curPos = pathSetter.transform.position;

        if (curPos != lastPos)
        {  // when the controller is held
            currLine.positionCount = numClicks + 1;
            currLine.SetPosition(numClicks, curPos);
            numClicks++;
            lastPos = curPos;
        }

    }

    public Vector3[] getPathPoints()
    {
        Vector3[] pos = new Vector3[currLine.positionCount];
        currLine.GetPositions(pos);
        return pos;
    }
}
