using System;
using UnityEngine.InputSystem;

namespace BallzMerge.Root
{
    using Settings;

    public class UserInputHandler : IDisposable
    {
        private SettingsMenuView _menuView;
        private MainInputMap _userInput;

        public UserInputHandler(SettingsMenuView menuView, MainInputMap userInput)
        {
            _menuView = menuView;
            _userInput = userInput;
            _userInput.MainInput.MenuRequier.performed += OnPlayerMenuButt;
        }

        public void Dispose()
        {
            _userInput.MainInput.MenuRequier.performed -= OnPlayerMenuButt;
        }

        private void OnPlayerMenuButt(InputAction.CallbackContext ctx)
        {
            _menuView.ChangeActivity();
        }
    }
}
