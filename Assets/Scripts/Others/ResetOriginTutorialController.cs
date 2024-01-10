using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetOriginTutorialController : MonoBehaviour
{
    [SerializeField] private Image _imgController;
    [SerializeField] private Sprite _spriteQuest2Controller;
    [SerializeField] private Sprite _spriteQuestProController;

    private void Start()
    {
        var questType = OVRPlugin.GetSystemHeadsetType();

        var questProEnuns = new List<OVRPlugin.SystemHeadset> { OVRPlugin.SystemHeadset.Meta_Link_Quest_Pro, OVRPlugin.SystemHeadset.Meta_Quest_Pro };

        if (questProEnuns.Contains(questType)) _imgController.sprite = _spriteQuestProController;
        else _imgController.sprite = _spriteQuest2Controller;

        gameObject.SetActive(false);
        Destroy(this);
    }
}
