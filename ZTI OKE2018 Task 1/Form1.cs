using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Datasets;
using VDS.RDF.Writing;
using VDS.RDF.Writing.Formatting;

namespace ZTI_OKE2018_Task_1
{
    public partial class Form1 : Form
    {
        class Data
        {
            int startIndex, stopIndex;
            string text;
            public List<Data> Person = new List<Data>();

            public int StartIndex { get => startIndex; set => startIndex = value; }
            public int StopIndex { get => stopIndex; set => stopIndex = value; }
            public string Text { get => text; set => text = value; }

            public void findPersons()
            {
                string pattern = "([A-Z][a-z]+ )+";
                Regex regex = new Regex(pattern);
                MatchCollection matches = regex.Matches(text);
                foreach(Match m in matches)
                {
                    SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"));

                    //Console.WriteLine(m.ToString());
                    string find = m.ToString().Substring(0,m.ToString().Length-1);

                    // Ask DBPedia to describe the first thing it finds which is a Person
                    var query2 = "Select ?name ?person WHERE {?person a <http://dbpedia.org/ontology/Person>. ?person foaf:name ?name. FILTER (?name=\"" + find + "\"@en).} LIMIT 1";

                    //Get the result
                    var dbpGraph = endpoint.QueryWithResultSet(query2);
                    if (dbpGraph.Results.Count > 0)
                    {
                        Data newPersonData = new Data();
                        //newPersonData.Text = m.ToString(); //sam tekst
                        newPersonData.Text = dbpGraph.Results.First()[1].ToString(); //referencja do dbpedii
                        newPersonData.StartIndex = m.Index;
                        newPersonData.StopIndex = m.Index + text.Length;

                        Person.Add(newPersonData);
                    }
                }
            }
        };
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IGraph g = new Graph();

            //Load using a Filename
            new TurtleParser().Load(g, "oke17task1Training.xml.ttl");

            //Testing parsed data
            foreach (var t in g.Triples)
            {
               // Console.WriteLine($"Subject: {t.Subject}" + Environment.NewLine + $"Predicate: {t.Predicate}" + Environment.NewLine + $"Object: {t.Object}" + Environment.NewLine);
            }

            //Get the Query processor
            ISparqlQueryProcessor processor = new LeviathanQueryProcessor(new InMemoryDataset(g));

            //Use the SparqlQueryParser to give us a SparqlQuery object
            //Should get a Graph back from a CONSTRUCT query
            var query = new SparqlQueryParser().ParseFromString("CONSTRUCT { ?s ?p ?o } WHERE {?s ?p ?o}");
            var results = processor.ProcessQuery(query);
            if (!(results is IGraph g1)) return;

            //Print out the Results
            var formatter = new NTriplesFormatter();

            var Triples = g.Triples.Where(q => q.Predicate.ToString().ToLower().Contains("isString".ToLower())).GroupBy(q => q.Object).Select(q => q.First());

            List<Data> dataSet = new List<Data>();


            foreach (var t in Triples)
            {
                //Console.WriteLine(t.ToString(formatter));
                string pattern = "char=([0-9]+),([0-9]+)";
                Regex regex = new Regex(pattern);
                Match match = regex.Match(t.Subject.ToString());
                if (match.Success)
                {
                    Data newData = new Data();
                    newData.StartIndex = 0;
                    newData.StopIndex= Int32.Parse(match.Groups[2].Value);
                    newData.Text = t.Object.ToString().Substring(newData.StartIndex, newData.StopIndex);
                    dataSet.Add(newData);
                }
            }
            foreach(var data in dataSet)
            {
                data.findPersons();
                foreach(var d in data.Person)
                {
                    Console.WriteLine(string.Format("nif:beginIndex:{0},\nnif:endIndex :{1}\nnif:isString:{2}",d.StartIndex,d.StopIndex,d.Text));
                }
            }

            Console.WriteLine("Query took " + query.QueryExecutionTime + " milliseconds\n");

            // ------------------------------------------------------------------------------
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"));

            string find = inputTextBox.Text;

            // Ask DBPedia to describe the first thing it finds which is a Person
            var query2 = "Select ?name ?person WHERE {?person a <http://dbpedia.org/ontology/Person>. ?person foaf:name ?name. FILTER (?name=\"" + find + "\"@en).} LIMIT 1";

            //Get the result
            var dbpGraph = endpoint.QueryWithResultSet(query2);
            if (dbpGraph.Results.Count > 0)
            {
                var result = dbpGraph.Results.First().ToString();
                Console.WriteLine("finded in dbpedia:");
                Console.WriteLine(result);
                //outputTextBox.Text = "finded in dbpedia:\n";
                //outputTextBox.Text += result;
            }
            else
                Console.WriteLine("\ncouldnt find\n");
                //outputTextBox.Text = "couldnt find";

            var query3 = "Select ?name ?place WHERE {?place a <http://dbpedia.org/ontology/Place>. ?place foaf:name ?name. FILTER (?name=\"" + find + "\"@en).} LIMIT 5";

            //Get the result
            var dbpGraph2 = endpoint.QueryWithResultSet(query3);
            if (dbpGraph2.Results.Count > 0)
            {
                var result2 = dbpGraph2.Results.First().ToString();
                Console.WriteLine("finded in dbpedia:");
                Console.WriteLine(result2);
                //outputTextBox.Text += "\nfinded in dbpedia:\n";
                //outputTextBox.Text += result2;
            }
            else
                Console.WriteLine("\ncouldnt find\n");
                //outputTextBox.Text += "\ncouldnt find";
                
            var query4 = "Select ?name ?organisation WHERE {?organisation a<http://dbpedia.org/ontology/Organisation>. ?organisation foaf:name ?name. FILTER (?name=\"" + find + "\"@en).} LIMIT 5";

            //Get the result
            var dbpGraph3 = endpoint.QueryWithResultSet(query4);
            if (dbpGraph3.Results.Count > 0)
            {
                var result3 = dbpGraph3.Results.First().ToString();
                Console.WriteLine("finded in dbpedia:");
                Console.WriteLine(result3);
                //outputTextBox.Text += "\nfinded in dbpedia:\n";
                //outputTextBox.Text += result3;
            }
            else
                Console.WriteLine("\ncouldnt find\n");
                //outputTextBox.Text += "\ncouldnt find";

            RdfXmlWriter rdfxmlwriter = new RdfXmlWriter();
            //rdfxmlwriter.Save(dbpGraph, Console.Out); // view test result details for output.
            //rdfxmlwriter.Save(dbpGraph, "output.rdf");
            // ------------------------------------------------------------------------------

            Console.ReadLine();
        }
    }
}
