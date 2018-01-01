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

        public ShellShieldAction()
        {
            Name = "Shell Shield";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(874, 46, 22, 22)),
                "Protect Mario from attacks\nwith a giant shell.", Enumerations.MoveResourceTypes.FP, 4, Enumerations.CostDisplayTypes.Shown,
                Enumerations.MoveAffectionTypes.Ally, TargetSelectionMenu.EntitySelectionType.Single, false, null);

            SetMoveSequence(new ShellShieldSequence(this, MaxShellHP));
            actionCommand = null;//
        }
    }
}
