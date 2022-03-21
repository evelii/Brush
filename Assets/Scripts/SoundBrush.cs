using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBrush : MonoBehaviour
{
    public bool active;

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
        if(active)
        {
            if (lineCount == 3)
            {
            }

            // returns true only on the first frame during which the mouse button is clicked
            if (Input.GetMouseButtonDown(0))
            {
                newLine = new GameObject();
                lineRenderer = newLine.AddComponent<LineRenderer>();
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

                Color c = ColorManager.Instance.GetColor();
                c = Color.red;
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
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                StrokeType strokeType = recognizeStroke(linePoints);
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
    }

    Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return ray.origin + ray.direction * 10;
    }

    public void resetBrush()
    {
        lineCount = 0;
        verticalLineCount = 0;
    }

    /// <summary>
    /// Method which takes a list of points for a stroke and returns the stroke type
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    StrokeType recognizeStroke(List<Vector3> points)
    {
        // guarantee there are at least two points in the list
        if (points.Count < 2) return StrokeType.Unknown;

        bool isLine = true;

        Vector3 firstPoint = points[0];
        Vector3 secondPoint = points[0];
        int startIndex = 0;

        for (int i = 0; i < points.Count; i++)
        {
            if (!points[i].Equals(firstPoint))
            {
                secondPoint = points[i + 1]; // try excluding some points which are at beginning
                startIndex = i + 1;
                break;
            }
        }

        if (startIndex == 0) return StrokeType.Point; // all positions are the same

        float initSlope = slope(secondPoint, firstPoint);
        bool isVerticalLine = canBeVerticalLine(initSlope);

        for (int i = startIndex; i < points.Count; i++)
        {
            float curSlope = slope(points[i], firstPoint);
            //Debug.LogWarning(initSlope + ", " + curSlope);
            if (Mathf.Abs(curSlope - initSlope) > 0.4f)
            {
                isLine = false;
                //break;
            }
            if (!canBeVerticalLine(curSlope)) isVerticalLine = false;
        }

        if (isLine) return StrokeType.Line;
        if (isVerticalLine) return StrokeType.VerticalLine;

        return StrokeType.Unknown;
    }

    bool canBeVerticalLine(float s)
    {
        if (s == 0f || Mathf.Abs(s) >= 6f) return true;
        return false;
    }

    float slope(Vector3 p1, Vector3 p2)
    {
        if (p2.x - p1.x == 0) return 0;
        return (p2.y - p1.y) / (p2.x - p1.x);
    }
}