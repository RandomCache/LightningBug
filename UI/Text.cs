using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightningBug.UI
{
    class Text
    {
        public static string WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');
            StringBuilder newSentence = new StringBuilder();
            string line = "";
            Vector2 totalLineSize, tempSize;
            totalLineSize = tempSize = Vector2.Zero;
            foreach (string word in words)
            {
                tempSize = spriteFont.MeasureString(word);
                totalLineSize += tempSize;
                // if this new word will put us over the length add it to the next line
                if (totalLineSize.X > maxLineWidth)
                {
                    // If this is the first word of the line, don't add an empty line
                    if (line.Length > 0)
                    {
                        newSentence.AppendLine(line);
                        line = string.Empty;
                        totalLineSize = tempSize;
                    }

                    // if the single word is longer than the line, cut it up
                    if (tempSize.X > maxLineWidth)
                    {
                        string tempStr = string.Empty;
                        int i;
                        for (i = 0; i < word.Length; i++)
                        {
                            tempStr += word[i];
                            Vector2 mytempstrsize = spriteFont.MeasureString(tempStr);
                            if (mytempstrsize.X >= maxLineWidth)
                            {
                                if (i > 1)
                                {
                                    tempStr = tempStr.Substring(0, tempStr.Length - 1);
                                }

                                newSentence.AppendLine(tempStr);
                                // line will be the beginning of the next line
                                if (i < word.Length)
                                {
                                    line = word.Substring(i) + " ";
                                    totalLineSize = spriteFont.MeasureString(line);
                                }
                                else
                                {
                                    line = string.Empty;
                                    totalLineSize = Vector2.Zero;
                                }
                                break;
                            }
                        }
                        // If it's longer than the maxWidth, but not an entire character more just add the whole thing
                        if (i == word.Length)
                        {
                            newSentence.AppendLine(tempStr);
                            line = string.Empty;
                            totalLineSize = Vector2.Zero;
                        }
                    }
                    else
                        line += word + " ";
                }
                else if (word != words.Last())
                    line += word + " ";
                else
                    line += word;
            }

            if (line.Length > 0)
                newSentence.Append(line);
            return newSentence.ToString();
        }
    }
}
