using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SketchEntity : MonoBehaviour
{
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

    bool inCollision;
    string rootFolder;
    Dictionary<string, bool> supportedSketches = new Dictionary<string, bool>();
    Vector3 selfSoundStartPoint;
    bool editingMode;
    List<GameObject> soundMarkCollection;
    public bool bouncyAdded = false;

    public float moveSpeed; // the speed when moving along the path
    public float rotationSpeed;

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
        selfSoundStartPoint = Vector3.zero;
        supportedSketches.Add("ambulance", true);
        supportedSketches.Add("airplane", true);
        supportedSketches.Add("dog", true);
        supportedSketches.Add("police car", true);
        supportedSketches.Add("basketball", true);
        path = GameObject.Find("Path").GetComponent<PathFollower>();

        moveSpeed = 1f;
        rotationSpeed = 20f;

        if (gameObject.name != "Floor" && gameObject.name != "Wall" && gameObject.name != "Table" && gameObject.name != "Chair")
        {
            ObjectIdentity("basketball");
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
        if (aniStart)
        {
            HideSoundMarks();

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
        inCollision = true;
        movingSound.Stop();
        selfSound.Stop();
        Debug.LogWarning("collide!");
        if (collision.gameObject.GetComponent<SketchEntity>() == null) Debug.LogWarning("is null! " + collision.gameObject.name);
        if (collision.gameObject.GetComponent<SketchEntity>().softness == "hard")
            collisionHard.Play();
        else collisionSoft.Play();
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
}
