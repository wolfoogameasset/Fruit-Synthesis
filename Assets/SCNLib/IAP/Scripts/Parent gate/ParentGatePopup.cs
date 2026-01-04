using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParentGatePopup : MonoBehaviour
{
    [SerializeField] GameObject _dialog;

    [SerializeField] Image _popUpImage;
    [SerializeField] Button _closeBtn;
    //[SerializeField] Button _closeBtnImage;
    [SerializeField] Text _calculationText;
    [SerializeField] Text _answerText;

    [SerializeField] Transform _options;

    public GameObject Dialog { get => _dialog; set => _dialog = value; }
    public Image PopUpImage { get => _popUpImage; set => _popUpImage = value; }
    public Button CloseBtn { get => _closeBtn; set => _closeBtn = value; }
    //public Button CloseBtnImage { get => _closeBtnImage; set => _closeBtnImage = value; }
    public Text CalculationText { get => _calculationText; set => _calculationText = value; }
    public Text AnswerText { get => _answerText; set => _answerText = value; }
    public Transform Options { get => _options; set => _options = value; }
}
