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

            EntityAfflicted.DamageTakenEvent += OnEntityDamaged;
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            EntityAfflicted.DamageTakenEvent -= OnEntityDamaged;
        }

        private void OnEntityDamaged(InteractionHolder damageInfo)
        {
            //Attacks that miss or deal less than 1 damage can't wake up BattleEntities
            if (damageInfo.Hit == false || damageInfo.TotalDamage <= 0) return;

            //Test if the Entity afflicted with sleep should wake up
            if (UtilityGlobals.TestRandomCondition(WakeUpChance) == true)
            {
                Debug.Log($"{EntityAfflicted.Name} woke up while taking damage!");

                EntityAfflicted.DamageTakenEvent -= OnEntityDamaged;

                //Show the little exclamation icon indicating the BattleEntity woke up - it's the same one for stylish data
                BattleObjManager.Instance.AddBattleObject(new StylishIndicatorVFX(EntityAfflicted, new Sequence.StylishData(0d, WakeUpEffectDur, 0)));

                //Remove the status
                EntityAfflicted.RemoveStatus(StatusType, true, false);
            }
        }

        public sealed override StatusEffect Copy()
        {
            return new SleepStatus(Duration);
        }
    }
}
