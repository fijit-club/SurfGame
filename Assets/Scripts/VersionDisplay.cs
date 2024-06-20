using UnityEngine;
using TMPro;

public class VersionDisplay : MonoBehaviour
{
    private void Awake()
    {
        TMP_Text text = GetComponent<TMP_Text>();
        text.text = "v" + Application.version;
    }
}
