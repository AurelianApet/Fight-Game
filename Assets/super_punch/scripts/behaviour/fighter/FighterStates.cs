using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using game_core;
/// <summary>
/// Fighter controller.
/// </summary>
public partial class  FighterController : MonoBehaviour 
{
		/// <summary>
		/// Idle.
		/// </summary>
		class Idle:State<CombatTransitions,CombatStates>
		{
				private		FighterController	_parent;

				/// <summary>
				/// Initializes a new instance of the <see cref="startCombat"/> class.
				/// </summary>
				/// <param name="gc">Gc.</param>
				public Idle(FighterController parent)
				{
						_parent		=	parent;
						stateID 	= 	CombatStates.idle;
				}


				public override void Reason()
				{

						if(_parent.Health<=0.0f){
								_parent.SetTransition (CombatTransitions.groggyTransition);
								return;
						}
						if(Input.GetKeyDown(KeyCode.A) || InputManager.triggerFire2){
								_parent.SetTransition (CombatTransitions.chargingLeftTransition);
								return;
						}
						if(Input.GetKeyDown(KeyCode.S) || InputManager.triggerFire1){
								_parent.SetTransition (CombatTransitions.chargingRightTransition);
								return;
						}


						/*UNCOMMENT THIS TO BLOCK MOVEMENT*/
						/*if(_parent.humanFlag && (_parent.axisH!=0 || _parent.axisV!=0) && _parent.onCollision && (_parent.enemy.currentState==CombatStates.throwingLeft || _parent.enemy.currentState==CombatStates.throwingRight || _parent.enemy.currentState==CombatStates.chargingLeft  || _parent.enemy.currentState==CombatStates.chargingRight))
						{
								switch (_parent.WhatsUp ()) 
								{
								case relativePositions.up:
										if (Input.GetKey(KeyCode.DownArrow)) {
												//Debug.Log ("BLOCK");
												_parent.SetTransition (CombatTransitions.blockingTransition);
										}
										break;
								case relativePositions.down:
										if(Input.GetKey(KeyCode.UpArrow)){
												Debug.Log ("idle BLOCK");
												_parent.SetTransition (CombatTransitions.blockingTransition);
										}
										break;
								case relativePositions.left:

										if(Input.GetKey(KeyCode.LeftArrow))
										{
												_parent.SetTransition (CombatTransitions.blockingTransition);
												//Debug.Log ("BLOCK");
										}
										break;
								case relativePositions.right:

										if(Input.GetKey(KeyCode.RightArrow) ) 
										{
												_parent.SetTransition (CombatTransitions.blockingTransition);
										} 
										break;

								}

								return;	
						}*/
						if( _parent.axisH!=0 || _parent.axisV!=0)
						{
								//WALKING
								_parent.SetTransition(CombatTransitions.walkingTransition);

								return;
						}

				}


				public override void Act()
				{ 
						_parent.lookAtEnemy ();

				}

			
				public override void DoBeforeEntering() 
				{
						/*InputManager.triggerFire2 	= false;
						InputManager.triggerFire1 	= false;
						InputManager.axisH 			= 0f;
						InputManager.axisV 			= 0f;*/
				}

		
				public override void DoBeforeLeaving()
				{

				}
		}
		/// <summary>
		/// Walking.
		/// </summary>
		class Walking:State<CombatTransitions,CombatStates>
		{
				private		FighterController	_parent;
				private  	Vector3 			_nextPosition			=	Vector3.zero;

				public Walking(FighterController parent){
						_parent		=	parent;
						stateID 	= 	CombatStates.walking;
				}
						
