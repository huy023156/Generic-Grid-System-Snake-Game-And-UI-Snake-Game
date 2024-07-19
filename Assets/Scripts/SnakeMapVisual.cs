using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SnakeMapVisual : MonoBehaviour
{
	private Grid<SnakeCellGridObject> grid;
	private Mesh mesh;
	private bool updateMesh;

	private void Awake()
	{
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
	}


	public void SetGrid(Grid<SnakeCellGridObject> grid)
	{
		this.grid = grid;
		UpdateVisual();

		grid.OnGridObjectChanged += Grid_OnGridObjectChanged;
	}

	private void Grid_OnGridObjectChanged(object sender, Grid<SnakeCellGridObject>.OnGridObjectChangedEventArgs e)
	{
		updateMesh = true;
	}

	private void LateUpdate()
	{
		if (updateMesh)
		{
			updateMesh = false;
			UpdateVisual();
		}
	}

	private void UpdateVisual()
	{
		MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

		for (int x = 0; x < grid.GetWidth(); x++)
		{
			for (int y = 0; y < grid.GetHeight(); y++)
			{
				int index = x * grid.GetHeight() + y;
				Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();

				SnakeCellGridObject gridObject = grid.GetGridObject(x, y);

				float gridValueNormalized;
				switch (gridObject.GetCurrentStatus())
				{
					case SnakeCellGridObject.Status.Blank:
						gridValueNormalized = 0;
						break;
					case SnakeCellGridObject.Status.Snake:
						gridValueNormalized = 1f;
						break;
					case SnakeCellGridObject.Status.Fruit:
						gridValueNormalized = .1f;
						break;
					default:
						gridValueNormalized = 0;
						break;
				}

				Vector2 gridValueUV = new Vector2(gridValueNormalized, 0f);
				MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridValueUV, gridValueUV);

			}
		}

		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
	}
}
