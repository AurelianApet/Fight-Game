using UnityEngine;
using System.Collections;

namespace game_core
{
	public class timeOutScene : MonoBehaviour {

			//VARIABLES
			public float 	timeOut		=	1.0f;
			public string 	sceneName	=	"menu";


			/// <summary>
			/// Use this for initialization 
			/// </summary>
			void Start () {

					Invoke ("sceneJump", timeOut);
			}

			//END SPLASH
			/// <summary>
			/// END SCENE.
			/// </summary>
			void sceneJump()
			{
					LevelManager.Load (sceneName);
			}
	}
}
