//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------
#pragma warning disable 0618
#if !UNITY_3_5 && !UNITY_FLASH
#define DYNAMIC_FONT
#endif


using UnityEngine;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// Helper class containing functionality related to using dynamic fonts.
/// </summary>

static public class NGUIText
{
    public enum Alignment
    {
        Automatic,
        Left,
        Center,
        Right,
        Justified,
    }

    public enum SymbolStyle
    {
        None,
        Normal,
        Colored,
    }

    public class GlyphInfo
    {
        public Vector2 v0;
        public Vector2 v1;
        public Vector2 u0;
        public Vector2 u1;
        public float advance = 0f;
        public int channel = 0;
        public bool rotatedUVs = false;
    }

    /// <summary>
    /// When printing text, a lot of additional data must be passed in. In order to save allocations,
    /// this data is not passed at all, but is rather set in a single place before calling the functions that use it.
    /// </summary>

    static public UIFont bitmapFont;
#if DYNAMIC_FONT
    static public Font dynamicFont;
#endif
    static public GlyphInfo glyph = new GlyphInfo();

    static public int fontSize = 16;
    static public float fontScale = 1f;
    static public float pixelDensity = 1f;
    static public FontStyle fontStyle = FontStyle.Normal;
    static public Alignment alignment = Alignment.Left;
    static public Color tint = Color.white;

    static public int rectWidth = 1000000;
    static public int rectHeight = 1000000;
    static public int maxLines = 0;

    static public bool gradient = false;
    static public Color gradientBottom = Color.white;
    static public Color gradientTop = Color.white;

    static public bool encoding = false;
    static public float spacingX = 0f;
    static public float spacingY = 0f;
    static public bool premultiply = false;
    static public SymbolStyle symbolStyle;

    static public int finalSize = 0;
    static public float finalSpacingX = 0f;
    static public float finalLineHeight = 0f;
    static public float baseline = 0f;
    static public bool useSymbols = false;

    /// <summary>
    /// Recalculate the 'final' values.
    /// </summary>

    static public void Update() { Update(true); }

    /// <summary>
    /// Recalculate the 'final' values.
    /// </summary>

    static public void Update(bool request)
    {
        finalSize = Mathf.RoundToInt(fontSize / pixelDensity);
        finalSpacingX = spacingX * fontScale;
        finalLineHeight = (fontSize + spacingY) * fontScale;
        useSymbols = (bitmapFont != null && bitmapFont.hasSymbols) && encoding && symbolStyle != SymbolStyle.None;

#if DYNAMIC_FONT
        if (dynamicFont != null && request)
        {
            dynamicFont.RequestCharactersInTexture(")_-", finalSize, fontStyle);

            if (!dynamicFont.GetCharacterInfo(')', out mTempChar, finalSize, fontStyle))
            {
                dynamicFont.RequestCharactersInTexture("A", finalSize, fontStyle);
                {
                    if (!dynamicFont.GetCharacterInfo('A', out mTempChar, finalSize, fontStyle))
                    {
                        baseline = 0f;
                        return;
                    }
                }
            }

            float y0 = mTempChar.vert.yMax;
            float y1 = mTempChar.vert.yMin;
            baseline = Mathf.Round(y0 + (finalSize - y0 + y1) * 0.5f);
        }
#endif
    }

    /// <summary>
    /// Prepare to use the specified text.
    /// </summary>

    static public void Prepare(string text)
    {
#if DYNAMIC_FONT
        if (dynamicFont != null)
            dynamicFont.RequestCharactersInTexture(text, finalSize, fontStyle);
#endif
    }

    /// <summary>
    /// Get the specified symbol.
    /// </summary>

    static public BMSymbol GetSymbol(string text, int index, int textLength)
    {
        return (bitmapFont != null) ? bitmapFont.MatchSymbol(text, index, textLength) : null;
    }

    /// <summary>
    /// Get the width of the specified glyph. Returns zero if the glyph could not be retrieved.
    /// </summary>

    static public float GetGlyphWidth(int ch, int prev)
    {
        if (bitmapFont != null)
        {
            BMGlyph bmg = bitmapFont.bmFont.GetGlyph(ch);

            if (bmg != null)
            {
                return fontScale * ((prev != 0) ? bmg.advance + bmg.GetKerning(prev) : bmg.advance);
            }
        }
#if DYNAMIC_FONT
        else if (dynamicFont != null)
        {
            if (dynamicFont.GetCharacterInfo((char)ch, out mTempChar, finalSize, fontStyle))
                return Mathf.Round(mTempChar.width * fontScale * pixelDensity);
        }
#endif
        return 0f;
    }

    /// <summary>
    /// Get the specified glyph.
    /// </summary>

    static public GlyphInfo GetGlyph(int ch, int prev)
    {
        if (bitmapFont != null)
        {
            BMGlyph bmg = bitmapFont.bmFont.GetGlyph(ch);

            if (bmg != null)
            {
                int kern = (prev != 0) ? bmg.GetKerning(prev) : 0;
                glyph.v0.x = (prev != 0) ? bmg.offsetX + kern : bmg.offsetX;
                glyph.v1.y = -bmg.offsetY;

                glyph.v1.x = glyph.v0.x + bmg.width;
                glyph.v0.y = glyph.v1.y - bmg.height;

                glyph.u0.x = bmg.x;
                glyph.u0.y = bmg.y + bmg.height;

                glyph.u1.x = bmg.x + bmg.width;
                glyph.u1.y = bmg.y;

                glyph.advance = bmg.advance + kern;
                glyph.channel = bmg.channel;
                glyph.rotatedUVs = false;

                if (fontScale != 1f)
                {
                    glyph.v0 *= fontScale;
                    glyph.v1 *= fontScale;
                    glyph.advance *= fontScale;
                }
                return glyph;
            }
        }
#if DYNAMIC_FONT
        else if (dynamicFont != null)
        {
            if (dynamicFont.GetCharacterInfo((char)ch, out mTempChar, finalSize, fontStyle))
            {
                glyph.v0.x = mTempChar.vert.xMin;
                glyph.v1.x = glyph.v0.x + mTempChar.vert.width;

                glyph.v0.y = mTempChar.vert.yMax - baseline;
                glyph.v1.y = glyph.v0.y - mTempChar.vert.height;

                glyph.u0.x = mTempChar.uv.xMin;
                glyph.u0.y = mTempChar.uv.yMin;

                glyph.u1.x = mTempChar.uv.xMax;
                glyph.u1.y = mTempChar.uv.yMax;

                glyph.advance = mTempChar.width;
                glyph.channel = 0;
                glyph.rotatedUVs = mTempChar.flipped;

                float pd = fontScale * pixelDensity;

                if (pd != 1f)
                {
                    glyph.v0 *= pd;
                    glyph.v1 *= pd;
                    glyph.advance *= pd;
                }

                glyph.advance = Mathf.Round(glyph.advance);
                return glyph;
            }
        }
#endif
        return null;
    }

    static Color mInvisible = new Color(0f, 0f, 0f, 0f);
    static BetterList<Color> mColors = new BetterList<Color>();
#if DYNAMIC_FONT
    static CharacterInfo mTempChar;
#endif

    /// <summary>
    /// Parse a RrGgBb color encoded in the string.
    /// </summary>

    static public Color ParseColor(string text, int offset)
    {
        int r = (NGUIMath.HexToDecimal(text[offset]) << 4) | NGUIMath.HexToDecimal(text[offset + 1]);
        int g = (NGUIMath.HexToDecimal(text[offset + 2]) << 4) | NGUIMath.HexToDecimal(text[offset + 3]);
        int b = (NGUIMath.HexToDecimal(text[offset + 4]) << 4) | NGUIMath.HexToDecimal(text[offset + 5]);
        float f = 1f / 255f;
        Color color = Color.clear;
        color.r = f * r;
        color.g = f * g;
        color.b = f * b;
        return color;
    }
    /// <summary>
    /// Parse a RrGgBb color encoded in the string.
    /// </summary>

