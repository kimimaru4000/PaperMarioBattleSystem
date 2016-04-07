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
    /// The standard Goomba enemy
    /// </summary>
    public class Goomba : BattleEnemy
    {
        //TEMPORARY
        private double Timer = 0d;

        public Goomba() : base(new Stats(1, 2, 0, 1, 0))
        {
            Name = "Goomba";

            SpriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Enemies/Goomba");
        }

        public override void OnTurnStart()
        {
            base.OnTurnStart();

            Timer = Time.ActiveMilliseconds + 2000d;
        }

        public override void TurnUpdate()
        {
            if (Time.ActiveMilliseconds >= Timer)
            {
                //End turn
                EndTurn();   
            } 
        }

        public override void Draw()
        {
            Rectangle rect = new Rectangle(67, 107, 26, 28);
            SpriteRenderer.Instance.Draw(SpriteSheet, Position, rect, Color.White, new Vector2(0, 0), false, .1f);
        }
    }
}
