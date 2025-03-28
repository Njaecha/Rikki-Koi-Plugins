﻿using ChaCustom;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PseudoMaker.UI
{
    public class PickerComponent : MonoBehaviour
    {
        private Text text;
        private Button pickerButton;
        private Text pickerText;
        private Image thumbnail;

        public string Name;
        public ChaListDefine.CategoryNo CategoryNo;
        public Func<string> GetId;
        public Func<int> GetCurrentValue;
        public Action<CustomSelectInfo> SetCurrentValue;

        private void Awake()
        {
            text = GetComponentInChildren<Text>(true);

            pickerButton = transform.Find("PickerButton").gameObject.GetComponent<Button>();
            pickerButton.onClick.AddListener(() =>
            {
                PickerPanel.SetCategory(GetId(), Name, CategoryNo, GetCurrentValue, (info) =>
                {
                    pickerText.text = info.name;
                    thumbnail.sprite = PickerPanel.GetThumbSprite(info);
                    SetCurrentValue(info);
                });
            });

            pickerText = pickerButton.GetComponentInChildren<Text>(true);

            thumbnail = pickerButton.transform.Find("Image").gameObject.GetComponent<Image>();
        }

        private void Start()
        {
            text.text = Name;
        }

        private void OnEnable()
        {
            if (GetCurrentValue == null)
                return;

            var current = PickerPanel.dictSelectInfo[CategoryNo].FirstOrDefault(x => x.index == GetCurrentValue());
            if (current != null)
            {
                pickerText.text = current.name;
                thumbnail.sprite = PickerPanel.GetThumbSprite(current);
            }
        }
    }
}
