using UnityEngine;
using System.Collections;
using game_core;
using UnityEngine.UI;

/// <summary>
/// Fighter states.
/// </summary>
public enum CombatStates
{
		NullStateID 		= 	0,
		initializing		=	28,
		fighting			=	27,
		idle				=	1,
		walking				=	2,
		chargingRight		=	3,
		throwingRight		=	4,
		returningRight		=	5,
		dodgeRight			=	6,
		surprise			=	7,
		blocking			=	8,
		gettingHurtRight	=	9,
		groggyStart			=	10,
		groggy				=	11,
		groggyEnd			=	12,

		gameOver			=	14,

		chargingLeft		=	15,
		throwingLeft		=	16,
		returningLeft		=	17,
		dodgeLeft			=	18,
		gettingHurtLeft		=	19,
		evade				=	20,
		falling				=	21,
		knockOut			=	22,

		injuredStarting		=	23,
		injured				=	24,
		injuredEnding		=	25,
};

/// <summary>
/// Fighter transitions.
/// </summary>
public enum CombatTransitions
{
		NullTransition 					=	0,
		fightingTransition				=	26,
		idleTransition					=	1,
		walkingTransition				=	2,
		chargingRightTransition			=	3,
		throwingRightTransition			=	4,
		returningRightTransition		=	5,
		dodgeRightTransition			=	6,
		surpriseTransition				=	7,
		blockingTransition				=	8,
		gettingHurtRightTransition 		=	9,
		groggyStartTransition			=	10,
		groggyTransition				=	11,
		groggyEndingTransition			=	12,

		gameOverTransition				=	14,

		chargingLeftTransition			=	15,
		throwingLeftTransition			=	16,
		returningLeftTransition			=	17,
		dodgeLeftTransition				=	18,
		gettingHurtLeftTransition 		=	19,
		evadeTransition					=	20,
		fallingTransition				=	21,
		knockOutTransition				=	22,
		injuredStartingTransition		=	23,
		injuredTransition				=	24,
		injuredEndingTransition			=	25,
};

/// <summary>
/// Relative positions.
/// </summary>
public enum relativePositions
{
		up		=	0,
		down	=	1,
		right	=	2,
		left	=	3,

};

/// <summary>
/// Fighter model.
/// </summary>
public interface FighterModel
{
		float 			Health			{ get; 		}
		bool			IsHuman			{ get; set; }
		bool			IsActive		{ get; set; }
		Vector3			Position		{ get; set;	}
		int				TotalSucceedHits{ get; set;	}
		int				KnockOuts		{ get;		}
		bool			isDead			{ get; set;	}
		CombatStates 	currentState	{ get;		}
		bool			Enable			{ get; set;	}
		bool			OnHitSucceed	{ get; set;	}
		string			Tag				{ get; set;	}
		void 			SetTransition(CombatTransitions t);
		void			GoToInitialPosition();
}
		
/// <summary>
/// Fighter controller.
/// </summary>
public partial class  FighterController : MonoBehaviour,FighterModel {
		#region VARIABLES
	public 		Sprite[] 			avatars;
	public 		Sprite 				largeAvatar;
	public 		AudioClip[] 		punchSFX;
	public 		AudioClip[] 		throwingPunchSFX;
	public 		AudioClip[] 		blockingPunchSFX;
	public 		AudioClip[] 		groggyEffectsSFX;
	public 		int 				knockOutsLimit 			= 	3;
	public 		int 				hits4SuperPunch			=	5;
	public 		int 				injuryHits 				= 	3;
	public 		int 				punchRate				=	3;
	public 		int 				superPunchHits			=	4;
	public		float				health					=	0.0f;
	public 		float 				punchPower				=	5.0f;
	public 		float 				superPunchPower			=	10;
	public 		float 				speed					=	35f;
	public 		float 				precision				=	1.0f;
	public 		bool 				superPunchFlag 			= 	false;
	public 		bool 				humanFlag 				= 	false;
	public 		bool 				superPunchAlwaysOn 		= 	false;
	private 	float 					_maxHealth				=	0.0f;
	private 	int 					_knockOuts				=	0;
	private 	int 					_maxSuperPunchHits		=	0;
	private 	bool 					_isDead					=	false;
	private 	int 					_succeedHits 			= 	0;
	private 	int 					_totalSucceedHits		=	0;
	private 	float 					_maxX,_minX,_maxY,_minY	=	0.0f;
	private 	bool 					_hitOnTargetFlag		=	false;
	private 	int						_actionCounter			=	0;
	private 	int 					_rightActionCounter		=	0;
	private 	float 					_animSpeed				=	0.0f;//7.0f/((float)1.0f/punchRate)/24.0f;
	private 	Vector3 				_initialPosition		=	Vector3.zero;
	private 	FighterModel			_enemy;
	private 	GameObject 				_knockOutInfo;
	private 	Image 					avatarView;
	private 	GameObject 				healthBar;
	private 	GameObject 				powerBar;
	private 	GameObject				_playerOne;
	private 	Transform 				_countdown;
	private 	PunchController 		_punchOne,_punchTwo;
	private 	FighterInjuredBehaviour _fighterInjured;
	private 	Animator 				_animator;
	private		AudioSource				_audioSource;
	protected 	FSMSys<CombatTransitions,CombatStates> 	fsm;
	protected 	Vector3 								oldPosition	=	Vector3.zero;
		#endregion


		#region UNITY_CALLBACKS
		/// <summary>
		/// Use this for initialization
		/// </summary>
		void OnEnable () 
		{
				initializeVariables ();
		}

		/// <summary>
		/// Update is called once per frame
		/// </summary>
		void Update () 
		{

				fsm.CurrentState.Reason();
				fsm.CurrentState.Act ();

				AnimSync ();
		}

