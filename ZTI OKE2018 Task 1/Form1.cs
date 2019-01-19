using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using edu.stanford.nlp.ie.crf;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using ZTI_OKE2018_Task_1.Properties;

namespace ZTI_OKE2018_Task_1
{
	/// <summary>
	///     Main class of program
	/// </summary>
	public partial class SparQLButton : Form
	{
		/// <summary>
		///     Construktor - Initialize Componets
		/// </summary>
		public SparQLButton()
		{
			InitializeComponent();
			Height = 140;
			SparQL.Image = Resources.SparQL1;
		}

		/// <summary>
		///     Object of NER Classifier
		/// </summary>
		private CRFClassifier Classifier { get; set; }

		/// <summary>
		///     FilePath to file that with request data
		/// </summary>
		private string InputFilePath { get; set; }

		/// <summary>
		///     Field responsible for the action of the program button. Default: false
		/// </summary>
		private bool IsSparQL { get; set; }

		/// <summary>
		///     Field responsible for the action of the program button. Default: false
		/// </summary>
		private bool IsDebug { get; set; }

		/// <summary>
		///     Main action for program.
		///     If <see cref="IsSparQL" /> is true then use textbox.
		/// </summary>
		/// <param name="sender">Unused</param>
		/// <param name="e">Unused</param>
		private void button1_Click(object sender, EventArgs e)
		{
			if (IsSparQL)
			{
				var endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"));

				//Checking for each Ontology Class that input-text from textbox is in DBPedia
				foreach (var c in (OntologyClasses[]) Enum.GetValues(typeof(OntologyClasses)))
				{
					//Creating valid querry to ask dbpedia via sparql
					var query = Extensions.CreateQuery(inputTextBox.Text, c, 5);

					//Send request to DBPedia and get respond
					var graph = endpoint.QueryWithResultSet(query);

					if (!IsDebug) continue;

					//Writing results
					if (graph.Results.Count > 0)
						outputTextBox.Text += @"Found in dbpedia: " + graph.Results.First() + Environment.NewLine + Environment.NewLine;
					else
						outputTextBox.Text += @"Couldn't find anything matching to query: " + query + Environment.NewLine + Environment.NewLine;

					outputTextBox.Text += Environment.NewLine;
				}

				return;
			}

			IGraph g = new Graph();

			//Load using a Filename
			new TurtleParser().Load(g, InputFilePath);

			//Create triples based on string text elements
			var triples = g.Triples.Where(q => q.Predicate.ToString().ToLower().Contains("isString".ToLower())).GroupBy(q => q.Object).Select(q => q.First());

			#region Original from LINQ below

			/*
			var dataSet = new List<InputData>();
			foreach (var t in triples)
			{
				var match = new Regex("char=([0-9]+),([0-9]+)").Match(t.Subject.ToString());
				if (!match.Success) continue;

				var startIndex = 0;
				var stopIndex = int.Parse(match.Groups[2].Value);

				var newData = new InputData(startIndex, stopIndex, t.Object.ToString().Substring(startIndex, stopIndex), t.Subject.ToString(), Classifier);

				dataSet.Add(newData);
			}
			// */

			#endregion Original from LINQ below

			//Creating and parsing proper input data
			var dataSet = (
				from t
					in triples
				let match = new Regex("char=([0-9]+),([0-9]+)").Match(t.Subject.ToString())
				where match.Success
				let startIndex = 0
				let stopIndex = int.Parse(match.Groups[2].Value)
				select new InputData(t.Object.ToString().Substring(startIndex, stopIndex), t.Subject.ToString(), Classifier)
			).ToList();

			//Initialize components of output data
			var persons = new List<OutputData>();
			var locations = new List<OutputData>();
			var organizations = new List<OutputData>();

			//Get information from NER which strings belong to a given ontology
			foreach (var data in dataSet)
			{
				persons = persons.Concat(data.GetOntologyEntries(NerClasses.PERSON).ToList()).ToList();
				locations = locations.Concat(data.GetOntologyEntries(NerClasses.LOCATION).ToList()).ToList();
				organizations = organizations.Concat(data.GetOntologyEntries(NerClasses.ORGANIZATION).ToList()).ToList();
			}

			//Initializing output data
			var output = new List<IEnumerable<OutputData>> {persons.Distinct(), locations.Distinct(), organizations.Distinct()};

			//Checking if entries have references in DBPedia
			Extensions.AskDB(output);

			//Writing output data in another textbox

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
				foreach (var devil in god) outputTextBox.Text += devil.ToString();
			}

			#endregion Debug

			//Saving output data to .rdf
			#region RdfXmlWrite

			var rdf = Extensions.CreateOutput(InputFilePath, output);

			File.WriteAllText("output.rdf", rdf);

			#endregion RdfXmlWrite

			MessageBox.Show("OK\r\n\r\nWyniki możesz podejrzeć naciskając guzik 'SparQL', następnie 'Debug'.", "Zakończono działanie.");
		}

		/// <summary>
		///     NER Classifier file choosing handler
		/// </summary>
		/// <param name="sender">Unused</param>
		/// <param name="e">Unused</param>
		private void JarFileLocation_Click(object sender, EventArgs e)
		{
			var c = Extensions.OFD("gz files (*.gz)|*.gz|All files (*.*)|*.*");
			if (c == string.Empty) return;
			Classifier = CRFClassifier.getClassifierNoExceptions(c);
			Stanford.Text = c;
		}

		/// <summary>
		///     Request data file choosing handler
		/// </summary>
		/// <param name="sender">Unused</param>
		/// <param name="e">Unused</param>
		private void FileLocation_Click(object sender, EventArgs e)
		{
			var i = Extensions.OFD("ttl files (*.ttl)|*.ttl|All files (*.*)|*.*");
			if (i == string.Empty) return;
			InputFilePath = i;
			InputFile.Text = i;
		}

		/// <summary>
		///     SparQL Button handler
		/// </summary>
		/// <param name="sender">Unused</param>
		/// <param name="e">Unused</param>
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

		/// <summary>
		///     Debug Textbox handler
		/// </summary>
		/// <param name="sender">Unused</param>
		/// <param name="e">Unused</param>
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