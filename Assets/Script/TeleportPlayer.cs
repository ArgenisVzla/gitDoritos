using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{

    public Transform m_PlayerGO;
    public Rigidbody2D m_rb;
    public Vector3 m_PointObjective;

    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>(); 
        m_PlayerGO = gameObject.transform;

    }

    public void Teleport()
    {
        m_rb.velocity = new Vector3 (0f, 0f, 0f);
        m_PlayerGO.position = m_PointObjective;
    }

    public void PositionSpawn(Vector3 position)
    {
        m_PointObjective = position;
    }

}
