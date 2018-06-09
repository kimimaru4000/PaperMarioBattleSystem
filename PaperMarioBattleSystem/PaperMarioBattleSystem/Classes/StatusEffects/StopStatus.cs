using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaperMarioBattleSystem.Extensions;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Stop Status Effect.
    /// Entities afflicted with this cannot move until it wears off.
    /// <para>Mario and his Partner cannot Guard or Superguard when afflicted with this Status Effect.</para>
    /// </summary>
    public class StopStatus : MessageEventStatus
    {
        public StopStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Stop;
            Alignment = StatusAlignments.Negative;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(604, 9, 38, 46));

            Duration = duration;

            AfflictedMessage = "Immobilized! Movement will\nbe impossible for a while!";
        }
        
        protected override void OnAfflict()
        {
            //Prevent the entity from moving on affliction and mark it as using up all of its turns
            EntityAfflicted.SetMaxTurns(0);
            EntityAfflicted.SetTurnsUsed(EntityAfflicted.MaxTurns);

            //Specify that this status makes the entity Immobile
            EntityAfflicted.AddIntAdditionalProperty(Enumerations.AdditionalProperty.Immobile, 1);

            Debug.Log($"{StatusType} set MaxTurns to 0 for {EntityAfflicted.Name}");

            base.OnAfflict();
        }

        protected override void OnEnd()
        {
            if (EntityAfflicted.MaxTurns > 0 && EntityAfflicted.MaxTurns < EntityAfflicted.BaseTurns)
            {
                EntityAfflicted.SetMaxTurns(EntityAfflicted.BaseTurns);
                Debug.Log($"{StatusType} set MaxTurns to {EntityAfflicted.BaseTurns} for {EntityAfflicted.Name}");
            }

            //Remove the Immobile property after getting its count
            EntityAfflicted.SubtractIntAdditionalProperty(Enumerations.AdditionalProperty.Immobile, 1);

            base.OnEnd();
        }

        protected override void OnPhaseCycleStart()
        {
            ProgressTurnCount();
            if (IsSuppressed(Enumerations.StatusSuppressionTypes.Effects) == false)
            {
                if (IsTurnFinished == false)
                {
                    EntityAfflicted.SetMaxTurns(0);
                    Debug.Log($"{StatusType} set MaxTurns to 0 for {EntityAfflicted.Name}");
                }
            }
        }

        protected override void OnSuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.SetMaxTurns(EntityAfflicted.BaseTurns);

                //Remove the Immobile property
                EntityAfflicted.SubtractIntAdditionalProperty(Enumerations.AdditionalProperty.Immobile, 1);
            }
        }

        protected override void OnUnsuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.SetMaxTurns(0);

                //Re-add the Immobile property
                EntityAfflicted.AddIntAdditionalProperty(Enumerations.AdditionalProperty.Immobile, 1);
            }
        }

        public override StatusEffect Copy()
        {
            return new StopStatus(Duration);
        }
    }
}
