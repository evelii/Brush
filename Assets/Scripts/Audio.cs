using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public AudioSource bounceSound;
    public GameObject ball;
    float startingPitch = 0.5f;
    float startingVolume;

    private void Start()
    {
        startingVolume = bounceSound.volume;
    }

    private void OnCollisionEnter()
    {
        bounceSound.pitch = startingPitch + ball.GetComponent<Rigidbody>().velocity.sqrMagnitude * 0.05f;
        bounceSound.volume = startingVolume + ball.GetComponent<Rigidbody>().velocity.sqrMagnitude * 0.5f;
        bounceSound.Play();
    }
}
