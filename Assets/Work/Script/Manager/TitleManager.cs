using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RaindowStudio.DesignPattern;
using RaindowStudio.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TitleManager : Processor<TitleManager, TitleState>
{
    [SerializeField] private PlayableDirector pd_tapToStart;
    [SerializeField] private CharacterPreview characterPreview;
    [SerializeField] private Slider sld_titlePatch;
    [SerializeField] private TextMeshProUGUI txt_titlePatch;
    [SerializeField] private GameObject tapToStart;
    [SerializeField] private GameObject titlePatch;

    public void ChangeGameManagerState(string state)
    {        
        GameManager.Instance.ChangeStateByString(state);
    }

    public void InitializePatchUI(string text)
    {
        txt_titlePatch.SetText($"Downloading '{text}'...");
        sld_titlePatch.value = 0;
    }

    private void OnCharacterChangedEvent(string id)
    {
        characterPreview.Initialize();
    }

    void DeActivate_Intro()
    {
        pd_tapToStart.Play();
    }

    protected override void Initialization()
    {
        base.Initialization();
        
        State = TitleState.Intro;
        GameManager.CharacterChangedEvent += OnCharacterChangedEvent;
        
        // Start patching for basic data.
        AddressableManager am = AddressableManager.Instance;
        this.WaitUntilToDo(() => am.Initialized, () =>
        {
            InitializePatchUI("Character Resources");
            am.LoadAssetsByLabel<CharacterDataSet>(AddressableManager.LABEL_GLOBAL,
                a => am.Character.Add(a.ID, a),
                d => sld_titlePatch.value = d.PercentComplete,
                c =>
                {
                    GameManager.Instance.LoadSaveData();

                    InitializePatchUI("UI Resources");

                    am.LoadAssetsByLabel<UIData>(AddressableManager.LABEL_GLOBAL,
                        aa => aa.UIDataList.ForEach(ds => am.UI[ds.id] = ds.sprite),
                        dd => sld_titlePatch.value = dd.PercentComplete,
                        _ =>
                        {
                            sld_titlePatch.value = 1;
                            titlePatch.GetComponent<CanvasGroup>().DOFade(0, .5f);
                            this.DelayToDo(.5f, () => titlePatch.SetActive(false));
                            tapToStart.SetActive(true);
                        });
                });
        });
    }

    private void OnDestroy()
    {
        GameManager.CharacterChangedEvent -= OnCharacterChangedEvent;
    }
}

[Serializable]
public enum TitleState
{
    Intro = -1,
    Shop = 0,
    Summon = 1,
    Main = 2,
    Pvp = 3,
    Account = 4
}
