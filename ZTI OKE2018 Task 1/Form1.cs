using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using edu.stanford.nlp.ie.crf;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Writing;
using ZTI_OKE2018_Task_1.Properties;

namespace ZTI_OKE2018_Task_1
{
	public partial class SparQLButton : Form
	{
		public SparQLButton()
		{
			InitializeComponent();
			Height = 140;
			SparQL.Image = Resources.SparQL1;
		}

		private CRFClassifier Classifier { get; set; }
		private string InputFilePath { get; set; }
		private bool IsSparQL { get; set; }
		private bool IsDebug { get; set; } = false;

		private void button1_Click(object sender, EventArgs e)
		{
			if (IsSparQL)
			{
				var endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"));

				foreach (var c in (OntologyClasses[]) Enum.GetValues(typeof(OntologyClasses)))
				{
					var query = Extensions.CreateQuery(inputTextBox.Text, c, 5);
					var graph = endpoint.QueryWithResultSet(query);

					if (!IsDebug) continue;

					if (graph.Results.Count > 0)
					{
						outputTextBox.Text += @"Found in dbpedia: " + graph.Results.First() + Environment.NewLine + Environment.NewLine;
					}
					else
					{
						outputTextBox.Text += @"Couldn't find anything matching to query: " + query + Environment.NewLine + Environment.NewLine;
					}

					outputTextBox.Text += Environment.NewLine;
				}

				return;
			}

			IGraph g = new Graph();

			//Load using a Filename
			new TurtleParser().Load(g, InputFilePath);

			var triples = g.Triples.Where(q => q.Predicate.ToString().ToLower().Contains("isString".ToLower())).GroupBy(q => q.Object).Select(q => q.First());

			#region LINQ

			/* Orginal from LINQ below
			foreach (var t in triples)
			{
				var match = new Regex("char=([0-9]+),([0-9]+)").Match(t.Subject.ToString());
				if (!match.Success) continue;

				var startIndex = 0;
				var stopIndex = int.Parse(match.Groups[2].Value);

				var newData = new Data(startIndex, stopIndex, t.Object.ToString().Substring(startIndex, stopIndex), Classifier);

				dataSet.Add(newData);
			}
			 */

			#endregion

			var dataSet = (
				from t
					in triples
				let match = new Regex("char=([0-9]+),([0-9]+)").Match(t.Subject.ToString())
				where match.Success
				let startIndex = 0
				let stopIndex = int.Parse(match.Groups[2].Value)
				select new Data(startIndex, stopIndex, t.Object.ToString().Substring(startIndex, stopIndex), Classifier)
			).ToList();

			var persons = new List<Data.DataProperties>();
			var locations = new List<Data.DataProperties>();
			var organisations = new List<Data.DataProperties>();

			foreach (var data in dataSet)
			{
				persons = persons.Concat(data.GetOntologyEntries(NerClasses.PERSON).ToList()).ToList();
				locations = locations.Concat(data.GetOntologyEntries(NerClasses.LOCATION).ToList()).ToList();
				organisations = organisations.Concat(data.GetOntologyEntries(NerClasses.ORGANIZATION).ToList()).ToList();
			}

			var output = new List<List<Data.DataProperties>> {persons, locations, organisations};

			Extensions.AskDB(output);


			#region Debug

			for (var index = 0; index < output.Count; index++)
			{
				switch (index)
				{
					case 0:
						outputTextBox.Text += "Person:" + Environment.NewLine;
						break;
					case 1:
						outputTextBox.Text += "Location:" + Environment.NewLine;
						break;
					case 2:
						outputTextBox.Text += "Organization:" + Environment.NewLine;
						break;
					default:
						outputTextBox.Text += Environment.NewLine;
						break;
				}

				var god = output[index];
				foreach (var devil in god)
				{
					outputTextBox.Text += $"nif:beginIndex: {devil.StartIndex}" + Environment.NewLine;
					outputTextBox.Text += $"nif:endIndex: {devil.StopIndex}" + Environment.NewLine;
					outputTextBox.Text += $"nif:isString: {devil.Text}" + Environment.NewLine;
					outputTextBox.Text += $"nif:reference:{(devil.InDBpedia ? devil.DBpediaREF : "No in DBpedia")}" + Environment.NewLine;
					outputTextBox.Text += Environment.NewLine;
				}
			}

			#endregion Debug

			#region RdfXmlWrite

			new RdfXmlWriter().Save(g, "output.rdf");

			#endregion RdfXmlWrite

			MessageBox.Show("OK\r\n\r\nWyniki możesz podejrzeć naciskając guzik 'SparQL', następnie 'Debug'.","Zakończono działanie.");

		}

		private void JarFileLocation_Click(object sender, EventArgs e)
		{
			var c = Extensions.OFD("gz files (*.gz)|*.gz|All files (*.*)|*.*");
			if(c == string.Empty) return;
			Classifier = CRFClassifier.getClassifierNoExceptions(c);
			Stanford.Text = c;
		}

		private void FileLocation_Click(object sender, EventArgs e)
		{
			var i = Extensions.OFD("ttl files (*.ttl)|*.ttl|All files (*.*)|*.*");
			if (i == string.Empty) return;
			InputFilePath = i;
			InputFile.Text = i;
		}

		private void SparQL_Click(object sender, EventArgs e)
		{
			IsSparQL = !IsSparQL;

			if (IsSparQL)
			{
				SparQL.Image = Resources.SparQL2;
				Height = 300;
				DebugButton.Visible = true;
				return;
			}

			SparQL.Image = Resources.SparQL1;
			Height = 140;
			DebugButton.Visible = false;
		}

		private void DebugButton_Click(object sender, EventArgs e)
		{
			IsDebug = !IsDebug;

			if (IsDebug)
			{
				DebugButton.Text = "Debug ↑↑↑";
				Height = 600;
				return;
			}

			DebugButton.Text = "Debug ↓↓↓";
			Height = 300;
		}
	}
}