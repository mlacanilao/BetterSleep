using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace BetterSleep.Patches;

internal static class CharaPatch
{
    public static bool CanSleepPrefix(Chara __instance, ref bool __result)
    {
        if (__instance.IsPC == false)
        {
            return true;
        }

        if (EClass._zone.events.GetEvent<ZoneEventQuest>() != null)
        {
            return true;
        }

        bool enableCanSleep = BetterSleepConfig.EnableCanSleep?.Value ?? true;
        bool enableCanSleepDuringMeditate = BetterSleepConfig.EnableCanSleepDuringMeditate?.Value ?? false;
        bool isResting = __instance.ai is AI_Meditate;

        if (enableCanSleep == true &&
            isResting == false)
        {
            __result = true;
            FeatureTestLog.Log(
                feature: "Sleep Anytime",
                detail: "allowed PC sleep outside Rest");
            return false;
        }

        if (enableCanSleepDuringMeditate == true &&
            isResting == true)
        {
            __result = true;
            FeatureTestLog.Log(
                feature: "Sleep During Rest",
                detail: "allowed PC sleep while using Rest");
            return false;
        }

        if (enableCanSleepDuringMeditate == false &&
            isResting == true)
        {
            __result = false;
            FeatureTestLog.Log(
                feature: "Sleep During Rest",
                detail: "blocked PC sleep while using Rest");
            return false;
        }

        return true;
    }

    public static IEnumerable<CodeInstruction> OnSleepTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> instructionList = new List<CodeInstruction>(collection: instructions);
        MethodInfo? adjustSleepRecoveryPower = AccessTools.Method(
            type: typeof(CharaPatch),
            name: nameof(AdjustSleepRecoveryPower),
            parameters: new[] { typeof(int), typeof(Chara) });

        if (adjustSleepRecoveryPower == null)
        {
            BetterSleep.LogError(message: "Chara.OnSleep transpiler: recovery multiplier method not found");
            return instructionList;
        }

        int matchCount = CountRecoveryPowerAssignments(instructions: instructionList);
        if (matchCount != 1)
        {
            BetterSleep.LogError(
                message: "Chara.OnSleep transpiler expected 1 recovery power assignment but found " +
                    matchCount.ToString() +
                    ". Sleep recovery multiplier will remain vanilla.");
            return instructionList;
        }

        CodeMatcher codeMatcher = new CodeMatcher(instructions: instructionList);
        codeMatcher.MatchStartForward(matches: new[]
        {
            new CodeMatch(opcode: OpCodes.Ldarg_1),
            new CodeMatch(opcode: OpCodes.Ldarg_2),
            new CodeMatch(opcode: OpCodes.Mul),
            new CodeMatch(opcode: OpCodes.Stloc_0)
        });

        if (codeMatcher.IsValid == false)
        {
            BetterSleep.LogError(
                message: "Chara.OnSleep transpiler failed to replace the matched recovery power assignment. " +
                    "Sleep recovery multiplier will remain vanilla.");
            return codeMatcher.Instructions();
        }

        codeMatcher.Advance(offset: 3);
        codeMatcher.Insert(instructions: new[]
        {
            new CodeInstruction(opcode: OpCodes.Ldarg_0),
            new CodeInstruction(opcode: OpCodes.Call, operand: adjustSleepRecoveryPower)
        });

        BetterSleep.LogDebug(message: "Chara.OnSleep transpiler: matched recovery power assignment");
        return codeMatcher.Instructions();
    }

    public static void SleepPostfix(Chara __instance)
    {
        bool enableSleepDelay = BetterSleepConfig.EnableSleepDelay?.Value ?? false;
        int customSleepDelayTurns = BetterSleepConfig.GetEffectiveSleepDelayTurns();

        ConSleep? consleep = __instance.conSleep;
        if (enableSleepDelay == true &&
            consleep != null)
        {
            consleep.pcSleep = customSleepDelayTurns;
            FeatureTestLog.Log(
                feature: "Sleep Delay",
                detail: "set ConSleep.pcSleep to " + customSleepDelayTurns.ToString());
        }
    }

    private static int AdjustSleepRecoveryPower(int recoveryPower, Chara chara)
    {
        if (chara.IsPCParty == false)
        {
            return recoveryPower;
        }

        bool enableSleepPowerMultiplier = BetterSleepConfig.EnableSleepPowerMultiplier?.Value ?? false;
        if (enableSleepPowerMultiplier == false)
        {
            return recoveryPower;
        }

        int multiplier = BetterSleepConfig.GetEffectiveSleepPowerMultiplier();
        int adjustedRecoveryPower = recoveryPower * multiplier;
        FeatureTestLog.Log(
            feature: "Sleep Recovery Multiplier",
            detail: "originalPower=" + recoveryPower.ToString() +
                ", multiplier=" + multiplier.ToString() +
                ", adjustedPower=" + adjustedRecoveryPower.ToString());

        return adjustedRecoveryPower;
    }

    private static int CountRecoveryPowerAssignments(List<CodeInstruction> instructions)
    {
        int count = 0;

        for (int i = 0; i < instructions.Count; i++)
        {
            if (IsRecoveryPowerAssignmentStart(instructions: instructions, startIndex: i))
            {
                count++;
            }
        }

        return count;
    }

    private static bool IsRecoveryPowerAssignmentStart(List<CodeInstruction> instructions, int startIndex)
    {
        if (startIndex + 3 >= instructions.Count)
        {
            return false;
        }

        return instructions[index: startIndex].opcode == OpCodes.Ldarg_1 &&
            instructions[index: startIndex + 1].opcode == OpCodes.Ldarg_2 &&
            instructions[index: startIndex + 2].opcode == OpCodes.Mul &&
            instructions[index: startIndex + 3].opcode == OpCodes.Stloc_0;
    }
}
