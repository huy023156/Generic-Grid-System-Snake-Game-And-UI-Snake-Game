using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
	private Vector2Int coordinates;

	private Tile snakeTile;
	private Tile fruitTile;

	public void SetCoordinate(int x, int y)
	{
		coordinates = new Vector2Int(x, y);
	}

	public void SetSnakeTile(Tile tile)
	{
		this.snakeTile = tile;
	}

	public void SetFruitTile(Tile tile)
	{
		this.fruitTile = tile;
	}

	public Vector2Int GetCoordinates() => coordinates;

	public bool HasSnakeTile() => snakeTile != null;
	public bool HasFruitTile() => fruitTile != null;

	public Tile GetSnakeTile() => snakeTile;


	public Tile GetFruitTile() => fruitTile; 
}
