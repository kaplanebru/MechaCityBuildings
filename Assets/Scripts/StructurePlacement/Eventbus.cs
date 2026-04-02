using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public static class Eventbus
{
    public static Action<Dictionary<StructureType, List<CellData>>> OnReplacementWithSavedRequest;

    public static Action OnReloadCall;

}
