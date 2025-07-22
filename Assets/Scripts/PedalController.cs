using UnityEngine;
using VirtualiSurg.SmartTools;

public class PedalController : MonoBehaviour
{
    [Header("ScriptableObjects Variable")]
    [SerializeField] private Int32STSO[] potVars = new Int32STSO[2];
    [SerializeField] private BoolSTSO[]  btnVars = new BoolSTSO[2];

    [Header("Pedal Transform")]
    [SerializeField] private Transform[] pedals = new Transform[2];

     [Header("Button Transform")]
    [SerializeField] private Transform[] buttons = new Transform[2];

    [Header("Pedal Rotation Settings")]
    [SerializeField] private float angleMin = -20f;   
    [SerializeField] private float angleMax = 0f;  

     [Header("Button Push Settings")]
    [SerializeField] private float heightMin = -23f;   
    [SerializeField] private float heightMax = -31f;  

    [Header("Color Settings")]
    [SerializeField] private Color colorOff = Color.white;
    [SerializeField] private Color colorOn  = Color.red;

    // Rotation on z axis
    private Vector3[] initialEuler;

    private Vector3[] initialButtonPos;

    void Start()
    {
        // Freeze rotation on x & y axis
        initialEuler = new Vector3[pedals.Length];
        for (int i = 0; i < pedals.Length; i++)
            initialEuler[i] = pedals[i].localEulerAngles;

        // Freeze position on x & z axis
        initialButtonPos = new Vector3[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
            initialButtonPos[i] = buttons[i].localPosition;
    }

    void Update()
    {
        for (int i = 0; i < pedals.Length; i++)
        {
            int raw = potVars[i].CurrentValue;
            float t = Mathf.InverseLerp(0, 4095, raw);
            float ang = Mathf.Lerp(angleMax, angleMin, t);

            Vector3 e = initialEuler[i];
            e.x = ang;
            pedals[i].localEulerAngles = e;
        }

        for (int i = 0; i < buttons.Length; i++)
        { 
             bool pressed = btnVars[i].CurrentValue;
            Vector3 pos = initialButtonPos[i];
            pos.z = pressed ? heightMax : heightMin;
            buttons[i].localPosition = pos;

            var rend = buttons[i].GetComponentInChildren<Renderer>(true);
            if (rend != null)
            {
                rend.material.color = pressed ? colorOn : colorOff;
            }
        }
}
}
