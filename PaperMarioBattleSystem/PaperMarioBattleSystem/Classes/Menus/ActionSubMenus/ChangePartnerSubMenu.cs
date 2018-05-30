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
        public ChangePartnerSubMenu(BattleEntity user) : base(user)
        {
            Name = "Change Partner";
            Position = new Vector2(230, 150);

            HeaderSize.X = 200f;

            BattlePartner[] partners = Inventory.Instance.partnerInventory.GetAllPartners();

            for (int i = 0; i < partners.Length; i++)
            {
                ChangePartnerAction partnerChange = new ChangePartnerAction(User, partners[i]);

                //If this Partner is the current one out in battle or is dead, disable the option to select it
                if (partners[i] == BattleManager.Instance.Partner || partners[i].IsDead == true)
                {
                    partnerChange.Disabled = true;
                    if (partners[i].IsDead == false)
                    {
                        partnerChange.DisabledString = $"{partners[i].Name} is already out!";
                    }
                    else
                    {
                        partnerChange.DisabledString = $"{partners[i].Name} is unable to battle!";
                    }
                }

                BattleActions.Add(partnerChange);
            }
        }
    }
}
