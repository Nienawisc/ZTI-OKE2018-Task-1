using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZTI_OKE2018_Task_1
{
	public static class Extensions
	{
		public static string CreateQuery(string word, Data.OntologyClasses oClass, int limit = 1)
		{
			return (
				"Select ?name " +
				"?" + oClass.ToString().ToLower() +
				" WHERE {" +
				"?" + oClass.ToString().ToLower() +
				" a <http://dbpedia.org/ontology/" + oClass + ">. " +
				"?" + oClass.ToString().ToLower() +
				" foaf:name ?name. " +
				"FILTER (?name=\"" + word + "\"@en).} " +
				"LIMIT " + limit
			);
		}
	}
}
