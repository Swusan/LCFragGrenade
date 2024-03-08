using GameNetcodeStuff;
using UnityEngine;

namespace LethalFragGrenade;

public class FragGrenade : GrabbableObject
{
   // private void Awake()
// {
//    
// }


// public bool pinPulled;
public bool hasExploded;
public bool DestroyGrenade;
private PlayerControllerB playerThrownBy;
public float explodeTimer;
public float fuse = 2.25f;
private RaycastHit grenadeHit;
private Ray grenadeThrowRay;
public AnimationCurve grenadeFallCurve;
public AnimationCurve grenadeVerticalFallCurveNoBounce;
public AnimationCurve grenadeVerticalFallCurve;


public override void ItemActivate(bool used, bool buttonDown = true)
{
   base.ItemActivate(used, buttonDown);
   Plugin.TheLogger.LogInfo("Grenade Activated");
   playerThrownBy = playerHeldBy;
   if (IsOwner)
   {
       playerHeldBy.DiscardHeldObject(true, null, GetGrenadeThrowDestination());
   }
}


public override void EquipItem()
{
   SetControlTipForGrenade();
   EnableItemMeshes(true);
   isPocketed = false;
}


private void SetControlTipForGrenade()
{
    string[] allLines;
        allLines = new[]
        {
            "Throw grenade: [RMB]"
        };
    if (IsOwner)
    {
        HUDManager.Instance.ChangeControlTipMultiple(allLines, true, itemProperties);
    }
}

public override void Update()
{
   base.Update();
   if (!hasExploded && playerThrownBy is not null)
   {
       explodeTimer += Time.deltaTime;
       if (explodeTimer > fuse)
       {
           ExplodeFragGrenade(DestroyGrenade);
       }
   }
}


public override void FallWithCurve()
{
   float magnitude = (startFallingPosition - targetFloorPosition).magnitude;
   transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(itemProperties.restingRotation.x, transform.eulerAngles.y, itemProperties.restingRotation.z), 14f * Time.deltaTime / magnitude);
   transform.localPosition = Vector3.Lerp(startFallingPosition, targetFloorPosition, grenadeFallCurve.Evaluate(fallTime));
   if (magnitude > 5f)
   {
       transform.localPosition = Vector3.Lerp(new Vector3(transform.localPosition.x, startFallingPosition.y, transform.localPosition.z), new Vector3(transform.localPosition.x, targetFloorPosition.y, transform.localPosition.z), grenadeVerticalFallCurveNoBounce.Evaluate(fallTime));
   }
   else
   {
       transform.localPosition = Vector3.Lerp(new Vector3(transform.localPosition.x, startFallingPosition.y, transform.localPosition.z), new Vector3(transform.localPosition.x, targetFloorPosition.y, transform.localPosition.z), grenadeVerticalFallCurve.Evaluate(fallTime));
   }
   fallTime += Mathf.Abs(Time.deltaTime * 12f / magnitude);
}


private void ExplodeFragGrenade(bool destroy = false)
{
   if (hasExploded)
   {
       return;
   }
   hasExploded = true;
   Landmine.SpawnExplosion(transform.position, true, 100f, 110f);
   DestroyGrenade = true;
   if (DestroyGrenade)
   {
       DestroyObjectInHand(playerThrownBy);
   }
}


public Vector3 GetGrenadeThrowDestination()
{
   Vector3 vector = transform.position;
   Debug.DrawRay(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward, Color.yellow, 15f);
   grenadeThrowRay = new Ray(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward);
   if (Physics.Raycast(grenadeThrowRay, out grenadeHit, 12f, StartOfRound.Instance.collidersAndRoomMaskAndDefault))
   {
       vector = grenadeThrowRay.GetPoint(grenadeHit.distance - 0.05f);
   }
   else
   {
       vector = grenadeThrowRay.GetPoint(10f);
   }
   Debug.DrawRay(vector, Vector3.down, Color.blue, 15f);
   grenadeThrowRay = new Ray(vector, Vector3.down);
   Vector3 result;
   if (Physics.Raycast(grenadeThrowRay, out grenadeHit, 30f, StartOfRound.Instance.collidersAndRoomMaskAndDefault))
   {
       result = grenadeHit.point + Vector3.up * 0.05f;
   }
   else
   {
       result = grenadeThrowRay.GetPoint(30f);
   }
   return result;
}

}