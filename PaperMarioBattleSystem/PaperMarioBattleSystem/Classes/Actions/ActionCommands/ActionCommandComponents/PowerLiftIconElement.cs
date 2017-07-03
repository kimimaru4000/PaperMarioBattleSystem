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
    /// An element corresponding to an icon for the <see cref="PowerLiftCommand"/>.
    /// <para>The element fades in for a duration, stays on screen, then fades out.
    /// If the player selects the icon at all when it is present, it disappears.</para>
    /// </summary>
    //NOTE: In TTYD, the icons are on top of a 3D model of the Gold Star
    //We'll just have the icons only for now
    public sealed class PowerLiftIconElement : IUpdateable, IDrawable, ITintable
    {
        /// <summary>
        /// The states of the icon element.
        /// </summary>
        private enum IconElementStates
        {
            FadeIn,
            Stay,
            FadeOut,
            Done
        }

        public PowerLiftCommand.PowerLiftIcons PowerliftIcon { get; private set; } = PowerLiftCommand.PowerLiftIcons.None;
        private double IconFadeTime = 1000d;
        private double IconStayTime = 2500d;

        /// <summary>
        /// The current state of the icon element. It starts out at FadeIn.
        /// </summary>
        private IconElementStates IconElementState = IconElementStates.FadeIn;

        private Vector2 Position = Vector2.Zero;
        private float Depth = 0f;

        private CroppedTexture2D CroppedTex = null;

        /// <summary>
        /// The elapsed time shared for the FadeIn, Stay, and FadeOut states.
        /// </summary>
        private double ElapsedTime = 0d;

        private Color OrigColor = Color.White;

        /// <summary>
        /// The column the icon is on the grid.
        /// </summary>
        public int Column { get; private set; } = 0;

        /// <summary>
        /// The row the icon is on the grid.
        /// </summary>
        public int Row { get; private set; } = 0;

        public Color TintColor { get; set; } = Color.White;

        public bool IsDone => (IconElementState == IconElementStates.Done);

        public PowerLiftIconElement(PowerLiftCommand.PowerLiftIcons powerLiftIcon, Vector2 position, int column, int row, double iconFadeTime, double iconStayTime, float depth)
        {
            PowerliftIcon = powerLiftIcon;

            Position = position;
            Column = column;
            Row = row;

            IconFadeTime = iconFadeTime;
            IconStayTime = iconStayTime;

            Depth = depth;

            Initialize();
        }

        private void Initialize()
        {
            Texture2D battleGFX = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX");

            switch (PowerliftIcon)
            {
                case PowerLiftCommand.PowerLiftIcons.Poison:
                    CroppedTex = new CroppedTexture2D(battleGFX, new Rectangle(90, 270, 106, 108));
                    break;
                case PowerLiftCommand.PowerLiftIcons.Attack:
                    OrigColor = TintColor = Color.Red;
                    TintColor *= 0f;
                    goto default;
                case PowerLiftCommand.PowerLiftIcons.Defense:
                    OrigColor = TintColor = Color.Blue;
                    TintColor *= 0f;
                    goto default;
                default:
                    //By default select the arrow
                    CroppedTex = new CroppedTexture2D(battleGFX, new Rectangle(5, 353, 50, 61));
                    break;
            }
        }

        public void Update()
        {
            //Perform different things based on the icon state
            switch (IconElementState)
            {
                case IconElementStates.FadeIn:
                    HandleFadeIn();
                    break;
                case IconElementStates.Stay:
                    HandleStay();
                    break;
                case IconElementStates.FadeOut:
                    HandleFadeOut();
                    break;
                default:
                    return;
            }
        }

        private void HandleFadeIn()
        {
            Color startColor = OrigColor * 0f;
            Color endColor = OrigColor;

            //Increment elapsed time and lerp from a fully transparent color to a fully opaque color
            ElapsedTime += Time.ElapsedMilliseconds;
            TintColor = Color.Lerp(startColor, endColor, (float)(ElapsedTime / IconFadeTime));

            //We're done, so go into the Stay state
            if (ElapsedTime >= IconFadeTime)
            {
                TintColor = endColor;

                IconElementState = IconElementStates.Stay;
                ElapsedTime = 0d;
            }
        }

        private void HandleStay()
        {
            //Increment elapsed time
            ElapsedTime += Time.ElapsedMilliseconds;

            //Go to the FadeOut state since we're done
            if (ElapsedTime >= IconStayTime)
            {
                IconElementState = IconElementStates.FadeOut;
                ElapsedTime = 0d;
            }
        }

        private void HandleFadeOut()
        {
            Color startColor = OrigColor;
            Color endColor = OrigColor * 0f;

            //Increment elapsed time and lerp from a fully opaque color to a fully transparent color
            ElapsedTime += Time.ElapsedMilliseconds;
            TintColor = Color.Lerp(startColor, endColor, (float)(ElapsedTime / IconFadeTime));

            //The icon element is completely done, so mark as finished
            if (ElapsedTime >= IconFadeTime)
            {
                TintColor = endColor;

                IconElementState = IconElementStates.Done;
                ElapsedTime = 0d;
            }
        }

        public void Draw()
        {
            SpriteRenderer.Instance.Draw(CroppedTex.Tex, Position, CroppedTex.SourceRect, TintColor, new Vector2(.5f, .5f), false, false, Depth, false);
        }
    }
}
