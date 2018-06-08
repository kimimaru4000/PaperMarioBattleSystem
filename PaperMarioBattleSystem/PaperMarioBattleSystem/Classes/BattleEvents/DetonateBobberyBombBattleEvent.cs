using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PaperMarioBattleSystem.Extensions;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Battle Event when Bobbery Bombs are detonated.
    /// They damage all BattleEntities around them, including other Bobbery Bombs.
    /// <para>This is a Battle Event because this occurs after some other events (Ex. Status Effects)
    /// and after another bomb takes explosive damage, causing it to detonate earlier than usual.</para>
    /// </summary>
    public class DetonateBobberyBombBattleEvent : BattleEvent
    {
        private const double WaitTime = 1000d;

        private BobberyBomb Bomb = null;
        private DamageData DamageInfo = default(DamageData);
        private HeightStates[] AffectedHeightStates = null;
        private Rectangle ExplosionArea = Rectangle.Empty;

        private double ElapsedTime = 0d;

        public DetonateBobberyBombBattleEvent(BobberyBomb bobberyBomb, DamageData damageInfo, Rectangle explosionArea,
            params HeightStates[] heightStates)
        {
            Bomb = bobberyBomb;
            DamageInfo = damageInfo;
            AffectedHeightStates = heightStates;
            ExplosionArea = explosionArea;
        }

        protected override void OnStart()
        {
            base.OnStart();

            //Damage everyone
            List<BattleEntity> entities = new List<BattleEntity>();
            Bomb.BManager.GetAllBattleEntities(entities, AffectedHeightStates);
            entities.Remove(Bomb);

            for (int i = 0; i < entities.Count; i++)
            {
                //If the entity is not in the explosion area, remove it from the list
                if (ExplosionArea.Contains(entities[i].Position) == false)
                {
                    entities.RemoveAt(i);
                    i--;
                }
                else
                {
                    entities[i] = entities[i].GetTrueTarget();
                }
            }

            //Attempt to damage them
            Interactions.AttemptDamageEntities(Bomb, entities, DamageInfo, null);

            ElapsedTime = 0d;
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            //The bomb dies after exploding
            if (Bomb.IsDead == false)
                Bomb.Die();

            Bomb = null;
            AffectedHeightStates = null;
            ElapsedTime = 0d;
        }

        protected override void OnUpdate()
        {
            //Wait some time
            ElapsedTime += Time.ElapsedMilliseconds;

            //End when over the wait time
            if (ElapsedTime >= WaitTime)
            {
                End();
            }
        }
    }
}
