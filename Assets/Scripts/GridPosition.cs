[System.Serializable]
public struct GridPosition
{
	public GridPosition(int row, int column)
	{
		this.row = row;
		this.col = column;
	}

	public int row;
	public int col;
}
