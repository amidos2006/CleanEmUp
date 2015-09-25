using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine
{
    public static class Input
    {
        private static Dictionary<Keys, GameButtonState> keyboardStateButtons;

        private static GameButtonState leftMouseButton;
        private static GameButtonState rightMouseButton;

        private static GameButtonState leftGamepadButton;
        private static GameButtonState rightGamepadButton;

        public static Vector2 GetMousePosition(Camera camera)
        {
            Vector2 mousePosition = new Vector2();
            mousePosition.X = Mouse.GetState().X;
            mousePosition.Y = Mouse.GetState().Y;

            return camera.ConvertToWorld(mousePosition);
        }

        public static Vector2 GetLeftStick()
        {
            Vector2 leftStick = new Vector2();
            if(GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                leftStick = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left;
            }
            
            return leftStick;
        }

        public static Vector2 GetRightStick()
        {
            Vector2 rightStick = new Vector2();
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                rightStick = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right;
            }

            return rightStick;
        }

        public static GameButtonState CheckLeftMouseButton()
        {
            return leftMouseButton;
        }

        public static GameButtonState CheckRightMouseButton()
        {
            return rightMouseButton;
        }

        public static GameButtonState CheckLeftGamepadButton()
        {
            return leftGamepadButton;
        }

        public static GameButtonState CheckRightGamepadButton()
        {
            return rightGamepadButton;
        }

        public static GameButtonState CheckKeyboardButton(Keys key)
        {
            return keyboardStateButtons[key];
        }

        public static void Intialzie()
        {
            keyboardStateButtons = new Dictionary<Keys, GameButtonState>();

            leftMouseButton = GameButtonState.Up;
            rightMouseButton = GameButtonState.Up;

            leftGamepadButton = GameButtonState.Up;
            rightGamepadButton = GameButtonState.Up;

            Array keys = Enum.GetValues(typeof(Keys));

            foreach (Keys key in keys)
            {
                keyboardStateButtons.Add(key, GameButtonState.Up);
            }
        }

        public static void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();
            Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
            GamePadState currentGamePad = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
            Dictionary<Keys,KeyState> currentKeyState = new Dictionary<Keys,KeyState>();

            #region Mouse Update

            if(currentMouseState.LeftButton == ButtonState.Pressed)
            {
                switch (leftMouseButton)
                {
		            case GameButtonState.Pressed:
                        leftMouseButton = GameButtonState.Down;
                    break;
                    case GameButtonState.Released:
                        leftMouseButton = GameButtonState.Pressed;
                    break;
                    case GameButtonState.Down:
                        leftMouseButton = GameButtonState.Down;
                    break;
                    case GameButtonState.Up:
                        leftMouseButton = GameButtonState.Pressed;
                    break;
                }
            }
            else
            {
                switch (leftMouseButton)
                {
		            case GameButtonState.Pressed:
                        leftMouseButton = GameButtonState.Released;
                    break;
                    case GameButtonState.Released:
                        leftMouseButton = GameButtonState.Up;
                    break;
                    case GameButtonState.Down:
                        leftMouseButton = GameButtonState.Released;
                    break;
                    case GameButtonState.Up:
                        leftMouseButton = GameButtonState.Up;
                    break;
                }
            }

            if(currentMouseState.RightButton == ButtonState.Pressed)
            {
                switch (rightMouseButton)
                {
		            case GameButtonState.Pressed:
                        rightMouseButton = GameButtonState.Down;
                    break;
                    case GameButtonState.Released:
                        rightMouseButton = GameButtonState.Pressed;
                    break;
                    case GameButtonState.Down:
                        rightMouseButton = GameButtonState.Down;
                    break;
                    case GameButtonState.Up:
                        rightMouseButton = GameButtonState.Pressed;
                    break;
                }
            }
            else
            {
                switch (rightMouseButton)
                {
		            case GameButtonState.Pressed:
                        rightMouseButton = GameButtonState.Released;
                    break;
                    case GameButtonState.Released:
                        rightMouseButton = GameButtonState.Up;
                    break;
                    case GameButtonState.Down:
                        rightMouseButton = GameButtonState.Released;
                    break;
                    case GameButtonState.Up:
                        rightMouseButton = GameButtonState.Up;
                    break;
                }
            }

            #endregion

            #region Keyboard Update

            //Clear the keyboard state
            Array keys = Enum.GetValues(typeof(Keys));
            foreach (Keys key in keys)
            {
                currentKeyState[key] = KeyState.Up;
	        }

            foreach (Keys key in pressedKeys)
            {
                currentKeyState[key] = KeyState.Down;
            }

            foreach (KeyValuePair<Keys,KeyState> keyPair in currentKeyState)
            {
                switch (keyboardStateButtons[keyPair.Key])
                {
                    case GameButtonState.Pressed:
                        if (keyPair.Value == KeyState.Down)
                        {
                            keyboardStateButtons[keyPair.Key] = GameButtonState.Down;
                        }
                        else
                        {
                            keyboardStateButtons[keyPair.Key] = GameButtonState.Released;
                        }
                        break;
                    case GameButtonState.Released:
                        if (keyPair.Value == KeyState.Down)
                        {
                            keyboardStateButtons[keyPair.Key] = GameButtonState.Pressed;
                        }
                        else
                        {
                            keyboardStateButtons[keyPair.Key] = GameButtonState.Up;
                        }
                        break;
                    case GameButtonState.Down:
                        if (keyPair.Value == KeyState.Down)
                        {
                            keyboardStateButtons[keyPair.Key] = GameButtonState.Down;
                        }
                        else
                        {
                            keyboardStateButtons[keyPair.Key] = GameButtonState.Released;
                        }
                        break;
                    case GameButtonState.Up:
                        if (keyPair.Value == KeyState.Down)
                        {
                            keyboardStateButtons[keyPair.Key] = GameButtonState.Pressed;
                        }
                        else
                        {
                            keyboardStateButtons[keyPair.Key] = GameButtonState.Up;
                        }
                        break;
                }
            }

            #endregion

            #region Gamepad Update

            if (currentGamePad.IsConnected)
            {
                if (currentGamePad.IsButtonDown(Buttons.LeftShoulder) || currentGamePad.IsButtonDown(Buttons.LeftTrigger))
                {
                    switch (leftGamepadButton)
                    {
                        case GameButtonState.Pressed:
                            leftGamepadButton = GameButtonState.Down;
                            break;
                        case GameButtonState.Released:
                            leftGamepadButton = GameButtonState.Pressed;
                            break;
                        case GameButtonState.Down:
                            leftGamepadButton = GameButtonState.Down;
                            break;
                        case GameButtonState.Up:
                            leftGamepadButton = GameButtonState.Pressed;
                            break;
                    }
                }
                else
                {
                    switch (leftGamepadButton)
                    {
                        case GameButtonState.Pressed:
                            leftGamepadButton = GameButtonState.Released;
                            break;
                        case GameButtonState.Released:
                            leftGamepadButton = GameButtonState.Up;
                            break;
                        case GameButtonState.Down:
                            leftGamepadButton = GameButtonState.Released;
                            break;
                        case GameButtonState.Up:
                            leftGamepadButton = GameButtonState.Up;
                            break;
                    }
                }

                if (currentGamePad.IsButtonDown(Buttons.RightShoulder) || currentGamePad.IsButtonDown(Buttons.RightTrigger))
                {
                    switch (rightGamepadButton)
                    {
                        case GameButtonState.Pressed:
                            rightGamepadButton = GameButtonState.Down;
                            break;
                        case GameButtonState.Released:
                            rightGamepadButton = GameButtonState.Pressed;
                            break;
                        case GameButtonState.Down:
                            rightGamepadButton = GameButtonState.Down;
                            break;
                        case GameButtonState.Up:
                            rightGamepadButton = GameButtonState.Pressed;
                            break;
                    }
                }
                else
                {
                    switch (rightGamepadButton)
                    {
                        case GameButtonState.Pressed:
                            rightGamepadButton = GameButtonState.Released;
                            break;
                        case GameButtonState.Released:
                            rightGamepadButton = GameButtonState.Up;
                            break;
                        case GameButtonState.Down:
                            rightGamepadButton = GameButtonState.Released;
                            break;
                        case GameButtonState.Up:
                            rightGamepadButton = GameButtonState.Up;
                            break;
                    }
                }
            }

            #endregion
        }
    }
}
