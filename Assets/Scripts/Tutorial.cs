using Oculus.Interaction;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Step-by-step VR tutorial for the Tutorial scene (picked from the menu).
// Builds its own floating panel in front of the player, so nothing has to be set up in the scene.
// Everything runs on unscaled time because the time-stop watch sets Time.timeScale to 0.
public class Tutorial : MonoBehaviour
{
    // The tutorial starts automatically in this scene
    const string _tutorialScene = "Tutorial";
    const int _menuScene = 0;

    // Kept static so the tutorial resumes where it was after dying / resetting the map
    static int s_step;
    static string s_lastScene;

    [Header("Panel")]
    public float _panelDistance = 1.4f;
    public float _panelDrop = 0.15f;
    public float _followAngle = 35;
    public float _followSpeed = 4;

    [Header("Input")]
    public float _skipAllHoldTime = 2;

    class Step
    {
        public string _title;
        public Func<string> _body;
        public Func<bool> _done;
        public Func<Vector3?> _pointAt;
        public bool _info;
        public bool _enemiesAwake;
    }

    List<Step> _steps;
    float _stepTime;
    float _skipHold;

    bool _fired, _timeStarted, _timeEnded, _killed, _headshot;

    Transform _head;
    Transform _panel;
    TextMeshProUGUI _titleText, _bodyText, _footerText;
    Image _progressBar;
    LineRenderer _line;
    Transform _marker;
    bool _following = true;

