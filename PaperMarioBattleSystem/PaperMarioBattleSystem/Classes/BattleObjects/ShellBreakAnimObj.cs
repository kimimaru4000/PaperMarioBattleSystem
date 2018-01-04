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
    /// The animation for the Shell Shield Shell breaking.
    /// </summary>
    public sealed class ShellBreakAnimObj : BattleObject, IPosition, ITintable, IRotatable
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Color TintColor { get; set; } = Color.White;
        public float Rotation { get; set; } = 0f;

        private double AnimDuration = 1000d;
        private float MaxRotation = 30f;
        private double RotationTime = 500d;

        private CroppedTexture2D ShellLeftHalf = null;
        private CroppedTexture2D ShellRightHalf = null;

        private double ElapsedTime = 0d;
        private double ElapsedRotationTime = 0d;

        private Vector2 LeftHalfPosOffset = Vector2.Zero;
        private Vector2 RightHalfPosOffset = Vector2.Zero;

        public ShellBreakAnimObj(Vector2 position, double animDuration, float maxRotationVal, double rotationTime)
        {
            Position = position;
            AnimDuration = animDuration;
            MaxRotation = maxRotationVal;
            RotationTime = rotationTime;

            //Load the halves
            Texture2D shellSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Neutral/ShellShieldShell.png");

            ShellLeftHalf = new CroppedTexture2D(shellSheet, new Rectangle(7, 298, 108, 129));
            ShellRightHalf = new CroppedTexture2D(shellSheet, new Rectangle(117, 298, 94, 128));

            //Calculate the position offsets for the shell halves
            //They're not the same size, with the left half being larger than the right half
            Vector2 leftHalfSize = ShellLeftHalf.WidthHeightToVector2();
            LeftHalfPosOffset = new Vector2(0f, leftHalfSize.Y / 2);

            RightHalfPosOffset = new Vector2(leftHalfSize.X - (Math.Abs(ShellRightHalf.SourceRect.Value.Width - leftHalfSize.X)), LeftHalfPosOffset.Y);
        }

        public override void CleanUp()
        {
            ShellLeftHalf = null;
            ShellRightHalf = null;

            LeftHalfPosOffset = RightHalfPosOffset = Vector2.Zero;
        }

        public override void Update()
        {
            ElapsedTime += Time.ElapsedMilliseconds;

            if (ElapsedRotationTime < RotationTime)
            {
                ElapsedRotationTime += Time.ElapsedMilliseconds;
                if (ElapsedRotationTime >= RotationTime)
                {
                    //Start the blinking with the remaining animation time
                    BattleObjManager.Instance.AddBattleObject(new BlinkVFX(this, 0f, 34d, AnimDuration - ElapsedTime));

                    ElapsedRotationTime = RotationTime;
                }

                Rotation = UtilityGlobals.Lerp(0f, MaxRotation, (float)(ElapsedRotationTime / RotationTime));
            }

            //Mark for removal once the animation is complete
            if (ElapsedTime >= AnimDuration)
            {
                ReadyForRemoval = true;
            }
        }

        public override void Draw()
        {
            //Render the shell halves
            //Set their origins to their respective ends so the rotation acts like it does in TTYD
            SpriteRenderer.Instance.Draw(ShellLeftHalf.Tex, Position + LeftHalfPosOffset, ShellLeftHalf.SourceRect, TintColor, -Rotation, new Vector2(0f, 1f), new Vector2(.5f, .5f), false, false, .8f);
            SpriteRenderer.Instance.Draw(ShellRightHalf.Tex, Position + RightHalfPosOffset, ShellRightHalf.SourceRect, TintColor, Rotation, new Vector2(1f, 1f), new Vector2(.5f, .5f), false, false, .8f);
        }
    }
}
