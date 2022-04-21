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
            collisionHard.loop = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (aniStart)
        {
            if (!movingSound.isPlaying && !inCollision)
            {
                movingSound.Play();
            }
        }

        if (selfSoundStartPoint != Vector3.zero && !editingMode)
        { 
            float distance = Vector3.Distance(selfSoundStartPoint, gameObject.transform.position);
            Debug.Log(distance);
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

    public void MarkSelfSoundStartPoint()
    {
        selfSoundStartPoint = gameObject.transform.position;
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
