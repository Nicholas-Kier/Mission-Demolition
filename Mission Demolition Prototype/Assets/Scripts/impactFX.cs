using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class impactFX : MonoBehaviour
{
    public AudioSource impactSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 1)
        {
            impactSound.Play();
        }
    }
}