				public override void Reason()
				{

						if (!_parent.humanFlag) {
								_parent.SetTransition(_parent.ActionDispatcher (_parent.precision));
						}
						if( _parent.axisH==0 && _parent.axisV==0)
						{
								//IDLE
								_parent.SetTransition(CombatTransitions.idleTransition);

								return;
								//IDLECOMBAT
						}

						if(_parent.humanFlag && (Input.GetKeyDown(KeyCode.A) || InputManager.triggerFire2)){
								_parent.SetTransition (CombatTransitions.chargingLeftTransition);
								return;

						}

						if(_parent.humanFlag && (Input.GetKeyDown(KeyCode.S) || InputManager.triggerFire1)){
								_parent.SetTransition (CombatTransitions.chargingRightTransition);
								return;

						}

						/*UNCOMMENT THIS TO BLOCK MOVEMENT*/
						/*if(_parent.humanFlag && (_parent.axisH!=0 || _parent.axisV!=0) && _parent.onCollision && (_parent.enemy.currentState==CombatStates.throwingLeft || _parent.enemy.currentState==CombatStates.throwingRight || _parent.enemy.currentState==CombatStates.chargingLeft  || _parent.enemy.currentState==CombatStates.chargingRight))
						{
								switch (_parent.WhatsUp ()) 
								{
								case relativePositions.up:
										if (Input.GetKey(KeyCode.DownArrow)) {
												//Debug.Log ("BLOCK");
												_parent.SetTransition (CombatTransitions.blockingTransition);
										}
										break;
								case relativePositions.down:
										if(Input.GetKey(KeyCode.UpArrow)){
												//Debug.Log ("BLOCK");
												_parent.SetTransition (CombatTransitions.blockingTransition);
										}
										break;
								case relativePositions.left:

										if(Input.GetKey(KeyCode.LeftArrow))
										{
												_parent.SetTransition (CombatTransitions.blockingTransition);
												//Debug.Log ("BLOCK");
										}
										break;
								case relativePositions.right:

										if(Input.GetKey(KeyCode.RightArrow) ) 
										{
												_parent.SetTransition (CombatTransitions.blockingTransition);
										} 
										break;

								}

								return;	
						}*/




				}
						
				public override void Act()
				{ 
						_parent.lookAtEnemy ();
						_nextPosition 	= _parent.Position;
						_nextPosition.x = _nextPosition.x + (_parent.axisH * .2f);
						_nextPosition.y = _nextPosition.y + (_parent.axisV * .2f);


						if(Vector3.Distance (_nextPosition, _parent.enemy.Position) < 3.5f)
						{
								_nextPosition=(_nextPosition-_parent.enemy.Position).normalized * (3.5f-Vector3.Distance (_nextPosition, _parent.enemy.Position))+ _nextPosition;
						}
						_nextPosition.x=Mathf.Clamp(_nextPosition.x,_parent._minX,_parent._maxX);
						_nextPosition.y=Mathf.Clamp(_nextPosition.y,_parent._minY,_parent._maxY);
						_parent.Position = Vector3.Lerp (_parent.Position, _nextPosition, _parent.speed * Time.deltaTime);

				}
						
				public override void DoBeforeEntering() 
				{


				}
						
				public override void DoBeforeLeaving()
				{

				}
		}





	/// <summary>
	/// Idle A.I
	/// </summary>
		class IdleAI:State<CombatTransitions,CombatStates>
		{
				private		FighterController	_parent;
				private 	float 				_timeCounter		=	0.0f;

				/// <summary>
				/// Initializes a new instance of the <see cref="startCombat"/> class.
				/// </summary>
				/// <param name="gc">Gc.</param>
				public IdleAI(FighterController parent)
				{
						_parent			=	parent;
						stateID 		= 	CombatStates.idle;

				}

				public override void Reason()
				{

						if (_parent.enemy != null) {
								_parent.SetTransition (_parent.ActionDispatcher (_parent.precision));
						}
				}

				public override void Act()
				{ 
						if(!_parent.humanFlag){_timeCounter += Time.deltaTime;}
						_parent.lookAtEnemy ();
				}


				public override void DoBeforeEntering() 
				{
						_timeCounter = 0.0f;
				}

				public override void DoBeforeLeaving()
				{

				}
		}

		/// <summary>
		/// Dodge right.
		/// </summary>
		class DodgeRight:State<CombatTransitions,CombatStates>
		{
				private		FighterController	_parent;
				private  	Vector3 			_nextPosition			=	Vector3.zero;
				private 	float 				_timeCounter			=	0.0f;
				private 	float 				_duration 				= 	0.35f;

				public DodgeRight(FighterController parent){
						_parent		=	parent;
						stateID 	= 	CombatStates.dodgeRight;
				}


				public override void Reason()
				{
						if(_timeCounter >=_duration && _parent.onCollision)
						{
								_parent.SetTransition(CombatTransitions.chargingLeftTransition);
								return;
						}
						if(_timeCounter >=_duration)
						{
								_parent.SetTransition(CombatTransitions.idleTransition);
								return;
						}
						}
								
