using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    public class QuakeHammerAction : HammerAction
    {
        private int BaseDamage = 1;
        private int DamageScalePerBadge = 1;

        public QuakeHammerAction(int numBadges)
        {
            Name = "Quake Hammer";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(907, 102, 23, 24)),
                "Slightly damage all ground\nenemies.", MoveResourceTypes.FP, 3 /*2 FP in PM*/, CostDisplayTypes.Shown,
                MoveAffectionTypes.Other, TargetSelectionMenu.EntitySelectionType.All, true,
                new HeightStates[] { HeightStates.Grounded, HeightStates.Ceiling }, User.GetOpposingEntityType());

            //Scale by the number of badges the move has
            int damage = BaseDamage + (DamageScalePerBadge * (numBadges - 1));

            DamageInfo = new DamageData(damage, Elements.Normal, true, ContactTypes.None, ContactProperties.Ranged, null, DamageEffects.FlipsShelled | DamageEffects.FlipsClefts);

            SetMoveSequence(new QuakeHammerSequence(this));
            actionCommand = new HammerCommand(MoveSequence, 4, 500d);
        }
    }
}
