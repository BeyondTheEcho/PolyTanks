using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_ObjectLifetime = 10f;


    private void Start()
    {
        Destroy(gameObject, m_ObjectLifetime);
    }
}
