using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
namespace game_core{

/// <summary>
/// Level manager class; Deals with level load 
/// transaction.
/// </summary>
public class LevelManager:MonoBehaviour{

	protected static LevelManager 	instance;
	protected static string 		levelName="";

	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>The instance.</value>
	public static LevelManager Instance
	{
		get
		{
			if(instance == null)
			{
				GameObject obj=new GameObject("LevelManager");
				instance =obj.AddComponent<LevelManager>();
				
				if (instance == null)
				{
					Debug.LogError("An instance of is needed in the scene, but there is none.");
				}
			}
			
			return instance;
		}
	}
	/// <summary>
	/// Gets or sets the loading level.
	/// </summary>
	/// <value>The loading level.</value>
	public static string loadingLevel
	{
		set{levelName=value;}
		get{return levelName;}
	}

	/// <summary>
	/// Load the specified name.
	/// </summary>
	/// <param name="name">Name.</param>
	public static void Load(string name) {
		TimeManager.isPaused 	= false;
		loadingLevel 			= name;
		Instance.StartCoroutine(Instance.InnerLoad(name));
	}

	/// <summary>
	/// Inners the load.
	/// </summary>
	/// <returns>The load.</returns>
	/// <param name="name">Name.</param>
	IEnumerator InnerLoad(string name) {
		//load transition scene
		Object.DontDestroyOnLoad(Instance.gameObject);
		SceneManager.LoadScene ("Loading");
		
		//wait one frame (for rendering, etc.)
		yield return null;
		
		//load the target scene
		SceneManager.LoadScene(name);
	}
}
}