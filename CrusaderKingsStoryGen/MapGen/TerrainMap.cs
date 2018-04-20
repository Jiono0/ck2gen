// <copyright file="TerrainMap.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>



namespace CrusaderKingsStoryGen.MapGen
{
    using CrusaderKingsStoryGen.Helpers;
    using LibNoise;
    using LibNoise.Modfiers;
    using LibNoise.Modifiers;

    public class TerrainMap
    {
        Perlin terrainType = new Perlin();
        RidgedMultifractal mountainTerrain = new RidgedMultifractal();
        Billow baseLandTerrain = new Billow();
        Billow baseWaterTerrain = new Billow();
        private NoiseTextureHelper output;

        public NoiseTextureHelper ResultBitmap { get; set; }

        private double seaLevel = 0.07f;
        public float ZoomMultiplier = 1.7f;
        public float MinLandFreq = 0.1f;
        public float MaxLandFreq = 1f;

        public TerrainMap(LockBitmap landBitmap = null, LockBitmap hillBitmap = null, LockBitmap mountainBitmap = null)
        {
            this.LandBitmap = landBitmap;

            this.MountainBitmap = mountainBitmap;
        }

        public LockBitmap LandBitmap { get; set; }

        public LockBitmap MountainBitmap { get; set; }

        public void Init(int seed, int width, int height)
        {
            if (this.LandBitmap != null && width != this.LandBitmap.Width)
            {
                this.LandBitmap.ResizeImage(width, height, false);
            }

            if (this.MountainBitmap != null && width != this.MountainBitmap.Width)
            {
                this.MountainBitmap.ResizeImage(width, height, false);
             }

            float delta = width / 3072.0f;
            delta *= this.ZoomMultiplier;
            this.DivNoise = delta;

            //   Rand.SetSeed(seed);
            this.baseWaterTerrain.Frequency = 2.0;
            this.baseLandTerrain.Frequency = (2.0);
            ScaleBiasOutput flatTerrain = new ScaleBiasOutput(this.baseLandTerrain);
            flatTerrain.Scale = 0.005;
            flatTerrain.Bias = this.seaLevel; //SeaLevel;
            this.MinLandFreq = 0.2f;
            this.MaxLandFreq = 1f;
            if (this.LandBitmap != null)
            {
                this.MinLandFreq = 0.7f;
            }

            ScaleBiasOutput hillTerrain = new ScaleBiasOutput(this.baseLandTerrain);
            hillTerrain.Scale = 0.09;
            hillTerrain.Bias = this.seaLevel + 0.2; //SeaLevel;

            ScaleBiasOutput waterTerrain = new ScaleBiasOutput(this.baseWaterTerrain);
            waterTerrain.Bias = -0.33f; //SeaLevel;
            waterTerrain.Scale = 0.001;

            Perlin waterLandType = new Perlin();
            float landFreq = RandomIntHelper.Next((int)(this.MinLandFreq * 10000), (int)(this.MaxLandFreq * 10000)) / 10000.0f;
            waterLandType.Persistence = 0.45;
            waterLandType.Frequency = landFreq;
            //waterLandType.OctaveCount = 12;
            waterLandType.Seed = RandomIntHelper.Next(1000000);

            Select waterLandSelector = new Select(waterLandType, waterTerrain, flatTerrain);

            if (this.LandBitmap != null)
            {
                waterLandSelector = new BitmapSelect(waterLandType, waterTerrain, flatTerrain, this.DivNoise, this.LandBitmap);
             }

            waterLandSelector.EdgeFalloff = (0.145);
            waterLandSelector.SetBounds(-0.0, 1000); ;


            Select landHillSelector = new Select(waterLandType, waterLandSelector, hillTerrain);

            if (this.LandBitmap != null)
            {
                landHillSelector = new BitmapSelect(waterLandType, waterLandSelector, hillTerrain, this.DivNoise, this.LandBitmap);
             }

            landHillSelector.EdgeFalloff = (0.45);
            landHillSelector.SetBounds(0.25f, 1000); ;

            this.terrainType.Persistence = 0.3;
            this.terrainType.Frequency = 0.3;
            this.terrainType.Seed = RandomIntHelper.Next(10000000);

            var clamp = new ClampOutput(this.terrainType);
            clamp.SetBounds(0, 1);
            //            mountainTerrain.Frequency /= 1.5f;
            this.mountainTerrain.Lacunarity = 35;
            this.mountainTerrain.Frequency = 3.2;
            this.mountainTerrain.Seed = RandomIntHelper.Next(10000000);
            MultiplyPositive mul = new MultiplyPositive(waterLandType, waterLandType);

            ScaleOutput scaled = new ScaleOutput(mul, 0.00001);

            Add add = new Add(new BiasOutput(this.mountainTerrain, 0.8 + this.seaLevel), landHillSelector);

            MultiplyPositive mul2 = new MultiplyPositive(add, add);
            MultiplyPositive mul3 = new MultiplyPositive(clamp, mul);

            Select terrainSelector = new Select(mul3, landHillSelector, add);

            if (this.MountainBitmap != null)
            {
                terrainSelector = new BitmapSelect(mul3, landHillSelector, add, this.DivNoise, this.MountainBitmap);
             }

            terrainSelector.EdgeFalloff = (7.925);
            terrainSelector.SetBounds(0.3, 1000);

            Turbulence finalTerrain = new Turbulence(terrainSelector);
            finalTerrain.Frequency = 4;
            finalTerrain.Power = 0.075;
            this.Width = width;
            this.Height = height;
            //   ResultBitmap2 = new NoiseTexture(width, height, clamp);
            //   System.Console.Out.WriteLine("Left: " + ResultBitmap2.minRange + " - " + ResultBitmap2.maxRange);

            //   ResultBitmap2 = new NoiseTexture(width, height, finalTerrain, DivNoise, 1.25f, -0.66f);
            //    System.Console.Out.WriteLine("Left: " + ResultBitmap2.minRange + " - " + ResultBitmap2.maxRange);
            this.ResultBitmap = new NoiseTextureHelper(width, height, finalTerrain, this.DivNoise, 1.25f, -0.66f);
            System.Console.Out.WriteLine("Right: " + this.ResultBitmap.minRange + " - " + this.ResultBitmap.maxRange);
        }

