using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PaperMarioBattleSystem.Extensions;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Pokey's Body Throw move; throw a body part at the target to damage it.
    /// The body part can be Superguarded and returned back at the Pokey for full damage.
    /// </summary>
    public class BodyThrowAction : MoveAction
    {
        public BodyThrowAction(BattleEntity user, ISegmentBehavior segmentBehavior, CroppedTexture2D segmentTex) : base(user)
        {
            Name = "Body Throw";

            MoveInfo = new MoveActionData(null, "Throw a body part at the target.", MoveResourceTypes.FP, 0f, CostDisplayTypes.Shown,
                MoveAffectionTypes.Other, EntitySelectionType.Single, true, null, User.GetOpposingEntityType());

            DamageInfo = new DamageData(0, Elements.Normal, false, ContactTypes.None, ContactProperties.Ranged, null, DamageEffects.None);

            SetMoveSequence(new BodyThrowSequence(this, segmentBehavior, segmentTex));
            actionCommand = null;
        }
    }
}
