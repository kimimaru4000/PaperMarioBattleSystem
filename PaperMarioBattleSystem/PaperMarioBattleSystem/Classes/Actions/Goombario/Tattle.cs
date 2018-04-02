using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
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

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(874, 14, 22, 22)),
                "View enemies' descriptions\nand see their HP in battle.", MoveResourceTypes.FP, 0,
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
            base.Initialize();

            if (Disabled == true)
            {
                DisabledString = "There's no one that can be tattled!";
            }
        }

        protected override BattleEntity[] GetCustomAffectedEntities()
        {
            return GetTattleableEntities();
        }
        
        /// <summary>
        /// Gets all BattleEntities that can be tattled.
        /// They all implement the <see cref="ITattleableEntity"/> interface and are available to be tattled.
        /// </summary>
        /// <returns>An array of BattleEntities of the opposing EntityType that can be tattled.</returns>
        private BattleEntity[] GetTattleableEntities()
        {
            List<BattleEntity> tattleableEntities = new List<BattleEntity>();

            //Get all BattleEntities of the types specified in the move information
            if (MoveProperties.OtherEntTypes != null)
            {
                List<BattleEntity> entities = new List<BattleEntity>();

                for (int i = 0; i < MoveProperties.OtherEntTypes.Length; i++)
                {
                    BattleManager.Instance.GetEntities(entities, MoveProperties.OtherEntTypes[i], null);

                    for (int j = 0; j < entities.Count; j++)
                    {
                        //Safely cast to see if the BattleEntity can be tattled
                        ITattleableEntity tattleableEntity = entities[j] as ITattleableEntity;

                        //Add it if the entity is tattleable and can currently be tattled
                        if (tattleableEntity != null && tattleableEntity.CanBeTattled == true)
                        {
                            tattleableEntities.Add(entities[j]);
                        }
                    }

                    entities.Clear();
                }
            }

            return tattleableEntities.ToArray();
        }
    }
}
