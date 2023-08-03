using UnityEngine;
using System.Collections;
using game_core;

/// <summary>
/// Blood meter model.
/// Interface used to pass data through
/// layers.
/// </summary>
public interface BloodMeterModel
{
		float 	MaxBlood		{ get; 			}
		float 	Measure			{ get; 	set;	}	
		float  	RelativeMeasure	{ get;	set;	}
}

/// <summary>
/// Blood meter.
/// GameObject used to detect the amount of blood
/// spread over the ring.
/// </summary>
public class BloodMeter : MonoBehaviour,BloodMeterModel {

	public 	float	maxBlood			=	25.0f;
	public 	float	bloodFactor			=	0.5f; 
	public 	float 	bloodDownFactor		=	0.25f;
	public 	float 	totalBlood			=	0.0f;
	
	/// <summary>
	/// Fixeds the update.
	/// </summary>
	void FixedUpdate()
	{

				Measure -= (TimeManager.deltaTime*bloodDownFactor);
	}
	
	/// <summary>
	/// Function called on collision enter
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnTriggerEnter2D(Collider2D coll) 
	{
		if(coll.tag=="blood")
		{
			Measure	+=	bloodFactor;
		}
	}

		public float MaxBlood{
				get{ return maxBlood;}
		}
	/// <summary>
	/// Gets or sets the measure.
	/// </summary>
	/// <value>The measure.</value>
	public float Measure
	{
		get{return totalBlood;}
		set{
						
			if(value<=0)
			{
				totalBlood=0.0f;
			}else{
				totalBlood=(value>maxBlood)?maxBlood:value;
			}
		}
	}

	/// <summary>
	/// Gets the relative measure.
	/// </summary>
	/// <value>The relative measure.</value>
	public float RelativeMeasure{
			get{ 
					if (maxBlood == 0 || totalBlood< 0 || maxBlood < 0 ) 
					{
							return 0f;
					}
					return	totalBlood / maxBlood;
			}
				set{ 
						totalBlood	= value * maxBlood;

				}
	}
}
