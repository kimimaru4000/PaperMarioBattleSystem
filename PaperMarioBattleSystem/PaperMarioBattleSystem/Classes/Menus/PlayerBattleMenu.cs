using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base BattleMenu for both Mario and his Partners.
    /// <para>They share Strategies/Tactics and Item commands.</para>
    /// </summary>
    public abstract class PlayerBattleMenu : BattleMenu
    {
        protected override int LastSelection
        {
            get
            {
                return ActionButtons.Count - 1;
            }
        }

        protected List<ActionButton> ActionButtons = new List<ActionButton>();

        protected PlayerTypes PlayerType { get; private set; } = PlayerTypes.Mario;

        protected PlayerBattleMenu(PlayerTypes playerType) : base(MenuTypes.Horizontal)
        {
            PlayerType = playerType;

            ActionButtons.Add(new ActionButton("Tactics", AssetManager.Instance.LoadAsset<Texture2D>("UI/Battle/JumpButton"),
                new Vector2(-270, 50), new JumpSubMenu()));
            ActionButtons.Add(new ActionButton("Items", AssetManager.Instance.LoadAsset<Texture2D>("UI/Battle/JumpButton"),
                new Vector2(-220, 50), new HammerSubMenu()));
        }

        protected override void HandleSelectionInput()
        {
            if (Input.GetKeyDown(Keys.X)) OnBackOut();
            else if (Input.GetKeyDown(Keys.Z)) OnConfirm();
            else if (Input.GetKeyDown(Keys.C))
                BattleManager.Instance.SwitchToTurn(PlayerType == PlayerTypes.Partner ? PlayerTypes.Mario : PlayerTypes.Partner, true);
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
