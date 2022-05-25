using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SketchEntity : MonoBehaviour
{
    public Transform cam;
    public GameObject cursor;
    public CanvasHandler canvas;
    public SoundBrush soundBrush;
    public ControllerMode controllerMode;

    public AudioSource selfSound;
    public AudioSource movingSound;
    public AudioSource collisionHard;
    public AudioSource collisionSoft;
    public string identity; // sketch recognition result
    public string softness; // hard or soft
    public bool aniStart;
    public bool rigidBodyAdded = false;
    public Vector3[] trajectory; // null or a customized path
    public int curIdx = 0;
    static Vector3 currentPosHolder;
    public List<GameObject> childStrokes;
    public PathFollower path;
    GameObject go;

    bool inCollision;
    string rootFolder;
    Dictionary<string, bool> supportedSketches = new Dictionary<string, bool>();
    Vector3 closest;
    Vector3 selfSoundStartPoint;
    bool editingMode;
    List<GameObject> soundMarkCollection;
    public bool bouncyAdded = false;

    public float moveSpeed; // the speed when moving along the path
    public float rotationSpeed;

    // dependencies
    public List<Dependency> dependencies;

    // Start is called before the first frame update
    void Start()
    {
        rootFolder = "SFX";
        selfSound = gameObject.AddComponent<AudioSource>();
        movingSound = gameObject.AddComponent<AudioSource>();
        collisionHard = gameObject.AddComponent<AudioSource>();
        collisionSoft = gameObject.AddComponent<AudioSource>();
        selfSound.playOnAwake = false;
        movingSound.playOnAwake = false;
        collisionHard.playOnAwake = false;
        collisionSoft.playOnAwake = false;
        inCollision = false;
        editingMode = false;
        soundMarkCollection = new List<GameObject>();
        dependencies = new List<Dependency>();
        selfSoundStartPoint = Vector3.zero;
        supportedSketches.Add("ambulance", true);
        supportedSketches.Add("airplane", true);
        supportedSketches.Add("dog", true);
        supportedSketches.Add("police car", true);
        supportedSketches.Add("basketball", true);
        cam = GameObject.Find("CenterEyeAnchor").transform;
        cursor = GameObject.Find("3DCursor");
        canvas = GameObject.Find("Canvas").GetComponent<CanvasHandler>();
        path = GameObject.Find("Path").GetComponent<PathFollower>();
        soundBrush = GameObject.Find("SoundLayer").GetComponent<SoundBrush>();
        controllerMode = GameObject.Find("RightControllerPf").GetComponent<ControllerMode>();
        go = null;

        moveSpeed = 0.7f;
        rotationSpeed = 20f;

        if (gameObject.name != "Floor" && gameObject.name != "Wall" && gameObject.name != "Table" && gameObject.name != "Chair")
        {
            //ObjectIdentity("police car");
        }

    }

    public void ObjectIdentity(string recognitionResult)
    {
        identity = recognitionResult;
        Debug.LogWarning(identity);
        AddSound();
    }

    void AddSound()
    {
        identity = identity.Replace("_", " ");
        if (supportedSketches.ContainsKey(identity))
        {
            selfSound.clip = Resources.Load<AudioClip>(rootFolder + "/" + identity + "/" + "self");
            if (identity == "ambulance" || identity == "police car")
            {
                movingSound.clip = Resources.Load<AudioClip>(rootFolder + "/car/" + "moving");
                collisionHard.clip = Resources.Load<AudioClip>(rootFolder + "/car/" + "hardCollision");
            }
            else
            {
                movingSound.clip = Resources.Load<AudioClip>(rootFolder + "/" + identity + "/" + "moving");
                collisionHard.clip = Resources.Load<AudioClip>(rootFolder + "/" + identity + "/" + "hardCollision");
            }

            if (identity == "ambulance" || identity == "police car" || identity == "airplane") softness = "hard";
            else softness = "soft";

            selfSound.loop = true;
            movingSound.loop = true;
            collisionHard.loop = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.name == "Wall" || gameObject.name == "Floor" || gameObject.name == "Table" || gameObject.name == "Chair") return;


        if (aniStart)
        {
            HideSoundMarks();
            soundBrush.sketchSwitch(false);
            if (go != null) go.SetActive(false);

            // dependencies
            if (dependencies.Count > 1) Debug.LogError("more than one dependencies");
            else if (dependencies.Count > 0)
            {
                if (!dependencies[0].visible) dependencies[0].depSketch.gameObject.SetActive(false);
                else dependencies[0].depSketch.gameObject.SetActive(true);
                float distance = Vector3.Distance(dependencies[0].showupPos, gameObject.transform.position);
                if (distance <= 0.3f) dependencies[0].visible = true;
            }

            if (!rigidBodyAdded)
            {
                gameObject.AddComponent<Rigidbody>();
                rigidBodyAdded = true;
            }

            // Check if there is user defined path
            if (trajectory == null) trajectory = path.GetPathPoints();

            // 1. There is a customized movement path, just follow the path
            if (trajectory != null)
            {
                gameObject.GetComponent<Rigidbody>().useGravity = false;
                MovementInit();
                FollowMovementPath();
            }

            // 2. There is no customized path, use the gravity
            else if (!bouncyAdded) AddBouncingEffect();

            if (!movingSound.isPlaying && !inCollision)
            {
                movingSound.Play();
            }
        }

        if (selfSoundStartPoint != Vector3.zero && !editingMode)
        { 
            float distance = Vector3.Distance(selfSoundStartPoint, gameObject.transform.position);
            if (distance <= 0.2f)
            {
                selfSound.Play();
            }
        }

        // mark along the trajectory
        if (soundBrush.ready && canvas.curBrush == "sound" && OVRInput.GetDown(OVRInput.Button.One) && !soundBrush.isSketchShown())
        {
            Vector3 stampPos = cursor.transform.position;

            if (trajectory == null) trajectory = path.GetPathPoints();
            if (trajectory == null) Debug.LogError("No trajectory is given!");
            else
            {
                float dist = Vector3.Distance(stampPos, trajectory[0]);
                closest = trajectory[0];

                // find the point of the trajectory which is the closest to the stamped point
                for (int i = 1; i < trajectory.Length; i++)
                {
                    if (Vector3.Distance(stampPos, trajectory[i]) < dist)
                    {
                        dist = Vector3.Distance(stampPos, trajectory[i]);
                        closest = trajectory[i];
                    }
                }

                go = GameObject.Instantiate(gameObject);
                go.transform.position = closest;
                Vector3 relativePos = cam.position - go.transform.position;

                // the second argument, upwards, defaults to Vector3.up
                Quaternion rotation = Quaternion.LookRotation(relativePos);
                rotation *= Quaternion.Euler(0, 47, 0);
                go.transform.rotation = rotation;
                //go.transform.LookAt(cam);

                soundBrush.sketchSwitch(true);
            }
        }

        if (canvas.curBrush == "sound" && soundMarkCollection.Count == 4)
        {
            selfSoundStartPoint = closest;
        }

    }

    void MovementInit()
    {
        CheckPos();

        if (trajectory != null) return;

        trajectory = path.GetPathPoints();
    }

    void CheckPos()
    {
        if (curIdx >= 0 && curIdx < trajectory.Length)
        {
            currentPosHolder = trajectory[curIdx];
        }
        else
        {
            ResetPath();
        }
    }

    float percentsPerSecond = 0.15f; // %15 of the path moved per second
    float currentPathPercent = 0.0f; //min 0, max 1

    private void FollowMovementPath()
    {
        if (currentPathPercent >= 1)
        {
            // reset
            currentPathPercent = 0;
        }

        currentPathPercent += percentsPerSecond * Time.deltaTime;
        Vector3 tarPos = Interp(trajectory, currentPathPercent);
        float distance = Vector3.Distance(currentPosHolder, gameObject.transform.position);
        tarPos = currentPosHolder;  // TODO: tarpos and change moveSpeed*Time.deltaTime to moveSpeed

        gameObject.transform.right = Vector3.RotateTowards(gameObject.transform.right, tarPos - gameObject.transform.position, rotationSpeed * Time.deltaTime, 0.0f);
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, tarPos, moveSpeed * Time.deltaTime);

        Vector3 relativePos = - cam.position + gameObject.transform.position;

        // the second argument, upwards, defaults to Vector3.up
        //Quaternion rotation = Quaternion.LookRotation(relativePos);
        //rotation *= Quaternion.Euler(0, 50, 0);
        //gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

        if (distance <= 0.3f)
        {
            curIdx += 1;
            CheckPos();
        }
    }

    void ResetPath()
    {
        curIdx = 0;
        currentPathPercent = 0;
        if (trajectory != null)
        {
            gameObject.transform.position = trajectory[0];
            currentPosHolder = trajectory[0];
        }
    }

    private void AddBouncingEffect()
    {
        SquashAndStretchKit.SquashAndStretch tem = gameObject.AddComponent<SquashAndStretchKit.SquashAndStretch>();
        tem.enableSquash = true;
        tem.enableStretch = true;
        tem.maxSpeedThreshold = 20;
        tem.minSpeedThreshold = 1;
        tem.maxSquash = 1.6f;
        tem.maxStretch = 1.5f;

        bouncyAdded = true;  // necessary components have been added, so turned off the bool
        aniStart = false;
    }

    public void TurnOnEditingMode()
    {
        editingMode = true;
    }

    public void TurnOffEditingMode()
    {
        editingMode = false;
    }

    public bool InEditingMode()
    {
        return editingMode;
    }

    public void MarkSelfSoundStartPoint()
    {
        selfSoundStartPoint = gameObject.transform.position;
    }

    public bool SoundStartNotMarked()
    {
        return selfSoundStartPoint == Vector3.zero;
    }

    public void AddSoundMark(GameObject newMark)
    {
        soundMarkCollection.Add(newMark);
    }

    public int GetSoundMarkCount()
    {
        return soundMarkCollection.Count;
    }

    public void HideSoundMarks()
    {
        // hide the sound lines from the display
        foreach (GameObject l in soundMarkCollection)
        {
            l.SetActive(false);
        }
    }

    public void AddChildStroke(GameObject newChild)
    {
        if (childStrokes == null) childStrokes = new List<GameObject>();
        childStrokes.Add(newChild);
    }

    private void OnCollisionEnter(Collision collision)
    {
        string softness;
        if (collision.gameObject.name == "TubeStroke")
        {
            softness = collision.gameObject.transform.parent.gameObject.GetComponent<SketchEntity>().softness;
        }
        else softness = collision.gameObject.GetComponent<SketchEntity>().softness;

        inCollision = true;
        movingSound.Stop();
        selfSound.Stop();
        Debug.LogWarning("collide!");
        
        if (softness == "hard")
            collisionHard.Play();
        else collisionSoft.Play();
    }

    private void OnCollisionExit(Collision collision)
    {
        inCollision = false;
    }

    public void AddColliders()
    {
        foreach (GameObject child in childStrokes)
        {
            Collider collider = child.AddComponent<BoxCollider>();
            collider.material.bounciness = 1.0f;
        }
    }

    private Vector3 Interp(Vector3[] pts, float t)
    {
        int numSections = pts.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
        float u = t * (float)numSections - (float)currPt;

        Vector3 a = pts[currPt];
        Vector3 b = pts[currPt + 1];
        Vector3 c = pts[currPt + 2];
        Vector3 d = pts[currPt + 3];

        return .5f * (
            (-a + 3f * b - 3f * c + d) * (u * u * u)
            + (2f * a - 5f * b + 4f * c - d) * (u * u)
            + (-a + c) * u
            + 2f * b
        );
    }

    public void SketchWrapUp()
    {
        AddColliders();
        FitColliderToChildren(gameObject);
        gameObject.AddComponent<PointerEvent>();
    }

    private void FitColliderToChildren(GameObject parentObj)
    {
        BoxCollider bc = parentObj.GetComponent<BoxCollider>();
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero); // center, size
        bool hasBounds = false;
        Renderer[] renderers = parentObj.GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renderers)
        {
            if (hasBounds)
            {
                bounds.Encapsulate(render.bounds);
            }
            else
            {
                bounds = render.bounds;
                hasBounds = true;
            }
        }
        if (hasBounds)
        {
            bc.center = bounds.center - parentObj.transform.position;
            bc.size = bounds.size;
        }
        else
        {
            bc.size = bc.center = Vector3.zero;
            bc.size = Vector3.zero;
        }
    }

    public void AddDependency(Dependency newDependency)
    {
        float dist = Vector3.Distance(newDependency.selfPos, trajectory[0]);
        closest = trajectory[0];

        // find the point of the trajectory which is the closest to the stamped point
        for (int i = 1; i < trajectory.Length; i++)
        {
            if (Vector3.Distance(newDependency.selfPos, trajectory[i]) < dist)
            {
                dist = Vector3.Distance(newDependency.selfPos, trajectory[i]);
                closest = trajectory[i];
            }
        }

        newDependency.showupPos = closest;

        dependencies.Add(newDependency);
        
    }

    public bool IsSelected()
    {
        return gameObject.GetComponentsInChildren<MeshRenderer>()[0].material.color == Color.white;
    }
}
