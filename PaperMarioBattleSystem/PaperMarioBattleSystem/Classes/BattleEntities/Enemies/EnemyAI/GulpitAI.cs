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
            Enemy.BManager.GetEntities(usableEntities, Enumerations.EntityTypes.Neutral, Enumerations.HeightStates.Grounded);
            for (int i = 0; i < usableEntities.Count; i++)
            {
                IUsableEntity usableEntity = usableEntities[i] as IUsableEntity;
                //Remove from the list if this entity is not usable
                if (usableEntity == null)
                {
                    usableEntities.RemoveAt(i);
                    i--;
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
                action = new RockSpitAction(Enemy, usableEntities[chosenIndex]);
            else action = new LickAction(Enemy);

            Enemy.StartAction(action, false, Enemy.BManager.FrontPlayer.GetTrueTarget());
        }
    }
}
