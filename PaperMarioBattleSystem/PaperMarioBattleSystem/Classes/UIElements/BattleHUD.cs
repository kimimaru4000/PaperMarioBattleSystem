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
    /// The HUD shown during Battle.
    /// </summary>
    public sealed class BattleHUD : IUpdateable, IDrawable
    {
        private CroppedTexture2D HPBanner = null;
        private CroppedTexture2D FPBanner = null;
        private CroppedTexture2D StarPowerBanner = null;

        private CroppedTexture2D MarioHPIcon = null;
        private CroppedTexture2D PartnerHPIcon = null;
        private CroppedTexture2D FPIcon = null;
        private CroppedTexture2D StarPowerIcon = null;

        private BattleManager BManager = null;

        public BattleHUD(BattleManager bManager)
        {
            SetBattleManager(bManager);

            Texture2D hudTex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleHUD.png");

            HPBanner = new CroppedTexture2D(hudTex, new Rectangle(358, 9, 166, 30));
            FPBanner = new CroppedTexture2D(hudTex, new Rectangle(532, 9, 165, 30));
            StarPowerBanner = new CroppedTexture2D(hudTex, new Rectangle(358, 49, 295, 30));

            MarioHPIcon = new CroppedTexture2D(hudTex, new Rectangle(2, 38, 45, 41));
            FPIcon = new CroppedTexture2D(hudTex, new Rectangle(56, 42, 34, 37));
            //Use generic icon for Partners for now
            PartnerHPIcon = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"),
                new Rectangle(324, 407, 61, 58));

            StarPowerIcon = new CroppedTexture2D(hudTex, new Rectangle(102, 40, 40, 39));
        }

        public void SetBattleManager(BattleManager bManager)
        {
            BManager = bManager;
        }

        public void Update()
        {

        }

        public void Draw()
        {
            if (BManager == null) return;

            BattleMario mario = BManager.Mario;
            BattlePartner partner = BManager.Partner;

            if (mario != null)
            {
                SpriteRenderer.Instance.DrawUI(HPBanner.Tex, new Vector2(60, 50), HPBanner.SourceRect, Color.White, 0f, Vector2.Zero, 1f, false, false, .1f, true);
                SpriteRenderer.Instance.DrawUI(MarioHPIcon.Tex, new Vector2(50, 40), MarioHPIcon.SourceRect, Color.White, 0f, Vector2.Zero, 1f, false, false, .11f, true);

                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont,
                    $"{mario.CurHP}/{mario.BattleStats.MaxHP}", new Vector2(120, 54), Color.White, .2f);
            }
            if (partner != null)
            {
                SpriteRenderer.Instance.DrawUI(HPBanner.Tex, new Vector2(60, 90), HPBanner.SourceRect, Color.White, 0f, Vector2.Zero, 1f, false, false, .1f, true);
                SpriteRenderer.Instance.DrawUI(PartnerHPIcon.Tex, new Vector2(50, 85), PartnerHPIcon.SourceRect, Color.White, 0f, Vector2.Zero, .7f, false, false, .11f, true);

                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont,
                    $"{partner.CurHP}/{partner.BattleStats.MaxHP}", new Vector2(120, 94), Color.White, .2f);
            }
            if (mario != null)
            {
                //Draw FP info
                SpriteRenderer.Instance.DrawUI(FPBanner.Tex, new Vector2(250, 50), FPBanner.SourceRect, Color.White, 0f, Vector2.Zero, 1f, false, false, .1f, true);
                SpriteRenderer.Instance.DrawUI(FPIcon.Tex, new Vector2(240, 45), FPIcon.SourceRect, Color.White, 0f, Vector2.Zero, 1f, false, false, .11f, true);
                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont,
                    $"{mario.CurFP}/{mario.BattleStats.MaxFP}", new Vector2(305, 53), Color.White, .2f);

                //Draw Star Power info
                SpriteRenderer.Instance.DrawUI(StarPowerBanner.Tex, new Vector2(250, 90), StarPowerBanner.SourceRect, Color.White, 0f, Vector2.Zero, 1f, false, false, .1f, true);
                SpriteRenderer.Instance.DrawUI(StarPowerIcon.Tex, new Vector2(240, 85), StarPowerIcon.SourceRect, Color.White, 0f, Vector2.Zero, 1f, false, false, .11f, true);
                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont,
                    $"{mario.MStats.SSStarPower.SPU}/{mario.MStats.SSStarPower.MaxSPU}", new Vector2(305, 93), Color.White, .2f);
            }
        }
    }
}
