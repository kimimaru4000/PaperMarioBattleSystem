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
    /// Press A when the cursor is near the middle of the big circle.
    /// </summary>
    public sealed class TattleCommand : ActionCommand
    {
        /*I'm making this based on position instead of time as I feel this is linear enough where neither implementation would make a difference
         If for some reason this doesn't work out, feel free to make it timing dependent*/

        private const int BigCursorSize = 46;
        private const int SmallCursorStartOffset = 150;

        private CroppedTexture2D BigCursor = null;
        private CroppedTexture2D SmallCursor = null;

        private Vector2 BigCursorPos = Vector2.Zero;
        private Vector2 SmallCursorPos = Vector2.Zero;

        private float SmallCursorSpeed = 2f;
        private float ElapsedTime = 0f;

        private Keys InputButton = Keys.Z;

        private Rectangle SuccessRect = new Rectangle(0, 0, BigCursorSize, BigCursorSize);

        private bool WithinRange => (SuccessRect.Contains(SmallCursorPos));
        private bool PastRange => (SmallCursorPos.X > (SuccessRect.Right + SuccessRect.Width));

        public TattleCommand(IActionCommandHandler commandHandler, float smallCursorSpeed) : base(commandHandler)
        {
            Texture2D battleGFX = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX");
            
            BigCursor = new CroppedTexture2D(battleGFX, new Rectangle(14, 273, 46, 46));
            SmallCursor = new CroppedTexture2D(battleGFX, new Rectangle(10, 330, 13, 12));

            SmallCursorSpeed = smallCursorSpeed;

            //Description = "Line up the small cursor with\n the center of the big cursor!"
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            if (values == null || values.Length == 0)
            {
                Debug.LogError($"{nameof(TattleCommand)} requires the position of the BattleEntity targeted to place the cursor in the correct spot!");
                return;
            }

            BigCursorPos = Camera.Instance.SpriteToUIPos((Vector2)values[0]);
            SmallCursorPos = BigCursorPos - new Vector2(SmallCursorStartOffset, 0);

            SuccessRect = new Rectangle((int)BigCursorPos.X - (BigCursorSize / 2), (int)BigCursorPos.Y - (BigCursorSize / 2), BigCursorSize, BigCursorSize);
        }

        public override void EndInput()
        {
            base.EndInput();
        }

        protected override void ReadInput()
        {
            //Failed if no input was pressed on time
            if (PastRange == true)
            {
                OnComplete(CommandResults.Failure);
                return;
            }

            //Check if the player pressed the input button at the right time
            if (Input.GetKeyDown(InputButton) == true)
            {
                //Out of range, so a failure
                if (WithinRange == false)
                {
                    OnComplete(CommandResults.Failure);
                    return;
                }
                //In range, so success
                else
                {
                    OnComplete(CommandResults.Success);
                    return;
                }
            }

            SmallCursorPos.X += SmallCursorSpeed;
            ElapsedTime += (float)Time.ElapsedMilliseconds;
        }

        protected override void OnDraw()
        {
            //The cursor is drawn on top of the entity being targeted
            //Only 1/4 of the full cursor is stored as a texture, so we can just draw 3 more versions flipped differently

            string text = "NO!";
            Color color = Color.Red;
            if (WithinRange == true)
            {
                text = "OKAY!";
                color = Color.Green;
            }

            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, text, new Vector2(300, 100), color, .7f);

            //Handle rotation
            float rotation = -ElapsedTime * UtilityGlobals.ToRadians(.1f);

            DrawBigCursor(rotation);
            DrawSmallCursor(rotation);

            //Show success rectangle (comment out if not debugging)
            //Texture2D DebugBoxTex = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Box");
            //SpriteRenderer.Instance.Draw(DebugBoxTex, new Vector2(SuccessRect.X, SuccessRect.Y), null, Color.Red, 0f, Vector2.Zero, 
            //    new Vector2(SuccessRect.Width, SuccessRect.Height), false, false, .21f, true);
        }

        private void DrawBigCursor(float rotation)
        {
            Vector2 bigOrigin = new Vector2((float)BigCursor.SourceRect.Value.Width, (float)BigCursor.SourceRect.Value.Height);

            //origins are offset instead of position so each piece rotates from the center of the overall big circle they create
            //May need adjusting
            SpriteRenderer.Instance.Draw(BigCursor.Tex, BigCursorPos, BigCursor.SourceRect, Color.White, rotation, bigOrigin, 1f, false, false, .2f, true);
            SpriteRenderer.Instance.Draw(BigCursor.Tex, BigCursorPos, BigCursor.SourceRect, Color.White, rotation, new Vector2(-bigOrigin.X, bigOrigin.Y), 1f, true, false, .2f, true);
            SpriteRenderer.Instance.Draw(BigCursor.Tex, BigCursorPos, BigCursor.SourceRect, Color.White, rotation, new Vector2(bigOrigin.X, 0), 1f, false, true, .2f, true);
            SpriteRenderer.Instance.Draw(BigCursor.Tex, BigCursorPos, BigCursor.SourceRect, Color.White, rotation, -bigOrigin, 1f, true, true, .2f, true);

            //Draw the middle cursor indicating the small cursor is near
            //It gets smaller the closer the small cursor is to the center
            if (WithinRange == true)
            {
                //Cap the scale so it can be seen clearly at all times
                const float maxScale = .8f;

                //Get the absolute value of the distance from the cursor to the center
                //Divide by half the SuccessRect's width since we're scaling based on how close it is to the center
                float diff = Math.Abs(SmallCursorPos.X - SuccessRect.Center.X) / (SuccessRect.Width / 2f);
                float scale = UtilityGlobals.Clamp(diff, 0f, maxScale);

                //Draw the middle cursor
                SpriteRenderer.Instance.Draw(BigCursor.Tex, BigCursorPos, BigCursor.SourceRect, Color.White, rotation, bigOrigin, scale, false, false, .2f, true);
                SpriteRenderer.Instance.Draw(BigCursor.Tex, BigCursorPos, BigCursor.SourceRect, Color.White, rotation, new Vector2(-bigOrigin.X, bigOrigin.Y), scale, true, false, .2f, true);
                SpriteRenderer.Instance.Draw(BigCursor.Tex, BigCursorPos, BigCursor.SourceRect, Color.White, rotation, new Vector2(bigOrigin.X, 0), scale, false, true, .2f, true);
                SpriteRenderer.Instance.Draw(BigCursor.Tex, BigCursorPos, BigCursor.SourceRect, Color.White, rotation, -bigOrigin, scale, true, true, .2f, true);
            }
        }

        private void DrawSmallCursor(float rotation)
        {
            Vector2 smallOrigin = new Vector2(SmallCursor.SourceRect.Value.Width, SmallCursor.SourceRect.Value.Height);

            SpriteRenderer.Instance.Draw(SmallCursor.Tex, SmallCursorPos, SmallCursor.SourceRect, Color.White, smallOrigin, false, false, .25f, true);
            SpriteRenderer.Instance.Draw(SmallCursor.Tex, SmallCursorPos + new Vector2(smallOrigin.X, 0), SmallCursor.SourceRect, Color.White, smallOrigin, true, false, .25f, true);
            SpriteRenderer.Instance.Draw(SmallCursor.Tex, SmallCursorPos + new Vector2(0, smallOrigin.Y), SmallCursor.SourceRect, Color.White, smallOrigin, false, true, .25f, true);
            SpriteRenderer.Instance.Draw(SmallCursor.Tex, SmallCursorPos + new Vector2(smallOrigin.X, smallOrigin.Y), SmallCursor.SourceRect, Color.White, smallOrigin, true, true, .25f, true);
        }
    }
}
