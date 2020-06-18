using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
	[Range(0, 1000)][SerializeField] private float m_JumpForce = 55;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .2f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .2f;	// How much to smooth out the movement
	[Range(0, 15f)] [SerializeField] private float m_ScaleVelocity = 6.5f;		// How much to maxSpeed applied to movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;
	

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;



	// -- Sliding & SlidingJump
	public UnityEvent OnWallEvent;
	[SerializeField] private Transform m_WallCheck;							// A position marking where to check if the player is touching to wall.
	private bool m_Walled;			// Whether or not the player is wall.
	const float k_WallRadius = .2f; // Radius of the overlap circle to determine if wall
	[SerializeField] private LayerMask m_WhatIsWall;							// A mask determining what is wall to the character
	private bool m_wallSliding = false;
	[Range(0, 6f)][SerializeField] private float k_wallSlidingSpeed = 3.3f;			// resistence sliding. recomend 3
	private bool m_wallJumping;
	[Range(1, 700)][SerializeField] private float k_RepulsionForceWall = 500f;							// Amount of force added when the player jumps on wall. recomend 500f
	[Range(1, 700)][SerializeField] private float k_WallJumpingForce = 500f;							// Amount of force added when the player jumps on wall. recomend 500f


	// -- Extras Jumps
	private int k_ExtraJumps;
	[Range(0,5)] public int k_ExtraJumpValue =3;																// recomend 2
	[Range(0, 1000)][SerializeField] private float k_ExtraJumpForceValue = 400f;							// Amount of force added when the player do an ExtraJumps.
	private bool m_ExtraJumpBool = false;																	//Initial  false


	//-- Hold Key Jump
	private bool m_IsJumping = false;
	private float k_JumpingTimeCounter;
	[Range(0,1)][SerializeField] private float k_JumpTime = 0.25f; //Time for Hold Key Jump recomend 0.25f


	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		k_ExtraJumps = k_ExtraJumpValue;

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
		
		if (OnWallEvent == null)
			OnWallEvent = new UnityEvent();
		
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		bool wasWall_Ed = m_Walled;
		m_Walled= false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}

		// The player is grounded if a circlecast to the wallccheck position hits anything designated as wall
		Collider2D[] colliders_w = Physics2D.OverlapCircleAll(m_WallCheck.position, k_WallRadius, m_WhatIsWall);
		for (int i = 0; i < colliders_w.Length; i++)
		{
			if (colliders_w[i].gameObject != gameObject)
			{
				m_Walled = true;
				if (!wasWall_Ed)
					OnWallEvent.Invoke();
			}
		}
	}


	public void Move(float move, bool crouch, bool jump, bool jumping)
	{

		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}
			
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * m_ScaleVelocity, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded)
		{
			if (jump)
			{
				// Add a vertical force to the player.
				m_Grounded = false;
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
				jump = false;
				
				move=0;
				
				m_IsJumping = true;
				k_JumpingTimeCounter = k_JumpTime;


				m_ExtraJumpBool=false;
			} 
		}
		//-- Hold Key - Jump
		if(jumping && m_IsJumping){

			if(k_JumpingTimeCounter > 0){
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
				k_JumpingTimeCounter -= Time.fixedDeltaTime;
			}else{
				m_IsJumping = false;
			}

		}
		if (!jumping){
			m_IsJumping = false;
		}

		// -- Sliding and JumpSliding ---
		
		if (m_Walled && !m_Grounded && move != 0)
		{
			m_wallSliding = true;
		}else{
			m_wallSliding = false;
		}

		if (m_wallSliding)
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, Mathf.Clamp(m_Rigidbody2D.velocity.y, -k_wallSlidingSpeed, float.MaxValue));

		if (m_wallSliding && jump )
		{
			m_wallJumping = true;
			jump = false;
		}
		
		if (m_wallJumping)
		{
			if(move>0)
			{
				
				m_Rigidbody2D.AddForce(new Vector2(k_RepulsionForceWall*-1f, k_WallJumpingForce));
				m_wallJumping = false;
				StartCoroutine(ComeBackToWall(move));
				move=0;
				m_Rigidbody2D.velocity= new Vector2(m_Rigidbody2D.velocity.x, 0f);
			}
			if (move <0)
			{
				m_Rigidbody2D.AddForce(new Vector2(k_RepulsionForceWall, k_WallJumpingForce));
				m_wallJumping = false;
				StartCoroutine(ComeBackToWall(move));
				move=0;

			}
		}

		// - - Extra Jumps
		if(!m_Grounded)
		{
			StartCoroutine(TimmingExtraJump(jump));
			if(k_ExtraJumps>0)
			{
				if(!m_wallSliding && jump )
				{
					m_Rigidbody2D.AddForce(new Vector2(0f, k_ExtraJumpForceValue));
					m_Grounded = false;
					move=0;
					k_ExtraJumps--;
					m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0f);

				}	
			}
		}

		if(m_Grounded)
		{
			m_ExtraJumpBool=false;
			k_ExtraJumps = k_ExtraJumpValue;
		}



	}
	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
			//	Vector3 theScale = transform.localScale;
			//	theScale.x *= -1;
			//	transform.localScale = theScale;
			transform.Rotate(0f, 180f, 0f);
	}
	
	
	private IEnumerator ComeBackToWall(float move)
	{
		yield return new WaitForSeconds(0.1f);
		if(move > 0)
		{
			 m_Rigidbody2D.AddForce(new Vector2(k_RepulsionForceWall*0.15f, k_WallJumpingForce*0.10f));
			 
		}
		if (move < 0)
		{
			m_Rigidbody2D.AddForce(new Vector2(k_RepulsionForceWall*0.15f*-1f, k_WallJumpingForce*0.10f));
		}
	}
	private IEnumerator TimmingExtraJump(bool jump)
	{
		yield return new WaitForSeconds(0.2f);
		if(jump)
			m_ExtraJumpBool=true;
	}

}