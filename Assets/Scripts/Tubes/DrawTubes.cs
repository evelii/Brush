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
    public GameObject curSketch;
    public Transform cursor;

    public Color strokeColor;
    public ColorPickerTriangle CP;

    [Range(0.001f, 0.1f)]
    public float strokeRadius;

    [Range(0.01f, 0.1f)]
    public float updateLineInterval;

    public StrokeState state;
    //public bool canDraw;
    private TubeStroke _currentTubeStroke;
    public List<List<Vector3>> strokesList;

    Vector3 lastPos;
    public List<Vector3> fullPoints; // all points drawn by the controller

    public GameObject clientObject;

    GameObject newStroke;
    ControllerCursor cursorScript;

    static public bool buttonOneIsDown;
    static public bool buttonOneIsUp;

    public void Start()
    {
        newStroke = null;
        state = StrokeState.WAITING;
        strokesList = new List<List<Vector3>>();
        fullPoints = new List<Vector3>();
        cursorScript = cursor.gameObject.GetComponent<ControllerCursor>();
    }

    public void Update()
    {
        bool canDraw = cursorScript.canDraw;

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            buttonOneIsDown = true;
            UnityEngine.EventSystems.OVRInputModule.buttonDown = true;
        } else buttonOneIsDown = false;

        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            buttonOneIsUp = true;
            UnityEngine.EventSystems.OVRInputModule.buttonDown = false;
        }
        else buttonOneIsUp = false;

        if (state == StrokeState.DRAW)
        {
            if (lastPos != cursor.position)
            {
                fullPoints.Add(cursor.position);
            }
            lastPos = cursor.position;
            if (!canDraw) state = StrokeState.WAITING;
        }

        //if (OVRInput.GetDown(OVRInput.Button.One)) Debug.LogWarning("detect!");
        //if (OVRInput.Get(OVRInput.Button.One)) Debug.LogWarning("detect get!");

        if (OVRInput.GetDown(OVRInput.Button.One) && canDraw)
        {
            if (state == StrokeState.WAITING)
            {
                state = StrokeState.START_STROKE;
                _createNewTube();
                _currentTubeStroke.AddPoint(cursor.position);
                state = StrokeState.DRAW;
            }
        }
        else if (OVRInput.GetUp(OVRInput.Button.One) && state == StrokeState.DRAW)
        {
            if (_currentTubeStroke)
            {
                _currentTubeStroke = null;
                state = StrokeState.WAITING;

                if (fullPoints.Count > 0)
                {
                    strokesList.Add(new List<Vector3>(fullPoints));
                    fullPoints.Clear();
                }
            }

            TCPClient client = (TCPClient)clientObject.GetComponent(typeof(TCPClient));
            client.strokesList = strokesList;
            client.curObjectForRecognition = curSketch.GetComponent<SketchEntity>();
        }

        OVRInput.Update();
    }

    public void FinishSketch()
    {
        if (SketchManager.curEditingObject != null)
        {
            SketchManager.curEditingObject.SketchWrapUp();
        }
        newStroke = null;
        strokesList.Clear();
    }

    private void _createNewTube()
    {
        if (newStroke == null)
        {
            newStroke = new GameObject("NewStroke");
            newStroke.transform.position = cursor.transform.position;
            curSketch = newStroke;
            newStroke.AddComponent<BoxCollider>();
            SketchEntity se = newStroke.AddComponent<SketchEntity>() as SketchEntity;
            se.initPos = newStroke.transform.position;
            CollisionIgnore ci = newStroke.AddComponent<CollisionIgnore>() as CollisionIgnore;

            // check if the current sketch means to be a dependency of another existing sketch
            if (SketchManager.curSelected != null)
            {
                Dependency dependent = new Dependency(curSketch.GetComponent<SketchEntity>(), newStroke.transform.position, SketchManager.curSelected.gameObject);
                SketchManager.curSelected.AddDependency(dependent);
                ci.label = SketchManager.curSelected.gameObject.GetComponent<CollisionIgnore>().label + "_dependency";
                ci.guardian = SketchManager.curSelected.gameObject;
                ci.AvoidCollision();
                se.label = ci.label;
                se.guardian = ci.guardian.GetComponent<SketchEntity>();             
            }
            else
            { 
                ci.label = SketchManager.labelCounter.ToString();
                se.label = ci.label;
                SketchManager.labelCounter++;
            }
            SketchManager._parentObject = curSketch;
            SketchManager.curEditingObject = curSketch.GetComponent<SketchEntity>();
        }

        GameObject go = new GameObject("TubeStroke");
        go.transform.parent = newStroke.transform;
        CollisionIgnore goCI = go.AddComponent<CollisionIgnore>() as CollisionIgnore;
        goCI.label = newStroke.GetComponent<CollisionIgnore>().label + "_child";
        goCI.guardian = go.transform.parent.gameObject.GetComponent<CollisionIgnore>().guardian;
        curSketch = newStroke;
        newStroke.GetComponent<SketchEntity>().AddChildStroke(go);

        _currentTubeStroke = go.AddComponent<TubeStroke>();
        TubeRenderer tube = go.AddComponent<TubeRenderer>();
        tube.MarkDynamic();
        go.GetComponent<MeshRenderer>().material.color = strokeColor;
        tube.radius = strokeRadius;

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
