using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform m_PlayerSpawnPoint; //Punto de inicio de nuestro jugador
    public GameObject m_Player1;
    public static GameManager instance { get; private set; } //la instancia solo puede haber una en todo el proyecto

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
      
    }
    void Start()
    {

        m_Player1 = GameObject.FindGameObjectWithTag("Player1");

        m_Player1.transform.position = m_PlayerSpawnPoint.position;
    }

}
