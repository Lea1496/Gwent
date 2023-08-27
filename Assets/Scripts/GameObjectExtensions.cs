using UnityEngine;

public static class GameObjectExtensions
{
    public static float Width(this GameObject gameObject)
    {
        return gameObject.GetComponent<RectTransform>().rect.width;
    }
    public static float X(this GameObject gameObject)
    {
        return gameObject.transform.position.x;
    }
    public static float Y(this GameObject gameObject)
    {
        return gameObject.transform.position.y;
    }
}