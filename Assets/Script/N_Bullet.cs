using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class N_Bullet : MonoBehaviour
{
    [HideInInspector] public float k_Speed = 15f;
    [HideInInspector] public float k_LifeTime = 4f;
    [HideInInspector] public Rigidbody2D m_RigidBody2D;
    [HideInInspector] public float k_dmg = 0;


    // - Teleport

     [HideInInspector] public bool m_TeleportBool = false;
     public TeleportPlayer m_Teleport;
     [HideInInspector] public LayerMask m_WhatIsTouchT;



    private void Awake(){
        m_RigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable(){
        StartCoroutine(Desactivate(k_LifeTime));
        m_RigidBody2D.velocity = transform.right*k_Speed;
    }
    
    void OnTriggerEnter2D (Collider2D hitInfo)
	{
        if (!m_TeleportBool)
        {
            HP m_HP = hitInfo.GetComponent<HP>();
            if (m_HP != null)
            {
                m_HP.TakeDamage(k_dmg);
                gameObject.SetActive(false);
            }
        }else if (m_TeleportBool)
        {
            m_Teleport.PositionSpawn(gameObject.transform.position);
            gameObject.SetActive(false);
        }

	}
    private IEnumerator Desactivate(float time){
        yield return new WaitForSeconds(time);
		gameObject.SetActive(false );
    }
}