		/// <summary>
		/// Animation Sync.
		/// Plays the right animation based on player state.
		/// </summary>
		/// <returns><c>true</c> <c>false</c> otherwise.</returns>
		public bool AnimSync()
		{
				string stateName	=	fsm.CurrentStateID.ToString();

				if (!_animator.GetCurrentAnimatorStateInfo (0).IsName (fsm.CurrentStateID.ToString ())) 
				{
						isSuperPunchLoaded	=	((SucceedHits>=hits4SuperPunch) || superPunchAlwaysOn);
						_animator.speed		=	1.0f;

						switch (fsm.CurrentStateID) 
						{

						case CombatStates.injuredEnding:
								stateName = CombatStates.injuredEnding.ToString () ;

								break;
						case CombatStates.injuredStarting:
								stateName = CombatStates.injuredStarting.ToString () ;

								break;
						case CombatStates.injured:
								stateName = CombatStates.injured.ToString () ;

								break;
						case CombatStates.groggy:
								stateName 	= CombatStates.groggy.ToString ();

								break;
						case CombatStates.falling:
								stateName = CombatStates.falling.ToString () ;

								break;
						case CombatStates.knockOut:
								stateName = CombatStates.knockOut.ToString () ;

								break;
						case CombatStates.blocking:
								stateName 	= 	isSuperPunchLoaded?CombatStates.blocking.ToString ()+"SP":CombatStates.blocking.ToString ();

								break;
						case CombatStates.chargingLeft:
								stateName 	= 	isSuperPunchLoaded?CombatStates.chargingLeft.ToString ()+"SP":CombatStates.chargingLeft.ToString ();

								_animator.speed+=_animSpeed;
								break;
						case CombatStates.chargingRight:
								stateName 	= 	isSuperPunchLoaded?CombatStates.chargingRight.ToString ()+"SP":CombatStates.chargingRight.ToString ();

								_animator.speed+=_animSpeed;
								break;
						case CombatStates.throwingLeft:
								stateName 	= 	isSuperPunchLoaded?CombatStates.throwingLeft.ToString ()+"SP":CombatStates.throwingLeft.ToString ();

								_animator.speed+=_animSpeed;
								break;
						case CombatStates.throwingRight:
								stateName 	= 	isSuperPunchLoaded?CombatStates.throwingRight.ToString ()+"SP":CombatStates.throwingRight.ToString ();

								_animator.speed+=_animSpeed;
								break;
						case CombatStates.returningLeft:
								stateName 	= isSuperPunchLoaded?CombatStates.returningLeft.ToString ()+"SP":CombatStates.returningLeft.ToString ();

								_animator.speed+=_animSpeed;
								break;
						case CombatStates.returningRight:
								stateName 	= 	isSuperPunchLoaded?CombatStates.returningRight.ToString ()+"SP":CombatStates.returningRight.ToString ();

								_animator.speed+=_animSpeed;
								break;
						case CombatStates.dodgeRight:
						case CombatStates.dodgeLeft:
						case CombatStates.walking:
								stateName 	= 	isSuperPunchLoaded?CombatStates.walking.ToString ()+"SP":CombatStates.walking.ToString ();

								break;
						case CombatStates.gettingHurtLeft:
								stateName 	= 	CombatStates.gettingHurtLeft.ToString ();

								break;
						case CombatStates.gettingHurtRight:
								stateName = CombatStates.gettingHurtRight.ToString () ;

								break;
						case CombatStates.idle:
						default:
								
								stateName	= isSuperPunchLoaded? CombatStates.idle.ToString ()+"SP" : CombatStates.idle.ToString ();
								break;
						}

						_animator.Play (stateName);
				}

				return (_animator.GetCurrentAnimatorStateInfo (0).IsName (fsm.CurrentStateID.ToString ()));
		}
		#endregion

		#region FSM
		/// <summary>
		/// Sets the transition.
		/// </summary>
		/// <param name="t">T.</param>
		public void SetTransition(CombatTransitions t) 
		{ 
				fsm.PerformTransition(t); 
		}
				
		/// <summary>
		/// Initializes the variables.
		/// </summary>
		public void initializeVariables()
		{
				//8.0f  ANIMATION FRAMES NUMBER/ PUNCH DURATION / 24.0f FRAME RATE 
				_animSpeed			=	8.0f/(1.0f / (float)punchRate)/24.0f;
				_maxHealth 			= 	health;
				_maxSuperPunchHits	=	superPunchHits;
				superPunchFlag 		= 	superPunchAlwaysOn;

				//CHILD GAMEOBJECTS
				for(int i=0;i<transform.childCount;i++)
				{
						switch(transform.GetChild(i).name)
						{
						case "playerOne":
								_playerOne = transform.GetChild (i).gameObject;
								_playerOne.SetActive (IsHuman);
							
								break;
						case "injured":
								transform.GetChild (i).gameObject.SetActive (true);	
								_fighterInjured		=	transform.GetChild (i).GetComponent<FighterInjuredBehaviour> ();
								transform.GetChild (i).gameObject.SetActive (true);	
								break;
		
						case "glove_1":
								_punchOne			=	transform.GetChild(i).GetComponent<PunchController>();
								break;

						case "glove_2":
								_punchTwo			=	transform.GetChild(i).GetComponent<PunchController>();
								break;

						case "countdown":
								_countdown = transform.GetChild (i);
								break;
						
						}
				}

				//GAMEOBJECTS
				_knockOutInfo 		= 	GameObject.Find ("info");
				GameObject aEnemy 	= 	GameObject.Find ("avatar"+tag);
				healthBar			=	GameObject.Find ("healthBar"+tag);
				GameObject boundary	=	GameObject.Find ("boundary");

				//INITIALIZE GAMEOBJECTS
				KnockOutInfo 		= 	false;
				EffectivePower 		= 	punchPower;
				if(aEnemy!=null)
				{
						avatarView	=	aEnemy.GetComponent<Image> ();	
				}
				if(largeAvatar!=null)
				{
						GameObject versusAvatar	=	GameObject.Find ("fighter"+tag);
						if (versusAvatar != null) {
								versusAvatar.GetComponent<Image> ().sprite = largeAvatar;
						}
				}
				if(boundary!=null)
				{
						BoxCollider2D bCollider = boundary.GetComponent<BoxCollider2D> ();
						if(bCollider!=null)
						{
								_maxX 	= boundary.transform.position.x + (bCollider.size.x*0.5f);
								_minX	= boundary.transform.position.x - (bCollider.size.x*0.5f);
								_maxY	= boundary.transform.position.y + (bCollider.size.y*0.5f);
								_minY 	= boundary.transform.position.y - (bCollider.size.y*0.5f);
						}	
				}
				_initialPosition 	=	transform.position;
				_animator			=	GetComponent<Animator>();
				_audioSource		=	GetComponent<AudioSource> ();
				updateFighterHealth	();
				updateFighterPower 	();

				if (IsHuman) {
						
						initFSM ();		
				} else {
						initAI ();
				}
		}

