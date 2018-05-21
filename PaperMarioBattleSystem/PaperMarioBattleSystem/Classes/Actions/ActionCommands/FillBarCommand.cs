using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A base class for Action Commands that involve filling up a bar
    /// </summary>
    public abstract class FillBarCommand : ActionCommand
    {
        /// <summary>
        /// The amount the bar progressed
        /// </summary>
        public double CurBarValue = 0d;

        /// <summary>
        /// The max value of the bar
        /// </summary>
        public double MaxBarValue = 100d;

        public bool IsBarFull => (CurBarValue >= MaxBarValue);

        protected FillBarCommand(IActionCommandHandler commandAction, double maxBarValue) : base(commandAction)
        {
            MaxBarValue = maxBarValue;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            CurBarValue = 0d;
        }

        protected void FillBar(double amount)
        {
            FillBar(amount, false);
        }

        protected void FillBar(double amount, bool clamp)
        {
            CurBarValue += amount;
            
            //Clamp the bar
            if (clamp == true)
            {
                CurBarValue = UtilityGlobals.Clamp(CurBarValue, 0d, MaxBarValue);
            }
        }
    }
}
