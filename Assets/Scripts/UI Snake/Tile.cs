using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
	private Cell cell;

	private Image image;

	private void Awake()
	{
		image = GetComponent<Image>();
	}

	public void SetCell(Cell cell)
	{
		this.cell = cell;

	}

	public Image GetImage() => image;

	public Cell GetCell() => cell;
}
