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
using edu.stanford.nlp.ie.crf;
using Console = System.Console;

namespace ZTI_OKE2018_Task_1
{
    public partial class Form1 : Form
    {
        class properties
        {
            int startIndex, stopIndex;
            string text;
            string DBpediaREF;
            bool inDBpedia;
            
            public properties()
            {
                inDBpedia = false;
            }

            public void setDBpediaREF(string dbref)
            {
                DBpediaREF = dbref;
                inDBpedia = true;
            }

            public string Text { get => text; set => text = value; }
            public int StopIndex { get => stopIndex; set => stopIndex = value; }
            public int StartIndex { get => startIndex; set => startIndex = value; }
            public bool InDBpedia { get => inDBpedia; set => inDBpedia = value; }
            public string DBpediaREF1 { get => DBpediaREF; set => DBpediaREF = value; }
        }

        class Data
        {
            int startIndex, stopIndex;
            string text;
            string nerText;
            public List<properties> Person = new List<properties>();
            public List<properties> Location = new List<properties>();
            public List<properties> Organization = new List<properties>();

            // Loading 3 class classifier model
            CRFClassifier classifier;

            public int StartIndex { get => startIndex; set => startIndex = value; }
            public int StopIndex { get => stopIndex; set => stopIndex = value; }
            public string Text { get => text; set => text = value; }

            public Data(CRFClassifier c)
            {
                classifier =c;
            }

            public void findNer()
            {
                Console.WriteLine("{0}\n", classifier.classifyWithInlineXML(Text));
                nerText = classifier.classifyWithInlineXML(Text);
            }

            public int CountStringOccurrences(string text, string pattern)
            {
                // Loop through all instances of the string 'text'.
                int count = 0;
                int i = 0;
                while ((i = text.IndexOf(pattern, i)) != -1)
                {
                    i += pattern.Length;
                    count++;
                }
                return count;
            }

            public void findIndex(String text1, String text2)
            {
                int i = 0;
                int j = 0;
                int k = 0;

                int count = 0;

                int occurrences = CountStringOccurrences(nerText, text1);

                while (count<occurrences)
                {
                    int posA = nerText.IndexOf(text1,i);
                    int posB = nerText.IndexOf(text2,j);
                    int adjustedPosA = posA + text1.Length;
                    String nazwa = "";

                    if (posA >= 0 && posB >= 0)
                    {
                        i = posA+text1.Length;
                        j = posB+text2.Length;
                        nazwa = nerText.Substring(adjustedPosA, posB - adjustedPosA);
                        count += 1;
                        properties newprop = new properties();
                        newprop.Text = nazwa;
                        newprop.StartIndex = text.IndexOf(nazwa, k);
                        newprop.StopIndex = (text.IndexOf(nazwa, k) + nazwa.Length - 1);
                        k = text.IndexOf(nazwa, k) + nazwa.Length;

                        if (text1 == "<PERSON>") Person.Add(newprop);
                        if (text1 == "<LOCATION>") Location.Add(newprop);
                        if (text1 == "<ORGANIZATION>") Organization.Add(newprop);
                    }
                }
            }

            public void askdb()
            {
                SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"));

                //Console.WriteLine(m.ToString());
                foreach(var p in Person)
                {
                    string find = p.Text;

                    // Ask DBPedia to describe the first thing it finds which is a Person
                    var query2 = "Select ?name ?person WHERE {?person a <http://dbpedia.org/ontology/Person>. ?person foaf:name ?name. FILTER (?name=\"" + find + "\"@en).} LIMIT 1";

                    //Get the result
                    var dbpGraph = endpoint.QueryWithResultSet(query2);
                    if (dbpGraph.Results.Count > 0)
                    {
                        p.setDBpediaREF(dbpGraph.Results.First()[1].ToString()); //referencja do dbpedii
                    }
                }
                foreach (var p in Location)
                {
                    string find = p.Text;

                    // Ask DBPedia to describe the first thing it finds which is a Person
                    var query2 = "Select ?name ?place WHERE {?place a <http://dbpedia.org/ontology/Place>. ?place foaf:name ?name. FILTER (?name=\"" + find + "\"@en).} LIMIT 5";

                    //Get the result
                    var dbpGraph = endpoint.QueryWithResultSet(query2);
                    if (dbpGraph.Results.Count > 0)
                    {
                        p.setDBpediaREF(dbpGraph.Results.First()[1].ToString()); //referencja do dbpedii
                    }
                }
                foreach (var p in Organization)
                {
                    string find = p.Text;

                    // Ask DBPedia to describe the first thing it finds which is a Person
                    var query2 = "Select ?name ?organisation WHERE {?organisation a<http://dbpedia.org/ontology/Organisation>. ?organisation foaf:name ?name. FILTER (?name=\"" + find + "\"@en).} LIMIT 5";

                    //Get the result
                    var dbpGraph = endpoint.QueryWithResultSet(query2);
                    if (dbpGraph.Results.Count > 0)
                    {
                        p.setDBpediaREF(dbpGraph.Results.First()[1].ToString()); //referencja do dbpedii
                    }
                }
            }
           
