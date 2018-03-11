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
    public sealed class BlownStatus : StatusEffect
    {
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
            BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.Death - 1,
                new BattleManager.BattleState[] { BattleManager.BattleState.Turn, BattleManager.BattleState.TurnEnd },
                new BlownAwayBattleEvent(EntityAfflicted, new Vector2(SpriteRenderer.Instance.WindowSize.X, EntityAfflicted.Position.Y), 1200d));
        }

        protected override void OnEnd()
        {

        }

        protected override void OnPhaseCycleStart()
        {

        }

        //Blown cannot be suspended, as it instantly kills any entity afflicted with it
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
