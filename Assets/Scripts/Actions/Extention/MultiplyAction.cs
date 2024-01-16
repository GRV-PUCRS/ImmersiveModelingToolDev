
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiplyAction : AbstractSimpleAction
{
    [SerializeField] private MultiplyUIManeger _multiplyUIPrefab;

    public override void ApplyAction(List<GameObject> sceneElements)
    {
        // Retorna icone da acao para a prateleira
        ObjectStore.Instance.RetrieveObjectToStore(transform);

        // Verificar se o objeto original foi fornecido e se há apenas um elemento selecionado
        if (sceneElements.Count != 1 && sceneElements[0].transform.childCount != 1)
            return;

        var element = sceneElements[0].transform.GetChild(0);
        
        // Instancia objeto na cena
        var multiplyUIInstance = Instantiate(_multiplyUIPrefab);
        multiplyUIInstance.ativar(sceneElements[0]);

        // Posiciona UI para ficar na frente do usuario
        var hmdToObjectDirection = InputController.Instance.HMD.position - element.transform.position;
        multiplyUIInstance.transform.position = element.transform.position + hmdToObjectDirection * 0.2f;
        multiplyUIInstance.transform.rotation = Quaternion.LookRotation(-hmdToObjectDirection);
    }
}
