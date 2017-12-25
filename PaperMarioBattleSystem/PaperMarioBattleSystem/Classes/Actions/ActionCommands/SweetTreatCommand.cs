using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Hold Left to ready a star and release to throw it. Hit the icons to heal HP and FP.
    /// Hitting a Poison Mushroom makes you unable to throw stars for a short time.
    /// </summary>
    public class SweetTreatCommand : ActionCommand
    {
        private bool StarReady = false;

        private UIFourPiecedTex Cursor = null;

        /// <summary>
        /// Handles spawning the icons.
        /// </summary>
        protected SweetTreatElementSpawner IconSpawner = null;

        private Vector2 StartPosition = Vector2.Zero;
        private Vector2 StarThrowVelocity = new Vector2(6f, 6f);

        private double CircleRadius = 60d;
        private double CursorAngle = 90d;

        private double CursorMoveSpeed = 1.5d;
        private double CursorRotSpeed = .25d;

        private const double MaxCursorAngle = 5d;
        private const double MinCursorAngle = -75d;

        protected Keys ButtonToThrow = Keys.Left;

        protected double ElapsedTime = 0d;

        public SweetTreatCommand(IActionCommandHandler commandHandler) : base(commandHandler)
        {
            CroppedTexture2D croppedTex2D = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(14, 273, 46, 46));
            Cursor = new UIFourPiecedTex(croppedTex2D, croppedTex2D.WidthHeightToVector2(), .5f, Color.White);
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            ElapsedTime = 0d;

            StartPosition = Camera.Instance.SpriteToUIPos((Vector2)values[0]);

            CursorAngle = MinCursorAngle;

            Cursor.Position = UtilityGlobals.GetPointAroundCircle(StartPosition, CircleRadius, CursorAngle, true);

            Vector2 startPos = new Vector2(500, 15);
            Vector2 endPos = new Vector2(startPos.X, BattleManager.Instance.PartnerPos.Y + 350f);
            IconSpawner = new SweetTreatElementSpawner(4, 40f, 5000d, 750d, startPos, endPos, new SweetTreatGlobals.RestoreTypes[]
            {
                SweetTreatGlobals.RestoreTypes.MarioHP, SweetTreatGlobals.RestoreTypes.PartnerHP, SweetTreatGlobals.RestoreTypes.FP, SweetTreatGlobals.RestoreTypes.PoisonMushroom
            }, new int[] { 7, 7, 6, 2 });
        }

        public override void EndInput()
        {
            base.EndInput();

            ElapsedTime = 0d;

            IconSpawner = null;
        }

        protected override void ReadInput()
        {
            //If a star isn't ready, check if the player held the button required to ready one
            if (StarReady == false)
            {
                //Ready the star
                if (Input.GetKey(ButtonToThrow) == true)
                {
                    StarReady = true;

                    
                }
            }
            else
            {
                //Check if the player released the button while a star was ready, then throw it
                if (Input.GetKey(ButtonToThrow) == false)
                {
                    StarReady = false;
                }
            }

            ElapsedTime += Time.ElapsedMilliseconds;

            UpdateCursor();
            IconSpawner.Update();

            //If the spawner is completely done, end the Action Command
            if (IconSpawner.CompletelyDone == true)
            {
                OnComplete(CommandResults.Success);
            }
        }

        private void UpdateCursor()
        {
            CursorAngle = UtilityGlobals.Clamp(CursorAngle + CursorMoveSpeed, MinCursorAngle, MaxCursorAngle);

            //If it reached its limits, reverse the angle
            if (CursorAngle >= MaxCursorAngle || CursorAngle <= MinCursorAngle)
            {
                CursorMoveSpeed = -CursorMoveSpeed;
            }

            Cursor.Position = UtilityGlobals.GetPointAroundCircle(StartPosition, CircleRadius, CursorAngle, true);
            Cursor.Rotation = (float)(-ElapsedTime * UtilityGlobals.ToRadians(CursorRotSpeed));
        }

        protected override void OnDraw()
        {
            base.OnDraw();

            Cursor.Draw();
        }
    }
}
