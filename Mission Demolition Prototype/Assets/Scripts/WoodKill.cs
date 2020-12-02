using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodKill : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            Destroy(gameObject);
        }
    }
}
