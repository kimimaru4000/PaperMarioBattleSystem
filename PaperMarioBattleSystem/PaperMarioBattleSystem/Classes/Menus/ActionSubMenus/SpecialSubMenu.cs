﻿using System;
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
            Name = "Special";
            Position = new Vector2(230, 150);

            BattleActions.Add(new FocusAction());
            BattleActions.Add(new SweetTreatAction());
            BattleActions.Add(new RefreshAction());
            BattleActions.Add(new LullabyAction());
            BattleActions.Add(new PowerLiftAction());
            BattleActions.Add(new ArtAttackAction());

            if (BattleActions.Count == 0)
            {
                MessageAction noSpecials = new MessageAction("No Specials", null, "No Special Moves are available.",
                    (int)BattleGlobals.BattleEventPriorities.Message, "You can't select that!");

                BattleActions.Add(noSpecials);
            }
        }
    }
}
