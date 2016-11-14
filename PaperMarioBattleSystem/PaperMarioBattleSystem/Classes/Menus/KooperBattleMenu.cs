using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public class KooperBattleMenu : BattleMenu
    {
        protected override int LastSelection
        {
            get
            {
                return ActionButtons.Count - 1;
            }
        }

        public List<ActionButton> ActionButtons = new List<ActionButton>();

        public KooperBattleMenu() : base(MenuTypes.Horizontal)
        {
            ActionButtons.Add(new ActionButton("Abilities", AssetManager.Instance.LoadAsset<Texture2D>("UI/Battle/JumpButton"),
                new Vector2(-190, 50), new KooperSubMenu()));
        }

        protected override void HandleSelectionInput()
        {
            if (Input.GetKeyDown(Keys.X)) OnBackOut();
            else if (Input.GetKeyDown(Keys.Z)) OnConfirm();
            else if (Input.GetKeyDown(Keys.C))
                BattleManager.Instance.SwitchToTurn(Enumerations.PlayerTypes.Mario, true);
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
