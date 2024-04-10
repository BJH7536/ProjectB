using System;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(Rigidbody2D))][RequireComponent(typeof(CapsuleCollider2D))][RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    // Player의 컨트롤은 State에 기반해 이를 변경하는 방식으로 구현.
    // 현재 Player의 State는 Idle, Walk, Run, Jump, Fall, Attack 총 6개의 State로 구분.
    // 새로운 State를 추가할 때는 IPlayerState 인터페이스를 상속받는 클래스를 정의하고,
    // PlayerController가 해당 클래스의 인스턴스를 들고있도록 수정하면 바로 적용할 수 있다.

    public interface IPlayerState
    {
        void Enter(PlayerController player);
        void Execute();
        void Exit();
    }

    public class PlayerIdleState : IPlayerState
    {
        private PlayerController _playerController;
        public void Enter(PlayerController player)
        {
            _playerController = player;
        }

        public void Execute()
        {
            if (_playerController.inputVector.x == 0) return;
            _playerController.CurrentState = _playerController.shiftToggled ? _playerController._runState : _playerController._walkState;
        }

        public void Exit()
        {

        }
    }

    public class PlayerWalkState : IPlayerState
    {
        private PlayerController _playerController;
        public void Enter(PlayerController player)
        {
            _playerController = player;
        }

        public void Execute()
        {
            // 플레이어의 입력에 따라 캐릭터 이동
            _playerController._rigidbody2D.velocity = DialogueManager.GetInstance().dialogueIsPlaying ? Vector2.zero :
                new Vector2(_playerController.inputVector.x * _playerController.speed,
                    _playerController._rigidbody2D.velocity.y);

            _playerController.FlipImage();
        }

        public void Exit()
        {

        }
    }

    public class PlayerRunState : IPlayerState
    {
        private PlayerController _playerController;
        public void Enter(PlayerController player)
        {
            _playerController = player;
        }

        public void Execute()
        {
            // 플레이어의 입력에 따라 캐릭터 이동
            _playerController._rigidbody2D.velocity = DialogueManager.GetInstance().dialogueIsPlaying ? Vector2.zero :
                new Vector2(_playerController.inputVector.x * _playerController.runningSpeed,
                    _playerController._rigidbody2D.velocity.y);

            _playerController.FlipImage();
        }

        public void Exit()
        {

        }
    }

    public class PlayerJumpState : IPlayerState
    {
        private PlayerController _playerController;
        public void Enter(PlayerController player)
        {
            _playerController = player;

            if (_playerController.jumpForced) return;
            else if (DialogueManager.GetInstance().dialogueIsPlaying) return;
            _playerController._rigidbody2D.velocity = new Vector2(_playerController._rigidbody2D.velocity.x, 0);
            _playerController._rigidbody2D.AddForce(Vector2.up * _playerController.jumpPower, ForceMode2D.Impulse);

            _playerController.jumpForced = true;
        }

        public void Execute()
        {
            float speed = (_playerController.shiftToggled) ? _playerController.runningSpeed : _playerController.speed;

            // 플레이어의 입력에 따라 캐릭터 이동
            _playerController._rigidbody2D.velocity =
                new Vector2(_playerController.inputVector.x * speed,
                    _playerController._rigidbody2D.velocity.y);

            _playerController.FlipImage();
        }

        public void Exit()
        {

        }
    }

    public class PlayerFallState : IPlayerState
    {
        private PlayerController _playerController;
        public void Enter(PlayerController player)
        {
            _playerController = player;
        }

        public void Execute()
        {

        }

        public void Exit()
        {

        }
    }

    public class PlayerAttackState : IPlayerState
    {
        private PlayerController _playerController;
        public void Enter(PlayerController player)
        {
            _playerController = player;
        }

        public void Execute()
        {

        }

        public void Exit()
        {

        }
    }

    [Tab("Information")]
    [SerializeField] private Vector2 inputVector;
    [SerializeField] private float speed = 400f;
    [SerializeField] private float runningSpeed = 700f;
    [SerializeField] private float jumpPower = 1800f;
    [SerializeField] private bool isJumpable = false;
    [SerializeField] private bool jumpForced = false;
    [SerializeField] private bool shiftToggled = false;
    [SerializeField] private IPlayerState _currentState;
    [SerializeField] private Sprite[] playerPortrait;
    [Tab("Quest & Dialogue")]
    [SerializeField] private QuestManager _questManager;
    [SerializeField] public bool isQuestPanelActive;
    private bool interactPressed;
    private static PlayerController instance;
    public static PlayerController GetInstance() { return instance; }
    public Sprite getplayerPortrait(int num)
    {
        return playerPortrait[num];
    }
    [SerializeField]
    public IPlayerState CurrentState
    {
        get => _currentState;
        private set
        {
            if (_currentState?.GetType() == value.GetType())  //  상태가 변하지 않았다면 업데이트하지 않는다. 
            {
                DebugEx.Log(_currentState?.GetType().ToString() + " -> " + value.GetType().ToString());
                return;
            }

            bool ran = CurrentState is PlayerRunState;
            _currentState?.Exit();                 // 이전의 State에서 탈출

            _currentState = value;

            _currentState.Enter(this);      // State에 진입
            switch (_currentState)
            {
                case PlayerIdleState playerIdleState:
                    _animator.CrossFade("PlayerIdle", idleCFCoef * (ran ? 3 : 1));
                    break;
                case PlayerWalkState playerWalkState:
                    _animator.CrossFade("PlayerWalk", walkCFCoef * (ran ? 3 : 1));
                    break;
                case PlayerRunState playerRunState:
                    _animator.CrossFade("PlayerRun", runCFCoef);
                    break;
                case PlayerJumpState playerJumpState:
                    _animator.CrossFade("PlayerJump", jumpCFCoef);
                    break;
                case PlayerFallState playerFallState:
                    _animator.CrossFade("PlayerFall", fallCFCoef);
                    break;
                case PlayerAttackState playerAttackState:
                    break;
            }

        }
    }

    private PlayerIdleState _idleState;
    private PlayerWalkState _walkState;
    private PlayerRunState _runState;
    private PlayerJumpState _jumpState;
    private PlayerFallState _fallState;
    private PlayerAttackState _attackState;

    private float idleCFCoef = 0.1f;
    private float walkCFCoef = 0.1f;
    private float runCFCoef = 0.3f;
    private float jumpCFCoef = 0.1f;
    private float fallCFCoef = 0.1f;

    [Tab("KeyComponents")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _rightHandBone;

    private PlayerInputActions _playerInputActions;
    private Vector3 _localScale;

    // [Tab("Interaction")]
    // [SerializeField] public GameObject NPC;
    // [SerializeField] private bool isNPCAvailable = false;

    private void Awake()
    {
        // 중요한 컴포넌트들을 모두 초기화
        InitKeyComponents();
        // State 초기화
        InitStates();
        DialogueManager.GetInstance().playerController = this;
        _localScale = transform.localScale;

        interactPressed = false;
        instance = this;
    }

    /// <summary>
    /// 플레이어 컨트롤러에서 중요한 역할을 하는 컴포넌트를 초기화하는 함수.
    /// 현재 부착되어있는 컴포넌트들을 가져와 캐싱한다.
    /// </summary>
    private void InitKeyComponents()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.freezeRotation = true;

        _animator = GetComponent<Animator>();

        _playerInputActions = new PlayerInputActions();
    }

    /// <summary>
    /// 플레이어의 State 객체들을 초기화하는 함수.
    /// 이 객체들은 영구적으로 쓰인다.
    /// </summary>
    private void InitStates()
    {
        _idleState = new PlayerIdleState();
        _walkState = new PlayerWalkState();
        _runState = new PlayerRunState();
        _jumpState = new PlayerJumpState();
        _fallState = new PlayerFallState();
        _attackState = new PlayerAttackState();

        CurrentState = _idleState;
    }

    /// <summary>
    /// 손에 쥔 물건을 바꾸는 함수
    /// </summary>
    /// <param name="context"></param>
    private void OnChange(InputAction.CallbackContext context)
    {
        DebugEx.Log($"Changed to {context.control.name}th tool!");

        int targetTool = int.Parse(context.control.name) - 1;

        for (int i = 0; i < _rightHandBone.childCount; i++)
            _rightHandBone.GetChild(i).gameObject.SetActive(false);

        _rightHandBone.GetChild(targetTool).gameObject.SetActive(true);
        _rightHandBone.GetChild(targetTool).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
    }

    private void FixedUpdate()
    {
        CurrentState?.Execute();

        #region NPC Interaction

        // Debug.DrawRay(_rigidbody2D.position,Vector3.right,Color.red );
        // RaycastHit2D hit = Physics2D.Raycast(_rigidbody2D.position, Vector3.right, 1, LayerMask.GetMask("NPC"));
        // if(hit.collider != null)
        // {
        //     isNPCAvailable = true;
        //     DebugEx.Log("NPC Available : " + isNPCAvailable);
        //     NPC = GameObject.Find("NPC");     
        // }
        // if (isNPCAvailable && Vector2.Distance(transform.position, NPC.transform.position) > 3)
        // {
        //     isNPCAvailable = false;
        //     DebugEx.Log("NPC Available : " + isNPCAvailable);
        //     NPC = GameObject.Find("NULLNPC");
        // }

        #endregion
    }

    private void Update()
    {
        RayCheckGround();
    }

    private void RayCheckGround()
    {
        Vector2 rayStart = _rigidbody2D.position + Vector2.down * 10 + Vector2.left * 20;
        float rayDistance = 40;

        Debug.DrawRay(rayStart, Vector3.right * rayDistance, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector3.right, rayDistance, LayerMask.GetMask("Ground"));
        if (Equals(hit.collider, null))     // ray에 땅이 닿지 않을때 (= 공중일 때)
        {
            isJumpable = false;
            if (_rigidbody2D.velocity.y < 0 && CurrentState is not PlayerFallState)
                CurrentState = _fallState;
        }
        else                                // ray에 땅이 닿았을 때
        {
            jumpForced = false;
            isJumpable = true;
            if (CurrentState is PlayerFallState)
            {
                CurrentState = shiftToggled ? _runState : _idleState;
            }
        }
    }

    private void FlipImage()
    {
        Vector3 directedToRight = new Vector3(_localScale.x * -1, _localScale.y, _localScale.z);
        Vector3 directedToLeft = new Vector3(_localScale.x, _localScale.y, _localScale.z);

        if (inputVector.x == 0) transform.localScale = _localScale;
        else if (inputVector.x < 0) transform.localScale = directedToLeft;
        else transform.localScale = directedToRight;
    }

    #region Move

    void MoveStarted(InputAction.CallbackContext context)
    {
        // DebugEx.Log($"MoveStarted");

        if (CurrentState is PlayerFallState or PlayerJumpState) return;
        CurrentState = shiftToggled ? _runState : _walkState;
    }
    void MovePerformed(InputAction.CallbackContext context)
    {
        // DebugEx.Log($"MovePerformed");

        inputVector.x = context.ReadValue<Vector2>().x;
    }
    void MoveCanCeled(InputAction.CallbackContext context)
    {
        // DebugEx.Log($"MoveCanceled");

        if (CurrentState is not PlayerFallState && CurrentState is not PlayerJumpState)
            CurrentState = _idleState;
        inputVector.x = 0;
    }

    void RunningSwitchEnter(InputAction.CallbackContext context)
    {
        shiftToggled = true;

        if (CurrentState is PlayerWalkState && shiftToggled)
            CurrentState = _runState;
    }

    void RunningSwitchExit(InputAction.CallbackContext context)
    {
        shiftToggled = false;

        if (CurrentState is PlayerRunState && !shiftToggled)
            CurrentState = _walkState;
    }

    #endregion

    #region Jump

    void JumpStarted(InputAction.CallbackContext context)
    {
        // DebugEx.Log($"JumpStarted");
        if (!isJumpable) return;

        CurrentState = _jumpState;
    }
    void JumpPerformed(InputAction.CallbackContext context)
    {
        // DebugEx.Log($"JumpPerformed");
    }
    void JumpCanceled(InputAction.CallbackContext context)
    {
        // DebugEx.Log($"JumpCanceled");
    }

    #endregion

    #region Interact
    void InteractStarted(InputAction.CallbackContext context)
    {
        // DebugEx.Log($"InteractStarted");
    }
    void InteractPerformed(InputAction.CallbackContext context)
    {
        //NpcData.GetInstance().playerController = this;
        //DebugEx.Log($"InteractPerformed {context}");
        //if (DialogueManager.GetInstance().choicelen > 1)
        //{
        //    int id = DialogueManager.GetInstance().curchoice;
        //    UI_DialoguePopup.GetInstance().choiceButton[id].onClick.Invoke();
        //}
        interactPressed = true;
    }

    public bool GetInteractPressed()
    {
        bool result = interactPressed;
        interactPressed = false;
        return result;
    }
    void InteractCanceled(InputAction.CallbackContext context)
    {
        // DebugEx.Log($"InteractCanceled");
    }
    #endregion

    #region Pause

    void PauseOrResume(InputAction.CallbackContext context)
    {
        // if (Managers.UI.FindPopup<UI_PausePopup>() == null)
        // {
        //     Managers.UI.ShowPopupUI<UI_PausePopup>();
        // }
        // else
        // {
        //     Managers.UI.CloseAllPopupUI();
        //     Time.timeScale = 1.0f;
        // }

    }

    #endregion
    #region QuestCall
    void QuestCall(InputAction.CallbackContext context)
    {

        if (!isQuestPanelActive)
        {
            QuesttabOpen();
            isQuestPanelActive = true;
        }
        else
        {
            QuesttabClose();
            isQuestPanelActive = false;
        }
    }

    private void QuesttabOpen()
    {
        Managers.UI.ShowPopupUI<UI_NotebookPopup>();

    }

    private void QuesttabClose()
    {
        Managers.UI.ClosePopupUI();
    }
    #endregion

    #region AdvanceQuest
    void AdvanceQuest(InputAction.CallbackContext context)
    {
        Advance();
    }

    private void Advance()
    {
        QuestManager.GetInstance().AdvanceQuest(1);
    }
    #endregion
    #region About PlayerInput
    private void OnEnable()
    {

        // PlayerInput을 컴포넌트 대신 스크립트로
        _playerInputActions.PlayerAction.Move.started += MoveStarted;
        _playerInputActions.PlayerAction.Move.performed += MovePerformed;
        _playerInputActions.PlayerAction.Move.canceled += MoveCanCeled;
        _playerInputActions.PlayerAction.RunningSwitch.started += RunningSwitchEnter;
        _playerInputActions.PlayerAction.RunningSwitch.canceled += RunningSwitchExit;
        _playerInputActions.PlayerAction.Jump.started += JumpStarted;
        _playerInputActions.PlayerAction.Jump.performed += JumpPerformed;
        _playerInputActions.PlayerAction.Jump.canceled += JumpCanceled;
        _playerInputActions.PlayerAction.Interact.started += InteractStarted;
        _playerInputActions.PlayerAction.Interact.performed += InteractPerformed;
        _playerInputActions.PlayerAction.Interact.canceled += InteractCanceled;
        _playerInputActions.PlayerAction.WeaponChange.performed += OnChange;
        _playerInputActions.PlayerAction.Escape.started += PauseOrResume;
        _playerInputActions.PlayerAction.QuestCall.performed += QuestCall;
        _playerInputActions.PlayerAction.AdvanceQuest.performed += AdvanceQuest;
        _playerInputActions.Enable();
    }
    
    private void OnDisable()
    {
        // PlayerInput을 컴포넌트 대신 스크립트로
        _playerInputActions.PlayerAction.Move.started -= MoveStarted;
        _playerInputActions.PlayerAction.Move.performed -= MovePerformed;
        _playerInputActions.PlayerAction.Move.canceled -= MoveCanCeled;
        _playerInputActions.PlayerAction.RunningSwitch.started -= RunningSwitchEnter;
        _playerInputActions.PlayerAction.RunningSwitch.canceled -= RunningSwitchExit;
        _playerInputActions.PlayerAction.Jump.started -= JumpStarted;
        _playerInputActions.PlayerAction.Jump.performed -= JumpPerformed;
        _playerInputActions.PlayerAction.Jump.canceled -= JumpCanceled;
        _playerInputActions.PlayerAction.Interact.started -= InteractStarted;
        _playerInputActions.PlayerAction.Interact.performed -= InteractPerformed;
        _playerInputActions.PlayerAction.Interact.canceled -= InteractCanceled;
        _playerInputActions.PlayerAction.WeaponChange.performed -= OnChange;
        _playerInputActions.PlayerAction.Escape.started -= PauseOrResume;
        _playerInputActions.PlayerAction.QuestCall.performed -= QuestCall;
        _playerInputActions.PlayerAction.AdvanceQuest.performed -= AdvanceQuest;
        _playerInputActions.Disable();
    }
    
        #endregion
}