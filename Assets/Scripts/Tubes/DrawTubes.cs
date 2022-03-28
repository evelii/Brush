using System.Collections.Generic;
using UnityEngine;

public enum StrokeState
{
    WAITING,
    START_STROKE,
    DRAW
}

public class DrawTubes : MonoBehaviour
{
    public Transform cursor;

    public Color strokeColor;
    public ColorPickerTriangle CP;

    [Range(0.1f, 1f)]
    public float strokeRadius;

    [Range(0.01f, 0.1f)]
    public float updateLineInterval;

    public StrokeState state;
    //public bool canDraw;
    private TubeStroke _currentTubeStroke;
    public List<List<Vector3>> strokesList;

    Vector3 lastPos;
    public List<Vector3> fullPoints; // all points drawn by the controller

    public AddAnimation addAnimation;

    public void Start()
    { 
        state = StrokeState.WAITING;
        //canDraw = true;
        strokesList = new List<List<Vector3>>();
        fullPoints = new List<Vector3>();
    }

    public void Update()
    { 
        bool canDraw = GameObject.Find("3DCursor").GetComponent<Cursor>().canDraw;
        if (state == StrokeState.DRAW && !canDraw)
        {
            if (_currentTubeStroke != null)
            {
                //strokesList.Add(_currentTubeStroke.getStrokePoints());
                if(fullPoints.Count > 0)
                {
                    strokesList.Add(new List<Vector3>(fullPoints));
                    fullPoints.Clear();
                }
            }

            GameObject clientObject = GameObject.Find("TCPClient");
            TCPClient client = (TCPClient)clientObject.GetComponent(typeof(TCPClient));
            client.strokesList = strokesList;
        }

        // when a new dragging on the cursor, create a new tube
        if (Input.GetMouseButton(0) && canDraw)
        {
            state = StrokeState.START_STROKE;
            _currentTubeStroke = null;

        }

        if (state == StrokeState.WAITING)
        {
            if (canDraw) state = StrokeState.START_STROKE;
            _currentTubeStroke = null;
        }

        if (state == StrokeState.START_STROKE)
        {
            _createNewTube();
            state = StrokeState.DRAW;
            _currentTubeStroke.AddPoint(cursor.position);
            lastPos = cursor.position;
        }

        if (state == StrokeState.DRAW)
        {
            if (lastPos != cursor.position)
            {
                fullPoints.Add(cursor.position);
            }
            lastPos = cursor.position;
            if (!canDraw) state = StrokeState.WAITING;
        }
    }

    private void _createNewTube()
    {
        GameObject newStroke = new GameObject("NewStroke");
        newStroke.AddComponent<SketchedObject>();
        newStroke.transform.position = cursor.transform.position;

        GameObject go = new GameObject("TubeStroke");
        go.transform.parent = newStroke.transform;

        if(addAnimation.insertKeyframe)
        {
            addAnimation.keyframeObject = newStroke;
            addAnimation.keyframePos = addAnimation.animatedObject.transform.position;
        } else
        {
            addAnimation._parentObject = newStroke;
            addAnimation.animatedObject = newStroke;
            addAnimation._currentObject = go;
        }
        
        _currentTubeStroke = go.AddComponent<TubeStroke>();
        TubeRenderer tube = go.AddComponent<TubeRenderer>();
        tube.MarkDynamic();
        go.GetComponent<MeshRenderer>().material.color = ColorManager.Instance.GetColor();
        tube.radius = strokeRadius;

        SketchManager.manager.sketchObjects.Add(newStroke);
    }

    public void FixedUpdate()
    {
        if (state == StrokeState.DRAW)
        {
            if (_currentTubeStroke.StrokesCount > 0 && _currentTubeStroke.LastStrokePoint == cursor.position)
            {
                return;
            }
            AddPoint(cursor.position);
        }
    }

    private Vector3 _lastPoint;
    private bool canAdd = true;
    void AddPoint(Vector3 point)
    {
        if (canAdd && state == StrokeState.DRAW)
        {
            canAdd = false;

            if (_currentTubeStroke.CanUpdate())
            {
                _lastPoint = _currentTubeStroke.LastStrokePoint;
            }
            else
            {
                Debug.LogWarning("Too Many Points Exception. Creating a new tube.");
                _createNewTube();
                _currentTubeStroke.AddPoint(_lastPoint);
                state = StrokeState.DRAW;
            }
            _currentTubeStroke.AddPoint(point);
            _currentTubeStroke.UpdateAnchorPoints();

            Invoke("invoke", updateLineInterval);
        }
        
    }

    void invoke()
    {
        canAdd = true;
    }
}
