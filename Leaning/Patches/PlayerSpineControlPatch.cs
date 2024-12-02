using HarmonyLib;
using UnityEngine;

namespace AetharNet.Mods.ZumbiBlocks2.Leaning.Patches;

[HarmonyPatch(typeof(PlayerSpineControl))]
public class PlayerSpineControlPatch
{
    private const float LeanAngle = 40f;
    private const float LeanSpeed = 3f;
    private const float RayCastLength = 0.7f;
    private static readonly int RayCastMask = LayerMask.GetMask("Default", "Geometry");

    private static float currentLeanAngle;
    private static PlayerSpineControl localSpineControl;
    private static PlayerArms localArms;
    private static PlayerInteraction localInteraction;

    [HarmonyPostfix]
    [HarmonyPatch("AngleSpineFirstPerson")]
    public static void HandleFirstPersonLeaning(PlayerSpineControl __instance)
    {
        if (localSpineControl == null)
        {
            if (PlayersController.instance.MyPlayer().SpineControl == __instance)
            {
                localSpineControl = __instance;
                localArms = PlayersController.instance.MyPlayer().arms;
                localInteraction = PlayersController.instance.MyPlayer().interaction;
            }
        }

        if (localSpineControl != __instance) return;


        var targetAngle = 0f;

        if (localArms.ads)
        {
            if (Input.GetKey(Leaning.KeyLeanLeft) &&
                !IsDirectionObstructed(__instance.playerTransform, RayCastDirection.Left))
            {
                targetAngle += LeanAngle;
            }

            if (Input.GetKey(Leaning.KeyLeanRight) &&
                !IsDirectionObstructed(__instance.playerTransform, RayCastDirection.Right))
            {
                targetAngle -= LeanAngle;
            }
        }

        currentLeanAngle = Mathf.Lerp(currentLeanAngle, targetAngle, Time.deltaTime * LeanSpeed);

        foreach (var boneTransform in __instance.shoulderBones)
        {
            boneTransform.Rotate(__instance.playerTransform.forward, currentLeanAngle, Space.World);
            boneTransform.Translate(__instance.playerTransform.right * (-currentLeanAngle / 100f), Space.World);
        }
    }

    private enum RayCastDirection
    {
        Left = -1,
        Right = 1
    }

    private static bool IsDirectionObstructed(Transform playerTransform, RayCastDirection direction)
    {
        var isObstructed = Physics.Raycast(playerTransform.position, playerTransform.right * (int)direction,
            out var hit, RayCastLength, RayCastMask);

        if (isObstructed)
        {
            var interactableFurniture = hit.collider.gameObject.GetComponentInParent<InteractableFurniture>();

            switch (interactableFurniture?.targetFurniture.id)
            {
                case Furniture.ID.WoodenWindow:
                case Furniture.ID.PoliceWindow:
                    interactableFurniture.TakeDamage(new Damage(interactableFurniture.health));
                    return false;
                case Furniture.ID.WoodenDoor:
                case Furniture.ID.CellDoor:
                    if (interactableFurniture.state == interactableFurniture.baseState)
                    {
                        localInteraction.Interact(interactableFurniture);
                    }

                    return false;
                default:
                    return true;
            }
        }

        return false;
    }
}
