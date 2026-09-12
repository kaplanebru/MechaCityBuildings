using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public static class Eventbus
{
    public static Action<List<List<SlotData>>> OnDispositionActivationRequest;
    public static Action OnReloadCall;
}