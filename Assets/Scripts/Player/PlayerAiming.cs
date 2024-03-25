using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Transform m_TurretPivotTransform;
    [SerializeField] private Transform m_BarrelPivotTransform;
    [SerializeField] private InputReader m_InputReader;
    [SerializeField] private CinemachineVirtualCamera m_MainCam;
    [SerializeField] private CinemachineVirtualCamera m_AimCam;
    [SerializeField] private UIController m_UIController;

    [Header("Turret Settings")]
    [SerializeField] private float m_TurretRotationSpeed = 20f;
    [SerializeField] private float m_MinTurretYaw = -90f;
    [SerializeField] private float m_MaxTurretYaw = 90f;

    [Header("Barrel Settings")]
    [SerializeField] private float m_BarrelRotationSpeed = 1f;
    [SerializeField] private float m_MinBarrelPitch = -10f;
    [SerializeField] private float m_MaxBarrelPitch = 0f;

    [Header("Camera Settings")]
    [SerializeField] private int m_PlayerCamPriority = 100;


    //vars
    private bool m_IsMouseLocked = false;
    private bool m_IsAiming = false;
    private Crosshair m_Crosshair;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        
        m_InputReader.a_EscapePressed += HandleMouse;
        m_InputReader.a_Aim += UpdateAimStatus;

        m_MainCam.Priority = m_PlayerCamPriority;
        m_AimCam.Priority = m_PlayerCamPriority;

        HandleMouse();
    }

    private void Start()
    {
        if (!IsOwner) return;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        m_InputReader.a_EscapePressed -= HandleMouse;
        m_InputReader.a_Aim -= UpdateAimStatus;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (m_Crosshair == null)
        {
            m_Crosshair = m_UIController.m_PlayerUI.GetCrosshair();
        }

        if (m_IsAiming)
        {
            m_MainCam.gameObject.SetActive(false);
            m_AimCam.gameObject.SetActive(true);

            m_Crosshair.Show();
        }
        else
        {
            m_MainCam.gameObject.SetActive(true);
            m_AimCam.gameObject.SetActive(false);

            m_Crosshair.Hide();
        }

    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (!m_IsAiming) return;

        // Cast a ray from the camera through the center of the screen
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        // Iterate through the hits to find the first non-player target
        foreach (RaycastHit hit in hits)
        {
            // Ignore hit if the highest level parent transform is the same as the script's highest level parent transform
            if (!IsSameHighestParent(hit.transform, transform))
            {
                // Calculate the direction to the hit point
                Vector3 direction = hit.point - m_TurretPivotTransform.position;
                direction.y = 0; // Ignore the Y component

                // Calculate the target rotation for the turret
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

                // Rotate the turret towards the target rotation
                m_TurretPivotTransform.rotation = Quaternion.RotateTowards(m_TurretPivotTransform.rotation, targetRotation, Time.deltaTime * m_TurretRotationSpeed);

                // Calculate the local direction to the hit point from the barrel pivot
                Vector3 targetDirLocal = m_BarrelPivotTransform.parent.InverseTransformPoint(hit.point);

                // Add a small tolerance to handle minor positional differences
                float positionTolerance = 0.01f;
                targetDirLocal += targetDirLocal.normalized * positionTolerance;

                // Calculate the angle between the local direction to the hit point and the barrel's forward direction
                float angle = Vector3.SignedAngle(Vector3.forward, targetDirLocal, Vector3.right);

                // Clamp the angle between the desired range
                float clampedAngle = Mathf.Clamp(angle, m_MinBarrelPitch, m_MaxBarrelPitch);

                // Adjust the speed of barrel rotation
                float finalAngle = Mathf.MoveTowardsAngle(m_BarrelPivotTransform.localEulerAngles.x, clampedAngle, Time.deltaTime * m_BarrelRotationSpeed);

                // Rotate the barrel towards the clamped angle
                m_BarrelPivotTransform.localRotation = Quaternion.Euler(finalAngle, 0, 0);

                // Clamp the turret's rotation between -90 and 90 degrees
                float currentYaw = m_TurretPivotTransform.localEulerAngles.y;
                float clampedYaw = currentYaw;

                // Wrap the angle if it exceeds 180 or -180 degrees
                if (currentYaw > 180f)
                {
                    clampedYaw -= 360f;
                }
                else if (currentYaw < -180f)
                {
                    clampedYaw += 360f;
                }

                // Clamp the wrapped angle between -90 and 90 degrees
                clampedYaw = Mathf.Clamp(clampedYaw, -90f, 90f);

                // Apply the clamped rotation to the turret
                m_TurretPivotTransform.localRotation = Quaternion.Euler(0, clampedYaw, 0);

                // Debug draw the aim direction
                Debug.DrawRay(m_BarrelPivotTransform.position, m_BarrelPivotTransform.forward * 1000f, Color.red);

                // Break out of the loop since we found the target
                break;
            }
        }
    }

    private void HandleMouse()
    {
        if (!IsOwner) return;

        if (m_IsMouseLocked)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        Cursor.visible = m_IsMouseLocked;
        m_IsMouseLocked = !m_IsMouseLocked;
    }

    private void UpdateAimStatus(bool aimStatus)
    {
        m_IsAiming = aimStatus;
    }

    // Function to check if two transforms have the same highest level parent
    bool IsSameHighestParent(Transform transform1, Transform transform2)
    {
        Transform parent1 = transform1;
        Transform parent2 = transform2;

        while (parent1.parent != null)
        {
            parent1 = parent1.parent;
        }

        while (parent2.parent != null)
        {
            parent2 = parent2.parent;
        }

        return parent1 == parent2;
    }
}
