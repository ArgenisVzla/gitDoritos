using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    private int extraJumps;
    public int extraJumpsValue;
    private bool isGrounded;
    //private RigidBody2D rb;

    void Start(){
        extraJumps = extraJumpsValue;
    }

    void Update(){
        if (isGrounded == true)
        {
            extraJumps = extraJumpsValue;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && extraJumps>0){
           // rb.velocity = Vector2.up*jumpForce;
            extraJumps--;
        }else if(Input.GetKeyDown(KeyCode.UpArrow) && extraJumps == 0 && isGrounded == true){
           // rb.velocity = Vector2.up * jumpForce;
        }
    }
}