				public override void Act()
				{ 
						_parent.lookAtEnemy ();
						_nextPosition			=	_parent.Position;
						_nextPosition.x 		+= 	(_parent.transform.right.x * .2f);
						_nextPosition.y 		+= 	(_parent.transform.right.y * .2f);
						if(Vector3.Distance (_nextPosition, _parent.enemy.Position) > 3.75f && _parent.onCollision )
						{
								_nextPosition	=	(_nextPosition-_parent.enemy.Position).normalized * (3.75f-Vector3.Distance (_nextPosition, _parent.enemy.Position))+ _nextPosition;
						}
						if(Vector3.Distance (_nextPosition, _parent.enemy.Position) < 3.75f  )
						{
								_nextPosition	=	(_nextPosition-_parent.enemy.Position).normalized * (3.75f-Vector3.Distance (_nextPosition, _parent.enemy.Position))+ _nextPosition;
						}
						_nextPosition.x			=	Mathf.Clamp(_nextPosition.x,_parent._minX,_parent._maxX);
						_nextPosition.y			=	Mathf.Clamp(_nextPosition.y,_parent._minY,_parent._maxY);
						_parent.Position		= 	Vector3.Lerp (_parent.Position, _nextPosition, _parent.speed * Time.deltaTime);
						_timeCounter 			+= 	Time.deltaTime;
				}
						
				public override void DoBeforeEntering() 
				{
						_parent.animator.SetInteger("stateID",(int)CombatStates.walking);
						_timeCounter 			= 	0.0f;

				}
										
				public override void DoBeforeLeaving()
				{

				}
		}

/// <summary>
/// Dodge left.
/// </summary>
class DodgeLeft:State<CombatTransitions,CombatStates>
{
		private		FighterController	_parent;
		private  	Vector3 			_nextPosition			=	Vector3.zero;
		private 	float 				_timeCounter			=	0.0f;
		private 	float 				_duration 				= 	0.35f;

		/// <summary>
		/// Initializes a new instance of the <see cref="startCombat"/> class.
		/// </summary>
		/// <param name="gc">Gc.</param>
		public DodgeLeft(FighterController parent){
				_parent		=	parent;
				stateID 	= 	CombatStates.dodgeLeft;
		}
				
		public override void Reason()
		{
				if(_timeCounter >=_duration && _parent.onCollision)
				{
						_parent.SetTransition(CombatTransitions.chargingRightTransition);
						return;
				}
				if(_timeCounter >=_duration)
				{
						_parent.SetTransition(CombatTransitions.idleTransition);
						return;
				}
		}


		public override void Act()
		{ 
				_parent.lookAtEnemy ();
				_nextPosition			=	_parent.Position;
				_nextPosition.x 		+= 	(_parent.transform.right.x * -.2f);
				_nextPosition.y 		+= 	(_parent.transform.right.y * -.2f);

				if(Vector3.Distance (_nextPosition, _parent.enemy.Position) > 3.75f && _parent.onCollision )
				{
						_nextPosition	=	(_nextPosition-_parent.enemy.Position).normalized * (3.75f-Vector3.Distance (_nextPosition, _parent.enemy.Position))+ _nextPosition;
					
				}
				if(Vector3.Distance (_nextPosition, _parent.enemy.Position) < 3.75f  )
				{
						_nextPosition	=	(_nextPosition-_parent.enemy.Position).normalized * (3.75f-Vector3.Distance (_nextPosition, _parent.enemy.Position))+ _nextPosition;
				}
				_nextPosition.x			=	Mathf.Clamp		(_nextPosition.x,_parent._minX,_parent._maxX);
				_nextPosition.y			=	Mathf.Clamp		(_nextPosition.y,_parent._minY,_parent._maxY);
				_parent.Position		= 	Vector3.Lerp	(_parent.Position, _nextPosition, _parent.speed * Time.deltaTime);
				_timeCounter 			+= 	Time.deltaTime;
		}

		public override void DoBeforeEntering() 
		{
				_parent.animator.SetInteger("stateID",(int)CombatStates.walking);
				_timeCounter 	= 	0.0f;
			
		}


		public override void DoBeforeLeaving()
		{

		}
}

/// <summary>
/// Block.
/// </summary>
class Block:State<CombatTransitions,CombatStates>
{
		private		FighterController	_parent;

		/// <summary>
		/// Initializes a new instance of the <see cref="startCombat"/> class.
		/// </summary>
		/// <param name="gc">Gc.</param>
		public Block(FighterController parent){
				_parent		=	parent;
				stateID 	= 	CombatStates.blocking;
		}
				
