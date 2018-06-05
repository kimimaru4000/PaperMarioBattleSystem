using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Lullaby action. Mamar puts all enemies to sleep at the cost of 1 SP.
    /// </summary>
    public sealed class LullabyAction : SpecialMoveAction
    {
        public LullabyAction(BattleEntity user) : base(user)
        {
            Name = "Lullaby";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(66, 961, 24, 24)),
                "Lull enemies to sleep with\na tender lullaby.", Enumerations.MoveResourceTypes.SSSP,
                100, Enumerations.CostDisplayTypes.Shown, Enumerations.MoveAffectionTypes.Other,
                Enumerations.EntitySelectionType.All, false, null, Enumerations.EntityTypes.Enemy);
                
            DamageInfo = new DamageData(0, Enumerations.Elements.Star, true, Enumerations.ContactTypes.None, Enumerations.ContactProperties.Ranged,
                new StatusChanceHolder[] { new StatusChanceHolder(100d, new SleepStatus(3)) }, Enumerations.DamageEffects.None);

            SetMoveSequence(new LullabySequence(this));
        }
    }
}
