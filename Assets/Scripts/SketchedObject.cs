using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SketchedObject : MonoBehaviour
{
    public AudioSource selfSound;
    public AudioSource movingSound;
    public AudioSource collisionHard;
    public AudioSource collisionSoft;
    public string identity; // sketch recognition result
    public string softness; // hard or soft
    public bool aniStart;
    bool inCollision;
    string rootFolder;

    Vector3 selfSoundStartPoint;
    bool editingMode;
    List<GameObject> soundMarkCollection;

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

        if (gameObject.name == "Floor")
        {
            identity = "floor";
            softness = "hard";
        }
        else if (gameObject.name == "Wall")
        {
            identity = "wall";
            softness = "hard";
        }
        ObjectIdentity("dog");
    }

    public void ObjectIdentity(string recognitionResult)
    {
        identity = recognitionResult;
        Debug.LogWarning(identity);
        AddSound();
    }

    void AddSound()
    {
        if (identity == "car" || identity == "airplane" || identity == "dog")
        {
            selfSound.clip = Resources.Load<AudioClip>(rootFolder + "/" + identity + "/" + "self");
            movingSound.clip = Resources.Load<AudioClip>(rootFolder + "/" + identity + "/" + "moving");
            collisionHard.clip = Resources.Load<AudioClip>(rootFolder + "/" + identity + "/" + "hardCollision");
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

    void HideSoundMarks()
    {
        // hide the sound lines from the display
        foreach (GameObject l in soundMarkCollection)
        {
            l.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        inCollision = true;
        movingSound.Stop();
        if (collision.gameObject.GetComponent<SketchedObject>().softness == "hard")
            collisionHard.Play();
        else collisionSoft.Play();
    }
}
