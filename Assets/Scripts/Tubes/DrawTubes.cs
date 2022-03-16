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
    public bool canDraw;
    private TubeStroke _currentTubeStroke;
    public List<List<Vector3>> strokesList;

    public AddAnimation addAnimation;

    public void Start()
    { 
        state = StrokeState.WAITING;
        canDraw = true;
        strokesList = new List<List<Vector3>>();
    }

    public void Update()
    {
        if(state == StrokeState.DRAW && !canDraw)
        {
            if (_currentTubeStroke != null)
            {
                strokesList.Add(_currentTubeStroke.getStrokePoints());
            }
            string strokeStr = "";
            foreach(List<Vector3> strokes in strokesList)
            {
                for(int i = 0; i < strokes.Count; i++)
                {
                    Vector2 v2 = strokes[i];
                    strokeStr += v2.ToString();
                    if(i != strokes.Count-1) strokeStr += ",";
                    
                }
                strokeStr = strokeStr.Replace(" ", "");
                Debug.LogError(strokeStr);
            }
            
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
        }

        if (state == StrokeState.DRAW)
        {
            if (!canDraw) state = StrokeState.WAITING;
        }
    }

    private void _createNewTube()
    {
        // save the previous strokes first for sketch recognition
        if(_currentTubeStroke != null)
        {
            strokesList.Add(_currentTubeStroke.getStrokePoints());
        }

        GameObject newStroke = new GameObject("NewStroke");
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
