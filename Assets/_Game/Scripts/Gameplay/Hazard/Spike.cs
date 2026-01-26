

using System.Diagnostics;

public class Spike : Hazard, IHazardEffect
{
    public void Apply(IAffectable affectable)
    {
        if (affectable is Player)
        {
            UnityEngine.Debug.Log("vcl");
        }
    }
}