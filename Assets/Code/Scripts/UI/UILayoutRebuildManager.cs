using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILayoutRebuildManager : MonoBehaviour
{
    public static UILayoutRebuildManager instance;

    private HashSet<RectTransform> queued = new();
    private bool running;

    private void Awake()
    {
        instance = this;
    }

    public void RequestRebuild(RectTransform target)
    {
        if (target == null) return;

        queued.Add(target);

        if (!running)
            StartCoroutine(RebuildRoutine());
    }

    private IEnumerator RebuildRoutine()
    {
        running = true;

        // FRAME 1 : on marque
        yield return null;

        foreach (var rt in queued)
        {
            if (rt != null)
                LayoutRebuilder.MarkLayoutForRebuild(rt);
        }

        // FRAME 2 : Unity calcule vraiment
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();

        queued.Clear();
        running = false;
    }

}