using System;
using System.Linq;

namespace ZTI_OKE2018_Task_1
{
	public partial class Data
	{
		public class DataProperties
		{
			public DataProperties(string referenceContext, string anchorOf, NerClasses nerClass, int beginIndex, int endIndex, bool inDBpedia = false)
			{
				ReferenceContext = referenceContext ?? throw new ArgumentNullException(nameof(referenceContext));
				AnchorOf = anchorOf ?? throw new ArgumentNullException(nameof(anchorOf));
				BeginIndex = beginIndex;
				EndIndex = endIndex;
				InDBpedia = inDBpedia;
				NerClass = nerClass;
			}

			public string AnchorOf { get; }
			public NerClasses NerClass { get; }

			private int BeginIndex { get; }

			private int EndIndex { get; }

			private bool InDBpedia { get; set; }

			private string ReferenceContext { get; }

			public string TaIdentRef
			{
				get => _taIdentRef;
				set
				{
					_taIdentRef = value;
					InDBpedia = true;
				}
			}

			private string _taIdentRef;

			public override string ToString()
			{
				var output = string.Empty;
				var noCharAddress = ReferenceContext.Replace(">", string.Empty).Split('#')[0];

				output += $"<{noCharAddress}#char={BeginIndex},{EndIndex}>" + Environment.NewLine;
				output += Insert("\t") + "a" + Insert("\t", 6) + "nif:RFC5147String, nif:String;" + Environment.NewLine;
				output += Insert("\t") + "nif:anchorOf" + Insert("\t", 3) + $"\"{AnchorOf}\"@en;" + Environment.NewLine;
				output += Insert("\t") + "nif:beginIndex" + Insert("\t", 3) + $"\"{BeginIndex}\"^^xsd:nonNegativeInteger;" + Environment.NewLine;
				output += Insert("\t") + "nif:endIndex" + Insert("\t", 3) + $"\"{EndIndex}\"^^xsd:nonNegativeInteger; " + Environment.NewLine;
				output += Insert("\t") + "nif:referenceContext" + Insert("\t") + $"<{ReferenceContext}>" + Environment.NewLine;
				output += Insert("\t") + "itsrdf:taIdentRef" + Insert("\t", 2) + (InDBpedia ? $"dbpedia:{TaIdentRef.Split('/').Last()}" : $"<http://aksw.org/notInWiki/{AnchorOf.Replace(' ', '_')}>") + "." + Environment.NewLine;
				output += Environment.NewLine;
				return output;
			}

			private static string Insert(string s, int n = 1)
			{
				var output = string.Empty;

				for (var i = 0; i < n; i++) output += s;

				return output;
			}
		}
	}
}