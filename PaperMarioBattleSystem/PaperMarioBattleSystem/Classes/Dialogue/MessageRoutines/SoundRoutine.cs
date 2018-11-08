using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A routine that plays a sound.
    /// </summary>
    public sealed class SoundRoutine : MessageRoutine
    {
        private string SoundPath = string.Empty;
        private DialogueGlobals.SoundLoadTypes SoundType = DialogueGlobals.SoundLoadTypes.Raw;
        private float SoundVolume = 1f;

        public SoundRoutine(DialogueBubble bubble, in string soundPath, in DialogueGlobals.SoundLoadTypes soundType, in float soundVolume)
            : base(bubble)
        {
            SoundPath = soundPath;
            SoundType = soundType;
            SoundVolume = soundVolume;
        }

        public override void OnStart()
        {
            //Play the appropriate type of sound
            if (SoundType == DialogueGlobals.SoundLoadTypes.Raw)
            {
                SoundManager.Instance.PlayRawSound(SoundPath, SoundVolume);
            }
            else
            {
                SoundManager.Instance.PlaySound(SoundPath, SoundVolume);
            }

            Complete = true;
        }

        public override void OnEnd()
        {

        }

        public override void Update()
        {

        }
    }
}
