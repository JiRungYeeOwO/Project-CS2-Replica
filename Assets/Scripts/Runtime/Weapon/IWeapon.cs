using UnityEngine;

public interface IWeapon
{
    void Attack(Camera playerCam);

    void Reload();
}
