using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Goombario's Tattle action.
    /// </summary>
    public sealed class Tattle : MoveAction
    {
        public Tattle(bool isGoombella)
        {
            Name = "Tattle";

            MoveInfo = new MoveActionData(null, "View enemies' descriptions\nand see their HP in battle.", MoveResourceTypes.FP, 0,
                CostDisplayTypes.Hidden, Enumerations.MoveAffectionTypes.Custom, TargetSelectionMenu.EntitySelectionType.Single, false,
                null, User.GetOpposingEntityType(), EntityTypes.Neutral);

            SetMoveSequence(new TattleSequence(this));
            
            //Default to no Action Command
            //If this is Goombella, then set the Action Command
            actionCommand = null;
            if (isGoombella == true)
            {
                actionCommand = new TattleCommand(MoveSequence, 2f);
            }
        }

        public override void Initialize()
        {
            //Check if there are any BattleEntities that can be tattled
            //If there are none, disable the action
            BattleEntity[] tattleEntities = GetTattleableEntities();
            if (tattleEntities.Length == 0)
            {
                Disabled = true;
                DisabledString = "There's no one that can be tattled!";
            }
        }

        protected override BattleEntity[] GetCustomAffectedEntities()
        {
            return GetTattleableEntities();
        }

        /// <summary>
        /// Gets all BattleEntities that can be tattled.
        /// They all implement the <see cref="ITattleableEntity"/> interface.
        /// </summary>
        /// <returns>An array of BattleEntities of the opposing EntityType that can be tattled.</returns>
        private BattleEntity[] GetTattleableEntities()
        {
            List<BattleEntity> tattleableEntities = new List<BattleEntity>();

            //Get all BattleEntities of the types specified in the move information
            if (MoveProperties.OtherEntTypes != null)
            {
                for (int i = 0; i < MoveProperties.OtherEntTypes.Length; i++)
                {
                    BattleEntity[] entities = BattleManager.Instance.GetEntities(MoveProperties.OtherEntTypes[i], null);

                    for (int j = 0; j < entities.Length; j++)
                    {
                        //Safely cast to see if the BattleEntity can be tattled and add it if so
                        ITattleableEntity tattleableEntity = entities[j] as ITattleableEntity;
                        if (tattleableEntity != null)
                        {
                            tattleableEntities.Add(entities[j]);
                        }
                    }
                }
            }

            return tattleableEntities.ToArray();
        }
    }
}
