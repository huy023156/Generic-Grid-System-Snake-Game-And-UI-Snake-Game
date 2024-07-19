using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeCellGridObject 
{
	public enum Status
	{
		Blank,
		Snake,
		Fruit
	}

	private Grid<SnakeCellGridObject> grid;
	private int x, y;

	public Status currentStatus;


	
	public SnakeCellGridObject(Grid<SnakeCellGridObject> grid, int x, int y)
	{
		this.grid = grid;
		this.x = x;
		this.y = y;
	}

	public void ChangeStatus(Status status)
	{
		currentStatus = status;
		grid.TriggerGridObjectChanged(x, y);
	}


	public Status GetCurrentStatus()
	{
		return currentStatus;
	}

	public Vector2Int GetCoordinate()
	{
		return new Vector2Int(x, y);
	}

	public override string ToString()
	{
		return currentStatus.ToString();
	}
}
