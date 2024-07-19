using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeCube
{
	protected SnakeCellGridObject cell;

	public void SetCell(SnakeCellGridObject cell)
	{ 
		this.cell = cell;
		this.cell.ChangeStatus(SnakeCellGridObject.Status.Snake);
	}

	public SnakeCellGridObject GetCell()
	{
		return cell;
	}
}
