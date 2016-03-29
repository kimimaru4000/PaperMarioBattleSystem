using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's battle menu. It contains Strategies, Items, Jump, Hammer, and Star Spirits
    /// </summary>
    public sealed class MarioBattleMenu : BattleMenu
    {
        protected override int LastSelection
        {
            get
            {
                return ActionButtons.Count - 1;
            }
        }

        public List<ActionButton> ActionButtons = null;

        public MarioBattleMenu() : base(MenuTypes.Horizontal)
        {
            ActionButtons = new List<ActionButton>() { new JumpButton() };
        }

        protected override void HandleSelectionInput()
        {
            if (Input.GetKeyDown(Keys.X)) OnBackOut();
            else if (Input.GetKeyDown(Keys.Z)) OnConfirm();
            else if (Input.GetKeyDown(Keys.C)) BattleManager.Instance.SwitchToTurn(true);
        }

        protected override void OnConfirm()
        {
            ActionButtons[CurSelection].OnSelected();
        }

        public override void Draw()
        {
            for (int i = 0; i < ActionButtons.Count; i++)
            {
                ActionButtons[i].Draw();
            }
        }
    }
}
