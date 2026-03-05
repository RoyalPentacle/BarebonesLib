using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.DataStructures
{
    /// <summary>
    /// Represent an RGBA Colour value, using floats instead of bytes, ranging from -255 to 255.
    /// </summary>
    /// <remarks>Used for changing colours smoothly over time.</remarks>
    public struct ColorF
    {
        private float R;
        private float G;
        private float B;
        private float A;

        /// <summary>
        /// Creates a new ColorF with the specified RGBA
        /// </summary>
        /// <param name="r">The R value.</param>
        /// <param name="g">The G value.</param>
        /// <param name="b">The B value.</param>
        /// <param name="a">The A value.</param>
        public ColorF(float r, float g, float b, float a)
        {
            R = Math.Clamp(r, -255f, 255f);
            G = Math.Clamp(g, -255f, 255f);
            B = Math.Clamp(b, -255f, 255f);
            A = Math.Clamp(a, -255f, 255f);
        }

        /// <summary>
        /// Creates a new ColorF from an existing <see cref="Color"/>.
        /// </summary>
        /// <param name="color">The Color to copy.</param>
        public ColorF(Color color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = color.A;
        }

        /// <summary>
        /// Returns a new ColorF that is the amount the starting colour would need to change every frame to reach the destination colour in the alotted time.
        /// </summary>
        /// <param name="startColour">The starting colour.</param>
        /// <param name="destColour">The destination colour.</param>
        /// <param name="time">The time in milliseconds for the change to occur.</param>
        /// <returns>The change over time.</returns>
        public static ColorF GetChangeOverTime(Color startColour, Color destColour, float time)
        {
            ColorF overTime = new ColorF();
            float delta = 1000f / time / 60;
            overTime.R = (destColour.R - startColour.R) * delta;
            overTime.G = (destColour.G - startColour.G) * delta;
            overTime.B = (destColour.B - startColour.B) * delta;
            overTime.A = (destColour.A - startColour.A) * delta;
            return overTime;
        }

        /// <summary>
        /// Gets the current RGBA value as a <see cref="Color"/>
        /// </summary>
        public Color GetColour
        {
            get
            {
                return new Color((byte)Math.Abs(R), (byte)Math.Abs(G), (byte)Math.Abs(B), (byte)Math.Abs(A));
            }
        }

        /// <summary>
        /// Adds the two colours together.
        /// </summary>
        /// <param name="a">Colour A.</param>
        /// <param name="b">Colour B.</param>
        /// <returns>The sum of A and B.</returns>
        public static ColorF operator +(ColorF a, ColorF b)
        {
            ColorF result = new ColorF();
            result.R = Math.Clamp(a.R + b.R, -255f, 255f);
            result.G = Math.Clamp(a.G + b.G, -255f, 255f);
            result.B = Math.Clamp(a.B + b.B, -255f, 255f);
            result.A = Math.Clamp(a.A + b.A, -255f, 255f);
            return result;
        }

        /// <summary>
        /// Subtract the two colours.
        /// </summary>
        /// <param name="a">Colour A.</param>
        /// <param name="b">Colour B.</param>
        /// <returns>The result of subtracting B from A.</returns>
        public static ColorF operator -(ColorF a, ColorF b)
        {
            ColorF result = new ColorF();
            result.R = Math.Clamp(a.R - b.R, -255f, 255f);
            result.G = Math.Clamp(a.G - b.G, -255f, 255f);
            result.B = Math.Clamp(a.B - b.B, -255f, 255f);
            result.A = Math.Clamp(a.A - b.A, -255f, 255f);
            return result;
        }

        /// <summary>
        /// Multiply the two colours.
        /// </summary>
        /// <param name="a">Colour A.</param>
        /// <param name="b">Colour B.</param>
        /// <returns>The product of A and B.</returns>
        public static ColorF operator *(ColorF a, ColorF b)
        {
            ColorF result = new ColorF();
            result.R = Math.Clamp(a.R * b.R, -255f, 255f);
            result.G = Math.Clamp(a.G * b.G, -255f, 255f);
            result.B = Math.Clamp(a.B * b.B, -255f, 255f);
            result.A = Math.Clamp(a.A * b.A, -255f, 255f);
            return result;
        }

        /// <summary>
        /// Divide the two colours.
        /// </summary>
        /// <param name="a">Colour A.</param>
        /// <param name="b">Colour B.</param>
        /// <returns>The result of dividing A by B.</returns>
        public static ColorF operator /(ColorF a, ColorF b)
        {
            ColorF result = new ColorF();
            result.R = Math.Clamp(a.R / b.R, -255f, 255f);
            result.G = Math.Clamp(a.G / b.G, -255f, 255f);
            result.B = Math.Clamp(a.B / b.B, -255f, 255f);
            result.A = Math.Clamp(a.A / b.A, -255f, 255f);
            return result;
        }

    }
}
