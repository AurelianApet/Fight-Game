using UnityEngine;
using System.Collections;
using game_core;
using UnityEngine.UI;
using UnityEngine.Advertisements;
/// <summary>
/// Game transitions.
/// </summary>
public enum GameTransitions
{
		NullTransition 		=	0, // Use this transition to represent a non-existing transition in your system
		initCombat			=	1,
		startRound			=	2,
		fight				=	3,
		endRound			=	4,
		playerLoses			=	5,
		playerWins			=	6,
		limitRoundReached	=	7,
		gameOver			=	8,
}

/// <summary>
/// Game states.
/// </summary>
public enum GameStates
{
		NullStateID 		= 	0, // Use this ID to represent a non-existing State in your system	
		initCombat			=	1,
		startRound			=	2,
		fight				=	3,
		endRound			=	4,
		playerLoses			=	5,
		playerWins			=	6,
		limitRoundReached	=	7,
		gameOver			=	8,


}
		
/// <summary>
/// Game behaviour.
/// </summary>
public class GameBehaviour : MonoBehaviour 
{
	public		AudioClip[]							music;
	public		GameObject[] 						fighters;
	public 		bool 								dynamicCrowd 			= 	false;
	public 		Vector2 							angryCrowdRange			=	new Vector2(0f,0.25f);
	public 		Vector2 							excitedCrowdRange		=	new Vector2(0.7f,1f);
	public 		float 								initialBloodMeasure 	= 	0.0f;//CLAMPED BETWEEN 0-1
	public 		Vector3 							playerTwoPosition		=	new Vector3(0,7.47f,0);
	public 		Vector3 							playerOnePosition		=	new Vector3(0,-5.49f,0);
	public 		int 								maxRounds				=	10;
	public 		float 								roundDuration			=	20.0f;
	public		GameObject							roundGirl;
	public 		GameObject 							spectatorsExcited;
	public 		GameObject 							spectatorsAngry;
	public 		GameObject							bell;
	public 		GameObject							versus;
	public		GameObject 							win;
	public 		GameObject 							lose;
	public 		GameObject 							roundStats;
	public 		GameObject 							knockOutsoStats;
	public 		GameObject 							succeedHitsStats;
	public		GameObject 							gamePad;
	public 		GameObject 							gameOver;
	
