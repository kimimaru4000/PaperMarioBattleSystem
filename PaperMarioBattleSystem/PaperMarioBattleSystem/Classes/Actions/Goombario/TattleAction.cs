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
    /// Goombario's Tattle action.
    /// </summary>
    public sealed class TattleAction : MoveAction
    {
        public TattleAction(BattleEntity user, bool isGoombella) : base(user)
        {
            Name = "Tattle";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(874, 14, 22, 22)),
                "View enemies' descriptions\nand see their HP in battle.", MoveResourceTypes.FP, 0,
                CostDisplayTypes.Hidden, Enumerations.MoveAffectionTypes.Custom, Enumerations.EntitySelectionType.Single, false,
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

        protected override void GetCustomAffectedEntities(List<BattleEntity> entityList)
        {
            GetTattleableEntities(entityList);
        }
        
        /// <summary>
        /// Gets all BattleEntities that can be tattled.
        /// They all implement the <see cref="ITattleableEntity"/> interface and are available to be tattled.
        /// </summary>
        /// <param name="entityList">The list to add the BattleEntities that can be tattled into.</param>
        private void GetTattleableEntities(List<BattleEntity> entityList)
        {
            //Get all BattleEntities of the types specified in the move information
            if (MoveProperties.OtherEntTypes != null && MoveProperties.OtherEntTypes.Length > 0)
            {
                List<BattleEntity> entities = new List<BattleEntity>();

                for (int i = 0; i < MoveProperties.OtherEntTypes.Length; i++)
                {
                    User.BManager.GetEntities(entities, MoveProperties.OtherEntTypes[i], null);

                    for (int j = 0; j < entities.Count; j++)
                    {
                        //Safely cast to see if the BattleEntity can be tattled
                        ITattleableEntity tattleableEntity = entities[j] as ITattleableEntity;

                        //Add it if the entity is tattleable and can currently be tattled
                        if (tattleableEntity != null && tattleableEntity.CanBeTattled == true)
                        {
                            entityList.Add(entities[j]);
                        }
                    }

                    entities.Clear();
                }
            }
        }
    }
}
