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
    int lineCount;

    public SketchEntity editingSketch; // the object to be animated

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
        if (SketchManager.curEditingObject != null) editingSketch = SketchManager.curEditingObject;

        if (active)
        {
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

                editingSketch.AddSoundMark(newLine);
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

        // turn on the self sound
        if (editingSketch && editingSketch.InEditingMode() && editingSketch.SoundStartNotMarked()
            && editingSketch.GetSoundMarkCount() >= 4)
        {
            //Debug.Log("4!!");
            editingSketch.MarkSelfSoundStartPoint();
        }
    }

    Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return ray.origin + ray.direction * 10;
    }

    public void ResetBrush()
    {
        
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

        float initSlope = Slope(secondPoint, firstPoint);
        bool isVerticalLine = CanBeVerticalLine(initSlope);

        for (int i = startIndex; i < points.Count; i++)
        {
            float curSlope = Slope(points[i], firstPoint);
            if (Mathf.Abs(curSlope - initSlope) > 0.4f)
            {
                isLine = false;
                //break;
            }
            if (!CanBeVerticalLine(curSlope)) isVerticalLine = false;
        }

        if (isLine) return StrokeType.Line;
        if (isVerticalLine) return StrokeType.Line;

        return StrokeType.Unknown;
    }

    bool CanBeVerticalLine(float s)
    {
        if (s == 0f || Mathf.Abs(s) >= 6f) return true;
        return false;
    }

    float Slope(Vector3 p1, Vector3 p2)
    {
        if (p2.x - p1.x == 0) return 0;
        return (p2.y - p1.y) / (p2.x - p1.x);
    }
}