    static public bool ParseColor2(string text, int offset,out Color color) 
    {
        color = Color.clear;
        int iValue1 = NGUIMath.HexToDecimal2(text[offset]);
        if (iValue1 < 0)
            return false;

        int iValue2 = NGUIMath.HexToDecimal2(text[offset + 1]);
        if (iValue2 < 0)
            return false;

        int r = (iValue1 << 4) | iValue2;

        iValue1 = NGUIMath.HexToDecimal(text[offset + 2]);
        if (iValue1 < 0)
            return false;

        iValue2 = NGUIMath.HexToDecimal2(text[offset + 3]);
        if (iValue2 < 0)
            return false;

        int g = (iValue1 << 4) | iValue2;
        iValue1 = NGUIMath.HexToDecimal(text[offset + 4]);
        if (iValue1 < 0)
            return false;

        iValue2 = NGUIMath.HexToDecimal2(text[offset + 5]);
        if (iValue2 < 0)
            return false;

        int b = (iValue1 << 4) | iValue2;

        float f = 1f / 255f;
        color.a = 0;
        color.r = f * r;
        color.g = f * g;
        color.b = f * b;
        return true;
    }
    /// <summary>
    /// The reverse of ParseColor -- encodes a color in RrGgBb format.
    /// </summary>

    static public string EncodeColor(Color c)
    {
        int i = 0xFFFFFF & (NGUIMath.ColorToInt(c) >> 8);
        return NGUIMath.DecimalToHex(i);
    }

    /// <summary>
    /// Parse an embedded symbol, such as [FFAA00] (set color) or [-] (undo color change). Returns whether the index was adjusted.
    /// </summary>

    static public bool ParseSymbol(string text, ref int index)
    {
        int n = 1;
        bool bold = false;
        bool italic = false;
        bool underline = false;
        bool strikethrough = false;
        return ParseSymbol(text, ref index, null, false, ref n, ref bold, ref italic, ref underline, ref strikethrough);
    }

    /// <summary>
    /// Parse the symbol, if possible. Returns 'true' if the 'index' was adjusted. Advanced symbol support contributed by Rudy Pangestu.
    /// </summary>

