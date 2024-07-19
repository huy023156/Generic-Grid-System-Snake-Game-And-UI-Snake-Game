using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Board : MonoBehaviour
{
	public static Board Instance;

	public event EventHandler OnGameOver;

	[SerializeField]
	private Tile tilePrefab;
	private TileGrid grid;
	private List<Tile> snakeTiles;

	private Tile head;

	private float stepTimer;
	[SerializeField]
	private float stepTimerMax;

	private Tile fruit;

	public enum Direction
	{
		Up,
		Down,
		Left,
		Right
	}

	private Direction currentDirection;

	private bool IsMovedDone;

	private void Awake()
	{
		Instance = this;
		grid = GetComponentInChildren<TileGrid>();
		snakeTiles = new List<Tile>();
		stepTimerMax = 0.2f;
		currentDirection = Direction.Right;
		IsMovedDone = true;
		fruit = Instantiate(tilePrefab, this.transform);
		fruit.GetImage().color = Color.red;
	}

	private void Start()
	{
		SpawnSnake();
		SpawnFruit();
	}

	private void Update()
	{
		HandleInput();
		HandleMovement();
	}

	private void HandleInput()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			RestartGame();
		}

		if (!IsMovedDone) return;

		if (Input.GetKeyDown(KeyCode.W))
		{
			if (currentDirection == Direction.Down) return;
			currentDirection = Direction.Up;
			IsMovedDone = false;
		}
		else if (Input.GetKeyDown(KeyCode.S))
		{
			if (currentDirection == Direction.Up) return;
			currentDirection = Direction.Down;
			IsMovedDone = false;
		}
		else if (Input.GetKeyDown(KeyCode.D))
		{
			if (currentDirection == Direction.Left) return;
			currentDirection = Direction.Right;
			IsMovedDone = false;
		}
		else if (Input.GetKeyDown(KeyCode.A)) 
		{
			if (currentDirection == Direction.Right) return;
			currentDirection = Direction.Left;
			IsMovedDone = false;
		}

	}
	private void HandleMovement()
	{
		stepTimer -= Time.deltaTime;

		if (stepTimer < 0)
		{
			stepTimer = stepTimerMax;
			Move(currentDirection);
		}
	}

	private void SpawnFruit()
	{
		Cell spawnCell = GetRandomCellToSpawnTile();

		fruit.SetCell(spawnCell);
		spawnCell.SetFruitTile(fruit);
		fruit.transform.position = spawnCell.transform.position;
	}

	private Cell GetRandomCellToSpawnTile()
	{
		System.Random random = new System.Random();

		int randomIndex = random.Next(0, grid.Cells.Count());

		int originRandomIndex = randomIndex;

		while (true)
		{
			if (!grid.Cells[randomIndex].HasSnakeTile() && !grid.Cells[randomIndex].HasFruitTile())
			{
				return grid.Cells[randomIndex];
			}

			randomIndex++;
			if (randomIndex >= grid.Cells.Count())
			{
				randomIndex = 0;
			}

			if (randomIndex == originRandomIndex)
				return null;
		}
	}

	private void SpawnSnake()
	{
		Vector2Int middlePoint = new Vector2Int(grid.GetWidth() / 2, grid.GetHeight() / 2);

		Debug.Log(middlePoint);
		Debug.Log(grid.GetHeight() + " " + grid.GetWidth());

		Cell middleCell = grid.Rows[middlePoint.y].Cells[middlePoint.x];
		head = Instantiate(tilePrefab, this.transform);
		head.GetImage().color = Color.white;
		middleCell.SetSnakeTile(head);
		head.SetCell(middleCell);
		head.transform.position = middleCell.transform.position;
		snakeTiles.Add(head);

		Cell cell = grid.Rows[middlePoint.y].Cells[middlePoint.x - 1];
		Tile tile = Instantiate(tilePrefab, this.transform);
		cell.SetSnakeTile(tile);
		tile.SetCell(cell);
		tile.transform.position = cell.transform.position;
		snakeTiles.Add(tile);

		cell = grid.Rows[middlePoint.y].Cells[middlePoint.x - 2];
		tile = Instantiate(tilePrefab, this.transform);
		cell.SetSnakeTile(tile);
		tile.SetCell(cell);
		tile.transform.position = cell.transform.position;
		snakeTiles.Add(tile);
	}

	private void Move(Direction direction)
	{
		Cell nextCell;
		Tile addedSnakeTile;
		Vector2Int headPosition = head.GetCell().GetCoordinates();
		int nextX = headPosition.x;
		int nextY = headPosition.y;

		// Xác định vị trí tiếp theo dựa trên hướng di chuyển
		switch (direction)
		{
			case Direction.Left:
				nextX = headPosition.x - 1;
				break;
			case Direction.Right:
				nextX = headPosition.x + 1;
				break;
			case Direction.Up:
				nextY = headPosition.y - 1;
				break;
			case Direction.Down:
				nextY = headPosition.y + 1;
				break;
		}

		// Xử lý khi đi ra ngoài biên
		if (nextX < 0)
		{
			nextX = grid.GetWidth() - 1;
		}
		else if (nextX >= grid.GetWidth())
		{
			nextX = 0;
		}
		else if (nextY < 0)
		{
			nextY = grid.GetHeight() - 1;
		}
		else if (nextY >= grid.GetHeight())
		{
			nextY = 0;
		}

		nextCell = grid.Rows[nextY].Cells[nextX];

		// Kiểm tra va chạm với chính mình
		if (nextCell.HasSnakeTile())
		{
			Time.timeScale = 0;
			Debug.Log("gameOver");
			OnGameOver?.Invoke(this, EventArgs.Empty);
			return;
		}

		// Kiểm tra va chạm với trái cây
		if (nextCell.HasFruitTile())
		{
			nextCell.GetFruitTile().SetCell(null);
			nextCell.SetFruitTile(null);
			addedSnakeTile = Instantiate(tilePrefab, this.transform);
			snakeTiles.Add(addedSnakeTile);
			SpawnFruit();
		}

		// Cập nhật vị trí của các phần thân rắn
		for (int i = snakeTiles.Count - 1; i >= 1; i--)
		{
			snakeTiles[i].GetCell()?.SetSnakeTile(null);
			snakeTiles[i].SetCell(snakeTiles[i - 1].GetCell());
			snakeTiles[i].GetCell().SetSnakeTile(snakeTiles[i]);
			snakeTiles[i].transform.position = snakeTiles[i].GetCell().transform.position;
		}



		nextCell.SetSnakeTile(head);
		head.SetCell(nextCell);
		head.transform.position = nextCell.transform.position;

		foreach (var snakeTile in snakeTiles)
		{
			if (!snakeTile.GetCell().HasSnakeTile())
			{
				snakeTile.GetCell().SetSnakeTile(snakeTile);
			}
		}

		IsMovedDone = true;
	}

	public void RestartGame()
	{
		Time.timeScale = 1;
		foreach (var snakeTile in snakeTiles)
		{
			snakeTile.GetCell().SetSnakeTile(null);
			Destroy(snakeTile.gameObject);
		}

		snakeTiles.Clear();
		IsMovedDone = true;
		fruit.GetCell().SetFruitTile(null);

		SpawnFruit();
		SpawnSnake();
	}
}
