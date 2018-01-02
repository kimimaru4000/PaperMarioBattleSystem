using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Koops' Shell Shield action.
    /// </summary>
    public class ShellShieldAction : MoveAction
    {
        /// <summary>
        /// The Max HP the Shell can have.
        /// </summary>
        private int MaxShellHP = 8;

        private float BarScale = 200f;
        private float MaxBarVal = 100f;

        private double CommandTime = 5000d;
        private double CursorTime = 200d;

        public ShellShieldAction()
        {
            Name = "Shell Shield";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(874, 46, 22, 22)),
                "Protect Mario from attacks\nwith a giant shell.", Enumerations.MoveResourceTypes.FP, 4, Enumerations.CostDisplayTypes.Shown,
                Enumerations.MoveAffectionTypes.Ally, TargetSelectionMenu.EntitySelectionType.Single, false, null);

            SetMoveSequence(new ShellShieldSequence(this, MaxShellHP));
            actionCommand = new ShellShieldCommand(MoveSequence, BarScale, MaxBarVal, CommandTime, CursorTime, GetCommandRangeData());
        }

        private ShellShieldCommand.BarRangeData[] GetCommandRangeData()
        {
            //Scale them by the max bar value so it works for any value
            return new ShellShieldCommand.BarRangeData[]
            {
                new ShellShieldCommand.BarRangeData(0, (MaxBarVal * .2f) + 1, 2, ActionCommand.CommandRank.Nice, Color.Blue),
                new ShellShieldCommand.BarRangeData((MaxBarVal * .8f) + 1, (MaxBarVal + 1), 2, ActionCommand.CommandRank.Nice, Color.Blue),

                new ShellShieldCommand.BarRangeData((MaxBarVal * .2f) + 1, (MaxBarVal * .35f) + 1, 4, ActionCommand.CommandRank.Nice, Color.DarkBlue),
                new ShellShieldCommand.BarRangeData((MaxBarVal * .65f) + 1, (MaxBarVal * .8f) + 1, 4, ActionCommand.CommandRank.Nice, Color.DarkBlue),

                new ShellShieldCommand.BarRangeData((MaxBarVal * .35f) + 1, (MaxBarVal * .45f) + 1, 6, ActionCommand.CommandRank.Good, Color.DarkRed),
                new ShellShieldCommand.BarRangeData((MaxBarVal * .55f) + 1, (MaxBarVal * .65f) + 1, 6, ActionCommand.CommandRank.Good, Color.DarkRed),

                new ShellShieldCommand.BarRangeData((MaxBarVal * .45f) + 1, (MaxBarVal * .55f) + 1, 8, ActionCommand.CommandRank.Great, Color.Orange)
            };

            //new ShellShieldCommand.BarRangeData(0, 21, 2, ActionCommand.CommandRank.Nice, Color.Blue),
            //    new ShellShieldCommand.BarRangeData(80, 101, 2, ActionCommand.CommandRank.Nice, Color.Blue),
            //
            //    new ShellShieldCommand.BarRangeData(21, 36, 4, ActionCommand.CommandRank.Nice, Color.DarkBlue),
            //    new ShellShieldCommand.BarRangeData(65, 80, 4, ActionCommand.CommandRank.Nice, Color.DarkBlue),
            //
            //    new ShellShieldCommand.BarRangeData(36, 45, 6, ActionCommand.CommandRank.Nice, Color.DarkRed),
            //    new ShellShieldCommand.BarRangeData(56, 65, 6, ActionCommand.CommandRank.Nice, Color.DarkRed),
            //
            //    new ShellShieldCommand.BarRangeData(45, 56, 8, ActionCommand.CommandRank.Great, Color.Orange)
        }
    }
}
