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
    /// Mario's Jump action
    /// </summary>
    public class Jump : BattleAction
    {
        protected float WalkDuration = 1000f;
        protected float JumpDuration = 1000f;

        public Jump()
        {
            Name = "Jump";
            Description = "Jump and stomp on an enemy.";
            ContactType = Enumerations.ContactTypes.JumpContact;
            BaseDamage = 1;

            Command = new JumpCommand(this, JumpDuration, (int)(JumpDuration / 2f));
            HeightsAffected = new Enumerations.HeightStates[] { HeightStates.Grounded, HeightStates.Airborne };
        }

        public override void OnCommandSuccess()
        {
            //Show "NICE" here or something
            SequenceStep += 1;
        }

        public override void OnCommandFailed()
        {

        }

        public override void OnCommandResponse(int response)
        {
            
        }

        public override void OnMenuSelected()
        {
            base.OnMenuSelected();
        }

        protected override void OnProgressSequence()
        {
            switch(SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequence = new MoveTo(BattleManager.Instance.GetPositionInFront(EntitiesAffected[0]), WalkDuration);
                    break;
                case 1:
                    User.PlayAnimation(AnimationGlobals.IdleName);
                    CurSequence = new MoveAmount(new Vector2(0f, -100f), JumpDuration);
                    break;
                case 2:
                    if (SequenceStep == 2 && CommandEnabled == true) Command.StartInput();
                    CurSequence = new MoveAmount(new Vector2(0f, 100f), JumpDuration);
                    break;
                case 3:
                    User.PlayAnimation(AnimationGlobals.RunningName);
                    DealDamage(BaseDamage);
                    CurSequence = new MoveTo(User.BattlePosition, WalkDuration);
                    if (SequenceStep == 3) SequenceStep += 500;
                    break;
                case 4:
                    DealDamage(BaseDamage);
                    goto case 1;
                case 5:
                    goto case 2;
                case 6:
                    goto case 3;
                default:
                    User.PlayAnimation(AnimationGlobals.IdleName);
                    EndSequence();
                    break;
            }
        }
    }
}
