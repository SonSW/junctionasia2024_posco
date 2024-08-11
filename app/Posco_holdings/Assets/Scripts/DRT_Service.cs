using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using SocketIOClass = SocketIOClient.SocketIO; // Add SocketIO client library
using System.Text;
using Debug = UnityEngine.Debug;

public class DRT_Service : MonoBehaviour
{
    public GameObject service_Pannel;
    public GameObject InputField;

    public GameObject Arrival;
    
    public InputField Dep_inpf;
    public InputField Ariv_inpf;
    public GameObject Dep_ConButt;
    public GameObject Ariv_ConButt;

    public GameObject SelectedPlace;

    public Text Dep_input_Txt;
    public Text Ariv_input_Txt;
    public Text SelectedPlaceTxt;

    public GameObject NextButton;
    public GameObject PreviousButton;

    private Vector2 nowPos, prePos;
    private Vector2 movePosDiff;

    private Vector3 initInptPos; // Added this line
    private Vector3 initPanPos; // Added this line
    private int Dep_Set = 0; // Added this line
    private int Ariv_Set = 0; // Added this line

    bool pannelDown;
    bool pannelUp;
    public bool pannelState;

    bool pannelDown2;
    bool pannelUp2;
    public bool pannelState2;

    Vector3 velocity = Vector3.zero;

    private SocketIOClass client;

    // Start is called before the first frame update
    void Start()
    {
        initInptPos = InputField.transform.localPosition;
        initPanPos = service_Pannel.transform.localPosition;

        ConnectToServer("http://127.0.0.1:5000");
    }

    // Update is called once per frame
    void Update()
    {
        /*
        getTouchDragValue();
        if (Mathf.Abs(movePosDiff.y) > 2)
        {
            if (pannelState) pannelDown = true;
            else pannelUp = true;
        }
        */

        if (Dep_inpf.GetComponent<InputField>().text.Length != Dep_Set && !pannelState)
        {
            Dep_Set = Dep_inpf.GetComponent<InputField>().text.Length;
            Dep_ConButt.SetActive(true);
        }

        if (Ariv_inpf.GetComponent<InputField>().text.Length != Ariv_Set && !pannelState)
        {
            Ariv_Set = Ariv_inpf.GetComponent<InputField>().text.Length;
            Ariv_ConButt.SetActive(true);
        }

        if (pannelDown)
        {
            if (Mathf.Abs(initInptPos.y - 1400 - InputField.transform.localPosition.y) >= 1)
            {
                InputField.transform.localPosition = Vector3.Lerp(InputField.transform.localPosition, initInptPos + Vector3.down * 1400, 0.1f);
            }
            if (Mathf.Abs(initPanPos.y - 1530 - service_Pannel.transform.localPosition.y) >= 1)
            {
                service_Pannel.transform.localPosition = Vector3.Lerp(service_Pannel.transform.localPosition, initPanPos + Vector3.down * 1530, 0.1f);
            }
            else
            {
                InputField.transform.localPosition = initInptPos + Vector3.down * 1400;
                service_Pannel.transform.localPosition = initPanPos + Vector3.down * 1530;
                pannelDown = false;
            }
        }


        if (pannelUp)
        {
            if (Mathf.Abs(InputField.transform.localPosition.y - initInptPos.y) >= 1)
            {
                InputField.transform.localPosition = Vector3.Lerp(InputField.transform.localPosition, initInptPos, 0.1f);
            }
            if (Mathf.Abs(service_Pannel.transform.localPosition.y - initPanPos.y) >= 1)
            {
                service_Pannel.transform.localPosition = Vector3.Lerp(service_Pannel.transform.localPosition, initPanPos, 0.1f);
            }
            else
            {
                InputField.transform.localPosition = initInptPos;
                service_Pannel.transform.localPosition = initPanPos;
                pannelUp = false;
            }
        }



        if (pannelDown2)
        {
            if (Mathf.Abs(initInptPos.y - 1400 - InputField.transform.localPosition.y) >= 1)
            {
                InputField.transform.localPosition = Vector3.Lerp(InputField.transform.localPosition, initInptPos + Vector3.down * 1400, 0.1f);
            }
            if (Mathf.Abs(initPanPos.y - 1530 - service_Pannel.transform.localPosition.y) >= 1)
            {
                service_Pannel.transform.localPosition = Vector3.Lerp(service_Pannel.transform.localPosition, initPanPos + Vector3.down * 1530, 0.1f);
            }
            else
            {
                InputField.transform.localPosition = initInptPos + Vector3.down * 1400;
                service_Pannel.transform.localPosition = initPanPos + Vector3.down * 1530;
                pannelDown2 = false;
            }
        }


        if (pannelUp2)
        {
            if (Mathf.Abs(InputField.transform.localPosition.y - initInptPos.y - 80) >= 1)
            {
                InputField.transform.localPosition = Vector3.Lerp(InputField.transform.localPosition, initInptPos + Vector3.up * 80, 0.1f);
            }
            if (Mathf.Abs(service_Pannel.transform.localPosition.y - initPanPos.y - 1789) >= 1)
            {
                service_Pannel.transform.localPosition = Vector3.Lerp(service_Pannel.transform.localPosition, initPanPos + Vector3.up * 1789, 0.1f);
            }
            else
            {
                InputField.transform.localPosition = initInptPos + Vector3.up * 80;
                service_Pannel.transform.localPosition = initPanPos + Vector3.up * 1789;
                pannelUp2 = false;
            }
        }
    }

