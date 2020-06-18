using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] private Transform m_BulletStart;
    private GameObject m_bulletPrefab;
    public KeyCode m_ShootKey;
    [SerializeField] private bool m_UseLeftClick;
    private bool m_Shoot;
    private GameObject[] m_TypeBullets;
    private N_Bullet[] m_StoreBulletArray;


    // N_Bullets - Normales (El nombre del GO en escena debe coincidir)
    public KeyCode m_NBullletKeySelection;
    private string k_typeBullet= "N_Bullet";
    [SerializeField] private float k_dmgNBullet = 10f;
    [SerializeField] private float k_Speed = 25f;
    [SerializeField] private float k_LifeTime = 2f;
    [Space(20)]

    // T_Bullets - Teletrasportation
    public KeyCode m_TBullletKeySelection;
    private string k_TBullet= "T_Bullet";
    [SerializeField] private float k_dmgTBullet = 0f;
    [SerializeField] private bool m_TeleportBool = true;
    [SerializeField] private float k_SpeedTeleport = 5f;
    [SerializeField] private float k_LifeTimeTeleport = 4f;
    [SerializeField] private LayerMask m_WhatIsTouchT;
    [SerializeField] private TeleportPlayer m_TeleportPlayer;


   




    // Start is called before the first frame update
    void Start()
    {
        m_TeleportPlayer = GetComponent<TeleportPlayer>();
        m_TypeBullets = GameObject.FindGameObjectsWithTag("Proyectil");
        OrderBullet(m_WhatIsTouchT, k_typeBullet, k_dmgNBullet, k_Speed, k_LifeTime, false);


    }

    void OnGUI()
    {
        // - Normal Bullet --
        SelectionTypeBullet (m_NBullletKeySelection, m_WhatIsTouchT , k_typeBullet, k_dmgNBullet, k_Speed, k_LifeTime, false);

        // - Transportation Bullet --
        //SelectionTypeBullet (m_TBullletKeySelection, m_WhatIsTouchT, k_TBullet, k_dmgTBullet, k_SpeedTeleport, k_LifeTimeTeleport, m_TeleportBool);

    }


    // Update is called once per frame
    void Update()
    {
        ShottingAtack( m_ShootKey, m_UseLeftClick );

       

    }

    private void SelectionTypeBullet (KeyCode k_KeyAcces, LayerMask m_WhatIsTouchT, string k_typeBullet, float k_dmgBullet, float k_Speed, float k_LifeTime, bool m_TeleportBool)
    {
        if (Event.current.Equals(Event.KeyboardEvent(k_KeyAcces.ToString())))
        {
            OrderBullet(m_WhatIsTouchT, k_typeBullet, k_dmgBullet, k_Speed, k_LifeTime, m_TeleportBool);
        }
    }

    private void OrderBullet(LayerMask m_WhatIsTouchT , string k_typeBullet, float k_dmgBullet, float k_Speed , float k_LifeTime , bool m_TeleportBool)
    {
        for (int i=0; i< m_TypeBullets.Length; i++)
        {
            if (m_TypeBullets[i].name == k_typeBullet)
            {
                m_StoreBulletArray = m_TypeBullets[i].GetComponentsInChildren<N_Bullet>(true);
                for (int z=0; z< m_StoreBulletArray.Length; z++)
                {
                    m_StoreBulletArray[z].k_dmg = k_dmgBullet;
                    m_StoreBulletArray[z].k_Speed = k_Speed;
                    m_StoreBulletArray[z].k_LifeTime = k_LifeTime;
                    m_StoreBulletArray[z].m_WhatIsTouchT = m_WhatIsTouchT;
                    m_StoreBulletArray[z].m_TeleportBool = m_TeleportBool;
                    
                }
            }
        }
    
    }


    private void ShottingAtack( KeyCode m_ShootKey, bool m_UseLeftClick )
    {
        if(Input.GetKeyDown(m_ShootKey) || (m_UseLeftClick && Input.GetMouseButtonDown(0))) 
       {
           Shoot ();
       }
    }

    private void Shoot ()
    {
           GameObject bullet = GetFreeBullet();
           PrepareBulletArray(bullet);
    }

    
    private GameObject GetFreeBullet()
    {
        for (int i = 0; i < m_StoreBulletArray.Length; i++)
        {
            if(m_StoreBulletArray[i].gameObject.activeInHierarchy == false)
            {
                return m_StoreBulletArray[i].gameObject;
            }
        }
        return null;
    }

    private void PrepareBulletArray( GameObject bullet)
        {
            if (bullet != null)
            {
                bullet.transform.position = m_BulletStart.position;
                bullet.transform.rotation = m_BulletStart.rotation;
                bullet.SetActive(true);
            }
        }

    

}
