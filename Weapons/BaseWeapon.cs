using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class BaseWeapon : MonoBehaviour
{
    //Variables
    #region
    //Checkers
    protected bool autoFire = false;
    protected bool allowShoot = true;
    private bool isReloading = false;

    //Basic
    protected int bulletCount;
    protected int magSize;
    protected float reloadTime;
    protected float fireRate;
    protected int damage;

    //Recoil
    //protected Vector3 currentRot;
    //protected Vector3 targetRot;
    //protected float multrecoilX, multrecoilZ = 1.0f;
    //protected float recoilY = 2;
    //protected float snappiness = 6;
    //protected float returnSpeed = 2;
    #endregion

    public int BulletCount { get => bulletCount; set => bulletCount = value; }
    public int MagSize { get => magSize; }
    public bool AutoFire { get => autoFire; }
    public bool AllowShoot { get => allowShoot; }

    public virtual void Shoot(Camera mainCam)
    {
        if (allowShoot && bulletCount > 0) 
        {
            if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit hit, 100))
            {
                if (hit.transform.CompareTag("Targetable"))
                {
                    hit.transform.GetComponent<ITarget>().OnDamaged(damage);
                }
            }
            bulletCount--;
            allowShoot = false;
            if (bulletCount == 0)
            {
                Reloading();
            }
            else
            {
                Invoke(nameof(ReadyToShoot), fireRate);
            }
        }
    }
    public void ReadyToShoot()
    {
        allowShoot = true;
    }

    public void Reloading()
    {
        if (isReloading)
        {
            Debug.Log("Already reloading...");
            return;
        }
        else
        {
            isReloading = true;
            bulletCount = 0;
            Debug.Log("Reloading");
            Invoke(nameof(Reloaded), reloadTime);
        }
    }
    public void Reloaded()
    {
        bulletCount = magSize;
        isReloading = false;
        allowShoot = true;
        Debug.Log("Reloaded");
    }

    //public void RecoilUpdate(Transform camRot)
    //{
    //    targetRot = Vector3.Lerp(targetRot, Vector3.zero, returnSpeed * Time.deltaTime);
    //    currentRot = Vector3.Slerp(currentRot, targetRot, snappiness * Time.deltaTime);
    //    camRot.transform.localRotation = Quaternion.Euler(currentRot);
    //}
    //public void RecoilFire(Camera mainCam)
    //{
    //    float camRotY = (mainCam.transform.localEulerAngles.y % 360 + 360) % 360;
    //    float rotX, rotZ;

    //    if (camRotY >= 0 && camRotY < 90)
    //    {
    //        rotX = Mathf.Lerp(-2f, 0f, camRotY / 90f);
    //        rotZ = Mathf.Lerp(0f, 2f, camRotY / 90f);
    //    }
    //    else if (camRotY >= 90 && camRotY < 180)
    //    {

    //        rotX = Mathf.Lerp(0f, 2f, (camRotY - 90) / 90f);
    //        rotZ = Mathf.Lerp(2f, 0f, (camRotY - 90) / 90f);
    //    }
    //    else if (camRotY >= 180 && camRotY < 270)
    //    {
    //        rotX = Mathf.Lerp(2f, 0f, (camRotY - 180) / 90f);
    //        rotZ = Mathf.Lerp(0f, -2f, (camRotY - 180) / 90f);
    //    }
    //    else
    //    {
    //        rotX = Mathf.Lerp(0f, -2f, (camRotY - 270) / 90f);
    //        rotZ = Mathf.Lerp(-2f, 0f, (camRotY - 270) / 90f);
    //    }
    //    targetRot += new Vector3(multrecoilX * rotX, Random.Range(-recoilY, recoilY), multrecoilZ * rotZ);
    //}
}
