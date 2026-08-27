namespace System;
public static class DoubleExtensionMethods
{
    public static double Truncate2(this double value)
    {
        return Math.Truncate(value * 100) / 100;
    }
}
