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

namespace ZTI_OKE2018_Task_1
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private CRFClassifier Classifier { get; set; }

		private void button1_Click(object sender, EventArgs e)
		{
			IGraph g = new Graph();

			//Load using a Filename
			new TurtleParser().Load(g, "oke17task1Training.xml.ttl");

			var triples = g.Triples.Where(q => q.Predicate.ToString().ToLower().Contains("isString".ToLower())).GroupBy(q => q.Object).Select(q => q.First());


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

			var dataSet = (
				from t
					in triples
				let match = new Regex("char=([0-9]+),([0-9]+)").Match(t.Subject.ToString())
				where match.Success
				let startIndex = 0
				let stopIndex = int.Parse(match.Groups[2].Value)
				select new Data(startIndex, stopIndex, t.Object.ToString().Substring(startIndex, stopIndex), Classifier)
			).ToList();


			foreach (var data in dataSet)
			{
				foreach (var oc in (OntologyClasses[]) Enum.GetValues(typeof(OntologyClasses)))
				{
					data.findIndex(oc);
					foreach (var d in data.Person)
						Console.WriteLine(@"nif:beginIndex:{0},nif:endIndex :{1}nif:isString:{2}", d.StartIndex, d.StopIndex, d.Text);
				}

			}

			#region SparQL

			var endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"));

			var find = inputTextBox.Text;

			foreach (var c in (OntologyClasses[]) Enum.GetValues(typeof(OntologyClasses)))
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

			#endregion SparQL

			#region RdfXmlWrite

			new RdfXmlWriter().Save(g, "output.rdf");

			#endregion RdfXmlWrite

			Console.ReadLine();
		}

		private void GetJarLocation(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case MouseButtons.Right:
				{
					using (var ofd = new OpenFileDialog
					{
						InitialDirectory = @"C:\",
						Title = "Browse stanford file",

						CheckFileExists = true,
						CheckPathExists = true,

						DefaultExt = "gz",
						Filter = "gz files (*.zg)|*.gz|All files (*.*)|*.*",
						FilterIndex = 2,
						RestoreDirectory = true,

						ReadOnlyChecked = true,
						ShowReadOnly = true
					})
					{
						if (ofd.ShowDialog() == DialogResult.OK)
							Classifier = CRFClassifier.getClassifierNoExceptions(ofd.FileName);
					}
				}
					break;

				case MouseButtons.Left:
					break;

				case MouseButtons.None:
					break;

				case MouseButtons.Middle:
					break;

				case MouseButtons.XButton1:
					break;

				case MouseButtons.XButton2:
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}


	}
}