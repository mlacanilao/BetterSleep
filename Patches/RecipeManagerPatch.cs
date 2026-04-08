namespace BetterSleep.Patches;

internal static class RecipeManagerPatch
{
    public static void GetRandomRecipePrefix(ref bool onlyUnlearned)
    {
        bool enableOnlyUnlearnedRecipes = BetterSleepConfig.EnableOnlyUnlearnedRecipes?.Value ?? false;

        if (enableOnlyUnlearnedRecipes == false)
        {
            return;
        }

        onlyUnlearned = true;
    }
}
