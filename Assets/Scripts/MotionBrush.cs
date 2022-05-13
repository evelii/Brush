using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBrush : MonoBehaviour
{
    public bool active; // motion brush is activated

    List<Vector3> linePoints;
    float timer;
    public float timerDelay;

    GameObject newLine;
    List<GameObject> lineCollection;
    LineRenderer lineRenderer;
    public float width;

    public GameObject animatedObject; // the object to be animated
    int lineCount = 0; // horizontal
    int verticalLineCount = 0;
    public string moveDirection = "";

    // AddAnimation script
    public AddAnimation addAnimation;

    // Colors
    public ColorPickerTriangle CP;

    // Stroke Recognizer
    enum StrokeType
    {
        Line, // horizontal, diagonal
        VerticalLine,
        Point,
        Curve,
        Unknown
    }

    // Start is called before the first frame update
    void Start()
    {
        linePoints = new List<Vector3>();
        timer = timerDelay;
        lineCollection = new List<GameObject>();
        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(SketchManager.curEditingObject != null) animatedObject = SketchManager.curEditingObject.gameObject;

        if (lineCount == 3)
        {
            addAnimation.movement = true;

            // hide the motion lines from the display
            foreach (GameObject l in lineCollection)
            {
                l.SetActive(false);
            }
            ResetBrush();
        }

        else if(verticalLineCount == 3)
        {
            // hide the motion lines from the display
            foreach (GameObject l in lineCollection)
            {
                l.SetActive(false);
            }
            ResetBrush();
        }

        if(active)
        {
            // returns true only on the first frame during which the mouse button is clicked
            if (Input.GetMouseButtonDown(0))
            {
                newLine = new GameObject();
                lineRenderer = newLine.AddComponent<LineRenderer>();
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

                Color c = ColorManager.Instance.GetColor();
                c = Color.cyan;
                lineRenderer.material.color = c;

                lineRenderer.startWidth = width;
                lineRenderer.endWidth = width;

                lineCollection.Add(newLine);
            }

            // returns true always if the mouse button is being pressed
            if (Input.GetMouseButton(0))
            {
                //Debug.DrawRay(Camera.main.ScreenToWorldPoint(Input.mousePosition), GetMousePosition(), Color.red);
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    Vector3 mousePos = GetMousePosition();
                    linePoints.Add(mousePos);  // the end of the ray
                    lineRenderer.positionCount = linePoints.Count;
                    lineRenderer.SetPositions(linePoints.ToArray());
                    timer = timerDelay;

                    if (moveDirection == "")
                    {
                        if (mousePos.x < animatedObject.transform.position.x) moveDirection = "right";
                        else moveDirection = "left";
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                StrokeType strokeType = RecognizeStroke(linePoints);
                if (strokeType == StrokeType.Line)
                {
                    lineCount++;
                }
                else if (strokeType == StrokeType.VerticalLine)
                {
                    verticalLineCount++;
                }

                linePoints.Clear();
            }
        }

        else if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse is down");

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                Debug.Log("Hit " + hitInfo.transform.gameObject.name);
            }
            else
            {
                Debug.Log("No hit");
            }
        }
    }

    Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return ray.origin + ray.direction * 10;
    }

    public void ResetBrush()
    {
        lineCount = 0;
        verticalLineCount = 0;
        moveDirection = "";
    }

    /// <summary>
    /// Method which takes a list of points for a stroke and returns the stroke type
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    StrokeType RecognizeStroke(List<Vector3> points)
    {
        // guarantee there are at least two points in the list
        if (points.Count < 2) return StrokeType.Unknown;

        bool isLine = true;

        Vector3 firstPoint = points[0];
        Vector3 secondPoint = points[0];
        int startIndex = 0;

        for(int i = 0; i < points.Count; i++)
        {
            if(!points[i].Equals(firstPoint))
            {
                secondPoint = points[i+1]; // try excluding some points which are at beginning
                startIndex = i+1;
                break;
            }
        }

        if (startIndex == 0) return StrokeType.Point; // all positions are the same

        float initSlope = Slope(secondPoint, firstPoint);
        bool isVerticalLine = CanBeVerticalLine(initSlope);

        for (int i = startIndex; i < points.Count; i++)
        {
            float curSlope = Slope(points[i], firstPoint);
            //Debug.LogWarning(initSlope + ", " + curSlope);
            if (Mathf.Abs(curSlope - initSlope) > 0.4f)
            {
                isLine = false;
                //break;
            }
            if (!CanBeVerticalLine(curSlope)) isVerticalLine = false;
        }
        
        if(isLine) return StrokeType.Line;
        if(isVerticalLine) return StrokeType.VerticalLine;

        return StrokeType.Unknown;
    }

    bool CanBeVerticalLine(float s)
    {
        if (s == 0f ||  Mathf.Abs(s) >= 6f) return true;
        return false;
    }

    float Slope(Vector3 p1, Vector3 p2)
    {
        if (p2.x - p1.x == 0) return 0;
        return (p2.y - p1.y) / (p2.x - p1.x);
    }
}
