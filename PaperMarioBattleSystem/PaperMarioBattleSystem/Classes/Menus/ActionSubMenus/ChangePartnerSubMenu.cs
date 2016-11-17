using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The menu that shows Mario's list of partners.
    /// You can select one to change to.
    /// </summary>
    public sealed class ChangePartnerSubMenu : ActionSubMenu
    {
        public ChangePartnerSubMenu()
        {
            Position = new Vector2(230, 150);

            BattlePartner[] partners = Inventory.Instance.partnerInventory.GetAllPartners();

            for (int i = 0; i < partners.Length; i++)
            {
                //NOTE: We will need a way to gray out options so they're not selectable. For now, this works
                if (partners[i] == BattleManager.Instance.GetPartner()) continue;

                MoveAction partnerChange = new MoveAction(partners[i].Name, new MoveActionData(null, 0, partners[i].PartnerDescription,
                    TargetSelectionMenu.EntitySelectionType.Single, Enumerations.EntityTypes.Player, null),
                    new ChangePartnerSequence(null, partners[i]));

                BattleActions.Add(partnerChange);
            }

            Initialize(BattleActions);
        }
    }
}
