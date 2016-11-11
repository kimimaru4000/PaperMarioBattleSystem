using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public List<ActionButton> ActionButtons = new List<ActionButton>();

        public MarioBattleMenu() : base(MenuTypes.Horizontal)
        {
            ActionButtons.Add(new ActionButton("Jump", AssetManager.Instance.LoadAsset<Texture2D>("UI/Battle/JumpButton"),
                new Vector2(-170, 50), new JumpSubMenu()));
            ActionButtons.Add(new ActionButton("Hammer", AssetManager.Instance.LoadAsset<Texture2D>("UI/Battle/JumpButton"),
                new Vector2(-120, 50), new HammerSubMenu()));
        }

        protected override void HandleSelectionInput()
        {
            if (Input.GetKeyDown(Keys.X)) OnBackOut();
            else if (Input.GetKeyDown(Keys.Z)) OnConfirm();
            else if (Input.GetKeyDown(Keys.C))
                BattleManager.Instance.SwitchToTurn(Enumerations.PlayerTypes.Partner, true);
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
