using System;

public static class Assert
{
	public static void AssertIsTrue(bool b)
	{
		if (!b)
		{
			throw new Exception("Assert failure");
		}
	}

	public static void AssertNotNull(object obj)
	{
		AssertIsTrue(!object.ReferenceEquals(null, obj));
	}
}
