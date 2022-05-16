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
    public List<GameObject> childStrokes;

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
}
