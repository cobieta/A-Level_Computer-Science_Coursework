using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public GameObject mainCamera;
	public float movementSpeed = 0.5f;
	public Vector2Int goal;
	public List<Vector3Int> path;

	private List<Node> openNodes, visitedNodes;
	private int[,] dungeonGrid;
	private int gridLength;
	private Camera playerCam;
	private bool characterMoving;
	private int count;
	private float tempTime;
	private LevelManager lvlManager;

	//Finds the camera, dungeon grid and gridLength.
	void Start () {
		lvlManager = FindObjectOfType<LevelManager>();
		playerCam = mainCamera.GetComponent<Camera>();
		dungeonGrid = PersistentGameData.dungeonMapSave;
		float totalElements = (float)PersistentGameData.dungeonMapSave.Length;
		gridLength = Mathf.RoundToInt(Mathf.Sqrt(totalElements)) - 1;
		characterMoving = false;
		count = 0;
		tempTime = 0f;
		gameObject.transform.position = PersistentGameData.playerPosSave;
		openNodes = new List<Node>();
		visitedNodes = new List<Node>();
		path = new List<Vector3Int>();
	}

	//Checks every frame for the player's touch on screen, if the player selects a floor tile in the dungeon
	//the player character will travel there. If the player is at the exit a new dungeon level is loaded. 
	void Update () {
		//If the player steps on the exit tile, the next level of the dungeon is loaded.
		if (gameObject.transform.position == PersistentGameData.exitPosSave) {
			Debug.Log("Player is at exit");
			PersistentGameData.dungeonLevel += 1;
			PersistentGameData.dungeonMapSave = new int[1,1];
			lvlManager.ReLoadLevel(); 
		} else if (characterMoving == true) {
			Debug.Log("Player is moving");
			tempTime += Time.deltaTime;
			if (tempTime > movementSpeed) {
				AnimateWalk ();
			}
		} else if (Input.touchCount == 1) {
			Touch fingerPos = Input.GetTouch(0);
			Vector2 tileTouchedPos = playerCam.ScreenToWorldPoint(fingerPos.position);
			goal = Vector2Int.RoundToInt(tileTouchedPos);
			if (goal.x > 0 && goal.x < gridLength && goal.y > 0 && goal.y < gridLength) {
				if (dungeonGrid[goal.x, goal.y] != 0) {
					openNodes.Clear();
					visitedNodes.Clear();
					path.Clear();
					AStarPathfinding();
					characterMoving = true;
				}
			}
		//Debugging condition for searching for mouse input.
		} else if (Input.GetMouseButtonDown(0)) {
			Vector2 tileTouchedPos = playerCam.ScreenToWorldPoint(Input.mousePosition);
			goal = Vector2Int.RoundToInt(tileTouchedPos);
			if (goal.x > 0 && goal.x < gridLength && goal.y > 0 && goal.y < gridLength) {
				if (dungeonGrid[goal.x, goal.y] != 0) {
					openNodes.Clear();
					visitedNodes.Clear();
					path.Clear();
					AStarPathfinding();
					characterMoving = true;
				}
			}
		}
	}

	//Save the player's position before another scene is loaded. 
	void OnDisable () {
		PersistentGameData.playerPosSave = gameObject.transform.position;
	}

	//Called by the DisplayDungeon class to get the newly generated dungeon.
	public void GetNewDungeon () {
		dungeonGrid = PersistentGameData.dungeonMapSave;
		float totalElements = (float)PersistentGameData.dungeonMapSave.Length;
		gridLength = Mathf.RoundToInt(Mathf.Sqrt(totalElements)) - 1;
	}

	//Animate the player character moving through the dungeon and give them a 1 in 18 chance of 
	//encountering a monster.
	void AnimateWalk () {
		tempTime = 0f;
		if (count != path.Count) {
			gameObject.transform.position = path[count];
			count++;
			int chance = Random.Range(1, 19);
			if (chance == 18) {
				lvlManager.LoadNextAfterFade();
			}
		} else {
			PersistentGameData.playerPosSave = gameObject.transform.position;
			characterMoving = false;
			count = 0;
		}
	}

	//Pathfinds to the goal coordinates using the A* algorithm.
	void AStarPathfinding () {
		Vector2Int startCoords = Vector2Int.RoundToInt(gameObject.transform.position);
		Node startNode = new Node(startCoords, null, 0, 0);
		openNodes.Add(startNode);
		Node currentNode = startNode;
		//While the path has not reached the goal.
		while (currentNode.coords != goal) {
			Vector2Int[] neighbours = FindNeighbours(currentNode.coords);
			foreach (Vector2Int n in neighbours) {
				if (n != Vector2Int.zero) {
					Node newNode = new Node(n, currentNode, 0, currentNode.gCost + 1);
					int FCost = CalculateFCost(newNode);
					newNode.fCost = FCost;
					Node duplicate = FindNodeInList(newNode, false);
					if (duplicate == null) {
						duplicate = FindNodeInList(newNode, true);
						if (duplicate != null && newNode.fCost < duplicate.fCost) {
							openNodes.Add(newNode);
						} else if (duplicate == null) {
							openNodes.Add(newNode);
						}
					}
				}
			}
			visitedNodes.Add(currentNode);
			openNodes.Remove(currentNode);
			currentNode.fCost = int.MaxValue;
			foreach (Node m in openNodes) {
				//Find the node with least fCost to be next current Node.
				if (m.fCost <= currentNode.fCost) {
					currentNode = m;
				}
			}
		}
		visitedNodes.Add(currentNode);
		CreatePath();
	}

	//Creates a list of the coordinates of each tile on the path found by the A* algorithm. 
	void CreatePath () {
		Node currentNode = visitedNodes[visitedNodes.Count-1];
		while (currentNode.parentNode != null) {
			Vector3Int nodeCoords = new Vector3Int(currentNode.coords.x, currentNode.coords.y, 0);
			path.Add(nodeCoords);
			currentNode = currentNode.parentNode;
		}
		path.Reverse();
	}

	//Finds a node with specific coordinates in a specific list and return it, otherwise return null.
	Node FindNodeInList (Node node, bool openList) {
		//Finds any node that has the same coordinates as the node given.
		System.Predicate<Node> duplicateNodeFinder = (Node n) => {return n.coords == node.coords; };
		//If I want to search the openNodes list.
		if (openList == true) {
			Node foundNode1 = openNodes.Find(duplicateNodeFinder);
			return foundNode1;
		//If I want to search the visitedNodes list.
		} else {
			Node foundNode2 = visitedNodes.Find(duplicateNodeFinder);
			return foundNode2;
		}
	}

	//Calculates the fCost of a node using the diagonal distance heuristic.
	int CalculateFCost (Node node) {
		int xCoord = node.coords.x;
		int yCoord = node.coords.y;
		int hCost = Mathf.Max(Mathf.Abs(xCoord-goal.x), Mathf.Abs(yCoord-goal.y));
		int fCost = node.gCost + hCost;
		return fCost;
	}

	//Find the given tile's neighbours. 
	Vector2Int[] FindNeighbours (Vector2Int coords) {
		int xCoord = coords.x;
		int yCoord = coords.y;
		Vector2Int[] NESWArray = new Vector2Int[4];
		NESWArray [0] = new Vector2Int (xCoord, yCoord + 1);
		NESWArray [1] = new Vector2Int (xCoord + 1, yCoord);
		NESWArray [2] = new Vector2Int (xCoord, yCoord - 1);
		NESWArray [3] = new Vector2Int (xCoord - 1, yCoord);
		//Get rid of coordinates that are walls or outside the grid.
		for (int i = 0; i <= 3; i++) {
			if (NESWArray [i].x > gridLength || NESWArray [i].x < 0) {
				NESWArray [i] = Vector2Int.zero;
			} else if (NESWArray [i].y > gridLength || NESWArray [i].y < 0) {
				NESWArray[i] = Vector2Int.zero;
			} else if (dungeonGrid[NESWArray[i].x, NESWArray[i].y] == 0) {
				NESWArray[i] = Vector2Int.zero;
			}
		}
		return NESWArray;
	}

}

//The class used to create nodes, which keeps a Node's data in one object. 
class Node {

	public Vector2Int coords;
	public Node parentNode;
	public int fCost, gCost;

	//A node keeps track of it's coordinates, it's parent node and it's f and g costs.
	public Node (Vector2Int coOrds, Node parent, int costF, int costG) {
		coords = coOrds;
		parentNode = parent;
		fCost = costF;
		gCost = costG;
	}
}
