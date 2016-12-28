using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The menu shown when attempting to cancel Double or Triple Dip.
    /// <para>Two options are presented: Yes and No.
    /// Choosing Yes ends the entity's turn and clears the menu stack.
    /// Choosing No goes back to the Item menu.</para>
    /// </summary>
    public sealed class CancelDipMenu : BattleMenu
    {
        protected override int LastSelection
        {
            get { return MenuOptions.Count - 1; }
        }

        private Vector2 Position = new Vector2(230, 150);

        private List<BattleMenuOption> MenuOptions = new List<BattleMenuOption>(); 

        public CancelDipMenu() : base(MenuTypes.Vertical)
        {
            MenuOptions.Add(new BattleMenuOption("Yes", OnChooseYes));
            MenuOptions.Add(new BattleMenuOption("No", OnChooseNo));
        }

        protected override void OnConfirm()
        {
            MenuOptions[CurSelection].OnSelectOption?.Invoke();
        }

        protected override void OnBackOut()
        {
            OnChooseNo();
        }

        private void OnChooseYes()
        {
            BattleUIManager.Instance.ClearMenuStack();

            //Remove any remaining item turns from the BattleEntity and end its turn if it no longer wants to use items
            BattleManager.Instance.EntityTurn.EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.DipItemTurns);
            BattleManager.Instance.EntityTurn.EndTurn();
        }

        private void OnChooseNo()
        {
            //This menu would be at the top, so pop it
            BattleUIManager.Instance.PopMenu();
        }

        public override void Draw()
        {
            //Draw text
            string header = "Don't use an item?";
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, header, Position, Color.White, .7f);

            for (int i = 0; i < MenuOptions.Count; i++)
            {
                float alphaMod = 1f;
                Vector2 pos = Position + new Vector2(75, (i + 2) * 20);

                if (CurSelection != i || BattleUIManager.Instance.TopMenu != this) alphaMod *= .7f;

                SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, MenuOptions[i].Option, pos, Color.White * alphaMod, .7f);
            }
        }
    }
}
