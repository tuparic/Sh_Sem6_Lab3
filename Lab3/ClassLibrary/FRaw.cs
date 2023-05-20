using System;

namespace ClassLibrary
{

    public delegate double FRaw(double x);
    public enum FRawEnum { linear, cube, rand };
    public static class FRawFunctions
    {
        public static double linear(double x)
        {
            return x;
        }

        public static double cube(double x)
        {
            return x * x * x;
        }

        public static double rand(double x)
        {
            Random rnd = new Random();
            double value = rnd.NextDouble() * 20 - 10;
            return value;
        }
    }
}
