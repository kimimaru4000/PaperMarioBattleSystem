using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Gulpit enemy AI.
    /// </summary>
    public sealed class GulpitAI : EnemyAIBehavior
    {
        public GulpitAI(BattleEnemy gulpit) : base(gulpit)
        {

        }

        public override void PerformAction()
        {
            //If any IUsableEntities are found in the Neutral BattleEntity list, perform Rock Spit with it
            //Otherwise, perform Lick
            List<BattleEntity> usableEntities = new List<BattleEntity>();

            int chosenIndex = -1;

            //Get all Neutral entities
            BattleEntity[] neutralEntities = BattleManager.Instance.GetEntities(Enumerations.EntityTypes.Neutral, Enumerations.HeightStates.Grounded);
            for (int i = 0; i < neutralEntities.Length; i++)
            {
                IUsableEntity usableEntity = neutralEntities[i] as IUsableEntity;
                //Add to the list if this entity is usable
                if (usableEntity != null)
                {
                    usableEntities.Add(neutralEntities[i]);
                }
            }

            //Choose a random entity out of the ones we found
            if (usableEntities.Count > 0)
            {
                chosenIndex = GeneralGlobals.Randomizer.Next(0, usableEntities.Count);
            }

            //If we found and chose a usable entity, use Rock Spit with the entity
            //Otherwise, use Lick
            MoveAction action = null;
            if (chosenIndex >= 0)
                action = new RockSpitAction(usableEntities[chosenIndex]);
            else action = new LickAction();

            Enemy.StartAction(action, false, BattleManager.Instance.GetFrontPlayer().GetTrueTarget());
        }
    }
}
