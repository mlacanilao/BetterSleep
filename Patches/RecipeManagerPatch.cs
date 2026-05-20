using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace BetterSleep.Patches;

internal static class RecipeManagerPatch
{
    public static IEnumerable<CodeInstruction> OnSleepTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> instructionList = new List<CodeInstruction>(collection: instructions);

        MethodInfo? getRandomRecipe = AccessTools.Method(
            type: typeof(RecipeManager),
            name: nameof(RecipeManager.GetRandomRecipe),
            parameters: new[] { typeof(int), typeof(string), typeof(bool) });
        MethodInfo? recipeAdd = AccessTools.Method(
            type: typeof(RecipeManager),
            name: nameof(RecipeManager.Add),
            parameters: new[] { typeof(string), typeof(bool) });
        MethodInfo? shouldUseOnlyUnlearnedRecipes = AccessTools.Method(
            type: typeof(RecipeManagerPatch),
            name: nameof(ShouldUseOnlyUnlearnedRecipes),
            parameters: Array.Empty<Type>());

        if (getRandomRecipe == null ||
            recipeAdd == null ||
            shouldUseOnlyUnlearnedRecipes == null)
        {
            BetterSleep.LogError(message: "RecipeManager.OnSleep transpiler: required methods not found");
            return instructionList;
        }

        int matchCount = CountSleepRecipeCalls(
            instructions: instructionList,
            getRandomRecipe: getRandomRecipe,
            recipeAdd: recipeAdd);

        if (matchCount != 1)
        {
            BetterSleep.LogError(
                message: "RecipeManager.OnSleep transpiler expected 1 sleep recipe call but found " +
                    matchCount.ToString() +
                    ". Unlearned recipe preference during sleep will remain vanilla.");
            return instructionList;
        }

        CodeMatcher codeMatcher = new CodeMatcher(instructions: instructionList);

        codeMatcher.MatchStartForward(matches: new[]
        {
            new CodeMatch(opcode: OpCodes.Ldnull),
            new CodeMatch(opcode: OpCodes.Ldc_I4_0),
            new CodeMatch(predicate: instruction => instruction.Calls(method: getRandomRecipe)),
            new CodeMatch(opcode: OpCodes.Ldc_I4_1),
            new CodeMatch(predicate: instruction => instruction.Calls(method: recipeAdd))
        });

        if (codeMatcher.IsValid == false)
        {
            BetterSleep.LogError(
                message: "RecipeManager.OnSleep transpiler failed to replace the matched sleep recipe call. " +
                    "Unlearned recipe preference during sleep will remain vanilla.");
            return codeMatcher.Instructions();
        }

        codeMatcher.Advance(offset: 1);
        CodeInstruction originalInstruction = codeMatcher.Instruction;
        CodeInstruction replacementInstruction = new CodeInstruction(
                opcode: OpCodes.Call,
                operand: shouldUseOnlyUnlearnedRecipes)
            .WithLabels(labels: originalInstruction.labels)
            .WithBlocks(blocks: originalInstruction.blocks);

        BetterSleep.LogDebug(message: "RecipeManager.OnSleep transpiler: matched sleep recipe onlyUnlearned argument");
        codeMatcher.SetInstruction(instruction: replacementInstruction);

        return codeMatcher.Instructions();
    }

    private static bool ShouldUseOnlyUnlearnedRecipes()
    {
        bool shouldUseOnlyUnlearnedRecipes = BetterSleepConfig.EnableOnlyUnlearnedRecipes?.Value ?? false;
        FeatureTestLog.Log(
            feature: "Prioritize Unlearned Recipes",
            detail: "onlyUnlearned=" + shouldUseOnlyUnlearnedRecipes.ToString());

        return shouldUseOnlyUnlearnedRecipes;
    }

    private static int CountSleepRecipeCalls(
        List<CodeInstruction> instructions,
        MethodInfo getRandomRecipe,
        MethodInfo recipeAdd)
    {
        int count = 0;

        for (int i = 0; i < instructions.Count; i++)
        {
            if (IsSleepRecipeCallStart(
                instructions: instructions,
                startIndex: i,
                getRandomRecipe: getRandomRecipe,
                recipeAdd: recipeAdd))
            {
                count++;
            }
        }

        return count;
    }

    private static bool IsSleepRecipeCallStart(
        List<CodeInstruction> instructions,
        int startIndex,
        MethodInfo getRandomRecipe,
        MethodInfo recipeAdd)
    {
        if (startIndex + 4 >= instructions.Count)
        {
            return false;
        }

        return instructions[index: startIndex].opcode == OpCodes.Ldnull &&
            instructions[index: startIndex + 1].opcode == OpCodes.Ldc_I4_0 &&
            instructions[index: startIndex + 2].Calls(method: getRandomRecipe) &&
            instructions[index: startIndex + 3].opcode == OpCodes.Ldc_I4_1 &&
            instructions[index: startIndex + 4].Calls(method: recipeAdd);
    }
}