		public override void Reason()
		{


				if( 	_parent.humanFlag 													&& 
						(
							(	_parent.axisH				==	0 							&& 
								_parent.axisV				==	0
							) 																||  
							(	_parent.enemy.currentState	!=	CombatStates.throwingLeft 	&& 
								_parent.enemy.currentState	!=	CombatStates.chargingLeft 	&& 
								_parent.enemy.currentState	!=	CombatStates.throwingRight 	&& 
								_parent.enemy.currentState	!=	CombatStates.chargingRight
							)
						)
				)
				{
						_parent.SetTransition (CombatTransitions.idleTransition);	
						return;
				}
						if( !_parent.humanFlag && _parent.enemy.currentState!=CombatStates.throwingRight && _parent.enemy.currentState!=CombatStates.chargingRight && _parent.enemy.currentState!=CombatStates.throwingLeft  && _parent.enemy.currentState!=CombatStates.chargingLeft ){
						_parent.SetTransition(CombatTransitions.idleTransition);

				}

		}

		public override void Act()
		{ 
				_parent.lookAtEnemy ();

		}

		public override void DoBeforeEntering() 
		{
				_parent.playBlockEffects();	
		}


		public override void DoBeforeLeaving()
		{

		}
}
/// <summary>
/// Charging right.
/// </summary>
class ChargingRight:State<CombatTransitions,CombatStates>
{
		private		FighterController	_parent;
		private 	float 				_timeCounter	=	0.0f;
		private		float 				_duration 		= 	0.0f;

		/// <summary>
		/// Initializes a new instance of the <see cref="startCombat"/> class.
		/// </summary>
		/// <param name="gc">Gc.</param>
		public ChargingRight(FighterController parent, float duration){
				_parent		=	parent;
				_duration	=	duration;
				stateID 	= 	CombatStates.chargingRight;
		}
				
		public override void Reason()
		{
				if(_timeCounter>=_duration){

						_parent.SetTransition (CombatTransitions.throwingRightTransition);
				}
		}

		public override void Act()
		{ 
				_timeCounter += Time.deltaTime;								
		}
		public override void DoBeforeEntering() 
		{
				_timeCounter = 0.0f;
				_parent.onHitSucceed = false;
				//REFACTORIZE THIS ON A PARENT FUNCTION
				if( _parent.isSuperPunchLoaded)
				{
						_parent.EffectivePower	=	_parent.superPunchPower;
						_parent.SuperPunchHits--;
				}

				if (_parent.isSuperPunchLoaded && _parent.SuperPunchHits < 0 && !_parent.superPunchAlwaysOn) 
				{
						_parent.SucceedHits 	= 	0;
						_parent.SuperPunchHits 	= 	_parent.superPunchHits;
						_parent.EffectivePower 	= 	_parent.punchPower;
				}

		}

		public override void DoBeforeLeaving()
		{

		}
}

/// <summary>
/// Charging left.
/// </summary>
class ChargingLeft:State<CombatTransitions,CombatStates>
{
		private		FighterController	_parent;
		private float _timeCounter	=	0.0f;
		private float _duration 	= 	0.0f;

		/// <summary>
		/// Initializes a new instance of the <see cref="startCombat"/> class.
		/// </summary>
		/// <param name="gc">Gc.</param>
		public ChargingLeft(FighterController parent,float duration)
		{
				_parent		=	parent;
				_duration	=	duration;
				stateID 	= 	CombatStates.chargingLeft;
		}
				
		public override void Reason()
		{
				if(_timeCounter>=_duration){

						_parent.SetTransition (CombatTransitions.throwingLeftTransition);
				}
		}
				
		public override void Act()
		{ 
				_timeCounter += Time.deltaTime;	
		}

		public override void DoBeforeEntering() 
		{
				_timeCounter = 0.0f;
				_parent.onHitSucceed = false;

				//REFACTORIZE THIS ON A PARENT FUNCTION
				if( _parent.isSuperPunchLoaded)
				{
						_parent.EffectivePower	=	_parent.superPunchPower;
						_parent.SuperPunchHits--;
				}

				if (_parent.isSuperPunchLoaded && _parent.SuperPunchHits < 0 && !_parent.superPunchAlwaysOn) 
				{
						_parent.SucceedHits 	= 	0;
						_parent.SuperPunchHits 	= 	_parent.superPunchHits;
						_parent.EffectivePower 	= 	_parent.punchPower;
				}
		}
				
		public override void DoBeforeLeaving()
		{

		}

}

/// <summary>
/// Throwing right.
/// </summary>
class ThrowingRight:State<CombatTransitions,CombatStates>
{
		private	FighterController	_parent;
		private float 				_timeCounter	=	0.0f;
		private float				_duration		=	0.0f;
		/// <summary>
		/// Initializes a new instance of the <see cref="startCombat"/> class.
		/// </summary>
		/// <param name="gc">Gc.</param>
		public ThrowingRight(FighterController parent, float duration){
				_parent		=	parent;
				_duration	=	duration;
				stateID 	= 	CombatStates.throwingRight;
		}
				
