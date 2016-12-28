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
    /// The base BattleMenu for both Mario and his Partners. It displays all the available actions in a wheel like in the first Paper Mario.
    /// <para>They share Strategies/Tactics and Item commands.</para>
    /// </summary>
    public abstract class PlayerBattleMenu : BattleMenu
    {
        /// <summary>
        /// The radius of the wheel.
        /// </summary>
        private const float WheelRadius = 100f;

        /// <summary>
        /// A constant to determine how the actions are spaced out on the wheel.
        /// </summary>
        private const int MaxWheelActions = 4;

        /// <summary>
        /// The global rotation offset of the menu.
        /// This is used to change the rotation of the wheel so the current selection is in a more desirable location.
        /// </summary>
        private const double GlobalRotOffset = -(Math.PI / MaxWheelActions);

        protected override int LastSelection
        {
            get
            {
                return ActionButtons.Count - 1;
            }
        }

        protected List<ActionButton> ActionButtons = new List<ActionButton>();

        protected PlayerTypes PlayerType { get; private set; } = PlayerTypes.Mario;

        /// <summary>
        /// The position of the menu.
        /// This is set to the current entity's battle position.
        /// </summary>
        private Vector2 Position = Vector2.Zero;

        /// <summary>
        /// The current rotation of the menu wheel. This changes when changing selections.
        /// </summary>
        private double RotationOffset = 0d;

        protected PlayerBattleMenu(PlayerTypes playerType) : base(MenuTypes.Horizontal)
        {
            PlayerType = playerType;
            
            ActionButtons.Add(new ActionButton("Tactics", AssetManager.Instance.LoadAsset<Texture2D>("UI/Battle/JumpButton"),
                MoveCategories.Tactics, new TacticsSubMenu()));

            ActionSubMenu itemMenu = null;
            if (CheckUseDipMenu() == false)
                itemMenu = new ItemSubMenu(1, 0);
            else itemMenu = new ItemDipSubMenu();

            ActionButtons.Add(new ActionButton("Items", AssetManager.Instance.LoadAsset<Texture2D>("UI/Battle/JumpButton"),
                MoveCategories.Item, itemMenu));
        }

        /// <summary>
        /// Initializes the menu, setting its position and starting on a particular menu item.
        /// </summary>
        /// <param name="startingSelection">The menu item to start the menu selected on.</param>
        protected void Initialize(int startingSelection)
        {
            Position = BattleManager.Instance.EntityTurn.BattlePosition;

            ChangeSelection(startingSelection);

            for (int i = 0; i < ActionButtons.Count; i++)
            {
                ActionButtons[i].Initialize();
            }
        }

        protected override void OnSelectionChanged(int newSelection)
        {
            double spacing = Math.PI / MaxWheelActions;
            RotationOffset = newSelection * -spacing;
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
            ArrangeInCircle();
            for (int i = 0; i < ActionButtons.Count; i++)
            {
                ActionButtons[i].Draw(CurSelection == i);
            }
        }

        private void ArrangeInCircle()
        {
            double spacing = Math.PI / MaxWheelActions;

            for (int i = 0; i < ActionButtons.Count; i++)
            {
                double rotation = (spacing * i) + RotationOffset + GlobalRotOffset;
                ActionButtons[i].Position = Position + new Vector2((int)(WheelRadius * (float)Math.Cos(rotation)), (int)(WheelRadius * (float)Math.Sin(rotation)));
            }
        }

        /// <summary>
        /// Tells whether to add the alternate Item menu containing the Double Dip and/or Triple Dip options.
        /// </summary>
        /// <returns>true if the BattleEntity using this menu has at least one Double Dip or Triple Dip Badge equipped.</returns>
        private bool CheckUseDipMenu()
        {
            int doubleDipCount = BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.DoubleDip);
            int tripleDipCount = BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.TripleDip);

            return (doubleDipCount > 0 || tripleDipCount > 0);
        }
    }
}
