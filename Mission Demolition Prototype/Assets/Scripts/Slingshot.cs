using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    static public Slingshot Sling;

    public GameObject prefabProjectile;
    public float velocityMultiplier = 4.0F;
    //public bool 

    public GameObject launchPoint;
    public Vector3 launchPosition;
    public GameObject projectile;
    public bool isAiming;

    private void Awake()
    {
        Sling = this;
        Transform launchPointTrans = transform.Find("Launchpoint");
    }

    private void OnMouseEnter()
    {
        print("Slingshot:OnMouseEnter()");
    }
    private void OnMouseExit()
    {
        print("Slingshot:OnMouseExit()");
    }
}
