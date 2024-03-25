using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerFiring : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject m_ServerProjectilePrefab;
    [SerializeField] private GameObject m_ClientProjectilePrefab;
    [SerializeField] private Transform m_ProjectileSpawnPoint;
    [SerializeField] private ParticleSystem m_MuzzleFlash;
    [SerializeField] private InputReader m_InputReader;
    [SerializeField] private Collider[] m_PlayerColliders;
    [SerializeField] private CinemachineImpulseSource m_RecoilImpulse;

    [Header("Settings")]
    [SerializeField] private float m_ProjectileSpeed;
    [SerializeField] private float m_FireRate;

    //Local Vars
    private bool m_IsFiring = false;
    private float m_TimeSinceLastShot;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        
        m_InputReader.a_PrimaryFire += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        m_InputReader.a_PrimaryFire -= HandlePrimaryFire;
    }

    private void HandlePrimaryFire(bool isFiring)
    {
        m_IsFiring = isFiring;
    }

    private void Start()
    {
        m_TimeSinceLastShot = Time.time;
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (!m_IsFiring) return;

        if (Time.time - m_TimeSinceLastShot < 1 / m_FireRate) return;

        PrimaryFireServerRpc(m_ProjectileSpawnPoint.position, m_ProjectileSpawnPoint.forward);
        SpawnDummyProjectile(m_ProjectileSpawnPoint.position, m_ProjectileSpawnPoint.forward);

        m_RecoilImpulse.GenerateImpulse();

        m_TimeSinceLastShot = Time.time;
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 forward)
    {
        TriggerMuzzleFlash();

        // Calculate the direction of travel (normalized)
        Vector3 direction = forward.normalized;

        // Calculate the rotation to align the projectile with the direction of travel
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        var projectile = Instantiate(m_ClientProjectilePrefab, spawnPos, targetRotation);

        foreach (Collider collider in m_PlayerColliders)
        {
            Physics.IgnoreCollision(collider, projectile.GetComponent<Collider>());
        }

        if (projectile.TryGetComponent(out Rigidbody rb))
        {
            rb.velocity = direction * m_ProjectileSpeed;
        }
    }



    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 forward)
    {
        // Calculate the direction of travel (normalized)
        Vector3 direction = forward.normalized;

        // Calculate the rotation to align the projectile with the direction of travel
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        var projectile = Instantiate(m_ServerProjectilePrefab, spawnPos, targetRotation);

        foreach(Collider collider in m_PlayerColliders)
        {     
            Physics.IgnoreCollision(collider, projectile.GetComponent<Collider>());     
        }

        if (projectile.TryGetComponent(out DealDamageOnContact damage))
        {
            damage.SetOwner(OwnerClientId);
        }

        if(projectile.TryGetComponent(out Rigidbody rb))
        {
            rb.velocity = rb.transform.forward * m_ProjectileSpeed;
        }

        SpawnDummyProjectileClientRpc(spawnPos, forward);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 forward)
    {
        if (IsOwner) return;

        SpawnDummyProjectile(spawnPos, forward);
    }

    private void TriggerMuzzleFlash()
    {
        if (m_MuzzleFlash != null)
        {
            if (m_MuzzleFlash.isPlaying)
            {
                m_MuzzleFlash.Stop();
            }

            m_MuzzleFlash.Play();
        }
    }
}