        public void InitGen(int seed, int width, int height)
        {
            if (this.LandBitmap != null && width != this.LandBitmap.Width)
            {
                this.LandBitmap.ResizeImage(width, height, false);
            }

            if (this.MountainBitmap != null && width != this.MountainBitmap.Width)
            {
                this.MountainBitmap.ResizeImage(width, height, false);
            }

            float delta = width / 3072.0f;
            delta *= this.ZoomMultiplier;
            this.DivNoise = delta;

            //   Rand.SetSeed(seed);
            this.baseWaterTerrain.Frequency = 2.0;
            this.baseLandTerrain.Frequency = (2.0);
            ScaleBiasOutput flatTerrain = new ScaleBiasOutput(this.baseLandTerrain);
            flatTerrain.Scale = 0.005;
            flatTerrain.Bias = this.seaLevel; //SeaLevel;
            this.MinLandFreq = 0.2f;
            this.MaxLandFreq = 1f;
            if (this.LandBitmap != null)
            {
                this.MinLandFreq = 0.7f;
            }

            ScaleBiasOutput hillTerrain = new ScaleBiasOutput(this.baseLandTerrain);
            hillTerrain.Scale = 0.09;
            hillTerrain.Bias = this.seaLevel + 0.2; //SeaLevel;

            ScaleBiasOutput waterTerrain = new ScaleBiasOutput(this.baseWaterTerrain);
            waterTerrain.Bias = -0.33f; //SeaLevel;
            waterTerrain.Scale = 0.001;

            Perlin waterLandType = new Perlin();
            float landFreq = RandomIntHelper.Next((int)(this.MinLandFreq * 10000), (int)(this.MaxLandFreq * 10000)) / 10000.0f;
            waterLandType.Persistence = 0.45;
            waterLandType.Frequency = landFreq;
            //waterLandType.OctaveCount = 12;
            waterLandType.Seed = RandomIntHelper.Next(1000000);

            Select waterLandSelector = new Select(waterLandType, waterTerrain, flatTerrain);

            waterLandSelector.EdgeFalloff = (0.145);
            waterLandSelector.SetBounds(-0.0, 1000); ;


            Select landHillSelector = new Select(waterLandType, waterLandSelector, hillTerrain);


            landHillSelector.EdgeFalloff = (0.45);
            landHillSelector.SetBounds(0.25f, 1000); ;

            this.terrainType.Persistence = 0.3;
            this.terrainType.Frequency = 0.3;
            this.terrainType.Seed = RandomIntHelper.Next(10000000);

            var clamp = new ClampOutput(this.terrainType);
            clamp.SetBounds(0, 1);
            //            mountainTerrain.Frequency /= 1.5f;
            this.mountainTerrain.Lacunarity = 35;
            this.mountainTerrain.Frequency = 3.2;
            this.mountainTerrain.Seed = RandomIntHelper.Next(10000000);
            MultiplyPositive mul = new MultiplyPositive(waterLandType, waterLandType);

            ScaleOutput scaled = new ScaleOutput(mul, 0.00001);

            Add add = new Add(new BiasOutput(this.mountainTerrain, 0.8 + this.seaLevel), landHillSelector);

            MultiplyPositive mul2 = new MultiplyPositive(add, add);
            MultiplyPositive mul3 = new MultiplyPositive(clamp, mul);

            Select terrainSelector = new Select(mul3, landHillSelector, add);

            terrainSelector.EdgeFalloff = (0.325);
            terrainSelector.SetBounds(0.28, 1000);

            Turbulence finalTerrain = new Turbulence(terrainSelector);
            finalTerrain.Frequency = 4;
            finalTerrain.Power = 0.075;
            this.Width = width;
            this.Height = height;
            //   ResultBitmap2 = new NoiseTexture(width, height, clamp);
            //   System.Console.Out.WriteLine("Left: " + ResultBitmap2.minRange + " - " + ResultBitmap2.maxRange);

            //   ResultBitmap2 = new NoiseTexture(width, height, finalTerrain, DivNoise, 1.25f, -0.66f);
            //    System.Console.Out.WriteLine("Left: " + ResultBitmap2.minRange + " - " + ResultBitmap2.maxRange);
            this.ResultBitmap = new NoiseTextureHelper(width, height, finalTerrain, this.DivNoise, 1.25f, -0.66f);
            System.Console.Out.WriteLine("Right: " + this.ResultBitmap.minRange + " - " + this.ResultBitmap.maxRange);
        }

