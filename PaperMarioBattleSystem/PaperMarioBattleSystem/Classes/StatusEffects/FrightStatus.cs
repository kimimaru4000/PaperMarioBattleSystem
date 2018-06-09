using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Fright Status Effect.
    /// Entities afflicted with it run out of battle in fear and are removed from battle, essentially dying.
    /// </summary>
    public sealed class FrightStatus : MessageEventStatus
    {
        /// <summary>
        /// How long BattleEntities afflicted with Fright run.
        /// </summary>
        private const double FrightRunTime = 1200d;

        /// <summary>
        /// The speed and direction BattleEntities afflicted with Fright run.
        /// </summary>
        private readonly Vector2 FrightSpeed = new Vector2(16f, 0f);

        public FrightStatus()
        {
            StatusType = Enumerations.StatusTypes.Fright;
            Alignment = StatusAlignments.Negative;

            //Fright doesn't have an icon, as once it's inflicted, the entity dies
            StatusIcon = null;

            //Fright doesn't have a duration, as once it's inflicted, the entity dies
            Duration = 1;
        }

        protected override void OnAfflict()
        {
            //Remove entities afflicted with Fright
            EntityAfflicted.BManager.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Fright,
                new BattleGlobals.BattleState[] { BattleGlobals.BattleState.Turn, BattleGlobals.BattleState.TurnEnd },
                new FrightBattleEvent(EntityAfflicted, FrightSpeed, FrightRunTime));
        }

        protected override void OnEnd()
        {

        }

        protected override void OnPhaseCycleStart()
        {

        }

        protected override void OnSuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {

        }

        protected override void OnUnsuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {

        }

        public override StatusEffect Copy()
        {
            return new FrightStatus();
        }
    }
}
