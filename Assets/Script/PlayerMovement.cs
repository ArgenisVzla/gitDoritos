using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    float horizontalMove = 0f;
    [Range (1,50)] public float walkSpeed = 30f;
    [Range (1,50)] public float runSpeed = 45f;
    private float scaleSpeed;
    private float coldDownForRun=0.5f;
    private float timer;
    bool jump = false;
    bool crouch = false;
    bool jumping = false;
    // Update is called once per frame
    void Update()
    {

        if(Input.GetButton("Horizontal"))
         {
            timer+= Time.deltaTime;
             if (timer>coldDownForRun)
             {
                  scaleSpeed=runSpeed;
              }
              else
               {
               scaleSpeed = walkSpeed;
              }
                
         }else{timer=0;}
         horizontalMove = Input.GetAxisRaw("Horizontal") * scaleSpeed;

        if(Input.GetButtonDown("Jump"))
            jump = true;
        


        if(Input.GetKey(KeyCode.Space))
        {
            jumping = true;
        }else {
            jumping = false;
        }


        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        } else if (Input.GetButtonUp("Crouch"))
        {
            crouch= false;
        }




    }

    void FixedUpdate()
    {
        controller.Move (horizontalMove * Time.fixedDeltaTime, crouch, jump, jumping);
        jump = false;
    }
}
