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
    /// The Cloud Nine Status Effect.
    /// The entity's Evasion is raised by a certain amount until it ends.
    /// <para>This Status Effect is inflicted with Lakilester's Cloud Nine move.</para>
    /// </summary>
    public sealed class CloudNineStatus : DodgyStatus
    {
        public CloudNineStatus(int duration) : base(duration)
        {
            StatusType = Enumerations.StatusTypes.CloudNine;

            //Cloud Nine's Evasion is 50%
            EvasionValue = .5d;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(461, 350, 30, 27));

            //CloudRect = new Rectangle(503, 353, 35, 21);

            AfflictedMessage = "Chances of being attacked will decrease!";
            RemovedMessage = "The effect of Cloud Nine have worn off!";
        }

        public override StatusEffect Copy()
        {
            return new CloudNineStatus(Duration);
        }
    }
}
