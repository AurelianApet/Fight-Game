using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Round girl behaviour.
/// </summary>
public class RoundGirlBehaviour : MonoBehaviour {

		private Text 		_roundNumber;
		private int 		_round = 0;
		private Text 		_roundNumberView;
	/// <summary>
	/// Use this for initialization
	/// </summary>
	void OnEnable () {
				if (_roundNumber == null) {
						_roundNumber=  GetComponentInChildren<Text>() ;
				}
				_round++;
				if(_roundNumber!=null)
				{
						_roundNumber.text = _round.ToString ("00");		
				}

	}
}
