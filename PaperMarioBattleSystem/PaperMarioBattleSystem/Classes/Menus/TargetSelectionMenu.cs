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
    /// The target selection menu when performing an action
    /// </summary>
    public class TargetSelectionMenu : BattleMenu
    {
        public enum EntitySelectionType
        {
            Single, All, First
        }

        private ReverseAnimation Cursor = null;
        private NineSlicedTexture2D NameImage = null;

        private BattleEntity[] Targets = null;

        public delegate void OnSelection(params BattleEntity[] targets);
        public OnSelection Selection = null;

        protected EntitySelectionType SelectionType = EntitySelectionType.Single;
        protected override int LastSelection => Targets.Length - 1;

        public TargetSelectionMenu() : base(MenuTypes.Horizontal)
        {
            Texture2D cursorSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Cursor.png");

            Cursor = new ReverseAnimation(cursorSheet, AnimationGlobals.InfiniteLoop, true,
                new Animation.Frame(new Rectangle(4, 5, 14, 20), 110d),
                new Animation.Frame(new Rectangle(26, 5, 16, 20), 110d),
                new Animation.Frame(new Rectangle(49, 5, 20, 20), 110d),
                new Animation.Frame(new Rectangle(75, 5, 22, 16), 110d),
                new Animation.Frame(new Rectangle(105, 5, 21, 16), 110d));
            NameImage = new NineSlicedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"),
                new Rectangle(457, 812, 32, 16), 7, 6, 7, 9);

            //Both games wrap the cursor
            WrapCursor = true;
        }

        public void StartSelection(OnSelection onSelection, EntitySelectionType selectionType, params BattleEntity[] targets)
        {
            Targets = targets;
            Selection = onSelection;
            SelectionType = selectionType;

            //Failsafe - end selection immediately if inputs are invalid
            if (Targets == null || Targets.Length == 0)
            {
                Debug.LogError($"Null or empty target array in {nameof(TargetSelectionMenu)}.{nameof(StartSelection)}, ending selection...");
                EndSelection();
            }
        }

        public void StartSelection(OnSelection onSelection, EntitySelectionType selectionType, int startIndex, params BattleEntity[] targets)
        {
            StartSelection(onSelection, selectionType, targets);

            //Works with the failsafe in the other overload
            if (Targets != null)
            {
                ChangeSelection(startIndex);
            }
        }

        protected override void HandleCursorInput()
        {
            if (SelectionType == EntitySelectionType.First || SelectionType == EntitySelectionType.All) return;

            base.HandleCursorInput();
        }

        protected override void OnSelectionChanged(int newSelection)
        {
            base.OnSelectionChanged(newSelection);

            SoundManager.Instance.PlaySound(SoundManager.Sound.CursorMove);
        }

        protected override void OnBackOut()
        {
            SoundManager.Instance.PlaySound(SoundManager.Sound.MenuBackOut);
            EndSelection();
        }

        protected override void OnConfirm()
        {
            if (SelectionType == EntitySelectionType.Single || SelectionType == EntitySelectionType.First)
            {
                Selection?.Invoke(Targets[CurSelection]);
            }
            else if (SelectionType == EntitySelectionType.All)
            {
                Selection?.Invoke(Targets);
            }

            SoundManager.Instance.PlaySound(SoundManager.Sound.MenuSelect);

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

            Cursor.Reset();
        }

        public override void Update()
        {
            Cursor.Update();

            base.Update();
        }

        public override void Draw()
        {
            string name = "All";

            if (SelectionType == EntitySelectionType.Single || SelectionType == EntitySelectionType.First)
            {
                DrawSingle();
                name = Targets[CurSelection].Name;
            }
            else if (SelectionType == EntitySelectionType.All)
            {
                DrawAll();
            }

            //Show who is being selected in the header
            Vector2 location = new Vector2(170, 220);
            Vector2 size = AssetManager.Instance.TTYDFont.MeasureString(name);
            Vector2 buffer = new Vector2(5f, 5f);
            SpriteRenderer.Instance.DrawUISliced(NameImage, new Rectangle((int)location.X, (int)location.Y, (int)size.X + ((int)buffer.X * 2), (int)(size.Y / 1.5f) + ((int)buffer.Y * 2)), Color.Blue, .31f);
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, name, location + buffer, Color.White, 0f, Vector2.Zero, 1f, .32f);
        }

        private void DrawAll()
        {
            for (int i = 0; i < Targets.Length; i++)
            {
                DrawAtTarget(Targets[i]);
            }
        }

        private void DrawSingle()
        {
            DrawAtTarget(Targets[CurSelection]);
        }

        private void DrawAtTarget(BattleEntity target)
        {
            //Draw the cursor slightly above the target
            Vector2 pos = Camera.Instance.SpriteToUIPos(target.GetDrawnPosAbove(new Vector2(0, -10)));

            Cursor.Draw(pos, Color.White, 0f, new Vector2(0f, 1f), Vector2.One, false, .3f);
        }
    }
}
