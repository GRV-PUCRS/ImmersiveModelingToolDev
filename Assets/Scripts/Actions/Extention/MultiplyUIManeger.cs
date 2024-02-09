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
        float spacingX = float.Parse(spacingXInputField.text)/100f;
        float spacingY = float.Parse(spacingYInputField.text)/100f;
        float spacingZ = float.Parse(spacingZInputField.text)/100f;

        // Armazena a posição e a rotação originais
        Transform cubo = originalObjectPrefab.transform.GetChild(0);
        Vector3 originalPosition = cubo.position;

        Quaternion originalRotation = cubo.rotation;

        if (axisObject != null)
        {
            Destroy(axisObject);
        }

        //Quaternion originalRotation = originalObjectPrefab.transform.rotation;
        // Instanciar as cópias do objeto original com o espaçamento especificado
        for (int i = 1; i <= quantidade; i++)
        {
            //GameObject newObject = Instantiate(originalObjectPrefab, originalObjectPrefab.transform.position, originalRotation);
            //newObject.transform.Translate(new Vector3(spacingX, spacingY, spacingZ) * i);

            // Calcula a nova posição com base no espaçamento
            Vector3 newPosition = originalPosition;
            GameObject sceneElement = new GameObject("copia "+ i);
            // Instancia o novo objeto
            GameObject newObject = Instantiate(cubo.gameObject, newPosition, originalRotation);

            newObject.transform.Translate(new Vector3(spacingX * i, spacingY * i, spacingZ * i));
            // Define o objeto como filho do objeto original

            newObject.transform.SetParent(sceneElement.transform);
            newObject.GetComponent<DragUI>().SetNewTransform(sceneElement.transform);
            //GameObject newObject = Instantiate(originalObjectPrefab, originalObjectPrefab.transform.position, Quaternion.identity);
            //newObject.transform.Translate(new Vector3(i * spacingX, i * spacingY, i * spacingZ));
            OculusManager.Instance.TaskManager.AddObjectInTask(sceneElement.transform);
        }
          
        Destroy(gameObject);
    }   
    internal void ativar(GameObject gameObject)
    {
        transform.position = gameObject.transform.position;
        originalObjectPrefab = gameObject;

        // Acessa o componente Collider do objeto original
        Collider collider = originalObjectPrefab.GetComponent<Collider>();

        if (collider != null)
        {
            // Obtém as dimensões do Collider
            Vector3 size = collider.bounds.size;

            // Exibe as dimensões nas caixas de texto
            XrealInputField.text = size.x.ToString();
            YrealInputField.text = size.y.ToString();
            ZrealInputField.text = size.z.ToString();
        }

        var child = originalObjectPrefab.transform.GetChild(0);
        // GameObject newObject = Instantiate(axisPrefab, originalObjectPrefab.transform.position, originalObjectPrefab.transform.rotation);
        axisObject = Instantiate(axisPrefab, child.position, child.rotation);
        axisObject.transform.SetParent(child);
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
