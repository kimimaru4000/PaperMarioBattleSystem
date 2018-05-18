using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Battle Event when the Shell Shield Shell is broken. The Shell ends its protection.
    /// </summary>
    public sealed class ShellBreakBattleEvent : BattleEvent
    {
        private Shell ShellRef = null;

        public ShellBreakBattleEvent(Shell shell)
        {
            ShellRef = shell;
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            ShellRef = null;
        }

        protected override void OnUpdate()
        {
            //Remove protection
            ShellRef.RemoveEntityDefending();

            //Start the animation in a BattleObject
            ShellBreakAnimObj breakAnim = new ShellBreakAnimObj(ShellRef.Position, 2000d, (float)UtilityGlobals.ToRadians(25d), 750d);
            BattleObjManager.Instance.AddBattleObject(breakAnim);

            End();
        }
    }
}
