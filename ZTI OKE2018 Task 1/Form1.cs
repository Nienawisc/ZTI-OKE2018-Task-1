using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Writing;

namespace ZTI_OKE2018_Task_1
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			IGraph g = new Graph();

			//Load using a Filename
			new TurtleParser().Load(g, @"D:\!MEGA\!Studia\Semestr 7\ZTI\oke17task1TrainingSmall.ttl");

			var triples = g.Triples.Where(q => q.Predicate.ToString().ToLower().Contains("isString".ToLower()))
				.GroupBy(q => q.Object).Select(q => q.First());

			#region LINQ

			/*
			 * Orginał LINQ poniżej
			 *
				foreach (var t in Triples)
				{
					var match = new Regex("char=([0-9]+),([0-9]+)").Match(t.Subject.ToString());
					if (!match.Success) continue;

					var startIndex = 0;
					var stopIndex = int.Parse(match.Groups[2].Value);

					var newData = new Data(startIndex, stopIndex, t.Object.ToString().Substring(startIndex, stopIndex));
					dataSet.Add(newData);
				}
			 *
			 */

			#endregion LINQ

			var dataSet = (
				from t
					in triples
				let match = new Regex("char=([0-9]+),([0-9]+)").Match(t.Subject.ToString())
				where match.Success
				let startIndex = 0
				let stopIndex = int.Parse(match.Groups[2].Value)
				select new Data(startIndex, stopIndex, t.Object.ToString().Substring(startIndex, stopIndex))).ToList();

			#region DebugPersons

			foreach (var data in dataSet)
			{
				var Persons = data.GetListOf(Data.OntologyClasses.Person);

				foreach (var d in Persons)
				{
					Console.WriteLine($@"nif:beginIndex:{d.StartIndex}, nif:endIndex :{d.StopIndex}, nif:isString:{d.Text}");
				}
			}

			#endregion DebugPersons

			var endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"));


			var find = inputTextBox.Text;

			foreach (var c in (Data.OntologyClasses[])Enum.GetValues(typeof(Data.OntologyClasses)))
			{
				var query = Extensions.CreateQuery(find, c, 5);
				var graph = endpoint.QueryWithResultSet(query);

				//Get the result
				if (graph.Results.Count > 0)
				{
					var result = graph.Results.First().ToString();

					//Debug
					MessageBox.Show(result, @"Found in dbpedia:");
				}
				else
				{
					//Debug
					MessageBox.Show(query, @"Couldn't find anything matching to query:");
				}

			}

			var rdfxmlwriter = new RdfXmlWriter();

			//rdfxmlwriter.Save(dbpGraph, Console.Out); // view test result details for output.
			//rdfxmlwriter.Save(dbpGraph, "output.rdf");
			// ------------------------------------------------------------------------------
		}
	}
}