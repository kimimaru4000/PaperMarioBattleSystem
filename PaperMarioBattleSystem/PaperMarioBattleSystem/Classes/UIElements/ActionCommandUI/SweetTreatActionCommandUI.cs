using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles the UI for Sweet Treat.
    /// </summary>
    public class SweetTreatActionCommandUI : ActionCommandUI<SweetTreatCommand>
    {
        protected bool Initialized = false;
        protected bool ShowPartnerInfo = false;

        protected UIFourPiecedTex Cursor = null;

        protected UICroppedTexture2D MarioHPIcon = null;
        protected UICroppedTexture2D PartnerHPIcon = null;
        protected UICroppedTexture2D FPIcon = null;
        protected UIText MarioHPText = null;
        protected UIText PartnerHPText = null;
        protected UIText FPText = null;

        protected int MarioHPRestoredVal = 0;
        protected int PartnerHPRestoredVal = 0;
        protected int FPRestoredVal = 0;

        public SweetTreatActionCommandUI(SweetTreatCommand sweetTreatCommand, bool showPartnerInfo) : base(sweetTreatCommand)
        {
            ShowPartnerInfo = showPartnerInfo;
        }

        private void Initialize()
        {
            //Define the UI to display
            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            CroppedTexture2D croppedTex2D = new CroppedTexture2D(battleGFX, new Rectangle(14, 273, 46, 46));
            Cursor = new UIFourPiecedTex(croppedTex2D, croppedTex2D.WidthHeightToVector2(), .5f, Color.White);

            MarioHPIcon = new UICroppedTexture2D(new CroppedTexture2D(battleGFX, new Rectangle(324, 407, 61, 58)));
            FPIcon = new UICroppedTexture2D(new CroppedTexture2D(battleGFX, new Rectangle(179, 416, 40, 39)));

            MarioHPText = new UIText("0", Color.Black);
            FPText = new UIText("0", Color.Black);

            //Set UI properties
            MarioHPIcon.Position = MarioHPText.Position = ActionCmd.StartPosition + new Vector2(-10, -45);
            FPIcon.Position = FPText.Position = ActionCmd.StartPosition + new Vector2(-45, -95);

            MarioHPText.Position += new Vector2(0f, 10f);
            FPText.Position += new Vector2(0f, 10f);

            if (ShowPartnerInfo == true)
            {
                PartnerHPIcon = new UICroppedTexture2D(new CroppedTexture2D(battleGFX, new Rectangle(324, 407, 61, 58)));
                PartnerHPText = new UIText("0", Color.Black);

                PartnerHPIcon.Position = PartnerHPText.Position = ActionCmd.StartPosition + new Vector2(-80, -45);
                PartnerHPText.Position += new Vector2(0f, 10f);
                PartnerHPIcon.Origin = PartnerHPText.Origin = new Vector2(.5f, .5f);
                PartnerHPIcon.Depth = .6f;
                PartnerHPText.Depth = .61f;
            }

            MarioHPIcon.Depth = FPIcon.Depth = .6f;
            MarioHPText.Depth = FPText.Depth = .61f;

            MarioHPIcon.Origin = FPIcon.Origin = MarioHPText.Origin = FPText.Origin = new Vector2(.5f, .5f);

            //Set cursor position
            Cursor.Position = UtilityGlobals.GetPointAroundCircle(new Circle(ActionCmd.StartPosition, ActionCmd.CircleRadius), ActionCmd.CursorAngle, true);
        }

        public override void Update()
        {
            if (ActionCmd?.AcceptingInput == false) return;

            //Initialize if not initialized
            if (Initialized == false)
            {
                Initialize();
                Initialized = true;
            }

            if (MarioHPRestoredVal != ActionCmd.HealingResponse.MarioHPRestored)
            {
                MarioHPRestoredVal = ActionCmd.HealingResponse.MarioHPRestored;
                MarioHPText.Text = MarioHPRestoredVal.ToString();
            }

            if (PartnerHPText != null && PartnerHPRestoredVal != ActionCmd.HealingResponse.PartnerHPRestored)
            {
                PartnerHPRestoredVal = ActionCmd.HealingResponse.PartnerHPRestored;
                PartnerHPText.Text = PartnerHPRestoredVal.ToString();
            }

            if (FPRestoredVal != ActionCmd.HealingResponse.FPRestored)
            {
                FPRestoredVal = ActionCmd.HealingResponse.FPRestored;
                FPText.Text = FPRestoredVal.ToString();
            }

            Cursor.Position = UtilityGlobals.GetPointAroundCircle(new Circle(ActionCmd.StartPosition, ActionCmd.CircleRadius), ActionCmd.CursorAngle, true);
            Cursor.Rotation = (float)(-ActionCmd.ElapsedTime * UtilityGlobals.ToRadians(ActionCmd.CursorRotSpeed));
        }

        public override void Draw()
        {
            if (ActionCmd?.AcceptingInput == false) return;

            Cursor.Draw();

            MarioHPIcon.Draw();
            MarioHPText.Draw();
            FPIcon.Draw();
            FPText.Draw();
            if (ShowPartnerInfo == true)
            {
                PartnerHPIcon.Draw();
                PartnerHPText.Draw();
            }

            for (int i = 0; i < ActionCmd.StarsThrown.Count; i++)
            {
                ActionCmd.StarsThrown[i].Draw();
            }
        }
    }
}