    private Vector2 getTouchDragValue()
    {
        movePosDiff = Vector3.zero;

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                prePos = touch.position - touch.deltaPosition;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                nowPos = touch.position - touch.deltaPosition;
                movePosDiff = (Vector2)(prePos - nowPos) * Time.deltaTime;
                prePos = touch.position - touch.deltaPosition;
            }
        }
        return movePosDiff;
    }
    public void DepOkButton()
    {
        pannelState = true;
        pannelDown = true;

        SelectedPlaceTxt.text = Dep_input_Txt.text.ToString();

        Arrival.SetActive(true);

        SelectedPlace.SetActive(true);
        PreviousButton.SetActive(false);
        Dep_ConButt.SetActive(false);

        SendMessageToServer(SelectedPlaceTxt.text);
    }
    public void ArivOkButton()
    {
        pannelState = true;
        pannelDown = true;

        SelectedPlaceTxt.text = Ariv_input_Txt.text.ToString();

        NextButton.SetActive(true);
        SelectedPlace.SetActive(true);
        PreviousButton.SetActive(false);
        Ariv_ConButt.SetActive(false);

        SendMessageToServer(SelectedPlaceTxt.text);
    }
    public void SelectInputField()
    {
        pannelState = false;
        pannelUp = true;
    }
    public void NextButtonActing()
    {
        pannelState2 = true;
        pannelUp2 = true;

        NextButton.SetActive(false);
        SelectedPlace.SetActive(false);
        PreviousButton.SetActive(true);
    }
    public void PreviousButtonActing()
    {
        pannelState2 = false;
        pannelDown2 = true;

        NextButton.SetActive(true);
        SelectedPlace.SetActive(true);
        PreviousButton.SetActive(false);
    }
    private async void ConnectToServer(string serverUrl)
    {
        try
        {
            client = new SocketIOClass(serverUrl);
            client.OnConnected += (sender, e) =>
            {
                Debug.Log("Connected to server");
            };
            client.OnError += (sender, e) =>
            {
                Debug.LogError("Connection error: " + e);
            };
            await client.ConnectAsync();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Exception: " + e);
        }
    }

    private async void SendMessageToServer(string message)
    {
        if (client == null || !client.Connected)
        {
            Debug.LogError("Not connected to server");
            return;
        }

        try
        {
            await client.EmitAsync("unity_message", message);
            Debug.Log("Emitted: " + message);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Exception: " + e);
        }
    }

    private async void OnApplicationQuit()
    {
        if (client != null)
        {
            await client.DisconnectAsync();
        }
    }
}