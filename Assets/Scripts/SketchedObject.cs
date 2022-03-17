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

    float startingPitch = 0.5f;
    float startingVolume;

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
        collisionHard.clip = Resources.Load<AudioClip>("SFX/ball_bounce_sound");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if(Collision.ga)
        //Debug.LogWarning("collide!");
        collisionHard.Play();
    }
}