        public void InitFromExisting(int seed, int width, int height, float[,] selectionMap)
        {
            float delta = width / 3072.0f;
            delta *= this.ZoomMultiplier;
            this.DivNoise = delta;

            //   Rand.SetSeed(seed);
            this.baseWaterTerrain.Frequency = 2.0;
            this.baseLandTerrain.Frequency = (2.0);
            ScaleBiasOutput flatTerrain = new ScaleBiasOutput(this.baseLandTerrain);
            flatTerrain.Scale = 0.005;
            flatTerrain.Bias = this.seaLevel; //SeaLevel;

            ScaleBiasOutput hillTerrain = new ScaleBiasOutput(this.baseLandTerrain);
            hillTerrain.Scale = 0.065;
            hillTerrain.Bias = this.seaLevel + 0.2; //SeaLevel;

            ScaleBiasOutput waterTerrain = new ScaleBiasOutput(this.baseWaterTerrain);
            waterTerrain.Bias = -0.73f; //SeaLevel;
            waterTerrain.Scale = 0.05;

            Perlin waterLandType = new Perlin();

            waterLandType.Persistence = 0.45;
            waterLandType.Frequency = 0.5;
            //waterLandType.OctaveCount = 12;
            waterLandType.Seed = RandomIntHelper.Next(1000000);
            Select waterLandSelector = new Select(waterLandType, waterTerrain, flatTerrain);
            waterLandSelector.EdgeFalloff = (0.045);
            waterLandSelector.SetBounds(-0.0, 1000); ;

            Select landHillSelector = new Select(waterLandType, waterLandSelector, hillTerrain);
            landHillSelector.EdgeFalloff = (0.15);
            landHillSelector.SetBounds(0.4, 1000); ;


            this.terrainType.Persistence = 0.3;
            this.terrainType.Frequency = 0.3;
            this.terrainType.Seed = RandomIntHelper.Next(10000000);

            var clamp = new ClampOutput(this.terrainType);
            clamp.SetBounds(0, 1);
            //            mountainTerrain.Frequency /= 1.5f;
            this.mountainTerrain.Lacunarity = 30;
            this.mountainTerrain.Frequency = 1.9;
            MultiplyPositive mul = new MultiplyPositive(waterLandType, waterLandType);

            ScaleOutput scaled = new ScaleOutput(mul, 0.00001);

            Add add = new Add(new BiasOutput(this.mountainTerrain, 1 + this.seaLevel), landHillSelector);

            MultiplyPositive mul2 = new MultiplyPositive(mul, mul);
            MultiplyPositive mul3 = new MultiplyPositive(clamp, mul);

            Select terrainSelector = new Select(mul3, landHillSelector, add);
            terrainSelector.EdgeFalloff = (0.425);

            terrainSelector.SetBounds(0.2, 1000);

            Turbulence finalTerrain = new Turbulence(terrainSelector);
            finalTerrain.Frequency = 4;
            finalTerrain.Power = 0.075;
            this.Width = width;
            this.Height = height;
            //   ResultBitmap2 = new NoiseTexture(width, height, clamp);
            //   System.Console.Out.WriteLine("Left: " + ResultBitmap2.minRange + " - " + ResultBitmap2.maxRange);

            //   ResultBitmap2 = new NoiseTexture(width, height, finalTerrain, DivNoise, 1.25f, -0.66f);
            //    System.Console.Out.WriteLine("Left: " + ResultBitmap2.minRange + " - " + ResultBitmap2.maxRange);
            this.ResultBitmap = new NoiseTextureHelper(width, height, finalTerrain, this.DivNoise, 1.25f, -0.66f);
            System.Console.Out.WriteLine("Right: " + this.ResultBitmap.minRange + " - " + this.ResultBitmap.maxRange);
        }

