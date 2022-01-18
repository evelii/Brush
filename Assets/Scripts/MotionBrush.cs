using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBrush : MonoBehaviour
{
    List<Vector3> linePoints;
    float timer;
    public float timerDelay;

    GameObject newLine;
    List<GameObject> lineCollection;
    LineRenderer lineRenderer;
    public float width;

    public GameObject animatedObject; // the object to be animated
    int count = 0;
    string moveDirection = "";

    // Colors
    public ColorPickerTriangle CP;

    // Start is called before the first frame update
    void Start()
    {
        linePoints = new List<Vector3>();
        timer = timerDelay;
        lineCollection = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(count == 3)
        {
            if (moveDirection == "right")
            {
                if (animatedObject.transform.position.x < 7)
                    animatedObject.transform.Translate(Vector3.right * 9 * Time.deltaTime);
                else
                {
                    count = 0;
                    moveDirection = "";
                }
            }

            else if (moveDirection == "left") {
                if (animatedObject.transform.position.x > -7)
                {
                    animatedObject.transform.Translate(Vector3.left * 9 * Time.deltaTime);
                }
                    
                else
                {
                    count = 0;
                    moveDirection = "";
                }
            }

            foreach (GameObject l in lineCollection)
            {
                l.SetActive(false);
            }
        }

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

                if(moveDirection=="")
                {
                    if (mousePos.x < animatedObject.transform.position.x) moveDirection = "right";
                    else moveDirection = "left";
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            count++;
            linePoints.Clear();
        }
    }

    Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return ray.origin + ray.direction * 10;
    }
}
