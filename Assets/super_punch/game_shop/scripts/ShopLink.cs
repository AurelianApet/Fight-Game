﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace game_shop{

/// <summary>
/// I product.
/// </summary>
public interface IProduct{

		string ImageSource			{ get; set;}
		string ProductName			{ get; set;}
		string ProductLink			{ get; set;}
		string ProductPrice			{ get; set;}
		string ProductDescription	{ get; set;}
}

/// <summary>
/// Shop link.
/// </summary>
public class ShopLink : MonoBehaviour, IProduct {

	public 	string 		link		=	"http://looneybits.com";
	private string  	_name 		= 	"";
	private string  	_text 		= 	"";
	private string 		_price		=	"";
	private string		_imgLink	=	"";	
	private Transform 	_buttonBuy;
	private Transform	_inputName;
	private Transform	_inputDescription;
	private Transform 	_inputPrice;
	private Transform	_inputImage;
	
	/// <summary>
	/// Use this for initialization
	/// </summary>
	void OnEnable()
	{
		_inputName 			= transform.FindChild ("InputFieldName");
		_inputImage 		= transform.FindChild ("ProductImageContainer");
		_inputDescription 	= transform.FindChild ("InputFieldDescription");
		_inputPrice			= transform.FindChild ("InputFieldPrice");
		_buttonBuy 			= transform.FindChild ("ButtonBuy");
	}
	
	/// <summary>
	/// Gets or sets the image source.
	/// </summary>
	/// <value>The image source.</value>
	public string ImageSource
	{
		get{return _imgLink;}
		set{
			_imgLink	=	value;
			WWW www 	= 	new WWW(_imgLink);
			StartCoroutine(WaitForRequest(www));
		}
	}
	
	/// <summary>
	/// Gets or sets the product link.
	/// </summary>
	/// <value>The product link.</value>
	public string ProductLink
	{
		get{return link;}
		set{
				link=value;
				if(_buttonBuy!=null)
				{
										_buttonBuy.GetComponent<LinkCanvasButton> ().link = link;			
				}
		}
	}

	/// <summary>
	/// Gets or sets the name of the product.
	/// </summary>
	/// <value>The name of the product.</value>
	public string ProductName
	{
			get{return _name;}
			set{
				_name	=	value;
				if(_inputName!=null)
				{
						_inputName.GetComponent<InputField>().text=_name;
				}
			}
	}

	/// <summary>
	/// Gets or sets the product description.
	/// </summary>
	/// <value>The product description.</value>
	public string ProductDescription
	{
		get{return _text;}
		set{
			_text	=	value;
			if(_inputDescription!=null)
			{
				_inputDescription.GetComponent<InputField>().text=_text;
			}
		}
	}
	/// <summary>
	/// Gets or sets the product price.
	/// </summary>
	/// <value>The product price.</value>
	public string ProductPrice
	{
			get{return _price;}
			set{
					_price	=	value;
					if(_inputPrice!=null)
					{
						_inputPrice.GetComponent<InputField>().text=_price;
					}
			}
	}

	/// <summary>
	/// Waits for request.
	/// </summary>
	/// <returns>The for request.</returns>
	/// <param name="www">Www.</param>
	IEnumerator WaitForRequest(WWW www)
	{
		yield return www;
		
		// check for errors
		if (www.error == null)
		{
			if(_inputImage!=null)
			{
					GameObject imageAux=_inputImage.transform.FindChild ("Image").gameObject;
					if(imageAux!=null)
					{
						imageAux.GetComponent<Image> ().sprite = Sprite.Create(www.texture,new Rect(0, 0, www.texture.width, www.texture.height),new Vector2(0.5f,0.5f));	
					}
			}
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}
	}
						
}
}
