using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public sealed class BombSquadActionCommandUI : ActionCommandUI<BombSquadCommand>
    {
        private UIFourPiecedTex Cursor = null;
        private UIFourPiecedTex ThrownCursor = null;

        private double ElapsedTime = 0d;

        public BombSquadActionCommandUI(BombSquadCommand bombSquadCommand) : base(bombSquadCommand)
        {
            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png");

            CroppedTexture2D croppedTex2D = new CroppedTexture2D(battleGFX, new Rectangle(14, 273, 46, 46));

            Cursor = new UIFourPiecedTex(croppedTex2D, croppedTex2D.WidthHeightToVector2(), .5f, Color.White);
        }

        public override void Update()
        {
            ElapsedTime += Time.ElapsedMilliseconds;

            Cursor.Position = ActionCmd.CursorPosition;
            Cursor.Rotation = ActionCmd.CursorRotation;

            //Remove the thrown cursor if enough time passed
            if (ThrownCursor != null && ElapsedTime >= ActionCmd.ThrownCursorTime)
            {
                ThrownCursor = null;
            }
            else if (ThrownCursor == null && ElapsedTime < ActionCmd.ThrownCursorTime)
            {
                //Create the cursor
                ThrownCursor = Cursor.Copy();
                ThrownCursor.Depth -= .01f;
                ThrownCursor.TintColor = Color.Gray;
            }
        }

        public override void Draw()
        {
            if (ActionCmd?.AcceptingInput == false) return;

            if (ActionCmd.AllBombsThrown == false)
            {
                Cursor.Draw();
            }

            ThrownCursor?.Draw();
        }
    }
}