        public void InitO(int seed, int width, int height)
        {
            float delta = width / 3072.0f;
            delta *= this.ZoomMultiplier;
            this.DivNoise = delta;

            //   Rand.SetSeed(seed);
            this.baseWaterTerrain.Frequency = 2.0;
            this.baseLandTerrain.Frequency = (2.0);
            ScaleBiasOutput flatTerrain = new ScaleBiasOutput(this.baseLandTerrain);
            flatTerrain.Scale = 0.005;
            flatTerrain.Bias = this.seaLevel; //SeaLevel;

            ScaleBiasOutput hillTerrain = new ScaleBiasOutput(this.baseLandTerrain);
            hillTerrain.Scale = 0.065;
            hillTerrain.Bias = this.seaLevel + 0.2; //SeaLevel;

            ScaleBiasOutput waterTerrain = new ScaleBiasOutput(this.baseWaterTerrain);
            waterTerrain.Bias = -0.73f; //SeaLevel;
            waterTerrain.Scale = 0.05;

            Perlin waterLandType = new Perlin();

            waterLandType.Persistence = 0.45;
            waterLandType.Frequency = 0.5;
            //waterLandType.OctaveCount = 12;
            waterLandType.Seed = RandomIntHelper.Next(1000000);
            Select waterLandSelector = new Select(waterLandType, waterTerrain, flatTerrain);
            waterLandSelector.EdgeFalloff = (0.045);
            waterLandSelector.SetBounds(-0.0, 1000); ;

            Select landHillSelector = new Select(waterLandType, waterLandSelector, hillTerrain);
            landHillSelector.EdgeFalloff = (0.15);
            landHillSelector.SetBounds(0.4, 1000); ;


            this.terrainType.Persistence = 0.3;
            this.terrainType.Frequency = 0.3;
            this.terrainType.Seed = RandomIntHelper.Next(10000000);

            var clamp = new ClampOutput(this.terrainType);
            clamp.SetBounds(0, 1);
            //            mountainTerrain.Frequency /= 1.5f;
            this.mountainTerrain.Lacunarity = 30;
            this.mountainTerrain.Frequency = 1.3;
            MultiplyPositive mul = new MultiplyPositive(waterLandType, waterLandType);

            ScaleOutput scaled = new ScaleOutput(mul, 0.00001);

            Add add = new Add(new BiasOutput(this.mountainTerrain, 1 + this.seaLevel), landHillSelector);

            MultiplyPositive mul2 = new MultiplyPositive(mul, mul);
            MultiplyPositive mul3 = new MultiplyPositive(clamp, mul);

            Select terrainSelector = new Select(mul3, landHillSelector, add);
            terrainSelector.EdgeFalloff = (0.425);

            terrainSelector.SetBounds(0.3, 1000);

            Turbulence finalTerrain = new Turbulence(terrainSelector);
            finalTerrain.Frequency = 4;
            finalTerrain.Power = 0.075;
            this.Width = width;
            this.Height = height;
            //   ResultBitmap2 = new NoiseTexture(width, height, clamp);
            //   System.Console.Out.WriteLine("Left: " + ResultBitmap2.minRange + " - " + ResultBitmap2.maxRange);

            this.ResultBitmap = new NoiseTextureHelper(width, height, finalTerrain, this.DivNoise, 1.25f, -0.66f);
            System.Console.Out.WriteLine("Range: " + this.ResultBitmap.minRange + " - " + this.ResultBitmap.maxRange);
        }

        public float DivNoise { get; set; }

        public NoiseTextureHelper ResultBitmap2 { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }
    }
}
