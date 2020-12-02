using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class impactFX : MonoBehaviour
{
    public AudioSource impactSound;

    private void Start()
    {
        //impactSound = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //impactSound.Play();
    }
}
