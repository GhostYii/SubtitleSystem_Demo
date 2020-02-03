//ORG: Ghostyii & MOONLIGHTGAME
using System;
using System.Collections;
using UnityEngine;

namespace SubtitleSystem
{
    public class SubtitleUtility
    {
        //等待duration时间后执行action
        public static void WaitSecondsForSomething(Action action, float duration)
        {
            GameObject obj = new GameObject("EventBuilder");
            EventBuilder script = obj.AddComponent<EventBuilder>();
            script.StartCoroutine(script.OnEvent(action, duration));
        }

        //获取字体
        public static Font GetFontByFontName(string fontName)
        {
            return Resources.Load<Font>(string.Format("Font/{0}", fontName));
        }

        //在非Mono脚本中执行协程
        public static void StartCoroutine(IEnumerator routine)
        {
            GameObject obj = new GameObject("CoroutineBuilder");
            EventBuilder script = obj.AddComponent<EventBuilder>();
            script.StartCoroutine(script.OnCoroutine(routine));
        }
        public static void StartCoroutine(IEnumerator routine, string routineTag)
        {
            GameObject obj = new GameObject(routineTag);
            EventBuilder script = obj.AddComponent<EventBuilder>();
            script.StartCoroutine(script.OnCoroutine(routine));
        }

        //终止某tag上的所有协程
        public static void StopAllCoroutines(string coroutineTag)
        {
            GameObject obj = GameObject.Find(coroutineTag);
            if (obj != null)
            {
                var script = obj.GetComponent<EventBuilder>();
                script.StopAllCoroutines();
                script.Destroy();
            }
        }
    }

    public class EventBuilder : MonoBehaviour
    {
        public IEnumerator OnEvent(Action action, float duration)
        {
            yield return new WaitForSeconds(duration);
            action();
            Destroy(gameObject);
        }

        public IEnumerator OnCoroutine(IEnumerator coroutine)
        {
            yield return coroutine;
            Destroy(gameObject);
        }

        //销毁带有该脚本的gameObject
        public void Destroy()
        {
            Destroy(gameObject);
        }

    }


}