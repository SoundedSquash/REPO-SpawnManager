namespace SpawnManager.Extensions
{
    public static class ValuableObjectExtensions
    {
        public static string FriendlyName(this ValuableObject valuableObject)
        {
            return valuableObject.name.Replace("Valuable ", "");
        }
    }
}