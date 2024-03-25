using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomGravity : MonoBehaviour
{
    [SerializeField] private float m_GravityScale = 1.0f;

    private Rigidbody m_RB;

    private void Start()
    {
        m_RB = GetComponent<Rigidbody>();
        m_RB.useGravity = true;
    }

    private void FixedUpdate()
    {
        Vector3 customGravity = Physics.gravity * m_GravityScale;
        m_RB.AddForce(customGravity, ForceMode.Acceleration);
    }
}
