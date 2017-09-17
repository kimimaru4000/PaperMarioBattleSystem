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
    /// The Fast Status Effect.
    /// Entities afflicted with this can move an additional turn each phase cycle it's active
    /// </summary>
    public sealed class FastStatus : StatusEffect
    {
        private const int AdditionalTurns = 1;

        public FastStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Fast;
            Alignment = StatusAlignments.Positive;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(555, 205, 38, 46));

            Duration = duration;

            AfflictedMessage = "More chances to attack\nare now available!";
        }

        protected override void OnAfflict()
        {
            //Set this on affliction as well, as the entity could have not used its turn yet if it's in the same phase
            //First check if the max turn count is greater than 0, as a turn count of 0 indicates the entity was not able to move this turn
            if (EntityAfflicted.MaxTurns > 0)
            {
                EntityAfflicted.SetMaxTurns(EntityAfflicted.BaseTurns + AdditionalTurns);
                Debug.Log($"{StatusType} set MaxTurns to {EntityAfflicted.BaseTurns + AdditionalTurns} for {EntityAfflicted.Name}");
            }
        }

        protected override void OnEnd()
        {
            EntityAfflicted.SetMaxTurns(EntityAfflicted.BaseTurns);
            Debug.Log($"{StatusType} set MaxTurns to {EntityAfflicted.BaseTurns} for {EntityAfflicted.Name}");
        }

        protected override void OnPhaseCycleStart()
        {
            EntityAfflicted.SetMaxTurns(EntityAfflicted.BaseTurns + AdditionalTurns);
            Debug.Log($"{StatusType} set MaxTurns to {EntityAfflicted.BaseTurns + AdditionalTurns} for {EntityAfflicted.Name}");
            ProgressTurnCount();
        }

        protected override void OnSuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.SetMaxTurns(EntityAfflicted.BaseTurns);
            }
        }

        protected override void OnUnsuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.SetMaxTurns(EntityAfflicted.BaseTurns + AdditionalTurns);
            }
        }

        public override StatusEffect Copy()
        {
            return new FastStatus(Duration);
        }
    }
}
