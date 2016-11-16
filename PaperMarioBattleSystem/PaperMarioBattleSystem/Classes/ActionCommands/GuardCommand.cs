using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The action command for Guard.
    /// <para>The result is not known until the attack hits. As such, this action command's responses aid in determining the result</para>
    /// </summary>
    public class GuardCommand : ActionCommand
    {
        protected double GuardCooldown = (10d / 60d) * Time.MsPerS;

        protected double PrevGuardInputTime = 0f;
        protected double PrevGuardCooldown = 0f;

        protected Keys GuardButton = Keys.Z;

        public GuardCommand(IActionCommandHandler commandAction) : base(commandAction)
        {
            
        }

        public override void StartInput()
        {
            base.StartInput();

            PrevGuardInputTime = 0f;
            PrevGuardCooldown = 0f;
        }

        public override void EndInput()
        {
            base.EndInput();

            PrevGuardInputTime = 0f;
            PrevGuardCooldown = 0f;
        }

        protected override void ReadInput()
        {
            if (Input.GetKeyDown(GuardButton) == true)
            {
                if (Time.ActiveMilliseconds >= PrevGuardCooldown)
                {
                    //Debug.Log("Pressed correct time for Guard!");

                    Handler.OnCommandSuccess();
                }
                else
                {
                    Handler.OnCommandFailed();
                }

                PrevGuardInputTime = Time.ActiveMilliseconds;
                PrevGuardCooldown = Time.ActiveMilliseconds + GuardCooldown;
            }
        }
    }
}
