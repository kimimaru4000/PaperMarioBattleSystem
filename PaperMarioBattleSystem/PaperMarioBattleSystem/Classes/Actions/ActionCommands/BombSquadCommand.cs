using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Press A to throw bombs at a trajectory determined by the cursor's position.
    /// </summary>
    public sealed class BombSquadCommand : ActionCommand
    {
        private const float ThrowGravity = .1f;

        /// <summary>
        /// The time it takes to throw the bomb automatically if the player doesn't input anything.
        /// </summary>
        private const double AutomaticThrowTime = 6000d;

        /// <summary>
        /// How long the cursor indicating the angle the bomb was thrown at is active
        /// </summary>
        private const double ThrownCursorDur = 1000d;

        /// <summary>
        /// The number of bombs to throw.
        /// </summary>
        private readonly int BombCount = 3;

        /// <summary>
        /// The number of bombs thrown.
        /// </summary>
        private int NumBombsThrown = 0;

        private Vector2 StartPosition = Vector2.Zero;
        private double CircleRadius = 60d;
        public double CursorAngle { get; private set; } = 90d;

        private Vector2 BaseThrowVelocity = new Vector2(6f, 6f);

        private UIFourPiecedTex Cursor = null;
        private UIFourPiecedTex ThrownCursor = null;

        public Vector2 CursorPosition { get; private set; } = Vector2.Zero;
        public float CursorRotation { get; private set; } = 0f;

        private Vector2 CursorThrownPosition = Vector2.Zero;
        private double CursorMoveSpeed = 1.5d;
        public double CursorRotSpeed { get; private set; } = .25d;

        private const double MaxCursorAngle = 0d;
        private const double MinCursorAngle = -90d;

        private double ElapsedTime = 0d;
        private double LastBombThrowTime = 0d;
        public double ThrownCursorTime { get; private set; } = 0d;

        private Keys ButtonToPress = Keys.Z;

        public bool AllBombsThrown => (NumBombsThrown >= BombCount);

        public BombSquadCommand(IActionCommandHandler commandHandler, int bombCount) : base(commandHandler)
        {
            BombCount = bombCount;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            StartPosition = Camera.Instance.SpriteToUIPos((Vector2)values[0]);

            NumBombsThrown = 0;
            ElapsedTime = 0d;

            LastBombThrowTime = ElapsedTime + AutomaticThrowTime;
            CursorAngle = MinCursorAngle;

            CursorPosition = UtilityGlobals.GetPointAroundCircle(new Circle(StartPosition, CircleRadius), CursorAngle, true);

            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png");
            CroppedTexture2D croppedTex2D = new CroppedTexture2D(battleGFX, new Rectangle(14, 273, 46, 46));
            Cursor = new UIFourPiecedTex(croppedTex2D, croppedTex2D.WidthHeightToVector2(), .5f, Color.White);

            Cursor.Position = CursorPosition;
        }

        public override void EndInput()
        {
            base.EndInput();

            NumBombsThrown = 0;

            ElapsedTime = LastBombThrowTime = ThrownCursorTime = 0d;

            Cursor = null;
            ThrownCursor = null;
        }

        protected override void ReadInput()
        {
            ElapsedTime += Time.ElapsedMilliseconds;

            //Remove the thrown cursor if enough time passed
            if (ThrownCursor != null && ElapsedTime >= ThrownCursorTime)
            {
                ThrownCursor = null;
                ThrownCursorTime = 0d;
            }

            //Throw a bomb if the player either pressed the button or didn't press it in a certain amount of time
            if (AllBombsThrown == false)
            {
                if (Input.GetKeyDown(ButtonToPress) == true || ElapsedTime >= LastBombThrowTime)
                {
                    ThrowBomb();
                }

                //Update the cursor information
                UpdateCursor();
            }
            else
            {
                //If we threw all the bombs, end the command with a success once the thrown cursor disappears
                if (AllBombsThrown == true && ElapsedTime >= ThrownCursorTime)
                {
                    OnComplete(CommandResults.Success);
                    return;
                }
            }
        }

        private void ThrowBomb()
        {
            double angleRadians = UtilityGlobals.ToRadians(CursorAngle);

            Vector2 throwVelocity = new Vector2((float)Math.Cos(angleRadians) * BaseThrowVelocity.X, (float)Math.Sin(angleRadians) * BaseThrowVelocity.Y);

            //Debug.Log($"CursorAngle: {angleRadians}, ThrowVelocity: {throwVelocity}, Cos: {Math.Cos(angleRadians)}, Sin: {Math.Sin(angleRadians)}");

            NumBombsThrown++;

            LastBombThrowTime = ElapsedTime + AutomaticThrowTime;
            ThrownCursorTime = ElapsedTime + ThrownCursorDur;

            //Show a grey cursor indicating this is where the bomb was thrown for 1 second
            CursorThrownPosition = CursorPosition;
            ThrownCursor = Cursor.Copy();
            ThrownCursor.Depth -= .01f;
            ThrownCursor.TintColor = Color.Gray;

            SendResponse(new ActionCommandGlobals.BombSquadResponse(throwVelocity, ThrowGravity));
        }

        private void UpdateCursor()
        {
            CursorAngle = UtilityGlobals.Clamp(CursorAngle + CursorMoveSpeed, MinCursorAngle, MaxCursorAngle);

            //If it reached its limits, reverse the angle
            if (CursorAngle >= MaxCursorAngle || CursorAngle <= MinCursorAngle)
            {
                CursorMoveSpeed = -CursorMoveSpeed;
            }

            CursorPosition = UtilityGlobals.GetPointAroundCircle(new Circle(StartPosition, CircleRadius), CursorAngle, true);
            CursorRotation = (float)(-ElapsedTime * UtilityGlobals.ToRadians(CursorRotSpeed));

            Cursor.Position = CursorPosition;
            Cursor.Rotation = CursorRotation;
        }
    }
}