	private		FighterModel						_playerONE;
	private 	FighterModel 						_mac;
	private 	FSMSys<GameTransitions,GameStates> 	_fsm;
	private 	int 								_round					=	0;
	private 	BloodMeterModel 					_bloodMeter;
	private		Text								_roundNumber;
		#region UNITY_CALLBACKS
	/// <summary>
	/// Use this for initialization
	/// </summary>
	void OnEnable () {

		InitVariables ();
		InitFSM ();
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () 
	{
		_fsm.CurrentState.Reason ();
		_fsm.CurrentState.Act ();

		//TIME CONTROL
		TimeManager.timeScale 	= 	(TimeManager.isPaused) ? 0 : 1;
		//SetActivePauseMenu(TimeManager.isPaused);
		TimeManager.time 		-= 	TimeManager.deltaTime;
	}
		/// <summary>
		/// Inits the variables.
		/// </summary>
		void InitVariables()
		{
				//Init
				if( fighters.Length > 0)
				{
						int index 		= 	(SettingsManager.selectedID % fighters.Length);
						//index 			= 	0;
						PlayerONE		=	 (FighterModel)((GameObject)Instantiate(fighters[index],playerOnePosition, Quaternion.identity)).GetComponent<FighterController>();
						PlayerONE.Tag	=	"Player";
						PlayerONE.IsHuman = true;
						PlayerONE.IsActive = true;
						index 			= 	(Random.Range (0, 100))%fighters.Length;
						//index 			= 	1;
						MAC				=	(FighterModel)((GameObject)Instantiate(fighters[index],playerTwoPosition, Quaternion.identity)).GetComponent<FighterController>();
						MAC.Tag			=	"Enemy";
						MAC.IsHuman 	= false;
						MAC.IsActive = true;

				}
				_bloodMeter		=	FindObjectOfType<BloodMeter> ();
		}

		/// <summary>
		/// Inits the FS.
		/// </summary>
		void InitFSM()
		{
				//init
				_fsm 					= new  FSMSys<GameTransitions , GameStates>();
				_fsm.nullStateID 		= GameStates.NullStateID;
				_fsm.nullTransition 	= GameTransitions.NullTransition;

				//STATE 1 INITCOMBAT
				//SHOW VS SCREEN
				InitCombat  initCombat 		= new  InitCombat(this,3f);
				initCombat.nullStateID 		= _fsm.nullStateID;
				initCombat.nullTransition 	= _fsm.nullTransition;
				initCombat.AddTransition (GameTransitions.startRound	,GameStates.startRound);

				//STATE 2 STARTROUND
				//SHOW ROUND GIRL
				//PLAY BELL
				//RESET TIMERS
				//SET FIGHTERS TO THE INITIAL POSITION
				startRound sRound 			= new startRound (this,4f);
				sRound.nullStateID 			= _fsm.nullStateID;
				sRound.nullTransition 		= _fsm.nullTransition;
				sRound.AddTransition (GameTransitions.fight		,GameStates.fight);

				//STATE 3 FIGHT
				//IF GROGGY STOP TIMER
				fight f 					= new fight (this,RoundDuration);
				f.nullStateID 				= _fsm.nullStateID;
				f.nullTransition 			= _fsm.nullTransition;
				f.AddTransition (GameTransitions.endRound			,GameStates.endRound);
				f.AddTransition (GameTransitions.playerLoses		,GameStates.playerLoses);
				f.AddTransition (GameTransitions.playerWins			,GameStates.playerWins);

				//STATE 4 ENDROUND
				//PLAY BELL
				//SET FIGHTERS TO THE INITIAL POSITION
				//RESTORE PROPORTIONAL HEALTH TO MAC IF FLAG IS ENABLED
				endRound eRound 			= new endRound (this,2f);
				eRound.nullStateID 			= _fsm.nullStateID;
				eRound.nullTransition 		= _fsm.nullTransition;
				eRound.AddTransition (GameTransitions.limitRoundReached	,GameStates.limitRoundReached);
				eRound.AddTransition (GameTransitions.startRound		,GameStates.startRound);
				eRound.AddTransition (GameTransitions.playerLoses		,GameStates.playerLoses);
				eRound.AddTransition (GameTransitions.playerWins		,GameStates.playerWins);

				//STATE 5 PLAYER LOSES
				//SHOW MSG
				PlayerLoses pLoses 			= new PlayerLoses (this);
				pLoses.nullStateID 			= _fsm.nullStateID;
				pLoses.nullTransition 		= _fsm.nullTransition;
				pLoses.AddTransition (GameTransitions.gameOver			,GameStates.gameOver);

				//STATE 6 PLAYER WINS
				//SHOW MSG
				PlayerWins pWins 			= new PlayerWins (this);
				pWins.nullStateID 			= _fsm.nullStateID;
				pWins.nullTransition 		= _fsm.nullTransition;
				pWins.AddTransition (GameTransitions.gameOver,GameStates.gameOver);

				//STATE 7 LIMITROUNDREACHED
				//SHOW MSG
				//PLAYER LOSES
				limitRoundReached lRReached = new limitRoundReached (this);
				lRReached.nullStateID 		= _fsm.nullStateID;
				lRReached.nullTransition 	= _fsm.nullTransition;
				lRReached.AddTransition (GameTransitions.gameOver,GameStates.gameOver);

				//STATE 8 GAMEOVER
				//SHOW MSG
				GameOver gameOver=new GameOver(this);
				gameOver.nullStateID 		= _fsm.nullStateID;
				gameOver.nullTransition 	= _fsm.nullTransition;

				//ADD STATES
				_fsm.AddState (initCombat);
				_fsm.AddState (sRound);
				_fsm.AddState (f);
				_fsm.AddState (eRound);
				_fsm.AddState (pLoses);
				_fsm.AddState (pWins);
				_fsm.AddState (lRReached);
				_fsm.AddState (gameOver);
		}
		#endregion
	
		#region FSM_HELPERS
		public void showInterstitialAds()
		{
				#if ENABLE_UNITYADS_RUNTIME
				if(Advertisement.isInitialized){Advertisement.Show ();}
				#endif
				AdmobManager.ShowInterstitial ();
		}
		/// <summary>
		/// Plaies the bell.
		/// </summary>
		public void playTheBell(){
				if (bell != null) 
				{
						
						bell.GetComponent<AudioSource> ().Play();

				}
		}
		/// <summary>
		/// Sets the crowd behaviour.
		/// </summary>
		public void setCrowdBehaviour(){
			if(_bloodMeter!=null)
			{
				float bLevel		=	RelativeBloodMeasure;
				if ( bLevel>=angryCrowdRange.x && bLevel< angryCrowdRange.y)
				{
						AngrySpectators 	= true;
						ExcitedSpectators 	= false;
				}//UNHAPPY
				if (bLevel >= angryCrowdRange.y 	&& 	bLevel<=excitedCrowdRange.x)
				{
						AngrySpectators 	= false;
						ExcitedSpectators 	= false;
				}//NORMAL
				if (bLevel > excitedCrowdRange.x &&	bLevel<= excitedCrowdRange.y) 
				{
						AngrySpectators 	= false;
						ExcitedSpectators 	= true;
				}//HAPPY
			}
		}
		/// <summary>
		/// Sets the game over stats.
		/// </summary>
		public void setGameOverStats(){
		
				if(knockOutsoStats!=null)
				{
						knockOutsoStats.GetComponent<Text> ().text = PlayerONE.KnockOuts.ToString("0000");		
				}

				if(roundStats!=null)
				{
						roundStats.GetComponent<Text> ().text = Round.ToString("0000");		
				}
				if(succeedHitsStats!=null)
				{
						succeedHitsStats.GetComponent<Text> ().text = PlayerONE.TotalSucceedHits.ToString("0000");	
				}
				if (gameOver != null) {
						gameOver.SetActive (true);
				}

		}
		public void loadCombatMusic(){
				if (music.Length > 0) {
						int index= Random.Range(0,(music.Length-1));
						soundEffects.clip=music[index];
					
				}
		}
		#endregion

		#region DATA
		/// <summary>
		/// Gets or sets the round.
		/// </summary>
		/// <value>The round.</value>
		public int Round{
				set{ _round = value;}
				get{ return _round;}
		}

		/// <summary>
		/// Sets a value indicating whether this <see cref="GameBehaviour"/> is versus.
		/// </summary>
		/// <value><c>true</c> if versus; otherwise, <c>false</c>.</value>
		public bool Versus{
				set{ 
						if (versus != null) {
								versus.SetActive (value);
						}
				}
		}

		/// <summary>
		/// Gets the max rounds.
		/// </summary>
		/// <value>The max rounds.</value>
		public int MaxRounds
		{
				get{return maxRounds; }
		}

		/// <summary>
		/// Gets the duration of the round.
		/// </summary>
		/// <value>The duration of the round.</value>
		public float RoundDuration{
				get{ return roundDuration;}
		}

		/// <summary>
		/// Gets or sets the player ON.
		/// </summary>
		/// <value>The player ON.</value>
		public FighterModel PlayerONE
		{
				set{ _playerONE=value;}
				get{ return _playerONE; }
		}

		/// <summary>
		/// Gets or sets the MA.
		/// </summary>
		/// <value>The MA.</value>
		public FighterModel MAC{
				set{  _mac = value;}
				get{ return _mac;}
		}

		/// <summary>
		/// Sets a value indicating whether this <see cref="GameBehaviour"/> round girl.
		/// </summary>
		/// <value><c>true</c> if round girl; otherwise, <c>false</c>.</value>
		public bool RoundGirl{
				set
				{
						if(roundGirl!=null)
						{
								roundGirl.SetActive (value);	
						}
				}
		}

		/// <summary>
		/// Sets a value indicating whether this <see cref="GameBehaviour"/> is window.
		/// </summary>
		/// <value><c>true</c> if window; otherwise, <c>false</c>.</value>
		public bool Win{
				set{ 
						if(win!=null)
						{
								win.SetActive (value);
						}
				}
		}

		/// <summary>
		/// Sets a value indicating whether this <see cref="GameBehaviour"/> is lose.
		/// </summary>
		/// <value><c>true</c> if lose; otherwise, <c>false</c>.</value>
		public bool Lose{
				set{ 
						if(lose!=null)
						{
								lose.SetActive (value);
						}
				}
		}

		/// <summary>
		/// Sets the round number.
		/// </summary>
		/// <value>The round number.</value>
		public string RoundNumber{
				set{
						if(_roundNumber==null)
						{
								GameObject aux	=	GameObject.Find ("roundNumber");	
								if(aux!=null){_roundNumber	=	aux.GetComponent<Text> ();}
						}
						if (_roundNumber !=null ) 
						{
								_roundNumber.text = value;
						}

				}

		}

		/// <summary>
		/// Sets a value indicating whether this <see cref="GameBehaviour"/> angry spectators.
		/// </summary>
		/// <value><c>true</c> if angry spectators; otherwise, <c>false</c>.</value>
		public bool AngrySpectators{
				set
				{
						if(spectatorsAngry!=null)
						{
								spectatorsAngry.SetActive (value);	
						}
				}
		}

		/// <summary>
		/// Sets a value indicating whether this <see cref="GameBehaviour"/> excited spectators.
		/// </summary>
		/// <value><c>true</c> if excited spectators; otherwise, <c>false</c>.</value>
		public bool ExcitedSpectators{
				set
				{
						if(spectatorsExcited!=null)
						{
								spectatorsExcited.SetActive (value);	
						}
				}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="GameBehaviour"/> dynamic crowd.
		/// </summary>
		/// <value><c>true</c> if dynamic crowd; otherwise, <c>false</c>.</value>
		public bool DynamicCrowd{
				get{ return dynamicCrowd;}
		}

		/// <summary>
		/// Gets or sets the relative blood measure.
		/// </summary>
		/// <value>The relative blood measure.</value>
		public float RelativeBloodMeasure{
				set{ 
						if(_bloodMeter!=null)
						{
								
								_bloodMeter.RelativeMeasure = Mathf.Clamp(value,0f,1f);	
						}
				}
				get{ 
						if(_bloodMeter!=null){ return _bloodMeter.RelativeMeasure;}
						return 0f;
				}
		}

		/// <summary>
		/// Sets a value indicating whether this <see cref="GameBehaviour"/> is crowd.
		/// </summary>
		/// <value><c>true</c> if crowd; otherwise, <c>false</c>.</value>
		public bool Crowd{
				set{ 
						AngrySpectators = ExcitedSpectators = value;
				}
		}

		/// <summary>
		/// Sets a value indicating whether this <see cref="GameBehaviour"/> game pad.
		/// </summary>
		/// <value><c>true</c> if game pad; otherwise, <c>false</c>.</value>
		public bool GamePad{
				get{ 
						return gamePad.activeSelf;
				}
				set{ 
						if(gamePad!=null)
						{
								gamePad.SetActive (value);
						}
				}
		}

		/// <summary>
		/// Gets the sound effects.
		/// </summary>
		/// <value>The sound effects.</value>
		public AudioSource soundEffects{
				get{ return GetComponent<AudioSource> ();}

		}
		#endregion
		#region FSM
	/// <summary>
	/// Sets the transition.
	/// </summary>
	/// <param name="t">T.</param>
	public void SetTransition(GameTransitions t) { _fsm.PerformTransition(t); }



	/// <summary>
	/// Init combat.
	/// </summary>

	public class InitCombat:State<GameTransitions,GameStates>
	{
			private 	GameBehaviour 	_parent;
			private 	float 			_duration 		= 	0.0f;
			private 	float 			_timeCounter	=	0.0f;
			public InitCombat(GameBehaviour parent,float duration	=	0.1f){
					_duration	=	duration;
					_parent		=	parent;
					stateID 	= 	GameStates.initCombat;
			}

			public override void Reason()
			{
						if(_timeCounter>=_duration)
						{
								_parent.SetTransition (GameTransitions.startRound);
						}				
			}

			public override void Act()
			{ 
						_parent.PlayerONE.Enable 	= 	true;
						_parent.MAC.Enable 			= 	true;
						_parent.MAC.GoToInitialPosition ();
						_parent.PlayerONE.GoToInitialPosition ();
						_timeCounter 				+= 	Time.deltaTime;
						_parent.Versus 				= 	true;
			}

			public override void DoBeforeEntering() 
			{
						_timeCounter = 0.0f;

			}
	
			public override void DoBeforeLeaving()
			{
						_parent.Versus 				= 	false;
			}
	}

		/// <summary>
		/// Start round.
		/// </summary>
		public class startRound:State<GameTransitions,GameStates>
		{
				private		GameBehaviour	_parent;
				private		float			_duration		=	0.0f;
				private		float 			_timeCounter	= 	0.0f;

				public startRound(GameBehaviour parent,float duration=0.1f){
						_duration	=	duration;
						_parent		=	parent;
						stateID 	= 	GameStates.startRound;
				}

			
				public override void Reason()
				{
						if(_parent.Round>=_parent.MaxRounds)
						{
								_parent.SetTransition(GameTransitions.limitRoundReached);
								return;		
						}
						if(_timeCounter>=_duration)
						{
								_parent.SetTransition (GameTransitions.fight);
								return;
						}
				}

			
				public override void Act()
				{
						_parent.MAC.GoToInitialPosition ();
						_parent.PlayerONE.GoToInitialPosition ();	
						_timeCounter += Time.deltaTime;
				}


				public override void DoBeforeEntering() 
				{
						_timeCounter = 0.0f;
						_parent.Round++;
						_parent.RoundNumber=_parent.Round.ToString ("00");

						//SHOW THE ROUND GIRL
						_parent.ExcitedSpectators=_parent.RoundGirl=true;


				}

			
				public override void DoBeforeLeaving()
				{
						// PLAY THE BELL
						_parent.playTheBell();
						//DISABLE THE ROUND GIRL
						_parent.ExcitedSpectators=_parent.RoundGirl=false;


				}
		}

		/// <summary>
		/// Fight.
		/// </summary>
		public class fight:State<GameTransitions,GameStates>
		{

				private 	GameBehaviour	_parent;
				private float _duration 	= 0.0f;
				private float _timeCounter 	= 0.0f;
				private bool _gamePadInitialStatus	=	false;
				public fight(GameBehaviour parent, float duration=0.1f){
						_duration	=	duration;
						_parent		=	parent;
						stateID 	= 	GameStates.fight;
				}


				public override void Reason()
				{
						if(_parent.PlayerONE.isDead)
						{
								_parent.SetTransition (GameTransitions.playerLoses);
								return;
						}
						if(_parent.MAC.isDead)
						{
								_parent.SetTransition (GameTransitions.playerWins);
								return;
						}
						if(_timeCounter>=_duration)
						{
								_parent.SetTransition (GameTransitions.endRound);
								return;
						}
				}


				public override void Act()
				{
						if (_parent.MAC.currentState != CombatStates.groggy &&
						   _parent.MAC.currentState != CombatStates.groggyEnd &&
						   _parent.PlayerONE.currentState != CombatStates.groggy &&
						   _parent.PlayerONE.currentState != CombatStates.groggyEnd &&
						   _parent.PlayerONE.currentState != CombatStates.injured &&
						   _parent.PlayerONE.currentState != CombatStates.injuredEnding &&
						   _parent.PlayerONE.currentState != CombatStates.injuredStarting) {
								if (!_parent.soundEffects.isPlaying) {
										_parent.soundEffects.Play ();
								}
								_parent.GamePad = _gamePadInitialStatus;
								_timeCounter += Time.deltaTime;
								if(_parent.DynamicCrowd)
								{
										_parent.setCrowdBehaviour ();

								}

						} else {
								_parent.Crowd 	= false;
								_parent.GamePad = false;
								if (_parent.soundEffects.isPlaying) {
										_parent.soundEffects.Pause ();
								}
						}

				}


				public override void DoBeforeEntering() 
				{ 
						_timeCounter	=	0.0f;
						_parent.Crowd 	= 	false;
						if(_parent.DynamicCrowd)
						{
								_parent.RelativeBloodMeasure = _parent.initialBloodMeasure;		
						}
						_parent.loadCombatMusic ();
						_gamePadInitialStatus	=	_parent.GamePad;
				}


				public override void DoBeforeLeaving() 
				{ 
						_parent.soundEffects.Stop ();
						_parent.playTheBell ();
						_parent.Crowd 	= 	false;
						_parent.GamePad	=	_gamePadInitialStatus;


				}
		}

		/// <summary>
		/// End round.
		/// </summary>
		public class endRound:State<GameTransitions,GameStates>
		{
			
				private		GameBehaviour	_parent;

				private float _duration 	= 	0.0f;
				private float _timeCounter	=	0.0f;
				public endRound(GameBehaviour parent,float duration=0.1f)
				{
						_duration	=	duration;
						_parent		=	parent;
						stateID 	= 	GameStates.endRound;
				}
						
				public override void Reason()
				{
						if(_parent.PlayerONE.isDead)
						{
								_parent.SetTransition (GameTransitions.playerLoses);
								return;
						}
						if(_parent.MAC.isDead)
						{
								_parent.SetTransition (GameTransitions.playerWins);
								return;
						}
						if(_timeCounter>=_duration)
						{
								_parent.SetTransition (GameTransitions.startRound);
								return;
						}

				}
			
				public override void Act()
				{
						_parent.MAC.GoToInitialPosition ();
						_parent.PlayerONE.GoToInitialPosition ();
						_timeCounter += Time.deltaTime;
				}


				public override void DoBeforeEntering() 
				{ 
						_timeCounter = 0.0f;
						_parent.ExcitedSpectators=true;
						//SHOW MSG
					

				}

				public override void DoBeforeLeaving()
				{
						
				}

		}

		/// <summary>
		/// PLayer Loses.
		/// </summary>
		public class PlayerLoses:State<GameTransitions,GameStates>
		{

				private		GameBehaviour	_parent;
				private		float 			_timeCounter 	= 	0.0f;
				private 	float 			_duration		=	1.0f;

				public PlayerLoses(GameBehaviour parent)
				{
						_parent=parent;
						stateID 	= 	GameStates.playerLoses;
				}

				public override void Reason()
				{
						if(_timeCounter>=_duration)
						{
								_parent.SetTransition (GameTransitions.gameOver);
								return;
						}

				}

				public override void Act()
				{

						_timeCounter += Time.deltaTime;
				}


				public override void DoBeforeEntering() 
				{ 
						//SHOW MSG
						int lost=PlayerPrefs.GetInt("superScore");
						lost++;
						PlayerPrefs.SetInt ("superScore",lost);
						_parent.AngrySpectators	=	true;
						_parent.Lose 			= 	true;
						_timeCounter 			= 	0.0f;
				}

				public override void DoBeforeLeaving()
				{

				}

		}

		/// <summary>
		/// Player Wins.
		/// </summary>
		public class PlayerWins:State<GameTransitions,GameStates>
		{

				private		GameBehaviour	_parent;
				private float _timeCounter 	= 0.0f;
				private float _duration 	= 1.0f;

				public PlayerWins(GameBehaviour parent)
				{
						_parent		=	parent;
						stateID 	= 	GameStates.playerWins;
				}

				public override void Reason()
				{
						if(_timeCounter>=_duration)
						{
								_parent.SetTransition (GameTransitions.gameOver);
								return;
						}
				}

				public override void Act()
				{
						_timeCounter += Time.deltaTime;

				}


				public override void DoBeforeEntering() 
				{ 
						int lost=PlayerPrefs.GetInt("score");
						lost++;
						PlayerPrefs.SetInt ("score",lost);
						//SHOW MSG
						_parent.ExcitedSpectators=true;
						_parent.Win 	= true;
						_timeCounter 	= 0.0f;

				}

				public override void DoBeforeLeaving()
				{

				}

		}



		/// <summary>
		/// The limit Round is Reached
		/// </summary>
		public class limitRoundReached:State<GameTransitions,GameStates>
		{

				private		GameBehaviour	_parent;
				private 	float 			_timeCounter 	= 0.0f;
				private 	float 			_duration 		= 1.0f;

				public limitRoundReached(GameBehaviour parent)
				{
						_parent=parent;
						stateID 	= 	GameStates.limitRoundReached;
				}

				public override void Reason()
				{
						if(_timeCounter>=_duration)
						{
								_parent.SetTransition (GameTransitions.gameOver);
								return;
						}
				}

				public override void Act()
				{

						_timeCounter += Time.deltaTime;
				}


				public override void DoBeforeEntering() 
				{ 
						//SHOW MSG
						_timeCounter 	= 0.0f;
				}

				public override void DoBeforeLeaving()
				{

				}

		}

		/// <summary>
		/// Game Over
		/// </summary>
		public class GameOver:State<GameTransitions,GameStates>
		{

				private		GameBehaviour	_parent;


				public GameOver(GameBehaviour parent)
				{
						_parent=parent;
						stateID 	= 	GameStates.gameOver;
				}

				public override void Reason()
				{

				}

				public override void Act()
				{


				}


				public override void DoBeforeEntering() 
				{ 

						//SHOW MSG
						_parent.loadCombatMusic();
						_parent.soundEffects.Play ();
						_parent.setGameOverStats ();
						_parent.GamePad = false;
				}

				public override void DoBeforeLeaving()
				{
					
				}

		}
		#endregion
	
}
