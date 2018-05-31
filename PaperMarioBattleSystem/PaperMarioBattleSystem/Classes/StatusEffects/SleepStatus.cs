using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sleep Status Effect.
    /// Entities afflicted with this cannot move until it ends.
    /// There is a 50% chance that the entity will wake up and end this status when it is attacked.
    /// </summary>
    public sealed class SleepStatus : StopStatus
    {
        /// <summary>
        /// The chance of the BattleEntity waking up from sleep after being hit by an attack.
        /// </summary>
        private const int WakeUpChance = 50;

        private const double WakeUpEffectDur = 1000d;

        private SleepZVFX SleepVFX = null;

        public SleepStatus(int duration) : base(duration)
        {
            StatusType = Enumerations.StatusTypes.Sleep;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(555, 9, 38, 46));

            AfflictedMessage = "Sleepy! It'll take time for\nthe sleepiness to wear off!";
        }

        protected override void OnAfflict()
        {
            base.OnAfflict();

            EntityAfflicted.DamageTakenEvent -= OnEntityDamaged;
            EntityAfflicted.DamageTakenEvent += OnEntityDamaged;

            EntityAfflicted.BManager.EntityAddedEvent -= OnEntityAdded;
            EntityAfflicted.BManager.EntityAddedEvent += OnEntityAdded;

            EntityAfflicted.BManager.EntityRemovedEvent -= OnEntityRemoved;
            EntityAfflicted.BManager.EntityRemovedEvent += OnEntityRemoved;

            //Add the sleep VFX
            AddSleepVFX();
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            EntityAfflicted.DamageTakenEvent -= OnEntityDamaged;
            EntityAfflicted.BManager.EntityAddedEvent -= OnEntityAdded;
            EntityAfflicted.BManager.EntityRemovedEvent -= OnEntityRemoved;

            //Remove the sleep VFX
            RemoveSleepVFX();
        }

        private void OnEntityDamaged(in InteractionHolder damageInfo)
        {
            //Attacks that miss or deal less than 1 damage can't wake up BattleEntities
            if (damageInfo.Hit == false || damageInfo.TotalDamage <= 0) return;

            //Test if the Entity afflicted with sleep should wake up
            if (UtilityGlobals.TestRandomCondition(WakeUpChance) == true)
            {
                Debug.Log($"{EntityAfflicted.Name} woke up when taking damage!");

                EntityAfflicted.DamageTakenEvent -= OnEntityDamaged;

                //Show the little exclamation icon indicating the BattleEntity woke up - it's the same one for stylish data
                BattleObjManager.Instance.AddBattleObject(new StylishIndicatorVFX(EntityAfflicted, new Sequence.StylishData(0d, WakeUpEffectDur, 0)));

                //Remove the status
                EntityAfflicted.RemoveStatus(StatusType, true, false);
            }
        }

        private void OnEntityAdded(BattleEntity battleEntity)
        {
            if (battleEntity != EntityAfflicted) return;
            
            //If added to battle but not cleaned up beforehand (Ex. Partners), add the sleep VFX back
            AddSleepVFX();
        }

        private void OnEntityRemoved(BattleEntity battleEntity)
        {
            if (battleEntity != EntityAfflicted) return;

            //If removed from battle but not cleaned up (Ex. Partners), remove the sleep VFX
            RemoveSleepVFX();
        }

        private void AddSleepVFX()
        {
            if (SleepVFX == null)
            {
                SleepVFX = new SleepZVFX(EntityAfflicted);
                BattleObjManager.Instance.AddBattleObject(SleepVFX);
            }
        }

        private void RemoveSleepVFX()
        {
            if (SleepVFX != null)
            {
                BattleObjManager.Instance.RemoveBattleObject(SleepVFX);
                SleepVFX = null;
            }
        }

        public sealed override StatusEffect Copy()
        {
            return new SleepStatus(Duration);
        }
    }
}
