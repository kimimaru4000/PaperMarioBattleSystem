using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The second half of Bow's Outta Sight Sequence.
    /// <para>This occurs at the start of Bow's next turn through an event in Outta Sight.
    /// It should not be assigned to an action added to a menu.</para>
    /// </summary>
    public sealed class OuttaSightSecondHalfSequence : Sequence
    {
        private const double MoveTime = 500d;

        private float AlphaVal = .3f;
        
        public OuttaSightSecondHalfSequence(MoveAction moveAction, float alphaVal) : base(moveAction)
        {
            AlphaVal = alphaVal;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Remove the evasion mod and turn transparency back to normal
                    //NOTE: Find a way to get the original transparency (Ex. Invisible causes additional transparency)
                    BattleEntity allyAffected = EntitiesAffected[0];
                    User.RemoveEvasionMod(0d);
                    allyAffected.RemoveEvasionMod(0d);

                    //Turn transparency back
                    Color entityColor = User.TintColor;
                    Color allyColor = allyAffected.TintColor;

                    double colorDiff = (1 / (double)AlphaVal);
                
                    User.TintColor = new Color((int)Math.Ceiling(entityColor.R * colorDiff), (int)Math.Ceiling(entityColor.G * colorDiff), (int)Math.Ceiling(entityColor.B * colorDiff), (int)Math.Ceiling(entityColor.A * colorDiff));
                    allyAffected.TintColor = new Color((int)Math.Ceiling(allyColor.R * colorDiff), (int)Math.Ceiling(allyColor.G * colorDiff), (int)Math.Ceiling(allyColor.B * colorDiff), (int)Math.Ceiling(allyColor.A * colorDiff));

                    User.EntityProperties.UnsuppressStatuses(Enumerations.StatusSuppressionTypes.Effects, Enumerations.StatusTypes.Poison, Enumerations.StatusTypes.Burn, Enumerations.StatusTypes.Frozen);
                    allyAffected.EntityProperties.UnsuppressStatuses(Enumerations.StatusSuppressionTypes.Effects, Enumerations.StatusTypes.Poison, Enumerations.StatusTypes.Burn, Enumerations.StatusTypes.Frozen);
                    User.EntityProperties.UnsuppressStatuses(Enumerations.StatusSuppressionTypes.TurnCount, Enumerations.StatusTypes.Invisible);
                    allyAffected.EntityProperties.UnsuppressStatuses(Enumerations.StatusSuppressionTypes.TurnCount, Enumerations.StatusTypes.Invisible);

                    //Make the ally play its idle animation
                    allyAffected.AnimManager.PlayAnimation(EntitiesAffected[0].GetIdleAnim());

                    CurSequenceAction = new WaitSeqAction(0d);

                    ChangeSequenceBranch(SequenceBranch.Main);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Make the user move back to its battle position
                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition, MoveTime);

                    ChangeSequenceBranch(SequenceBranch.End);
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
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceEndBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //End the sequence
                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMissBranch()
        {
            switch (SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
