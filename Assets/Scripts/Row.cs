using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
	public Cell[] Cells { get; private set; }

	private void Awake()
	{
		Cells = GetComponentsInChildren<Cell>();
	}
}