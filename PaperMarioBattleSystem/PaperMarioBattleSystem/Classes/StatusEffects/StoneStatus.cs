using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Stone Status Effect.
    /// This is essentially Immobilized except the entity afflicted with it is immune to damage.
    /// <para>This has a Positive Alignment because entities that attack an entity afflicted with this basically waste their turns</para>
    /// </summary>
    public sealed class StoneStatus : ImmobilizedStatus
    {
        public StoneStatus(int duration) : base(duration)
        {
            StatusType = Enumerations.StatusTypes.Stone;
            Alignment = StatusAlignments.Positive;
        }

        protected sealed override void OnAfflict()
        {
            base.OnAfflict();

            EntityAfflicted.AddMiscProperty(Enumerations.MiscProperty.StatusImmune, new MiscValueHolder(true));
            EntityAfflicted.AddMiscProperty(Enumerations.MiscProperty.Invincible, new MiscValueHolder(true));

            EntityAfflicted.PlayAnimation(AnimationGlobals.StatusBattleAnimations.StoneName);
        }

        protected sealed override void OnEnd()
        {
            base.OnEnd();

            EntityAfflicted.RemoveMiscProperty(Enumerations.MiscProperty.StatusImmune);
            EntityAfflicted.RemoveMiscProperty(Enumerations.MiscProperty.Invincible);

            EntityAfflicted.PlayAnimation(AnimationGlobals.IdleName);
        }

        protected sealed override void OnSuspend()
        {
            base.OnSuspend();

            EntityAfflicted.RemoveMiscProperty(Enumerations.MiscProperty.StatusImmune);
            EntityAfflicted.RemoveMiscProperty(Enumerations.MiscProperty.Invincible);

            EntityAfflicted.PlayAnimation(AnimationGlobals.IdleName);
        }

        protected sealed override void OnResume()
        {
            base.OnResume();

            EntityAfflicted.AddMiscProperty(Enumerations.MiscProperty.StatusImmune, new MiscValueHolder(true));
            EntityAfflicted.AddMiscProperty(Enumerations.MiscProperty.Invincible, new MiscValueHolder(true));

            EntityAfflicted.PlayAnimation(AnimationGlobals.StatusBattleAnimations.StoneName);
        }

        public sealed override StatusEffect Copy()
        {
            return new StoneStatus(Duration);
        }
    }
}
