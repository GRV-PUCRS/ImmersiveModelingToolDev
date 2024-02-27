using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultiplyUIManeger : Singleton<MultiplyUIManeger>

{
    public GameObject View;
    public GameObject originalObjectPrefab;
    public TextMeshProUGUI quantityInputField;
    public TextMeshProUGUI spacingXInputField;
    public TextMeshProUGUI spacingYInputField;
    public TextMeshProUGUI spacingZInputField;
    public GameObject axisPrefab;
    public GameObject axisObject;
    public TextMeshProUGUI XrealInputField;
    public TextMeshProUGUI YrealInputField;
    public TextMeshProUGUI ZrealInputField;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z))
        {
            quantityInputField.text = 2.ToString();
            spacingXInputField.text = (0.3).ToString();
            spacingYInputField.text = 0.ToString();
            spacingZInputField.text = 0.ToString();

            metodo();
        }
    }

    public void ChangeAxisValue(TextMeshProUGUI txtField)
    {
        //  teclado virtual para obter a entrada do usuário
        KeyboardManager.Instance.GetInput(result => UpdateUIValue(txtField, result), null, txtField.text, TouchScreenKeyboardType.NumbersAndPunctuation | TouchScreenKeyboardType.DecimalPad);
    }

    // Método para atualizar o valor do campo de texto da UI
    private void UpdateUIValue(TextMeshProUGUI txtField, string result)
    {
        // if (txtField == quantityInputField)
        // {
        //     txtField.text = result;
        // }
        //  else
        //  {
        //    float resultInCentimeters = float.Parse(result) / 100f;
        //  txtField.text = resultInCentimeters.ToString();
        //}
        // float resultInCentimeters = float.Parse(result) / 100f;

        // Atribuindo o valor convertido ao campo de texto
        //  txtField.text = resultInCentimeters.ToString();
        txtField.text = result;
    }

    public void metodo()
    {
        int quantidade = int.Parse(quantityInputField.text);
        float spacingX = float.Parse(spacingXInputField.text) / 100f;
        float spacingY = float.Parse(spacingYInputField.text) / 100f;
        float spacingZ = float.Parse(spacingZInputField.text) / 100f;

        /*   // Armazena a posição média dos objetos no grupo
           Vector3 groupCenter = Vector3.zero;
           int objectCount = 0;

           // Itera por todos os objetos no grupo para calcular a posição média
           foreach (Transform child in originalObjectPrefab.transform)
           {
               groupCenter += child.position;
               objectCount++;
           }

           // Calcula o centro médio
           if (objectCount > 0)
           {
               groupCenter /= objectCount;
           }

           Quaternion originalRotation = originalObjectPrefab.transform.rotation;
           if (axisObject != null)
           {
               axisObject.SetActive(false);

           }

           // Instanciar as cópias do grupo original com o espaçamento especificado
           for (int i = 1; i <= quantidade; i++)
           {
               GameObject sceneElement = new GameObject("copia " + i);

               // Instancia uma cópia de cada objeto no grupo
               foreach (Transform child in originalObjectPrefab.transform)
               {
                   // Instancia o novo objeto
                   GameObject newObject = Instantiate(child.gameObject, child.position, originalRotation);

                   // Calcula a posição com base no espaçamento e no centro do grupo
                   Vector3 offset = new Vector3(spacingX * i, spacingY * i, spacingZ * i);
                   newObject.transform.position = groupCenter + (child.position - groupCenter) + offset;

                   // Define o objeto como filho do objeto original
                   newObject.transform.SetParent(sceneElement.transform);
                   newObject.GetComponent<DragUI>().SetNewTransform(sceneElement.transform);
               }

               OculusManager.Instance.TaskManager.AddObjectInTask(sceneElement.transform);
           }

           Destroy(gameObject);
       */
        // Armazena a posição e a rotação originais
        // Transform cubo = originalObjectPrefab.transform.GetChild(0);
        Vector3 originalPosition = originalObjectPrefab.transform.position;

        Quaternion originalRotation = originalObjectPrefab.transform.rotation;

        if (axisObject != null)
        {
            //  Destroy(axisObject, 0f);
            axisObject.SetActive(false);

        }

        //Quaternion originalRotation = originalObjectPrefab.transform.rotation;
        // Instanciar as cópias do objeto original com o espaçamento especificado
        for (int i = 1; i <= quantidade; i++)
        {
            //GameObject newObject = Instantiate(originalObjectPrefab, originalObjectPrefab.transform.position, originalRotation);
            //newObject.transform.Translate(new Vector3(spacingX, spacingY, spacingZ) * i);

            // Calcula a nova posição com base no espaçamento
            Vector3 newPosition = originalPosition;
            // GameObject sceneElement = new GameObject("copia "+ i);
            // Instancia o novo objeto
            GameObject sceneElement = Instantiate(originalObjectPrefab, newPosition, originalRotation);

            sceneElement.transform.Translate(new Vector3(spacingX * i, spacingY * i, spacingZ * i));
            // Define o objeto como filho do objeto original

            //sceneElement.transform.SetParent(sceneElement.transform);
            //sceneElement.GetComponent<DragUI>().SetNewTransform(sceneElement.transform);
            //GameObject newObject = Instantiate(originalObjectPrefab, originalObjectPrefab.transform.position, Quaternion.identity);
            //newObject.transform.Translate(new Vector3(i * spacingX, i * spacingY, i * spacingZ));
            OculusManager.Instance.TaskManager.AddObjectInTask(sceneElement.transform);


            foreach (DragUI child in sceneElement.GetComponentsInChildren<DragUI>())
            {
                child.SetNewTransform(sceneElement.transform);
            }

            if (axisObject != null)
            {
                // Destroy(axisObject, 0f);
                //  axisObject.SetActive(false);
            }
        }


        Destroy(gameObject);
    }

    public void ativar(GameObject gameObject)
    {
        transform.position = gameObject.transform.position;
        originalObjectPrefab = gameObject;
        var child = originalObjectPrefab.transform.GetChild(0);
        // Acessa o componente Collider do objeto original
        //  Collider collider = originalObjectPrefab.transform.GetChild(0).GetComponent<Collider>();


        // Obtém as dimensões do Collider
        // Vector3 size = collider.bounds.size;

        // Exibe as dimensões nas caixas de texto
        XrealInputField.text = (child.localScale.x * 100f).ToString("F2");
        YrealInputField.text = (child.localScale.y * 100f).ToString("F2");
        ZrealInputField.text = (child.localScale.z * 100f).ToString("F2");

        //  var child = originalObjectPrefab.transform.GetChild(0);
        // GameObject newObject = Instantiate(axisPrefab, originalObjectPrefab.transform.position, originalObjectPrefab.transform.rotation);
        axisObject = Instantiate(axisPrefab, gameObject.transform.position, gameObject.transform.rotation);
        axisObject.transform.SetParent(gameObject.transform);
    }
}



