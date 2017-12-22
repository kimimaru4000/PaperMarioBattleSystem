using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Press the jump button once before Mario almost hits the enemy
    /// </summary>
    public class JumpCommand : ActionCommand
    {
        protected float Leniency = 500f;
        protected float StartRange = 0f;
        protected float EndRange = 0f;
        protected float TotalRange = 0f;

        protected Keys ButtonToPress = Keys.Z;

        protected CommandRank RankToSend = CommandRank.Nice;

        protected bool WithinRange
        {
            get
            {
                float windowPressed = (float)Time.ActiveMilliseconds;
                return (windowPressed >= StartRange && windowPressed < EndRange);
            }
        }

        public JumpCommand(IActionCommandHandler commandAction, float totalRange, float leniency) : base(commandAction)
        {
            TotalRange = totalRange;
            Leniency = leniency;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            if (values != null && values.Length > 0)
                RankToSend = (CommandRank)values[0];

            StartRange = (float)Time.ActiveMilliseconds + Leniency;
            EndRange = (float)Time.ActiveMilliseconds + TotalRange;
        }

        protected override void ReadInput()
        {
            //Failure if it wasn't pressed
            if ((float)Time.ActiveMilliseconds >= EndRange)
            {
                OnComplete(CommandResults.Failure);
                return;
            }

            if (Input.GetKeyDown(ButtonToPress))
            {
                //Success if within range
                if (WithinRange == true)
                {
                    SendCommandRank(RankToSend);
                    OnComplete(CommandResults.Success);
                }
                //Otherwise failure
                else
                {
                    OnComplete(CommandResults.Failure);
                }
            }
        }

        protected override void OnDraw()
        {
            base.OnDraw();

            string text = "NO!";
            Color color = Color.Red;
            if (WithinRange == true)
            {
                text = "OKAY!";
                color = Color.Green;
            }

            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, text, new Vector2(300, 100), color, .7f);
        }
    }
}
