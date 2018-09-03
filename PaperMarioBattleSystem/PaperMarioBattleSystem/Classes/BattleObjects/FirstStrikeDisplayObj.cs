using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaperMarioBattleSystem.Extensions;

namespace PaperMarioBattleSystem
{
    public sealed class FirstStrikeDisplayObj : BattleObject
    {
        private readonly Color PlayerFirstStrikeColor = Color.Red;
        private readonly Color EnemyFirstStrikeColor = Color.Blue;

        private const string PlayerFirstStrikeText = "<clear><shake><color value=\"FFFFFFFF\">You made the First Strike!!</color></shake></clear>";
        private const string EnemyFirstStrikeText = "<clear><shake><color value=\"FFFFFFFF\">You're hit by the First Strike!</color></shake></clear>";

        private NineSlicedTexture2D BGImage = null;
        private DialogueBubble Bubble = null;

        private readonly Vector2 BGSize = new Vector2(380f, 30f);

        private Color BGColor = Color.White;

        private double DisplayTime = 0d;
        private double ElapsedTime = 0d;

        public FirstStrikeDisplayObj(in bool playerFirstStrike, in double displayTime)
        {
            BGImage = new NineSlicedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"),
                new Rectangle(457, 812, 32, 16), 7, 6, 7, 9);
            Bubble = new DialogueBubble();
            Bubble.Position = RenderingGlobals.BaseResolutionHalved - BGSize.HalveInt();
            Bubble.Position -= new Vector2(0f, RenderingGlobals.BaseResolutionHeight / 5);

            string text = string.Empty;
            if (playerFirstStrike == true)
            {
                text = PlayerFirstStrikeText;
                BGColor = PlayerFirstStrikeColor;
            }
            else
            {
                text = EnemyFirstStrikeText;
                BGColor = EnemyFirstStrikeColor;
            }

            Bubble.SetFont(AssetManager.Instance.TTYDFont);
            Bubble.SetText(text);
            Bubble.TimeBetweenCharacters = 0d;

            DisplayTime = displayTime;
        }

        public override void CleanUp()
        {
            base.CleanUp();

            Bubble.CleanUp();
            Bubble = null;

            BGImage = null;
        }

        public override void Update()
        {
            Bubble.Update();

            ElapsedTime += Time.ElapsedMilliseconds;

            //Remove the UI after some time
            if (ElapsedTime >= DisplayTime)
            {
                ReadyForRemoval = true;
            }
        }

        public override void Draw()
        {
            Rectangle bgRect = new Rectangle((int)Bubble.Position.X, (int)Bubble.Position.Y, (int)BGSize.X, (int)BGSize.Y);

            SpriteRenderer.Instance.DrawUISliced(BGImage, bgRect, BGColor, .94f);
            Bubble.DrawText();
        }
    }
}
