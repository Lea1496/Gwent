using System;
using System.Linq;
using UnityEngine;

public static class GameObjectsDisposition
{
    public static void DistributeCenter(GameObject container, GameObject[] objects, float spaceBetween)
    {
        DistributeEvenly(container, objects, spaceBetween, spaceLeft => spaceLeft / 2);
    }

    public static void DistributeLeft(GameObject container, GameObject[] objects, float spaceBetween)
    {
        DistributeEvenly(container, objects, spaceBetween, spaceLeft => 0);
    }

    public static void DistributeRight(GameObject container, GameObject[] objects, float spaceBetween)
    {
        DistributeEvenly(container, objects, spaceBetween, spaceLeft => spaceLeft);
    }

    public static void DistributeEvenly(GameObject container, GameObject[] objects, float spaceBetween, Func<float, float> getInitialSpacing)
    {
        var totalWidth = container.Width();
        var totalWidthNeeded = objects.Select(o => o.Width()).Sum() + ((objects.Length - 1) * spaceBetween);
        var spaceLeft = Math.Max(totalWidth - totalWidthNeeded, 0);

        var minimalObjectWidth = totalWidth / objects.Length;

        var initialX = container.X() - (totalWidth / 2) + ( objects[0].Width() / 2);
        var x = initialX + getInitialSpacing(spaceLeft);
        var y = container.Y();


        var mustShrink = spaceLeft == 0;

        for (var i = 0; i < objects.Length; i++)
        {
            var gameObject = objects[i];
            if ((int)gameObject.transform.position.x != (int)x || (int)gameObject.transform.position.y != (int)y)
            {
                gameObject.transform.position = new Vector2(x, y);
            }

            if (mustShrink)
            {
                x += minimalObjectWidth;
            }
            else
            {
                x += gameObject.Width() + spaceBetween;
            }
        }
    }
}
