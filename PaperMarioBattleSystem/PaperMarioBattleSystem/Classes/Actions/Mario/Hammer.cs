using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's Hammer action
    /// </summary>
    public class Hammer : BattleAction
    {
        protected float WalkDuration = 1000f;
        protected int DamageMod = 1;

        /// <summary>
        /// The number of lights lit at which the hammer windup animation's speed increases
        /// </summary>
        protected int LitWindupSpeed = 3;

        protected string PickupAnimName = AnimationGlobals.MarioBattleAnimations.HammerPickupName;
        protected string WindupAnimName = AnimationGlobals.MarioBattleAnimations.HammerWindupName;
        protected string SlamAnimName = AnimationGlobals.MarioBattleAnimations.HammerSlamName;

        public Hammer()
        {
            Name = "Hammer";
            Description = "Whack an enemy with your Hammer.";
            BaseDamage = 1;

            Command = new HammerCommand(this);
        }

        public override void OnCommandSuccess()
        {
            DamageMod *= 2;
        }

        public override void OnCommandFailed()
        {
            
        }

        public override void OnCommandResponse(int response)
        {
            if (response == LitWindupSpeed)
            {
                Animation windupAnim = User.GetAnimation(WindupAnimName);
                windupAnim?.SetSpeed(2f);
            }
        }

        protected override void OnProgressSequence()
        {
            float x = User.EntityType == Enumerations.EntityTypes.Player ? 100f : -100f;

            switch (SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequence = new MoveTo(BattleManager.Instance.GetPositionInFront(EntitiesAffected[0]), WalkDuration);
                    break;
                case 1:
                    User.PlayAnimation(PickupAnimName, true);
                    CurSequence = new WaitForAnimation(PickupAnimName);
                    break;
                case 2:
                    User.PlayAnimation(WindupAnimName);
                    if (CommandEnabled == true) Command.StartInput();
                    CurSequence = new WaitForCommand(1500f, Command, CommandEnabled);
                    break;
                case 3:
                    User.PlayAnimation(SlamAnimName, true);
                    DealDamage(BaseDamage * DamageMod);
                    CurSequence = new WaitForAnimation(SlamAnimName);
                    break;
                case 4:
                    User.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequence = new MoveTo(User.BattlePosition, WalkDuration);
                    break;
                default:
                    User.PlayAnimation(AnimationGlobals.IdleName, true);
                    EndSequence();
                    break;
            }
        }
    }
}
