using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PathSetState
{
    WAITING,
    START_SETTING,
    DRAW
}

public class PathFollower : MonoBehaviour
{
    public PathSetState state;
    public Transform pathSetter; // a path cursor user used to defince the movement path
    private LineRenderer _currLine;
    private Vector3 lastPos, curPos;
    public int numClicks = 0;
    public bool canDraw = true;

    // Start is called before the first frame update
    void Start()
    {
        state = PathSetState.WAITING;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == PathSetState.WAITING)
        {
            if (canDraw) state = PathSetState.START_SETTING;
            _currLine = null;
        }

        if (state == PathSetState.START_SETTING)
        {
            _createNewPath();
            state = PathSetState.DRAW;
        }

        if (state == PathSetState.DRAW)
        {
            if (!canDraw) state = PathSetState.WAITING;
        }
    }

    private void _createNewPath()
    {
        GameObject newPath = new GameObject("New Path");
        lastPos = transform.position;
        _currLine = newPath.AddComponent<LineRenderer>();
        _currLine.startWidth = .1f;
        _currLine.endWidth = .1f;
        numClicks = 0;
    }

    public void FixedUpdate()
    {
        if (state == PathSetState.DRAW)
        {
            curPos = pathSetter.transform.position;

            if (curPos != lastPos)
            {  // when the controller is held
                _currLine.positionCount = numClicks + 1;
                _currLine.SetPosition(numClicks, curPos);
                numClicks++;
                lastPos = curPos;
            }
        }
    }

    public Vector3[] getPathPoints()
    {
        Vector3[] pos = new Vector3[_currLine.positionCount];
        _currLine.GetPositions(pos);
        return pos;
    }
}
