//ORG: Ghostyii & MOONLIGHTGAME
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubtitleSystem
{
    //字幕序列
    //所有对字幕序列进行的操作仅在未播放前操作有效
    public class SubtitleSequence
    {
        private bool isPause = false;

        private SubtitleCollection currentSubs = null;
        private List<SubtitleCollection> scLst = new List<SubtitleCollection>();

        public bool IsPause { get => isPause; }

        public SubtitleSequence()
        {
            scLst = new List<SubtitleCollection>();
            isPause = false;
        }
        
        //尾部添加
        public void Append(Subtitle item)
        {
            SubtitleCollection c = new SubtitleCollection();
            c.Add(item);
            scLst.Add(c);
        }
        
        //尾部添加空白间隔
        public void AppendInterval(float interval)
        {
            SubtitleCollection c = new SubtitleCollection();
            c.AddInterval(interval);
            scLst.Add(c);
        }
        
        //插入字幕
        public void Insert(int index, Subtitle item)
        {
            scLst.Insert(index, new SubtitleCollection(item));
        }
        
        //插入间隔
        public void InsertInterval(int index, float interval)
        {
            scLst.Insert(index, new SubtitleCollection(interval));
        }
        
        //添加同时播放的字幕
        public void Join(Subtitle item)
        {
            Join(scLst.Count - 1, item);
        }
        public void Join(int index, Subtitle item)
        {
            if (scLst.Count == 0)
                scLst.Add(new SubtitleCollection(item));
            else
                scLst[index].Add(item);
        }

        //开始播放
        public void Start()
        {
            if (scLst == null || scLst.Count == 0)
                return;

            isPause = false;
            SubtitleUtility.StartCoroutine(Play(), string.Format("SubtitleCoroutine{0}", GetHashCode()));
        }
        //切换暂停状态
        public void TogglePause()
        {
            isPause = !isPause;
           
            if (isPause && currentSubs != null)
                foreach (Subtitle s in currentSubs)                
                    s.Pause();
            else if (currentSubs != null && !isPause)
                foreach (Subtitle s in currentSubs)
                    s.TogglePause();
        }
        //停止播放
        public void Stop()
        {
            SubtitleUtility.StopAllCoroutines(string.Format("SubtitleCoroutine{0}", GetHashCode()));
            if (currentSubs != null)
                foreach (Subtitle s in currentSubs)
                    s.Stop();
            isPause = false;
        }
        //立即停止播放并且重新开始
        public void Restart()
        {
            Stop();
            Start();
        }

        public void Clear()
        {
            Stop();
            scLst.Clear();
        }

        private IEnumerator Play()
        {
            yield return new WaitUntil(() => !isPause);

            foreach (var subs in scLst)
            {
                currentSubs = subs;
                foreach (Subtitle sub in subs)                
                    sub.Play();
                
                yield return new WaitUntil(() => subs.IsCompleted && !isPause);
            }
        }
    }
}