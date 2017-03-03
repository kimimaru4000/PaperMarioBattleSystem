using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The DEFDown Status Effect.
    /// The entity's Defense is reduced by a certain value until it ends.
    /// </summary>
    public class DEFDownStatus : StatusEffect
    {
        /// <summary>
        /// The amount to reduce the entity's Defense by.
        /// </summary>
        protected int DefenseValue = 0;

        public DEFDownStatus(int defenseValue, int duration)
        {
            StatusType = Enumerations.StatusTypes.DEFDown;
            Alignment = StatusAlignments.Negative;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(555, 156, 38, 46));

            DefenseValue = defenseValue;
            Duration = duration;

            AfflictedMessage = "Defense has dropped!";
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.LowerDefense(DefenseValue);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.RaiseDefense(DefenseValue);
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
        }

        protected override void OnSuspend()
        {
            EntityAfflicted.RaiseDefense(DefenseValue);
        }

        protected override void OnResume()
        {
            EntityAfflicted.LowerDefense(DefenseValue);
        }

        public override StatusEffect Copy()
        {
            return new DEFDownStatus(DefenseValue, Duration);
        }
    }
}
