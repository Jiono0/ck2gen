// <copyright file="Rand.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Helpers
{
    using System;

    public class RandomIntHelper
    {
        internal static Random rand = new Random(Seed);

        public static int Seed { get; set; } = 1;

        public static int Next(int max)
        {
            return rand.Next(max);
        }

        public static int Next(int minSize, int maxSize)
        {
            return rand.Next(minSize, maxSize);
        }

        public static void SetSeed(int i)
        {
            Seed = i;
            rand = new Random(Seed);
        }

        public static void SetSeed()
        {
            rand = new Random();
            Seed = rand.Next(100000);
            rand = new Random(Seed);
        }

        public static Random Get()
        {
            return rand;
        }
    }
}
