using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    public class PokeySegmentBehavior : ISegmentBehavior
    {
        public BattleEntity Entity = null;

        public uint MaxSegments { get; set; } = 4;

        public uint CurSegmentCount { get; set; } = 4;

        public Enumerations.DamageEffects SegmentRemovedOnEffects { get; set; } = Enumerations.DamageEffects.RemovesSegment;

        public PokeySegmentBehavior(BattleEntity entity, uint maxSegments, Enumerations.DamageEffects segmentRemovedOnEffects)
        {
            Entity = entity;

            MaxSegments = maxSegments;
            CurSegmentCount = MaxSegments;

            SegmentRemovedOnEffects = segmentRemovedOnEffects;

            //Subscribe to taking damage so it can check the damage effects
            Entity.DamageTakenEvent -= OnDamageTaken;
            Entity.DamageTakenEvent += OnDamageTaken;
        }

        public void CleanUp()
        {
            if (Entity != null)
            {
                Entity.DamageTakenEvent -= OnDamageTaken;
            }
        }

        public void HandleSegmentAdded(uint segmentsAdded)
        {
            uint finalSegmentCount = CurSegmentCount + segmentsAdded;
            if (finalSegmentCount > MaxSegments)
            {
                Debug.LogWarning($"Cannot add {segmentsAdded} segments; the number of segments is >= {MaxSegments}. Setting {nameof(CurSegmentCount)} to {MaxSegments}");
                CurSegmentCount = MaxSegments;
            }
            else
            {
                CurSegmentCount += segmentsAdded;
            }
        }

        public void HandleSegmentRemoved(uint segmentsRemoved)
        {
            int finalSegmentCount = (int)(CurSegmentCount - segmentsRemoved);
            if (finalSegmentCount < 0)
            {
                Debug.LogWarning($"Cannot remove {segmentsRemoved} segments since it makes {nameof(CurSegmentCount)} less than 0. Setting {nameof(CurSegmentCount)} to 0");

                CurSegmentCount = 0;
            }
            else
            {
                CurSegmentCount -= segmentsRemoved;
            }
        }

        protected virtual void OnDamageTaken(in InteractionHolder damageInfo)
        {
            if (Entity.IsDead == true || damageInfo.Hit == false) return;

            //Check if the entity was hit with DamageEffects that remove a segment
            if (CurSegmentCount > 0 && UtilityGlobals.DamageEffectHasFlag(SegmentRemovedOnEffects, damageInfo.DamageEffect) == true)
            {
                //Remove a segment
                HandleSegmentRemoved(1);
            }
        }
    }
}
