// <copyright file="DateFunctions.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Helpers
{
    using CrusaderKingsStoryGen.Managers;

    public static class DateHelper
    {
        public static int MakeDOBAtLeastAdult(int inDOB)
        {
            int age = SimulationManager.instance.Year + 1 - inDOB;

            if (age < 16)
            {
                inDOB -= (16 - age);
            }

            return inDOB;
        }
    }
}