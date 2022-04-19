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

    // Start is called before the first frame update
    void Start()
    {
        selfSound = gameObject.AddComponent<AudioSource>();
        movingSound = gameObject.AddComponent<AudioSource>();
        collisionHard = gameObject.AddComponent<AudioSource>();
        collisionSoft = gameObject.AddComponent<AudioSource>();
        selfSound.playOnAwake = false;
        movingSound.playOnAwake = false;
        collisionHard.playOnAwake = false;
        collisionSoft.playOnAwake = false;
        inCollision = false;
    }

    public void ObjectIdentity(string recognitionResult)
    {
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
        else identity = recognitionResult;
        Debug.LogWarning(identity);
        AddSound();
    }

    void AddSound()
    {
        if (identity == "car")
        {
            selfSound.clip = Resources.Load<AudioClip>("SFX/police_siren");
            movingSound.clip = Resources.Load<AudioClip>("SFX/car_engine_sound");
            collisionHard.clip = Resources.Load<AudioClip>("SFX/car_crash_sound");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (aniStart)
        {
            if(!movingSound.isPlaying && ! inCollision) movingSound.Play();
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
