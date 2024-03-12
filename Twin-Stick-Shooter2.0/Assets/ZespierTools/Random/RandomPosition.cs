using UnityEngine;

public class RandomPosition {

    public static Vector2 Square_2D(float radius) {

        return new Vector2(Random.Range(-radius, radius), Random.Range(-radius, radius));
    }

    public static Vector2 Circle_2D(float radius, bool clampedToCircunference) {

        Vector2 newPosition = new Vector2(Random.Range(-radius, radius), Random.Range(-radius, radius));


        if (clampedToCircunference) {
            return newPosition.normalized * radius;

        } else {

            if (newPosition.sqrMagnitude > radius * radius) {
                return newPosition.normalized * radius;
            }

            return newPosition;
        }
    }
}