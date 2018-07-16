namespace CrusaderKingsStoryGen
{
    public class GenerationOptions
    {
        public static int IdealIndependentEmpireCount = 8;
        public static int IdealIndependentKingCount = 8;
        public static int IdealIndependentDukeCount = 0;
        public static int MaxConcurrentConquerers = 4;

        #region

        public static int KingdomStability
        {
            get { return _kingdomStability; }

            set
            {
                _kingdomStability = value;

                switch (_kingdomStability)
                {
                    case 0:
                        IdealIndependentKingCount = 32;
                        break;
                    case 1:
                        IdealIndependentKingCount = 16;
                        break;
                    case 2:
                        IdealIndependentKingCount = 8;
                        break;
                    case 3:
                        IdealIndependentKingCount = 4;
                        break;
                    case 4:
                        IdealIndependentKingCount = 2;
                        break;
                    case 5:
                        IdealIndependentKingCount = 1;
                        break;
                }
            }
        }

        #endregion

        #region EmpireStability
        public static int EmpireStability
        
        {
            get { return _empireStability; }

            set
            {
                _empireStability = value;

                switch (_empireStability)
                {
                    case 0:
                        IdealIndependentEmpireCount = 32;
                        break;
                    case 1:
                        IdealIndependentEmpireCount = 16;
                        break;
                    case 2:
                        IdealIndependentEmpireCount = 8;
                        break;
                    case 3:
                        IdealIndependentEmpireCount = 4;
                        break;
                    case 4:
                        IdealIndependentEmpireCount = 2;
                        break;
                    case 5:
                        IdealIndependentEmpireCount = 1;
                        break;
                }
            }
        }

        #endregion

        public static int GovernmentMutate { get; set; } = 2;
        public static int ReligionMutate { get; set; } = 2;
        public static int CultureMutate { get; set; } = 2;
        public static int TechAdvanceRate { get; set; } = 2;
        public static int TechSpreadRate { get; set; } = 2;
        public static int HoldingDevSpeed { get; set; } = 2;

         #region Conquerers

        private static int _conquerers = 2;
        private static int _empireStability = 2;
        private static int _kingdomStability = 2;

        public static int Conquerers
        {
            get { return _conquerers; }

            set
            {
                _conquerers = value;

                switch (value)
                {
                    case 0:
                        MaxConcurrentConquerers = 32;
                        break;
                    case 1:
                        MaxConcurrentConquerers = 16;
                        break;
                    case 2:
                        MaxConcurrentConquerers = 8;
                        break;
                    case 3:
                        MaxConcurrentConquerers = 4;
                        break;
                    case 4:
                        MaxConcurrentConquerers = 2;
                        break;
                    case 5:
                        MaxConcurrentConquerers = 1;
                        break;
                }
            }
        }
        #endregion
    }
}
