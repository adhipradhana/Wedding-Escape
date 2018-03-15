using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shape : MonoBehaviour {

	// constant attributes
	public const int Empty = 4;
	public const int BlueDiamond = 0;
	public const int PurpleDiamond = 1;
	public const int GoldCoin = 2;
	public const int SilverCoin = 3;

	// image renderer
	public Sprite[] shapesArray = new Sprite[5];

	// bonus image
	public GameObject bonusImage;

	// object attributes
	private int type; 
	private int row; 
	private int col; 
	private int bonus;

	// attributes setter and getter
	public int Type {
		get {
			return type;
		} set {
			type = value;
		}
	}
	public int Row {
		get { 
			return row;
		} set { 
			row = value;
		}
	}
	public int Col {
		get { 
			return col;
		} set { 
			col = value;
		}
	}
	public int Bonus {
		get { 
			return bonus;
		} set { 
			bonus = value;
		}
	}

	// assignment
	public void Assign(int _row, int _col, int _type, int _bonus) {
		this.row = _row;
		this.col = _col;
		this.type = _type;
		this.bonus = _bonus;
		RenderImage ();
	}

	/// <summary>
	/// This method is used to get the swap type of shape.
	/// </summary>
	/// <returns>The type of swap shape.</returns>
	public int SwapShape() {
		if (this.type == BlueDiamond) {
			return PurpleDiamond;
		} else if (this.type == PurpleDiamond) {
			return BlueDiamond;
		} else if (this.type == GoldCoin) {
			return SilverCoin;
		} else {
			return GoldCoin;
		}
	}

	/// <summary>
	/// This method is used to assign sprite with its type.
	/// </summary>
	void RenderImage() {
		GetComponent<Image> ().sprite = shapesArray [type];
	}

	void RenderBonus() {
		if (bonus != 0) {
			bonusImage.GetComponent<Image> ().enabled = true;
		} else {
			bonusImage.GetComponent<Image> ().enabled = false;
		}
	}

	/// <summary>
	/// Determines whether _s is the same state (same state or swap shape).
	/// </summary>
	/// <returns><c>true</c> if this instance is same state the specified _s; otherwise, <c>false</c>.</returns>
	/// <param name="_s">S is the other shape.</param>
	public bool IsSameState(Shape _s) {
		if (type <= 1) {
			return (_s.type <= 1); 
		} else {
			return (_s.type > 1);
		}
	}

	/// <summary>
	/// Determines whether _s is the same type.
	/// </summary>
	/// <returns><c>true</c> if this instance is same type the specified _s; otherwise, <c>false</c>.</returns>
	/// <param name="_s">S is the other type.</param>
	public bool IsSameType(Shape _s) {
		return (type == _s.type);
	}

	void Update() {
		RenderBonus ();
	}

}
