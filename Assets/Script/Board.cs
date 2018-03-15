using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

	// struct of point
	public struct Point {
		public int x;
		public int y;
	}

	// constant attributes
	public const int Size = 6;
	private const float XStart = -2.5f;
	private const float YStart = 0.5f;

	// shape object
	public GameObject shape;

	// board
	public GameObject[,] map = new GameObject[Size,Size];

	// Get random shape
	private int GetRandomType() {
		return Random.Range(0,4);
	}
		
	/// <summary>
	/// Constructor of the shapes at the beginning of the game.
	/// </summary>
	/// <param name="row"> The row position of shape.</param>
	/// <param name="col"> The column position of the shape.</param>
	public void InitiateRandomShape(int row, int col) {
		
		// shape position relative to parent (board)
		float xPosition = (XStart + col) * 100;
		float yPosition = (YStart - row) * 100;

		// get random type
		int type = GetRandomType ();

		// Instantiate shape and set it to parent
		GameObject temp = Instantiate (shape) as GameObject;
		temp.transform.SetParent (GameObject.Find("Board").transform, false);

		// Change shape position
		Vector3 currentPosition = temp.transform.position;
		currentPosition.x += xPosition;
		currentPosition.y += yPosition;
		temp.transform.position = currentPosition;

		temp.GetComponent<Shape> ().Assign (row, col ,type, 0);

		map [row, col] = temp;
	}

	/// <summary>
	/// Replace the shape with the opposite shape.
	/// </summary>
	/// <param name="row"> The row position of the shape.</param>
	/// <param name="col"> The column position of the shape.</param>
	public void PlaceSwapShape(int row, int col) {
		int type = map[row, col].GetComponent<Shape>().SwapShape ();
		int bonus = map [row, col].GetComponent<Shape> ().Bonus;
		map[row, col].GetComponent<Shape>().Assign (row, col ,type,bonus);
	}

	/// <summary>
	/// Place a random shape. 
	/// </summary>
	/// <param name="row"> The row position of the shape.</param>
	/// <param name="col"> The column position of the shape.</param>
	public void PlaceRandomShape(int row, int col) {
		int type = GetRandomType ();
		int bonus = map [row, col].GetComponent<Shape> ().Bonus;
		map [row, col].GetComponent<Shape> ().Assign (row, col, type, bonus);
	}

	/// <summary>
	/// Shuffles the board at the beggnning or if there is no possible move.
	/// </summary>
	public void ShuffleBoard() {
		for (int i = 0; i < Size; i++) {
			for (int j = 0; j < Size; j++) {
				InitiateRandomShape (i, j);
			}
		}
		if (!IsPossibleMove () || IsMatch ()) {
			foreach (Transform child in transform) {
				GameObject.Destroy(child.gameObject);
			}
			ShuffleBoard ();
		}
	}

	/// <summary>
	/// Determines whether there is a possible move.
	/// </summary>
	/// <returns><c>true</c> if there is possible move; otherwise, <c>false</c>.</returns>
	public bool IsPossibleMove() {
		// check horizontal
		for (int i = 0; i < Size; i++) {
			int j = 0;
			int currentCol = 0;
			int count = 0;
			while (j < Size) {
				if (this.map[i,currentCol].GetComponent<Shape>().IsSameState(this.map[i,j].GetComponent<Shape>())) {
					count++;
				} else {
					currentCol = j;
					count = 1;
				}
				if (count == 3)  {
					return true;	
				}
				j++;
			}
		}

		// check vertical
		for (int j = 0; j < Size; j++) {
			int i = 0;
			int currentRow = 0;
			int count = 0;
			while (i < Size) {
				if (this.map[currentRow,j].GetComponent<Shape>().IsSameState(this.map[i,j].GetComponent<Shape>())) {
					count++;
				} else {
					currentRow = i;
					count = 1;
				}
				if (count == 3)  {
					return true;	
				}
				i++;
			}
		}

		return false;
	}

	/// <summary>
	/// Determines whether there is a matched shapes.
	/// </summary>
	/// <returns><c>true</c> if there is matched shapes; otherwise, <c>false</c>.</returns>
	public bool IsMatch() {
		// check horizontal
		for (int i = 0; i < Size; i++) {
			int j = 0;
			int currentCol = 0;
			int count = 0;
			while (j < Size) {
				if (this.map[i,currentCol].GetComponent<Shape>().IsSameType(this.map[i,j].GetComponent<Shape>())) {
					count++;
				} else {
					currentCol = j;
					count = 1;
				}
				if (count == 3)  {
					return true;	
				}
				j++;
			}
		}

		// check vertical
		for (int j = 0; j < Size; j++) {
			int i = 0;
			int currentRow = 0;
			int count = 0;
			while (i < Size) {
				if (this.map[currentRow,j].GetComponent<Shape>().IsSameType(this.map[i,j].GetComponent<Shape>())) {
					count++;
				} else {
					currentRow = i;
					count = 1;
				}
				if (count == 3)  {
					return true;	
				}
				i++;
			}
		}
			
		return false;
	}

	/// <summary>
	/// Counts horizontally matched shapes.
	/// </summary>
	/// <returns> Numbers of horizontally matched shapes.</returns>
	/// <param name="row">The row of shape.</param>
	/// <param name="col">The col of shape.</param>
	public int CountMatchHorizontal(int row, int col) {
		Shape currentShape = map [row, col].GetComponent<Shape>();
		int count = 0; 
		int j = col;
		while (j < Size) {
			if (currentShape.IsSameType (map [row, j].GetComponent<Shape> ())) {
				count++;
				j++;
			} else {
				break;
			}
		}

		return count;
	}

	/// <summary>
	/// Counts vertically matched shapes.
	/// </summary>
	/// <returns> Numbers of vertically matched shapes.</returns>
	/// <param name="row">The row of shape.</param>
	/// <param name="col">The col of shape.</param>
	public int CountMatchVertical(int row, int col) {
		Shape currentShape = map [row, col].GetComponent<Shape>();
		int count = 0;
		int i = row;
		while (i < Size) {
			if (currentShape.IsSameType (map [i, col].GetComponent<Shape> ())) {
				count++;
				i++;
			} else {
				break;
			}
		}

		return count;
	}

	/// <summary>
	/// Explores any matched shapes.
	/// </summary>
	/// <returns>List of horizontal and vertical matched shapes.</returns>
	public List<Point> ExploreMatch() {
		List<Point> explore = new List <Point> ();

		// check horizontal
		for (int i = 0; i < Size; i++) {
			int j = 0;
			while (j < Size) {
				int count = CountMatchHorizontal (i, j);
				if (count > 3) {
					for (int k = j; k < j + (count - 1); k++) {
						Point temp;
						temp.x = i;
						temp.y = k;

						explore.Add (temp);
					}
					map [i, j + (count - 1)].GetComponent<Shape> ().Bonus = count - 3;
				} else if (count == 3) {
					for (int k = j; k < j + count; k++) {
						Point temp;
						temp.x = i;
						temp.y = k;

						explore.Add (temp);
					}
				}
				j += count;
			}
		}

		// check vertical 
		for (int j = 0; j < Size; j++) {
			int i = 0;
			while (i < Size) {
				int count = CountMatchVertical (i, j);
				if (count > 3) {
					for (int k = i; k < i + (count - 1); k++) {
						Point temp;
						temp.x = k;
						temp.y = j;

						explore.Add (temp);
					}
					map [i + (count - 1), j].GetComponent<Shape> ().Bonus = count - 3;
				} else if (count == 3) {
					for (int k = i; k < i + count; k++) {
						Point temp;
						temp.x = k;
						temp.y = j;

						explore.Add (temp);
					}
				}
				i += count;
			}
		}

		return explore;
	}


	/// <summary>
	/// Falling mechanism after successful move.
	/// </summary>
	public void FallingThrough() {
		for (int i = Size - 1; i >= 0; i--) {
			for (int j = 0; j < Size; j++) {
				if (map [i, j].GetComponent<Shape>().Type == Shape.Empty) {
					int k = FindCollapseShapeRow (i, j);
					if (k != Shape.Empty) {
						int type = map[k,j].GetComponent<Shape> ().Type;
						int bonus = map [k, j].GetComponent<Shape> ().Bonus;
						map[i,j].GetComponent<Shape> ().Assign (i, j, type, bonus);
						map [k, j].GetComponent<Shape> ().Type = Shape.Empty;	
					} else {
						PlaceRandomShape (i, j);
					}
				}
			}
		}
	}

	/// <summary>
	/// Destroy any matched shapes.
	/// </summary>
	/// <param name="destroy">List of matched shapes point.</param>
	public void DestroyShape(List<Point> destroy) {
		foreach (Point point in destroy) {
			map [point.x, point.y].GetComponent<Shape> ().Assign(point.x,point.y, Shape.Empty, 0);
		}
	}

	/// <summary>
	/// Finds the first non empty shape on one column.
	/// </summary>
	/// <returns>The first non empty shape's row.</returns>
	/// <param name="row">.</param>
	/// <param name="col">Col.</param>
	private int FindCollapseShapeRow(int row, int col) {
		for (int i = row; i >= 0; i--) {
			if (map [i, col].GetComponent<Shape>().Type != Shape.Empty) {
				return i;
			} 
		}
		return Shape.Empty;
	}

	/// <summary>
	/// Determines whether there is bonus available.
	/// </summary>
	/// <returns><c>true</c> if there is bonus available; otherwise, <c>false</c>.</returns>
	public bool IsBonusAvailable() {
		for (int i = 0; i < Size; i++) {
			for (int j = 0; j < Size; j++) {
				if (map [i, j].GetComponent<Shape> ().Bonus != 0) {
					return true;
				}
			}
		}
		return false;
	}
		
} 