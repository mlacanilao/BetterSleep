namespace BetterSleep.Patches
{
    public static class RecipeManagerPatch
    {
        public static void GetRandomRecipePrefix(ref bool onlyUnlearned)
        {
            bool enableOnlyUnlearnedRecipes = BetterSleepConfig.EnableOnlyUnlearnedRecipes?.Value ?? false;

            if (enableOnlyUnlearnedRecipes == true)
            {
                onlyUnlearned = true;
            }
        }
    }
}