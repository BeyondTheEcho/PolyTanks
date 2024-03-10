using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Transform m_TurretTransform;
    [SerializeField] private Transform m_TurretBarrelTransform;
    [SerializeField] private InputReader m_InputReader;

    [Header("Settings")]
    [SerializeField] private float m_TurretRotationSpeed = 50f;

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        // Cast a ray from the camera through the mouse position
        Ray ray = Camera.main.ScreenPointToRay(m_InputReader.m_AimPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            // Calculate the direction to the hit point
            Vector3 direction = hit.point - m_TurretTransform.position;
            direction.y = 0; // Ignore the Y component

            // Calculate the target rotation using trigonometry
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            // Rotate the turret towards the target rotation
            m_TurretTransform.rotation = Quaternion.RotateTowards(m_TurretTransform.rotation, targetRotation, Time.deltaTime * m_TurretRotationSpeed);
        }
    }
}
