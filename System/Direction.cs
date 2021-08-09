using UnityEngine;

public enum Direction {
	E, W, None
}


public static class DirectionExtensions {
	public static readonly string[] DirectionStrings = {"East", "West"};

	public static Direction Opposite (this Direction direction) {
		return (Direction)(((int)direction + 1) % 2);
	}

	public static int ToInt (this Direction direction) {
		switch(direction)
		{
			case Direction.E:
				return 1;
			case Direction.W:
				return -1;
			default:
				return 0;
		}
	}
}
