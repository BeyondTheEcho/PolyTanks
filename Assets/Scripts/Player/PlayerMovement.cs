using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader m_InputReader;
    [SerializeField] private Transform m_BodyTransform;
    [SerializeField] private Rigidbody m_Rb;

    [Header("Settings")]
    [SerializeField] private float m_MovementSpeed = 4f;
    [SerializeField] private float m_TurnRate = 30f;

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

        float yRotation = m_PreviousMovement.x * m_TurnRate * Time.deltaTime;
        m_BodyTransform.Rotate(0f, yRotation, 0f);
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