		public override void Reason()
		{

				if( _timeCounter>=_duration){

						_parent.SetTransition (CombatTransitions.returningRightTransition);
						return;
				}
		}
				
		public override void Act()
		{ 
				_timeCounter += Time.deltaTime;	
		}
		public override void DoBeforeEntering() 
		{
				_timeCounter = 0.0f;
				_parent.playPunchEffects ();
		}
				
		public override void DoBeforeLeaving()
		{
				_parent.updateFighterPower ();
		}
	
}



/// <summary>
/// Throwing left.
/// </summary>
class ThrowingLeft:State<CombatTransitions,CombatStates>
{
		private	FighterController	_parent;
		private float 				_timeCounter	=	0.0f;
		private float				_duration		=	0.0f;
		/// <summary>
		/// Initializes a new instance of the <see cref="startCombat"/> class.
		/// </summary>
		/// <param name="gc">Gc.</param>
		public ThrowingLeft(FighterController parent,float duration){
				_parent		=	parent;
				_duration	=	duration;
				stateID 	= 	CombatStates.throwingLeft;	
				//Debug.Log("Duration "+_duration);
		}
				
		public override void Reason()
		{

				if( _timeCounter>=_duration){

						_parent.SetTransition (CombatTransitions.returningLeftTransition);
						return;
				}
		}
				
		public override void Act()
		{ 
				_timeCounter += Time.deltaTime;	
		}
		public override void DoBeforeEntering() 
		{
				_timeCounter = 0.0f;
				_parent.playPunchEffects ();
		}
				
		public override void DoBeforeLeaving()
		{

				_parent.updateFighterPower ();
		}
		/// <summary>
		/// Gets or sets the duration.
		/// </summary>
		/// <value>The duration.</value>
		public float duration{
				set{ _duration = value;}
				get{ return _duration;}
		}
}

/// <summary>
/// Returning right.
/// </summary>
class ReturningRight:State<CombatTransitions,CombatStates>
{
		private		FighterController	_parent;
		private 	float 				_timeCounter	=	0.0f;
		private 	float 				_duration		=	0.0f;
		/// <summary>
		/// Initializes a new instance of the <see cref="startCombat"/> class.
		/// </summary>
		/// <param name="gc">Gc.</param>
		public ReturningRight(FighterController parent, float duration){
				_parent		=	parent;
				_duration	= 	duration;
				stateID 	= 	CombatStates.returningRight;
		}
				
		public override void Reason()
		{
				if(!_parent.humanFlag && _timeCounter>=_duration && !_parent.onHitSucceed)
				{
						_parent.SetTransition (CombatTransitions.surpriseTransition);		
						return;
				}
				if(!_parent.humanFlag && _timeCounter>=_duration){
						_parent.SetTransition (CombatTransitions.idleTransition);
						return;

				}
				if(_timeCounter>=_duration){

						_parent.SetTransition (CombatTransitions.idleTransition);
				}
		}
				
		public override void Act()
		{ 
				_parent.lookAtEnemy ();
				_timeCounter += Time.deltaTime;
		}
		public override void DoBeforeEntering() 
		{
				_timeCounter = 0.0f;

		}

		public override void DoBeforeLeaving()
		{
						
		}
		/// <summary>
		/// Gets or sets the duration.
		/// </summary>
		/// <value>The duration.</value>
		public float duration{
				set{ _duration = value;}
				get{ return _duration;}
		}
}

/// <summary>
/// Returning left.
/// </summary>
class ReturningLeft:State<CombatTransitions,CombatStates>
{
		private		FighterController	_parent;
		private 	float 				_timeCounter	=	0.0f;
		private 	float 				_duration		=	0.0f;
		/// <summary>
		/// Initializes a new instance of the <see cref="startCombat"/> class.
		/// </summary>
		/// <param name="gc">Gc.</param>
		public ReturningLeft(FighterController parent, float duration){
				_parent		=	parent;
				_duration	=	duration;
				stateID 	= 	CombatStates.returningLeft;
		}
				
		public override void Reason()
		{
				if(!_parent.humanFlag && _timeCounter>=_duration && !_parent.onHitSucceed)
				{
						_parent.SetTransition (CombatTransitions.surpriseTransition);		
						return;
				}
				if(!_parent.humanFlag && _timeCounter>=_duration){
						_parent.SetTransition (CombatTransitions.idleTransition);
						return;

				}
				if(_timeCounter>=_duration){

						_parent.SetTransition (CombatTransitions.idleTransition);
				}
		}
				
