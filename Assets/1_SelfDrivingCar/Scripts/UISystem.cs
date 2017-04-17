using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.SceneManagement;

public class UISystem : MonoSingleton<UISystem>
{
  public CarController carController;
  public string GoodCarStatusMessage;
  public string BadSCartatusMessage;
  public Text MPH_Text;
  public Image MPH_Animation;
  public Text Angle_Text;
  public Text RecordStatus_Text;
  public Text DriveStatus_Text;
  public Text SaveStatus_Text;
  public GameObject RecordingPause;
  public GameObject RecordDisabled;
  public bool isTraining = false;

  private bool recording;
  private float topSpeed;
  private bool saveRecording;


  // Use this for initialization
  void Start()
  {
    Debug.Log(isTraining);
    topSpeed = carController.MaxSpeed;
    recording = false;
    RecordingPause.SetActive(false);
    RecordStatus_Text.text = "RECORD";
    DriveStatus_Text.text = "";
    SaveStatus_Text.text = "";
    SetAngleValue(0);
    SetMPHValue(0);
    if (!isTraining)
    {
      DriveStatus_Text.text = "Mode: Autonomous";
      RecordDisabled.SetActive(true);
      RecordStatus_Text.text = "";
    }
  }

  public void SetAngleValue(float value)
  {
    Angle_Text.text = value.ToString("N2") + "°";
  }

  public void SetMPHValue(float value)
  {
    MPH_Text.text = value.ToString("N2");
    //Do something with value for fill amounts
    MPH_Animation.fillAmount = value / topSpeed;
  }

  public void ToggleRecording(Heading heading = Heading.Any)
  {
    // Don't record in autonomous mode
    if (!isTraining)
    {
      return;
    }

    if (!recording)
    {
      if (carController.checkSaveLocation())
      {
        recording = true;
        RecordingPause.SetActive(true);
        RecordStatus_Text.text = "RECORDING HEADING: " + heading;

        carController.Heading = heading;
        carController.IsRecording = true;
      }
    }
    else
    {
      saveRecording = true;
      carController.IsRecording = false;
    }
  }

  void UpdateCarValues()
  {
    SetMPHValue(carController.CurrentSpeed);
    SetAngleValue(carController.CurrentSteerAngle);
  }

  // Update is called once per frame
  void Update()
  {

    // Easier than pressing the actual button :-)
    // Should make recording training data more pleasant.

    if (carController.getSaveStatus())
    {
      SaveStatus_Text.text = "Capturing Data: " + (int)(100 * carController.getSavePercent()) + "%";
      //Debug.Log ("save percent is: " + carController.getSavePercent ());
    }
    else if (saveRecording)
    {
      SaveStatus_Text.text = "";
      recording = false;
      RecordingPause.SetActive(false);
      RecordStatus_Text.text = "RECORD";
      saveRecording = false;
    }

    // Records always
    if (Input.GetKeyDown(KeyCode.R))
    {
      ToggleRecording();
    }
    // Records only if we're driving a right curve
    else if (Input.GetKeyDown(KeyCode.L))
    {
      ToggleRecording(Heading.RightCurve);
    }
    // Records only if we're driving a left curve
    else if (Input.GetKeyDown(KeyCode.T))
    {
      ToggleRecording(Heading.LeftCurve);
    }
    // Record only if we're driving straight ahead
    else if (Input.GetKeyDown(KeyCode.B))
    {
      ToggleRecording(Heading.Straight);
    }


    if (!isTraining)
    {
      if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S)))
      {
        DriveStatus_Text.color = Color.red;
        DriveStatus_Text.text = "Mode: Manual";
      }
      else
      {
        DriveStatus_Text.color = Color.white;
        DriveStatus_Text.text = "Mode: Autonomous";
      }
    }

    if (Input.GetKeyDown(KeyCode.Escape))
    {
      //Do Menu Here
      SceneManager.LoadScene("MenuScene");
    }

    if (Input.GetKeyDown(KeyCode.Return))
    {
      //Do Console Here
    }

    UpdateCarValues();
  }
}