    static public bool ParseSymbol(string text, ref int index, BetterList<Color> colors, bool premultiply,
        ref int sub, ref bool bold, ref bool italic, ref bool underline, ref bool strike)
    {
        int length = text.Length;

        if (index + 3 > length || text[index] != '[') return false;

        if (text[index + 2] == ']')
        {
            if (text[index + 1] == '-')
            {
                if (colors != null && colors.size > 1)
                    colors.RemoveAt(colors.size - 1);
                index += 3;
                return true;
            }
            //string sub3 = text.Substring(index, 3);
            if (string.Compare("[b]", 0, text, index, 3) == 0)
            {
                bold = true;
                index += 3;
                return true;
            }
            else if (string.Compare("[i]", 0, text, index, 3) == 0)
            {
                italic = true;
                index += 3;
                return true;
            }
            else if (string.Compare("[u]", 0, text, index, 3) == 0)
            {
                underline = true;
                index += 3;
                return true;
            }
            else if (string.Compare("[s]", 0, text, index, 3) == 0)
            {
                strike = true;
                index += 3;
                return true;
            }
//             }
//             switch (sub3)
//             {
//                 case "[b]":
//                     bold = true;
//                     index += 3;
//                     return true;
// 
//                 case "[i]":
//                     italic = true;
//                     index += 3;
//                     return true;
// 
//                 case "[u]":
//                     underline = true;
//                     index += 3;
//                     return true;
// 
//                 case "[s]":
//                     strike = true;
//                     index += 3;
//                     return true;
//             }
        }

        if (index + 4 > length) return false;

        if (text[index + 3] == ']')
        {
            //string sub4 = text.Substring(index, 4);
            if (string.Compare("[/b]", 0, text, index, 4) == 0)
            {
                bold = false;
                index += 4;
                return true;
            }
            else if (string.Compare("[/i]", 0, text, index, 4) == 0)
            {
                italic = false;
                index += 4;
                return true;
            }
            else if (string.Compare("[/u]", 0, text, index, 4) == 0)
            {
                underline = false;
                index += 4;
                return true;
            }
            else if (string.Compare("[/s]", 0, text, index, 4) == 0)
            {
                strike = false;
                index += 4;
                return true;
            }
//             switch (sub4)
//             {
//                 case "[/b]":
//                     bold = false;
//                     index += 4;
//                     return true;
// 
//                 case "[/i]":
//                     italic = false;
//                     index += 4;
//                     return true;
// 
//                 case "[/u]":
//                     underline = false;
//                     index += 4;
//                     return true;
// 
//                 case "[/s]":
//                     strike = false;
//                     index += 4;
//                     return true;
//             }
        }

        if (index + 5 > length) return false;

        if (text[index + 4] == ']')
        {
           // string sub5 = text.Substring(index, 5);
            if(string.Compare("[sub]", 0, text, index, 5) == 0)
            {
                sub = 1;
                index += 5;
                return true;
            }
            else if(string.Compare("[sup]", 0, text, index, 5) == 0)
            {
                sub = 2;
                index += 5;
                return true;
            }
//             switch (sub5)
//             {
//                 case "[sub]":
//                     sub = 1;
//                     index += 5;
//                     return true;
// 
//                 case "[sup]":
//                     sub = 2;
//                     index += 5;
//                     return true;
//             }
        }

        if (index + 6 > length) return false;

        if (text[index + 5] == ']')
        {
            //string sub6 = text.Substring(index, 6);
            if(string.Compare("[/sub]",0,text,index,6)==0)
            {
                sub = 0;
                index += 6;
                return true;
            }
            else if(string.Compare("[/sup]", 0, text, index, 6) == 0)
            {
                sub = 0;
                index += 6;
                return true;
            }
            else if(string.Compare("[/url]", 0, text, index, 6) == 0)
            {
                index += 6;
                return true;
            }
//             switch (sub6)
//             {
//                 case "[/sub]":
//                     sub = 0;
//                     index += 6;
//                     return true;
// 
//                 case "[/sup]":
//                     sub = 0;
//                     index += 6;
//                     return true;
// 
//                 case "[/url]":
//                     index += 6;
//                     return true;
//             }
        }

        if (text[index + 1] == 'u' && text[index + 2] == 'r' && text[index + 3] == 'l' && text[index + 4] == '=')
        {
            int closingBracket = text.IndexOf(']', index + 4);

            if (closingBracket != -1)
            {
                index = closingBracket + 1;
                return true;
            }
        }

        if (index + 8 > length) return false;

        if (text[index + 7] == ']')
        {
            Color c ;
            if( !ParseColor2(text, index + 1, out c))
            {
                return false;
            }
            // string strColor = EncodeColor(c);
            //if (string.Compare(strColor, 0, text, index + 1, 6,true) != 0 )
                
            if (colors != null)
            {
                c.a = colors[colors.size - 1].a;
                if (premultiply && c.a != 1f)
                    c = Color.Lerp(mInvisible, c, c.a);
                colors.Add(c);
            }
            index += 8;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Runs through the specified string and removes all color-encoding symbols.
    /// </summary>

    static public string StripSymbols(string text)
    {
        if (text != null)
        {
            for (int i = 0, imax = text.Length; i < imax; )
            {
                char c = text[i];

                if (c == '[')
                {
                    int sub = 0;
                    bool bold = false;
                    bool italic = false;
                    bool underline = false;
                    bool strikethrough = false;
                    int retVal = i;

                    if (ParseSymbol(text, ref retVal, null, false, ref sub, ref bold, ref italic, ref underline, ref strikethrough))
                    {
                        text = text.Remove(i, retVal - i);
                        imax = text.Length;
                        continue;
                    }
                }
                ++i;
            }
        }
        return text;
    }

    /// <summary>
    /// Align the vertices to be right or center-aligned given the line width specified by NGUIText.lineWidth.
    /// </summary>

    static public void Align(List<Vector3> verts, int indexOffset, float printedWidth)
    {
        switch (alignment)
        {
            case Alignment.Right:
                {
                    float padding = rectWidth - printedWidth;
                    if (padding < 0f)
                        return;
#if UNITY_FLASH
				for (int i = indexOffset; i < verts.size; ++i)
					verts.buffer[i] = verts.buffer[i] + new Vector3(padding, 0f);
#else
                    for (int i = indexOffset; i < verts.Count; ++i)
                    {
                        Vector3 vTemp = verts[i];
                        vTemp.x+= padding;
                        verts[i] = vTemp;
                    }
#endif
                    break;
                }

            case Alignment.Center:
                {
                    float padding = (rectWidth - printedWidth) * 0.5f;
                    if (padding < 0f)
                        return;

                    // Keep it pixel-perfect
                    int diff = Mathf.RoundToInt(rectWidth - printedWidth);
                    int intWidth = Mathf.RoundToInt(rectWidth);

                    bool oddDiff = (diff & 1) == 1;
                    bool oddWidth = (intWidth & 1) == 1;
                    if ((oddDiff && !oddWidth) || (!oddDiff && oddWidth))
                        padding += 0.5f * fontScale;
#if UNITY_FLASH
                    Vector3 vTemp = Vector3.zero;
                    vTemp.x = padding;
                    for (int i = indexOffset; i < verts.size; ++i)
                        verts.buffer[i] = verts.buffer[i] + vTemp;
				
#else
                    for (int i = indexOffset; i < verts.Count; ++i)
                    {
                        Vector3 vTemp = verts[i];
                        vTemp.x += padding;
                        verts[i] = vTemp;
                       // verts.buffer[i].x += padding;
                    }
#endif
                    break;
                }

            case Alignment.Justified:
                {
                    // Printed text needs to reach at least 65% of the width in order to be justified
                    if (printedWidth < rectWidth * 0.65f)
                        return;

                    // There must be some padding involved
                    float padding = (rectWidth - printedWidth) * 0.5f;
                    if (padding < 1f)
                        return;

                    // There must be at least two characters
                    int chars = (verts.Count - indexOffset) / 4;
                    if (chars < 1)
                        return;

                    float progressPerChar = 1f / (chars - 1);
                    float scale = rectWidth / printedWidth;

                    for (int i = indexOffset + 4, charIndex = 1; charIndex < chars; ++charIndex)
                    {
                        if (i >= verts.Count)
                            continue;

                        float x0 = verts[i].x;
                        float x1 = verts[i + 2].x;
                        float w = x1 - x0;
                        float x0a = x0 * scale;
                        float x1a = x0a + w;
                        float x1b = x1 * scale;
                        float x0b = x1b - w;
                        float progress = charIndex * progressPerChar;

                        x0 = Mathf.Lerp(x0a, x0b, progress);
                        x1 = Mathf.Lerp(x1a, x1b, progress);
                        x0 = Mathf.Round(x0);
                        x1 = Mathf.Round(x1);
#if UNITY_FLASH
					 Vector3 vTemp = Vector3.zero;
                        vTemp.x = x0;
                        Vector3 vTemp1 = Vector3.zero;
                        vTemp1.x = x1;
                        verts.buffer[i] = verts.buffer[i] + vTemp;
                        verts.buffer[i + 1] = verts.buffer[i + 1] + vTemp;
                        verts.buffer[i + 2] = verts.buffer[i + 2] + vTemp1;
                        verts.buffer[i + 3] = verts.buffer[i + 3] + vTemp1;
                        i += 4;

#else
                        Vector3 vTemp = verts[i++];
                        vTemp.x = x0;
                        verts[i] = vTemp;

                        vTemp = verts[i++];
                        vTemp.x = x0;
                        verts[i] = vTemp;

                        vTemp = verts[i++];
                        vTemp.x = x1;
                        verts[i] = vTemp;

                        vTemp = verts[i++];
                        vTemp.x = x1;
                        verts[i] = vTemp;

#endif
                    }
                    break;
                }
        }
    }

    /// <summary>
    /// Get the index of the closest character within the provided list of values.
    /// This function first sorts by Y, and only then by X.
    /// </summary>

    static public int GetClosestCharacter(List<Vector3> verts, Vector2 pos)
    {
        // First sort by Y, and only then by X
        float bestX = float.MaxValue;
        float bestY = float.MaxValue;
        int bestIndex = 0;

        for (int i = 0; i < verts.Count; ++i)
        {
            float diffY = Mathf.Abs(pos.y - verts[i].y);
            if (diffY > bestY) continue;

            float diffX = Mathf.Abs(pos.x - verts[i].x);

            if (diffY < bestY)
            {
                bestY = diffY;
                bestX = diffX;
                bestIndex = i;
            }
            else if (diffX < bestX)
            {
                bestX = diffX;
                bestIndex = i;
            }
        }
        return bestIndex;
    }

    /// <summary>
    /// Convenience function that ends the line by either appending a new line character or replacing a space with one.
    /// </summary>

    static public void EndLine(ref StringBuilder s)
    {
        int i = s.Length - 1;
        if (i > 0 && s[i] == ' ') s[i] = '\n';
        else s.Append('\n');
    }

    /// <summary>
    /// Convenience function that ends the line by replacing a space with a newline character.
    /// </summary>

    static void ReplaceSpaceWithNewline(ref StringBuilder s)
    {
        int i = s.Length - 1;
        if (i > 0 && s[i] == ' ') s[i] = '\n';
    }

    /// <summary>
    /// Get the printed size of the specified string. The returned value is in pixels.
    /// </summary>

    static public Vector2 CalculatePrintedSize(string text)
    {
        Vector2 v = Vector2.zero;

        if (!string.IsNullOrEmpty(text))
        {
            // When calculating printed size, get rid of all symbols first since they are invisible anyway
            if (encoding) text = StripSymbols(text);

            // Ensure we have characters to work with
            Prepare(text);

            float x = 0f, y = 0f, maxX = 0f;
            int textLength = text.Length, ch = 0, prev = 0;

            for (int i = 0; i < textLength; ++i)
            {
                ch = text[i];

                // Start a new line
                if (ch == '\n')
                {
                    if (x > maxX) maxX = x;
                    x = 0f;
                    y += finalLineHeight;
                    continue;
                }

                // Skip invalid characters
                if (ch < ' ') continue;

                // See if there is a symbol matching this text
                BMSymbol symbol = useSymbols ? GetSymbol(text, i, textLength) : null;

                if (symbol == null)
                {
                    float w = GetGlyphWidth(ch, prev);

                    if (w != 0f)
                    {
                        w += finalSpacingX;

                        if (Mathf.RoundToInt(x + w) > rectWidth)
                        {
                            if (x > maxX) maxX = x - finalSpacingX;
                            x = w;
                            y += finalLineHeight;
                        }
                        else x += w;

                        prev = ch;
                    }
                }
                else
                {
                    float w = finalSpacingX + symbol.advance * fontScale;

                    if (Mathf.RoundToInt(x + w) > rectWidth)
                    {
                        if (x > maxX) maxX = x - finalSpacingX;
                        x = w;
                        y += finalLineHeight;
                    }
                    else x += w;

                    i += symbol.sequence.Length - 1;
                    prev = 0;
                }
            }

            v.x = ((x > maxX) ? x - finalSpacingX : maxX);
            v.y = (y + finalLineHeight);
        }
        return v;
    }

    static BetterList<float> mSizes = new BetterList<float>();

    /// <summary>
    /// Calculate the character index offset required to print the end of the specified text.
    /// </summary>

    static public int CalculateOffsetToFit(string text)
    {
        if (string.IsNullOrEmpty(text) || rectWidth < 1) return 0;

        Prepare(text);

        int textLength = text.Length, ch = 0, prev = 0;

        for (int i = 0, imax = text.Length; i < imax; ++i)
        {
            // See if there is a symbol matching this text
            BMSymbol symbol = useSymbols ? GetSymbol(text, i, textLength) : null;

            if (symbol == null)
            {
                ch = text[i];
                float w = GetGlyphWidth(ch, prev);
                if (w != 0f) mSizes.Add(finalSpacingX + w);
                prev = ch;
            }
            else
            {
                mSizes.Add(finalSpacingX + symbol.advance * fontScale);
                for (int b = 0, bmax = symbol.sequence.Length - 1; b < bmax; ++b) mSizes.Add(0);
                i += symbol.sequence.Length - 1;
                prev = 0;
            }
        }

        float remainingWidth = rectWidth;
        int currentCharacterIndex = mSizes.size;

        while (currentCharacterIndex > 0 && remainingWidth > 0)
            remainingWidth -= mSizes[--currentCharacterIndex];

        mSizes.Clear();

        if (remainingWidth < 0) ++currentCharacterIndex;
        return currentCharacterIndex;
    }

    /// <summary>
    /// Get the end of line that would fit into a field of given width.
    /// </summary>

    static public string GetEndOfLineThatFits(string text)
    {
        int textLength = text.Length;
        int offset = CalculateOffsetToFit(text);
        return text.Substring(offset, textLength - offset);
    }

    /// <summary>
    /// Text wrapping functionality. The 'width' and 'height' should be in pixels.
    /// </summary>

    static public bool WrapText(string text, out string finalText)
    {
        return WrapText(text, out finalText, false);
    }

    /// <summary>
    ///  修复3.5.5 ResiseHeight 功能无法自动调整高度bug
    /// flagSize用于解决计算文字框大小，当font还没加载好，为空值，导致不能正确计算文字换行（NPC随机说话功能）
    /// </summary>
    static StringBuilder sb = new StringBuilder();
    static public bool CustomWrapText(string text, out string finalText, int flagSize = 0)
    {
        if (rectWidth < 1 || rectHeight < 1)
        {
            finalText = string.Empty;
            return false;
        }

        if (string.IsNullOrEmpty(text)) text = " ";

        float height = (maxLines > 0) ? Mathf.Min(rectHeight, fontSize * maxLines) : rectHeight;
        float sum = fontSize + spacingY;
        int maxLineCount = (maxLines > 0) ? maxLines : 1000000;
        maxLineCount = Mathf.FloorToInt((sum > 0) ? Mathf.Min(maxLineCount, height / sum) : 0);

        if (maxLineCount == 0)
        {
            finalText = string.Empty;
            return false;
        }

        Prepare(text);
        if(sb.Length > 0 )
        {
            sb.Remove(0, sb.Length);
        }
       // StringBuilder sb = new StringBuilder();
        int textLength = text.Length;
        float remainingWidth = rectWidth;
        int start = 0, offset = 0, lineCount = 1, prev = 0;
        bool lineIsEmpty = true;

        // Run through all characters
        for (; offset < textLength; ++offset)
        {
            char ch = text[offset];

            // New line character -- start a new line
            if (ch == '\n')
            {
                if (lineCount == maxLineCount) break;
                remainingWidth = rectWidth;

                // Add the previous word to the final string
                if (start < offset)
                {
                    sb.Append(text, start, offset - start + 1);
                    //sb.Append(text.Substring(start, offset - start + 1));
                }
                else sb.Append(ch);

                lineIsEmpty = true;
                ++lineCount;
                start = offset + 1;
                prev = 0;
                continue;
            }
            
#if Internet
            //国际版开启单词换行功能
            // 换行文字中含有空格的话，会强制换两次行。故注释
            if (ch == ' ' && prev != ' ' && start < offset)
            {
                sb.Append(text, start, offset - start + 1);
                // sb.Append(text.Substring(start, offset - start + 1));
                lineIsEmpty = false;
                start = offset + 1;
                prev = ch;
            }
#endif

            // When encoded symbols such as [RrGgBb] or [-] are encountered, skip past them
            if (encoding && ParseSymbol(text, ref offset)) 
            { 
                --offset; 
                continue; 
            }

            // See if there is a symbol matching this text
            BMSymbol symbol = useSymbols ? GetSymbol(text, offset, textLength) : null;

            // Calculate how wide this symbol or character is going to be
            float glyphWidth;

            if (symbol == null)
            {
                // Find the glyph for this character
                float w = GetGlyphWidth(ch, prev);
                if (w == 0f)
                {
                    if (flagSize == 0)
                    {
                        continue;
                    }
                    else
                    {
                        w = flagSize;
                    }
                }

                glyphWidth = spacingX + w;
            }
            else glyphWidth = spacingX + symbol.advance;

            // Reduce the width
            remainingWidth -= glyphWidth;

            // Doesn't fit?
            if (remainingWidth < 0f)
            {
                // Can't start a new line
                if (lineIsEmpty || lineCount == maxLineCount)
                {
                    // This is the first word on the line -- add it up to the character that fits
                    //sb.Append(text.Substring(start, Mathf.Max(0, offset - start)));
                    sb.Append(text, start, Mathf.Max(0, offset - start));
                    if (lineCount++ == maxLineCount)
                    {
                        start = offset;
                        break;
                    }
                    EndLine(ref sb);

                    // Start a brand-new line
                    lineIsEmpty = true;

                    if (ch == ' ')
                    {
                        start = offset + 1;
                        remainingWidth = rectWidth;
                    }
                    else
                    {
                        start = offset;
                        remainingWidth = rectWidth - glyphWidth;
                    }
                    prev = 0;
                }
                else
                {
                    // Skip all spaces before the word
                    while (start < textLength && text[start] == ' ') ++start;

                    // Revert the position to the beginning of the word and reset the line
                    lineIsEmpty = true;
                    remainingWidth = rectWidth;
                    offset = start - 1;
                    prev = 0;

                    if (lineCount++ == maxLineCount) break;
                    EndLine(ref sb);
                    continue;
                }
            }
            else prev = ch;

            // Advance the offset past the symbol
            if (symbol != null)
            {
                offset += symbol.length - 1;
                prev = 0;
            }
        }

        if (start < offset)
        {
            // sb.Append(text.Substring(start, offset - start));
            sb.Append(text, start, offset - start);
        }
        finalText = sb.ToString();
        return (offset == textLength) || (lineCount <= Mathf.Min(maxLines, maxLineCount));
    }

#if Internet
    /// <summary>
    /// 是否包含中文
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    static public bool bCantainUTF8(string text)
    {
        int len = text.Length;
        for (int i = 0; i < len; i++)
        {
            if (text[i] >= 0x4E00 && text[i] <= 0x9FA5)
            {
                return true;
            }
        }
        return false;
    }
#endif
    /// <summary>
    /// 表情文本处理
    /// </summary>
    /// <param name="text"></param>
    /// <param name="finalText"></param>
    /// <returns></returns>
    static public bool WrapSymbolText(string text, out string finalText, int symbolSize)
    {
        if (rectWidth < 1 || rectHeight < 1)
        {
            finalText = string.Empty;
            return false;
        }

        if (string.IsNullOrEmpty(text)) text = " ";

        float height = (maxLines > 0) ? Mathf.Min(rectHeight, fontSize * maxLines) : rectHeight;
        float sum = fontSize + spacingY;
        int maxLineCount = (maxLines > 0) ? maxLines : 1000000;
        maxLineCount = Mathf.FloorToInt((sum > 0) ? Mathf.Min(maxLineCount, height / sum) : 0);

        if (maxLineCount == 0)
        {
            finalText = string.Empty;
            return false;
        }

        Prepare(text);

        StringBuilder sb = new StringBuilder();
        int textLength = text.Length;
        float remainingWidth = rectWidth;
        int start = 0, offset = 0, lineCount = 1, prev = 0;
        bool lineIsEmpty = true;
        // Run through all characters
        for (; offset < textLength; ++offset)
        {
            char ch = text[offset];

            // New line character -- start a new line
            if (ch == '\n')
            {
                if (lineCount == maxLineCount) break;
                remainingWidth = rectHeight;

                // Add the previous word to the final string
                if (start < offset)
                {
                    //   sb.Append(text.Substring(start, offset - start + 1));
                    sb.Append(text, start, offset - start + 1);
                }
                else sb.Append(ch);

                lineIsEmpty = true;
                ++lineCount;
                start = offset + 1;
                prev = 0;
                continue;
            }

           
            // If this marks the end of a word, add it to the final string.
            if (ch == ' ' && prev != ' ' && start < offset)
            {
                sb.Append(text, start, offset - start + 1);
                // sb.Append(text.Substring(start, offset - start + 1));
                lineIsEmpty = false;
                start = offset + 1;
                prev = ch;
            }
            

            // When encoded symbols such as [RrGgBb] or [-] are encountered, skip past them
            if (encoding && ParseSymbol(text, ref offset)) { --offset; continue; }

            // See if there is a symbol matching this text
            BMSymbol symbol = useSymbols ? GetSymbol(text, offset, textLength) : null;

            // Calculate how wide this symbol or character is going to be
            float glyphWidth;

            if (symbol == null)
            {
                // Find the glyph for this character
                float w = GetGlyphWidth(ch, prev);
                if (w == 0f) continue;
                glyphWidth = spacingX + w;
            }
            else glyphWidth = spacingX + symbol.advance * (symbolSize / bitmapFont.defaultSize);

            // Reduce the width
            remainingWidth -= glyphWidth;

            // Doesn't fit?
            if (remainingWidth < 0f)
            {
                // Can't start a new line
                if (lineIsEmpty || lineCount == maxLineCount)
                {
                    // This is the first word on the line -- add it up to the character that fits
                    //sb.Append(text.Substring(start, Mathf.Max(0, offset - start)));
                    sb.Append(text, start, Mathf.Max(0, offset - start));
                    if (lineCount++ == maxLineCount)
                    {
                        start = offset;
                        break;
                    }
                    EndLine(ref sb);

                    // Start a brand-new line
                    lineIsEmpty = true;

                    if (ch == ' ')
                    {
                        start = offset + 1;
                        remainingWidth = rectWidth;
                    }
                    else
                    {
                        start = offset;
                        remainingWidth = rectWidth - glyphWidth;
                    }
                    prev = 0;
                }
                else
                {
                    // Skip all spaces before the word
                    while (start < textLength && text[start] == ' ') ++start;

                    // Revert the position to the beginning of the word and reset the line
                    lineIsEmpty = true;
                    remainingWidth = rectWidth;
                    offset = start - 1;
                    prev = 0;

                    if (lineCount++ == maxLineCount) break;
                    EndLine(ref sb);
                    continue;
                }
            }
            else prev = ch;

            // Advance the offset past the symbol
            if (symbol != null)
            {
                offset += symbol.length - 1;
                prev = 0;
            }
        }

        if (start < offset)
        {
            sb.Append(text, start, offset - start);
         //   sb.Append(text.Substring(start, offset - start));
        }
        finalText = sb.ToString();
        return (offset == textLength) || (lineCount <= Mathf.Min(maxLines, maxLineCount));
    }

    /// <summary>
    /// Text wrapping functionality. The 'width' and 'height' should be in pixels.
    /// </summary>

    static public bool WrapText(string text, out string finalText, bool keepCharCount)
    {
        if (rectWidth < 1 || rectHeight < 1 || finalLineHeight < 1f)
        {
            finalText = string.Empty;
            return false;
        }

        float height = (maxLines > 0) ? Mathf.Min(rectHeight, finalLineHeight * maxLines) : rectHeight;
        int maxLineCount = (maxLines > 0) ? maxLines : 1000000;
        maxLineCount = Mathf.FloorToInt(Mathf.Min(maxLineCount, height / finalLineHeight) + 0.01f);

        if (maxLineCount == 0)
        {
            finalText = string.Empty;
            return false;
        }

        if (string.IsNullOrEmpty(text)) text = " ";
        Prepare(text);

        StringBuilder sb = new StringBuilder();
        int textLength = text.Length;
        float remainingWidth = rectWidth;
        int start = 0, offset = 0, lineCount = 1, prev = 0;
        bool lineIsEmpty = true;
        bool fits = true;
        bool eastern = false;

        // Run through all characters
        for (; offset < textLength; ++offset)
        {
            char ch = text[offset];
            if (ch > 12287) eastern = true;

            // New line character -- start a new line
            if (ch == '\n')
            {
                if (lineCount == maxLineCount) break;
                remainingWidth = rectWidth;

                // Add the previous word to the final string
                if (start < offset)
                {
                    sb.Append(text,start, offset - start + 1);
                 //   sb.Append(text.Substring(start, offset - start + 1));
                }
                else sb.Append(ch);

                lineIsEmpty = true;
                ++lineCount;
                start = offset + 1;
                prev = 0;
                continue;
            }

            // When encoded symbols such as [RrGgBb] or [-] are encountered, skip past them
            if (encoding && ParseSymbol(text, ref offset)) { --offset; continue; }

            // See if there is a symbol matching this text
            BMSymbol symbol = useSymbols ? GetSymbol(text, offset, textLength) : null;

            // Calculate how wide this symbol or character is going to be
            float glyphWidth;

            if (symbol == null)
            {
                // Find the glyph for this character
                float w = GetGlyphWidth(ch, prev);
                if (w == 0f) continue;
                glyphWidth = finalSpacingX + w;
            }
            else glyphWidth = finalSpacingX + symbol.advance * fontScale;

            // Reduce the width
            remainingWidth -= glyphWidth;

            // If this marks the end of a word, add it to the final string.
            if (ch == ' ' && !eastern)
            {
                if (prev == ' ')
                {
                    sb.Append(' ');
                    start = offset + 1;
                }
                else if (prev != ' ' && start < offset)
                {
                    int end = offset - start + 1;

                    // Last word on the last line should not include an invisible character
                    if (lineCount == maxLineCount && remainingWidth <= 0f && offset < textLength && text[offset] <= ' ') --end;
                    sb.Append(text, start, end);
                    // sb.Append(text.Substring(start, end));
                    lineIsEmpty = false;
                    start = offset + 1;
                    prev = ch;
                }
            }

            // Doesn't fit?
            if (Mathf.RoundToInt(remainingWidth) < 0)
            {
                // Can't start a new line
                if (lineIsEmpty || lineCount == maxLineCount)
                {
                    if (ch != ' ' && !eastern)
                    {
                        fits = false;
                        break;
                    }

                    // This is the first word on the line -- add it up to the character that fits
                    //sb.Append(text.Substring(start, Mathf.Max(0, offset - start)));
                    sb.Append(text, start, Mathf.Max(0, offset - start));
                    if (lineCount++ == maxLineCount)
                    {
                        start = offset;
                        break;
                    }

                    if (keepCharCount) ReplaceSpaceWithNewline(ref sb);
                    else EndLine(ref sb);

                    // Start a brand-new line
                    lineIsEmpty = true;

                    if (ch == ' ')
                    {
                        start = offset + 1;
                        remainingWidth = rectWidth;
                    }
                    else
                    {
                        start = offset;
                        remainingWidth = rectWidth - glyphWidth;
                    }
                    prev = 0;
                }
                else
                {
                    // Revert the position to the beginning of the word and reset the line
                    lineIsEmpty = true;
                    remainingWidth = rectWidth;
                    offset = start - 1;
                    prev = 0;

                    if (lineCount++ == maxLineCount) break;
                    if (keepCharCount) ReplaceSpaceWithNewline(ref sb);
                    else EndLine(ref sb);
                    continue;
                }
            }
            else prev = ch;

            // Advance the offset past the symbol
            if (symbol != null)
            {
                offset += symbol.length - 1;
                prev = 0;
            }
        }

        if (start < offset)
        {
            sb.Append(text, start, offset - start);
            //   sb.Append(text.Substring(start, offset - start));
        }
        finalText = sb.ToString();
        return fits && ((offset == textLength) || (lineCount <= Mathf.Min(maxLines, maxLineCount)));
    }

    static Color32 s_c0, s_c1;

    /// <summary>
    /// Print the specified text into the buffers.
    /// </summary>

    static public void Print(string text, List<Vector3> verts, List<Vector2> uvs, List<Color32> cols)
    {
        if (string.IsNullOrEmpty(text))
            return;

        int indexOffset = verts.Count;
        Prepare(text);

        // Start with the white tint
        mColors.Add(Color.white);

        int ch = 0, prev = 0;
        float x = 0f, y = 0f, maxX = 0f;
        float sizeF = finalSize;

        Color gb = tint * gradientBottom;
        Color gt = tint * gradientTop;
        Color32 uc = tint;
        int textLength = text.Length;

        Rect uvRect = new Rect();
        float invX = 0f, invY = 0f;
        float sizePD = sizeF * pixelDensity;

        // Advanced symbol support contributed by Rudy Pangestu.
        bool subscript = false;
        int subscriptMode = 0;  // 0 = normal, 1 = subscript, 2 = superscript
        bool bold = false;
        bool italic = false;
        bool underline = false;
        bool strikethrough = false;
        const float sizeShrinkage = 0.75f;

        float v0x;
        float v1x;
        float v1y;
        float v0y;
        float prevX = 0;

        if (bitmapFont != null)
        {
            uvRect = bitmapFont.uvRect;
            invX = uvRect.width / bitmapFont.texWidth;
            invY = uvRect.height / bitmapFont.texHeight;
        }

        for (int i = 0; i < textLength; ++i)
        {
            ch = text[i];

            prevX = x;

            // New line character -- skip to the next line
            if (ch == '\n')
            {
                if (x > maxX) maxX = x;

                if (alignment != Alignment.Left)
                {
                    Align(verts, indexOffset, x - finalSpacingX);
                    indexOffset = verts.Count;
                }

                x = 0;
                y += finalLineHeight;
                prev = 0;
                continue;
            }

            // Invalid character -- skip it
            if (ch < ' ')
            {
                prev = ch;
                continue;
            }

            // Color changing symbol
            if (encoding && ParseSymbol(text, ref i, mColors, premultiply, ref subscriptMode, ref bold, ref italic, ref underline, ref strikethrough))
            {
                Color fc = tint * mColors[mColors.size - 1];
                uc = fc;

                if (gradient)
                {
                    gb = gradientBottom * fc;
                    gt = gradientTop * fc;
                }
                --i;
                continue;
            }

            // See if there is a symbol matching this text
            BMSymbol symbol = useSymbols ? GetSymbol(text, i, textLength) : null;

            if (symbol != null)
            {
                v0x = x + symbol.offsetX * fontScale;
                v1x = v0x + symbol.width * fontScale;
                v1y = -(y + symbol.offsetY * fontScale);
                v0y = v1y - symbol.height * fontScale;

                // Doesn't fit? Move down to the next line
                if (Mathf.RoundToInt(x + symbol.advance * fontScale) > rectWidth)
                {
                    if (x == 0f)
                        return;

                    if (alignment != Alignment.Left && indexOffset < verts.Count)
                    {
                        Align(verts, indexOffset, x - finalSpacingX);
                        indexOffset = verts.Count;
                    }

                    v0x -= x;
                    v1x -= x;
                    v0y -= finalLineHeight;
                    v1y -= finalLineHeight;

                    x = 0;
                    y += finalLineHeight;
                    prevX = 0;
                }
                Vector3 vTemp = Vector3.zero;
                vTemp.x = v0x;
                vTemp.y = v0y;
                verts.Add(vTemp);
                vTemp.x = v0x;
                vTemp.y = v1y;
                verts.Add(vTemp);
                vTemp.x = v1x;
                vTemp.y = v1y;
                verts.Add(vTemp);
                vTemp.x = v1x;
                vTemp.y = v0y;
                verts.Add(vTemp);

                x += finalSpacingX + symbol.advance * fontScale;
                i += symbol.length - 1;
                prev = 0;

                if (uvs != null)
                {
                    Rect uv = symbol.uvRect;

                    float u0x = uv.xMin;
                    float u0y = uv.yMin;
                    float u1x = uv.xMax;
                    float u1y = uv.yMax;
                    Vector2 vTemp2 = Vector2.zero;
                    vTemp2.x = u0x;
                    vTemp2.y = u0y;
                    uvs.Add(vTemp2);
                    vTemp2.x = u0x;
                    vTemp2.y = u1y;
                    uvs.Add(vTemp2);
                    vTemp2.x = u1x;
                    vTemp2.y = u1y;
                    uvs.Add(vTemp2);
                    vTemp2.x = u1x;
                    vTemp2.y = u0y;
                    uvs.Add(vTemp2);
                }

                if (cols != null)
                {
                    if (symbolStyle == SymbolStyle.Colored)
                    {
                        for (int b = 0; b < 4; ++b) cols.Add(uc);
                    }
                    else
                    {
                        Color32 col = Color.white;
                        col.a = uc.a;
                        for (int b = 0; b < 4; ++b) cols.Add(col);
                    }
                }
            }
            else // No symbol present
            {
                GlyphInfo glyph = GetGlyph(ch, prev);
                if (glyph == null) continue;
                prev = ch;

                if (subscriptMode != 0)
                {
                    glyph.v0.x *= sizeShrinkage;
                    glyph.v0.y *= sizeShrinkage;
                    glyph.v1.x *= sizeShrinkage;
                    glyph.v1.y *= sizeShrinkage;

                    if (subscriptMode == 1)
                    {
                        glyph.v0.y -= fontScale * fontSize * 0.4f;
                        glyph.v1.y -= fontScale * fontSize * 0.4f;
                    }
                    else
                    {
                        glyph.v0.y += fontScale * fontSize * 0.05f;
                        glyph.v1.y += fontScale * fontSize * 0.05f;
                    }
                }

                float y0 = glyph.v0.y;
                float y1 = glyph.v1.y;

                v0x = glyph.v0.x + x;
                v0y = glyph.v0.y - y;
                v1x = glyph.v1.x + x;
                v1y = glyph.v1.y - y;

                float w = glyph.advance;
                if (finalSpacingX < 0f) w += finalSpacingX;

                // Doesn't fit? Move down to the next line
                if (Mathf.RoundToInt(x + w) > rectWidth)
                {
                    if (x == 0f)
                        return;

                    if (alignment != Alignment.Left && indexOffset < verts.Count)
                    {
                        Align(verts, indexOffset, x - finalSpacingX);
                        indexOffset = verts.Count;
                    }

                    v0x -= x;
                    v1x -= x;
                    v0y -= finalLineHeight;
                    v1y -= finalLineHeight;

                    x = 0;
                    y += finalLineHeight;
                    prevX = 0;
                }

                if (ch == ' ')
                {
                    if (underline)
                    {
                        ch = '_';
                    }
                    else if (strikethrough)
                    {
                        ch = '-';
                    }
                }

                // Advance the position
                x += (subscriptMode == 0) ? finalSpacingX + glyph.advance :
                    (finalSpacingX + glyph.advance) * sizeShrinkage;

                // No need to continue if this is a space character
                if (ch == ' ') continue;

                // Texture coordinates
                if (uvs != null)
                {
                    if (bitmapFont != null)
                    {
                        glyph.u0.x = uvRect.xMin + invX * glyph.u0.x;
                        glyph.u1.x = uvRect.xMin + invX * glyph.u1.x;
                        glyph.u0.y = uvRect.yMax - invY * glyph.u0.y;
                        glyph.u1.y = uvRect.yMax - invY * glyph.u1.y;
                    }

                    for (int j = 0, jmax = (bold ? 4 : 1); j < jmax; ++j)
                    {
                        if (glyph.rotatedUVs)
                        {
                            uvs.Add(glyph.u0);
                            Vector2 vTemp2 = Vector2.zero;
                            vTemp2.x = glyph.u1.x;
                            vTemp2.y = glyph.u0.y;
                            uvs.Add(vTemp2);
                            uvs.Add(glyph.u1);
                             vTemp2.x = glyph.u0.x;
                            vTemp2.y = glyph.u1.y;
                            uvs.Add(vTemp2);
                        }
                        else
                        {
                            uvs.Add(glyph.u0);
                            Vector2 vTemp = Vector2.zero;
                            vTemp.x = glyph.u0.x;
                            vTemp.y = glyph.u1.y;
                            uvs.Add(vTemp);
                            uvs.Add(glyph.u1);
                            vTemp.x = glyph.u1.x;
                            vTemp.y = glyph.u0.y;
                            uvs.Add(vTemp);
                        }
                    }
                }

                // Vertex colors
                if (cols != null)
                {
                    if (glyph.channel == 0 || glyph.channel == 15)
                    {
                        if (gradient)
                        {
                            float min = sizePD + y0 / fontScale;
                            float max = sizePD + y1 / fontScale;

                            min /= sizePD;
                            max /= sizePD;

                            s_c0 = Color.Lerp(gb, gt, min);
                            s_c1 = Color.Lerp(gb, gt, max);

                            for (int j = 0, jmax = (bold ? 4 : 1); j < jmax; ++j)
                            {
                                cols.Add(s_c0);
                                cols.Add(s_c1);
                                cols.Add(s_c1);
                                cols.Add(s_c0);
                            }
                        }
                        else
                        {
                            for (int j = 0, jmax = (bold ? 16 : 4); j < jmax; ++j)
                                cols.Add(uc);
                        }
                    }
                    else
                    {
                        // Packed fonts come as alpha masks in each of the RGBA channels.
                        // In order to use it we need to use a special shader.
                        //
                        // Limitations:
                        // - Effects (drop shadow, outline) will not work.
                        // - Should not be a part of the atlas (eastern fonts rarely are anyway).
                        // - Lower color precision

                        Color col = uc;

                        col *= 0.49f;

                        switch (glyph.channel)
                        {
                            case 1: col.b += 0.51f; break;
                            case 2: col.g += 0.51f; break;
                            case 4: col.r += 0.51f; break;
                            case 8: col.a += 0.51f; break;
                        }

                        Color32 c = col;
                        for (int j = 0, jmax = (bold ? 16 : 4); j < jmax; ++j)
                            cols.Add(c);
                    }
                }

                // Bold and italic contributed by Rudy Pangestu.
                if (!bold)
                {
                    if (!italic)
                    {
                        Vector3 vTemp = Vector3.zero;
                        vTemp.x = v0x;
                        vTemp.y = v0y;
                        verts.Add(vTemp);
                        vTemp.x = v0x;
                        vTemp.y = v1y;
                        verts.Add(vTemp);
                        vTemp.x = v1x;
                        vTemp.y = v1y;
                        verts.Add(vTemp);
                        vTemp.x = v1x;
                        vTemp.y = v0y;
                        verts.Add(vTemp);
                    }
                    else // Italic
                    {
                        float slant = fontSize * 0.1f * ((v1y - v0y) / fontSize);
                        Vector3 vTemp = Vector3.zero;
                        vTemp.x = v0x - slant;
                        vTemp.y = v0y;
                        verts.Add(vTemp);
                        vTemp.x = v0x + slant;
                        vTemp.y = v1y;
                        verts.Add(vTemp);
                        vTemp.x = v1x + slant;
                        vTemp.y = v1y;
                        verts.Add(vTemp);
                        vTemp.x = v1x - slant;
                        vTemp.y = v0y;
                        verts.Add(vTemp);
                    }
                }
                else // Bold
                {
                    for (int j = 0; j < 4; ++j)
                    {
                        float a = mBoldOffset[j * 2];
                        float b = mBoldOffset[j * 2 + 1];

                        float slant = a + (italic ? fontSize * 0.1f * ((v1y - v0y) / fontSize) : 0f);
                        Vector3 vTemp = Vector3.zero;
                        vTemp.x = v0x - slant;
                        vTemp.y = v0y + b;
                        verts.Add(vTemp);
                        vTemp.x = v0x + slant;
                        vTemp.y = v1y + b;
                        verts.Add(vTemp);
                        vTemp.x = v1x + slant;
                        vTemp.y = v1y + b;
                        verts.Add(vTemp);
                        vTemp.x = v1x - slant;
                        vTemp.y = v0y + b;
                        verts.Add(vTemp);
                    }
                }

                // Underline and strike-through contributed by Rudy Pangestu.
                if (underline || strikethrough)
                {
                    GlyphInfo dash = GetGlyph(strikethrough ? '-' : '_', prev);
                    if (dash == null) continue;

                    if (uvs != null)
                    {
                        if (bitmapFont != null)
                        {
                            dash.u0.x = uvRect.xMin + invX * dash.u0.x;
                            dash.u1.x = uvRect.xMin + invX * dash.u1.x;
                            dash.u0.y = uvRect.yMax - invY * dash.u0.y;
                            dash.u1.y = uvRect.yMax - invY * dash.u1.y;
                        }

                        float cx = (dash.u0.x + dash.u1.x) * 0.5f;
                        float cy = (dash.u0.y + dash.u1.y) * 0.5f;
                        Vector2 vTemp2 = Vector2.zero;
                        vTemp2.x = cx;
                        vTemp2.y = cy;
                        uvs.Add(vTemp2);
                        uvs.Add(vTemp2);
                        uvs.Add(vTemp2);
                        uvs.Add(vTemp2);
                    }

                    if (subscript && strikethrough)
                    {
                        v0y = (-y + dash.v0.y) * sizeShrinkage;
                        v1y = (-y + dash.v1.y) * sizeShrinkage;
                    }
                    else
                    {
                        v0y = (-y + dash.v0.y);
                        v1y = (-y + dash.v1.y);
                    }
                    Vector3 vTemp = Vector3.zero;
                    vTemp.x = prevX;
                    vTemp.y = v0y;
                    verts.Add(vTemp);
                    vTemp.x = prevX;
                    vTemp.y = v1y;
                    verts.Add(vTemp);
                    vTemp.x = x;
                    vTemp.y = v1y;
                    verts.Add(vTemp);
                    vTemp.x = x;
                    vTemp.y = v0y;
                    verts.Add(vTemp);

                    Color tint2 = uc;

                    if (strikethrough)
                    {
                        tint2.r *= 0.5f;
                        tint2.g *= 0.5f;
                        tint2.b *= 0.5f;
                    }
                    tint2.a *= 0.75f;
                    Color32 uc2 = tint2;

                    cols.Add(uc2);
                    cols.Add(uc);
                    cols.Add(uc);
                    cols.Add(uc2);
                }
            }
        }

        if (alignment != Alignment.Left && indexOffset < verts.Count)
        {
            Align(verts, indexOffset, x - finalSpacingX);
            indexOffset = verts.Count;
        }
        mColors.Clear();
    }

    static float[] mBoldOffset = new float[]
	{
		-0.5f, 0f, 0.5f, 0f,
		0f, -0.5f, 0f, 0.5f
	};

    /// <summary>
    /// Print character positions and indices into the specified buffer. Meant to be used with the "find closest vertex" calculations.
    /// </summary>

    static public void PrintCharacterPositions(string text, List<Vector3> verts, List<int> indices)
    {
        if (string.IsNullOrEmpty(text)) text = " ";

        Prepare(text);

        float x = 0f, y = 0f, maxX = 0f, halfSize = fontSize * fontScale * 0.5f;
        int textLength = text.Length, indexOffset = verts.Count, ch = 0, prev = 0;
        Vector3 vTemp = Vector3.zero;

        for (int i = 0; i < textLength; ++i)
        {
            ch = text[i];
            vTemp.x = x;
            vTemp.y = -y - halfSize;
            verts.Add(vTemp);
            indices.Add(i);

            if (ch == '\n')
            {
                if (x > maxX) maxX = x;

                if (alignment != Alignment.Left)
                {
                    Align(verts, indexOffset, x - finalSpacingX);
                    indexOffset = verts.Count;
                }

                x = 0;
                y += finalLineHeight;
                prev = 0;
                continue;
            }
            else if (ch < ' ')
            {
                prev = 0;
                continue;
            }

            if (encoding && ParseSymbol(text, ref i))
            {
                --i;
                continue;
            }

            // See if there is a symbol matching this text
            BMSymbol symbol = useSymbols ? GetSymbol(text, i, textLength) : null;

            if (symbol == null)
            {
                float w = GetGlyphWidth(ch, prev);

                if (w != 0f)
                {
                    w += finalSpacingX;

                    if (Mathf.RoundToInt(x + w) > rectWidth)
                    {
                        if (x == 0f)
                            return;

                        if (alignment != Alignment.Left && indexOffset < verts.Count)
                        {
                            Align(verts, indexOffset, x - finalSpacingX);
                            indexOffset = verts.Count;
                        }

                        x = w;
                        y += finalLineHeight;
                    }
                    else x += w;
                    Vector3 vTemp2 = Vector3.zero;
                    vTemp2.x = x;
                    vTemp2.y = -y - halfSize;
                    verts.Add(vTemp2);
                    indices.Add(i + 1);
                    prev = ch;
                }
            }
            else
            {
                float w = symbol.advance * fontScale + finalSpacingX;

                if (Mathf.RoundToInt(x + w) > rectWidth)
                {
                    if (x == 0f)
                        return;

                    if (alignment != Alignment.Left && indexOffset < verts.Count)
                    {
                        Align(verts, indexOffset, x - finalSpacingX);
                        indexOffset = verts.Count;
                    }

                    x = w;
                    y += finalLineHeight;
                }
                else x += w;
                Vector3 vTemp2 = Vector3.zero;
                vTemp2.x = x;
                vTemp2.y = -y - halfSize;

                verts.Add(vTemp2);
                indices.Add(i + 1);
                i += symbol.sequence.Length - 1;
                prev = 0;
            }
        }

        if (alignment != Alignment.Left && indexOffset < verts.Count)
            Align(verts, indexOffset, x - finalSpacingX);
    }

    /// <summary>
    /// Print the caret and selection vertices. Note that it's expected that 'text' has been stripped clean of symbols.
    /// </summary>

    static public void PrintCaretAndSelection(string text, int start, int end, List<Vector3> caret, List<Vector3> highlight)
    {
        if (string.IsNullOrEmpty(text)) text = " ";

        Prepare(text);

        int caretPos = end;

        if (start > end)
        {
            end = start;
            start = caretPos;
        }

        float x = 0f, y = 0f, maxX = 0f, fs = fontSize * fontScale;
        int caretOffset = (caret != null) ? caret.Count : 0;
        int highlightOffset = (highlight != null) ? highlight.Count : 0;
        int textLength = text.Length, index = 0, ch = 0, prev = 0;
        bool highlighting = false, caretSet = false;

        Vector2 last0 = Vector2.zero;
        Vector2 last1 = Vector2.zero;

        for (; index < textLength; ++index)
        {
            // Print the caret
            if (caret != null && !caretSet && caretPos <= index)
            {
                caretSet = true;
                Vector3 vTemp = Vector3.zero;
                vTemp.x = x - 1f;
                vTemp.y = -y - fs;
                caret.Add(vTemp);
                vTemp.x = x - 1f;
                vTemp.y = -y;
                caret.Add(vTemp);
                vTemp.x = x + 1f;
                vTemp.y = -y;
                caret.Add(vTemp);
                vTemp.x = x + 1f;
                vTemp.y = -y - fs;
                caret.Add(vTemp);
            }

            ch = text[index];

            if (ch == '\n')
            {
                // Used for alignment purposes
                if (x > maxX) maxX = x;

                // Align the caret
                if (caret != null && caretSet)
                {
                    if (alignment != Alignment.Left) Align(caret, caretOffset, x - finalSpacingX);
                    caret = null;
                }

                if (highlight != null)
                {
                    if (highlighting)
                    {
                        // Close the selection on this line
                        highlighting = false;
                        highlight.Add(last1);
                        highlight.Add(last0);
                    }
                    else if (start <= index && end > index)
                    {
                        // This must be an empty line. Add a narrow vertical highlight.
                        Vector3 vTemp = Vector3.zero;
                        vTemp.x =x;
                        vTemp.y = -y - fs;
                        highlight.Add(vTemp);
                        vTemp.x = x;
                        vTemp.y = -y;
                        highlight.Add(vTemp);
                        vTemp.x = x + 2f;
                        vTemp.y = -y;
                        highlight.Add(vTemp);
                        vTemp.x = x + 2f;
                        vTemp.y = -y - fs;
                        highlight.Add(vTemp);
                    }

                    // Align the highlight
                    if (alignment != Alignment.Left && highlightOffset < highlight.Count)
                    {
                        Align(highlight, highlightOffset, x - finalSpacingX);
                        highlightOffset = highlight.Count;
                    }
                }

                x = 0;
                y += finalLineHeight;
                prev = 0;
                continue;
            }
            else if (ch < ' ')
            {
                prev = 0;
                continue;
            }

            if (encoding && ParseSymbol(text, ref index))
            {
                --index;
                continue;
            }

            // See if there is a symbol matching this text
            BMSymbol symbol = useSymbols ? GetSymbol(text, index, textLength) : null;
            float w = (symbol != null) ? symbol.advance * fontScale : GetGlyphWidth(ch, prev);

            if (w != 0f)
            {
                float v0x = x;
                float v1x = x + w;
                float v0y = -y - fs;
                float v1y = -y;

                if (Mathf.RoundToInt(v1x + finalSpacingX) > rectWidth)
                {
                    if (x == 0f)
                        return;

                    // Used for alignment purposes
                    if (x > maxX) maxX = x;

                    // Align the caret
                    if (caret != null && caretSet)
                    {
                        if (alignment != Alignment.Left) Align(caret, caretOffset, x - finalSpacingX);
                        caret = null;
                    }

                    if (highlight != null)
                    {
                        if (highlighting)
                        {
                            // Close the selection on this line
                            highlighting = false;
                            highlight.Add(last1);
                            highlight.Add(last0);
                        }
                        else if (start <= index && end > index)
                        {
                            // This must be an empty line. Add a narrow vertical highlight.
                            Vector3 vTemp = Vector3.zero;
                            vTemp.x = x;
                            vTemp.y = -y - fs;
                            highlight.Add(vTemp);
     
                            vTemp.x = x;
                            vTemp.y = -y;
                            highlight.Add(vTemp);

                            vTemp.x = x + 2f;
                            vTemp.y =  -y;
                            highlight.Add(vTemp); 
             
                            vTemp.x = x + 2f;
                            vTemp.y = -y - fs;
                            highlight.Add(vTemp);
                        }

                        // Align the highlight
                        if (alignment != Alignment.Left && highlightOffset < highlight.Count)
                        {
                            LogSystem.Log("Aligning");
                            Align(highlight, highlightOffset, x - finalSpacingX);
                            highlightOffset = highlight.Count;
                        }
                    }

                    v0x -= x;
                    v1x -= x;
                    v0y -= finalLineHeight;
                    v1y -= finalLineHeight;

                    x = 0;
                    y += finalLineHeight;
                }

                x += w + finalSpacingX;

                // Print the highlight
                if (highlight != null)
                {
                    if (start > index || end <= index)
                    {
                        if (highlighting)
                        {
                            // Finish the highlight
                            highlighting = false;
                            highlight.Add(last1);
                            highlight.Add(last0);
                        }
                    }
                    else if (!highlighting)
                    {
                        // Start the highlight
                        highlighting = true;
                        Vector3 vTemp = Vector3.zero;
                        vTemp.x =v0x ;
                        vTemp.y = v0y;
                        highlight.Add(vTemp);
                        vTemp.x =v0x ;
                        vTemp.y = v1y;
                        highlight.Add(vTemp);
                    }
                }

                // Save what the character ended with
                Vector2 vTemp2 = Vector2.zero;
                vTemp2.x = v1x;
                vTemp2.y = v0y;
                last0 = vTemp2;
                vTemp2.x = v1x;
                vTemp2.y = v1y;
                last1 = vTemp2;
                prev = ch;
            }
        }

        // Ensure we always have a caret
        if (caret != null)
        {
            if (!caretSet)
            {
                Vector3 vTemp = Vector3.zero;
                vTemp.x =x - 1f ;
                vTemp.y = -y - fs;
                caret.Add(vTemp);
                vTemp.x = x - 1f;
                vTemp.y = -y;
                caret.Add(vTemp);
                vTemp.x = x + 1f;
                vTemp.y = -y;
                caret.Add(vTemp);
                vTemp.x = x + 1f;
                vTemp.y = -y - fs;
                caret.Add(vTemp);
            }

            if (alignment != Alignment.Left)
                Align(caret, caretOffset, x - finalSpacingX);
        }

        // Close the selection
        if (highlight != null)
        {
            if (highlighting)
            {
                // Finish the highlight
                highlight.Add(last1);
                highlight.Add(last0);
            }
            else if (start < index && end == index)
            {
                // Happens when highlight ends on an empty line. Highlight it with a thin line.
                Vector3 vTemp = Vector3.zero;
                vTemp.x =x ;
                vTemp.y = -y - fs;
                highlight.Add(vTemp);
                vTemp.x = x;
                vTemp.y = -y;
                highlight.Add(vTemp);
                vTemp.x = x + 2f;
                vTemp.y = -y;
                highlight.Add(vTemp);
                vTemp.x = x + 2f;
                vTemp.y = -y - fs;
                highlight.Add(vTemp);
            }

            // Align the highlight
            if (alignment != Alignment.Left && highlightOffset < highlight.Count)
                Align(highlight, highlightOffset, x - finalSpacingX);
        }
    }
}
#pragma warning restore 0618

