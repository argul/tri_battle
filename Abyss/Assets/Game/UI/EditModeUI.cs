using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EditModeUI : MonoBehaviour 
{
	public float hintDuration = 3f;
	public EditMode2DR mode;
	public GameObject editCanvasUIRoot;
	public GameObject editTraitUIRoot;
	public GameObject editMatchRuleUIRoot;
	public GameObject editOperationRuleUIRoot;

	public Dropdown dpdMode;
	public Button btnPrevMatch;
	public Button btnNextMatch;
	public Button btnNewMatch;
	public Button btnSaveMatch;
	public Button btnDeleteMatch;

	public Button btnPrevOperation;
	public Button btnNextOperation;
	public Button btnNewOperation;
	public Button btnSaveOperation;
	public Button btnDeleteOperation;

	public Button btnAddWidth;
	public Button btnSubWidth;
	public Button btnAddHeight;
	public Button btnSubHeight;
	public Text txtCanvasWidth;
	public Text txtCanvasHeight;
	public Button btnApplyCanvas;
	public Button btnRevertCanvas;

	public Text txtHint;

	private List<Button> buttons;
	void Start () 
	{
		OnEditModeChanged(dpdMode.value);

		btnPrevMatch.onClick.AddListener(()=>{
			mode.PrevMatchRule();
			SyncMatchRuleUI();
		});
		btnNextMatch.onClick.AddListener(()=>{
			mode.NextMatchRule();
			SyncMatchRuleUI();
		});
		btnNewMatch.onClick.AddListener(()=>{
			mode.NewMatchRule();
			SyncMatchRuleUI();
		});
		btnSaveMatch.onClick.AddListener(()=>{
			mode.SaveMatchRule();
		});
		btnDeleteMatch.onClick.AddListener(()=>{
			mode.DeleteMatchRule();
			SyncMatchRuleUI();
		});

		btnPrevOperation.onClick.AddListener(()=>{
			mode.PrevOperationRule();
			SyncOperationRuleUI();
		});
		btnNextOperation.onClick.AddListener(()=>{
			mode.NextOperationRule();
			SyncOperationRuleUI();
		});
		btnNewOperation.onClick.AddListener(()=>{
			mode.NewOperationRule();
			SyncOperationRuleUI();
		});
		btnSaveOperation.onClick.AddListener(()=>{
			mode.SaveOperationRule();
		});
		btnDeleteOperation.onClick.AddListener(()=>{
			mode.DeleteOperationRule();
			SyncOperationRuleUI();
		});

		btnAddWidth.onClick.AddListener(()=>{
			mode.ModifyCanvasSize(1, 0);	
		});
		btnSubWidth.onClick.AddListener(()=>{
			mode.ModifyCanvasSize(-1, 0);
		});
		btnAddHeight.onClick.AddListener(()=>{
			mode.ModifyCanvasSize(0, 1);
		});
		btnSubHeight.onClick.AddListener(()=>{
			mode.ModifyCanvasSize(0, -1);
		});
		btnApplyCanvas.onClick.AddListener(()=>{
			mode.ApplyCanvas();
		});
		btnRevertCanvas.onClick.AddListener(()=>{
			mode.RevertCanvas();
		});

		buttons = new List<Button>()
		{
			btnPrevMatch, btnNextMatch, btnNewMatch, btnSaveMatch, btnDeleteMatch,
			btnPrevOperation, btnNextOperation, btnNewOperation, btnSaveOperation, btnDeleteOperation,
			btnAddWidth, btnSubWidth, btnAddHeight, btnSubHeight, btnApplyCanvas, btnRevertCanvas
		};
	}

	void OnDestroy()
	{
		foreach (var btn in buttons)
		{
			if (null != btn) btn.onClick.RemoveAllListeners();
		}
	}

	private bool isHintShowing = false;
	private float hideHintAt = 0f;
	void Update () 
	{
		if (isHintShowing && Time.time > hideHintAt)
		{
			txtHint.text = "";
			isHintShowing = false;
		}
	}

	public void ShowHintText(string str)
	{
		txtHint.text = str;
		isHintShowing = true;
		hideHintAt = Time.time + hintDuration;
	}

	public void OnBackClick()
	{
		Game.CancelEditingScheme();
		Application.LoadLevel("scn_main");
	}

	public void OnDoneClick()
	{
		string reason = "";
		if (PlayableSchemeCensor.CheckScheme(Game.globalEdit.editingScheme, out reason))
		{
			Game.ConfirmEditingScheme();
			Application.LoadLevel("scn_main");
		}
		else
		{
			ShowHintText(reason);
		}
	}

	public void OnEditModeChanged(int idx)
	{
		switch (idx)
		{
		case 0:
		{
			UnityExtension.SetActive(true, editCanvasUIRoot);
			UnityExtension.SetActiveBatch(false, editTraitUIRoot, editMatchRuleUIRoot, editOperationRuleUIRoot);
			mode.StartEditCanvas();
		}
			break;
		case 1:
		{
			UnityExtension.SetActive(true, editTraitUIRoot);
			UnityExtension.SetActiveBatch(false, editCanvasUIRoot, editMatchRuleUIRoot, editOperationRuleUIRoot);
			mode.StartEditTraits();
		}
			break;
		case 2:
		{
			UnityExtension.SetActive(true, editMatchRuleUIRoot);
			UnityExtension.SetActiveBatch(false, editTraitUIRoot, editCanvasUIRoot, editOperationRuleUIRoot);
			mode.StartEditMatchRules();
			SyncMatchRuleUI();
		}
			break;
		case 3:
		{
			UnityExtension.SetActive(true, editOperationRuleUIRoot);
			UnityExtension.SetActiveBatch(false, editTraitUIRoot, editMatchRuleUIRoot, editCanvasUIRoot);
			mode.StartEditOperationRules();
			SyncOperationRuleUI();
		}
			break;
		default:
			throw new System.NotSupportedException();
		}
	}

	public void OnInputNameText(string txt)
	{
		if (string.IsNullOrEmpty(txt))
		{
			txtHint.text = Game.globalEdit.editingScheme.Name;
			ShowHintText("Illegeal scheme name");
		}
		else if (txt.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
		{
			txtHint.text = Game.globalEdit.editingScheme.Name;
			ShowHintText("Illegeal scheme name");
		}
		else
		{
			Game.globalEdit.editingScheme.Name = txt;
		}
	}

	private void SyncMatchRuleUI()
	{
		btnPrevMatch.gameObject.SetActive(mode.HasPrevMatchRule);
		btnNextMatch.gameObject.SetActive(mode.HasNextMatchRule);
		btnNewMatch.gameObject.SetActive(mode.HasNewMatchRule);
	}

	private void SyncOperationRuleUI()
	{
		btnPrevOperation.gameObject.SetActive(mode.HasPrevOperationRule);
		btnNextOperation.gameObject.SetActive(mode.HasNextOperationRule);
		btnNewOperation.gameObject.SetActive(mode.HasNewOperationRule);
	}
}
