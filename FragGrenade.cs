using System;
using System.Runtime.CompilerServices;
using Dissonance;
using DunGen;
using GameNetcodeStuff;
using Mono.Security.X509;
using UnityEngine;
using Object = UnityEngine.Object;

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
        playerThrownBy = playerHeldBy;
        if (base.IsOwner)
        {
            this.playerHeldBy.DiscardHeldObject(true, null, this.GetGrenadeThrowDestination(), true);
        }
    }
    
    public override void EquipItem()
    {
        //this.SetControlTipForGrenade();
        base.EnableItemMeshes(true);
        this.isPocketed = false;
    }
    
    // private void SetControlTipForGrenade()
    // {
    //     string[] allLines;
    //     if (this.pinPulled)
    //     {
    //         allLines = new string[]
    //         {
    //             "Throw grenade: [RMB]"
    //         };
    //     }
    //     else
    //     {
    //         allLines = new string[]
    //         {
    //             "Pull pin: [RMB]"
    //         };
    //     }
    //     if (base.IsOwner)
    //     {
    //         HUDManager.Instance.ChangeControlTipMultiple(allLines, true, this.itemProperties);
    //     }
    // }

    public override void Update()
    {
        base.Update();
        if (!this.hasExploded && playerThrownBy is not null)
        {
            this.explodeTimer += Time.deltaTime;
            if (this.explodeTimer > this.fuse)
            {
                this.ExplodeFragGrenade(this.DestroyGrenade);
            }
        }
    }
    
    public override void FallWithCurve()
    {
        float magnitude = (this.startFallingPosition - this.targetFloorPosition).magnitude;
        base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(this.itemProperties.restingRotation.x, base.transform.eulerAngles.y, this.itemProperties.restingRotation.z), 14f * Time.deltaTime / magnitude);
        base.transform.localPosition = Vector3.Lerp(this.startFallingPosition, this.targetFloorPosition, this.grenadeFallCurve.Evaluate(this.fallTime));
        if (magnitude > 5f)
        {
            base.transform.localPosition = Vector3.Lerp(new Vector3(base.transform.localPosition.x, this.startFallingPosition.y, base.transform.localPosition.z), new Vector3(base.transform.localPosition.x, this.targetFloorPosition.y, base.transform.localPosition.z), this.grenadeVerticalFallCurveNoBounce.Evaluate(this.fallTime));
        }
        else
        {
            base.transform.localPosition = Vector3.Lerp(new Vector3(base.transform.localPosition.x, this.startFallingPosition.y, base.transform.localPosition.z), new Vector3(base.transform.localPosition.x, this.targetFloorPosition.y, base.transform.localPosition.z), this.grenadeVerticalFallCurve.Evaluate(this.fallTime));
        }
        this.fallTime += Mathf.Abs(Time.deltaTime * 12f / magnitude);
    }
    
    private void ExplodeFragGrenade(bool destroy = false)
    {
        if (this.hasExploded)
        {
            return;
        }
        this.hasExploded = true;
        // this.itemAudio.PlayOneShot(this.explodeSFX);
        // WalkieTalkie.TransmitOneShotAudio(this.itemAudio, this.explodeSFX, 1f);
        Landmine.SpawnExplosion(base.transform.position, true);
        DestroyGrenade = true;
        if (this.DestroyGrenade)
        {
            this.DestroyObjectInHand(this.playerThrownBy);
        }
    }
    
    public Vector3 GetGrenadeThrowDestination()
    {
        if (playerHeldBy is null)
        {
            Plugin.TheLogger.LogInfo("Finn's Fault");
        }
        else
        {
            Plugin.TheLogger.LogInfo("Gavin's Fault");
        }
        Plugin.TheLogger.LogInfo("test0");
        Vector3 vector = base.transform.position;
        Debug.DrawRay(playerHeldBy.gameplayCamera.transform.position, this.playerHeldBy.gameplayCamera.transform.forward, Color.yellow, 15f);
        Plugin.TheLogger.LogInfo("test1");
        this.grenadeThrowRay = new Ray(this.playerHeldBy.gameplayCamera.transform.position, this.playerHeldBy.gameplayCamera.transform.forward);
        Plugin.TheLogger.LogInfo("test2");
        if (Physics.Raycast(this.grenadeThrowRay, out this.grenadeHit, 12f, StartOfRound.Instance.collidersAndRoomMaskAndDefault))
        {
            vector = this.grenadeThrowRay.GetPoint(this.grenadeHit.distance - 0.05f);
        }
        else
        {
            vector = this.grenadeThrowRay.GetPoint(10f);
        }
        Plugin.TheLogger.LogInfo("test3");
        Debug.DrawRay(vector, Vector3.down, Color.blue, 15f);
        Plugin.TheLogger.LogInfo("test4");
        this.grenadeThrowRay = new Ray(vector, Vector3.down);
        Plugin.TheLogger.LogInfo("test5");
        Vector3 result;
        Plugin.TheLogger.LogInfo("test6");
        if (Physics.Raycast(this.grenadeThrowRay, out this.grenadeHit, 30f, StartOfRound.Instance.collidersAndRoomMaskAndDefault))
        {
            result = this.grenadeHit.point + Vector3.up * 0.05f;
        }
        else
        {
            result = this.grenadeThrowRay.GetPoint(30f);
        }
        Plugin.TheLogger.LogInfo("test7");
        return result;
    }
    
}