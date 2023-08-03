using UnityEngine;
using System.Collections;

namespace game_core{
		
	/// <summary>
	/// Sound system transition.
	/// </summary>
	public enum SoundSystemTransition
	{
			//GENERIC STATE
			NullTransition 		=	0, // Use this transition to represent a non-existing transition in your system

			//PROCESS STATES
			loadTrack			=	1,
			wait				=	3,
			startTrack			=	4,
			playingTrack		=	5,
			pausedTrack			=	6,
	}

	public enum GameTheme
	{
			main		=	0,
			happy		=	1,
			sad			=	2,
			warning		=	3,
			danger		=	4,

	}
	/// <summary>
	/// Sound system state ID.
	/// </summary>
	public enum SoundSystemStateID
	{
			//GENERIC STATE
			NullStateID 		= 	0, // Use this ID to represent a non-existing State in your system	

			//PROCESS STATES
			loadTrack			=	1,
			wait				=	3,
			startTrack			=	4,
			playingTrack		=	5,
			pauseTrack			=	6,
	}
				
	/// <summary>
	/// unload Track.
	/// </summary>
	public class LoadTrack:State<SoundSystemTransition,SoundSystemStateID>
	{
			float volLevel		=	0.0f;
			float timePeriod	=	0.0f; 
			/// <summary>
			/// Initializes a new instance of the <see cref="LoadGame"/> class.
			/// </summary>
			/// <param name="gc">Gc.</param>
			public LoadTrack(GameObject gc){
					stateID 	= SoundSystemStateID.loadTrack;
			}

				/// <summary>
				/// This method decides if the state should transition to another on its list.
				/// </summary>
			public override void Reason()
			{
						if(SoundManager.NewSong!=null && SoundManager.enable && ( !SoundManager.channelOne.isPlaying || SoundManager.LoadedSong==null || !SoundManager.LoadedSong.smoothEnd || timePeriod>=1)  )
						{
								SoundManager.SetTransition (SoundSystemTransition.wait);
						}
			}

			/// <summary>
			/// This method controls the behavior of the object.
			/// </summary>
			public override void Act()
			{
						if(SoundManager.LoadedSong!=null && SoundManager.LoadedSong.smoothEnd)
						{
								volLevel 						= Mathf.Lerp (0.0f,SoundManager.LoadedSong.volume,timePeriod);	
								SoundManager.channelOne.volume 	= SoundManager.LoadedSong.volume - volLevel;
								if(timePeriod<= 1)
								{
										timePeriod+= TimeManager.deltaTime / SoundManager.LoadedSong.smoothDuration;
								}
								//Debug.Log ("TIME PERIOD "+timePeriod+" VOL LEVEL DECREASING "+volLevel+" VOL. "+SoundManager.channelOne.volume);
						}
			}

			/// <summary>
			/// This method is used to set up the State condition before entering it.
			/// It is called automatically by the FSMSystem class before assigning it
			/// to the current state.
			/// </summary>
			public override void DoBeforeEntering() 
			{
						timePeriod 	= 0.0f;
						volLevel 	= 0.0f;

			}

			/// <summary>
			/// This method is used to make anything necessary, as reseting variables
			/// before the FSMSystem changes to another one. It is called automatically
			/// by the FSMSystem before changing to a new state.
			/// </summary>
			public override void DoBeforeLeaving() 
			{
						//Debug.Log ("Current STATE Leaving"+this.ID.ToString());
					SoundManager.LoadedSong 		= 	SoundManager.NewSong;
			}
	}

	/// <summary>
	/// delay  track play if needed.
	/// </summary>
	public class WaitState:State<SoundSystemTransition,SoundSystemStateID>
	{
			
				float counter=0.0f;
			/// <summary>
			/// Initializes a new instance of the <see cref="LoadGame"/> class.
			/// </summary>
			/// <param name="gc">Gc.</param>
			public WaitState(GameObject gc){
					stateID 	= SoundSystemStateID.wait;
			}

				/// <summary>
				/// This method decides if the state should transition to another on its list.
				/// </summary>
			public override void Reason()
			{
					if(counter	>=	SoundManager.LoadedSong.delayedStart)
					{
						SoundManager.SetTransition (SoundSystemTransition.startTrack);
						return;
					}
					if(SoundManager.NewTrack || !SoundManager.enable)
					{
							SoundManager.SetTransition (SoundSystemTransition.loadTrack);
							return;
					}
			}

				/// <summary>
				/// This method controls the behavior of the object.
				/// </summary>
			public override void Act()
			{
						counter += TimeManager.deltaTime;
			}

			/// <summary>
			/// This method is used to set up the State condition before entering it.
			/// It is called automatically by the FSMSystem class before assigning it
			/// to the current state.
			/// </summary>
			public override void DoBeforeEntering() 
			{
						//Debug.Log ("Current STATE Entering"+this.ID.ToString());
						this.counter = 0.0f;
			}

			/// <summary>
			/// This method is used to make anything necessary, as reseting variables
			/// before the FSMSystem changes to another one. It is called automatically
			/// by the FSMSystem before changing to a new state.
			/// </summary>
			public override void DoBeforeLeaving() 
			{
						//Debug.Log ("Current STATE leaving"+this.ID.ToString());
			}
	}
		/// <summary>
		/// start Track.
		/// </summary>
		public class StartTrack:State<SoundSystemTransition,SoundSystemStateID>
		{
				float timePeriod=0.0f; 
				/// <summary>
				/// Initializes a new instance of the <see cref="LoadGame"/> class.
				/// </summary>
				/// <param name="gc">Gc.</param>
				public StartTrack(GameObject gc){
						stateID 	= SoundSystemStateID.startTrack;
				}

