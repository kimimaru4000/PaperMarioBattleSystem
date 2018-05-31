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
    /// A menu for the MoveActions relating to an ActionMenu. For example, any Jump actions would be in the ActionSubMenu for Jump.
    /// ActionSubMenus can lead to more ActionSubMenus, as is the case with "Change Partner"
    /// This is only used by the player
    /// <para>All ActionSubMenus have the "SubMenu" suffix</para>
    /// </summary>
    public class ActionSubMenu : BattleMenu, INameable
    {
        /// <summary>
        /// The user of this ActionSubMenu.
        /// </summary>
        public BattleEntity User { get; private set; } = null;

        /// <summary>
        /// The name of the SubMenu.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The list of move actions in the submenu
        /// </summary>
        public List<MoveAction> BattleActions { get; protected set; } = new List<MoveAction>();

        /// <summary>
        /// The category of the menu.
        /// </summary>
        public Enumerations.MoveCategories MoveCategory = Enumerations.MoveCategories.None;

        /// <summary>
        /// Tells if the ActionSubMenu should auto-select the first action if it's the only one available.
        /// This is used for Mario's Jump and Hammer moves.
        /// </summary>
        public bool AutoSelectSingle = false;

        /// <summary>
        /// The position of the submenu
        /// </summary>
        protected Vector2 Position = Vector2.Zero;

        /// <summary>
        /// The Y spacing of the action list.
        /// </summary>
        protected float YSpacing = 25f;

        protected float IconXOffset = 32f;
        protected float ResourceCostXOffset = 200f;

        protected Vector2 HeaderSize = new Vector2(100, 32);

        protected NineSlicedTexture2D HeaderImage = null;
        protected NineSlicedTexture2D MenuBG = null;

        protected TextBox BoxMenu = null;
        protected LoopAnimation SelectionCursor = null;

        protected override int LastSelection => BattleActions.Count - 1;

        protected ActionSubMenu(BattleEntity user) : base(MenuTypes.Vertical)
        {
            User = user;

            BoxMenu = new TextBox(new Vector2(SpriteRenderer.Instance.WindowCenter.X, SpriteRenderer.Instance.WindowCenter.Y + 220f), new Vector2(320f, 80f), null);
            BoxMenu.SetText(string.Empty);

            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            Rectangle rect = new Rectangle(743, 59, 15, 12);
            SelectionCursor = new LoopAnimation(battleGFX,
                AnimationGlobals.InfiniteLoop, true,
                new Animation.Frame(rect, 200d),
                new Animation.Frame(rect, 200d, new Vector2(1, 0)));

            HeaderImage = new NineSlicedTexture2D(battleGFX, new Rectangle(457, 812, 32, 16), 7, 6, 7, 9);
            MenuBG = new NineSlicedTexture2D(battleGFX, new Rectangle(485, 846, 16, 16), 8, 8, 8, 8);
        }

        public ActionSubMenu(BattleEntity user, string name, params MoveAction[] battleActions) : this(user)
        {
            Name = name;
            BattleActions.AddRange(battleActions);
        }

        public void Initialize()
        {
            BoxMenu.SetText(BattleActions[0].MoveProperties.Description);

            //The Lucky Star is required to perform Action Commands
            //NOTE: Cache the flag for having the Lucky Star somewhere
            bool hasLuckyStar = (Inventory.Instance.FindItem(LuckyStar.LuckyStarName, true) != null);

            for (int i = 0; i < BattleActions.Count; i++)
            {
                BattleActions[i].SetMoveCategory(MoveCategory);
                BattleActions[i].Initialize();

                //Enable Action Commands for players by default
                if (BattleActions[i].HasActionCommand == true)
                {
                    BattleActions[i].EnableActionCommand = hasLuckyStar;
                }
            }
        }

        protected override void OnSelectionChanged(int newSelection)
        {
            SoundManager.Instance.PlaySound(SoundManager.Sound.CursorMove);
            BoxMenu.SetText(BattleActions[CurSelection].MoveProperties.Description);
        }

        protected override void OnBackOut()
        {
            base.OnBackOut();

            SoundManager.Instance.PlaySound(SoundManager.Sound.MenuBackOut);
            BattleUIManager.Instance.PopMenu();
        }

        protected override void OnConfirm()
        {
            base.OnConfirm();

            if (BattleActions[CurSelection].Disabled == false)
            {
                BattleActions[CurSelection].OnMenuSelected();

                SoundManager.Instance.PlaySound(SoundManager.Sound.MenuSelect);
            }
            else
            {
                //Show the dialog here that the move can't be selected and state why
                string disabledString = BattleActions[CurSelection].DisabledString;

                User.BManager.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Message,
                    new BattleManager.BattleState[] { BattleManager.BattleState.Turn, BattleManager.BattleState.TurnEnd },
                    new MessageBattleEvent(disabledString, MessageBattleEvent.DefaultWaitDuration));

                Debug.LogError($"{BattleActions[CurSelection].Name} is disabled: {disabledString}");
            }
        }

        public override void Update()
        {
            base.Update();

            SelectionCursor.Update();
        }

        public override void Draw()
        {
            //List out actions with their name, icon, description, and FP cost
            for (int i = 0; i < BattleActions.Count; i++)
            {
                MoveAction moveAction = BattleActions[i];

                float alphaMod = 1f;

                Vector2 pos = Position + new Vector2(0, i * YSpacing);
                Color color = moveAction.Disabled == false ? MoveAction.EnabledColor : MoveAction.DisabledColor;
                Color textColor = moveAction.Disabled == false ? MoveAction.TextEnabledColor : MoveAction.TextDisabledColor;
                if (CurSelection != i || BattleUIManager.Instance.TopMenu != this) alphaMod = MoveAction.UnselectedAlpha;

                //Draw all information including name and FP cost
                DrawMoveActionInfo(moveAction, pos, color, textColor, alphaMod);
            }

            //Show description window at the bottom
            BoxMenu.Draw();
            
            //Draw the selection cursor
            SelectionCursor.Draw(Position + new Vector2(-(IconXOffset * 2), CurSelection * YSpacing), Color.White, 0f, Vector2.Zero, new Vector2(2f, 2f), false, .38f);

            //Draw the menu background
            SpriteRenderer.Instance.DrawUISliced(MenuBG, new Rectangle((int)(Position.X - IconXOffset) - 6, (int)(Position.Y - (YSpacing / 2)), (int)(ResourceCostXOffset + (IconXOffset * 3)), (int)((BattleActions.Count * YSpacing) + (YSpacing))),
                Color.White, .35f);

            //Draw the header
            Vector2 headerOffset = new Vector2(HeaderSize.X / 2, HeaderSize.Y / 2);
            Vector2 headerPos = new Vector2((int)(Position.X + (ResourceCostXOffset / 2f)), (int)(Position.Y - YSpacing) + (YSpacing / 4)) - headerOffset;
            Rectangle headerRect = new Rectangle((int)headerPos.X, (int)headerPos.Y, (int)HeaderSize.X, (int)HeaderSize.Y);
            SpriteRenderer.Instance.DrawUISliced(HeaderImage, headerRect, Color.Blue, .42f);

            Vector2 fontSize = AssetManager.Instance.TTYDFont.MeasureString(Name);
            Vector2 diff = new Vector2((HeaderSize.X - fontSize.X) / 2, ((HeaderSize.Y - fontSize.Y) / 2) + (fontSize.Y / 4));
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, Name, headerPos + diff, Color.White, 0f, Vector2.Zero, 1f, .43f);
        }

        private void DrawMoveActionInfo(in MoveAction moveAction, in Vector2 position, in Color color, in Color textColor, in float alphaMod)
        {
            //Draw icon
            CroppedTexture2D moveIcon = moveAction.MoveProperties.Icon;
            if (moveIcon != null && moveIcon.Tex != null)
            {
                SpriteRenderer.Instance.DrawUI(moveIcon.Tex, position - new Vector2(IconXOffset, 0), moveIcon.SourceRect, color * alphaMod, false, false, .39f);
            }

            //Draw name
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, moveAction.Name, position, textColor * alphaMod, 0f, Vector2.Zero, 1f, .4f);

            //Show cost if the move costs anything
            if ((moveAction.CostsFP == true || moveAction.CostsSP == true) && moveAction.MoveProperties.CostDisplayType != Enumerations.CostDisplayTypes.Hidden)
            {
                Color fpColor = textColor;

                //If the resource cost was lowered, show it a bluish-gray color (This feature is from PM)
                //Keep it gray if the move is disabled for any reason
                if (moveAction.Disabled == false && moveAction.MoveProperties.CostDisplayType == Enumerations.CostDisplayTypes.Special)
                {
                    Color blueGray = MoveAction.SpecialCaseColor;
                    fpColor = blueGray;
                }

                string costString = null;

                //Display the resource cost
                if (moveAction.MoveProperties.ResourceType == Enumerations.MoveResourceTypes.FP)
                {
                    costString = $"{moveAction.MoveProperties.ResourceCost} FP";
                }
                else
                {
                    costString = $"{moveAction.MoveProperties.ResourceCost / StarPowerGlobals.SPUPerStarPower} SP";
                }

                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, costString, position + new Vector2(ResourceCostXOffset, 0), fpColor * alphaMod, 0f, Vector2.Zero, 1f, .4f);
            }
        }
    }
}
