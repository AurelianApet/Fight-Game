using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace game_core
{
	/// <summary>
	/// Scenes.
	/// </summary>
	public enum Scenes
	{
			Default,
			splash,
			loading,
			story,
			menu,
			field,
			stats,
			shop,
			settings,
			credits,
	}

	[System.Serializable]
	/// <summary>
	/// Audio track.
	/// </summary>
	public class Track
	{
				public Scenes 		scene;
				public AudioClip 	clip;

				//TRACK SETTINGS
				public	Vector2		pitchRandomRange	=	new Vector2(0.95f,1.05f);
				public 	float		volume				=	1.0f;
				public 	float 		delayedStart 		= 	0.0f;
				public 	float		pitch				=	1.0f;
				public 	float		smoothDuration		=	0.0f;
				public 	bool		loop				=	false;
				public	bool 		randomPitch 		=	false;
				public 	bool 		smoothPlay			=	false;
				public	bool		smoothEnd			=	false;

				//PRIVATE CONTROL VARIABLES
				private bool		_isLoaded			=	false;

				/// <summary>
				/// Gets the key word.
				/// </summary>
				/// <value>The key word.</value>
				public 	string 		keyWord
				{
						get{ return scene.ToString (); }
				}

				/// <summary>
				/// Gets the name.
				/// </summary>
				/// <value>The name.</value>
				public string		Name
				{
						get{ return clip.name;}
				}

				/// <summary>
				/// Gets or sets a value indicating whether this <see cref="game_core.SoundTrack"/> is loaded.
				/// </summary>
				/// <value><c>true</c> if is loaded; otherwise, <c>false</c>.</value>
				public 	bool		isLoaded{
						get{ return		_isLoaded;}
						set{ _isLoaded 	= value;}
				}	
	}
	
	[System.Serializable]
	/// <summary>
	/// Channel.
	/// </summary>
	public class Channel
	{
				public int		channelID	=	0;

				private AudioSource _audioSource;
			
				/// <summary>
				/// Gets or sets the audio source.
				/// </summary>
				/// <value>The audio source.</value>
				public AudioSource audioSource
				{
						get{ return _audioSource; }
						set{ _audioSource = value;}
				}
	}

	/// <summary>
	/// Sound manager. 2D
	/// </summary>
	public class SoundManager : MonoBehaviour
	{
#region VARIABLES
			public				Track 			defaultTheme;
			public				Track[] 		soundTrack;
			public				Channel[]		channels;
	
			private 			AudioSource		_channelOne;
			private				AudioSource		_channelTwo;
			private				float			_duration			=	0.0f;
			private				float			_timePeriod			=	0.0f;
			private 			bool			_smoothSem			=	false;
			private				Track 			_trackForLoad		=	null;
			private				Track 			_loadedTrack;
			private				bool			_muteFlag			=	false;
			private 			FSMSys<SoundSystemTransition,SoundSystemStateID>			_fsm;
			private 			bool			_enable				=	true;
			private 			bool 			_isPaused=false;
			protected static SoundManager _instance;
#endregion
#region UNITY_CALLBACKS

				/// <summary>
				/// Gets the object instance.
				/// </summary>
				/// <value>The instance.</value>
				public static SoundManager Instance
				{
						get
						{
								if (_instance == null)
								{
										_instance =(SoundManager) FindObjectOfType(typeof(SoundManager));

										if ( FindObjectsOfType(typeof(SoundManager)).Length > 1 )
										{
											

												return _instance;
										}

										if (_instance == null)
										{
												GameObject singleton = new GameObject();
												_instance = singleton.AddComponent<SoundManager>();
												singleton.name = "(singleton) "+ typeof(SoundManager).ToString();
												DontDestroyOnLoad(singleton);
										}
								}

								return _instance;
						}
				}
			/// <summary>
			/// Use this for initialization
			/// </summary>
			void Awake () 
			{
						if (FindObjectsOfType (typeof(SoundManager)).Length > 1) {
								Destroy (gameObject);
						} else {
								channelOne = Instance.gameObject.AddComponent<AudioSource> ();
								channelTwo	=	Instance.gameObject.AddComponent<AudioSource> ();
								initSystem ();
								//AudioListener.volume	=	SettingsManager.sound;
								play (getTrackByScene (SceneManager.GetActiveScene ().name));
								DontDestroyOnLoad (gameObject);
						}
			}

			/// <summary>
			/// Raises the level was loaded event.
			/// </summary>
			/// <param name="level">Level.</param>
			void OnLevelWasLoaded(int level)
			{
						play (getTrackByScene(SceneManager.GetActiveScene ().name));
			}

			/// <summary>
			/// Fixeds the update.
			/// </summary>
			void Update()
			{
						SoundManager.channelOne.mute = (_muteFlag || !SettingsManager.music);
						_fsm.CurrentState.Reason ();
						_fsm.CurrentState.Act ();
			}
#endregion
#region PRIVATE_FUNCTIONS
			/// <summary>
			/// Gets the name of the clip by scene.
			/// </summary>
			/// <returns>The clip by scene name.</returns>
			/// <param name="sName">S name.</param>
			Track getTrackByScene(string sName)
			{
						if(Instance.soundTrack!=null){
					foreach(Track soundTrack in Instance.soundTrack)
					{
							if (soundTrack.keyWord == sName) 
							{
									return soundTrack;
							}
								}}
					return Instance.defaultTheme;
			}

			/// <summary>
			/// Gets the name of the clip by.
			/// </summary>
			/// <returns>The clip by name.</returns>
			/// <param name="name">Name.</param>
			Track getClipByName(string name)
			{
						if (Instance.soundTrack != null) {
								foreach (Track track in Instance.soundTrack) {
										if (track.Name == name) {
												return track;
										}
								}
						}
					return Instance.defaultTheme;		
			}

#endregion
#region PUBLIC_FUNCTIONS
			/// <summary>
			/// Play the specified clip.
			/// </summary>
			/// <param name="clip">Clip.</param>
			public static void play(Track clip)
			{

						if(clip!=null)
						{
								NewSong = clip;
								enable	= true;
								isPaused = false;		
						}
				
			} 

			/// <summary>
			/// Play the specified clip.
			/// </summary>
			/// <param name="clip">Clip.</param>
			public static void play(AudioClip clip)
			{
						isPaused = false;
						channelTwo.PlayOneShot(clip);
				
			}
			
				public static void play(){

						if (!channelOne.isPlaying) {
								
								isPaused = false;
						}

				}
			/// <summary>
			/// Stop this instance.
			/// </summary>
			public static void Stop()
			{
						if (FindObjectsOfType<SoundManager> ().Length > 0) 
						{
								enable	= false;
								channelOne.Stop ();
								channelTwo.Stop ();
								channelOne.clip = null;
								channelTwo.clip = null;
						}
			}

				public static void Pause(){
				
						if (FindObjectsOfType<SoundManager> ().Length > 0) 
						{
								//enable	= false;
								if (channelOne.isPlaying) {
										isPaused = true;

								}
						

						}
				}
			/// <summary>
			/// Mute the specified value.
			/// </summary>
			/// <param name="value">If set to <c>true</c> value.</param>
			public static void Mute(bool value)
			{
					if (FindObjectsOfType<SoundManager> ().Length > 0) 
					{
							Instance._muteFlag 	= 	value;
							channelOne.mute		=	value;
					}
			}
			
#endregion
			
#region FSM
			/// <summary>
			/// Sets the transition.
			/// </summary>
			/// <param name="t">T.</param>
			public static void SetTransition(SoundSystemTransition t) 
			{ 
					if(FindObjectsOfType<SoundManager>().Length>0)
					{
						Instance._fsm.PerformTransition(t);	
					}
			}

			/// <summary>
			/// Inits the process.
			/// </summary>
			void initSystem()
			{
					//1
					LoadTrack loadTrack=new LoadTrack(gameObject);
					loadTrack.AddTransition (SoundSystemTransition.wait,			SoundSystemStateID.wait);
					
					//3
					StartTrack startTrack=new StartTrack(gameObject);
					startTrack.AddTransition (SoundSystemTransition.playingTrack,	SoundSystemStateID.playingTrack);
					startTrack.AddTransition (SoundSystemTransition.pausedTrack,	SoundSystemStateID.pauseTrack);
					startTrack.AddTransition (SoundSystemTransition.loadTrack,		SoundSystemStateID.loadTrack);

					//4
					PlayingTrack playingTrack=new PlayingTrack(gameObject);
					playingTrack.AddTransition (SoundSystemTransition.pausedTrack,	SoundSystemStateID.pauseTrack);
					playingTrack.AddTransition (SoundSystemTransition.loadTrack,	SoundSystemStateID.loadTrack);

					//5
					WaitState waitState=new WaitState(gameObject);
					waitState.AddTransition (SoundSystemTransition.startTrack,		SoundSystemStateID.startTrack);
					waitState.AddTransition (SoundSystemTransition.loadTrack,		SoundSystemStateID.loadTrack);

					//6
					PauseTrack pauseTrack= new PauseTrack(gameObject);
					pauseTrack.AddTransition (SoundSystemTransition.playingTrack,	SoundSystemStateID.playingTrack);

					_fsm = new FSMSys<SoundSystemTransition,SoundSystemStateID>();

					//ADD STATES
					_fsm.AddState (loadTrack);
					_fsm.AddState (waitState);
					_fsm.AddState (startTrack);
					_fsm.AddState (playingTrack);
					_fsm.AddState (pauseTrack);
			}
#endregion
#region DATA
			
			/// <summary>
			/// Gets or sets the new song.
			/// </summary>
			/// <value>The new song.</value>
			public static Track NewSong{
					set{
						if(FindObjectsOfType<SoundManager>().Length<=1)
						{
								Instance._trackForLoad = value;
										if(		Instance._trackForLoad		!=null 	&& 
												Instance._loadedTrack		!=null 	&&
												Instance._loadedTrack.clip	!=null	&&
												Instance._trackForLoad.clip	!=null	&&
												Instance._trackForLoad.clip.name	==	Instance._loadedTrack.clip.name
										)
										{
												Instance._trackForLoad = null;
										}
						}	
					}
					get{ 
								return Instance._trackForLoad;
					}
			}
			
			/// <summary>
			/// Gets the loaded song.
			/// </summary>
			/// <value>The loaded song.</value>
			public static Track LoadedSong{
						get{
								return Instance._loadedTrack;
							}
						set{ 
								Instance._loadedTrack = value;
								if (Instance._loadedTrack != null) 
								{
										channelOne.mute 		=	Instance._muteFlag;
										channelOne.clip 		= 	Instance._loadedTrack.clip;
										channelOne.loop 		= 	Instance._loadedTrack.loop;
										//channelOne.volume 		=	SettingsManager.sound;
										channelOne.pitch		=	Instance._loadedTrack.pitch;
										NewSong 				= 	null;
								}
						}
			}

			/// <summary>
			/// Gets a value indicating whether this <see cref="game_core.SoundManager"/> new track.
			/// </summary>
			/// <value><c>true</c> if new track; otherwise, <c>false</c>.</value>
			public static bool NewTrack
			{
						get{
								if(FindObjectsOfType<SoundManager>().Length<=0){return false;}

								if(	channelOne.clip						!=null 			&& 
									Instance._trackForLoad				!=null 			&& 
									Instance._trackForLoad.clip			!=null 			&&
									Instance._trackForLoad.clip.name	== Instance._loadedTrack.clip.name )
								{
										return false;
								}
								return (Instance._trackForLoad!=null);
						}

			}

			/// <summary>
			/// Gets or sets the background theme.
			/// </summary>
			/// <value>The background theme.</value>
			public static AudioSource channelOne
			{
					get{ 
								if(Instance._channelOne==null)
								{
										Instance._channelOne=Instance.gameObject.AddComponent<AudioSource> ();
								}
								return Instance._channelOne;
						}
					set{ Instance._channelOne = value;}
			}

			/// <summary>
			/// Gets or sets the background theme.
			/// </summary>
			/// <value>The background theme.</value>
			public static AudioSource channelTwo
			{
					get{
								if(Instance._channelTwo==null)
								{
										Instance._channelTwo=Instance.gameObject.AddComponent<AudioSource> ();
								}
								return Instance._channelTwo;
						}
					set{ Instance._channelTwo = value;}
			}

			
			/// <summary>
			/// Gets or sets the time period.
			/// </summary>
			/// <value>The time period.</value>
			public static float timePeriod{
					get{return Instance._timePeriod; }
					set{ Instance._timePeriod=value;}
			}

			/// <summary>
			/// Gets or sets the duration.
			/// </summary>
			/// <value>The duration.</value>
			public static float duration{
					get{ return Instance._duration;}
					set{ Instance._duration = value;}
			}

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="game_core.SoundManager"/> smooth sem.
			/// </summary>
			/// <value><c>true</c> if smooth sem; otherwise, <c>false</c>.</value>
			public static bool smoothSem{
					get{ return Instance._smoothSem;}
					set{ Instance._smoothSem = value;}
			}

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="game_core.SoundManager"/> is enable.
			/// </summary>
			/// <value><c>true</c> if enable; otherwise, <c>false</c>.</value>
			public static bool enable
			{
					get{ return Instance._enable; }
					set{  
								
								Instance._enable = value;
						}
			}
			
				public static bool isPaused{
						get{ return Instance._isPaused;}
						set{ Instance._isPaused = value;}
				}
#endregion
	}
}