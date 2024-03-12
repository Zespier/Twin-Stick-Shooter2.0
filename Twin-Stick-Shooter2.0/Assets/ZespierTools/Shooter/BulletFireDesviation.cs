using UnityEngine;

public class BulletFireDesviation {
    public static Vector3 RandomBulletFireDesviation(Transform shootPoint, float desviationAngle) {

        Vector3 shootDesviation = shootPoint.forward;
        float randomDesviationY = Random.Range(-desviationAngle, desviationAngle);
        float randomDesviationX = Random.Range(-desviationAngle, desviationAngle);

        shootDesviation = Quaternion.AngleAxis(randomDesviationX, shootPoint.right) * shootDesviation;
        shootDesviation = Quaternion.AngleAxis(randomDesviationY, shootPoint.up) * shootDesviation;

        return shootDesviation;
    }

    public static Vector3 RandomBulletFireDesviation2D(Transform shootPoint, float desviationAngle, ShootDirectionReference shootDirectionReference) {

        Vector3 shootDesviation;
        switch (shootDirectionReference) {

            case ShootDirectionReference.up:
                shootDesviation = shootPoint.up;
                break;
            case ShootDirectionReference.right:
                shootDesviation = shootPoint.right;
                break;
            default:
                shootDesviation = shootPoint.up;
                break;
        }


        float randomDesviationZ = Random.Range(-desviationAngle, desviationAngle);

        shootDesviation = Quaternion.AngleAxis(randomDesviationZ, shootPoint.forward) * shootDesviation;

        return shootDesviation;
    }
}

public enum ShootDirectionReference {
    up,
    right
}
