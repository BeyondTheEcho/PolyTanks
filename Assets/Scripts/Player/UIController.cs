using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class UIController : NetworkBehaviour
{
    [SerializeField] private GameObject m_UIPrefab;
    [SerializeField] private CinemachineVirtualCamera m_PlayerAimCam;
    private GameObject m_UIInstance;
    public PlayerUI m_PlayerUI;
    private Health m_Health;
    private Cinemachine3rdPersonAim m_ThirdPersonAim;

    private void Awake()
    {
        m_Health = GetComponent<Health>();
    }

    private void Start()
    {
        if (IsOwner)
        {
            CreateUI();

            SetHealthInUI();

            SetupThirdPersonComponent();
        }
    }

    private void SetupThirdPersonComponent()
    {
        if (m_PlayerAimCam != null)
        {
            m_ThirdPersonAim = m_PlayerAimCam.GetComponent<Cinemachine3rdPersonAim>();

            //m_ThirdPersonAim.AimTargetReticle = m_PlayerUI.GetCrosshair().GetComponent<RectTransform>();
        }
    }

    private void SetHealthInUI()
    {
        if (m_UIInstance.TryGetComponent(out PlayerUI playerUI))
        {
            m_PlayerUI = playerUI;

            m_PlayerUI.SetHealth(m_Health);
        }
    }

    private void CreateUI()
    {
        m_UIInstance = Instantiate(m_UIPrefab);
        DontDestroyOnLoad(m_UIInstance);
    }
}
