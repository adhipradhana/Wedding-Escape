using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	// list of point
	private static List<Board.Point> swapShapeList = new List<Board.Point>();

	// magnet button bool
	private static bool isMagnetClicked = false;

	// tile and turn
	public static int turnLeft = 20;
	public static int tileLeft = 100;
	private Text turnLeftText;
	private Text tileLeftText;

	/// <summary>
	/// This method is used for regular move (No Power-Up).
	/// </summary>
	/// <param name="p">P is the clicked shape point.</param>
	public static void RegularMove() {
		Board board = GameObject.Find ("Board").GetComponent<Board>();
		GameObject magnetButton = GameObject.Find ("Magnet Power-Up");

		if (swapShapeList.Count > 5) {
			for (int i = 0; i < 5; i++) {
				board.PlaceSwapShape (swapShapeList [i].x, swapShapeList [i].y);
			}
		} else {
			for (int i = 0; i < swapShapeList.Count; i++) {
				board.PlaceSwapShape (swapShapeList [i].x, swapShapeList [i].y);
			}
		}

		if (board.IsMatch ()) {
			turnLeft--;
			DestroyTurn ();
			if (!board.IsPossibleMove ()) {
				board.ShuffleBoard ();
			}
			if (board.IsBonusAvailable()) {
				magnetButton.GetComponent<Image>().enabled = true;
				magnetButton.GetComponent<Image> ().color = new Color(0.53725490196f,0.53725490196f,0.53725490196f);
				magnetButton.GetComponent<Button> ().enabled = true;
			}
		} else {
			if (swapShapeList.Count > 5) {
				for (int i = 0; i < 5; i++) {
					board.PlaceSwapShape (swapShapeList [i].x, swapShapeList [i].y);
				}
			} else {
				for (int i = 0; i < swapShapeList.Count; i++) {
					board.PlaceSwapShape (swapShapeList [i].x, swapShapeList [i].y);
				}
			}
			swapShapeList.Clear ();
		}
			 
	}

	/// <summary>
	/// This method is used for magnet power-up move.
	/// </summary>
	/// <param name="p">P is the clicked shape point.</param>
	public static void MagnetPowerUpMove(Board.Point p) {
		Board board = GameObject.Find ("Board").GetComponent<Board> ();
		GameObject magnetButton = GameObject.Find ("Magnet Power-Up");

		int type = board.map [p.x, p.y].GetComponent<Shape> ().Type;
		List<Board.Point> sameType = new List<Board.Point> ();

		for (int i = 0; i < Board.Size; i++) {
			for (int j = 0; j < Board.Size; j++) {
				if (board.map [i, j].GetComponent<Shape> ().Type == type) {
					Board.Point point;
					point.x = i;
					point.y = j;

					sameType.Add (point);
				}
			}
		}

		board.DestroyShape (sameType);
		turnLeft--;
		tileLeft -= sameType.Count;
		DestroyTurn ();

		if (!board.IsPossibleMove ()) {
			board.ShuffleBoard ();
		}
		magnetButton.GetComponent<Image> ().enabled = false;
		magnetButton.GetComponent<Button> ().enabled = false;
		isMagnetClicked = false;
	}

	/// <summary>
	/// This method is used for destroying matched shapes and collapsing shapes.
	/// </summary>
	private static void DestroyTurn() {
		Board board = GameObject.Find ("Board").GetComponent <Board> ();

		List<Board.Point> destroy = board.ExploreMatch ();
		tileLeft -= destroy.Count;
		swapShapeList.Clear ();
		board.DestroyShape (destroy);
		board.FallingThrough ();
		if (board.IsMatch ()) {
			DestroyTurn ();
		}
	}

	/// <summary>
	/// This method is used to inform whether the magnet button is clicked.
	/// </summary>
	public void MagnetClick() {
		GameObject magnetButton = GameObject.Find ("Magnet Power-Up");

		if (!isMagnetClicked) {
			isMagnetClicked = true;
			magnetButton.GetComponent<Image> ().color = new Color (0, 0, 0);
		} else {
			isMagnetClicked = false;
			magnetButton.GetComponent<Image> ().color = new Color (0.53725490196f,0.53725490196f,0.53725490196f);
		}
	}

	private void WindowsInput() {
		if (isMagnetClicked) {
			if (Input.GetMouseButtonDown (0)) {
				Vector3 mousePos = Input.mousePosition;
				Vector2 mousePos2D = new Vector2 (mousePos.x, mousePos.y);

				RaycastHit2D hit = Physics2D.Raycast (mousePos2D, Vector2.zero);
				if (hit.collider != null) { 
					GameObject shape = hit.collider.gameObject;
					Board.Point p;

					p.x = shape.GetComponent<Shape> ().Row;
					p.y = shape.GetComponent<Shape> ().Col;

					if (shape.GetComponent<Shape> ().Bonus != 0) {  
						MagnetPowerUpMove (p);
					}
				}
			} 
		} else {
			if (Input.GetMouseButton (0)) {
				Vector3 mousePos = Input.mousePosition;
				Vector2 mousePos2D = new Vector2 (mousePos.x, mousePos.y);

				RaycastHit2D hit = Physics2D.Raycast (mousePos2D, Vector2.zero);
				if (hit.collider != null) {
					GameObject shape = hit.collider.gameObject;
					Board.Point p;

					p.x = shape.GetComponent<Shape> ().Row;
					p.y = shape.GetComponent<Shape> ().Col;

					if (!swapShapeList.Contains (p)) {
						swapShapeList.Add (p);
					}
				}
			}
			if (Input.GetMouseButtonUp (0)) {
				RegularMove ();
			}
		}
	}

	private void AndroidInput() {
		if (isMagnetClicked) {
			if (Input.touchCount == 1) {
				if (Input.GetTouch (0).phase == TouchPhase.Began) {
					Vector2 touchPos = Input.GetTouch (0).position;

					RaycastHit2D hit = Physics2D.Raycast (touchPos, Vector2.zero);
					if (hit.collider != null) { 
						GameObject shape = hit.collider.gameObject;
						Board.Point p;

						p.x = shape.GetComponent<Shape> ().Row;
						p.y = shape.GetComponent<Shape> ().Col;

						if (shape.GetComponent<Shape> ().Bonus != 0) {  
							MagnetPowerUpMove (p);
						} 
					}
				}
			} 
		} else {
			if (Input.touchCount == 1) {
				if (Input.GetTouch (0).phase == TouchPhase.Moved || Input.GetTouch (0).phase == TouchPhase.Stationary) {
					Vector2 touchPos = Input.GetTouch (0).position;

					RaycastHit2D hit = Physics2D.Raycast (touchPos, Vector2.zero);
					if (hit.collider != null) { 
						GameObject shape = hit.collider.gameObject;
						Board.Point p;

						p.x = shape.GetComponent<Shape> ().Row;
						p.y = shape.GetComponent<Shape> ().Col;

						if (!swapShapeList.Contains (p)) {
							swapShapeList.Add (p);
						}
					}
				} else if (Input.GetTouch (0).phase == TouchPhase.Ended) {
					RegularMove ();
				}
			}
		}
	}

	private void InputHandling() {
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {
			WindowsInput ();
		} else  {
			AndroidInput ();
		}
	}

	void Start() {
		Board board = GameObject.Find ("Board").GetComponent<Board> ();
		turnLeftText = GameObject.Find ("Turn").GetComponent<Text> ();
		tileLeftText = GameObject.Find ("Tile").GetComponent<Text> ();

		board.ShuffleBoard();
	}

	void Update() {
		if ((tileLeft <= 0) && (turnLeft >= 0)) {
			tileLeft = 100;
			turnLeft = 20;
			LevelManager.currentState = LevelManager.State.Win;
		} 
		if ((turnLeft == 0) && (tileLeft > 0)) {
			tileLeft = 100;
			turnLeft = 20;
			LevelManager.currentState = LevelManager.State.Lose;
		} 

		turnLeftText.text = turnLeft.ToString ();
		tileLeftText.text = tileLeft.ToString ();

		InputHandling ();
	}

}

