﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public class SpikedGoomba : Goomba
    {
        public SpikedGoomba()
        {
            Name = "Spiked Goomba";
            BattleStats = new Stats(1, 2, 0, 1, 0);

            EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Spiked);

            Texture2D spriteSheet = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}/Enemies/SpikedGoomba");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(34, 153, 28, 39), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(32, 68, 31, 36), 500d),
                new Animation.Frame(new Rectangle(128, 108, 31, 36), 500d)));
            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(128, 108, 31, 36), 1000d)));
        }
    }
}
