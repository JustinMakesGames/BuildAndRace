using UnityEngine;

public class PropPlacement
{
    public PropScriptableObject prop;
    public Vector3 position;
    public Vector3 rotation;

    public PropPlacement(PropScriptableObject prop, Vector3 position, Vector3 rotation)
    {
        this.prop = prop;
        this.position = position;
        this.rotation = rotation;
    }
}
