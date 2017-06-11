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
    /// Move the cursor across the 3x3 grid and select the red and blue arrows to boost your stats.
    /// Hitting a Poison Mushroom halves the cursor's speed for a short time.
    /// </summary>
    public sealed class PowerLiftCommand : ActionCommand
    {
        private double CommandTime = 30000d;
        private int CursorSpeed = 3;

        private float PoisonFactor = .5f;
        private double PoisonDur = 2000d;

        private CroppedTexture2D BigCursor = null;
        private CroppedTexture2D SmallCursor = null;

        public PowerLiftCommand(IActionCommandHandler commandHandler, double commandTime) : base(commandHandler)
        {
            Texture2D battleGFX = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX");

            BigCursor = new CroppedTexture2D(battleGFX, new Rectangle(14, 273, 46, 46));
            SmallCursor = new CroppedTexture2D(battleGFX, new Rectangle(10, 330, 13, 12));

            CommandTime = commandTime;
        }

        protected override void ReadInput()
        {
            
        }
    }
}
