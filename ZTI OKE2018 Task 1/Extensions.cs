using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using edu.stanford.nlp.ie.crf;
using VDS.RDF;
using VDS.RDF.Query;

namespace ZTI_OKE2018_Task_1
{
	/// <summary>
	/// Class full of static helpful methods
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Creates the correct query in SparQL
		/// </summary>
		/// <param name="word">What we ask in the query</param>
		/// <param name="oClass"><see cref="OntologyClasses"/> class of <see cref="word"/></param>
		/// <param name="limit">Limit of responds</param>
		/// <returns></returns>
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

		/// <summary>
		/// Get NER formatted and classified text
		/// </summary>
		/// <param name="crfClassifier">NER Classifier</param>
		/// <param name="text">Text to classify</param>
		/// <returns></returns>
		public static string GetNerText(CRFClassifier crfClassifier, string text)
		{
			return crfClassifier.classifyWithInlineXML(text);
		}

		/// <summary>
		/// Counts the occurrence of a <see cref="pattern"/> in the entire <see cref="text"/>
		/// </summary>
		/// <param name="text">The <see cref="string"/> what we search</param>
		/// <param name="pattern">What we are looking for</param>
		/// <returns>The number of occurrences of the <see cref="pattern"/></returns>
		public static int CountStringOccurrences(string text, string pattern)
		{
			int count = 0, i = 0;

			while ((i = text.IndexOf(pattern, i, StringComparison.Ordinal)) != -1)
			{
				i += pattern.Length;
				count++;
			}
			return count;
		}

		/// <summary>
		/// Asking DBPedia for reference and set it in <see cref="OutputData"/> entry.
		/// </summary>
		/// <param name="SetsOfOutputData"></param>
		public static void AskDB(IEnumerable<IEnumerable<OutputData>> SetsOfOutputData)
		{
			var endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"));

			foreach (var nerClass in SetsOfOutputData)
			{
				foreach (var outputData in nerClass)
				{
					var dbpGraph = endpoint.QueryWithResultSet(CreateQuery(outputData.AnchorOf, GetOntologyClass(outputData.NerClass)));

					//Sets reference to DBPedia
					if (dbpGraph.Results.Count > 0) outputData.TaIdentRef = dbpGraph.Results.First()[1].ToString();
				}
			}
		}

		/// <summary>
		/// Handler of <see cref="OpenFileDialog"/>
		/// </summary>
		/// <param name="filter"></param>
		/// <returns>Path to chosen file</returns>
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

		/// <summary>
		/// Creates .rdf formatted output file.
		/// </summary>
		/// <param name="inputFilePath">Request data filepath</param>
		/// <param name="list">List of <see cref="OutputData"/> divided into <see cref="OntologyClasses"/></param>
		/// <returns></returns>
		public static string CreateOutput(string inputFilePath, IEnumerable<IEnumerable<OutputData>> list)
		{
			var inputText = File.ReadAllText(inputFilePath) + Environment.NewLine;

			var response = list.SelectMany(god => god).Aggregate(string.Empty, (current, devil) => current + devil.ToString());

			return inputText + response;
		}

		/// <summary>
		/// Converts <see cref="NerClasses"/> class to <see cref="OntologyClasses"/> class.
		/// </summary>
		/// <param name="nc"><see cref="NerClasses"/> class</param>
		/// <returns><see cref="OntologyClasses"/> class</returns>
		private static OntologyClasses GetOntologyClass(NerClasses nc)
		{
			 return GetOntologyClass((int) nc);
		}

		/// <summary>
		/// Auxiliary method that converts <see cref="int"/> to <see cref="OntologyClasses"/> class.
		/// </summary>
		/// <param name="nc"></param>
		/// <returns></returns>
		private static OntologyClasses GetOntologyClass(int nc)
		{
			switch (nc)
			{
				case 0:
					return OntologyClasses.Person;
				case 1:
					return OntologyClasses.Place;
				case 2:
					return OntologyClasses.Organisation;
				default:
					throw new ArgumentOutOfRangeException(nameof(nc), nc, null);
			}
		}
	}
}