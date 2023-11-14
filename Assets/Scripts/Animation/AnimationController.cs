using UnityEngine;
using System.Collections;

public enum State {
	Running,
	AfterRun,
	Idle,
	Jumping,
	Dodging,
	Attacking,
	WaitingAttack,
	Blocking,
	WeaponSwitching,
	SmoothJumping,
	Walking,
	Waiting,
	Dead
}

public class AnimationController : MonoBehaviour {

	private Animator animator;
	private CharacterController controller;
	private PlayerController pc;

	public static State currentState;
	//private NetworkSyncAnim syncAnim;

	private bool waitingAttack;
	public int attackValue;
	public State editorState;
	private bool previousWaitingAttack;
	//public WeaponController wp;
	public bool local;

	// Use this for initialization
	void Start () 
	{
		animator = GetComponent<Animator> ();
		controller = transform.parent.GetComponent<CharacterController> ();
		pc = transform.parent.GetComponent<PlayerController> ();
		//syncAnim = GetComponent<NetworkSyncAnim> ();
		//wp = GetComponent<WeaponController> ();
	}
	
	// Update is called once per frame
	void Update () 
	{

		ManageStates ();
		UpdateAnimator ();

	}

	void ManageStates() 
	{
		editorState = currentState;
		if (animator.GetNextAnimatorStateInfo (1).IsName ("Block") || (animator.GetCurrentAnimatorStateInfo (1).IsName ("Block"))) {
			currentState = State.Blocking;
		} else if (currentState != State.Attacking && currentState != State.WaitingAttack) {
			if (animator.GetNextAnimatorStateInfo (0).IsName ("IdleJump")) {
				currentState = State.Jumping;
			}
			if (animator.GetNextAnimatorStateInfo (0).IsName ("RunJump")) {
				currentState = State.SmoothJumping;
			}
			if ((animator.GetNextAnimatorStateInfo (0).IsName ("Run") || animator.GetCurrentAnimatorStateInfo (0).IsName ("SideWalk") || animator.GetCurrentAnimatorStateInfo (0).IsName ("Run")) && controller.isGrounded) {
				currentState = State.Running;
			}
			if (animator.GetNextAnimatorStateInfo (0).IsName ("Roll")) {
				currentState = State.Dodging;
			}
			if (animator.GetNextAnimatorStateInfo (0).IsName ("Idle") || animator.GetCurrentAnimatorStateInfo (0).IsName ("Idle")) {
				currentState = State.Idle;
			}
		}

	}

	void UpdateAnimator() 
	{
		
		SetFloat ("Speed", pc._currentSpeed);

		/* Run */
		if (InputManager.GetInstance().AxisPressed() && pc.canMove) {
			SetBool ("Moving", true);
		} else {
			SetBool ("Moving", false);
		}

		/* Dodge */
		/*if (Input.GetKeyDown(KeyCode.LeftShift) && controller.isGrounded && currentState != State.Attacking) {
			if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Roll") && !animator.GetNextAnimatorStateInfo (0).IsName ("Roll")) {
				pc.LookForward ("input");
				SetBool ("Dodge", true);
			}
		}

		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Roll") && animator.GetCurrentAnimatorStateInfo (0).normalizedTime > 0.6f) {
			SetBool ("Dodge", false);
		}*/

		 /*Jump */
		if(Input.GetKeyDown(KeyCode.Space) && currentState != State.Jumping && currentState != State.SmoothJumping) {
			SetBool ("Jump", true);
		}
			
		if(animator.GetNextAnimatorStateInfo (0).IsName ("IdleJump") || animator.GetNextAnimatorStateInfo (0).IsName ("RunJump")) {
			SetBool ("Jump", false);
		} 

		if(controller.isGrounded && animator.GetCurrentAnimatorStateInfo(0).IsName("IdleJump") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f) {
			SetBool ("IdleJumpOut", true);
		} else {
			SetBool ("IdleJumpOut", false);
		}

		if(controller.isGrounded && animator.GetCurrentAnimatorStateInfo(0).IsName("RunJump") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f) {
			SetBool ("RunJumpOut", true);
		} else {
			SetBool ("RunJumpOut", false);
		}

		/* Attack */
		/*string value = "Attack" + attackValue.ToString ();
		waitingAttack = animator.GetCurrentAnimatorStateInfo (1).IsName (value) 
			&& animator.GetCurrentAnimatorStateInfo (1).normalizedTime > 0.95f;

		if (Input.GetMouseButtonDown(0) && controller.isGrounded && currentState != State.Dodging && currentState != State.Blocking) {
			//if (wp.weaponOut) {
				pc.LookForward ("camera");
				if (currentState != State.WeaponSwitching) {
					if (attackValue == 0 || waitingAttack) {
						attackValue++;
						StopCoroutine ("AttackTimeout");
						currentState = State.Attacking;
						pc.canMove = false;
					}

					if (attackValue == 4) {
						attackValue = 1;
					}
				}
		}

		SetInteger ("AttackValue", attackValue);

		if (previousWaitingAttack != waitingAttack && waitingAttack) {
			StartCoroutine ("AttackTimeout");
		}
		previousWaitingAttack = waitingAttack;

		if (animator.GetNextAnimatorStateInfo (1).IsName ("CancelAttack")) {
			attackValue = 0;
		}*/

		/*Weapon Switch */
		/*if(Input.GetKeyDown(KeyCode.X) && currentState != State.WeaponSwitching && currentState != State.Attacking) {
			StartCoroutine ("TakeWeapon", "normal");
		}

		
		if (Input.GetKeyDown (KeyCode.Mouse1) && controller.isGrounded && currentState != State.Dodging && currentState != State.Attacking && currentState != State.WeaponSwitching) {
			//if (!wp.weaponOut) {
				StartCoroutine ("TakeWeapon", "block");
			//} 
		}
		if(Input.GetKey(KeyCode.Mouse1) && controller.isGrounded && currentState != State.Dodging && currentState != State.Attacking && currentState != State.WeaponSwitching){
			//if (wp.weaponOut) {
				SetBool ("Shield", true);
				pc.LookForward ("camera");
			//}
		} else {
			SetBool ("Shield", false);
			SetBool ("Sidewalk", false);
		}


		//if (currentState == State.Blocking  && !UIManager.UiEnabled) {
		if (currentState == State.Blocking) {
			if(Input.GetAxisRaw("Vertical") != 0) {
				SetBool ("Sidewalk", false);
				SetBool ("Moving", true);
			} else {
				SetBool ("Moving", false);
				if (Input.GetAxisRaw ("Horizontal") != 0) {
					SetBool ("Sidewalk", true);
				} else {
					SetBool ("Sidewalk", false);
				}
			}
		}*/
	}

