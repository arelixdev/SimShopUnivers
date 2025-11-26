using UnityEngine;

public abstract class RadialAction : ScriptableObject
{
    public Sprite icon;

    public abstract void Execute(RadialActionContext context);
}
