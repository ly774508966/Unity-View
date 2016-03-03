using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityView.Component;

namespace UnityView
{
    public class ButtonView : UIView
    {
        public readonly Button Button;

        public Text TitleTextView;

        public string Title
        {
            set
            {
                if (TitleTextView == null)
                {
                    var text = TextView.BaseTextView();
                    TitleTextView = text.GetComponent<Text>();
                    text.transform.SetParent(RectTransform);
                    RectFill(text.GetComponent<RectTransform>());
                }
                TitleTextView.text = value;
            }
            get
            {
                return TitleTextView == null ? null : TitleTextView.text;
            }
        }

        public float FontSize
        {
            set
            {
                TitleTextView.fontSize = Mathf.RoundToInt((value * UIConstant.FontCoefficient));
            }
            get
            {
                return TitleTextView == null ? 0 : TitleTextView.fontSize / UIConstant.FontCoefficient;
            }
        }

        public Button.ButtonClickedEvent OnClickListener
        {
            get
            {
                return Button.onClick;
            }
        }

        public ButtonView()
            : this(UIViewManager.GetInstance().RootView) { }

        public ButtonView(UILayout layout)
            : base(layout)
        {
            Button = UIObject.AddComponent<Button>();
        }

        public ButtonView(GameObject gameObject)
            : base(gameObject)
        {
            Button = gameObject.GetComponent<Button>() ?? gameObject.AddComponent<Button>();
        }

        public static GameObject BaseButton()
        {
            var gameObject = BaseView();
            gameObject.AddComponent<Image>();
            gameObject.AddComponent<Button>();
            return gameObject;
        }
    }

    public class RadioGroup
    {
        private readonly List<RadioButtonView> _buttons = new List<RadioButtonView>();

        public void AddRadioButton(RadioButtonView radioButton)
        {
            if (_buttons.Contains(radioButton)) return;
            _buttons.Add(radioButton);
            radioButton.RadioGroup = this;
        }
        public void RemoveRadioButton(RadioButtonView radioButton)
        {
            _buttons.Remove(radioButton);
            radioButton.RadioGroup = null;
        }

        public void Notify(RadioButtonView radioButton)
        {
            if (!_buttons.Contains(radioButton)) return;
            foreach (var radioButtonView in _buttons)
            {
                if (radioButtonView == radioButton)
                {
                    if (!radioButton.StateOn)
                    {
                        radioButton.StateOn = true;
                    }
                }
                else
                {
                    radioButtonView.StateOn = false;
                }
            }
        }

        public void Select(RadioButtonView radioButton)
        {
            if (!_buttons.Contains(radioButton)) return;
            if (radioButton.StateOn) return;
            foreach (var radioButtonView in _buttons)
            {
                if (radioButtonView == radioButton) continue;
                radioButtonView.StateOn = false;
            }
            radioButton.StateOn = true;
        }
    }
    public class RadioButtonView : ButtonView
    {
        private bool _stateOn;

        public Sprite ImageOn { set; get; }
        public Sprite ImageOff { set; get; }
        public string TitleOn { set; get; }
        public string TitleOff { set; get; }
        public bool StateOn
        {
            set
            {
                _stateOn = value;
                if (value)
                {
                    if (ImageOn != null)
                    {
                        Sprite = ImageOn;
                    }
                    if (TitleOn != null)
                    {
                        Title = TitleOn;
                    }
                    if (RadioGroup != null)
                    {
                        RadioGroup.Notify(this);
                    }
                }
                else
                {
                    if (ImageOff != null)
                    {
                        Sprite = ImageOff;
                    }
                    if (TitleOff != null)
                    {
                        Title = TitleOff;
                    }
                }
            }
            get
            {
                return _stateOn;
            }
        }

        public RadioGroup RadioGroup { set; get; }

        public bool Switch()
        {
            StateOn = !StateOn;
            return StateOn;
        }

        public RadioButtonView() { }
        public RadioButtonView(UILayout layout) : base(layout) { }
        public RadioButtonView(GameObject gameObject) : base(gameObject) { }
    }
}
