using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using edu.stanford.nlp.ie.crf;
using VDS.RDF.Query;

namespace ZTI_OKE2018_Task_1
{
	public static class Extensions
	{
		public static string CreateQuery(string word, OntologyClasses oClass, int limit = 1)
		{
			return "Select ?name " +
			       "?" + oClass.ToString().ToLower() +
			       " WHERE {" +
			       "?" + oClass.ToString().ToLower() +
			       " a <http://dbpedia.org/ontology/" + oClass + ">. " +
			       "?" + oClass.ToString().ToLower() +
			       " foaf:name ?name. " +
			       "FILTER (?name=\"" + word + "\"@en).} " +
			       "LIMIT " + limit;
		}

		public static string GetNerText(CRFClassifier crfClassifier, string text)
		{
			return crfClassifier.classifyWithInlineXML(text);
		}

		public static int CountStringOccurrences(string text, string pattern)
		{
			// Loop through all instances of the string 'text'.
			var count = 0;
			var i = 0;
			while ((i = text.IndexOf(pattern, i, StringComparison.Ordinal)) != -1)
			{
				i += pattern.Length;
				count++;
			}

			return count;
		}

		public static void AskDB(List<List<Data.DataProperties>> world)
		{
			var endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"));

			for (var index = 0; index < world.Count; index++)
			{
				var god = world[index];
				foreach (var devil in god)
				{
					var dbpGraph = endpoint.QueryWithResultSet(CreateQuery(devil.Text, (OntologyClasses)index));
					if (dbpGraph.Results.Count > 0) devil.DBpediaREF = dbpGraph.Results.First()[1].ToString(); //referencja do dbpedii
				}
			}
		}

		public static string OFD(string filter)
		{
			using (var ofd = new OpenFileDialog
			{
				InitialDirectory = @"C:\", Title = "Browse file", CheckFileExists = true, CheckPathExists = true, Filter = filter, FilterIndex = 2, RestoreDirectory = true, ReadOnlyChecked = true, ShowReadOnly = true
			})
			{
				if (ofd.ShowDialog() == DialogResult.OK)
					return ofd.FileName;
			}

			return string.Empty;
		}
	}
}