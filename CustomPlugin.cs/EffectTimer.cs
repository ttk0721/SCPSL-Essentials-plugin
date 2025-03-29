using PluginAPI.Core;
using System;
using UnityEngine;

public class EffectTimer : MonoBehaviour
{
    private Action onComplete;

    public void Initialize(Player player, Action completeAction, float duration)
    {
        onComplete = completeAction;
        Destroy(this, duration);
    }

    private void OnDestroy()
    {
        onComplete?.Invoke();
    }
}