				/// <summary>
				/// This method decides if the state should transition to another on its list.
				/// </summary>
				public override void Reason()
				{
						if(TimeManager.isPaused)
						{
								SoundManager.SetTransition(SoundSystemTransition.pausedTrack);
								return;
						}
						if(	(!SoundManager.LoadedSong.smoothPlay) ||
							(SoundManager.channelOne.volume>=SoundManager.LoadedSong.volume)
						)
						{
								SoundManager.SetTransition (SoundSystemTransition.playingTrack);
								return;
					
						}
						if(SoundManager.NewTrack || !SoundManager.enable)
						{
								SoundManager.SetTransition(SoundSystemTransition.loadTrack);
								return;
						}

				}

				/// <summary>
				/// This method controls the behavior of the object.
				/// </summary>
				public override void Act()
				{
						if(SoundManager.LoadedSong.smoothPlay)
						{
							SoundManager.channelOne.volume	=	Mathf.Lerp (0.0f, SoundManager.LoadedSong.volume, timePeriod);
							if (timePeriod <= 1) {
									timePeriod += TimeManager.deltaTime / SoundManager.LoadedSong.smoothDuration;
							}
							//Debug.Log ("TIME PERIOD "+timePeriod+" VOL LEVEL INCREASING "+ SoundManager.channelOne.volume);
						}

				}
				/// <summary>
				/// This method is used to set up the State condition before entering it.
				/// It is called automatically by the FSMSystem class before assigning it
				/// to the current state.
				/// </summary>
				public override void DoBeforeEntering() 
				{
						//Debug.Log ("Current STATE Entering"+this.ID.ToString());
						timePeriod 						= 	0.0f;
						SoundManager.channelOne.volume 	= 	0.0f;
						SoundManager.channelOne.Play ();
				}

				/// <summary>
				/// This method is used to make anything necessary, as reseting variables
				/// before the FSMSystem changes to another one. It is called automatically
				/// by the FSMSystem before changing to a new state.
				/// </summary>
				public override void DoBeforeLeaving() 
				{
						//Debug.Log ("Current STATE Leaving"+this.ID.ToString());
						SoundManager.channelOne.volume=SoundManager.LoadedSong.volume;
				}
		}
		/// <summary>
		/// playing Track.
		/// </summary>
		public class PlayingTrack:State<SoundSystemTransition,SoundSystemStateID>
		{
				protected GameBehaviour gBehaviour;

				/// <summary>
				/// Initializes a new instance of the <see cref="LoadGame"/> class.
				/// </summary>
				/// <param name="gc">Gc.</param>
				public PlayingTrack(GameObject gc)
				{
						gBehaviour 	= gc.GetComponent<GameBehaviour>();
						stateID 	= SoundSystemStateID.playingTrack;
				}

				/// <summary>
				/// This method decides if the state should transition to another on its list.
				/// </summary>
				public override void Reason()
				{
						if (TimeManager.isPaused || SoundManager.isPaused) {
								SoundManager.SetTransition (SoundSystemTransition.pausedTrack);
								return;
						}
						if (!SoundManager.channelOne.isPlaying || SoundManager.NewTrack || !SoundManager.enable) 
						{
								SoundManager.SetTransition (SoundSystemTransition.loadTrack);
						}
				}

				/// <summary>
				/// This method controls the behavior of the object.
				/// </summary>
				public override void Act()
				{

				}

				/// <summary>
				/// This method is used to set up the State condition before entering it.
				/// It is called automatically by the FSMSystem class before assigning it
				/// to the current state.
				/// </summary>
				public override void DoBeforeEntering() 
				{
						//Debug.Log ("Current STATE Entering"+this.ID.ToString());
				}

				/// <summary>
				/// This method is used to make anything necessary, as reseting variables
				/// before the FSMSystem changes to another one. It is called automatically
				/// by the FSMSystem before changing to a new state.
				/// </summary>
				public override void DoBeforeLeaving() 
				{
						//Debug.Log ("Current STATE Leaving"+this.ID.ToString());
						SoundManager.channelOne.volume=SoundManager.LoadedSong.volume;
				}
		}
		/// <summary>
		/// pause Track.
		/// </summary>
		public class PauseTrack:State<SoundSystemTransition,SoundSystemStateID>
		{
				protected GameBehaviour gBehaviour;

				/// <summary>
				/// Initializes a new instance of the <see cref="LoadGame"/> class.
				/// </summary>
				/// <param name="gc">Gc.</param>
				public PauseTrack(GameObject gc){
						gBehaviour 	= gc.GetComponent<GameBehaviour>();
						stateID 	= SoundSystemStateID.pauseTrack;
				}

				/// <summary>
				/// This method decides if the state should transition to another on its list.
				/// </summary>
				public override void Reason()
				{
						if(!SoundManager.isPaused)
						{
								SoundManager.SetTransition (SoundSystemTransition.playingTrack);
						}
				}

				/// <summary>
				/// This method controls the behavior of the object.
				/// </summary>
				public override void Act()
				{

				}

				/// <summary>
				/// This method is used to set up the State condition before entering it.
				/// It is called automatically by the FSMSystem class before assigning it
				/// to the current state.
				/// </summary>
				public override void DoBeforeEntering() 
				{
						Debug.Log ("Current STATE Entering"+this.ID.ToString());
						SoundManager.channelOne.Pause ();
				}

				/// <summary>
				/// This method is used to make anything necessary, as reseting variables
				/// before the FSMSystem changes to another one. It is called automatically
				/// by the FSMSystem before changing to a new state.
				/// </summary>
				public override void DoBeforeLeaving() 
				{
						//Debug.Log ("Current STATE Leaving"+this.ID.ToString());
						SoundManager.channelOne.UnPause ();
				}

		}
}