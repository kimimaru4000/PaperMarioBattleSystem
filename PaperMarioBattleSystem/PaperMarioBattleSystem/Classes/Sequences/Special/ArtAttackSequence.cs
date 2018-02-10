using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for the Art Attack move.
    /// </summary>
    public sealed class ArtAttackSequence : CrystalStarMoveSequence
    {
        /// <summary>
        /// The ArtAttackResponse containing information from the Art Attack Action Command.
        /// </summary>
        private ActionCommandGlobals.ArtAttackResponse Response = default(ActionCommandGlobals.ArtAttackResponse);

        public ArtAttackSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override void OnCommandResponse(in object response)
        {
            base.OnCommandResponse(response);

            Response = (ActionCommandGlobals.ArtAttackResponse)response;
        }

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //1. Start Art Attack's Action Command
                    //2. Start drawing
                    //3. For each circle completed, a response will be sent down with the bounding box, then go to the Success branch
                    //4. Go back to this branch and restart the command

                    StartActionCommandInput();
                    CurSequenceAction = new WaitForCommandSeqAction(0d, actionCommand, CommandEnabled);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceSuccessBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    DrawingAttemptDamageEntities();

                    ChangeSequenceBranch(SequenceBranch.Main);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        /// <summary>
        /// Attempt to damage BattleEntities with information from the response sent back from Action Command.
        /// </summary>
        private void DrawingAttemptDamageEntities()
        {
            Rectangle boundingBox = Response.BoundingBox;

            for (int i = 0; i < EntitiesAffected.Length; i++)
            {
                //Check how much of the entity the box covers
                //NOTE: Use their position and keep the damage at 1 for now

                BattleEntity entity = EntitiesAffected[i];

                int damageMod = 0;

                //Since Art Attack's lines take place on the UI layer, convert the entity's position to UI coordinates
                Vector2 entityPos = Camera.Instance.SpriteToUIPos(entity.Position);

                //At least 1 damage is dealt
                if (boundingBox.Contains(entityPos) == true)
                {
                    AttemptDamage(BaseDamage + damageMod, entity, Action.DamageProperties, true);

                    ShowCommandRankVFX(HighestCommandRank, entity.Position);
                }
            }
        }
    }
}