    InputAction _next;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        SceneManager.sceneLoaded += (scene, mode) => TryStart(scene);
        TryStart(SceneManager.GetActiveScene());
    }

    static void TryStart(Scene scene)
    {
        // Coming in fresh (from the menu) starts over, reloading the tutorial scene resumes
        if (scene.name == _tutorialScene && s_lastScene != _tutorialScene) { s_step = 0; }
        s_lastScene = scene.name;

        if (scene.name != _tutorialScene || FindObjectOfType<Tutorial>() != null) { return; }

        new GameObject("Tutorial").AddComponent<Tutorial>();
    }

    private void Awake()
    {
        // B on the right controller (Enter on the keyboard for testing in the editor)
        _next = new InputAction("TutorialNext", InputActionType.Button);
        _next.AddBinding("<XRController>{RightHand}/secondaryButton");
        _next.AddBinding("<Keyboard>/enter");

        BuildSteps();
        BuildPanel();

        // Put the enemies on standby before their AI gets a chance to run
        SetEnemiesAwake(_steps.FindIndex(s => s._enemiesAwake) <= s_step);
    }

    private void OnEnable()
    {
        _next.Enable();

        Gun.OnFired += GunFired;
        TimeStop.OnStarted += TimeStarted;
        TimeStop.OnEnded += TimeEnded;
        EnemieDead.OnKilled += EnemyKilled;
    }

    private void OnDisable()
    {
        _next.Disable();

        Gun.OnFired -= GunFired;
        TimeStop.OnStarted -= TimeStarted;
        TimeStop.OnEnded -= TimeEnded;
        EnemieDead.OnKilled -= EnemyKilled;
    }

    private void OnDestroy()
    {
        _next.Dispose();

        if (_marker != null) { Destroy(_marker.gameObject); }
    }

    void GunFired(Gun gun) { _fired = true; }
    void TimeStarted(TimeStop watch) { _timeStarted = true; }
    void TimeEnded(TimeStop watch) { _timeEnded = true; }
    void EnemyKilled(EnemieDead enemy, bool headshot) { _killed = true; _headshot |= headshot; }

    private void Start()
    {
        ShowStep();
    }

    private void Update()
    {
        if (_head == null)
        {
            FindHead();
            if (_head == null) { return; }
            SnapPanel();
        }

        float _dt = Time.unscaledDeltaTime;
        _stepTime += _dt;

        FollowHead(_dt);

        Step _step = _steps[s_step];

        // Body is refreshed every frame so live values (cooldown etc.) stay correct
        _bodyText.text = _step._body();

        UpdatePointer(_step);

        // Hold B to leave the tutorial
        if (_next.IsPressed())
        {
            _skipHold += _dt;
            _progressBar.fillAmount = _skipHold / _skipAllHoldTime;

            if (_skipHold >= _skipAllHoldTime)
            {
                Finish();
                return;
            }
        }

        else
        {
            _progressBar.fillAmount = 0;
        }

        // Tap B to continue (action steps can be skipped after a short delay, in case something can't be done)
        bool _tapped = _next.WasReleasedThisFrame() && _skipHold < _skipAllHoldTime * 0.5f;
        if (!_next.IsPressed()) { _skipHold = 0; }

        bool _canSkip = _step._info ? _stepTime > 0.5f : _stepTime > 4;

        if ((_tapped && _canSkip) || (!_step._info && _step._done()))
        {
            NextStep();
        }
    }

    void NextStep()
    {
        s_step++;

        if (s_step >= _steps.Count)
        {
            Finish();
            return;
        }

        ShowStep();
    }

    void ShowStep()
    {
        Step _step = _steps[s_step];

        _stepTime = 0;
        _fired = _timeStarted = _timeEnded = _killed = false;

        _titleText.text = _step._title;
        _bodyText.text = _step._body();

        string _progress = $"{s_step + 1}/{_steps.Count}";
        _footerText.text = _step._info
            ? $"{_progress}   <b>B</b> continue   ·   hold <b>B</b> to leave tutorial"
            : $"{_progress}   do it to continue   ·   hold <b>B</b> to leave tutorial";

        // Pull the panel back into view when a new step appears
        _following = true;

        // Enemies stay passive until the combat part, so you can read in peace
        SetEnemiesAwake(_steps.FindIndex(s => s._enemiesAwake) <= s_step);

        // A small buzz so the player notices the new step
        StartCoroutine(Buzz());
    }

    System.Collections.IEnumerator Buzz()
    {
        OVRInput.SetControllerVibration(0.3f, 0.2f, OVRInput.Controller.RTouch);
        yield return new WaitForSecondsRealtime(0.1f);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }

    void SetEnemiesAwake(bool awake)
    {
        foreach (EnemieAI _ai in FindObjectsOfType<EnemieAI>(true))
        {
            _ai.enabled = awake;
        }
    }

    // Done or skipped: back to the menu
    void Finish()
    {
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        Time.timeScale = 1;
        SceneManager.LoadScene(_menuScene);
        enabled = false;
        Destroy(_panel.gameObject);
        Destroy(gameObject);
    }

    // ---------------------------------------------------------------- steps

    void BuildSteps()
    {
        _steps = new List<Step>
        {
            new Step
            {
                _title = "WELCOME TO TRAINING",
                _info = true,
                _body = () =>
                    "This course teaches you the three things that keep you alive:\n" +
                    "<color=#66E6FF>your weapons</color>, <color=#66E6FF>your time-stop watch</color> and <color=#FF6666>the enemies</color>.\n\n" +
                    "<b>Left stick</b>  move\n" +
                    "<b>Right stick</b>  turn",
            },

            // ---------------- weapons
            new Step
            {
                _title = "GRAB A WEAPON",
                _body = () =>
                    "Reach for a weapon and squeeze the <b>GRIP</b> button to pick it up.\n" +
                    "Keep squeezing to hold on to it.\n\n" +
                    "You can hold a weapon in <b>each hand</b>.",
                _done = HoldingWeapon,
                _pointAt = NearestFreeWeapon,
            },
            new Step
            {
                _title = "SHOOT",
                _body = () =>
                    "Aim and pull the <b>TRIGGER</b> to fire.\n\n" +
                    "Your controller rumbles with every shot.\n" +
                    "Holding the saber? Grab a gun for this one.",
                _done = () => _fired,
            },
            new Step
            {
                _title = "AMMO",
                _info = true,
                _body = () =>
                    "Every gun has a limited magazine.\n\n" +
                    "When it's empty the gun <color=#FF6666>glitches red</color> and won't fire.\n" +
                    "It reloads itself after <b>5 seconds</b>, or press <b>A</b> to reload right away.",
            },
            new Step
            {
                _title = "KNOW YOUR WEAPONS",
                _info = true,
                _body = () =>
                    "<b>Pistol</b>  one precise shot per trigger pull.\n" +
                    "<b>AK</b>  automatic, hold the trigger to keep firing.\n" +
                    "<b>Shotgun</b>  a wide spread of pellets, deadly up close.\n" +
                    "<b>Sniper</b>  look through the scope for long shots.\n" +
                    "<b>Saber</b>  no ammo. Swing the blade through an enemy.",
            },

            // ---------------- time stop watch
            new Step
            {
                _title = "YOUR TIME-STOP WATCH",
                _info = true,
                _body = () =>
                    "Look at the watch on your wrist.\n\n" +
                    "<color=#66E6FF>Cyan glow</color>  charged and ready to use.\n" +
                    "<color=#FF6666>Red flicker</color>  " + RechargeText(),
                _pointAt = WatchPosition,
            },
            new Step
            {
                _title = "STOP TIME",
                _body = () =>
                    "Touch the watch with your <b>other hand</b> to stop time.\n\n" +
                    (WatchReady() ? "The watch is charged. Try it!" : "<color=#FF6666>Recharging...</color> wait for the cyan glow."),
                _done = () => _timeStarted,
                _pointAt = WatchPosition,
            },
            new Step
            {
                _title = "TIME IS FROZEN",
                _body = () =>
                    $"You have <b>{ActiveTime():0} seconds</b>. Enemies and their bullets are frozen.\n\n" +
                    "You can still move and shoot, but <b>your bullets freeze too</b>.\n" +
                    "They hang in the air and all fly at once when time starts again.\n\n" +
                    "The screen pulses faster as time runs out.",
                _done = () => _timeEnded || Time.timeScale > 0,
            },

            // ---------------- enemies
            new Step
            {
                _title = "ENEMIES",
                _info = true,
                _body = () =>
                    "Enemies patrol until you get within about " + $"<b>{EnemyRange():0} m</b>.\n" +
                    "Then they turn towards you, aim and shoot.\n\n" +
                    "<color=#FF6666>One bullet kills you</color>, and one bullet kills them.\n" +
                    "Shoot or slice them first. Aim for the <b>head</b> for a headshot.\n\n" +
                    "They are on standby for now. That changes on the next step.",
                _pointAt = NearestEnemy,
            },
            new Step
            {
                _title = "TAKE ONE DOWN",
                _enemiesAwake = true,
                _body = () =>
                    "The enemies are <color=#FF6666>awake</color>. Defeat one!\n\n" +
                    "Tip: get close, <b>stop time</b>, line up your shots, and let time run again.\n" +
                    "Defeated enemies drop their weapon. Grab it!",
                _done = () => _killed || NearestEnemy() == null,
                _pointAt = NearestEnemy,
            },

            new Step
            {
                _title = "TRAINING COMPLETE",
                _info = true,
                _body = () =>
                    (_headshot ? "Headshot! " : "") + "You're ready.\n\n" +
                    "Remember: grab a weapon, stop time when it gets busy,\n" +
                    "and never let an enemy get the first shot.\n\n" +
                    "Press <b>B</b> to return to the menu.\n" +
                    "Want more practice? Pick <b>Training ground</b> there.",
            },
        };

        s_step = Mathf.Clamp(s_step, 0, _steps.Count - 1);
    }

    // ---------------------------------------------------------------- game queries

    bool HoldingWeapon()
    {
        foreach (Gun _gun in FindObjectsOfType<Gun>())
        {
            if (_gun._holdingGun) { return true; }
        }

        foreach (Katana _saber in FindObjectsOfType<Katana>())
        {
            Grabbable _grab = _saber.GetComponentInParent<Grabbable>();
            if (_grab != null && _grab._holding) { return true; }
        }

        return false;
    }

    Vector3? NearestFreeWeapon()
    {
        Vector3? _best = null;
        float _bestDist = float.MaxValue;

        foreach (Gun _gun in FindObjectsOfType<Gun>())
        {
            if (_gun._holdingGun) { continue; }

            float _d = Vector3.Distance(_head.position, _gun.transform.position);
            if (_d < _bestDist) { _bestDist = _d; _best = _gun.transform.position; }
        }

        return _best;
    }

    TimeStop Watch()
    {
        TimeStop _best = null;
        float _bestDist = float.MaxValue;

        foreach (TimeStop _watch in FindObjectsOfType<TimeStop>())
        {
            float _d = Vector3.Distance(_head.position, _watch.transform.position);
            if (_d < _bestDist) { _bestDist = _d; _best = _watch; }
        }

        return _best;
    }

    Vector3? WatchPosition()
    {
        TimeStop _watch = Watch();
        return _watch != null ? _watch.transform.position : (Vector3?)null;
    }

    bool WatchReady()
    {
        TimeStop _watch = Watch();
        return _watch == null || _watch._canPress;
    }

    float ActiveTime()
    {
        TimeStop _watch = Watch();
        return _watch != null ? _watch._timeStopActiveTime : 5;
    }

    string RechargeText()
    {
        TimeStop _watch = Watch();

        if (_watch == null || _watch._coolDown <= 0)
        {
            return "recharging. (No cooldown in training!)";
        }

        return $"recharging. It takes <b>{_watch._coolDown:0} seconds</b> after each use.";
    }

    float EnemyRange()
    {
        EnemieAI _ai = FindObjectOfType<EnemieAI>();
        return _ai != null ? _ai._dist : 8;
    }

    Vector3? NearestEnemy()
    {
        Vector3? _best = null;
        float _bestDist = float.MaxValue;

        foreach (EnemieDead _enemy in FindObjectsOfType<EnemieDead>())
        {
            float _d = Vector3.Distance(_head.position, _enemy.transform.position);
            if (_d < _bestDist) { _bestDist = _d; _best = _enemy.transform.position + Vector3.up * 1.2f; }
        }

        return _best;
    }

    // ---------------------------------------------------------------- panel

    void FindHead()
    {
        // The OVR rig's centre eye is the player's head
        GameObject _eye = GameObject.Find("CenterEyeAnchor");
        if (_eye != null)
        {
            _head = _eye.transform;
            return;
        }

        if (Camera.main != null) { _head = Camera.main.transform; }
    }

    Vector3 PanelTarget()
    {
        Vector3 _forward = Vector3.ProjectOnPlane(_head.forward, Vector3.up).normalized;
        if (_forward == Vector3.zero) { _forward = _head.up; }

        return _head.position + _forward * _panelDistance - Vector3.up * _panelDrop;
    }

    void SnapPanel()
    {
        _panel.position = PanelTarget();
        _panel.rotation = Quaternion.LookRotation(_panel.position - _head.position);
    }

    // Lazy follow: the panel stays put until you look away from it, then glides back in front of you
    void FollowHead(float dt)
    {
        Vector3 _target = PanelTarget();

        Vector3 _toPanel = Vector3.ProjectOnPlane(_panel.position - _head.position, Vector3.up);
        Vector3 _forward = Vector3.ProjectOnPlane(_head.forward, Vector3.up);

        if (Vector3.Angle(_toPanel, _forward) > _followAngle || Vector3.Distance(_panel.position, _target) > 1)
        {
            _following = true;
        }

        if (_following)
        {
            _panel.position = Vector3.Lerp(_panel.position, _target, dt * _followSpeed);

            if (Vector3.Distance(_panel.position, _target) < 0.02f) { _following = false; }
        }

        _panel.rotation = Quaternion.Slerp(_panel.rotation, Quaternion.LookRotation(_panel.position - _head.position), dt * _followSpeed * 2);
    }

    void UpdatePointer(Step step)
    {
        Vector3? _target = step._pointAt != null ? step._pointAt() : null;

        _line.enabled = _target.HasValue;
        _marker.gameObject.SetActive(_target.HasValue);

        if (!_target.HasValue) { return; }

        Vector3 _start = _panel.position - _panel.up * 0.29f;
        _line.SetPosition(0, _start);
        _line.SetPosition(1, _target.Value);

        float _pulse = 0.04f + Mathf.Sin(Time.unscaledTime * 6) * 0.01f;
        _marker.position = _target.Value;
        _marker.localScale = Vector3.one * _pulse;
    }

    void BuildPanel()
    {
        Color _cyan = new Color(0.4f, 0.9f, 1f);

        // Canvas: 1 unit = 1 mm
        GameObject _canvasObj = new GameObject("Tutorial Panel", typeof(RectTransform), typeof(Canvas));
        _panel = _canvasObj.transform;

        Canvas _canvas = _canvasObj.GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.WorldSpace;
        _canvas.sortingOrder = 100;

        RectTransform _rect = (RectTransform)_panel;
        _rect.sizeDelta = new Vector2(900, 560);
        _rect.localScale = Vector3.one * 0.001f;

        Image _background = CreateImage("Background", _panel, new Color(0.03f, 0.05f, 0.08f, 0.88f));
        Stretch(_background.rectTransform, 0, 0, 0, 0);

        Image _topBar = CreateImage("Top Bar", _panel, _cyan);
        Stretch(_topBar.rectTransform, 0, 0, 0, 552);

        _progressBar = CreateImage("Skip Bar", _panel, _cyan);
        Stretch(_progressBar.rectTransform, 0, 0, 552, 0);
        _progressBar.type = Image.Type.Filled;
        _progressBar.fillMethod = Image.FillMethod.Horizontal;
        _progressBar.fillAmount = 0;
        _progressBar.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.one * 0.5f);

        _titleText = CreateText("Title", _panel, 46, FontStyles.Bold, _cyan);
        Stretch(_titleText.rectTransform, 40, 40, 30, 450);

        _bodyText = CreateText("Body", _panel, 32, FontStyles.Normal, Color.white);
        Stretch(_bodyText.rectTransform, 40, 40, 115, 80);
        _bodyText.alignment = TextAlignmentOptions.TopLeft;

        _footerText = CreateText("Footer", _panel, 24, FontStyles.Normal, new Color(1, 1, 1, 0.6f));
        Stretch(_footerText.rectTransform, 40, 40, 490, 20);
        _footerText.alignment = TextAlignmentOptions.BottomLeft;

        // Pointer line from the panel to whatever the step is about
        Material _lineMat = new Material(Shader.Find("Sprites/Default"));

        _line = _canvasObj.AddComponent<LineRenderer>();
        _line.material = _lineMat;
        _line.useWorldSpace = true;
        _line.positionCount = 2;
        _line.startWidth = 0.004f;
        _line.endWidth = 0.01f;
        _line.startColor = new Color(_cyan.r, _cyan.g, _cyan.b, 0.2f);
        _line.endColor = _cyan;
        _line.enabled = false;

        GameObject _markerObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _markerObj.name = "Tutorial Marker";
        DestroyImmediate(_markerObj.GetComponent<Collider>());
        _markerObj.GetComponent<Renderer>().material = _lineMat;
        _markerObj.GetComponent<Renderer>().material.color = _cyan;
        _markerObj.SetActive(false);
        _marker = _markerObj.transform;
    }

    Image CreateImage(string name, Transform parent, Color color)
    {
        GameObject _obj = new GameObject(name, typeof(RectTransform), typeof(Image));
        _obj.transform.SetParent(parent, false);

        Image _image = _obj.GetComponent<Image>();
        _image.color = color;
        _image.raycastTarget = false;
        return _image;
    }

    TextMeshProUGUI CreateText(string name, Transform parent, float size, FontStyles style, Color color)
    {
        GameObject _obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        _obj.transform.SetParent(parent, false);

        TextMeshProUGUI _text = _obj.GetComponent<TextMeshProUGUI>();
        _text.fontSize = size;
        _text.fontStyle = style;
        _text.color = color;
        _text.enableWordWrapping = true;
        _text.raycastTarget = false;
        return _text;
    }

    // Offsets in canvas units from each edge
    void Stretch(RectTransform rect, float left, float right, float top, float bottom)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
    }
}
