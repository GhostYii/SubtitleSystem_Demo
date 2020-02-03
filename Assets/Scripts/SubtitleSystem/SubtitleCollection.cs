//ORG: Ghostyii & MOONLIGHTGAME
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubtitleSystem
{
    public class SubtitleCollection : IEnumerable
    {
        private List<Subtitle> subs;

        public bool IsCompleted
        {
            get
            {
                foreach (Subtitle s in subs)
                    if (!s.IsCompleted)
                        return false;

                return true;
            }
        }
        public List<Subtitle> Subs { get => subs; set => subs = value; }

        public Subtitle this[int index]
        {
            get => subs[index];
            set => subs[index] = value;
        }

        public SubtitleCollection()
        {
            subs = new List<Subtitle>();
        }

        public SubtitleCollection(Subtitle item)
        {
            subs = new List<Subtitle>();
            subs.Add(item);
        }

        public SubtitleCollection(float interval)
        {
            AddInterval(interval);
        }

        public void Add(Subtitle item)
        {
            subs.Add(item);
        }

        public void AddInterval(float interval)
        {
            if (subs == null)
                subs = new List<Subtitle>();

            subs.Add(new Subtitle(string.Empty, Vector3.zero, 14, Color.white, "Arial", interval));
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < subs.Count; i++)
                yield return subs[i];
        }
    }
}
