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
        protected int DamageMod = 1;

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

        protected override void OnProgressSequence()
        {
            float x = User.EntityType == Enumerations.EntityTypes.Player ? 100f : -100f;

            switch (SequenceStep)
            {
                case 0:
                    CurSequence = new MoveTo(BattleManager.Instance.GetPositionInFront(EntitiesAffected[0]), 1000f);
                    break;
                case 1:
                    if (CommandEnabled == true) Command.StartInput();
                    CurSequence = new WaitForCommand(1500f, Command, CommandEnabled);
                    break;
                case 2:
                    DealDamage(BaseDamage * DamageMod);
                    User.PlayAnimation(AnimationGlobals.HammerName);
                    CurSequence = new WaitForAnimation(AnimationGlobals.HammerName);
                    break;
                case 3:
                    User.PlayAnimation(AnimationGlobals.IdleName);
                    base.OnProgressSequence();
                    break;
                default:
                    EndSequence();
                    break;
            }
        }
    }
}
