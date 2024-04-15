using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader m_InputReader;
    [SerializeField] private Transform m_BodyTransform;
    [SerializeField] private Transform m_AimTargetTransform;
    [SerializeField] private Rigidbody m_Rb;

    [Header("Settings")]
    [SerializeField] private float m_MovementSpeed = 4f;
    [SerializeField] private float m_TankTurnRate = 30f;
    [SerializeField] private float m_CameraTurnRate = 1.0f;
    [SerializeField] private float m_MinPitch = -10f;
    [SerializeField] private float m_MaxPitch = 60f;

    // 1 = instant 0 = no interpolation
    [SerializeField][Range(0f, 1f)] private float m_RotationDamping = 1f;

    private Vector2 m_PreviousMovement;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        m_InputReader.a_PlayerMovement += HandleMove;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        m_InputReader.a_PlayerMovement -= HandleMove;
    }

    void Update()
    {
        if (!IsOwner) return;

        float yRotation = m_PreviousMovement.x * m_TankTurnRate * Time.deltaTime;
        m_BodyTransform.Rotate(0f, yRotation, 0f);

        // Rotate the aim target up and down based on mouse input
        float mouseX = m_InputReader.m_MouseDelta.x;
        float mouseY = m_InputReader.m_MouseDelta.y;

        float pitchChange = -mouseY * m_CameraTurnRate * Time.deltaTime;
        float yawChange = mouseX * m_CameraTurnRate * Time.deltaTime;

        Vector3 currentEulerAngles = m_AimTargetTransform.rotation.eulerAngles;
        float newPitch = currentEulerAngles.x + pitchChange;

        if (newPitch > 180f)
        {
            newPitch -= 360f;
        }

        newPitch = Mathf.Clamp(newPitch, m_MinPitch, m_MaxPitch);

        Quaternion targetRotation = Quaternion.Euler(newPitch, currentEulerAngles.y + yawChange, 0f);
        m_AimTargetTransform.rotation = Quaternion.Lerp(m_AimTargetTransform.rotation, targetRotation, m_RotationDamping);

    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        m_Rb.velocity = m_BodyTransform.forward * m_PreviousMovement.y * m_MovementSpeed;
    }
    
    private void HandleMove(Vector2 moveInput)
    {
        m_PreviousMovement = moveInput;
    }
}
