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
        public enum EntitySelectionType
        {
            Single, /*FirstGround, Ground, Air,*/ All
        }

        private Texture2D Cursor = null;

        private BattleEntity[] Targets = null;

        public delegate void OnSelection(params BattleEntity[] targets);
        public OnSelection Selection = null;

        protected EntitySelectionType SelectionType = EntitySelectionType.Single;
        protected override int LastSelection => Targets.Length - 1;

        public TargetSelectionMenu() : base(MenuTypes.Horizontal)
        {
            Cursor = AssetManager.Instance.LoadAsset<Texture2D>("UI/Cursor");
        }

        public void StartSelection(OnSelection onSelection, EntitySelectionType selectionType, params BattleEntity[] targets)
        {
            Targets = targets;
            Selection = onSelection;
            SelectionType = selectionType;
        }

        protected override void HandleCursorInput()
        {
            if (SelectionType == EntitySelectionType.All) return;

            base.HandleCursorInput();
        }

        protected override void OnBackOut()
        {
            EndSelection();
        }

        protected override void OnConfirm()
        {
            if (SelectionType == EntitySelectionType.Single)
            {
                Selection?.Invoke(Targets[CurSelection]);
            }
            else if (SelectionType == EntitySelectionType.All)
            {
                Selection?.Invoke(Targets);
            }

            EndSelection();
        }

        public void EndSelection()
        {
            CurSelection = 0;
            Targets = null;
            Selection = null;
            SelectionType = EntitySelectionType.Single;

            if (BattleUIManager.Instance.TopMenu != null)
            {
                BattleUIManager.Instance.PopMenu();
            }
        }

        public override void Draw()
        {
            if (SelectionType == EntitySelectionType.Single)
            {
                DrawSingle();
            }
            else if (SelectionType == EntitySelectionType.All)
            {
                DrawAll();
            }
        }

        private void DrawAll()
        {
            Rectangle rect = new Rectangle(4, 5, 14, 20);
            for (int i = 0; i < Targets.Length; i++)
            {
                Vector2 pos = Camera.Instance.SpriteToUIPos(Targets[i].Position + new Vector2(0, -20));
                SpriteRenderer.Instance.Draw(Cursor, pos, rect, Color.White, true, .3f, true);
            }
        }

        private void DrawSingle()
        {
            Rectangle rect = new Rectangle(4, 5, 14, 20);
            Vector2 pos = Camera.Instance.SpriteToUIPos(Targets[CurSelection].Position + new Vector2(0, -20));
            SpriteRenderer.Instance.Draw(Cursor, pos, rect, Color.White, true, .3f, true);
        }
    }
}
