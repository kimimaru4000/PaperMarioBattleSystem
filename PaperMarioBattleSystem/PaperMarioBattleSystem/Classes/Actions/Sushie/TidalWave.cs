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
    /// Sushie's Tidal Wave attack
    /// </summary>
    public class TidalWave : BattleAction
    {
        protected float WalkDuration = 700f;
        protected int AdditionalDamage = 0;

        public TidalWave()
        {
            Name = "Tidal Wave";
            Description = "A surge of water hits all enemies";
            SelectionType = TargetSelectionMenu.EntitySelectionType.All;
            ContactType = Enumerations.ContactTypes.None;
            Element = Enumerations.Elements.Water;
            BaseDamage = 1;
            HeightsAffected = new Enumerations.HeightStates[] { Enumerations.HeightStates.Grounded, Enumerations.HeightStates.Airborne, Enumerations.HeightStates.Ceiling };

            Command = new TidalWaveCommand(this);
        }

        protected override void OnEnd()
        {
            AdditionalDamage = 0;
        }

        public override void OnCommandSuccess()
        {
            ChangeSequenceBranch(SequenceBranch.Success);
        }

        public override void OnCommandFailed()
        {
            ChangeSequenceBranch(SequenceBranch.Failed);
        }

        public override void OnCommandResponse(int response)
        {
            AdditionalDamage = response;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequence = new MoveTo(BattleManager.Instance.GetPositionInFront(BattleManager.Instance.GetEntities(Enumerations.EntityTypes.Player)[0]), WalkDuration);
                    ChangeSequenceBranch(SequenceBranch.Command);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
        
        protected override void SequenceCommandBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.IdleName);
                    if (CommandEnabled == true) Command.StartInput();
                    CurSequence = new WaitForCommand(1500f, Command, CommandEnabled);
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
                    AttemptDamage(BaseDamage + AdditionalDamage, EntitiesAffected);
                    ChangeSequenceBranch(SequenceBranch.End);
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
                    AttemptDamage(BaseDamage + AdditionalDamage, EntitiesAffected);
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceEndBranch()
        {
            switch(SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequence = new MoveTo(User.BattlePosition, WalkDuration);
                    break;
                case 1:
                    User.PlayAnimation(AnimationGlobals.IdleName);
                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