		/// <summary>
		/// Inits the human behaviour
		/// </summary>
		void initFSM()
		{
				//FINITE STATE MACHINE
				fsm 				= new FSMSys<CombatTransitions,CombatStates> ();
				fsm.nullStateID 	= CombatStates.NullStateID;
				fsm.nullTransition 	= CombatTransitions.NullTransition;

				//IDLE STATE
				Idle idle=new Idle(this);
				idle.nullStateID 		= fsm.nullStateID;
				idle.nullTransition 	= fsm.nullTransition;
				idle.AddTransition (CombatTransitions.walkingTransition,CombatStates.walking);
				idle.AddTransition (CombatTransitions.groggyTransition,CombatStates.groggy);
				idle.AddTransition (CombatTransitions.chargingLeftTransition,CombatStates.chargingLeft);
				idle.AddTransition (CombatTransitions.chargingRightTransition,CombatStates.chargingRight);
				idle.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);
				idle.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);

				idle.AddTransition (CombatTransitions.blockingTransition,CombatStates.blocking);
				//WALKING STATE
				Walking walking=new Walking(this);
				walking.nullStateID 	= fsm.nullStateID;
				walking.nullTransition 	= fsm.nullTransition;
				walking.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);

				walking.AddTransition (CombatTransitions.walkingTransition,CombatStates.walking);
				walking.AddTransition (CombatTransitions.chargingLeftTransition,CombatStates.chargingLeft);
				walking.AddTransition (CombatTransitions.chargingRightTransition,CombatStates.chargingRight);
				walking.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);
				walking.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);


				//CHARGING
				ChargingLeft chargingLeft			= new ChargingLeft(this,(1f/punchRate/3.0f));
				chargingLeft.nullStateID 		= fsm.nullStateID;
				chargingLeft.nullTransition 	= fsm.nullTransition;
				chargingLeft.AddTransition (CombatTransitions.throwingLeftTransition,CombatStates.throwingLeft);
				chargingLeft.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);
				chargingLeft.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);


				ChargingRight chargingRight			= new ChargingRight(this,(1f/punchRate/3.0f));
				chargingRight.nullStateID 		= fsm.nullStateID;
				chargingRight.nullTransition 	= fsm.nullTransition;
				chargingRight.AddTransition (CombatTransitions.throwingRightTransition,CombatStates.throwingRight);
				chargingRight.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);
				chargingRight.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);

				//THROWING
				ThrowingLeft throwingLeft =new ThrowingLeft(this,(1f/punchRate/3.0f));
				throwingLeft.nullStateID 		= fsm.nullStateID;
				throwingLeft.nullTransition 	= fsm.nullTransition;
				throwingLeft.AddTransition (CombatTransitions.returningLeftTransition,CombatStates.returningLeft);
				throwingLeft.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);
				throwingLeft.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);

				//THROWING
				ThrowingRight throwingRight =new ThrowingRight(this,(1f/punchRate/3.0f));
				throwingRight.nullStateID 		= fsm.nullStateID;
				throwingRight.nullTransition 	= fsm.nullTransition;
				throwingRight.AddTransition (CombatTransitions.returningRightTransition,CombatStates.returningRight);
				throwingRight.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);
				throwingRight.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);
				//throwing.AddTransition (CombatTransitions.surpriseTransition,CombatStates.surprise);

				//RETURNING
				ReturningLeft returningLeft= new ReturningLeft(this,(1f/punchRate/3.0f));
				returningLeft.nullStateID 		= fsm.nullStateID;
				returningLeft.nullTransition 	= fsm.nullTransition;
				returningLeft.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);
				returningLeft.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);
				returningLeft.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);

				ReturningRight returningRight= new ReturningRight(this,(1f/punchRate/3.0f));
				returningRight.nullStateID 		= fsm.nullStateID;
				returningRight.nullTransition 	= fsm.nullTransition;
				returningRight.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);
				returningRight.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);
				returningRight.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);

				//GETTINGHURT
				GettingHurtLeft gettingHurt		=	new GettingHurtLeft(this);
				gettingHurt.nullStateID 		= 	fsm.nullStateID;
				gettingHurt.nullTransition 		= 	fsm.nullTransition;
				gettingHurt.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);
				gettingHurt.AddTransition (CombatTransitions.groggyTransition,CombatStates.groggy);



				GettingHurtRight gettingHurtRight		=	new GettingHurtRight(this);
				gettingHurtRight.nullStateID 			= 	fsm.nullStateID;
				gettingHurtRight.nullTransition 		= 	fsm.nullTransition;
				gettingHurtRight.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);
				gettingHurtRight.AddTransition (CombatTransitions.groggyTransition,CombatStates.groggy);


				Falling	falling = new Falling (this,0.16f);
				falling.nullStateID = fsm.nullStateID;
				falling.nullTransition = fsm.nullTransition;
				falling.AddTransition (CombatTransitions.knockOutTransition,CombatStates.knockOut);


				Groggy groggy 			= new Groggy (this,1f);
				groggy.nullStateID 		= fsm.nullStateID;
				groggy.nullTransition 	= fsm.nullTransition;
				groggy.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);
				groggy.AddTransition (CombatTransitions.fallingTransition,CombatStates.falling);
				groggy.AddTransition (CombatTransitions.groggyEndingTransition,CombatStates.groggyEnd);

				GroggyEnding groggyEnding = new GroggyEnding (this, 3f);
				groggyEnding.nullStateID = fsm.nullStateID;
				groggyEnding.nullTransition = fsm.nullTransition;
				groggyEnding.AddTransition (CombatTransitions.injuredStartingTransition,CombatStates.injuredStarting);

				InjuredStarting injuredStarting 	= new InjuredStarting (this,1f);
				injuredStarting.nullStateID 		= fsm.nullStateID;
				injuredStarting.nullTransition 	= fsm.nullTransition;
				injuredStarting.AddTransition (CombatTransitions.injuredTransition,CombatStates.injured);

				Injured injured 				= new Injured (this,10.0f);
				injured.nullStateID 			= fsm.nullStateID;
				injured.nullTransition 			= fsm.nullTransition;
				injured.AddTransition (CombatTransitions.injuredEndingTransition,CombatStates.injuredEnding);

				InjuredEnding injuredEnding = new InjuredEnding (this,5.3f);
				injuredEnding.nullStateID 	= fsm.nullStateID;
				injuredEnding.nullTransition 	= fsm.nullTransition;
				injuredEnding.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);
				injuredEnding.AddTransition (CombatTransitions.fallingTransition,CombatStates.falling);

				/*UNCOMMENT THIS TO BLOCK MOVEMENT*/
				/*Block block			= new Block(this);
				block.nullStateID 		= fsm.nullStateID;
				block.nullTransition 	= fsm.nullTransition;
				block.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);*/
				//FINITE STATE MACHINE

				fsm.AddState (idle);
				fsm.AddState (walking);

				/*UNCOMMENT THIS TO BLOCK MOVEMENT*/
				//fsm.AddState (block);

				fsm.AddState (chargingLeft);
				fsm.AddState (chargingRight);

				fsm.AddState (throwingLeft);
				fsm.AddState (throwingRight);
				fsm.AddState (returningLeft);
				fsm.AddState (returningRight);
				fsm.AddState (gettingHurt);
				fsm.AddState (gettingHurtRight);
				fsm.AddState (groggy);

				fsm.AddState (falling);

				fsm.AddState (groggyEnding);
				fsm.AddState (injuredStarting);
				fsm.AddState (injured);
				fsm.AddState (injuredEnding);

		}
		/// <summary>
		/// Inits the A.I. behaviour
		/// </summary>
		void initAI(){
			
				precision	=	Random.Range(0.5f,0.75f);
				if(precision < 0)
				{
						precision = 0.55f;
				}

				//FINITE STATE MACHINE
				fsm 					=	new FSMSys<CombatTransitions,CombatStates> ();
				fsm.nullStateID 		= 	CombatStates.NullStateID;
				fsm.nullTransition 		= 	CombatTransitions.NullTransition;



				//IDLE STATE
				IdleAI idle=new IdleAI(this);
				idle.nullStateID 		= fsm.nullStateID;
				idle.nullTransition 	= fsm.nullTransition;
				idle.AddTransition (CombatTransitions.walkingTransition,CombatStates.walking);
				idle.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);
				idle.AddTransition (CombatTransitions.chargingLeftTransition,CombatStates.chargingLeft);
				idle.AddTransition (CombatTransitions.chargingRightTransition,CombatStates.chargingRight);
				idle.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);
				idle.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);
				idle.AddTransition (CombatTransitions.groggyTransition,CombatStates.groggy);
				idle.AddTransition (CombatTransitions.blockingTransition,CombatStates.blocking);
				idle.AddTransition (CombatTransitions.dodgeRightTransition,CombatStates.dodgeRight);
				idle.AddTransition (CombatTransitions.dodgeLeftTransition,CombatStates.dodgeLeft);

				//CHASE STATE
				Walking walking=new Walking(this);
				walking.nullStateID 	= fsm.nullStateID;
				walking.nullTransition 	= fsm.nullTransition;

				walking.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);
				walking.AddTransition (CombatTransitions.walkingTransition,CombatStates.walking);
				walking.AddTransition (CombatTransitions.blockingTransition,CombatStates.blocking);
				walking.AddTransition (CombatTransitions.chargingLeftTransition,CombatStates.chargingLeft);
				walking.AddTransition (CombatTransitions.chargingRightTransition,CombatStates.chargingRight);
				walking.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);
				walking.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);
				walking.AddTransition (CombatTransitions.dodgeLeftTransition,CombatStates.dodgeLeft);
				walking.AddTransition (CombatTransitions.dodgeRightTransition,CombatStates.dodgeRight);


				//DODGE
				DodgeLeft dodge		= new DodgeLeft(this);
				dodge.nullStateID 		= fsm.nullStateID;
				dodge.nullTransition 	= fsm.nullTransition;
				dodge.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);
				dodge.AddTransition (CombatTransitions.chargingLeftTransition,CombatStates.chargingLeft);
				dodge.AddTransition (CombatTransitions.chargingRightTransition,CombatStates.chargingRight);
				dodge.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);
				dodge.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);

				//DODGE
				DodgeRight dodgeRight		= new DodgeRight(this);
				dodgeRight.nullStateID 		= fsm.nullStateID;
				dodgeRight.nullTransition 	= fsm.nullTransition;
				dodgeRight.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);
				dodgeRight.AddTransition (CombatTransitions.chargingRightTransition,CombatStates.chargingRight);
				dodgeRight.AddTransition (CombatTransitions.chargingLeftTransition,CombatStates.chargingLeft);
				dodgeRight.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);
				dodgeRight.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);

				//BLOCK
				Block block		= new Block(this);
				block.nullStateID 		= fsm.nullStateID;
				block.nullTransition 	= fsm.nullTransition;
				block.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);

				//CHARGING
				ChargingLeft charging			= new ChargingLeft(this,(1f/punchRate/3.0f));
				charging.nullStateID 		= fsm.nullStateID;
				charging.nullTransition 	= fsm.nullTransition;
				charging.AddTransition (CombatTransitions.throwingLeftTransition,CombatStates.throwingLeft);
				charging.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);
				charging.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);

				//CHARGING
				ChargingRight chargingRight			= new ChargingRight(this,(1f/punchRate/3.0f));
				chargingRight.nullStateID 		= fsm.nullStateID;
				chargingRight.nullTransition 	= fsm.nullTransition;
				chargingRight.AddTransition (CombatTransitions.throwingRightTransition,CombatStates.throwingRight);
				chargingRight.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);
				chargingRight.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);

				//THROWING
				ThrowingLeft throwing=new ThrowingLeft(this,(1f/punchRate/3.0f));
				throwing.nullStateID 		= fsm.nullStateID;
				throwing.nullTransition 	= fsm.nullTransition;
				throwing.AddTransition (CombatTransitions.returningLeftTransition,CombatStates.returningLeft);
				throwing.AddTransition (CombatTransitions.surpriseTransition,CombatStates.surprise);
				throwing.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);
				throwing.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);

				//THROWING
				ThrowingRight throwingRight=new ThrowingRight(this,(1f/punchRate/3.0f));
				throwingRight.nullStateID 		= fsm.nullStateID;
				throwingRight.nullTransition 	= fsm.nullTransition;
				throwingRight.AddTransition (CombatTransitions.returningRightTransition,CombatStates.returningRight);
				throwingRight.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);
				throwingRight.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);

				//RETURNING
				ReturningLeft returning= new ReturningLeft(this,(1f/punchRate/3.0f));
				returning.nullStateID 		= fsm.nullStateID;
				returning.nullTransition 	= fsm.nullTransition;
				returning.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);
				returning.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);
				returning.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);
				returning.AddTransition (CombatTransitions.surpriseTransition,CombatStates.surprise);

				//RETURNING
				ReturningRight returningRight	= new ReturningRight(this,(1f/punchRate/3.0f));
				returningRight.nullStateID 		= fsm.nullStateID;
				returningRight.nullTransition 	= fsm.nullTransition;
				returningRight.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);
				returningRight.AddTransition (CombatTransitions.gettingHurtLeftTransition,CombatStates.gettingHurtLeft);
				returningRight.AddTransition (CombatTransitions.gettingHurtRightTransition,CombatStates.gettingHurtRight);
				returningRight.AddTransition (CombatTransitions.surpriseTransition,CombatStates.surprise);

				//GETTINGHURT
				GettingHurtLeft gettingHurt		=new GettingHurtLeft(this);
				gettingHurt.nullStateID 	= fsm.nullStateID;
				gettingHurt.nullTransition 	= fsm.nullTransition;
				gettingHurt.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);

				//GETTINGHURT
				GettingHurtRight gettingHurtRight		=new GettingHurtRight(this);
				gettingHurtRight.nullStateID 	= fsm.nullStateID;
				gettingHurtRight.nullTransition 	= fsm.nullTransition;
				gettingHurtRight.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);
				gettingHurtRight.AddTransition (CombatTransitions.groggyTransition,CombatStates.groggy);

				Surprise surprise		= new Surprise(this,/*0.33f*(1f-precision)*/0f);
				surprise.nullStateID 	= fsm.nullStateID;
				surprise.nullTransition = fsm.nullTransition;
				surprise.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);

				Groggy groggy 			= new Groggy (this,1f);
				groggy.nullStateID 		= fsm.nullStateID;
				groggy.nullTransition 	= fsm.nullTransition;
				groggy.AddTransition (CombatTransitions.idleTransition,CombatStates.idle);
				groggy.AddTransition (CombatTransitions.fallingTransition,CombatStates.falling);

				Falling	falling = new Falling (this,0.16f);
				falling.nullStateID = fsm.nullStateID;
				falling.nullTransition = fsm.nullTransition;
				falling.AddTransition (CombatTransitions.knockOutTransition,CombatStates.knockOut);



				//FINITE STATE MACHINE

				fsm.AddState 	(idle);
				fsm.AddState 	(walking);
				fsm.AddState 	(dodge);
				fsm.AddState 	(dodgeRight);
				fsm.AddState 	(block);
				fsm.AddState 	(charging);
				fsm.AddState 	(chargingRight);
				fsm.AddState 	(throwing);
				fsm.AddState 	(throwingRight);
				fsm.AddState 	(returning);
				fsm.AddState 	(returningRight);
				fsm.AddState 	(gettingHurt);
				fsm.AddState 	(gettingHurtRight);
				fsm.AddState 	(surprise);
				fsm.AddState 	(groggy);
				fsm.AddState 	(falling);

		}
		#endregion
		#region FSM_HELPERS


		/// <summary>
		/// Plaies the effects.
		/// </summary>
		void playHurtEffects()
		{
				if(punchSFX.Length > 0)
				{
						_audioSource.pitch 	=  	1f;
						int randomIndex 	= 	Random.Range (0,(punchSFX.Length-1));	
						_audioSource.PlayOneShot (punchSFX[randomIndex]);
				}
		}
				
		/// <summary>
		/// Actions that the A.I. should execute based on precision var.
		/// </summary>
		/// <returns>The should be executed.</returns>
		/// <param name="precision">Precision.</param>
		CombatTransitions ActionDispatcher(float precision)
		{
				CombatTransitions 	cTransitionOut	=	CombatTransitions.idleTransition;
				float 				realPrecision 	= 	_actionCounter > 0?((float)_rightActionCounter / (float)_actionCounter):0.0f;
				if (onCollision) {
						switch (enemy.currentState) {
						case CombatStates.throwingLeft:
								cTransitionOut =	realPrecision > precision ? CombatTransitions.idleTransition : CombatTransitions.blockingTransition;
								if (realPrecision <= precision) {
										_rightActionCounter++;
								}
								_actionCounter++;
								break;
						case CombatStates.throwingRight:
								cTransitionOut =	realPrecision > precision ? CombatTransitions.idleTransition : CombatTransitions.blockingTransition;
								if (realPrecision <= precision) {
										_rightActionCounter++;
								}
								_actionCounter++;
								break;
						case CombatStates.chargingRight:
								cTransitionOut =	realPrecision > precision ? CombatTransitions.dodgeLeftTransition :  CombatTransitions.blockingTransition;
								if (realPrecision <= precision) {
										_rightActionCounter++;
								}
								_actionCounter++;
								break;
						case CombatStates.chargingLeft:
								cTransitionOut =	realPrecision > precision ? CombatTransitions.dodgeRightTransition : CombatTransitions.blockingTransition;
								if (realPrecision <= precision) {
										_rightActionCounter++;
								}
								_actionCounter++;
								break;
						case CombatStates.returningLeft:
								cTransitionOut =	realPrecision > precision ? CombatTransitions.dodgeRightTransition :CombatTransitions.dodgeLeftTransition;
								if (realPrecision <= precision) {
										_rightActionCounter++;
								}
								_actionCounter++;
								break;
						case CombatStates.returningRight:
								cTransitionOut =	realPrecision > precision ? CombatTransitions.dodgeLeftTransition : CombatTransitions.dodgeRightTransition;
								if (realPrecision <= precision) {
										_rightActionCounter++;
								}
								_actionCounter++;
								break;

						case CombatStates.idle:
								if (Random.Range (0.0f, 100.0f) > 50.0f) {
										cTransitionOut =	realPrecision > precision ? CombatTransitions.idleTransition : CombatTransitions.dodgeRightTransition;
								} else {
										cTransitionOut =	realPrecision > precision ? CombatTransitions.idleTransition : CombatTransitions.dodgeLeftTransition;
								}

								if (realPrecision <= precision) {
										_rightActionCounter++;
								}
								_actionCounter++;
								break;

						case CombatStates.walking:
								cTransitionOut =CombatTransitions.walkingTransition;
								if (realPrecision <= precision) {
										_rightActionCounter++;
								}
								_actionCounter++;

								break;

						default:
								cTransitionOut = CombatTransitions.idleTransition;
								break;
						}
				} else if (onPunchZone) {

						switch (enemy.currentState) {
						case CombatStates.chargingRight:
								cTransitionOut =	realPrecision > precision ? CombatTransitions.dodgeRightTransition : CombatTransitions.dodgeRightTransition;
								if (realPrecision <= precision) {
										_rightActionCounter++;
								}
								_actionCounter++;
								break;
						case CombatStates.chargingLeft:
								cTransitionOut =	realPrecision > precision ? CombatTransitions.dodgeLeftTransition : CombatTransitions.dodgeLeftTransition;
								if (realPrecision <= precision) {
										_rightActionCounter++;
								}
								_actionCounter++;
								break;
						case CombatStates.throwingLeft:
						case CombatStates.throwingRight:
								cTransitionOut =	realPrecision > precision ? CombatTransitions.idleTransition : CombatTransitions.blockingTransition;

								if (realPrecision <= precision) {
										_rightActionCounter++;
								}
								_actionCounter++;
								break;
						case CombatStates.walking:
						case CombatStates.idle:

								cTransitionOut = CombatTransitions.walkingTransition;
								if (realPrecision <= precision) {
										_rightActionCounter++;
								}
								_actionCounter++;

								break;

						default:
								cTransitionOut = CombatTransitions.idleTransition;
								if (realPrecision <= precision) {
										_rightActionCounter++;
								}
								_actionCounter++;
								break;
						}
				} else{
						cTransitionOut =CombatTransitions.walkingTransition;
						if (realPrecision <= precision) {
								_rightActionCounter++;
						}
						_actionCounter++;
				}
				if(Health<=0)
				{
						cTransitionOut	=	CombatTransitions.groggyTransition;
				}
				return cTransitionOut;
		}

		/// <summary>
		/// Looks at enemy.
		/// </summary>
		void lookAtEnemy(){

				Vector3 targetPos 			= (_enemy==null)? Vector3.zero : _enemy.Position;
				Quaternion targetRotation 	= Quaternion.LookRotation(transform.position - targetPos, Vector3.forward);
				targetRotation.x 			= 0.0f;
				targetRotation.y 			= 0.0f;
				transform.rotation 			= Quaternion.Slerp (transform.rotation, targetRotation, 5.0f *Time.deltaTime);
		}

		/// <summary>
		/// Moves the fighter to the initial position.
		/// </summary>
		public void GoToInitialPosition(){
				transform.position 			= _initialPosition;
		}

		/// <summary>
		/// To block an attack we need to know what side 
		/// the fighter is facing and is just what it does.
		/// Vector3.Dot returns 1 if they point in exactly 
		/// the same direction, -1 if they point in completely 
		/// opposite directions and zero if the vectors are 
		/// perpendicular.
		/// </summary>
		/// <returns>T.</returns>
		relativePositions WhatsUp()
		{
				float[] upDots = {
						Vector3.Dot (Vector3.up, transform.up),
						Vector3.Dot (Vector3.down, transform.up),
						Vector3.Dot (Vector3.up, transform.right),
						Vector3.Dot (Vector3.down, transform.right)
				};
				float 	max = upDots [0];
				int 	id 	= 0;
				for(int i=1;i<upDots.Length; i++){
						if(upDots[i]>max)
						{
								max = 	upDots [i];
								id	=	i;	
						}	
				}

				return (relativePositions)id;	
		}

		/// <summary>
		/// Updates the health bar status.
		/// </summary>
		public void updateFighterHealth()
		{
				if(avatarView!=null)
				{
						avatarView.GetComponent<Image> ().sprite = Avatar;	
				}
				HealthBar = RelativeHealth;

		}

		/// <summary>
		/// Updates the power bar status.
		/// </summary>
		public void updateFighterPower()
		{

				PowerBar = RelativePower;

		}
		/// <summary>
		/// This function receives a class called hitData.
		/// It contains the hit damage and what's colliding.
		/// </summary>
		/// <param name="hitData">hitData.</param>
		public void ApplyDamage(HitData hitData)
		{
				health -= hitData.damage;
				if(hitData.tag=="punchRight")
				{
						SetTransition (CombatTransitions.gettingHurtRightTransition);	
				}else{
						SetTransition (CombatTransitions.gettingHurtLeftTransition);
				}
				updateFighterHealth ();
		}

		/// <summary>
		/// Moves the fighter to the initial position.
		/// </summary>
		public void goToInitialPosition(){
				transform.position = _initialPosition;
				transform.rotation = Quaternion.Euler (Vector3.zero);
		}

		/// <summary>
		/// Plaies the groggy effects.
		/// </summary>
		public void playGroggyEffects(){
				if(groggyEffectsSFX.Length > 0)
				{
						_audioSource.pitch 	=	Random.Range(1f,1.3f);
						_audioSource.loop 	=	true;
						int randomIndex 	=	Random.Range (0,(groggyEffectsSFX.Length-1));	
						_audioSource.clip	=	groggyEffectsSFX[randomIndex];
						_audioSource.Play ();
				}
		}
		/// <summary>
		/// Stops the groggy effects.
		/// </summary>
		public void stopGroggyEffects(){
				_audioSource.Stop ();
				_audioSource.loop = false;
				_audioSource.clip = null;
		}

		/// <summary>
		/// Plaies the block effects.
		/// </summary>
		public void playBlockEffects(){
				if(blockingPunchSFX.Length > 0)
				{
						_audioSource.pitch 	= Random.Range(1f,1.3f);
						int randomIndex 	= Random.Range (0,(blockingPunchSFX.Length-1));	
						_audioSource.PlayOneShot(blockingPunchSFX[randomIndex]);
				}
		}
		/// <summary>
		/// Plaies the punch effects.
		/// </summary>
		public void playPunchEffects(){
				if(throwingPunchSFX.Length > 0)
				{
						_audioSource.pitch 	= Random.Range(0.75f,1.5f);
						int randomIndex 	= Random.Range (0,(throwingPunchSFX.Length-1));	
						_audioSource.PlayOneShot(throwingPunchSFX[randomIndex]);
				}
		}
		#endregion


		#region READ_ONLY_DATA
		/// <summary>
		/// Gets the axis h.
		/// </summary>
		/// <value>The axis h.</value>
		float axisH{
				get{ 
						if(humanFlag && InputManager.axisH !=0){ return (Mathf.Sign(InputManager.axisH)*1.0f);}
						if(!humanFlag && enemy!=null) {
								Vector3 direction 		= (enemy.Position-transform.position).normalized;

								return direction.x;
						}
						return 0.0f;
				}
		}

		/// <summary>
		/// Gets the axis v.
		/// </summary>
		/// <value>The axis v.</value>
		float axisV{
				get{ 
						if (humanFlag && InputManager.axisV != 0) {
								return (Mathf.Sign (InputManager.axisV) * 1.0f);
						} else if(!humanFlag && enemy!=null) {
								Vector3 direction 		= (enemy.Position-transform.position).normalized;

								return direction.y ;
						}
						return 0.0f;

				}

		}

		/// <summary>
		/// Gets the animator.
		/// </summary>
		/// <value>The animator.</value>
		Animator animator{
				get{ return _animator;}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="FighterController"/> on collision.
		/// </summary>
		/// <value><c>true</c> if on collision; otherwise, <c>false</c>.</value>
		bool onCollision{
				get{ 
						if(enemy!=null)
						{
								return (Vector3.Distance (Position, enemy.Position) <= 4.25f); 	
						}
						return false; 
				}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="FighterController"/> on punch zone.
		/// </summary>
		/// <value><c>true</c> if on punch zone; otherwise, <c>false</c>.</value>
		bool onPunchZone{
				get{ 
						if (enemy != null) {
								return (Vector3.Distance (Position, enemy.Position) <= 6.5f);
						}
						return false;
				}
		}

		/// <summary>
		/// Gets the state of the current.
		/// </summary>
		/// <value>The state of the current.</value>
		public CombatStates currentState{
				get{ return fsm.CurrentState.ID;}
		}


		/// <summary>
		/// Gets the enemy.
		/// </summary>
		/// <value>The enemy.</value>
		FighterModel enemy{
				get{ 
						if(_enemy==null)
						{
								FighterModel[] fighters = GameObject.FindObjectsOfType<FighterController> ();
								for(int i=0;i<fighters.Length;i++)
								{
										if (this !=(FighterController) fighters [i]) {
												_enemy = fighters [i];
										}
								}
						}
						return _enemy;
				}
		}

		/// <summary>
		/// Gets the max health.
		/// </summary>
		/// <value>The max health.</value>
		public float MaxHealth
		{
				get{ return _maxHealth; }
		}

		/// <summary>
		/// Gets the max super punch hits.
		/// </summary>
		/// <value>The max super punch hits.</value>
		public int MaxSuperPunchHits
		{
				get{ return _maxSuperPunchHits;}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="FighterController"/> on hit succeed.
		/// </summary>
		/// <value><c>true</c> if on hit succeed; otherwise, <c>false</c>.</value>
		public bool OnHitSucceed
		{
				get{ return onHitSucceed;}
				set{ 
						if (value) {
								SucceedHits++;
								TotalSucceedHits++;
						}
						onHitSucceed = value;
				}
		}
		/// <summary>
		/// Gets the health.
		/// </summary>
		/// <value>The health.</value>
		public float Health{
				get{ return health;}
		}

		/// <summary>
		/// Gets the relative power.
		/// </summary>
		/// <value>The relative power.</value>
		public float RelativePower{
				get{ 
						return Mathf.Clamp(((float)SucceedHits/(float)hits4SuperPunch),0f,1f);
				}
		}

		/// <summary>
		/// Gets the relative health.
		/// </summary>
		/// <value>The relative health.</value>
		public float RelativeHealth{
				get{ 
						return Mathf.Clamp( (Health / MaxHealth),0f,1f);
				}
		}
		/// <summary>
		/// Gets the injuries.
		/// </summary>
		/// <value>The injuries.</value>
		public int Injuries{
				get{ 
						if (_fighterInjured != null) {
								return _fighterInjured.Injuries;
						}
						return 0;

				}
		}

		/// <summary>
		/// Gets the avatar.
		/// </summary>
		/// <value>The avatar.</value>
		public Sprite Avatar
		{
				get{
						if (avatars.Length > 0) 
						{
								int 	factor	=	100 / (avatars.Length - 1);
								int 	index	=	((int)(100 - (Health/MaxHealth)*100)) / factor;
								return 	avatars[(index%avatars.Length)];
						}
						return null;
				}
		}
		#endregion
		#region EDITABLE_DATA

		/// <summary>
		/// Gets or sets a value indicating whether this instance is human.
		/// </summary>
		/// <value><c>true</c> if this instance is human; otherwise, <c>false</c>.</value>
		public bool IsHuman{
				get{ 
						return humanFlag;
				}
				set{ 
						if(_playerOne!=null)
						{
								_playerOne.SetActive (value);
						}
						humanFlag = value;
				}
		}

		/// <summary>
		/// Gets or sets the position.
		/// </summary>
		/// <value>The position.</value>
		public Vector3 Position{
				get{ return transform.position;}
				set{ transform.position = value;}
		}
				
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="FighterController"/> on hit succed.
		/// </summary>
		/// <value><c>true</c> if on hit succed; otherwise, <c>false</c>.</value>
		bool onHitSucceed{
				get{
						return _hitOnTargetFlag;
				}
				set{ 
						_hitOnTargetFlag = value;
				}
		}
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="FighterController"/> is dead.
		/// </summary>
		/// <value><c>true</c> if is dead; otherwise, <c>false</c>.</value>
		public bool isDead{
				get{ return _isDead;}
				set{ 
						_isDead = value;
				}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="FighterController"/> is super punch loaded.
		/// </summary>
		/// <value><c>true</c> if is super punch loaded; otherwise, <c>false</c>.</value>
		public bool isSuperPunchLoaded
		{
				get{
						return superPunchFlag;
				}
				set{ 
						superPunchFlag = value;
				}
		}

		/// <summary>
		/// Sets a value indicating whether this <see cref="FighterController"/> active injured.
		/// </summary>
		/// <value><c>true</c> if active injured; otherwise, <c>false</c>.</value>
		public bool activeInjured{
				set{ _fighterInjured.gameObject.SetActive (value);}
		}

		/// <summary>
		/// Sets the injured damage.
		/// </summary>
		/// <value>The injured damage.</value>
		public float InjuredDamage{
				set{ 
						if (_fighterInjured != null) {
								_fighterInjured.HitLimit = injuryHits;
								_fighterInjured.setInjuries (value);
						} 
				}
		}

		/// <summary>
		/// Sets the injured avatar.
		/// </summary>
		/// <value>The injured avatar.</value>
		public Sprite InjuredAvatar{
				set{ 
						_fighterInjured.Avatar	=	value;
				}
		}

		/// <summary>
		/// Gets or sets the total succeed hits.
		/// </summary>
		/// <value>The total succeed hits.</value>
		public int TotalSucceedHits
		{
				set{_totalSucceedHits = value;}
				get{ return _totalSucceedHits;}
		}

		/// <summary>
		/// Gets or sets the succeed hits.
		/// </summary>
		/// <value>The succeed hits.</value>
		public int SucceedHits{
				set{_succeedHits = value;}
				get{ return _succeedHits;}
		}

		/// <summary>
		/// Gets or sets the super punch hits.
		/// </summary>
		/// <value>The super punch hits.</value>
		public int SuperPunchHits{
				set{
						_maxSuperPunchHits= value;
						//if(_maxSuperPunchHits < 0){_maxSuperPunchHits = 0;}
				}
				get{ return _maxSuperPunchHits;}
		}

		/// <summary>
		/// Gets or sets the effective power.
		/// </summary>
		/// <value>The effective power.</value>
		public float EffectivePower{
				set{ 
						_punchOne.effectivePower = value;
						_punchTwo.effectivePower = value;
				}
				get{ 
						return _punchOne.effectivePower;
				}
		}

		/// <summary>
		/// Sets a value indicating whether this <see cref="FighterController"/> enable count down.
		/// </summary>
		/// <value><c>true</c> if enable count down; otherwise, <c>false</c>.</value>
		public bool EnableCountDown{

				set{ _countdown.gameObject.SetActive (value);}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="FighterController"/> is enable.
		/// </summary>
		/// <value><c>true</c> if enable; otherwise, <c>false</c>.</value>
		public bool Enable{
				set{ gameObject.SetActive (value);}
				get{ return gameObject.activeSelf;}
		}

		/// <summary>
		/// Gets or sets the tag.
		/// </summary>
		/// <value>The tag.</value>
		public string Tag{
				set{ gameObject.tag = value; }
				get{ return gameObject.tag;}
		}

		/// <summary>
		/// Sets the health bar.
		/// </summary>
		/// <value>The health bar.</value>
		public float HealthBar{
				set{ 

						if(healthBar==null)
						{
								healthBar=GameObject.Find ("healthBar"+tag);
						}
						if(healthBar!=null)
						{
								healthBar.transform.localScale= new Vector3(value,healthBar.transform.localScale.y,healthBar.transform.localScale.z);
						}
				}
		}

		/// <summary>
		/// Sets a value indicating whether this <see cref="FighterController"/> knock out info.
		/// </summary>
		/// <value><c>true</c> if knock out info; otherwise, <c>false</c>.</value>
		public bool KnockOutInfo{
				set{ 

						if(_knockOutInfo==null)
						{
								_knockOutInfo = GameObject.Find ("info");

						}
						if(_knockOutInfo!=null)
						{
								_knockOutInfo.SetActive (value);
						}
				}
		}

		/// <summary>
		/// Sets the power bar.
		/// </summary>
		/// <value>The power bar.</value>
		public float PowerBar{
				set{ 
						if(powerBar==null)
						{
								powerBar=GameObject.Find ("powerBar"+tag);

						}
						if(powerBar!=null)
						{
								powerBar.transform.localScale = new Vector3 (value,powerBar.transform.localScale.y,powerBar.transform.localScale.z);
						}
				}
		}

		/// <summary>
		/// Gets or sets the knock outs.
		/// </summary>
		/// <value>The knock outs.</value>
		public int KnockOuts{
				get{ return _knockOuts;}
				set{ _knockOuts	=	value;}
		}
		/// <summary>
		/// Gets or sets a value indicating whether this instance is active.
		/// </summary>
		/// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
		public bool IsActive{
				get{ return gameObject.activeSelf;}
				set{ gameObject.SetActive (value);}
		}
		#endregion









				






				





}

