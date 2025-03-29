namespace SpawnManager.Extensions
{
    public static class Extensions
    {
        // ValuableObject
        public static string FriendlyName(this ValuableObject valuableObject)
        {
            return valuableObject.name.Replace("Valuable ", "");
        }
        
        // Level
        public static string GetKey(this Level level)
        {
            return level.name;
        }
        public static string FriendlyName(this Level level)
        {
            return level.name.Replace("Level - ", "");
        }
        
        // LevelValuables
        public static string GetKey(this LevelValuables levelValuables)
        {
            return levelValuables.name;
        }

        public static bool IsGenericList(this LevelValuables levelValuables)
        {
            return levelValuables.name.Contains("Generic");
        }
    }
}