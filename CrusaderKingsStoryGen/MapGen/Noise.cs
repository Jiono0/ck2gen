// <copyright file="Noise.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.MapGen
{
    using System;

    public class NoiseGenerator
    {
        public int Seed { get; private set; }

        public int Octaves { get; set; }

        public double Amplitude { get; set; }

        public double Persistence { get; set; }

        public double FrequencyX { get; set; }

        public double FrequencyY { get; set; }

        public NoiseGenerator(int seed)
        {
            Random r = new Random(seed);
            //LOOOL
            this.Seed = RandomIntHelper.Next(int.MaxValue);
            this.Octaves = 8;
            this.Amplitude = 1;
            this.FrequencyX = 0.015;
            this.FrequencyY = 0.015;
            this.Persistence = 0.65;
        }

        public NoiseGenerator()
        {
            Random r = new Random();
            //LOOOL
            this.Seed = RandomIntHelper.Next(int.MaxValue);
            this.Octaves = 8;
            this.Amplitude = 1;
            this.FrequencyX = 0.015;
            this.FrequencyY = 0.015;
            this.Persistence = 0.65;
        }

        public double Noise(double x, double y)
        {
            y *= 1.5f;
            //returns -1 to 1
            double total = 0.0;
            double freqx = this.FrequencyX, amp = this.Amplitude, freqy = this.FrequencyY;
            for (int i = 0; i < this.Octaves; ++i)
            {
                total = total + this.Smooth(x * freqx, y * freqy) * amp;
                freqx *= 2;
                freqy *= 2;
                amp *= this.Persistence;
            }

            if (total < -2.4)
            {
                total = -2.4;
            }
            else if (total > 2.4)
            {
                total = 2.4;
            }

            return ((total / 2.4) / 2.0f) + 0.5f;
        }

        public double Noise(double x, double y, float mul)
        {
            //returns -1 to 1
            double total = 0.0;
            double freqx = this.FrequencyX, amp = this.Amplitude, freqy = this.FrequencyY;
            for (int i = 0; i < this.Octaves; ++i)
            {
                total = total + this.Smooth(x * freqx, y * freqy) * amp;
                freqx *= 2;
                freqy *= 2;
                x *= mul;
                y *= mul;
                amp *= this.Persistence;
            }

            if (total < -2.4)
            {
                total = -2.4;
            }
            else if (total > 2.4)
            {
                total = 2.4;
            }

            return (total / 2.4);
        }

        public double NoiseGeneration(int x, int y)
        {
            int n = x + y * 57;
            n = (n << 13) ^ n;

            return (1.0 - ((n * (n * n * 15731 + 789221) + this.Seed) & 0x7fffffff) / 1073741824.0);
        }

        private double Interpolate(double x, double y, double a)
        {
            double value = (1 - Math.Cos(a * Math.PI)) * 0.5;
            return x * (1 - value) + y * value;
        }

        private double Smooth(double x, double y)
        {
            double ox = x;
            double oy = y;
            x *= this.Mul;
            y *= this.Mul;
            double n1 = this.NoiseGeneration((int)x, (int)y);
            double n2 = this.NoiseGeneration((int)x + 1, (int)y);
            double n3 = this.NoiseGeneration((int)x, (int)y + 1);
            double n4 = this.NoiseGeneration((int)x + 1, (int)y + 1);

            double i1 = this.Interpolate(n1, n2, x - (int)x);
            double i2 = this.Interpolate(n3, n4, x - (int)x);



            return this.Interpolate(i1, i2, y - (int)y);
        }

        public double Mul = 512;
    }
}
