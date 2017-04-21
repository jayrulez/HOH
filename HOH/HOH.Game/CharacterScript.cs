using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Input;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Graphics;
using SiliconStudio.Xenko.Rendering;
using SiliconStudio.Xenko.Rendering.Composers;
using SiliconStudio.Xenko.Rendering.Sprites;
using SiliconStudio.Xenko.Games;
using SiliconStudio.Xenko.Animations;
using SiliconStudio.Xenko.Physics;

namespace HOH
{
    public class CharacterScript : SyncScript
    {
        [Flags]
        private enum InputState
        {
            None = 0x0,
            Up = 0x2,
            Down = 0x4,
            Left = 0x8,
            Right = 0x16
        }

        private InputState _oldInputState = InputState.None;
        private Vector3 _position;
        private Vector3 _velocity;
        private float _speed;

        private void Initialize()
        {
            _position = Vector3.Zero;
            _velocity = Vector3.Zero;

            _speed = 60;
        }

        private SpriteFromSheet _characterSpriteSheet;

        private Dictionary<InputState, int[]> _animationFrames = new Dictionary<InputState, int[]>
        {
            { InputState.Left, new [] { 4, 5, 6, 7 } },
            { InputState.Right, new [] { 12, 13, 14, 15 } },
            { InputState.Up, new [] { 8, 9, 10, 11 } },
            { InputState.Down, new [] { 0, 1, 2, 3 } }
        };

        private SpriteComponent _characterSpriteComponent;
        private Entity _characterEntity;

        private float _bufferWidth;
        private float _bufferHeight;

        public override void Start()
        {
            _characterSpriteSheet = Entity.Get<SpriteComponent>().SpriteProvider as SpriteFromSheet;
            _characterSpriteComponent = Entity.Get<SpriteComponent>();
            _characterEntity = SceneSystem.SceneInstance.Scene.Entities.First(e => e.Name == "Character");

            //_characterEntity.Transform.Scale = new Vector3(1f, 1f, 1);

            _bufferWidth = GraphicsDevice.Presenter.Description.BackBufferWidth;
            _bufferHeight = GraphicsDevice.Presenter.Description.BackBufferHeight;

            Initialize();
        }

        public override void Update()
        {
            if (Game.IsRunning)
            {
                var inputState = GetInputState();

                if (inputState == InputState.None)
                {
                    if (_oldInputState != InputState.None)
                    {
                        SpriteAnimation.Stop(_characterSpriteComponent);

                        _characterSpriteSheet.CurrentFrame = _animationFrames.FirstOrDefault(i => _oldInputState.HasFlag(i.Key)).Value.First();

                        _oldInputState = InputState.None;
                    }
                }
                else
                {
                    if (_oldInputState != inputState)
                    {
                        SpriteAnimation.Play(_characterSpriteComponent, _animationFrames.FirstOrDefault(i => inputState.HasFlag(i.Key)).Value, AnimationRepeatMode.LoopInfinite, 15);
                    }
                }

                _oldInputState = inputState;

                //UpdateTransform(inputState);

                UpdateTransform2();
            }
        }

        private InputState GetInputState()
        {
            InputState inputState = InputState.None;

            if (Input.IsKeyDown(Keys.Left))
            {
                inputState = InputState.Left;
            }

            if (Input.IsKeyDown(Keys.Right))
            {
                inputState = InputState.Right;
            }

            if (Input.IsKeyDown(Keys.Up))
            {
                inputState = InputState.Up;
            }

            if (Input.IsKeyDown(Keys.Down))
            {
                inputState = InputState.Down;
            }

            return inputState;
        }

        private void UpdateTransform(InputState inputState)
        {
            if (inputState.HasFlag(InputState.Up))
            {
                var position = _characterEntity.Transform.Position.Y + 60f * (float)Game.UpdateTime.Elapsed.Milliseconds / 1000;

                var halfHeight = _characterSpriteSheet.GetSprite().Size.Y / 2;

                if (position > _bufferHeight / 2 - halfHeight)
                {
                    position = _bufferHeight / 2 - halfHeight;
                }

                //_characterEntity.Get<CharacterComponent>().SetVelocity(new Vector3(0, position, 0));

                _characterEntity.Transform.Position.Y = position;
            }

            if (inputState.HasFlag(InputState.Down))
            {
                var halfHeight = _characterSpriteSheet.GetSprite().Size.Y / 2;

                var position = _characterEntity.Transform.Position.Y - 60f * (float)Game.UpdateTime.Elapsed.Milliseconds / 1000;

                if (position < -_bufferHeight / 2 + halfHeight)
                {
                    position = -_bufferHeight / 2 + halfHeight;
                }

                //_characterEntity.Get<CharacterComponent>().SetVelocity(new Vector3(0, position, 0));

                _characterEntity.Transform.Position.Y = position;
            }

            if (inputState.HasFlag(InputState.Left))
            {
                var halfWidth = _characterSpriteSheet.GetSprite().Size.X / 2;

                var position = _characterEntity.Transform.Position.X - 60f * (float)Game.UpdateTime.Elapsed.Milliseconds / 1000;
                if (position < (-_bufferWidth / 2) + halfWidth)
                {
                    position = (-_bufferWidth / 2) + halfWidth;
                }

                //_characterEntity.Get<CharacterComponent>().SetVelocity(new Vector3(position, 0, 0));

                _characterEntity.Transform.Position.X = position;
            }

            if (inputState.HasFlag(InputState.Right))
            {
                var position = _characterEntity.Transform.Position.X + 60f * (float)Game.UpdateTime.Elapsed.Milliseconds / 1000;

                var halfWidth = _characterSpriteSheet.GetSprite().Size.X / 2;

                if (position > _bufferWidth / 2 - halfWidth)
                {
                    position = _bufferWidth / 2 - halfWidth;
                }

                //_characterEntity.Get<CharacterComponent>().SetVelocity(Vector3.UnitX * position * 60f * (float)Game.UpdateTime.Elapsed.Milliseconds / 1000);
                //_characterEntity.Get<CharacterComponent>().SetVelocity(Vector3.Zero);

                _characterEntity.Transform.Position.X = position;
            }

            if (inputState.HasFlag(InputState.None))
            {
                //_characterEntity.Get<CharacterComponent>().Jump(Vector3.Zero);
            }

        }

        private void UpdateTransform2()
        {
            var inputState = GetInputState();

            float angle = 0f;

            if(inputState == InputState.Left)
            {
                angle = (float)Math.PI;
            }

            if (inputState == InputState.Up)
            {
                angle = (float)Math.PI/2;
            }

            if (inputState == InputState.Right)
            {
                angle = 0f;
            }

            if (inputState == InputState.Down)
            {
                angle = 3.0f * (float)Math.PI / 2;
            }

            if (inputState == InputState.None)
            {
                _velocity = Vector3.Zero;
            }
            else
            {
                _velocity = new Vector3((float)Math.Cos(angle) * _speed, (float)Math.Sin(angle) * _speed, 0f);
            }

            Entity.Get<CharacterComponent>().SetVelocity(_velocity);
        }
    }
}
