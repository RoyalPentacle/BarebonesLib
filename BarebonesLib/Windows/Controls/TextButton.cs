using Barebones.Asset;
using Barebones.Asset.Audio;
using Barebones.Config;
using Barebones.Drawable;
using Barebones.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Windows.Controls
{
    /// <summary>
    /// This is a button, but with text acting as the visual element.
    /// </summary>
    public class TextButton : Button
    {
        private ComplexSprite _background;
        private ComplexSprite _topEdge;
        private ComplexSprite _rightEdge;
        private ComplexSprite _bottomEdge;
        private ComplexSprite _leftEdge;
        private ComplexSprite _topLeftCorner;
        private ComplexSprite _topRightCorner;
        private ComplexSprite _bottomLeftCorner;
        private ComplexSprite _bottomRightCorner;

        private Text _displayText;
        private Vector2 _textOffset;
        private bool _hasBackground = false;

        /// <summary>
        /// Construct a new TextButton with the specified arguments. Creates a button of a specific size.
        /// </summary>
        /// <param name="name">The name of the button.</param>
        /// <param name="bounds">The size of the button.</param>
        /// <param name="hasBackground">Does this button have a background.</param>
        /// <param name="text">The text displayed by this button.</param>
        /// <param name="scriptPath">The script path to the font for the text in this button.</param>
        /// <param name="textScale">The scale of the text.</param>
        /// <param name="textColor">The colour of the text.</param>
        /// <param name="parent">The parent window.</param>
        /// <param name="action">The action when this button is activated.</param>
        public TextButton(string name, Rectangle bounds, bool hasBackground, string text, string scriptPath, float textScale, Color textColor, Window parent, Action<Button> action)
        {
            Initialize(name, bounds, hasBackground, text, scriptPath, textScale, textColor, parent, action);
        }

        /// <summary>
        /// Construct a new TextButton with the specified arguments. Creates a button of a specific size.
        /// </summary>
        /// <remarks>Defaults to having a background component.</remarks>
        /// <param name="name">The name of the button.</param>
        /// <param name="bounds">The size of the button.</param>
        /// <param name="text">The text displayed by this button.</param>
        /// <param name="scriptPath">The script path to the font for the text in this button.</param>
        /// <param name="textScale">The scale of the text.</param>
        /// <param name="textColor">The colour of the text.</param>
        /// <param name="parent">The parent window.</param>
        /// <param name="action">The action when this button is activated.</param>
        public TextButton(string name, Rectangle bounds, string text, string scriptPath, float textScale, Color textColor, Window parent, Action<Button> action) : this(name, bounds, true, text, scriptPath, textScale, textColor, parent, action)
        {
              
        }

        /// <summary>
        /// Construct a new TextButton with the specified arguments. Auto sizes the button to the size of the text.
        /// </summary>
        /// <param name="name">The name of the button.</param>
        /// <param name="pos">The location of the button.</param>
        /// <param name="hasBackground">Does this button have a background.</param>
        /// <param name="text">The text displayed by this button.</param>
        /// <param name="scriptPath">The script path to the font for the text in this button.</param>
        /// <param name="textScale">The scale of the text.</param>
        /// <param name="textColor">The colour of the text.</param>
        /// <param name="parent">The parent window.</param>
        /// <param name="action">The action when this button is activated.</param>
        public TextButton(string name, Point pos, bool hasBackground, string text, string scriptPath, float textScale, Color textColor, Window parent, Action<Button> action)
        {
            _displayText = new Text(text, scriptPath, textScale);
            _displayText.SetColour(textColor);
            _displayText.Font.IgnoreCulling = true;
            Rectangle bounds = new Rectangle(pos.X, pos.Y, _displayText.TextWidth, _displayText.TextHeight);
            bounds.Height = (int)(bounds.Height * 1.5f);
            if (bounds.Height % 2 == 1)
                bounds.Height += 1;
            float halfFirstChar = 0f;
            if (!string.IsNullOrEmpty(text))
            {
                if (_displayText.Font.GetAnimation(_displayText.StoredText[0].ToString(), out ComplexSprite.Anim? anim))
                {
                    if (anim != null)
                    {
                        bounds.Width += anim.Frames[0].Width;
                        halfFirstChar = 2; //anim.Frames[0].Width / 9f;
                    }
                }
            }
            if (bounds.Width % 2 == 1)
                bounds.Width += 1;
            _textOffset = new Vector2(-(_displayText.TextWidth / 2f - halfFirstChar), 0);

            Initialize(name, bounds, hasBackground, text, scriptPath, textScale, textColor, parent, action);
        }

        /// <summary>
        /// Construct a new TextButton with the specified arguments. Auto sizes the button to the size of the text.
        /// </summary>
        /// <remarks>Defaults to having a background component.</remarks>
        /// <param name="name">The name of the button.</param>
        /// <param name="pos">The location of the button.</param>
        /// <param name="text">The text displayed by this button.</param>
        /// <param name="scriptPath">The script path to the font for the text in this button.</param>
        /// <param name="textScale">The scale of the text.</param>
        /// <param name="textColor">The colour of the text.</param>
        /// <param name="parent">The parent window.</param>
        /// <param name="action">The action when this button is activated.</param>
        public TextButton(string name, Point pos, string text, string scriptPath, float textScale, Color textColor, Window parent, Action<Button> action) : this(name, pos, true, text, scriptPath, textScale, textColor, parent, action)
        {

        }

        private void Initialize(string name, Rectangle bounds, bool hasBackground, string text, string scriptPath, float textScale, Color textColor, Window parent, Action<Button> action)
        {
            _name = name;
            _bounds = bounds;
            _parent = parent;
            _action = action;
            _hasBackground = hasBackground;
            if (_displayText == null)
            {
                _displayText = new Text(text, scriptPath, textScale);
                _displayText.SetColour(textColor);
            _displayText.Font.IgnoreCulling = true;
                float halfFirstChar = 0f;
                if (!string.IsNullOrEmpty(text))
                {
                    if (_displayText.Font.GetAnimation(_displayText.StoredText[0].ToString(), out ComplexSprite.Anim? anim))
                    {
                        if (anim != null)
                            halfFirstChar = anim.Frames[0].Width / 2f;
                    }
                }
                _textOffset = new Vector2(-(_displayText.TextWidth / 2 - halfFirstChar), 0);
            }
            if (_hasBackground)
            {
                _background = new ComplexSprite(_parent.ScriptPath);
                _background.IgnoreCulling = true;
                _background.ChangeAnimation("TEXTBUTTONBACKGROUND");
                _background.SetScale(new Vector2(_bounds.Size.X / _background.CurrentFrame.Width, _bounds.Size.Y / _background.CurrentFrame.Height));
                _topEdge = new ComplexSprite(_parent.ScriptPath);
                _topEdge.IgnoreCulling = true;
                _topEdge.ChangeAnimation("TEXTBUTTONTOP");
                _topEdge.SetScale(new Vector2(_bounds.Size.X / _topEdge.CurrentFrame.Width, 1));
                _rightEdge = new ComplexSprite(_parent.ScriptPath);
                _rightEdge.IgnoreCulling = true;
                _rightEdge.ChangeAnimation("TEXTBUTTONRIGHT");
                _rightEdge.SetScale(new Vector2(1, _bounds.Size.Y / _rightEdge.CurrentFrame.Height));
                _bottomEdge = new ComplexSprite(_parent.ScriptPath);
                _bottomEdge.IgnoreCulling = true;
                _bottomEdge.ChangeAnimation("TEXTBUTTONBOTTOM");
                _bottomEdge.SetScale(new Vector2(_bounds.Size.X / _bottomEdge.CurrentFrame.Width, 1));
                _leftEdge = new ComplexSprite(_parent.ScriptPath);
                _leftEdge.IgnoreCulling = true;
                _leftEdge.ChangeAnimation("TEXTBUTTONLEFT");
                _leftEdge.SetScale(new Vector2(1, _bounds.Size.Y / _leftEdge.CurrentFrame.Height));
                _topLeftCorner = new ComplexSprite(_parent.ScriptPath);
                _topLeftCorner.IgnoreCulling = true;
                _topLeftCorner.ChangeAnimation("TEXTBUTTONTOPLEFT");
                _topRightCorner = new ComplexSprite(_parent.ScriptPath);
                _topRightCorner.IgnoreCulling = true;
                _topRightCorner.ChangeAnimation("TEXTBUTTONTOPRIGHT");
                _bottomLeftCorner = new ComplexSprite(_parent.ScriptPath);
                _bottomLeftCorner.IgnoreCulling = true;
                _bottomLeftCorner.ChangeAnimation("TEXTBUTTONBOTTOMLEFT");
                _bottomRightCorner = new ComplexSprite(_parent.ScriptPath);
                _bottomRightCorner.IgnoreCulling = true;
                _bottomRightCorner.ChangeAnimation("TEXTBUTTONBOTTOMRIGHT");
            }
        }

        /// <summary>
        /// Changes the animation of the button.
        /// </summary>
        /// <remarks>Currently unused, here for compatability.</remarks>
        /// <param name="anim">The animation to change to.</param>
        public override void ChangeAnim(string anim)
        {
            
        }

        /// <summary>
        /// Unload the button.
        /// </summary>
        public override void Unload()
        {
            _background?.UnloadSprite();
            _topEdge?.UnloadSprite();
            _rightEdge?.UnloadSprite();
            _bottomEdge?.UnloadSprite();
            _leftEdge?.UnloadSprite();
            _topLeftCorner?.UnloadSprite();
            _topRightCorner?.UnloadSprite();
            _bottomLeftCorner?.UnloadSprite();
            _bottomRightCorner?.UnloadSprite();
            _displayText?.Unload();
            _parent.DeregisterControl(this);
        }

        /// <summary>
        /// Update the button.
        /// </summary>
        public override void Update()
        {
            _background?.Update();
            _topEdge?.Update();
            _rightEdge?.Update();
            _bottomEdge?.Update();
            _leftEdge?.Update();
            _topLeftCorner?.Update();
            _topRightCorner?.Update();
            _bottomLeftCorner?.Update();
            _bottomRightCorner?.Update();
            _displayText?.Update();
            base.Update();
        }

        /// <summary>
        /// Draw the button.
        /// </summary>
        public override void Draw()
        {
            if (_hasBackground)
            {
                _parent.DrawLocal(_background, new Vector2(_bounds.Center.X, _bounds.Center.Y));
                _parent.DrawLocal(_topEdge, new Vector2(_bounds.Center.X, _bounds.Top - _topEdge.CurrentFrame.Origin.Y));
                _parent.DrawLocal(_rightEdge, new Vector2(_bounds.Right + _rightEdge.CurrentFrame.Origin.X, _bounds.Center.Y));
                _parent.DrawLocal(_bottomEdge, new Vector2(_bounds.Center.X, _bounds.Bottom + _bottomEdge.CurrentFrame.Origin.Y));
                _parent.DrawLocal(_leftEdge, new Vector2(_bounds.Left - _leftEdge.CurrentFrame.Origin.X, _bounds.Center.Y));

                _parent.DrawLocal(_topLeftCorner, new Vector2(_bounds.Left - _topLeftCorner.CurrentFrame.Origin.X, _bounds.Top - _topLeftCorner.CurrentFrame.Origin.Y));
                _parent.DrawLocal(_topRightCorner, new Vector2(_bounds.Right + _topRightCorner.CurrentFrame.Origin.X, _bounds.Top - _topRightCorner.CurrentFrame.Origin.Y));
                _parent.DrawLocal(_bottomLeftCorner, new Vector2(_bounds.Left - _bottomLeftCorner.CurrentFrame.Origin.X, _bounds.Bottom + _bottomLeftCorner.CurrentFrame.Origin.Y));
                _parent.DrawLocal(_bottomRightCorner, new Vector2(_bounds.Right + _bottomRightCorner.CurrentFrame.Origin.X, _bounds.Bottom + _bottomRightCorner.CurrentFrame.Origin.Y));
            }
            _parent.DrawLocal(_displayText, _bounds.Center.ToVector2() + _textOffset);
            base.Draw();
        }

    }
}
