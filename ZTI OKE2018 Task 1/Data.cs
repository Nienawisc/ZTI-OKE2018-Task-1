using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VDS.RDF.Query;

namespace ZTI_OKE2018_Task_1
{
	public class Data
	{
		public enum OntologyClasses
		{
			Person,
			Place,
			Organisation
		}

		public Data(int startIndex, int stopIndex, string text)
		{
			StartIndex = startIndex;
			StopIndex = stopIndex;
			Text = text ?? throw new ArgumentNullException(nameof(text));
		}

		internal int StartIndex { get; }

		internal int StopIndex { get; }

		internal string Text { get; }

		private MatchCollection FindByRegex(string pattern)
		{
			return new Regex(pattern).Matches(Text);
		}

		public IEnumerable<Data> GetListOf(OntologyClasses oClass)
		{
			var matches = FindByRegex("([A-Z][a-z]+ )+");

			var endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"));

			var persons = new List<Data>();

			foreach (Match m in matches)
			{
				var find = m.ToString().Substring(0, m.ToString().Length - 1);

				//Get the result
				var dbpGraph = endpoint.QueryWithResultSet(Extensions.CreateQuery(find, oClass));
				if (dbpGraph.Results.Count <= 0) continue;

				persons.Add(new Data(m.Index, m.Index + find.Length, dbpGraph.Results.First()[1].ToString()));
			}

			return persons;
		}
	}
}