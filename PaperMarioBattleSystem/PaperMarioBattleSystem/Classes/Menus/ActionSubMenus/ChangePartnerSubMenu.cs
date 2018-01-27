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
            Name = "Change Partner";
            Position = new Vector2(230, 150);

            HeaderSize.X = 200f;

            BattlePartner[] partners = Inventory.Instance.partnerInventory.GetAllPartners();

            for (int i = 0; i < partners.Length; i++)
            {
                ChangePartner partnerChange = new ChangePartner(partners[i]);

                //If this partner is the current one out in battle, disable the option
                if (partners[i] == BattleManager.Instance.GetPartner())
                {
                    partnerChange.Disabled = true;
                    partnerChange.DisabledString = $"{partners[i].Name} is already out!";
                }

                BattleActions.Add(partnerChange);
            }
        }
    }
}
