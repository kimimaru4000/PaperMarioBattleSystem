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

        /// <summary>
        /// The icon indicating if Mario or his Partner can switch with each other.
        /// </summary>
        private CroppedTexture2D SwitchIcon = null;

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

            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png");

            SwitchIcon = new CroppedTexture2D(battleGFX, new Rectangle(651, 13, 78, 30));

            CroppedTexture2D tacticsButton = new CroppedTexture2D(battleGFX, new Rectangle(146, 844, 24, 24));
            CroppedTexture2D itemsButton = new CroppedTexture2D(battleGFX, new Rectangle(146, 812, 24, 24));

            ActionButtons.Add(new ActionButton("Tactics", tacticsButton, MoveCategories.Tactics, new TacticsSubMenu()));

            ActionSubMenu itemMenu = null;
            if (CheckUseDipMenu() == false)
                itemMenu = new ItemSubMenu(1, 0);
            else itemMenu = new ItemDipSubMenu();

            ActionButtons.Add(new ActionButton("Items", itemsButton, MoveCategories.Item, itemMenu));
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

            SoundManager.Instance.PlaySound(SoundManager.Sound.CommandCursorMove);
        }

        protected override void HandleSelectionInput()
        {
            if (Input.GetKeyDown(Keys.X)) OnBackOut();
            else if (Input.GetKeyDown(Keys.Z)) OnConfirm();
            else if (Input.GetKeyDown(Keys.C))
            {
                BattleEntity otherPlayer = BattleManager.Instance.EntityTurn == BattleManager.Instance.GetFrontPlayer()
                    ? BattleManager.Instance.GetBackPlayer() : BattleManager.Instance.GetFrontPlayer();

                //Don't switch if the back player is dead
                if (CanSwitch() == true)
                {
                    //Switch turns with Mario or the Partner
                    //This updates the front and back player battle indices and their battle positions
                    BattleManager.Instance.SwitchToTurn(PlayerType == PlayerTypes.Partner ? PlayerTypes.Mario : PlayerTypes.Partner);

                    //Decrement the current entity's turns used and end its turn. This keeps that entity's number of turns the same
                    BattleManager.Instance.EntityTurn.SetTurnsUsed(BattleManager.Instance.EntityTurn.TurnsUsed - 1);
                    BattleManager.Instance.TurnEnd();

                    //Queue a Battle Event to swap the current positions of Mario and his Partner
                    //Since we updated the references earlier, their new positions are their own battle positions
                    BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.Stage,
                        new BattleManager.BattleState[] { BattleManager.BattleState.Turn, BattleManager.BattleState.TurnEnd },
                        new SwapPositionBattleEvent(BattleManager.Instance.GetBackPlayer(), BattleManager.Instance.GetFrontPlayer(),
                        BattleManager.Instance.GetBackPlayer().BattlePosition, BattleManager.Instance.GetFrontPlayer().BattlePosition, 500f));
                }
                else
                {
                    Debug.LogError($"{otherPlayer.Name} used all of his/her turns or is dead, so turns cannot be swapped with him/her.");
                }
            }
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

            //If Mario and his Partner can switch with each other, indicate it with the icon
            if (CanSwitch() == true)
            {
                if (SwitchIcon != null && SwitchIcon.Tex != null)
                {
                    Vector2 pos = Camera.Instance.SpriteToUIPos(new Vector2(Position.X - 40f, Position.Y + 70f));
                    SpriteRenderer.Instance.Draw(SwitchIcon.Tex, pos, SwitchIcon.SourceRect, Color.White, false, false, .2f, true);
                }
            }
        }

        private void ArrangeInCircle()
        {
            double spacing = Math.PI / MaxWheelActions;

            for (int i = 0; i < ActionButtons.Count; i++)
            {
                double rotation = (spacing * i) + RotationOffset + GlobalRotOffset;
                Vector2 pos = UtilityGlobals.GetPointAroundCircle(Position, WheelRadius, rotation, false);
                pos.X = (int)pos.X;
                pos.Y = (int)pos.Y;
                ActionButtons[i].Position = pos;
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

        /// <summary>
        /// Tells whether Mario and his Partner can switch places with each other.
        /// </summary>
        /// <returns>true if Mario or his Partner hasn't used up all of his or her turns and isn't dead, otherwise false.</returns>
        private bool CanSwitch()
        {
            BattleEntity otherPlayer = BattleManager.Instance.EntityTurn == BattleManager.Instance.GetFrontPlayer()
                    ? BattleManager.Instance.GetBackPlayer() : BattleManager.Instance.GetFrontPlayer();

            int immobile = otherPlayer.EntityProperties.GetAdditionalProperty<int>(AdditionalProperty.Immobile);

            return (otherPlayer.UsedTurn == false && otherPlayer.IsDead == false && immobile <= 0);
        }
    }
}
