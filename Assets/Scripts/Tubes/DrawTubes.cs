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

    public AddAnimation addAnimation;

    public void Start()
    { 
        state = StrokeState.WAITING;
        canDraw = true;
        //addAnimation = new AddAnimation();
    }

    public void Update()
    {

        // when a new dragging on the cursor, create a new tube
        if (Input.GetMouseButton(0))
        {
            Debug.Log("clicked");
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

        //if (bounce) addAnimation.bounce = true;
        //if (movement) addAnimation.movement = true;
    }

    private void _createNewTube()
    {
        gameObject.transform.position = cursor.transform.position;
        GameObject go = new GameObject("TubeStroke");
        go.transform.parent = transform;
        addAnimation._currentObject = go;
        //_squashParent = new GameObject(string.Format("_squash_{0}", name)).transform;
        //_originalScale = _currentObject.transform.localScale;
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


    //void Update()
    //{
        

    //    // returns true always if the mouse button is being pressed
    //    if (Input.GetMouseButton(0))
    //    {
    //        // Debug.DrawRay(Camera.main.ScreenToWorldPoint(Input.mousePosition), GetMousePosition(), Color.red);
    //        timer -= Time.deltaTime;
    //        if (timer <= 0)
    //        {
    //            linePoints.Add(GetMousePosition());  // the end of the ray
    //            lineRenderer.positionCount = linePoints.Count;
    //            lineRenderer.SetPositions(linePoints.ToArray());
    //            timer = timerDelay;

    //        }
    //    }

    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        linePoints.Clear();
    //    }
    //}

    //Vector3 GetMousePosition()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    return ray.origin + ray.direction * 10;
    //}
}
