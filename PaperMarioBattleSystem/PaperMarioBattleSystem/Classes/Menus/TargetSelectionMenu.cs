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
    /// The target selection menu when performing an action
    /// </summary>
    public class TargetSelectionMenu : BattleMenu
    {
        private Texture2D Cursor = null;

        private BattleEntity[] Targets = null;

        public delegate void OnSelection(/*params BattleEntity[] targets*/BattleEntity target);
        public event OnSelection SelectionEvent = null;

        protected override int LastSelection => Targets.Length - 1;

        public TargetSelectionMenu() : base(MenuTypes.Horizontal)
        {
            Cursor = AssetManager.Instance.LoadAsset<Texture2D>("UI/Cursor");
        }

        public void StartSelection(OnSelection onSelection, params BattleEntity[] targets)
        {
            Targets = targets;
            SelectionEvent += onSelection;
        }

        protected override void OnBackOut()
        {
            EndSelection();
        }

        protected override void OnConfirm()
        {
            BattleEntity target = Targets[CurSelection];

            SelectionEvent?.Invoke(target);
            EndSelection();
        }

        public void EndSelection()
        {
            CurSelection = 0;
            Targets = null;
            SelectionEvent = null;

            if (BattleUIManager.Instance.TopMenu != null)
            {
                BattleUIManager.Instance.PopMenu();
            }
        }

        public override void Draw()
        {
            Rectangle rect = new Rectangle(4, 5, 14, 20);
            SpriteRenderer.Instance.Draw(Cursor, Targets[CurSelection].Position + new Vector2(0, -20), rect, Color.White, true, .3f);
        }
    }
}
