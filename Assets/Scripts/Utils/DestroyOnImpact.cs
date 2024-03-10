using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestroyOnImpact : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        Destroy(gameObject);
    }
}
