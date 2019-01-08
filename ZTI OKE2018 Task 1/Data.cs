using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using edu.stanford.nlp.ie.crf;
using VDS.RDF.Query;

namespace ZTI_OKE2018_Task_1
{
	public partial class Data
	{
		private Data(int startIndex, int stopIndex, string text)
		{
			StartIndex = startIndex;
			StopIndex = stopIndex;
			Text = text ?? throw new ArgumentNullException(nameof(text));
		}

		public Data(int startIndex, int stopIndex, string text, CRFClassifier crfClassifier) : this(startIndex,
			stopIndex, text)
		{
			CrfClassifier = crfClassifier ?? throw new ArgumentNullException(nameof(crfClassifier));
		}

		private int StartIndex { get; }

		private int StopIndex { get; }

		private string Text { get; }

		private CRFClassifier CrfClassifier { get; }

		private MatchCollection FindByRegex(string pattern)
		{
			return new Regex(pattern).Matches(Text);
		}

		public IEnumerable<Data> GetListOf(OntologyClasses oClass)
		{
			var matches = FindByRegex("([A-Z][a-z]+ )+");

			var endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"));

			var list = new List<Data>();

			foreach (Match m in matches)
			{
				var find = m.ToString().Substring(0, m.ToString().Length - 1);

				//Get the result
				var dbpGraph = endpoint.QueryWithResultSet(Extensions.CreateQuery(find, oClass));
				if (dbpGraph.Results.Count <= 0) continue;

				list.Add(new Data(m.Index, m.Index + find.Length, dbpGraph.Results.First()[1].ToString()));
			}

			return list;
		}

		public IEnumerable<DataProperties> GetOntologyEntries(NerClasses nc)
		{
			var output = new List<DataProperties>();

			var tagOpen = "<" + nc + ">";
			var tagClose = "</" + nc + ">";

			var i = 0;
			var j = 0;
			var k = 0;
			var count = 0;

			var nerText = Extensions.GetNerText(CrfClassifier, Text);

			var occurrences = Extensions.CountStringOccurrences(nerText, tagOpen);

			while (count < occurrences)
			{
				var posA = nerText.IndexOf(tagOpen, i, StringComparison.Ordinal);
				var posB = nerText.IndexOf(tagClose, j, StringComparison.Ordinal);

				var adjustedPosA = posA + tagOpen.Length;

				var nazwa = string.Empty;

				if (posA < 0 || posB < 0) continue;

				i = posA + tagOpen.Length;
				j = posB + tagClose.Length;

				nazwa = nerText.Substring(adjustedPosA, posB - adjustedPosA);
				
				var t = new DataProperties(nazwa, Text.IndexOf(nazwa, k, StringComparison.Ordinal), (Text.IndexOf(nazwa, k, StringComparison.Ordinal) + nazwa.Length - 1));
				output.Add(t);
				
				k = Text.IndexOf(nazwa, k, StringComparison.Ordinal) + nazwa.Length;
				count += 1;
			}

			return output;
		}
	}
}