           /* public void findPersons()
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
            }*/
        };
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Path to the folder with classifiers models
            String jarRoot = @"C:\Users\G\Downloads\stanford-ner-2016-10-31";
            String classifiersDirecrory;
            classifiersDirecrory = jarRoot + @"\classifiers";
            CRFClassifier classifier= CRFClassifier.getClassifierNoExceptions(classifiersDirecrory + @"\english.all.3class.distsim.crf.ser.gz");

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
                    Data newData = new Data(classifier);
                    newData.StartIndex = 0;
                    newData.StopIndex= Int32.Parse(match.Groups[2].Value);
                    newData.Text = t.Object.ToString().Substring(newData.StartIndex, newData.StopIndex);
                    dataSet.Add(newData);
                }
            }
             foreach(var data in dataSet)
             {
                 data.findNer();
                 data.findIndex("<PERSON>","</PERSON>");
                 data.findIndex("<LOCATION>", "</LOCATION>");
                 data.findIndex("<ORGANIZATION>", "</ORGANIZATION>");
                 data.askdb();
                Console.WriteLine("Person:");
                foreach (var d in data.Person)
                 {
                     Console.WriteLine(string.Format("nif:beginIndex:{0},\nnif:endIndex :{1}\nnif:isString:{2}",d.StartIndex,d.StopIndex,d.Text));
                     Console.WriteLine(string.Format("nif:reference:{0}", d.InDBpedia?d.DBpediaREF1:"No in DBpedia" ));
                 }
                Console.WriteLine("Location:");
                foreach (var d in data.Location)
                {
                    Console.WriteLine(string.Format("nif:beginIndex:{0},\nnif:endIndex :{1}\nnif:isString:{2}", d.StartIndex, d.StopIndex, d.Text));
                    Console.WriteLine(string.Format("nif:reference:{0}", d.InDBpedia ? d.DBpediaREF1 : "No in DBpedia"));
                }
                Console.WriteLine("Organization:");
                foreach (var d in data.Organization)
                {
                    Console.WriteLine(string.Format("nif:beginIndex:{0},\nnif:endIndex :{1}\nnif:isString:{2}", d.StartIndex, d.StopIndex, d.Text));
                    Console.WriteLine(string.Format("nif:reference:{0}", d.InDBpedia ? d.DBpediaREF1 : "No in DBpedia"));
                }
            }

            Console.WriteLine("Query took " + query.QueryExecutionTime + " milliseconds\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Path to the folder with classifiers models
            String jarRoot = @"C:\Users\G\Downloads\stanford-ner-2016-10-31";
            String classifiersDirecrory;
            classifiersDirecrory = jarRoot + @"\classifiers";
            CRFClassifier classifier = CRFClassifier.getClassifierNoExceptions(classifiersDirecrory + @"\english.all.3class.distsim.crf.ser.gz");
            Data inputData = new Data(classifier);

            inputData.Text = inputTextBox.Text;
            inputData.StartIndex = 0;
            inputData.StopIndex = inputTextBox.Text.Length-1;

            inputData.findNer();
            inputData.findIndex("<PERSON>", "</PERSON>");
            inputData.findIndex("<LOCATION>", "</LOCATION>");
            inputData.findIndex("<ORGANIZATION>", "</ORGANIZATION>");
            inputData.askdb();

            outputTextBox.Text="Person:\n";
            foreach (var d in inputData.Person)
            {
                outputTextBox.Text+=string.Format("nif:beginIndex:{0},\nnif:endIndex :{1}\nnif:isString:{2}\n", d.StartIndex, d.StopIndex, d.Text);
                outputTextBox.Text+=(string.Format("nif:reference:{0}\n", d.InDBpedia ? d.DBpediaREF1 : "No in DBpedia"));
            }
            outputTextBox.Text += "Location:\n";
            foreach (var d in inputData.Location)
            {
                outputTextBox.Text += string.Format("nif:beginIndex:{0},\nnif:endIndex :{1}\nnif:isString:{2}\n", d.StartIndex, d.StopIndex, d.Text);
                outputTextBox.Text += (string.Format("nif:reference:{0}\n", d.InDBpedia ? d.DBpediaREF1 : "No in DBpedia"));
            }
            outputTextBox.Text += "Organization:\n";
            foreach (var d in inputData.Organization)
            {
                outputTextBox.Text += string.Format("nif:beginIndex:{0},\nnif:endIndex :{1}\nnif:isString:{2}\n", d.StartIndex, d.StopIndex, d.Text);
                outputTextBox.Text += (string.Format("nif:reference:{0}\n", d.InDBpedia ? d.DBpediaREF1 : "No in DBpedia"));
            }
        }
    }
}
