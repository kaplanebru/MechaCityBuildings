using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public static class Eventbus
{
    public static Action OnRandomizingExecuted;
    public static Action<Dictionary<ReplacementType, List<PlaceholderData>>> OnReplacementWithSavedRequest;

    public static Action OnReloadCall;

}
