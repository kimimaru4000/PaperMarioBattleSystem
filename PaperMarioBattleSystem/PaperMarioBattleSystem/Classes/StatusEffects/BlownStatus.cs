using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Blown Status Effect.
    /// Entities afflicted with it are blown out of battle and removed, essentially dying.
    /// <para>This Status Effect is inflicted with Flurrie's Gale Force move and Lakilester's Hurricane move.</para>
    /// </summary>
    public sealed class BlownStatus : MessageEventStatus
    {
        /// <summary>
        /// The time it takes for BattleEntities afflicted with Blown to move offscreen.
        /// </summary>
        private const double BlownMoveTime = 1200d;

        public BlownStatus()
        {
            StatusType = Enumerations.StatusTypes.Blown;
            Alignment = StatusAlignments.Negative;

            //Blown doesn't have an icon, as once it's inflicted, the entity dies
            StatusIcon = null;

            //Blown doesn't have a duration, as once it's inflicted, the entity dies
            Duration = 1;
        }

        protected override void OnAfflict()
        {
            //Remove entities afflicted with Blown
            EntityAfflicted.BManager.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.BlownAway,
                new BattleGlobals.BattleState[] { BattleGlobals.BattleState.Turn, BattleGlobals.BattleState.TurnEnd },
                new BlownAwayBattleEvent(EntityAfflicted, new Vector2(RenderingGlobals.BaseResolutionWidth + 100f, EntityAfflicted.Position.Y), BlownMoveTime));
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
            return new BlownStatus();
        }
    }
}