		public override void Act()
		{ 
				_parent.lookAtEnemy ();
				_timeCounter	+= Time.deltaTime;
		}
		public override void DoBeforeEntering() 
		{
				_timeCounter = 0.0f;



		}
				
		public override void DoBeforeLeaving()
		{
						
		}

		/// <summary>
		/// Gets or sets the duration.
		/// </summary>
		/// <value>The duration.</value>
		public float duration{
				set{ _duration = value;}
				get{ return _duration;}
		}
}



/// <summary>
/// Surprise.
/// </summary>
class Surprise:State<CombatTransitions,CombatStates>
{
		private		FighterController	_parent;
		private		float 				_timeCounter = 0.0f;
		private float _duration = 0.0f;
		/// <summary>
		/// Initializes a new instance of the <see cref="startCombat"/> class.
		/// </summary>
		/// <param name="gc">Gc.</param>
		public Surprise(FighterController parent, float duration){
				_parent		=	parent;
				_duration=duration;
				stateID 	= 	CombatStates.surprise;

		}
			
		public override void Reason()
		{
				if (_timeCounter>=_duration || _parent.onHitSucceed) 
				{
						_parent.SetTransition(CombatTransitions.idleTransition);
				}
		}
				
		public override void Act()
		{ 
				_timeCounter += Time.deltaTime;
		}
		public override void DoBeforeEntering() 
		{
				_timeCounter 			= 0.0f;
				_parent.onHitSucceed 	= false;
		}
		public float duration{
				get{ return _duration;}
				set{ _duration = value;}
		}
}

/// <summary>
/// Getting hurt right.
/// </summary>
class GettingHurtRight:State<CombatTransitions,CombatStates>
{
		private		FighterController	_parent;
		private 	float 				_duration		=	0.33f;
		private		float 				_timeCounter	=	0.0f;
		private 	Vector3 			_nextPosition;

		/// <summary>
		/// Initializes a new instance of the <see cref="startCombat"/> class.
		/// </summary>
		/// <param name="gc">Gc.</param>
		public GettingHurtRight(FighterController parent){
				_parent		=	parent;
				stateID 	= 	CombatStates.gettingHurtRight;
		}
				
		public override void Reason()
		{
				if(_parent.Health<=0)
				{
						_parent.SetTransition (CombatTransitions.groggyTransition);
						return;
				}
				if ( !_parent.humanFlag && _timeCounter>=_duration) 
				{
						_parent.SetTransition (CombatTransitions.idleTransition);
						return;
				}
				if (_timeCounter>=_duration) 
				{
						_parent.SetTransition (CombatTransitions.idleTransition);
						return;
				}
		}
				
		public override void Act()
		{ 

				_timeCounter 			+= Time.deltaTime;
				Vector3 direction 		=	-(_parent.enemy.Position-_parent.Position).normalized;
				_parent.lookAtEnemy ();
				_nextPosition 			=	_parent.Position 	+ (direction * 0.2f);
				if(Vector3.Distance (_nextPosition, _parent.enemy.Position) < 3.65f)
				{
						_nextPosition	=	(_nextPosition-_parent.enemy.Position).normalized * (3.65f-Vector3.Distance (_nextPosition, _parent.enemy.Position))+ _nextPosition;
				}
				_nextPosition.x			=	Mathf.Clamp(_nextPosition.x,_parent._minX,_parent._maxX);
				_nextPosition.y			=	Mathf.Clamp(_nextPosition.y,_parent._minY,_parent._maxY);
				_parent.Position 		=	Vector3.Lerp (_parent.Position, _nextPosition, _parent.speed * Time.deltaTime);

		}

		public override void DoBeforeEntering() 
		{
				_timeCounter 				= 0.0f;
				_parent.enemy.OnHitSucceed 	= true;
				_parent.playHurtEffects ();
		}

		public override void DoBeforeLeaving() 
		{


		}

}
				
/// <summary>
/// Getting hurt left.
/// </summary>
class GettingHurtLeft:State<CombatTransitions,CombatStates>
{
		private		FighterController	_parent;
		private 	float 				_duration		=	0.33f;
		private		float 				_timeCounter	=	0.0f;
		private 	Vector3 			_nextPosition;
		/// <summary>
		/// Initializes a new instance of the <see cref="startCombat"/> class.
		/// </summary>
		/// <param name="gc">Gc.</param>
		public GettingHurtLeft(FighterController parent)
		{
				_parent		=	parent;
				stateID 	= 	CombatStates.gettingHurtLeft;
		}
				
