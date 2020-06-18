using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour
{
    private float k_Health;
    [Range(0,1000)]public float k_Healthvalue = 500f;  //Defect value 500
    [Range(0, 2)]public float k_TimeToDie = 1f;         // defect value 1f

    private void Start(){
        k_Health = k_Healthvalue;
    }
    public void TakeDamage(float damage)
    {
        if (k_Health > 0)
        {
            k_Health -= damage;
        }

        if (k_Health <= 0)
        {
            StartCoroutine(Die(k_TimeToDie));

        }
    }

    private void Update(){
        print(k_Health);
    }

    private IEnumerator Die(float k_TimeToDie)
    {
        yield return new WaitForSeconds(k_TimeToDie);
        gameObject.SetActive(false);
    }  
}
