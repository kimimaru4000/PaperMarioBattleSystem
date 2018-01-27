using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Art Attack action. Use the Ruby Star's trail to draw around enemies and deal damage to them.
    /// <para>I'm not sure on the damage specifics, but it seems like every 33% of the hurtbox you cover increases damage by 1,
    /// starting at 1. Fully circling an enemy's hurtbox deals 3 damage.</para>
    /// </summary>
    public sealed class ArtAttack : SpecialMoveAction
    {
        private readonly double StartingDrawTime = 20000d;
        private readonly Vector2 StartingDrawLoc = Vector2.Zero;

        public ArtAttack()
        {
            Name = "Art Attack";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(202, 987, 39, 36)),
                "Draw around the enemy as many times as you can!", Enumerations.MoveResourceTypes.SSSP,
                400, Enumerations.CostDisplayTypes.Shown, Enumerations.MoveAffectionTypes.Other, TargetSelectionMenu.EntitySelectionType.All,
                false, null, Enumerations.EntityTypes.Enemy);
            DamageInfo = new DamageData(1, Enumerations.Elements.Star, true, Enumerations.ContactTypes.None, Enumerations.ContactProperties.Ranged, null, true, false,
                Enumerations.DefensiveActionTypes.None, Enumerations.DamageEffects.None);

            StartingDrawLoc = SpriteRenderer.Instance.WindowCenter;

            SetMoveSequence(new ArtAttackSequence(this));
            actionCommand = new ArtAttackCommand(MoveSequence, StartingDrawLoc, StartingDrawTime);
        }
    }
}
