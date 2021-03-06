﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    static private Slingshot s;

    // fields set in the Unity Inspector pane

    [Header("Set in Inspector")]

    public GameObject prefabProjectile;
    public float velocityMult = 8f;

    public AudioSource pullbackSound;
    public AudioSource releaseSound;

    // fields set dynamically

    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    private Rigidbody projectileRigidbody;
    private LineRenderer line;

    static public Vector3 LAUNCH_POS
    {
        get
        {
            if (s == null) return Vector3.zero;
            return s.launchPos;
        }
    }

    private void Awake()
    {
        s = this;
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
        line = GetComponent<LineRenderer>();
        
    }

    private void OnMouseEnter()
    {
        //print("Slingshot:OnMouseEnter()");
        launchPoint.SetActive(true);
    }
    private void OnMouseExit()
    {
        //print("Slingshot:OnMouseExit()");
        launchPoint.SetActive(false);
    }

    private void OnMouseDown()
    {
        // The player has pressed the mouse button while over Slingshot
        aimingMode = true;

        // Instantiate a Projectile
        projectile = Instantiate(prefabProjectile) as GameObject;

        // Start it at the launchPoint
        projectile.transform.position = launchPos;

        // Set it to isKinematic for now
        projectileRigidbody = projectile.GetComponent<Rigidbody>();
        projectileRigidbody.isKinematic = true;

        pullbackSound.Play();

        line.enabled = true;
        line.positionCount = 3;
        line.SetPosition(0, transform.Find("LeftArm").position);
        line.SetPosition(1, projectile.transform.position);
        line.SetPosition(2, transform.Find("RightArm").position);
    }

    private void Update()
    {
        // If Slingshot is not in aimingMode, don't run this code

        if (!aimingMode) return;

        // Get the current mouse position in 2D screen coordinates

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        // Find the delta from the launchPos to the mousePos3D

        Vector3 mouseDelta = mousePos3D - launchPos;

        // Limit mouseDelta to the radius of the Slingshot SphereCollider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;

        if(mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        // Move the projectile to this new position

        Vector3 projPos = launchPos + mouseDelta;
        line.SetPosition(1, projPos);
        projectile.transform.position = projPos;

        if (Input.GetMouseButtonUp(0))
        {
            // The mouse has been released

            line.enabled = false;
            aimingMode = false;
            projectileRigidbody.isKinematic = false;
            projectileRigidbody.velocity = -mouseDelta * velocityMult;
            FollowCam.POI = projectile;
            projectile = null;
            MissionDemolition.ShotFired();
            ProjectileLine.S.Poi = projectile;

            releaseSound.Play();
        }
    }
}