/*using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplyUIManeger : Singleton<MultiplyUIManeger>
{
    public GameObject View;
    public GameObject originalObjectPrefab;
    public TextMeshProUGUI quantityInputField;
    public TextMeshProUGUI spacingXInputField;
    public TextMeshProUGUI spacingYInputField;
    public TextMeshProUGUI spacingZInputField;

    // Adicione referências para os botões
    public Button BTNmult;
    public Button BTNX;
    public Button BTNY;
    public Button BTNZ;
    public Button BTNConfirmar;

    private void Start()
    {
        // Adicione ouvintes de clique para os botões
        BTNmult.onClick.AddListener(() => OnButtonClick(BTNmult));
        BTNX.onClick.AddListener(() => OnButtonClick(BTNX));
        BTNY.onClick.AddListener(() => OnButtonClick(BTNY));
        BTNZ.onClick.AddListener(() => OnButtonClick(BTNZ));
        BTNConfirmar.onClick.AddListener(OnConfirmarClick);
    }

    private void OnButtonClick(Button button)
    {
        // Desative os outros botões quando um botão é pressionado
        BTNmult.interactable = false;
        BTNX.interactable = false;
        BTNY.interactable = false;
        BTNZ.interactable = false;

        // Ative apenas o botão pressionado
        button.interactable = true;

        // Ative a interface do usuário
        View.SetActive(true);

        // Atualize o texto do campo de entrada para evitar valores residuais
        quantityInputField.text = "";
        spacingXInputField.text = "";
        spacingYInputField.text = "";
        spacingZInputField.text = "";
    }

    private void OnConfirmarClick()
    {
        // Validar entrada para garantir que seja um número inteiro positivo
        if (!int.TryParse(quantityInputField.text, out int quantidade) || quantidade <= 0)
        {
            Debug.LogError("Quantidade inválida. Insira um número inteiro positivo.");
            return;
        }

        // Chamar método correspondente ao botão pressionado
        if (BTNmult.interactable)
        {
            MultiplyObjects();
        }
        else if (BTNX.interactable)
        {
            MultiplyObjectsAlongAxis(Vector3.right);
        }
        else if (BTNY.interactable)
        {
            MultiplyObjectsAlongAxis(Vector3.up);
        }
        else if (BTNZ.interactable)
        {
            MultiplyObjectsAlongAxis(Vector3.forward);
        }

        // Restaure a interface do usuário para o estado inicial
        View.SetActive(false);
        BTNmult.interactable = true;
        BTNX.interactable = true;
        BTNY.interactable = true;
        BTNZ.interactable = true;
    }

    private void MultiplyObjects()
    {
        // Lógica de multiplicação geral aqui
        for (int i = 0; i < quantidade; i++)
        {
             GameObject newObject = Instantiate(originalObjectPrefab, sceneElements[0].transform.position, Quaternion.identity);
             newObject.transform.Translate(new Vector3(i * spacingX, i * spacingY, i * spacingZ));
        }
    }

    private void MultiplyObjectsAlongAxis(Vector3 axis)
    {
        // Lógica de multiplicação ao longo de um eixo específico aqui
        for (int i = 0; i < quantidade; i++)
        {
             GameObject newObject = Instantiate(originalObjectPrefab, sceneElements[0].transform.position, Quaternion.identity);
             newObject.transform.Translate(i * spacing * axis);
        }
    }
    internal void ativar(GameObject gameObject)
    {
        View.SetActive(true);
        transform.position = gameObject.transform.position;
    }
}*/
