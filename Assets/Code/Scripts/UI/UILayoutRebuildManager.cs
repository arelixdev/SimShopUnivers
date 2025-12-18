using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILayoutRebuildManager : MonoBehaviour
{
    public static UILayoutRebuildManager instance;

    private HashSet<RectTransform> queued = new();
    private bool coroutineRunning;

    private void Awake()
    {
        instance = this;
    }

    public void RequestRebuild(RectTransform target)
    {
        if (target == null) return;

        queued.Add(target);

        if (!coroutineRunning)
            StartCoroutine(RebuildEndOfFrame());
    }

    private IEnumerator RebuildEndOfFrame()
    {
        coroutineRunning = true;

        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();

        foreach (var rt in queued)
        {
            if (rt != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }

        queued.Clear();
        coroutineRunning = false;
    }

}