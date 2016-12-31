using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The menu that shows Special Moves, if they are available.
    /// <para>We can put both Star Spirit and Crystal Star Special Moves in this menu.
    /// Selecting one would bring up another menu with the respective set of Special Moves available.</para>
    /// </summary>
    public class SpecialSubMenu : ActionSubMenu
    {
        public SpecialSubMenu()
        {
            Position = new Vector2(230, 150);

            BattleActions.Add(new Focus());

            if (BattleActions.Count == 0)
            {
                SpecialMoveAction test = new SpecialMoveAction("No Specials", new MoveActionData(null, 0, "No Special Moves are available.",
                    TargetSelectionMenu.EntitySelectionType.Single, Enumerations.EntityTypes.Player, false, null), new NoSequence(null),
                    StarPowerGlobals.StarPowerTypes.CrystalStar, 0f);

                BattleActions.Add(test);
            }
        }
    }
}
