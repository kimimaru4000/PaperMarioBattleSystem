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
    /// The base class for the two different types of Star Power found in the Paper Mario games (Star Spirits, Crystal Stars).
    /// </summary>
    public abstract class StarPowerBase
    {
        //Each usable Star Power circle/bar is represented as 100.
        //So in PM, a full bar of 1 Star Power is 100 and in TTYD one filled circle is 100

        public float StarPowerLevel { get; protected set; } = 0;
        public float MaxStarPowerLevel { get; protected set; } = 700f;
    }
}
