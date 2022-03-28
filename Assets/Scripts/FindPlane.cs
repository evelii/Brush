using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPlane : MonoBehaviour
{
    public DrawTubes tubes;
    public List<Vector3> points;
    public bool getBestFit = false;
    private bool drawPlane = false;
    private bool flipHorizontally = false;

    // Best Fitting Plane
    public Vector3 normal;
    public Plane pl;
    public Plane plRotated;
    public List<Vector3> translatedPoints = new List<Vector3>();
    public float dist;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (getBestFit)
        {
            List<List<Vector3>> strokes = tubes.strokesList;
            if (strokes == null) return;
            foreach (List<Vector3> stroke in strokes)
            {
                points.AddRange(stroke);
            }
            compute(points.ToArray());
            plRotated = pl;
            drawPlane = true;
            getBestFit = false;
            rotatePlaneToParallelToXY();
        }

        if (drawPlane)
        {
            //Debug.LogWarning(pl.distance);
            DrawPlane(Vector3.zero - pl.distance * pl.normal, pl.normal, Color.green);
            DrawPlane(Vector3.zero - plRotated.distance * plRotated.normal, plRotated.normal, Color.magenta);
        }
    }


    public string getTranslatedPoints()
    {
        List<List<Vector3>> strokes = tubes.strokesList;
        if (strokes == null) return "";
        foreach (List<Vector3> stroke in strokes)
        {
            points.AddRange(populatePoints(stroke));
        }
        compute(points.ToArray());
        plRotated = pl;
        drawPlane = true;
        getBestFit = false;
        List<Vector2> mappedPoints = rotatePlaneToParallelToXY();

        string res = "";
        for (int i = 0; i < mappedPoints.Count; i++)
        {
            res += mappedPoints[i].ToString();
            if (i != mappedPoints.Count - 1) res += ",";

        }

        if (flipHorizontally) res += "Y";
        else res += "N";

        return res;
    }

    List<Vector3> populatePoints(List<Vector3> originalPoints)
    {
        List<Vector3> populated = new List<Vector3>();
        for(int i = 0; i < originalPoints.Count - 1; i++)
        {
            // mid point
            Vector3 pointA = originalPoints[i];
            Vector3 pointB = originalPoints[i + 1];
            Vector3 midpointB = new Vector3((pointA.x + pointB.x) / 2, (pointA.y + pointB.y) / 2, (pointA.z + pointB.z) / 2);

            // mid mid point
            Vector2 midpointA = new Vector3((pointA.x + midpointB.x) / 2, (pointA.y + midpointB.y) / 2, (pointA.z + midpointB.z) / 2);
            Vector2 midpointC = new Vector3((midpointB.x + pointB.x) / 2, (midpointB.y + pointB.y) / 2, (midpointB.z + pointB.z) / 2);

            populated.Add(pointA);
            populated.Add(midpointA);
            populated.Add(midpointB);
            populated.Add(midpointC);
        }
        populated.Add(originalPoints[originalPoints.Count - 1]);
        return populated;
    }

    List<Vector2> rotatePlaneToParallelToXY()
    {
        Vector3 xy = new Vector3(0, 0, 1);
        //float angle = Vector3.Angle(pl.normal, xy);
        //plRotated.normal = Quaternion.Euler(0, angle, 0) * pl.normal;
        //Debug.LogWarning(pl.normal);

        Quaternion rotation = Quaternion.FromToRotation(pl.normal, xy);
        Vector3 indegree = rotation.eulerAngles;
        if (indegree.y == 180.0f) flipHorizontally = true;

        plRotated.normal = rotation * pl.normal;
        
        //Debug.LogWarning(plRotated.normal); // should be a multiple of (0, 0, 1)
        for(int i = 0; i < translatedPoints.Count; i++)
        {
            translatedPoints[i] = rotation * translatedPoints[i];
        }
        List<Vector2> xyPoint = new List<Vector2>();
        foreach(Vector3 point in translatedPoints)
        {
            xyPoint.Add(point);
        }

        printPoints(xyPoint);

        return xyPoint;
    }

    void DrawPlane(Vector3 position, Vector3 normal, Color color)
    {

        Vector3 v3;

        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;

        v3 *= 10.0f;

        var corner0 = position + v3;
        var corner2 = position - v3;
        var q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        var corner1 = position + v3;
        var corner3 = position - v3;
        Debug.DrawLine(corner0, corner2, color);
        Debug.DrawLine(corner1, corner3, color);
        Debug.DrawLine(corner0, corner1, color);
        Debug.DrawLine(corner1, corner2, color);
        Debug.DrawLine(corner2, corner3, color);
        Debug.DrawLine(corner3, corner0, color);
        Debug.DrawRay(position, normal, Color.red);
    }

    void printPoints(List<Vector2> points)
    {
        string output = "";
        for (int i = 0; i < points.Count; i++)
        {
            output += points[i].ToString();
            if (i != points.Count - 1) output += ",";

        }
        output = output.Replace(" ", "");
        //Debug.LogError(output);
    }

    private void computePlane(Vector3[] points)
    {
        normal = computeNormal(points, points.Length);
        dist = computeDistance(points, normal);
        pl = new Plane(normal, dist);
    }

    private float computeDistance(Vector3[] points, Vector3 normal)
    {
        Vector3 sum = new Vector3(0, 0, 0);
        int n = points.Length;

        foreach (Vector3 p in points)
        {
            sum += p;
        }

        float d = -((Vector3.Dot(sum, normal)) / n);

        return d;
    }

    private Vector3 computeNormal(Vector3[] v, int n)
    {
        // Zero out sum
        Vector3 result = Vector3.zero;

        // Start with the ‘‘ previous’’ vertex as the last one.
        // This avoids an if−statement in the loop
        Vector3 p = v[n - 1];

        // Iterate through the vertices
        for (int i = 0; i < n; ++i)
        {

            // Get shortcut to the ‘‘current’’ vertex
            Vector3 c = v[i];

            // Add in edge vector products appropriately
            result.x += (p.z + c.z) * (p.y - c.y);
            result.y += (p.x + c.x) * (p.z - c.z);
            result.z += (p.y + c.y) * (p.x - c.x);

            // Next vertex, please
            p = c;
        }

        float d = 1.0f;

        // Normalize the result and return it
        result.Normalize();
        return result;
    }

    private List<Vector3> computeTranslatedPoints(List<Vector3> pnt)
    {
        List<Vector3> transP = new List<Vector3>();
        float scalarDist;
        Vector3 aux;

        foreach (Vector3 p in pnt)
        {
            scalarDist = pl.GetDistanceToPoint(p);
            aux = p - scalarDist * normal;
            if (!transP.Contains(aux))
            {
                transP.Add(aux);
            }
        }

        return transP;
    }

    public void compute(Vector3[] pnt)
    {
        List<Vector3> points = new List<Vector3>();
        List<Vector3> pointsAux = new List<Vector3>();
        Vector3 aux;

        for (int i = 0; i < pnt.Length; i++)
        {
            if (!points.Contains(pnt[i]))
            {
                points.Add(pnt[i]);
            }
        }

        for (int i = 0; i < pnt.Length; i++)
        {
            pointsAux.Add(pnt[i]);
        }

        computePlane(points.ToArray());
        translatedPoints = computeTranslatedPoints(pointsAux);
    }
}
