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
    /// The Slow Status Effect.
    /// Entities afflicted with this can move only once every two phase cycles.
    /// <para>Mario and his Partner can still Guard and Superguard when afflicted with this Status Effect</para>
    /// </summary>
    public sealed class SlowStatus : StatusEffect
    {
        private bool PreventMovement = true;

        public SlowStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Slow;
            Alignment = StatusAlignments.Negative;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(604, 205, 38, 46));

            Duration = duration;

            AfflictedMessage = "Less chances to attack\nare now available!";
        }

        protected override void OnAfflict()
        {
            //On affliction end the entity's turn
            EntityAfflicted.SetMaxTurns(0);
            EntityAfflicted.SetTurnsUsed(EntityAfflicted.MaxTurns);
            Debug.Log($"{StatusType} set MaxTurns to 0 for {EntityAfflicted.Name}");
        }

        protected override void OnEnd()
        {
            if (EntityAfflicted.MaxTurns < EntityAfflicted.BaseTurns)
            {
                EntityAfflicted.SetMaxTurns(EntityAfflicted.BaseTurns);
                Debug.Log($"{StatusType} set MaxTurns to {EntityAfflicted.BaseTurns} for {EntityAfflicted.Name}");
            }
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();

            if (IsFinished == false)
            {
                //If the entity shouldn't move this turn, set its max turns to 0
                if (PreventMovement == true)
                {
                    EntityAfflicted.SetMaxTurns(0);
                    Debug.Log($"{StatusType} set MaxTurns to 0 for {EntityAfflicted.Name}");
                }

                //Flip the flag telling whether the entity can move next turn or not
                PreventMovement = !PreventMovement;
            }
        }

        protected override void OnSuspend()
        {
            EntityAfflicted.SetMaxTurns(EntityAfflicted.BaseTurns);
        }

        protected override void OnResume()
        {
            if (PreventMovement == true)
            {
                EntityAfflicted.SetMaxTurns(0);
                EntityAfflicted.SetTurnsUsed(EntityAfflicted.MaxTurns);
            }
        }

        public override StatusEffect Copy()
        {
            return new SlowStatus(Duration);
        }
    }
}
