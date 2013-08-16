//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Sherlock.Shared.Core.Reporting
{
   /// <summary>
   /// Defines a set of extension methods used for translating normal text to
   /// Html text.
   /// </summary>
   internal static class HtmlHelpers
   {
      /// <summary>
      /// Replaces the new lines by HTML line breaks.
      /// </summary>
      /// <param name="text">The text that must be translated.</param>
      /// <returns>
      ///   A new string where the new line characters are replaced by the HTLM line break code.
      /// </returns>
      public static string ReplaceNewLinesByHtmlLineBreaks(this string text)
      {
         return text.Replace(Environment.NewLine, "<br />");
      }

      /// <summary>
      /// Replaces a space by a HTML non breaking space.
      /// </summary>
      /// <param name="text">The text that must be translated.</param>
      /// <returns>
      ///   A new string where the space character is replaced by 1 Html non-breaking space.
      /// </returns>
      public static string ReplaceSpaceByHtmlNonBreakingSpaces(this string text)
      {
         return text.Replace(" ", "&nbsp;");
      }

      /// <summary>
      /// Replaces the tabs by HTML non breaking spaces.
      /// </summary>
      /// <param name="text">The text that must be translated.</param>
      /// <returns>
      ///   A new string where the tab character is replaced by 4 Html non-breaking spaces.
      /// </returns>
      public static string ReplaceTabsByHtmlNonBreakingSpaces(this string text)
      {
         return text.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
      }
   }
}
