using Data;
using TMPro;
using UnityEngine;

namespace UI_Scripts
{
    public class DebugLog: MonoBehaviour
    {
        [SerializeField] private TMP_Text debugLog;
        
        public static DebugLog Instance;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            DontDestroyOnLoad(gameObject);
        }

            
        public void AddText(string text)
        {
            debugLog.text += text + "\n";
        }
        
    }
}
