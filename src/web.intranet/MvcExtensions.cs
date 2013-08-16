//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Web.Mvc;

namespace Sherlock.Web.Intranet
{
    /// <summary>
    /// Defines extension methods for use in MVC applications.
    /// </summary>
    internal static class MvcExtensions
    {
        /// <summary>
        /// Creates a select list from the members of an enumeration.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="enumObj">The enumeration object that will be set as the default selection.</param>
        /// <returns>A list containing select items for each of the enumeration values.</returns>
        public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                         select new 
                         { 
                             Id = e, 
                             Name = e.ToString() 
                         };

            return new SelectList(values, "Id", "Name", enumObj);
        }
    }
}
