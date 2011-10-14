﻿using System;
using System.Linq;
using Sitecore.Search;
using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Search.Search;

namespace Sitecore.Modules.WeBlog.Utilities
{
    public static class Search
    {
        /// <summary>
        /// Gets the WeBlog search index
        /// </summary>
        /// <returns>The index as found my the SearchManager</returns>
        public static Index GetSearchIndex()
        {
            return SearchManager.GetIndex(Settings.SearchIndexName);
        }

        /// <summary>
        /// Transforms an input to remove the whitespace and allow tokenising on other characters
        /// </summary>
        /// <param name="value">The string to transform</param>
        /// <returns>The transformed string</returns>
        public static string TransformCSV(string value)
        {
            var collapsed = value.Replace(" ", string.Empty);
            return collapsed.Replace(',', ' ');
        }

        /// <summary>
        /// Performs a search in the WeBlog search index, with a sort
        /// </summary>
        /// <typeparam name="T">The type of the items to be returned from the search</typeparam>
        /// <param name="query">The query to execute</param>
        /// <param name="maximumResults">The maximum number of results</param>
        /// <param name="sortField">The index field to sort on</param>
        /// <returns>An array of search results, or an empty array if there was an issue</returns>
        public static T[] Execute<T>(QueryBase query, int maximumResults, Action<List<T>, Item> func, string sortField, bool reverseSort)
        {
            if (query is CombinedQuery)
            {
                // Add on database
                (query as CombinedQuery).Add(new FieldQuery(Sitecore.Search.BuiltinFields.Database, Sitecore.Context.Database.Name), QueryOccurance.Must);
            }

            // I have to use Action<T> cause the compiler can't work out how to use implicit operators when T is one of the items classes (generated by CIG)
            var items = new List<T>();

            if (maximumResults > 0)
            {
                var index = Utilities.Search.GetSearchIndex();
                if (index != null)
                {
                    using (var searchContext = new SortableIndexSearchContext(index))
                    {
                        SearchHits hits;
                        if (!string.IsNullOrEmpty(sortField))
                        {
                            var sort = new Lucene.Net.Search.Sort(sortField, reverseSort);
                            hits = searchContext.Search(query, sort);
                        }
                        else
                        {
                            hits = searchContext.Search(query);
                        }

                        if (hits != null)
                        {
                            foreach (var result in hits.FetchResults(0, maximumResults))
                            {
                                var item = SearchManager.GetObject(result);
                                if (item != null)
                                    func(items, (Item)item);
                            }
                        }
                    }
                }
            }

            return items.ToArray();
        }

        /// <summary>
        /// Performs a search in the WeBlog search index, without a sort
        /// </summary>
        public static T[] Execute<T>(QueryBase query, int maximumResults, Action<List<T>, Item> func)
        {
            return Execute<T>(query, maximumResults, func, null, false);
        }
    }
}