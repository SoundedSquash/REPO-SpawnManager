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
        
        // LevelValuables
        public static string GetKey(this LevelValuables levelValuables)
        {
            return levelValuables.name;
        }
    }
}