		public override void Reason()
		{

				if ( !_parent.humanFlag && _timeCounter>=_duration) 
				{
						_parent.SetTransition (CombatTransitions.idleTransition);
						return;
				}
				if (_timeCounter>=_duration) 
				{
						_parent.SetTransition (CombatTransitions.idleTransition);
						return;
				}
		}
				
		public override void Act()
		{ 
			
				_timeCounter 			+= 	Time.deltaTime;
				Vector3 direction 		=	-(_parent.enemy.Position-_parent.Position).normalized;
				_parent.lookAtEnemy ();
				_nextPosition 			=	_parent.Position 	+ (direction * 0.2f);
				if(Vector3.Distance (_nextPosition, _parent.enemy.Position) < 3.65f)
				{
						_nextPosition	=	(_nextPosition-_parent.enemy.Position).normalized * (3.65f-Vector3.Distance (_nextPosition, _parent.enemy.Position))+ _nextPosition;
				}
				_nextPosition.x			=	Mathf.Clamp(_nextPosition.x,_parent._minX,_parent._maxX);
				_nextPosition.y			=	Mathf.Clamp(_nextPosition.y,_parent._minY,_parent._maxY);
				_parent.Position 		=	Vector3.Lerp (_parent.Position, _nextPosition, _parent.speed * Time.deltaTime);
		}
		public override void DoBeforeEntering() 
		{

				_timeCounter 				=	0.0f;
				_parent.enemy.OnHitSucceed 	= 	true;
				_parent.playHurtEffects ();

		}
		public override void DoBeforeLeaving() 
		{


		}

}
				
/// <summary>
/// Groggy.
/// </summary>
class Groggy:State<CombatTransitions,CombatStates>
{
		private		FighterController	_parent;
		private		float 				_duration 		= 	0.0f;
		private 	float 				_timeCounter 	=	0.0f;

		/// <summary>
		/// Initializes a new instance of the <see cref="startCombat"/> class.
		/// </summary>
		/// <param name="gc">Gc.</param>
		public Groggy(FighterController parent, float duration){
				_parent		=	parent;
				stateID 	= 	CombatStates.groggy;
				_duration	=	duration;

		}

	
		public override void Reason()
		{
				if(!_parent.humanFlag && !_parent.isDead && _timeCounter>=_duration )
				{
						_parent.health=(1f-((float)_parent.KnockOuts/(float)_parent.knockOutsLimit))*_parent.MaxHealth;
						_parent.updateFighterHealth ();
						_parent.SetTransition (CombatTransitions.idleTransition);
						return;
				}

				if(_parent.isDead && _timeCounter>=_duration)
				{
						_parent.SetTransition (CombatTransitions.fallingTransition);
						return;
				}

				if(_parent.humanFlag && _timeCounter>=_duration)
				{
						_parent.SetTransition (CombatTransitions.groggyEndingTransition);
						return;
				}
		}

	
		public override void Act()
		{ 

				_timeCounter+=  Time.deltaTime;
		}
				
		public override void DoBeforeEntering() 
		{
				 
				_timeCounter = 0.0f;
				_parent.KnockOuts++;
				if(_parent.KnockOuts>=_parent.knockOutsLimit)//limit
				{
						_parent.isDead = true;
				}
				_parent.playGroggyEffects ();
		}


		public override void DoBeforeLeaving()
		{
				_parent.stopGroggyEffects ();
		}
		public float Duration{
				get{ return _duration;}
				set{ _duration = value;}
		}
}

/// <summary>
/// Groggy ending.
/// </summary>
class GroggyEnding:State<CombatTransitions,CombatStates>
{
		private		FighterController	_parent;
		private 	float 				_duration		=	0.0f;
		private 	float 				_timeCounter	=	0.0f;

		public GroggyEnding(FighterController parent, float duration)
		{
				_parent		=	parent;
				stateID 	= 	CombatStates.groggyEnd;
				_duration	=	duration;
		}


		public override void Reason()
		{
				if (_parent.KnockOuts > 1) {
						_parent.SetTransition (CombatTransitions.injuredStartingTransition);
						return;
				}
				if(_timeCounter>=_duration)
				{
						_parent.SetTransition (CombatTransitions.injuredStartingTransition);
						return;
				}
		}


		public override void Act()
		{ 

				//COUNT TIME
				_timeCounter+=Time.deltaTime;
		}


		public override void DoBeforeEntering() 
		{
				_timeCounter = 0.0f;
				if(_parent.KnockOuts<=1)
				{
						_parent.KnockOutInfo = true;
				}

		}


