using UnityEngine;

public class LaserBuyUI : SelectableItemForController {

    public Ammo ammo;
    public int amountToBuy = 100; //Solo 3 numeros => 100 / 1.000 / 10.000
    public int monedaBarataPerLaser = 100;
    public int monedaCaraPerLaser = 0;

    public override void Use() {
        base.Use();
        Ship ship = (PlayerController.instance) as Ship;

        if (ship.monedaBarata >= monedaBarataPerLaser * amountToBuy) {
            ship.monedaBarata -= monedaBarataPerLaser * amountToBuy;
            ship.AddLaserAmmo(ammo.tier, amountToBuy);

        } else {
            //TODO: lo que sea que me digan si no tengo dinero
        }
    }
}