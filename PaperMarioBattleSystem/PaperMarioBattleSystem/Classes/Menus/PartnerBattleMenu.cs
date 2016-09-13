using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    public class PartnerBattleMenu : BattleMenu
    {
        protected override int LastSelection
        {
            get
            {
                return ActionButtons.Count - 1;
            }
        }

        public List<ActionButton> ActionButtons = null;

        public PartnerBattleMenu() : base(MenuTypes.Horizontal)
        {
            ActionButtons = new List<ActionButton>() { new BonkButton() };
        }

        protected override void HandleSelectionInput()
        {
            if (Input.GetKeyDown(Keys.X)) OnBackOut();
            else if (Input.GetKeyDown(Keys.Z)) OnConfirm();
            else if (Input.GetKeyDown(Keys.C)) BattleManager.Instance.SwitchToTurn(Enumerations.PlayerTypes.Mario, true);
        }

        protected override void OnConfirm()
        {
            ActionButtons[CurSelection].OnSelected();
        }

        public override void Draw()
        {
            for (int i = 0; i < ActionButtons.Count; i++)
            {
                ActionButtons[i].Draw(CurSelection == i);
            }
        }
    }
}