		public override void DoBeforeLeaving()
		{
				_parent.KnockOutInfo = false;
		
		}
}

/// <summary>
/// Injured starting.
/// </summary>
class InjuredStarting:State<CombatTransitions,CombatStates>
{
		private float _duration=0.0f;
		private float _timeCounter=0.0f;
		private FighterController _parent;
		public InjuredStarting(FighterController parent, float duration)
		{
				_parent		=	parent;
				_duration	=	duration;
				stateID 	= 	CombatStates.injuredStarting;
		}


		public override void Reason()
		{
				if(_timeCounter>=_duration)
				{
						_parent.SetTransition (CombatTransitions.injuredTransition);
						return;
				}
		}


		public override void Act()
		{ 

				_timeCounter += Time.deltaTime;
		}

	
		public override void DoBeforeEntering() 
		{
				_timeCounter 		= 	0.0f;
				_parent.oldPosition	=	_parent.Position;
				_parent.goToInitialPosition ();
				_parent.InjuredDamage	=(float)_parent.KnockOuts / (float)_parent.knockOutsLimit;
				if (_parent.Avatar != null) {
						_parent.InjuredAvatar = _parent.Avatar;
				}

		}

	
		public override void DoBeforeLeaving()
		{

		}
}

/// <summary>
/// Injured.
/// </summary>
class Injured:State<CombatTransitions,CombatStates>
{
		private FighterController 	_parent;
		private float 				_duration		=	0.0f;
		private float 				_timeCounter	=	0.0f;

		public Injured(FighterController parent, float duration){
				_parent		=	parent;
				_duration	=	duration;
				stateID 	= 	CombatStates.injured;
		}

	
		public override void Reason()
		{
				if(_timeCounter>=_duration || _parent.Injuries<=0)
				{
						_parent.SetTransition (CombatTransitions.injuredEndingTransition);
						return;
				}
		}


		public override void Act()
		{ 

				_timeCounter += Time.deltaTime;
		}

	
		public override void DoBeforeEntering() 
		{
				_timeCounter 			= 0.0f;
				_parent.activeInjured 	= true;
				_parent.EnableCountDown = true;
				//START COUNTDOWN ANIMATION
		}

	
		public override void DoBeforeLeaving()
		{
				_parent.EnableCountDown = false;
				if (_parent.Injuries > 0) 
				{
						_parent.isDead=true;
				}
		}
}

/// <summary>
/// Injured ending.
/// </summary>
class InjuredEnding:State<CombatTransitions,CombatStates>
{
		private float _duration			=	5.0f;
		private float  _timeCounter		=	0.0f;
		private FighterController _parent;
		public InjuredEnding(FighterController parent, float duration)
		{
				_parent		=	parent;
				_duration	=	duration;
				stateID 	= 	CombatStates.injuredEnding;
		}

	
		public override void Reason()
		{
				if(_timeCounter>=_duration)
				{
						_parent.SetTransition (CombatTransitions.idleTransition);
						return;
				}
				if(_parent.isDead)
				{
						_parent.SetTransition (CombatTransitions.fallingTransition);
						return;		
				}
		}

		public override void Act()
		{ 

				_timeCounter += Time.deltaTime;
		}


		public override void DoBeforeEntering() 
		{
				_timeCounter = 0.0f;
				if (!_parent.isDead) {
						_parent.health=(1f-((float)_parent.KnockOuts/(float)_parent.knockOutsLimit))*_parent.MaxHealth;
						if (_parent.Avatar != null) 
						{
								_parent.InjuredAvatar = _parent.Avatar;
						}
						_parent.updateFighterHealth ();
				}


		}
				
		public override void DoBeforeLeaving()
		{
				_parent.Position = _parent.oldPosition;
		}
}
				
/// <summary>
/// Start combat.
/// </summary>
class Falling:State<CombatTransitions,CombatStates>
{
		private		FighterController	_parent;
		private 	float 				_duration		=	0.0f;
		private 	float 				_timeCounter	=	0.0f;
		/// <summary>
		/// Initializes a new instance of the <see cref="startCombat"/> class.
		/// </summary>
		/// <param name="gc">Gc.</param>
		public Falling(FighterController parent,float duration){
				_parent		=	parent;
				stateID 	= 	CombatStates.falling;
				_duration	=	duration;
		}

		
		public override void Reason()
		{
				if(_timeCounter>=_duration)
				{
						_parent.SetTransition (CombatTransitions.knockOutTransition);		
				}
		}

		
		public override void Act()
		{ 
				_timeCounter += Time.deltaTime;

		}

		
		public override void DoBeforeEntering() 
		{
				_timeCounter = 0.0f;
		}

		
		public override void DoBeforeLeaving()
		{

		}
}
				

}
