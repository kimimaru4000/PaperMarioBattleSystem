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
        private Texture2D HUDTex = null;

        public BattleHUD()
        {
            HUDTex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleHUD.png");
        }

        public void Update()
        {

        }

        public void Draw()
        {
            //Vector2 hudPivot = HUDTex.GetCenterOrigin();
            SpriteRenderer.Instance.Draw(HUDTex, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, false, false, .1f, true);

            BattleMario mario = BattleManager.Instance.GetMario();
            BattlePartner partner = BattleManager.Instance.GetPartner();

            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont,
                $"{mario.Name} HP: {mario.CurHP}/{mario.BattleStats.MaxHP}", new Vector2(30, 10), Color.White, .2f);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont,
                $"{partner.Name} HP: {partner.CurHP}/{partner.BattleStats.MaxHP}", new Vector2(30, 30), Color.White, .2f);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont,
                $"FP: {mario.CurFP}/{mario.BattleStats.MaxFP}", new Vector2(30, 50), Color.White, .2f);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont,
                $"SP: {mario.MStats.SSStarPower.SPU}/{mario.MStats.SSStarPower.MaxSPU}", new Vector2(155, 50), Color.White, .2f);
        }
    }
}
