using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
					var dbpGraph = endpoint.QueryWithResultSet(CreateQuery(devil.Text, (OntologyClasses) index));
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

		public static string CreateOutput(string inputFilePath, List<List<Data.DataProperties>> list)
		{
			var response = string.Empty;

			var inputText = File.ReadAllText(inputFilePath);

			var address = inputText.Split('\n').First(s => (s.Contains("http://") || s.Contains("https://")) && s.Contains("#char="));

			var nocharAddress = address.Remove(0, 1).Replace(">", string.Empty).Split('#')[0];

			//response += inputText + Environment.NewLine;

			foreach (var god in list)
			{
				foreach (var devil in god)
				{
					response += $"<{nocharAddress}#char={devil.StartIndex},{devil.StopIndex}" + Environment.NewLine;
					response += Insert("\t") + "a" + Insert("\t", 5) + "nif:RFC5147String, nif:String;" + Environment.NewLine;
					response += Insert("\t") + "nif:anchorOf" + Insert("\t", 2) + $"\"{devil.Text}\"@en;" + Environment.NewLine;
					response += Insert("\t") + "nif:beginIndex" + Insert("\t", 2) + $"\"{devil.StartIndex}\"^^xsd:nonNegativeInteger;" + Environment.NewLine;
					response += Insert("\t") + "nif:endIndex" + Insert("\t", 2) + $"\"{devil.StopIndex}\"^^xsd:nonNegativeInteger; " + Environment.NewLine;
					response += Insert("\t") + "nif:referenceContext" + Insert("\t") + address + Environment.NewLine;
					response += Insert("\t") + "itsrdf:taIdentRef" + Insert("\t") + (devil.InDBpedia ? $"dbpedia:{devil.DBpediaREF.Split('/').Last()}" : $"<http://aksw.org/notInWiki/{devil.Text.Replace(' ', '_')}>") + "." + Environment.NewLine;
					response += Environment.NewLine;
				}
			}

			return response;
		}

		private static string Insert(string s, int n = 1)
		{
			var output = string.Empty;

			for (var i = 0; i < n; i++) output += s;

			return output;
		}
	}
}