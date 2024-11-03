namespace PropHunt
{
    public static class CustomGameOptions
    {        
        public static float hidingTime => CustomOption.CustumOptions.HideTime.Get();
        public static int maxMissedKills => (int)CustomOption.CustumOptions.MaximumMissedKills.Get();
        public static bool infection => CustomOption.CustumOptions.InfectionMode.Get();
    }
}