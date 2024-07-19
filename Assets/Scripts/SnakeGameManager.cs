using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeGameManager : MonoBehaviour
{
	[SerializeField] private SnakeMapVisual snakeMapVisual;
	private Grid<SnakeCellGridObject> grid;
	private List<SnakeCube> snakeCubeList;
	private SnakeCellGridObject fruitCell;

	public bool IsMovedDone { get; private set; }

	private float stepTimer;
	[SerializeField]
	private float stepTimerMax = 0.5f;

	public enum Direction
	{
		Up,
		Down,
		Left,
		Right
	}

	private Direction currentDirection;

	private void Start()
	{
		grid = new Grid<SnakeCellGridObject>(8, 4, 15f, new Vector3(-60, -30), (Grid<SnakeCellGridObject> g, int x, int y) => { return new SnakeCellGridObject(g, x, y); });
		snakeCubeList = new List<SnakeCube>();
		snakeMapVisual.SetGrid(grid);
		currentDirection = Direction.Right;
		IsMovedDone = true;
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
		//if (Input.GetKeyDown(KeyCode.Escape))
		//{
		//	RestartGame();
		//}

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
		if (fruitCell != null) {
			fruitCell.ChangeStatus(SnakeCellGridObject.Status.Blank);
		}

		fruitCell = GetRandomCellToSpawnTile();
		fruitCell.ChangeStatus(SnakeCellGridObject.Status.Fruit);
	}

	private SnakeCellGridObject GetRandomCellToSpawnTile()
	{
		System.Random random = new System.Random();

		SnakeCellGridObject[] flattenGrid = UtilsClass.Flatten(grid.GetGridObjectArray());

		int randomIndex = random.Next(0, flattenGrid.Length);

		int originRandomIndex = randomIndex;

		while (true)
		{
			if (!(flattenGrid[randomIndex].GetCurrentStatus() == SnakeCellGridObject.Status.Snake) && !(flattenGrid[randomIndex].GetCurrentStatus() == SnakeCellGridObject.Status.Fruit))
			{
				return flattenGrid[randomIndex];
			}

			randomIndex++;
			if (randomIndex >= flattenGrid.Length)
			{
				randomIndex = 0;
			}

			if (randomIndex == originRandomIndex)
				return null;
		}
	}

	private void SpawnSnake()
	{
		int xMiddle = grid.GetWidth() / 2;
		int yMiddle = grid.GetHeight() / 2;	

		for (int i = 0; i < 3; i++)
		{
			Debug.Log(xMiddle - i + " " + yMiddle);
			SnakeCube cube = new SnakeCube();
			Debug.Log("cube:" + cube);
			SnakeCellGridObject cell = grid.GetGridObject(xMiddle - i, yMiddle);
			cube.SetCell(cell);
			Debug.Log("cell:" + cell);
			snakeCubeList.Add(cube);
		}
	}

	private void Move(Direction direction)
	{
		SnakeCellGridObject nextCell;
		SnakeCube addedSnakeCube;
		Vector2Int headPosition = snakeCubeList[0].GetCell().GetCoordinate();
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
				nextY = headPosition.y + 1;
				break;
			case Direction.Down:
				nextY = headPosition.y - 1;
				break;
		}

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

		nextCell = grid.GetGridObject(nextX, nextY);

		// Kiểm tra va chạm với chính mình
		if (nextCell.GetCurrentStatus() == SnakeCellGridObject.Status.Snake && nextCell != snakeCubeList[snakeCubeList.Count - 1].GetCell())
		{
			Time.timeScale = 0;
			Debug.Log("gameOver");
			return;
		}

		// Kiểm tra va chạm với trái cây
		if (nextCell.GetCurrentStatus() == SnakeCellGridObject.Status.Fruit)
		{
			addedSnakeCube = new SnakeCube();
			snakeCubeList.Add(addedSnakeCube);
			SpawnFruit();
		} else
		{
			snakeCubeList[snakeCubeList.Count - 1].GetCell().ChangeStatus(SnakeCellGridObject.Status.Blank);
		}

		// Cập nhật vị trí của các phần thân rắn
		for (int i = snakeCubeList.Count - 1; i >= 1; i--)
		{
			snakeCubeList[i].SetCell(snakeCubeList[i - 1].GetCell());
		}

		snakeCubeList[0].SetCell(nextCell);


		IsMovedDone = true;
	}
}