	IEnumerator AttackTimeout() 
	{

		float start = Time.time;
		float elapsed = 0;

		currentState = State.WaitingAttack;

		while (elapsed < 1f) {
			elapsed = Time.time - start;


			if(elapsed > 0.25f) {
				pc.canMove = true;	

				if(Input.anyKey) {
					currentState = State.Running;
					//attackValue = -1;
					//yield return new WaitForSeconds (0.1f);
					attackValue = 0;
					yield break;
				}
			}
			yield return null;
		}

		if(waitingAttack) {
			currentState = State.Idle;
			attackValue = 0;
		}
	}

	/*public IEnumerator TakeWeapon(string type) 
	{
		currentState = State.WeaponSwitching;
		State oldState = currentState;

		SetTrigger ("Takeweapon");


		while (!animator.GetCurrentAnimatorStateInfo (1).IsName ("Takeweapon")) {
			yield return null;
		}

		while (animator.GetCurrentAnimatorStateInfo (1).normalizedTime < 0.95f) {
			yield return null;
		}

		wp.SwitchTrigger ();

		yield return new WaitForSeconds (0.2f);

		if(type == "attack") {
			attackValue++;
			StopCoroutine ("AttackTimeout");
			pc.LookForward ("camera");
			currentState = State.Attacking;
			pc.canMove = false;
			yield break;
		}
		if(type == "block") {
			SetBool ("Shield", true);
			pc.LookForward ("camera");
			yield break;
		}
		if (type == "normal") {
			if (oldState == State.WaitingAttack) {
				if (PlayerController.KeyPressed ()) {
					currentState = State.Running;
				} else {
					currentState = State.Idle;
				}

			} else {
				currentState = oldState;
			}
		}
	}*/
		

	void SetBool(string name, bool value) 
	{
		if (animator.GetBool (name) != value) {
			animator.SetBool (name, value);
			//if(!local)
			//	syncAnim.EmitAnimatorInfo (name, value.ToString());
		}
	}

	void SetFloat(string name, float value) 
	{
		if (Mathf.Abs (animator.GetFloat (name) - value) > 0.2f) {
			animator.SetFloat (name, value);
		//	if (!local) 
			//	syncAnim.EmitAnimatorInfo (name, (Mathf.Floor((value+0.01f)*100)/100).ToString ());
		}
	}

	void SetInteger(string name, int value) 
	{
		if (animator.GetInteger (name)  != value) {
			animator.SetInteger (name, value);
			//if (!local) 
			//	syncAnim.EmitAnimatorInfo (name, value.ToString ());
		}
	}

	void SetTrigger(string name) 
	{
		animator.SetTrigger (name);
		//if (!local)
		//	syncAnim.EmitAnimatorInfo (name, "");
	}
}


