using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    List<Vector3> linePoints;
    float timer;
    public float timerDelay;

    GameObject newLine;
    LineRenderer lineRenderer;
    public float width;

    // Colors
    public ColorPickerTriangle CP;

    // Start is called before the first frame update
    void Start()
    {
        linePoints = new List<Vector3>();
        timer = timerDelay;
    }

    // Update is called once per frame
    void Update()
    {
        // returns true only on the first frame during which the mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            newLine = new GameObject();
            lineRenderer = newLine.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

            Color c = ColorManager.Instance.GetColor();
            lineRenderer.material.color = c;

            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }

        // returns true always if the mouse button is being pressed
        if (Input.GetMouseButton(0))
        {
            Debug.DrawRay(Camera.main.ScreenToWorldPoint(Input.mousePosition), GetMousePosition(), Color.red);
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                linePoints.Add(GetMousePosition());  // the end of the ray
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

    Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return ray.origin + ray.direction * 10;
    }
}
