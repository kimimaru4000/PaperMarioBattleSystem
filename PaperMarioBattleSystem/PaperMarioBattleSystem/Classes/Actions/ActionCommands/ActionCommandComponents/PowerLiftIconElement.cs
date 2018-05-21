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
    public sealed class PowerLiftIconElement : IUpdateable, ITintable, IScalable
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

        public float Depth = 0f;

        private Color OrigColor = Color.White;

        public Color TintColor { get; set; } = Color.White;

        public Vector2 Scale { get; set; } = Vector2.One;

        /// <summary>
        /// The elapsed time shared for the FadeIn, Stay, and FadeOut states.
        /// </summary>
        private double ElapsedTime = 0d;

        public bool IsDone => (IconElementState == IconElementStates.Done);

        public PowerLiftIconElement(PowerLiftCommand.PowerLiftIcons powerLiftIcon, double iconFadeTime, double iconStayTime, float depth)
        {
            PowerliftIcon = powerLiftIcon;

            IconFadeTime = iconFadeTime;
            IconStayTime = iconStayTime;

            Depth = depth;

            Initialize();
        }

        private void Initialize()
        {
            switch (PowerliftIcon)
            {
                case PowerLiftCommand.PowerLiftIcons.Poison:
                    TintColor *= 0f;
                    Scale = new Vector2(.7f);
                    break;
                case PowerLiftCommand.PowerLiftIcons.Attack:
                    OrigColor = TintColor = Color.Red;
                    TintColor *= 0f;
                    break;
                case PowerLiftCommand.PowerLiftIcons.Defense:
                    OrigColor = TintColor = Color.Blue;
                    TintColor *= 0f;
                    break;
                default:
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
    }
}
