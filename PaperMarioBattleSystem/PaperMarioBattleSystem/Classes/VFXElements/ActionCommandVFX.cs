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
    /// A visual effect for showing a successful Action Command.
    /// </summary>
    public class ActionCommandVFX : VFXElement
    {
        private readonly Dictionary<ActionCommand.CommandRank, CommandRankVFXData> RankVFXData = new Dictionary<ActionCommand.CommandRank, CommandRankVFXData>()
        {
            { ActionCommand.CommandRank.NiceM2, new CommandRankVFXData(Color.Blue, new Rectangle(373, 809, 56, 22), new Vector2(.5f, .5f)) },
            { ActionCommand.CommandRank.NiceM1, new CommandRankVFXData(Color.Blue, new Rectangle(373, 809, 56, 22), new Vector2(.75f, .75f)) },
            { ActionCommand.CommandRank.Nice, new CommandRankVFXData(Color.Blue, new Rectangle(373, 809, 56, 22), Vector2.One) },
            { ActionCommand.CommandRank.Good, new CommandRankVFXData(Color.PeachPuff, new Rectangle(372, 840, 57, 21), Vector2.One) },
            { ActionCommand.CommandRank.Great, new CommandRankVFXData(Color.ForestGreen, new Rectangle(370, 867, 64, 23), Vector2.One) },
            { ActionCommand.CommandRank.Wonderful, new CommandRankVFXData(Color.ForestGreen, new Rectangle(370, 867, 64, 23), Vector2.One) },
            { ActionCommand.CommandRank.Excellent, new CommandRankVFXData(Color.ForestGreen, new Rectangle(370, 867, 64, 23), Vector2.One) }
        };

        /// <summary>
        /// How long the Action Command graphics stay on the screen.
        /// </summary>
        protected const double TimeShown = 1000d;

        protected ActionCommand.CommandRank SuccessRank = ActionCommand.CommandRank.NiceM2;

        protected Vector2 Position = Vector2.Zero;
        protected Vector2 StartPosition = Vector2.Zero;
        protected Vector2 EndPosition = Vector2.Zero;

        protected Vector2 Scale = Vector2.One;

        private double ElapsedTime = 0d;

        public ActionCommandVFX(ActionCommand.CommandRank successRank, Vector2 position)
        {
            SuccessRank = successRank;
            Position = position;

            StartPosition = Position;
            EndPosition = StartPosition + new Vector2(-15f, -15f);

            //Set scale
            if (RankVFXData.ContainsKey(SuccessRank) == true)
                Scale = RankVFXData[SuccessRank].MaxScale;
        }

        public override void Update()
        {
            ElapsedTime += Time.ElapsedMilliseconds;

            //Make it steadily move
            Position = Interpolation.Interpolate(StartPosition, EndPosition, ElapsedTime / TimeShown, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.Linear);

            if (ElapsedTime >= TimeShown)
            {
                ShouldRemove = true;
            }
        }

        public override void Draw()
        {
            //Default to white
            Color color = Color.White;
            Rectangle? sourceRect = new Rectangle(373, 809, 56, 22);

            //Get the color and rectangle from the table
            if (RankVFXData.ContainsKey(SuccessRank) == true)
            {
                color = RankVFXData[SuccessRank].RankColor;
                sourceRect = RankVFXData[SuccessRank].SourceRect;
            }

            SpriteRenderer.Instance.Draw(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                Position, sourceRect, color, 0f, Vector2.Zero, Scale, false, false, .2f);
        }

        /// <summary>
        /// Data regarding the VFX for the command rank.
        /// </summary>
        private struct CommandRankVFXData
        {
            /// <summary>
            /// The color to display the texture corresponding to the command rank.
            /// </summary>
            public Color RankColor;

            /// <summary>
            /// The region of the texture corresponding to the command rank.
            /// </summary>
            public Rectangle? SourceRect;

            /// <summary>
            /// The max scale the command rank VFX can be.
            /// </summary>
            public Vector2 MaxScale;

            public CommandRankVFXData(Color rankColor, Rectangle? sourceRect, Vector2 maxScale)
            {
                RankColor = rankColor;
                SourceRect = sourceRect;
                MaxScale = maxScale;
            }
        }
    }
}
