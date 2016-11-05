using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    public sealed class SuperguardCommand : GuardCommand
    {
        public SuperguardCommand(ICommandAction commandAction) : base(commandAction)
        {
            GuardButton = Keys.X;

            GuardCooldown = (5d / 60d) * Time.MsPerS;
        }
    }
}
