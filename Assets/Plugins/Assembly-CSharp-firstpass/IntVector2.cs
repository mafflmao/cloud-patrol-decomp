using UnityEngine;

public struct IntVector2
{
	public int x;

	public int y;

	public static IntVector2 zero
	{
		get
		{
			return new IntVector2(0, 0);
		}
	}

	public static IntVector2 one
	{
		get
		{
			return new IntVector2(1, 1);
		}
	}

	public static IntVector2 up
	{
		get
		{
			return new IntVector2(0, 1);
		}
	}

	public static IntVector2 down
	{
		get
		{
			return new IntVector2(0, -1);
		}
	}

	public static IntVector2 right
	{
		get
		{
			return new IntVector2(1, 0);
		}
	}

	public static IntVector2 left
	{
		get
		{
			return new IntVector2(-1, 0);
		}
	}

	public IntVector2(int aX, int aY)
	{
		x = aX;
		y = aY;
	}

	public IntVector2(int aN)
	{
		x = aN;
		y = aN;
	}

	public override string ToString()
	{
		return string.Format("({0}, {1})", x, y);
	}

	public override bool Equals(object aObj)
	{
		if (aObj is IntVector2)
		{
			return this == (IntVector2)aObj;
		}
		return base.Equals(aObj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public void SwapXY()
	{
		int num = x;
		x = y;
		y = num;
	}

	public static IntVector2 operator +(IntVector2 a, IntVector2 b)
	{
		return new IntVector2(a.x + b.x, a.y + b.y);
	}

	public static IntVector2 operator +(IntVector2 a, int b)
	{
		return new IntVector2(a.x + b, a.y + b);
	}

	public static IntVector2 operator +(int a, IntVector2 b)
	{
		return new IntVector2(a + b.x, a + b.y);
	}

	public static IntVector2 operator -(IntVector2 a, IntVector2 b)
	{
		return new IntVector2(a.x - b.x, a.y - b.y);
	}

	public static IntVector2 operator -(IntVector2 a, int b)
	{
		return new IntVector2(a.x - b, a.y - b);
	}

	public static IntVector2 operator -(int a, IntVector2 b)
	{
		return new IntVector2(a - b.x, a - b.y);
	}

	public static IntVector2 operator -(IntVector2 a)
	{
		return new IntVector2(-a.x, -a.y);
	}

	public static IntVector2 operator *(IntVector2 a, int b)
	{
		return new IntVector2(a.x * b, a.y * b);
	}

	public static IntVector2 operator *(int a, IntVector2 b)
	{
		return new IntVector2(a * b.x, a * b.y);
	}

	public static Vector2 operator *(IntVector2 a, float b)
	{
		return new Vector2((float)a.x * b, (float)a.y * b);
	}

	public static Vector2 operator *(float a, IntVector2 b)
	{
		return new Vector2(a * (float)b.x, a * (float)b.y);
	}

	public static bool operator ==(IntVector2 a, IntVector2 b)
	{
		return a.x == b.x && a.y == b.y;
	}

	public static bool operator !=(IntVector2 a, IntVector2 b)
	{
		return a.x != b.x || a.y != b.y;
	}

	public static implicit operator Vector2(IntVector2 a)
	{
		return new Vector2(a.x, a.y);
	}
}
