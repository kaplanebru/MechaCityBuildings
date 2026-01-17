using System;
using JetBrains.Annotations;
using UnityEngine;

public static class Eventbus
{
    public static Action OnRandomizerApplyButtonClickedForNewPool;
    public static Action OnRandomizerApplyButtonClickedForSamePool;

    public static Action<ReplacementType, PlaceholderData[]> OnReplacementRequest;
}
